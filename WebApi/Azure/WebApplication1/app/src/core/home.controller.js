angular.module("ehrDashboard")
    .controller("HomeCtrl", function(Data, Calculate) {
        var vm = this;
        vm.data = {};
        vm.groupDiag = "group";
        vm.unit = "number";
        Data.getData().then(function (data) {
            vm.data = data.data;
            vm.processed = Calculate.getMetrics(vm.data.patients, vm.data.providers);
        });
    });  