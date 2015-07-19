function GenerateMap(lat, lon) {
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
}

//google.maps.event.addDomListener(window, 'load', GenerateMap);
