angular.module("ehrDashboard")
    .controller("MainCtrl", function(Data, Calculate) {
        var vm = this;
        vm.data = {};
        vm.groupDiag = "diagnosis";
        vm.unit = "number";
        Data.getData().then(function (data) {
            console.log(data.data);
            vm.data = data.data;
            vm.processed = Calculate.getMetrics(vm.data.patients, vm.data.providers);
            console.log(vm.processed);
        });
    });  