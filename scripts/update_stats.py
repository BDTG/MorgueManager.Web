import subprocess
import re

def get_git_stats():
    # Run git shortlog
    result = subprocess.run(['git', 'shortlog', '-sn', '--all'], capture_output=True, text=True)
    if result.returncode != 0:
        print("Error running git shortlog")
        return None
    
    lines = result.stdout.strip().split('\n')
    
    stats = {
        'BDTG (Leader)': 0,
        'newiexk-cyber': 0,
        'khanhphamvn222': 0,
        'Simpson-31ev3n': 0
    }
    
    for line in lines:
        line = line.strip()
        if not line: continue
        
        # Output format: "    37  MorgueManager Dev"
        match = re.match(r'^(\d+)\s+(.+)$', line)
        if match:
            count = int(match.group(1))
            name = match.group(2).strip()
            
            # Map aliases to the 4 main team members
            if name.lower() in ['bdtg', 'morguemanager dev', 'normalprojectmain']:
                stats['BDTG (Leader)'] += count
            elif name.lower() == 'newiexk-cyber':
                stats['newiexk-cyber'] += count
            elif name.lower() == 'khanhphamvn222':
                stats['khanhphamvn222'] += count
            elif name.lower() == 'simpson-31ev3n':
                stats['Simpson-31ev3n'] += count
            else:
                # If there are other unknown committers, we can ignore or add to others
                pass
                
    return stats

def format_table(stats):
    total = sum(stats.values())
    if total == 0:
        total = 1 # prevent div by zero
        
    table = "| Thành viên | Số lượng Commits | Tỷ lệ (%) | Trạng thái |\n"
    table += "|---|---|---|---|\n"
    
    # Sort by count descending
    sorted_stats = sorted(stats.items(), key=lambda item: item[1], reverse=True)
    
    for name, count in sorted_stats:
        pct = (count / total) * 100
        
        status = "🔴 Cần cố gắng hơn"
        if pct >= 20:
            status = "🟢 Tốt"
        if name == 'BDTG (Leader)':
            status = "🟢 Vượt chỉ tiêu"
            
        table += f"| **{name}** | {count} | ~{pct:.1f}% | {status} |\n"
        
    table += f"\n*(Tổng số commits dự án: {total})*\n"
    return table

def update_readme(table_content):
    with open('README.md', 'r', encoding='utf-8') as f:
        content = f.read()
        
    pattern = r'<!-- START_STATS -->.*?<!-- END_STATS -->'
    replacement = f"<!-- START_STATS -->\n{table_content}<!-- END_STATS -->"
    
    new_content = re.sub(pattern, replacement, content, flags=re.DOTALL)
    
    with open('README.md', 'w', encoding='utf-8') as f:
        f.write(new_content)

if __name__ == "__main__":
    stats = get_git_stats()
    if stats:
        table = format_table(stats)
        update_readme(table)
        print("Updated README.md with dynamic stats.")
