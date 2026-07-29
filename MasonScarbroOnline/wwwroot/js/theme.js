
document.getElementById('theme-toggle')?.addEventListener('click', () => {
    const isDark = document.documentElement.classList.toggle('dark');
    localStorage.setItem('theme', isDark ? 'dark' : 'light');
});

const progressBar = document.getElementById('scroll-progress');
if (progressBar) {
    window.addEventListener('scroll', () => {
        const scrolled = window.scrollY;
        const height = document.documentElement.scrollHeight - window.innerHeight;
        progressBar.style.width = `${(scrolled / height) * 100}%`;
    });
}

const revealEls = document.querySelectorAll('.reveal');
const nodes = document.querySelectorAll('.timeline-node');

const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.classList.add('is-visible');
            const nodeId = entry.target.dataset.node;
            const node = document.querySelector(`[data-node-target="${nodeId}"]`);
            node?.classList.add('is-active');
        }
    });
}, { threshold: 0.3 });

revealEls.forEach(el => observer.observe(el));