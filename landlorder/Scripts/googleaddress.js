var placeSearch, autocomplete, map, infowindow;
var componentForm = {
    street_number: 'short_name',
    route: 'long_name',
    locality: 'long_name',
    administrative_area_level_1: 'short_name',
    country: 'long_name',
    postal_code: 'short_name'
};

var exactaddressProperty;
var relatedPropertyArray = [];

var markerarray = [];

function initialize() {
    autocomplete = new google.maps.places.Autocomplete(
        /** @type {HTMLInputElement} */(document.getElementById('autocomplete')),
        { types: ['geocode'] });
}

$( document ).ready(function() {
    searchbar = new google.maps.places.Autocomplete(
      /** @type {HTMLInputElement} */(document.getElementById('searchbar')),
      { types: ['geocode'] });

    var current_page_url = window.location.href;
    var current_page_id = current_page_url.split("/");
    var current_page_name = current_page_id[(current_page_id.length) - 1];

    if (current_page_name != "" && current_page_name != "Index" && current_page_name != "Home") {
        document.getElementById('searchDIV').style.display = 'block';
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


function initSearch() {
    //Read GET variable
    //query = GetParameterByName('locationinput');

    //****************************
    //SET UP CODE HERE TO GET RECENT REVIEWS IF LOCATION IS NULL
    //***************************

    GenerateMap();
    //SendLocationData(query);


    //Start the autocomplete service and get predictions from input
    //var service = new google.maps.places.AutocompleteService();
    //service.getQueryPredictions({ input: query }, autocomplete_callback);
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


//Map Essentials
function GenerateMap() {
    map = new google.maps.Map(document.getElementById('map-canvas'), {
        center: new google.maps.LatLng(40.7903, 73.9597),
        disableDefaultUI: true,
        zoom: 12
    });
}
function GetLocationDetailsFromID(ID) {
    infowindow = new google.maps.InfoWindow();
    var service = new google.maps.places.PlacesService(map);

    service.getDetails(ID, function (place, status) {
        if (status == google.maps.places.PlacesServiceStatus.OK) {
            var marker = new google.maps.Marker({
                map: map,
                title: place.formatted_address,
                position: place.geometry.location
            });

            markerarray.push(marker);

            //Change map position to geolocation of user input
            var panPoint = new google.maps.LatLng(place.geometry.location.lat(), place.geometry.location.lng());
            map.panTo(panPoint)

            //Format Data & send data to server side
            var locationData = FormatPlaceData(place);
            SendLocationData(locationData);


            infowindow.setContent(place.formatted_address);
            infowindow.open(map, marker);


            google.maps.event.addListener(marker, 'mouseover', function () {
                infowindow.setContent(place.formatted_address);
                infowindow.open(map, this);
            });
            google.maps.event.addListener(marker, 'mouseout', function () {
                infowindow.close();
            });
        }
    });
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
                title: html,
                zIndex: Math.round(latlng.lat() * -100000) << 5
            });

            markerarray.push(marker);

            google.maps.event.addListener(marker, 'mouseover', function () {
                infowindow.setContent(contentString);
                infowindow.open(map, marker);
            });
            google.maps.event.addListener(marker, 'mouseout', function () {
                infowindow.close();
            });
        }
    }
}


