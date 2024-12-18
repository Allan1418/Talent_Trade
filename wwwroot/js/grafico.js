document.addEventListener('DOMContentLoaded', function () {
    // Obtener los datos del modelo (MesesGanancias) desde C# a JavaScript
    // Es importante que el nombre de la variable coincida con el nombre en tu modelo
    var mesesGananciasData = @Html.Raw(Json.Serialize(Model.MesesGanancias));

    // Preparar los datos para el gráfico
    var labels = mesesGananciasData.map(function (item) {
        return item.mes; // Asegúrate de que 'mes' coincida con el nombre de la propiedad en tu modelo
    });
    var data = mesesGananciasData.map(function (item) {
        return item.ganancia; // Asegúrate de que 'ganancia' coincida con el nombre de la propiedad en tu modelo
    });

    // Obtener el contexto del canvas
    var ctx = document.getElementById('gananciasChart').getContext('2d');

    // Crear el gráfico de barras
    var myChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Ganancias por Mes',
                data: data,
                backgroundColor: 'rgba(54, 162, 235, 0.5)', // Color de las barras
                borderColor: 'rgba(54, 162, 235, 1)', // Color del borde de las barras
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            },
            plugins: {
                legend: {
                    labels: {
                        color: 'white'
                    }
                },

            },
            scales: {
                x: {
                    ticks: {
                        color: 'white'
                    }
                },
                y: {
                    ticks: {
                        color: 'white'
                    }
                }
            }
        }
    });
});