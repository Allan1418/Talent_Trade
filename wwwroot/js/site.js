const slider = document.querySelector('.slider');
const slides = document.querySelectorAll('.slide');
const prevBtn = document.querySelector('.prev');
const nextBtn = document.querySelector('.next');
let currentSlide = 0;

// Function to show the specified slide
function showSlide(n) {
    slides.forEach((slide) => {
        slide.style.display = 'none';
    });
    slides[n].style.display = 'block';
}

// Initial display of the first slide
showSlide(currentSlide);

// Event listener for the "next" button
nextBtn.addEventListener('click', () => {
    currentSlide = (currentSlide + 1) % slides.length;
    showSlide(currentSlide);
});

// Event listener for the "prev" button
prevBtn.addEventListener('click', () => {
    currentSlide = (currentSlide - 1 + slides.length) % slides.length;
    showSlide(currentSlide);
});

// Modal Functionality
// Obtener referencias a los elementos
const modal = document.getElementById('myModal');
const modalBackdrop = document.querySelector('.modal-backdrop');
const closeButton = document.querySelector('.close-modal'); // O cualquier otra clase para el botón de cerrar

// Mostrar el modal
function showModal() {
    modal.classList.add('show');
    modalBackdrop.classList.add('show');
}

// Ocultar el modal
function hideModal() {
    modal.classList.remove('show');
    modalBackdrop.classList.remove('show');
}

// Agregar un event listener al botón de cerrar
closeButton.addEventListener('click', hideModal);

// Agregar un event listener al backdrop para cerrar el modal al hacer clic fuera
modalBackdrop.addEventListener('click', (event) => {
    if (event.target === modalBackdrop) {
        hideModal();
    }
});

