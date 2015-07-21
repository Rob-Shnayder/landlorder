function initSearch() {
    var query = getParameterByName('locationinput');
    document.getElementById('getvar').value = query;

    autocomplete = new google.maps.places.Autocomplete(
        /** @type {HTMLInputElement} */(document.getElementById('getvar')),
        { types: ['geocode'] });
    fillInAddress();
}

function fillInAddress() {
    // Get the place details from the autocomplete object.
    var place = autocomplete.getPlace();

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

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}