// site.js - MySmartDevice Logic
document.addEventListener('DOMContentLoaded', () => {
    const themeToggle = document.getElementById('theme-toggle');
    const body = document.body;
    
    if (!themeToggle) return;

    const icon = themeToggle.querySelector('i');

    // Lógica del Toggle (Sesión Volátil)
    // Nota: Se eliminó localStorage por conflictos en Merge/Producción (MSD13)
    themeToggle.addEventListener('click', () => {
        const isDark = body.classList.toggle('dark-theme');

        // Cambiar icono
        if (icon) {
            if (isDark) {
                icon.classList.replace('fa-moon', 'fa-sun');
                if (icon.classList.contains('fa-moon')) icon.classList.remove('fa-moon');
                icon.classList.add('fa-sun');
            } else {
                icon.classList.replace('fa-sun', 'fa-moon');
                if (icon.classList.contains('fa-sun')) icon.classList.remove('fa-sun');
                icon.classList.add('fa-moon');
            }
        }

        // Animación de feedback
        themeToggle.style.transform = 'scale(0.9)';
        setTimeout(() => {
            themeToggle.style.transform = '';
        }, 100);
    });
});
