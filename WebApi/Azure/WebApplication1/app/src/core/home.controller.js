angular.module("proposalTemplate")
    .controller("HomeCtrl", function($scope, Data) {
        var vm = this;
        vm.group = "Banana";
        vm.outcome = "Apple";
        vm.show = "Institutional";
        vm.barX = "group";
        vm.barY = "unit";
        Data.getData().then(function(data) {
            console.log(data);
            vm.boxData = data[0];
            vm.data = data[1];
        });
    });
