window.tourHelper = {
    highlightElement: function (selector) {
        // Remove previous highlight
        const prev = document.querySelectorAll('.tour-highlighted');
        prev.forEach(el => el.classList.remove('tour-highlighted'));
        
        const el = document.querySelector(selector);
        if (el) {
            el.classList.add('tour-highlighted');
            
            // Wait for Blazor render / CSS transition, then calculate coordinates
            const rect = el.getBoundingClientRect();
            return {
                found: true,
                top: rect.top + window.scrollY,
                left: rect.left + window.scrollX,
                width: rect.width,
                height: rect.height
            };
        }
        return { found: false };
    },
    removeHighlight: function () {
        const prev = document.querySelectorAll('.tour-highlighted');
        prev.forEach(el => el.classList.remove('tour-highlighted'));
    },
    positionTooltip: function (tooltipSelector, targetSelector, position) {
        const tooltip = document.querySelector(tooltipSelector);
        if (!tooltip) return;

        const target = document.querySelector(targetSelector);
        if (!target) {
            // Center on screen if target element not found
            tooltip.style.position = 'fixed';
            tooltip.style.top = '50%';
            tooltip.style.left = '50%';
            tooltip.style.transform = 'translate(-50%, -50%)';
            return;
        }

        tooltip.style.position = 'absolute';
        tooltip.style.transform = ''; // Clear center transform

        const targetRect = target.getBoundingClientRect();
        const tooltipRect = tooltip.getBoundingClientRect();
        
        const targetTop = targetRect.top + window.scrollY;
        const targetLeft = targetRect.left + window.scrollX;
        
        let top = 0;
        let left = 0;
        
        if (position === 'bottom') {
            top = targetTop + targetRect.height + 12;
            left = targetLeft + (targetRect.width / 2) - (tooltipRect.width / 2);
        } else if (position === 'top') {
            top = targetTop - tooltipRect.height - 12;
            left = targetLeft + (targetRect.width / 2) - (tooltipRect.width / 2);
        } else if (position === 'left') {
            top = targetTop + (targetRect.height / 2) - (tooltipRect.height / 2);
            left = targetLeft - tooltipRect.width - 12;
        } else if (position === 'right') {
            top = targetTop + (targetRect.height / 2) - (tooltipRect.height / 2);
            left = targetLeft + targetRect.width + 12;
        }
        
        // Keep inside viewport bounds
        if (left < 10) left = 10;
        if (left + tooltipRect.width > window.innerWidth - 10) {
            left = window.innerWidth - tooltipRect.width - 10;
        }
        if (top < 10) top = 10;
        
        tooltip.style.top = top + 'px';
        tooltip.style.left = left + 'px';
    }
};
