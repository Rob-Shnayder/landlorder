var placeSearch, autocomplete, map, infowindow;
var componentForm = {
    street_number: 'short_name',
    route: 'long_name',
    locality: 'long_name',
    administrative_area_level_1: 'short_name',
    country: 'long_name',
    postal_code: 'short_name'
};

function initialize() {
    autocomplete = new google.maps.places.Autocomplete(
        /** @type {HTMLInputElement} */(document.getElementById('autocomplete')),
        { types: ['geocode'] });
}

$( document ).ready(function() {
    var current_page_url = window.location.href;
    var current_page_id = current_page_url.split("/");
    var current_page_name = current_page_id[(current_page_id.length) - 1];

    if (current_page_name != "" && current_page_name != "Index" && current_page_name != "Home") {
        document.getElementById('searchDIV').style.display = 'block';
        searchbar = new google.maps.places.Autocomplete(
      /** @type {HTMLInputElement} */(document.getElementById('searchbar')),
      { types: ['geocode'] });
    }
})




function geolocate() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var geolocation = new google.maps.LatLng(
                position.coords.latitude, position.coords.longitude);
            var circle = new google.maps.Circle({
                center: geolocation,
                radius: position.coords.accuracy
            });
            autocomplete.setBounds(circle.getBounds());
        });
    }
}

//**********************************
//SEARCH FUNCTIONS
//**********************************


function autocomplete_callback(predictions, status) {
    if (status != google.maps.places.PlacesServiceStatus.OK) {
        //CHANGE THIS!
        alert(status);
        return;
    }

    if (predictions) {
        var result = { placeId: predictions[0].place_id };
        GetLocationDetailsFromID(result);
    }

}


//Map Essentials
function GenerateMap() {
    map = new google.maps.Map(document.getElementById('map-canvas'), {
        center: new google.maps.LatLng(40.7903, 73.9597),
        disableDefaultUI: true,
        zoom: 5
    });
}

function GrabLocationData() {

    var inputs = document.getElementsByClassName('item');
    names = [].map.call(inputs, function (input) {
        return input.value;
    });

    var newArray = names.map(function (str) {
        return JSON.parse("{" + str.replace(/lat/, '"lat"').replace(/lng/, '"lng"').replace(/;/, "") + "}")
    });

    for (var i = 0; i < newArray.length; i++) {
        createMarker(newArray[i].lat, newArray[i].lng);
    }

}
function SetMap(lat, lng) {
    map = new google.maps.Map(document.getElementById('map-canvas'), {
        center: new google.maps.LatLng(lat, lng),
        disableDefaultUI: false,
        navigationControl: false,
        mapTypeControl: false,
        zoom: 12
    });
    
   if (lat == null || lng == null) {
       return;
   }

   var location = new google.maps.LatLng(lat, lng);

    var marker = new google.maps.Marker({
        map: map,
        position: location
    });


}
function SetMapDetails(lat, lng) {

    if (lat == null || lng == null) {
        lat = 0;
        lng = 0;
    }

    map = new google.maps.Map(document.getElementById('map-canvas'), {
        center: new google.maps.LatLng(lat, lng),
        scrollwheel: false,
        navigationControl: false,
        mapTypeControl: false,
        scaleControl: false,
        draggable: false,
        zoom: 14
    });



    var location = new google.maps.LatLng(lat, lng);

    var marker = new google.maps.Marker({
        map: map,
        position: location
    });


}
function createMarker(lat, lng) {
    if (lat == null || lng == null) {
        return;
    }

    var location = new google.maps.LatLng(lat, lng);

    var marker = new google.maps.Marker({
        map: map,
        position: location
    });
}
