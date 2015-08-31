var placeSearch, autocomplete, map, infowindow;
var markerarray = [];

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
    var address = document.getElementsByClassName('addressinfo');
    addresses = [].map.call(address, function (input) {
        return input.value;
    });

    var newArray = names.map(function (str) {
        return JSON.parse("{" + str.replace(/lat/, '"lat"').replace(/lng/, '"lng"').replace(/href: (.+)/, '"href": "$1"').replace(/address: (.+)/, '"address": "$1"').replace(/;/, "") + "}")
    });
    var addressArray = addresses.map(function (str) {
        return JSON.parse("{" + str.replace(/address: (.+)/, '"address": "$1"').replace(/;/, "") + "}")
    });

    for (var i = 0; i < newArray.length; i++) {
        createMarker(newArray[i].lat, newArray[i].lng, newArray[i].href, addressArray[i].address);
    }

}
function SetMap(lat, lng) {
    var icon = '../images/red-dot3.png';

    map = new google.maps.Map(document.getElementById('map-canvas'), {
        center: new google.maps.LatLng(lat, lng),
        disableDefaultUI: false,
        navigationControl: false,
        mapTypeControl: false,
        zoom: 10
    });
    
   if (lat == null || lng == null) {
       return;
   }

   var location = new google.maps.LatLng(lat, lng);

   var marker = new google.maps.Marker({
        map: map,
        position: location
   });
   marker.setIcon(icon);
   markerarray.push(marker);


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


function createMarker(lat, lng, href, address) {
    if (lat == null || lng == null) {
        return;
    }
    var icon = '../images/red-dot3.png';
    
    infowindow = new google.maps.InfoWindow();
    var location = new google.maps.LatLng(lat, lng);

    var marker = new google.maps.Marker({
        map: map,
        position: location,
        title: address,
        content: href
    });

    marker.setIcon(icon);
    markerarray.push(marker);
    infowindow.setContent(href);

    google.maps.event.addListener(marker, 'click', function () {
        infowindow.setContent(href);
        infowindow.open(map, this);
    });
    /*
    if (type == "exact" || type == "exact-new") {
        infowindow.open(map, marker);
    }*/
    
}


function PanToMarker(address, icon) {
    result = $.map(markerarray, function (obj, index) {
        if (obj.title == address) {
            return index;
        }
    })

    if (result.length > 0) {
        MarkerZoomTo(result[0], icon);
    }
}


function MarkerZoomTo(markerIdentifier, icon) {
    /*
    pt = markerarray[markerIdentifier].getPosition();
    newpt = new google.maps.LatLng(pt.lat(), pt.lng());
    map.panTo(newpt);
    */

    if (infowindow) {
        infowindow.close();
    }

    /*
   infowindow.setContent(markerarray[markerIdentifier].content);
   infowindow.setPosition(markerarray[markerIdentifier].getPosition());
   infowindow.open(map, markerarray[markerIdentifier]);
   */
    var hoverIcon;
    if (icon == "yellow") {
        hoverIcon = '../images/yellow-dot2.png';
        markerarray[markerIdentifier].setIcon(hoverIcon);
    }
    else if (icon == "default") {
        hoverIcon = '../images/red-dot3.png';
        markerarray[markerIdentifier].setIcon(hoverIcon);
    }
}
