
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
const askToggle = document.getElementById('ask-toggle');
const askPanel = document.getElementById('ask-panel');
const askInput = document.getElementById('ask-input');
const askSubmit = document.getElementById('ask-submit');
const askAnswer = document.getElementById('ask-answer');
const askLoading = document.getElementById('ask-loading');
const askError = document.getElementById('ask-error');

askToggle?.addEventListener('click', (e) => {
    e.stopPropagation();
    askPanel.classList.toggle('hidden');
    if (!askPanel.classList.contains('hidden')) askInput.focus();
});

document.addEventListener('click', (e) => {
    if (!askPanel?.contains(e.target) && e.target !== askToggle) {
        askPanel?.classList.add('hidden');
    }
});

async function submitAsk() {
    const question = askInput.value.trim();
    if (!question) return;

    askAnswer.classList.add('hidden');
    askError.classList.add('hidden');
    askLoading.classList.remove('hidden');
    askSubmit.disabled = true;

    try {
        const res = await fetch('/api/ask', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ question })
        });

        if (res.status === 429) {
            askError.textContent = "Too many questions — try again in a moment.";
            askError.classList.remove('hidden');
            return;
        }

        if (!res.ok) {
            askError.textContent = "Something went wrong — try again.";
            askError.classList.remove('hidden');
            return;
        }

        const data = await res.json();
        askAnswer.textContent = data.answer;
        askAnswer.classList.remove('hidden');
    } catch {
        askError.textContent = "Couldn't reach the server.";
        askError.classList.remove('hidden');
    } finally {
        askLoading.classList.add('hidden');
        askSubmit.disabled = false;
    }
}

askSubmit?.addEventListener('click', submitAsk);
askInput?.addEventListener('keydown', (e) => { if (e.key === 'Enter') submitAsk(); });