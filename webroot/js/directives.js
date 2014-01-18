'use strict';

/* Directives */


var module = angular.module('myApp.directives', []);
module.directive('appVersion', ['version', function (version) {
    return function (scope, elm, attrs) {
        elm.text(version);
    };
}]);

module.directive('bagGameController', [function () {
    return function (scope, elm, attrs) {
        //GameController.init({
           
        //    canvas: elm.get(0).id,
        //    left: {
        //        type: 'joystick',
        //        position: { left: '50%', bottom: '50%' },
        //        joystick: {
        //            touchMove: scope.$eval(attrs.bagGameController)
        //        }
        //    },
        //    right: { }
        //});
        
        

        scope.$watch(attrs.bagGameController, function (value) {
            //GameController.options.left.joystick.touchMove = value;
            
        });
        

    }
}]);
