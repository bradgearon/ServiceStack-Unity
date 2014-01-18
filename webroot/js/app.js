'use strict';

// Declare app level module which depends on filters, and services 
angular.module('myApp', ['myApp.filters', 'myApp.services', 'myApp.directives', 'ui.bootstrap', 'ngDragDrop']).
  config(['$routeProvider', '$locationProvider', function($routeProvider, $locationProvider) {
	
	$locationProvider.html5Mode(true);

    $routeProvider.when('/home', {templateUrl: 'partials/home.html', controller: MyCtrl1});
    $routeProvider.when('/about', {templateUrl: 'partials/about.md', controller: MyCtrl2});
	$routeProvider.when('/contact', {templateUrl: 'partials/contact.html', controller: MyCtrl2});
	$routeProvider.when('/meta', {templateUrl: 'partials/meta.html', controller: MetaCtrl});
    $routeProvider.otherwise({redirectTo: '/'});
  }]);
