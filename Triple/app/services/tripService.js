"use strict";

app.factory('tripService', [
    'FBURL', '$firebase', function (FBURL, $firebase) {

        var ref = new window.Firebase(FBURL + '/trips/DEMO/features');

        var icons = {
            'Default': L.MakiMarkers.icon({ icon: "marker", color: "#a23", size: "l" }),
            'Category': L.MakiMarkers.icon({ icon: "star", color: "#b0b", size: "l" }),
            'Accomodation': L.MakiMarkers.icon({ icon: "building", color: "#a12", size: "l" }),
            'FoodAndDrink': L.MakiMarkers.icon({ icon: "restaurant", color: "#a4e", size: "l" }),
            'Entertainment': L.MakiMarkers.icon({ icon: "theatre", color: "#557", size: "l" }),
            'Transportation': L.MakiMarkers.icon({ icon: "car", color: "#876", size: "l" }),
            'Nature': L.MakiMarkers.icon({ icon: "garden", color: "#4CC417", size: "l" }),
            'SightSeeing': L.MakiMarkers.icon({ icon: "monument", color: "#a12", size: "l" }),
            'Transport': L.MakiMarkers.icon({ icon: "car", color: "#b0b", size: "l" }),
            'Car': L.MakiMarkers.icon({ icon: "car", color: "#b0b", size: "l" }),
            'Airplane': L.MakiMarkers.icon({ icon: "airport", color: "#423", size: "l" }),
            'Train': L.MakiMarkers.icon({ icon: "rail", color: "#ae2F", size: "l" }),
            'Ship': L.MakiMarkers.icon({ icon: "ferry", color: "#b0b", size: "l" }),
            'Ferry': L.MakiMarkers.icon({ icon: "ferry", color: "#b0b", size: "l" }),
            'Image': L.MakiMarkers.icon({ icon: "camera", color: "#b0b", size: "l" }),
            'Content': L.MakiMarkers.icon({ icon: "suitcase", color: "#b0b", size: "l" }),
            'Venue': L.MakiMarkers.icon({ icon: "monument", color: "#888", size: "l" }),
            'Note': L.MakiMarkers.icon({ icon: "polling-place", color: "#b0b", size: "l" })
        }

        var trip = {};
        trip.connect = function (array) {
            ref.limit(1000).on('child_added', function (snapshot) {
                var feature = snapshot.val();
                if (!feature.properties.name) {
                    feature.properties.name = feature.properties.rel + "-" + feature.properties.category;
                }
                array.push(feature);
            });
            ref.limit(1000).on('child_changed', function (snapshot) {
                var feature = snapshot.val();
                //TODO
            });
            ref.limit(1000).on('child_removed', function (snapshot) {
                var feature = snapshot.val();
                //TODO
            });
        }

        trip.add = function (latlng, name) {
            ref.push({
                "type": "Feature",
                "properties": {
                    "name": name,
                    "rel": "Venue",
                    "category": Math.random() > 0.5 ? "Entertainment" : "Accomodation"
                },
                "geometry": {
                    "type": "Point",
                    "coordinates": [latlng.lng, latlng.lat]
                }
            });
        }

        trip.initial = {
            lat: 47.52,
            lng: 15.6,
            zoom: 6
        }

        trip.style = function (feature) {
            if (feature.properties.category) {
                return {
                    fillColor: "red",
                    weight: 5,
                    opacity: 0.5,
                    color: 'black',
                    dashArray: '5',
                    fillOpacity: 0.4
                };
            } else if (feature.properties.transport) {
                return {
                    fillColor: "blue",
                    weight: 5,
                    opacity: 0.5,
                    color: 'black',
                    dashArray: '5',
                    fillOpacity: 0.4
                };
            } else {
                return {
                    fillColor: "green",
                    weight: 5,
                    opacity: 0.5,
                    color: 'black',
                    dashArray: '5',
                    fillOpacity: 0.4
                };
            }
        };

        trip.pointToLayer = function (feature, latlng) {
            if (feature.properties.category) {
                switch (feature.properties.rel) {
                    case "Venue":
                        return new L.marker(latlng, {
                            icon: icons[feature.properties.category] || icons["SightSeeing"]
                        });
                    case "Image":
                        if (feature.properties.uri
                            && feature.properties.uri.indexOf("http") == 0) {
                            var divIcon = new L.divIcon({
                                html: '<img src="' + feature.properties.uri + '" height="100%" />',
                                iconSize: [50, 50] //  feature.properties.metadata.width/height
                            });
                            var marker = new L.Marker(latlng, {
                                icon: divIcon
                            });
                            return marker;
                        } else {
                            return new L.marker(latlng, {
                                //zIndexOffset: 1, // TODO put in front
                                icon: icons["Image"] || icons["Default"]
                            });
                        }
                    default:
                        var icon = icons[feature.properties.rel] || icons["Default"];
                        return new L.marker(latlng, {
                            icon: icon
                        });
                }
            }
            else if (feature.properties.transport) {
                return new L.marker(latlng, {
                    icon: icons[feature.properties.transport] || icons["Transport"]
                });
            }
            else {
                var geojsonMarkerOptions = {
                    radius: 3,
                    fillColor: "#ff7800",
                    fillOpacity: 0.5,
                    color: "#000",
                    weight: 1,
                    opacity: 1,
                };
                return L.circleMarker(latlng, geojsonMarkerOptions);
            }
        };

        return trip;
    }
]);