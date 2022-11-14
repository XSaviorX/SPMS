app.controller('APRDDash', ['$scope', '$http', '$document', function (s, h, d) {

    s.workforce = {};
    s.chartDataPL = [];
    s.chartLabelPlan = [];

    s.chartDataNP = [];
    s.chartLabelNP = [];
    s.incomingAppt = 0;
    s.pendingAppt = {};

    loadInitData();

    function loadInitData() {
        h.post('~/../../Home/APRDDashboardData').then(function (r) {
            if (r.data.status == "success") {
                s.workforce = r.data.workforce;
                s.chartDataPL = r.data.chartDataPL;
                s.chartLabelPL = r.data.chartLabelPL;
                s.pendingAppt = r.data.pendingAppt;
                loadChart();
            }
        });
    }

   

    function loadChart() {
        var ctx = document.getElementById('myChart').getContext('2d');

        var myChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                datasets: [{
                    data: s.chartDataPL,
                    backgroundColor: [
                        'red',
                        '#2851A3',
                        '#3578E5',
                        '#red',                   
                        '#00bcd4',
                        '#4caf50',
                        '#F5C33B',                       
                        '#FBCCD2',
                    ],
                }],
                labels: s.chartLabelPL,
            },
            options: {
                responsive: true,                
                plugins: {
                    legend: {
                        position: 'left',
                    },
                    title: {
                        display: true,
                        text: 'PLANTILLA'
                    }
                }
            },
        });

         
    }

}]);