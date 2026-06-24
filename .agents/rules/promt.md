---
trigger: always_on
---

## 👤 khanhphamvn222 — Backend Models & Services

---

### 📂 Files phụ trách (7 files)

| File | Là gì |
|------|-------|
| `Models/Corpse.cs` | Khuôn dữ liệu thi thể |
| `Models/StorageSlot.cs` | Khuôn dữ liệu ngăn lưu trữ |
| `Models/Shift.cs` | Khuôn dữ liệu ca trực |
| `Models/DashboardStats.cs` | Khuôn dữ liệu thống kê |
| `Services/AuthService.cs` | Xử lý đăng nhập / đăng xuất |
| `Services/ApiService.cs` | Lấy dữ liệu từ "API" |
| `Services/TemperatureSimulatorService.cs` | Giả lập nhiệt độ thay đổi liên tục |

---

### 🧠 Hiểu đơn giản — Models là gì?

Model giống như **mẫu giấy tờ** — định nghĩa thông tin cần có:

```csharp
// Corpse.cs — "Phiếu hồ sơ thi thể"
class Corpse {
    string HoTen;        // Họ tên
    DateTime NgayNhap;   // Ngày nhập kho
    string TrangThai;    // Đang lưu / Đã xuất / Quá hạn
    int NganSo;          // Đang ở ngăn số mấy
}
```

Mọi trang của **newiexk-cyber (FE2)** muốn hiển thị danh sách thi thể → đều phải dùng `Corpse.cs` của khanhphamvn222.

---

### ⚙️ Services là gì?

Service giống như **nhân viên văn phòng** — làm các việc lặp đi lặp lại:

| Service | Làm việc gì |
|---------|------------|
| `AuthService.cs` | Kiểm tra Access Key, lưu trạng thái đăng nhập vào localStorage |
| `ApiService.cs` | Tạo dữ liệu giả (mock) cho thi thể, ca trực, ngăn lưu trữ |
| `TemperatureSimulatorService.cs` | Cứ mỗi 3 giây tự động thay đổi số nhiệt độ → trang Temperature hiển thị real-time |

---

### 🔗 Liên quan đến phần khác thế nào?

```
khanhphamvn222 làm nền tảng dữ liệu
            ↓
┌───────────────────────────────────┐
│  Models/ → FE2 dùng để hiển thị  │
│  AuthService → FE1 (Login) dùng  │
│  ApiService → FE2 (Dashboard,    │
│               Corpses...) dùng   │
│  TempSimulator → FE2 (Temp       │
│                  Monitor) dùng   │
└───────────────────────────────────┘
```

> Nếu **khanhphamvn222 sửa tên property** trong Model → FE1, FE2 sẽ bị lỗi ngay. Vì vậy phải **báo nhóm trước** khi sửa Model.

---

### 🎯 Tóm lại

| | Mô tả |
|--|-------|
| **Vai trò** | Backend — xây dựng "bộ não" dữ liệu |
| **Người dùng thấy không?** | ❌ Không — chạy ngầm phía sau |
| **Ai phụ thuộc vào phần này?** | Simpson (Login), newiexk-cyber (toàn bộ Admin) |
| **Quan trọng không?** | ✅ Rất quan trọng — không có Models thì FE1, FE2 không chạy được |

Khanhpham là **nền móng** của toàn bộ hệ thống — làm ít file nhất nhưng ảnh hưởng nhiều nhất!

Bây giờ khi đóng góp code khi thấy issue hoặc có ý tưởng mới thì sử dụng 
git checkout main
git pull origin main
trước khi mở code
# 1. Cập nhật main mới nhất
git checkout main
git pull origin main

# 2. Tạo branch mới từ main (đặt tên theo tính năng)
git checkout -b feat/ten-tinh-nang

# 3. Code → Lưu file

# 4. Commit (commit nhỏ, rõ ràng)
git add TenFile.razor
git commit -m "Mô tả ngắn gọn bằng tiếng Việt"

# 5. Push lên GitHub
git push origin feat/ten-tinh-nang

# 6. Vào GitHub tạo Pull Request → Báo BDTG merge
Đéo được làm những thứ sau
git push origin main
Gộp tất cả vào 1 commit
Code trên branch cũ mà không pull
git add .
Nên làm 
Luôn push lên branch riêng
Commit từng phần nhỏ
Pull main trước khi tạo branch mới
git add TenFile.razor
Tuyệt đối không đụng vào file của người khác!
Simpson-31ev3n	Pages/Public/, Pages/Auth/, Pages/NotFound.razor
newiexk-cyber	Pages/Admin/
khanhphamvn222	Models/, Services/
BDTG	Components/, wwwroot/, Layout/, App.razor, Program.cs
feat/ten-tinh-nang     → Tính năng mới
fix/ten-loi            → Sửa lỗi  
update/ten-phan        → Cập nhật nội dung
Từ giờ mỗi khi ai muốn sửa gì thì:

Pull main
Tạo branch mới
Code → Commit → Push
Báo bạn merge

---

### 💡 Lưu ý dự án & Lịch sử sửa lỗi (Ghi nhớ cho khanhphamvn222)
- **Cấu hình Database khi push code**:
  - **Local Development**: Sử dụng SQL Server trong `MorgueManager.API/appsettings.json` (chứa chuỗi kết nối `Server=(localdb)...`).
  - **Khi push cho Thái**: Trước khi push, đổi `DefaultConnection` trong `MorgueManager.API/appsettings.json` thành PostgreSQL (Supabase) (`Host=aws-0-ap-northeast-1.pooler...`). Ngay sau khi push xong, đổi trả lại local thành SQL Server.
- **Xử lý Timeout kết nối Supabase Client (Blazor WebAssembly)**:
  - Khi thực hiện các lệnh insert/get trên client qua `Supabase.Client` (như trong `ContactService`), luôn bọc trong cơ chế **Timeout fail-fast 2 giây** bằng `Task.WhenAny(supabaseTask, Task.Delay(2000))`. 
  - Điều này giúp UI không bị treo đơ do mạng chập chờn hoặc RLS chặn. Nếu hết 2 giây chưa xong, hệ thống sẽ tự động bỏ qua để lưu trữ dự phòng trực tiếp vào LocalStorage (`local_contact_requests`), đảm bảo an toàn dữ liệu và hiển thị ngay lập tức.