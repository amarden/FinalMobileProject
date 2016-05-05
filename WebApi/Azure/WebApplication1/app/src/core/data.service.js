angular.module("ehrDashboard")
    .service("Data", function($http){
        this.getData = function () {
            return $http.get("api/Dashboard")
                .then(function (data) {
                    return data;
                });
        };
    });
