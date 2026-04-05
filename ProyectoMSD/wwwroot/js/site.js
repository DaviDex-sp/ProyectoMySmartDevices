// site.js - MySmartDevice Logic
document.addEventListener('DOMContentLoaded', () => {
    const themeToggle = document.getElementById('theme-toggle');
    const body = document.body;
    
    if (!themeToggle) return;

    const icon = themeToggle.querySelector('i');

    // Lógica del Toggle (Sesión persistida por Cookie)
    // Nota: Sustituye a localStorage por temas de compatibilidad en SSR
    themeToggle.addEventListener('click', () => {
        const isDark = body.classList.toggle('dark-theme');

        // Cambiar icono
        if (icon) {
            if (isDark) {
                icon.classList.replace('fa-moon', 'fa-sun');
            } else {
                icon.classList.replace('fa-sun', 'fa-moon');
            }
        }

        // Guardar preferencia en Cookie (vigencia de 1 año)
        const themeValue = isDark ? 'dark' : 'light';
        document.cookie = `ThemePref=${themeValue}; path=/; max-age=${60 * 60 * 24 * 365}; SameSite=Lax`;

        // Animación de feedback
        themeToggle.style.transform = 'scale(0.9)';
        setTimeout(() => {
            themeToggle.style.transform = '';
        }, 100);
    });
});
