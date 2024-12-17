const carouselContainer = document.querySelector('.carousel-container');
const prevButton = document.querySelector('.prev-button');
const nextButton = document.querySelector('.next-button');
const membershipCards = document.querySelectorAll('.membership-card');
const dotsContainer = document.querySelector('.carousel-dots');

let currentIndex = 0;
const cardsPerPage = 3; // Número de tarjetas visibles a la vez

// Función para actualizar el carrusel
function updateCarousel() {
    const offset = -currentIndex * membershipCards[0].offsetWidth;
    carouselContainer.style.transform = `translateX(${offset}px)`;

    // Actualizar los puntos
    updateDots();
}

// Función para crear los puntos de navegación
function createDots() {
    const totalDots = Math.ceil(membershipCards.length / cardsPerPage);
    for (let i = 0; i < totalDots; i++) {
        const dot = document.createElement('div');
        dot.classList.add('dot');
        dot.addEventListener('click', () => {
            currentIndex = i;
            updateCarousel();
        });
        dotsContainer.appendChild(dot);
    }
}

// Función para actualizar la clase 'active' de los puntos
function updateDots() {
    const dots = dotsContainer.querySelectorAll('.dot');
    dots.forEach(dot => dot.classList.remove('active'));
    dots[currentIndex].classList.add('active');
}

// Event listeners para los botones
prevButton.addEventListener('click', () => {
    currentIndex = Math.max(0, currentIndex - 1);
    updateCarousel();
});

nextButton.addEventListener('click', () => {
    const maxIndex = Math.ceil(membershipCards.length / cardsPerPage) - 1;
    currentIndex = Math.min(maxIndex, currentIndex + 1);
    updateCarousel();
});

// Inicialización
createDots();
updateCarousel();