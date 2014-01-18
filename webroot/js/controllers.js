'use strict';
/* Controllers */
var inject = ['$scope', '$location', 'Model', '$routeParams', '$cookies', '$resource'];

function MyCtrl1($scope, $location, Model, $routeParams, $cookies, $resource) {
    var moveResource = $resource('do/Cube/move/:x/:y/:z', {}, {});
    $scope.pos = {};
    $scope.logs = [];
    var scale = 100;

    $scope.move = function (m) {
        $scope.pos = moveResource.get({ x: m.x, y: m.y, z: m.z });
    };

    var item = $('#dragTarget');
    var initial = item.offset();

    var message = function (a) { console.log(a); };

    var connect = function () {
        var socket;

        var host = "ws://192.168.0.100:1081";
        socket = new WebSocket(host);

        message('Socket Status: ' + socket.readyState);

        socket.onopen = function () {
            message('Socket Status: ' + socket.readyState + ' (open)');
        };

        socket.onmessage = function (msg) {
            var pos = angular.extend({}, angular.fromJson(msg.data));
            if (pos.x && pos.y) {
                $scope.pos = { x: pos.x * scale, y: pos.y * -scale };
            }
            else {
                if (pos.log) {
                    $scope.logs.push(pos);
                }
                message('Received: ' + msg.data);
            }
            $scope.$apply();
        };

        socket.onclose = function () {
            message('Socket Status: ' + socket.readyState + ' (Closed)');
        };

        return socket;
    };

    var sckt = connect();

    $scope.onMove = function (m) {
        if (sckt == null || sckt.readyState != 1) {
            sckt.close();
            sckt = connect();
        }
        if (sckt.readyState == 1) {
            var offset = item.offset();
            $scope.pos = {
                x: offset.left - initial.left,
                y: offset.top - initial.top
            };

            var pos = { x: $scope.pos.x * 1 / scale, y: $scope.pos.y * -1 / scale };
            sckt.send(angular.toJson(pos));
            $scope.$apply();
        }

    };

    $scope.$watch('pos', function (pos) {
        item.offset({
            left: initial.left + pos.x,
            top: initial.top + pos.y
        });
    });

}
MyCtrl1.$inject = inject;


function MyCtrl2() {

}
MyCtrl2.$inject = [];

function MetaCtrl($scope, $location, Model, $routeParams, $cookies, $resource) {
    $scope.getBtnMethod = function (btn) {
        switch (btn) {
            case "PUT":
                return "label-success";
            case "POST":
                return "label-warning";
            case "GET":
                return "label-info";
            case "DELETE":
                return "label-important";
            default:
                return "label";
        }
    }

    $scope.metadata = Model.get({ service: 'resources' }, function (response) {

        for (var i = 0; i < response.apis.length; i++) {
            response.apis[i].name = $(response.apis[i].path.split('/')).last().get(0);
            $scope.metadata.apis[i].resource = Model.get({ service: 'resource', resource: response.apis[i].name });
        }
    });
}

MetaCtrl.$inject = inject;