var ctx = document.getElementById('gananciasChart').getContext('2d');
var gananciasChart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: @Html.Raw(Json.Serialize(Model.MesesGanancias.Select(m => $"{m.Month}/{m.Year}"))),
        datasets: [{
            label: 'Ganancias Mensuales',
            data: @Html.Raw(Json.Serialize(Model.MesesGanancias.Select(m => m.Total))),
            backgroundColor: 'rgba(54, 162, 235, 0.2)',
            borderColor: 'rgba(54, 162, 235, 1)',
            borderWidth: 1
        }]
    },
    options: {
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});