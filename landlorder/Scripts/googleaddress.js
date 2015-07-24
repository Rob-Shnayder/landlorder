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
    // Create the autocomplete object, restricting the search
    // to geographical location types.
    
    autocomplete = new google.maps.places.Autocomplete(
        /** @type {HTMLInputElement} */(document.getElementById('autocomplete')),
        { types: ['geocode'] });
    // When the user selects an address from the dropdown,
    // populate the address fields in the form.

    //google.maps.event.addListener(autocomplete, 'places_changed', function () {        
   //     fillInAddress()
    //});
    
}

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


function initSearch() {   
    //Read GET variable
    query = GetParameterByName('locationinput');
    
    //****************************
    //SET UP CODE HERE TO GET RECENT REVIEWS IF LOCATION IS NULL
    //***************************
    
    GenerateMap();

    //Start the autocomplete service and get predictions from input
    var service = new google.maps.places.AutocompleteService();
    service.getQueryPredictions({ input: query }, autocomplete_callback);    
}

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

function GetLocationDetailsFromID(ID) {   
    infowindow = new google.maps.InfoWindow();
    var service = new google.maps.places.PlacesService(map);

    service.getDetails(ID, function(place, status) {
        if (status == google.maps.places.PlacesServiceStatus.OK) {
            var marker = new google.maps.Marker({
                map: map,
                position: place.geometry.location
            });

            //Change map position to geolocation of user input
            var panPoint = new google.maps.LatLng(place.geometry.location.A, place.geometry.location.F);
            map.panTo(panPoint)

            //Format Data & send data to server side
            var type = place.address_components[0].types[0];
            var locationData = FormatPlaceData(place);
            SendLocationData(locationData, type);            
            
            
            infowindow.setContent(place.formatted_address);
            infowindow.open(map, marker);


            google.maps.event.addListener(marker, 'click', function() {
                infowindow.setContent(place.formatted_address);
                infowindow.open(map, this);
            });
        }
    });
}

function FormatPlaceData(place) {
    var locationData = {};
    for (var i = 0; i < place.address_components.length; i++) {
        var addressType = place.address_components[i].types[0];
        if (componentForm[addressType]) {
            var val = place.address_components[i][componentForm[addressType]];

            if (addressType == "administrative_area_level_1") {
                locationData["state"] = val;
            }
            else if (addressType == "locality") {
                locationData["city"] = val;
            }
            else {
                locationData[addressType] = val;
            }
        }
    }

    if (place.geometry.location) {
        locationData["latitude"] = place.geometry.location.A;
        locationData["longitude"] = place.geometry.location.F;
        locationData["formatted_address"] = place.formatted_address;
        locationData["type"] = place.types[0];
        locationData["vicinity"] = place.vicinity;
    }

    return locationData;
}

function CreateSurroundingLocationMarkers(properties) {
    for (var i = 0; i < properties.length; i++) {
        var Latlng = new google.maps.LatLng(properties[i].latitude, properties[i].longitude);
        CreateMarker(Latlng, properties[i].formatted_address);

        function CreateMarker(latlng, html) {
            var contentString = html;
            var marker = new google.maps.Marker({
                position: latlng,
                map: map,
                zIndex: Math.round(latlng.lat() * -100000) << 5
            });

            google.maps.event.addListener(marker, 'click', function () {
                infowindow.setContent(contentString);
                infowindow.open(map, marker);
            });
        }


    }
}

function GenerateMap() {
    map = new google.maps.Map(document.getElementById('map-canvas'), {
        center: new google.maps.LatLng(40.7903, 73.9597),
        zoom: 15
    });
}

function SendLocationData(data, datatype) {
    
    $.ajax({
        url: "/Home/GetLocationData",
        type: 'POST',
        data: JSON.stringify(data),
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            CreateSurroundingLocationMarkers(result.relatedproperties);
        },
        error: function (xhr, exception) {
            console.log(exception);
            
        }
    });
}

//Helper Function
function GetParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}


//**********************************
//END SEARCH FUNCTIONS
//**********************************


