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
        cursor.style.transform = `translate3d(${e.clientX - 16}px, ${e.clientY - 16}px, 0)`;
        cursorDot.style.transform = `translate3d(${e.clientX - 2}px, ${e.clientY - 2}px, 0)`;
    };
    
    document.removeEventListener('mousemove', onMouseMove);
    document.addEventListener('mousemove', onMouseMove);

    const onMouseEnter = () => {
        cursor.classList.add('scale-150', 'border-[var(--ice)]', 'bg-[rgba(72,219,251,0.05)]');
        cursorDot.classList.add('bg-[var(--ice)]');
    };
    const onMouseLeave = () => {
        cursor.classList.remove('scale-150', 'border-[var(--ice)]', 'bg-[rgba(72,219,251,0.05)]');
        cursorDot.classList.remove('bg-[var(--ice)]');
    };

    document.querySelectorAll('a, button, [data-cta]').forEach(el => {
        el.removeEventListener('mouseenter', onMouseEnter);
        el.removeEventListener('mouseleave', onMouseLeave);
        el.addEventListener('mouseenter', onMouseEnter);
        el.addEventListener('mouseleave', onMouseLeave);
    });
};

// Cập nhật Icons Lucide
window.initLucide = () => {
    if (window.lucide) {
        window.lucide.createIcons();
    }
};
