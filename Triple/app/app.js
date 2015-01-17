window.app = angular.module('TripleApp', ['ngRoute', 'ngResource', 'ui.bootstrap', 'toaster', 'chieffancypants.loadingBar', 'leaflet-directive', 'firebase']);

app.constant('FBURL', 'https://triple.firebaseio.com');

app.config([
    '$routeProvider', function($routeProvider) {

        $routeProvider.when("/explore/:trip", {
            controller: "placesExplorerController",
            templateUrl: "/appbuild/views/placesresults.html"
        });

        $routeProvider.when("/places", {
            controller: "myPlacesController",
            templateUrl: "/appbuild/views/myplaces.html"
        });

        $routeProvider.when("/about", {
            controller: "aboutController",
            templateUrl: "/appbuild/views/about.html"
        });

        $routeProvider.otherwise({ redirectTo: "/explore/DEMO" });

    }
]);