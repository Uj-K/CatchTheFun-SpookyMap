﻿function initMap() {
    const center = { lat: 47.2529, lng: -122.4443 }; // Tacoma

    const map = new google.maps.Map(document.getElementById("map"), {
        zoom: 13,
        center: center,
    });

    new google.maps.Marker({
        position: center,
        map: map,
        title: "🎃 Spooky Spot",
    });
}
