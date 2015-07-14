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

function initialize() {
    // Create the autocomplete object, restricting the search
    // to geographical location types.
    autocomplete = new google.maps.places.Autocomplete(
        /** @type {HTMLInputElement} */(document.getElementById('autocomplete')),
        { types: ['geocode'] });
    // When the user selects an address from the dropdown,
    // populate the address fields in the form.

    //google.maps.event.addListener(locationbtn, 'click', function () {
    //    fillInAddress();
    //});
}

// [START region_fillform]
function fillInAddress() {
    // Get the place details from the autocomplete object.
    var place = autocomplete.getPlace();

    if (place.address_components.length > 0) {
        sendlocationData(place);
    }


    /*Sends data piece by piece
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

function sendlocationData(array) {

    var data = {
        street_number: "10323",
        route: "sunstream lane",
    };
    console.log(data);

    $.ajax({
        url: "/Home/GetLocationData",
        type: 'POST',
        traditional: true,
        data: JSON.stringify(data),
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: window.location = '/Home/Search',
        error: function (jqXHR, exception) {
            alert('Error message.');
            console.log(jqXHR.statusText);
        }
    });
}
// [END region_geolocation]


function checkEnterBtnKeyDown(e) {
    if (e.keyCode == 13) {
        fillInAddress()
        return false;
    }
}