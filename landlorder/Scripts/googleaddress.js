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
        console.log(autocomplete.getPlaces()[0]);
        fillInAddress()
    });
}

// [START region_fillform]
function fillInAddress() {
    // Get the place details from the autocomplete object.
    var place = autocomplete.getPlace();
    console.log(document.getElementById('autocomplete'));
    
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
// [END region_fillform]

// [START region_geolocation]
// Bias the autocomplete object to the user's geographical location,
// as supplied by the browser's 'navigator.geolocation' object.
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

function sendlocationData(data, type) {
    
    $.ajax({
        url: "/Home/GetLocationData",
        type: 'POST',
        traditional: true,
        data: JSON.stringify(data),
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: window.location = '/Home/Search',
        error: function (jqXHR, exception) {
            console.log(jqXHR.statusText);
        }
    });
}
// [END region_geolocation]

