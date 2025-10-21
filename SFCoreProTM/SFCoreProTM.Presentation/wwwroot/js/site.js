(() => {
    const SIDEBAR_BREAKPOINT = 992;
    const body = document.body;
    const toggleBtn = document.getElementById('sidebarToggleBtn');
    const overlay = document.getElementById('sidebarOverlay');
    let previousIsMobile = window.innerWidth < SIDEBAR_BREAKPOINT;
    let resizeTimer;

    const isMobileView = () => window.innerWidth < SIDEBAR_BREAKPOINT;

    const updateAriaState = () => {
        if (!toggleBtn) {
            return;
        }

        const isCollapsed = isMobileView()
            ? !body.classList.contains('sidebar-open')
            : body.classList.contains('sidebar-collapsed');

        toggleBtn.setAttribute('aria-expanded', (!isCollapsed).toString());
        toggleBtn.setAttribute('aria-label', isCollapsed ? 'Show sidebar' : 'Hide sidebar');
    };

    const closeSidebar = () => {
        body.classList.add('sidebar-collapsed');
        body.classList.remove('sidebar-open');
        updateAriaState();
    };

    const toggleSidebar = () => {
        if (isMobileView()) {
            const isOpen = body.classList.contains('sidebar-open');
            if (isOpen) {
                body.classList.add('sidebar-collapsed');
                body.classList.remove('sidebar-open');
            } else {
                body.classList.remove('sidebar-collapsed');
                body.classList.add('sidebar-open');
            }
        } else {
            body.classList.toggle('sidebar-collapsed');
            body.classList.remove('sidebar-open');
        }

        updateAriaState();
    };

    const applyInitialState = () => {
        previousIsMobile = isMobileView();

        if (isMobileView()) {
            body.classList.add('sidebar-collapsed');
            body.classList.remove('sidebar-open');
        } else {
            body.classList.remove('sidebar-collapsed');
            body.classList.remove('sidebar-open');
        }

        updateAriaState();
    };

    document.addEventListener('DOMContentLoaded', () => {
        applyInitialState();

        toggleBtn?.addEventListener('click', toggleSidebar);
        overlay?.addEventListener('click', () => {
            if (isMobileView()) {
                closeSidebar();
            }
        });
    });

    window.addEventListener('resize', () => {
        clearTimeout(resizeTimer);
        resizeTimer = window.setTimeout(() => {
            const mobile = isMobileView();
            if (mobile !== previousIsMobile) {
                previousIsMobile = mobile;
                if (mobile) {
                    body.classList.add('sidebar-collapsed');
                    body.classList.remove('sidebar-open');
                } else {
                    body.classList.remove('sidebar-open');
                    body.classList.remove('sidebar-collapsed');
                }
                updateAriaState();
            }
        }, 150);
    });

    window.addEventListener('keydown', event => {
        if (event.key === 'Escape' && body.classList.contains('sidebar-open')) {
            closeSidebar();
        }
    });
})();
