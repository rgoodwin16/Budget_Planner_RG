'use strict';
angular.module('budget_planner').controller('houseCtrl', ['houseSvc', '$state', function (houseSvc, $state) {

    var self = this;

    this.display = {};
    this.inviteEmail = "";

    this.getHouse = function () {
        houseSvc.details().then(function (data) {
            self.display = data;
        })
    }

    this.createHouse = function () {
        houseSvc.create().then(function (data) {
            self.display = data;
        })
    }

    this.createInvite = function () {
        houseSvc.invite(self.inviteEmail)
    }

    this.joinHouse = function () {
        houseSvc.join()
    }

    this.leaveHouse = function () {
        houseSvc.leave()
    }

    this.getHouse();

}])
