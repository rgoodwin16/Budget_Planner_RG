'use strict';
angular.module('budget_planner').controller('loginCtrl', ['authSvc', '$state', function (authSvc, $state) {
    var self = this;

    self.username = '';
    self.password = '';

    self.errors = null;

    self.submit = function () {
        authSvc.login(self.username, self.password).then(function (success) {
            $state.go('household');
        }, function (error) {
            self.errors = error.data;
        });
    }

}])