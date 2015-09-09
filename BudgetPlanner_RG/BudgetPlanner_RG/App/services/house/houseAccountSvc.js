(function () {
    angular.module('budget_planner')
    .factory('houseAccountSvc', ['$http', function ($http) {
        var f = {};

        f.index = function () {
            return $http.post('api/HouseHoldAccounts/Index').then(function (response) {
                return response.data
            })
        }

        f.create = function (name, balance, householdid) {
            return $http.post('api/HouseHoldAccounts/Create', new { Name:name, Balance:balance, HouseHoldId:householdid }).then(function (response) {
                return response.data
            })
        }

        f.details = function (id) {
            return $http.post('api/HouseHoldAccounts/Details?id='+ id ).then(function (response) {
                return response.data
            })
        }

        f.edit = function (name, balance, isarchived) {
            return $http.post('api/HouseHoldAccounts/Edit', new { Name:name, Balance:balance, isArchived:isarchived }).then(function (response) {
                return response.data
            })
        }

        f.archive = function (id) {
            return $http.post('api/HouseHoldAccounts/Archive', new { HouseHoldAccountId:id }).then(function (response) {
                return response.data
            })
        }

        return f;

    }])
})();