function GenerateMap(property, relatedproperties) {
    if (property.length > 0) {
        var lat = property[0].latitude;
        var lon = property[0].longitude;
    }

    var myLatlng = new google.maps.LatLng(lat, lon);
    var mapOptions = {
        zoom: 16,
        center: myLatlng
    }
    var map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);

    var marker = new google.maps.Marker({
        position: myLatlng,
        map: map,
        title: 'Location'
    });


    var markersAmnt = relatedproperties.length;
    for (var i = 0; i < markersAmnt; i++) {

        var markerPos = new google.maps.LatLng(relatedproperties[i].latitude, relatedproperties[i].longitude);

        relatedproperties[i].marker = new google.maps.Marker({
            position: markerPos,
            map: map,
            title: relatedproperties[i].streetadress + relatedproperties[i].route,
        });

       // var infoWindow = new google.maps.InfoWindow({
        //    content: relatedproperties[i].content
        //});
    }
}

function GenerateMarkers(markers, map) {

    var markersAmnt = markers.length;
    for (var i = 0; i < markersAmnt; i++) {

        var markerPos = new google.maps.LatLng(markers[i].position.lat, markers[i].position.lng);

        markers[i].marker = new google.maps.Marker({
            position: markerPos,
            map: map,
            title: markers[i].title,
        });

        var infoWindow = new google.maps.InfoWindow({
            content: markers[i].content
        });
    }
}

//google.maps.event.addDomListener(window, 'load', GenerateMap);
