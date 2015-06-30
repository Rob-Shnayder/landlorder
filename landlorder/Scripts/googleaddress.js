var input = document.getElementById('locationinput');
var options = { types: ["geocode"], componentRestrictions: { country: 'us' } };

new google.maps.places.Autocomplete(input, options);
