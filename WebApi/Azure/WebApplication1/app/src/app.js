angular.module('ehrDashboard', [
    'ngAnimate',
    'ngAria',
    'ngMaterial',
    //'ui-router'
])
.config(function ($mdThemingProvider) {
    $mdThemingProvider.theme('default')
      .primaryPalette('blue')
      .accentPalette('green');
})
//.config(function ($stateProvider) {
//    $stateProvider
//        .state('home',
//        {
//            url: "/",
//            templateUrl: "core/home.html",
//            controller: "HomeCtrl",
//            controllerAs: "mc"
//        });
//});
