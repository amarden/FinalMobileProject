angular.module('ehrDashboard', [
    'ngAnimate',
    'ngAria',
    'ngMaterial'
])
.config(function ($mdThemingProvider) {
    $mdThemingProvider.theme('default')
      .primaryPalette('blue')
      .accentPalette('green');
});
