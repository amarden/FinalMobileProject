angular.module("ehrDashboard")
    .service("Calculate", function () {
        this.getMetrics = function (patient, provider) {
            var metric = {};
            metric.ageAverage = patient
                .map(function (d) { return d.Age })
                .reduce(function (a, b) { return a + b; })
                / patient.length;

            metric.chatAverage = patient
                .map(function (d) { return d.chatCount })
                .reduce(function (a, b) { return a + b; })
                / patient.length;

            metric.imageAverage = patient
                .map(function (d) { return d.imageCount })
                .reduce(function (a, b) { return a + b; })
                / patient.length;

            metric.providerAverage = patient
                .map(function (d) { return d.providerCount })
                .reduce(function (a, b) { return a + b; })
                / patient.length;

            var groupDiag = _.groupBy(patient, "diagnosis");
            metric.topDiagnoses = _.map(groupDiag, function (data, key) {
                return {
                    diagnosis: key,
                    number: data.length
                }
            });

            return metric;
        };
    });