//Functions for formatting Google API data and sending server-side
function SendLocationData(data) {

    $.ajax({
        url: "/Reviews/GetLocationData",
        type: 'POST',
        data: JSON.stringify(data),
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (result) {            
            ListPropertyResults(result.property, result.relatedproperties, data)
            CreateSurroundingLocationMarkers(result.relatedproperties);
        },
        error: function (xhr, exception) {
            console.log(exception);

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
        locationData["latitude"] = place.geometry.location.lat();
        locationData["longitude"] = place.geometry.location.lng();
        locationData["formatted_address"] = place.formatted_address;
        locationData["type"] = place.types[0];
        locationData["vicinity"] = place.vicinity;
    }

    return locationData;
}


function ListPropertyResults(exactProperty, relatedProperties, originalPlaceData) {
    var section = document.createElement('section');
    section.id = "results_section";
    relatedPropertyArray = relatedProperties;

    document.getElementById('spinnerholder').style.display = 'none';
    document.getElementById('map-canvas').style.opacity = '1';


    //Create the title
    document.getElementById("results-title").innerHTML = 'Landlord Reviews for "' + originalPlaceData.formatted_address + '"';
    



    //Write Exact Property
    var div = GenerateExactResultsDIV(exactProperty, originalPlaceData);
    if (div != null) {
        section.appendChild(div);

        //Create seperator line
        var seperator = GenerateSeperatorDIV();
        section.appendChild(seperator);
    }  
    

    //Write Related Properties
    for (var i = 0; i < relatedProperties.length; i++) {
        var div = document.createElement("div");
        var divLink = relatedPropertyArray[i].propertyID;
        div.innerHTML = "<a href= '/Reviews/Details/" + divLink + "'><h4>" + relatedProperties[i].formatted_address +
            "</h4> <h5>Reviews: " + relatedProperties[i].numofReviews + "</h5></a>";
        div.className = "item_holder";
        div.setAttribute('data-index', i);
        div.onmouseover = PanToMarker;
        section.appendChild(div);

        //Create seperator line
        var seperator = document.createElement("div");
        seperator.className = "seperator";
        document.getElementById('relatedpropertyDIV').appendChild(seperator);
        section.appendChild(seperator);
    }
    document.getElementById('relatedpropertyDIV').appendChild(section);

}

function GenerateExactResultsDIV(exactProperty, originalPlaceData) {
    var div = document.createElement("div");

    //Check if there are any reviews in the DB.
    if (exactProperty != null && exactProperty.length > 0) {
        var divLink = exactProperty[0].propertyID;
        //divLink = divLink.replace(/[ ]*,[ ]*|[ ]+/g, '-');
        //var divLink = exactProperty[0].streetaddress + "+" + exactProperty[0].route + "+" + exactProperty[0].city + "+" + exactProperty[0].state;

        div.innerHTML = "<a href= '/Reviews/Details/" + divLink + "'><h4>" + exactProperty[0].formatted_address +
            "</h4> <h5>Reviews: " + exactProperty[0].numofReviews + "</h5></a>";
        exactaddressProperty = exactProperty[0].formatted_address;
    }

    if (originalPlaceData.type == "street_address") {
        var divLink = originalPlaceData.formatted_address;
        //var divLink = exactProperty[0].streetaddress + "+" + exactProperty[0].route + "+" + exactProperty[0].city + "+" + exactProperty[0].state;

        div.innerHTML = "<a href= '/Reviews/DetailsNew/" + divLink + "'><h4>" + originalPlaceData.formatted_address +
            "</h4> <h5>Reviews: " + 0 + "</h5></a>";
        exactaddressProperty = originalPlaceData.formatted_address;
    }
    else {
        return;
    }

    div.className = "item_holder";
    div.onmouseover = PanToMarker_ExactAddress;

    return div;

}
function GenerateSeperatorDIV() {
    var seperator = document.createElement("div");
    document.createElement("div");
    seperator.className = "seperator";
    document.getElementById('relatedpropertyDIV').appendChild(seperator);
    return seperator;
}


function callback(results, status) {
    if (status == google.maps.places.PlacesServiceStatus.OK) {
        for (var i = 0; i < results.length; i++) {
            createMarker(results[i]);
        }
    }
}

function createMarker(place) {
    var placeLoc = place.geometry.location;
    var marker = new google.maps.Marker({
        map: map,
        position: place.geometry.location
    });

    google.maps.event.addListener(marker, 'click', function () {
        infowindow.setContent(place.name);
        infowindow.open(map, this);
    });
}


//Map Pan Functions for hover
function PanToMarker() {
    var i = this.getAttribute('data-index');
    var address;
    if (typeof relatedPropertyArray[i] != 'undefined') {
        address = relatedPropertyArray[i].formatted_address;
    }
    else { return; }


    result = $.map(markerarray, function (obj, index) {
        if (obj.title == address) {
            return index;
        }
    })

    if (result.length > 0) {
        MarkerZoomTo(result[0]);
    }
}
function PanToMarker_ExactAddress() {
    var address;
    if (exactaddressProperty != null) {
        address = exactaddressProperty;
    }
    else { return; }


    result = $.map(markerarray, function (obj, index) {
        if (obj.title == address) {
            return index;
        }
    })

    if (result.length > 0) {
        MarkerZoomTo(result[0]);
    }
}
function MarkerZoomTo(markerIdentifier) {
    pt = markerarray[markerIdentifier].getPosition();
    newpt = new google.maps.LatLng(pt.lat(), pt.lng());
    map.panTo(newpt);

    if (infowindow) {
        infowindow.close();
    }

    infowindow.setContent(markerarray[markerIdentifier].title);
    infowindow.setPosition(markerarray[markerIdentifier].getPosition());

    infowindow.open(map, markerarray[markerIdentifier]);
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



function geocode(place) {
    var configurl = 'http://www.mapquestapi.com/geocoding/v1/address?key=xpfcnvnPNvsWqLnz5uSNwmR9t8uk1o4C&location=' + place.formatted_address;
    
    $.ajax({
        type: "POST",                
        url: configurl,
        success: function(response) {                        
            //console.log(response.results[0].locations[0].latLng.lat);
            //console.log(response.results[0].locations[0].latLng.lng);
            SendLatLng(response.results[0].locations[0].latLng.lat, response.results[0].locations[0].latLng.lng, place.formatted_address);
        },
        error: function (xhr, exception) {
        console.log(xhr.responseText);
    }
    })

}


