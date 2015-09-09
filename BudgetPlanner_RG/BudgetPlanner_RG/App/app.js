var app = angular.module('budget_planner', ['ui.router', 'LocalStorageModule']);

app.config(function ($stateProvider, $urlRouterProvider) {
    //
    // For any unmatched url, redirect to /state1
    $urlRouterProvider.otherwise("/login");
    //
    // Now set up the states
    $stateProvider
      .state('login', {
          url: "/login",
          templateUrl: "/app/templates/login.html",
          controller: "loginCtrl as login"
      })
      .state('home', {
          url: "/home",
          templateUrl: "/app/templates/home.html",
          controller: "homeCtrl as home"
      })
      .state('household', {
         url: "/household",
         templateUrl: "/app/templates/household.html",
         controller: "houseCtrl as house"
      })
      .state('accounts', {
         url: "/accounts",
         templateUrl: "/app/templates/accounts.html",
         controller: "houseAccountCtrl as accounts"
      })
    .state('account', {
        url: "/account/{id}",
        templateUrl: "/app/templates/account.html",
        controller: "houseAccountCtrl as account"
    })

});

var serviceBase = 'http://localhost:51066/';

app.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorSvc');
});

app.run(['authSvc', function (authService) {
    authService.fillAuthData();
}]);