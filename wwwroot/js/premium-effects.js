// Khởi tạo các hiệu ứng mượt mà
window.initPremiumEffects = () => {
    // 1. Khởi tạo Lenis (cuộn mượt) nếu chưa có
    if (!window.lenisInstance && window.Lenis) {
        window.lenisInstance = new Lenis({
            duration: 1.2,
            easing: (t) => Math.min(1, 1.001 - Math.pow(2, -10 * t))
        });
        function raf(time) {
            window.lenisInstance.raf(time);
            requestAnimationFrame(raf);
        }
        requestAnimationFrame(raf);
    }

    // 2. Khởi tạo GSAP ScrollTrigger
    if (window.gsap && window.ScrollTrigger) {
        gsap.registerPlugin(ScrollTrigger);

        // Reset ScrollTrigger trước khi khởi tạo hoạt ảnh mới để tránh trùng lặp khi routing SPA
        ScrollTrigger.getAll().forEach(trigger => trigger.kill());
        
        // Hoạt ảnh Hero Section
        const heroTl = gsap.timeline({ defaults: { ease: 'power2.out' } });
        if (document.querySelector('.hero-overline')) {
            heroTl.fromTo('.hero-overline', { opacity: 0, letterSpacing: '0.1em' }, { opacity: 1, letterSpacing: '0.3em', duration: 1.5 });
        }
        if (document.querySelectorAll('.hero-line').length > 0) {
            heroTl.fromTo('.hero-line', { y: '110%' }, { y: '0%', duration: 1.2, stagger: 0.4 }, 0.4);
        }
        if (document.querySelector('.hero-subtitle')) {
            heroTl.fromTo('.hero-subtitle', { opacity: 0, y: 20 }, { opacity: 1, y: 0, duration: 1 }, 1.2);
        }
        if (document.querySelector('.hero-cta')) {
            heroTl.fromTo('.hero-cta', { opacity: 0, y: 20 }, { opacity: 1, y: 0, duration: 1 }, 1.5);
        }
        if (document.querySelectorAll('.hero-stat').length > 0) {
            heroTl.fromTo('.hero-stat', { opacity: 0, y: 30 }, { opacity: 1, y: 0, duration: 1, stagger: 0.15 }, 1.8);
        }

        // Hoạt ảnh cuộn cho Features
        if (document.querySelector('#features') && document.querySelectorAll('.feature-card').length > 0) {
            gsap.fromTo('.feature-card', 
                { opacity: 0, y: 60 },
                { 
                    opacity: 1, 
                    y: 0, 
                    duration: 1, 
                    stagger: 0.12, 
                    ease: 'power2.out',
                    scrollTrigger: {
                        trigger: '#features',
                        start: 'top 75%'
                    }
                }
            );
        }

        // Hoạt ảnh cuộn cho Counters
        if (document.querySelectorAll('.counter-item').length > 0) {
            gsap.fromTo('.counter-item',
                { opacity: 0, y: 40 },
                {
                    opacity: 1,
                    y: 0,
                    duration: 1.2,
                    stagger: 0.15,
                    ease: 'power2.out',
                    scrollTrigger: {
                        trigger: '.counter-item',
                        start: 'top 85%'
                    }
                }
            );
        }

        // Hoạt ảnh cuộn cho Temperature section
        if (document.querySelector('.temp-section')) {
            gsap.fromTo('.temp-section',
                { opacity: 0, y: 40 },
                {
                    opacity: 1,
                    y: 0,
                    duration: 1.2,
                    ease: 'power2.out',
                    scrollTrigger: {
                        trigger: '.temp-section',
                        start: 'top 75%'
                    }
                }
            );
        }
    }
};

// Khởi tạo Custom Cursor
window.initCustomCursor = () => {
    const cursor = document.getElementById('custom-cursor');
    const cursorDot = document.getElementById('custom-cursor-dot');
    if (!cursor || !cursorDot) return;

    // Reset styles ban đầu để tránh bị ẩn
    cursor.style.display = 'block';
    cursorDot.style.display = 'block';

    const onMouseMove = (e) => {
        cursor.style.setProperty('--cx', `${e.clientX - 16}px`);
        cursor.style.setProperty('--cy', `${e.clientY - 16}px`);
        cursorDot.style.setProperty('--dotx', `${e.clientX - 2}px`);
        cursorDot.style.setProperty('--doty', `${e.clientY - 2}px`);
    };
    
    document.removeEventListener('mousemove', onMouseMove);
    document.addEventListener('mousemove', onMouseMove);

    const handleMouseOver = (e) => {
        const target = e.target.closest('a, button, [data-cta]');
        if (target) {
            cursor.style.setProperty('--cscale', '1.5');
            cursor.style.setProperty('--cborder', 'var(--ice)');
            cursor.style.setProperty('--cbg', 'rgba(72,219,251,0.05)');
            cursorDot.style.setProperty('--dotbg', 'var(--ice)');
        }
    };

    const handleMouseOut = (e) => {
        const target = e.target.closest('a, button, [data-cta]');
        if (target) {
            cursor.style.setProperty('--cscale', '1');
            cursor.style.setProperty('--cborder', 'var(--cursor-ring)');
            cursor.style.setProperty('--cbg', 'transparent');
            cursorDot.style.setProperty('--dotbg', 'var(--cursor-dot)');
        }
    };

    document.removeEventListener('mouseover', handleMouseOver);
    document.removeEventListener('mouseout', handleMouseOut);
    
    document.addEventListener('mouseover', handleMouseOver);
    document.addEventListener('mouseout', handleMouseOut);
};

// Cập nhật Icons Lucide
window.initLucide = () => {
    if (window.lucide) {
        window.lucide.createIcons();
    }
};
