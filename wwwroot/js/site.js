// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
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
    currentSlide = (currentSlide + 1) % slides.length; // Loop back to the beginning
    showSlide(currentSlide);
});

// Event listener for the "prev" button
prevBtn.addEventListener('click', () => {
    currentSlide = (currentSlide - 1 + slides.length) % slides.length; // Loop back to the end
    showSlide(currentSlide);
});




var modal = document.getElementById('modal');

var btn = document.getElementById('modifyDataButton');

var span = document.getElementById('closeModal');

var form = document.getElementById('modifyDataForm');

btn.onclick = function () {
    modal.style.display = "block";
};

span.onclick = function () {
    modal.style.display = "none";
};

window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
};
