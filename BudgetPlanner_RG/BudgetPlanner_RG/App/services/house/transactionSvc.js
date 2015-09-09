(function () {
    angular.module('budget_planner')
    .factory('transactionSvc', ['$http', function ($http) {
        var f = {};

        f.index = function () {
            return $http.post('api/HouseHoldAccounts/Transactions/Index').then(function (response) {
                return response.data
            })
        }

        f.create = function () {
            return $http.post('api/HouseHoldAccounts/Transactions/Create').then(function (response) {
                return response.data
            })
        }

        f.details = function () {
            return $http.post('api/HouseHoldAccounts/Transactions/Details').then(function (response) {
                return response.data
            })
        }

        f.edit = function () {
            return $http.post('api/HouseHoldAccounts/Transactions/Edit').then(function (resposne) {
                return response.data
            })
        }

        f.delete = function () {
            return $http.post('api/HouseHoldAccounts/Transactions/Delete').then(function (response) {
                return response.data
            })
        }

        return f;
    }])
})();