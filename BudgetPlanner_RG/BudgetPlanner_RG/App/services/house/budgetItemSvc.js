(function () {
    angular.module('budget_planner')
    .factory('budgetItemSvc', ['$http', function ($http) {
        var f = {};

        f.index = function () {
            return $http.post('api/BudgetItems').then(function (response) {
                return response.data
            })
        }

        f.create = function () {
            return $http.post('api/BudgetItems/Create').then(function (response) {
                return response.data
            })
        }

        f.details = function () {
            return $http.post('api/BudgetItems/Details').then(function (response) {
                return response.data
            })
        }

        f.edit = function () {
            return $http.post('api/BudgetItems/Edit').then(function (response) {
                return response.data
            })
        }

        f.delete = function () {
            return $http.post('api/BudgetItems/Delete').then(function (response) {
                return response.data
            })
        }

        return f;

    }])
})();