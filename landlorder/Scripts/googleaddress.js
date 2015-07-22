//var input = document.getElementById('locationinput');
//var options = { types: ["geocode"], componentRestrictions: { country: 'us' } };
//new google.maps.places.Autocomplete(input, options);

var placeSearch, autocomplete;
var componentForm = {
    street_number: 'short_name',
    route: 'long_name',
    locality: 'long_name',
    administrative_area_level_1: 'short_name',
    country: 'long_name',
    postal_code: 'short_name'
};

var POSTcomponentForm = {
    street_number: 'short_name',
    route: 'long_name',
    city: 'long_name',
    state: 'short_name',
    country: 'long_name',
    postal_code: 'short_name',
    postal_code_suffix: 'short_name'
};

function initialize() {
    // Create the autocomplete object, restricting the search
    // to geographical location types.
    
    autocomplete = new google.maps.places.Autocomplete(
        /** @type {HTMLInputElement} */(document.getElementById('autocomplete')),
        { types: ['geocode'] });
    // When the user selects an address from the dropdown,
    // populate the address fields in the form.

    google.maps.event.addListener(autocomplete, 'places_changed', function () {        
        fillInAddress()
    });
    
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

function fillInAddress() {
    // Get the place details from the autocomplete object.  
    var place = searchcriteria.getPlace();    
    
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
    }    
   
    if (Object.keys(locationData).length > 0) {
        var type = place.address_components[0].types[0];
        sendlocationData(locationData, type);
    }


    /*Sends data piece by piece

    if (place.address_components.length > 0) {
        var type = place.address_components[0].types[0];
        sendlocationData(place, type);
    }
    for (var i = 0; i < place.address_components.length; i++) {
        var addressType = place.address_components[i].types[0];
        if (componentForm[addressType]) {
            var val = place.address_components[i][componentForm[addressType]]; 
            //document.getElementById(addressType).value = val;
        }
    }
    */
}

function initSearch() {   
    //Read GET variable
    query = getParameterByName('locationinput');
    
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
    var map = new google.maps.Map(document.getElementById('map-canvas'), {
        center: new google.maps.LatLng(-33.8665433, 151.1956316),
        zoom: 15
    });

    var infowindow = new google.maps.InfoWindow();
    var service = new google.maps.places.PlacesService(map);

    service.getDetails(ID, function(place, status) {
        if (status == google.maps.places.PlacesServiceStatus.OK) {
            var marker = new google.maps.Marker({
                map: map,
                position: place.geometry.location
            });
            google.maps.event.addListener(marker, 'click', function() {
                infowindow.setContent(place.name);
                infowindow.open(map, this);
            });
        }
    });
}


function GenerateMap(property, relatedproperties, inputlocation) {
    var lat;
    var lon;
    if (inputlocation.geometry) {
        lat = inputlocation.latitude
        lon = inputlocation.longitude;
    }
    else if (relatedproperties.length > 0) {
        lat = property[0].latitude;
        lon = property[0].longitude;
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

function sendlocationData(data, datatype) {
    
    $.ajax({
        url: "/Home/GetLocationData",
        type: 'POST',
        data: JSON.stringify(data),
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            window.location = result.Url;
            GenerateMap(result.property, result.relatedproperties, data)
        },
        error: function (xhr, exception) {
            console.log(exception);
            
        }
    });
}

//Helper Function
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}


