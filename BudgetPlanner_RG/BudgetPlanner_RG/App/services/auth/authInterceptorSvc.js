'use strict';
angular.module('budget_planner')
    .factory('authInterceptorSvc', ['$q', '$injector', '$location', 'localStorageService', function ($q, $injector, $location, localStorageService) {

        var authInterceptorServiceFactory = {};

        var _request = function (config) {

            config.headers = config.headers || {};

            var authData = localStorageService.get('authorizationData');
            if (authData) {
                config.headers.Authorization = 'Bearer ' + authData.token;
            }

            return config;
        }

        var _responseError = function (rejection) {
            var authService = $injector.get('authSvc');
            var authData = localStorageService.get('authorizationData');
            var $http = $injector.get('$http');

            switch (rejection.status) {
                case 401:
                    if (authData) {
                        return authService.refresh().then(function (response) {
                            return $http(rejection.config);
                        },
                            function (failure) {
                                authService.logOut();
                                $location.path('/login');
                            });
                    } else {
                        authService.logout();
                        $location.path('/login');
                    }
                case 403:
                    //Unauthorized access
                    authService.logout();
                    $location.path('/login');
                    break;
            }
            return $q.reject(rejection);
        }

        authInterceptorServiceFactory.request = _request;
        authInterceptorServiceFactory.responseError = _responseError;

        return authInterceptorServiceFactory;
    }]);