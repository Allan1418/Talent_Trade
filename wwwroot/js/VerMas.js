document.addEventListener('DOMContentLoaded', () => {
    const publicaciones = document.querySelectorAll('.publicacion'); // Selecciona todas las publicaciones
    const verMasBtn = document.getElementById('verMasBtn'); // Botón "Ver más"

    if (!verMasBtn || publicaciones.length === 0) {
        console.error("El botón o las publicaciones no fueron encontrados.");
        return;
    }

    let publicacionVisible = 0; // Índice de la última publicación visible
    const publicacionesPorPagina = 4; // Número de publicaciones a mostrar por clic

    // Mostrar las primeras publicaciones al cargar
    const mostrarPublicaciones = (cantidad) => {
        const totalPublicaciones = publicaciones.length;

        for (let i = publicacionVisible; i < publicacionVisible + cantidad && i < totalPublicaciones; i++) {
            publicaciones[i].style.display = 'block';
        }

        publicacionVisible += cantidad;

        // Ocultar el botón si todas las publicaciones están visibles
        if (publicacionVisible >= totalPublicaciones) {
            verMasBtn.style.display = 'none';
        }
    };

    mostrarPublicaciones(publicacionesPorPagina); // Mostrar las primeras 4 publicaciones

    // Evento para el botón "Ver más"
    verMasBtn.addEventListener('click', () => {
        mostrarPublicaciones(publicacionesPorPagina); // Mostrar 4 publicaciones más
    });
});
