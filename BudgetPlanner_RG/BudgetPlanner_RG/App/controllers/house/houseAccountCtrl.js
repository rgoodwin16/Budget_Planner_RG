'use strict';
angular.module('budget_planner').controller('houseAccountCtrl', ['houseAccountSvc', '$state', function (houseHoldSvc, $state) {

    var self = this;

    this.display = {};
    this.id = "";


    this.getAccount = function () {
        houseAccountSvc.details(id).then(function (data) {
            self.display = data;
        })
    }

    this.createAccount = function () {
        houseAccountSvc.create().then(function (data) {
            self.display = data;
        })
    }

    this.editAccount = function () {
        //console.log(self.inviteEmail);
        houseAccountSvc.edit()
    }

    this.archiveAccount = function () {
        houseAccountSvc.archive()
    }

    this.getAccount();

}])
