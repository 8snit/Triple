"use strict";

app.controller('placesExplorerController', [
    '$scope', 'placesExplorerService', 'placesPhotosService', '$filter', '$modal', '$http', 'leafletData', 'tripService', 'FBURL', '$firebase', '$routeParams',
    function ($scope, placesExplorerService, placesPhotosService, $filter, $modal, $http, leafletData, tripService, FBURL, $firebase, $routeParams) {
        if (!$routeParams.trip) {
            return;
        }

        $scope.geojson = {
            data: {
                "type": "FeatureCollection",
                "features": new Array()
            },
            style: tripService.style,
            pointToLayer: tripService.pointToLayer,
            onEachFeature: function (feature, layer) {
                layer.bindPopup(
                    '<fieldset class="clearfix input-pill pill mobile-cols">' +
                    JSON.stringify(feature.properties) +
                    '<br><input type="text" id="message" class="col9" />' +
                    '<button id="add-button" class="col3">Update' +
                    '</button></fieldset>');
            },
            filter: function (feature, layer) {
                return true;
            },
            resetStyleOnMouseout: true
        };
        tripService.connect($scope.geojson.data.features);
        
        angular.extend($scope, {
            world: tripService.initial
        });

        $scope.$on("leafletDirectiveMap.click", function (event, args) {
            var leafEvent = args.leafletEvent;
            tripService.add(leafEvent.latlng, "me@" + new Date());
        });

        $scope.exploreNearby = "Vienna";
        $scope.exploreQuery = "";
        $scope.filterValue = "";

        $scope.places = [];
        $scope.filteredPlaces = [];
        $scope.filteredPlacesCount = 0;

        //paging
        $scope.totalRecordsCount = 0;
        $scope.pageSize = 5;
        $scope.currentPage = 1;

        init();

        function init() {

            createWatch();
            getPlaces();
        }

        function getPlaces() {

            var offset = ($scope.pageSize) * ($scope.currentPage - 1);

            placesExplorerService.get({ near: $scope.exploreNearby, query: $scope.exploreQuery, limit: $scope.pageSize, offset: offset }, function (placesResult) {

                if (placesResult.response.groups) {
                    $scope.places = placesResult.response.groups[0].items;
                    $scope.totalRecordsCount = placesResult.response.totalResults;
                    filterPlaces('');
                } else {
                    $scope.places = [];
                    $scope.totalRecordsCount = 0;
                }

            });
        };

        function filterPlaces(filterInput) {
            $scope.filteredPlaces = $filter("placeNameCategoryFilter")($scope.places, filterInput);
            $scope.filteredPlacesCount = $scope.filteredPlaces.length;
        }

        function createWatch() {

            $scope.$watch("filterValue", function (filterInput) {
                filterPlaces(filterInput);
            });
        }

        $scope.doSearch = function () {

            $scope.currentPage = 1;
            getPlaces();

        };

        $scope.pageChanged = function (page) {

            $scope.currentPage = page;
            getPlaces();

        };

        $scope.showVenuePhotos = function (venueId, venueName) {

            placesPhotosService.get({ venueId: venueId }, function (photosResult) {

                var modalInstance = $modal.open({
                    templateUrl: 'appbuild/views/placesphotos.html',
                    controller: 'placesPhotosController',
                    resolve: {
                        venueName: function () {
                            return venueName;
                        },
                        venuePhotos: function () {
                            return photosResult.response.photos.items;
                        }
                    }
                });

                modalInstance.result.then(function () {
                    //$scope.selected = selectedItem;
                }, function () {
                });

            });

        };

        $scope.buildCategoryIcon = function (icon) {
            if (!icon) {
                return "";
            }

            return icon.prefix + '44' + icon.suffix;
        };

        $scope.buildVenueThumbnail = function (photo) {
            if (!photo) {
                return "";
            }
            return photo.items[0].prefix + '128x128' + photo.items[0].suffix;
        };

        $scope.bookmarkPlace = function (venue) {
            tripService.add(venue.location, venue.name);
            return;
        };

    }
]);