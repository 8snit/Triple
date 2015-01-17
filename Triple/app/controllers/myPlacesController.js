"use strict";

app.controller('myPlacesController', [
    '$scope', 'tripService', function ($scope, tripService) {
        // TODO paging
        $scope.myPlaces = new Array();
        $scope.totalRecordsCount = 0;
        $scope.pageSize = 10;
        $scope.currentPage = 1;

        $scope.filterPlaces = function (place) {
            return place && place.properties && place.properties.name;
        };

        init();

        function init() {
            tripService.connect($scope.myPlaces);
        }

        $scope.pageChanged = function (page) {
            $scope.currentPage = page;
        };
    }
]);