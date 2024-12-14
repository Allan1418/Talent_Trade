document.addEventListener('DOMContentLoaded', () => {
    const container = document.getElementById('cardContainer');
    const prevButton = document.getElementById('prev');
    const nextButton = document.getElementById('next');
    const dotsContainer = document.getElementById('dots');
    const cards = document.querySelectorAll('.membresias-card');

    let currentIndex = 0;
    const cardsPerView = 3; // Número de tarjetas visibles a la vez
    const totalPages = Math.ceil(cards.length / cardsPerView);

    function updateDots() {
        dotsContainer.innerHTML = '';
        for (let i = 0; i < totalPages; i++) {
            const dot = document.createElement('div');
            dot.classList.add('dot');
            if (i === currentIndex) {
                dot.classList.add('active');
            }
            dot.addEventListener('click', () => {
                scrollToPage(i);
            });
            dotsContainer.appendChild(dot);
        }
    }

    function scrollToPage(pageIndex) {
        currentIndex = pageIndex;
        container.scrollTo({ left: pageIndex * container.offsetWidth, behavior: 'smooth' });
        updateDots();
    }

    prevButton.addEventListener('click', () => {
        if (currentIndex > 0) {
            scrollToPage(currentIndex - 1);
        }
    });

    nextButton.addEventListener('click', () => {
        if (currentIndex < totalPages - 1) {
            scrollToPage(currentIndex + 1);
        }
    });

    container.addEventListener('scroll', () => {
        const scrollLeft = container.scrollLeft;
        const pageWidth = container.offsetWidth;
        currentIndex = Math.round(scrollLeft / pageWidth);
        updateDots();
    });

    updateDots();
});
