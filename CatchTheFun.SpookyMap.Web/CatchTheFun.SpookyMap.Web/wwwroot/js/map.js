//A dedicated JavaScript file that manages map - related logic separately.

function initMap() {
    const center = { lat: 47.2529, lng: -122.4443 }; // Tacoma

    const map = new google.maps.Map(document.getElementById("map"), {
        zoom: 13,
        center: center
    });

    // 테스트용 마커
    const marker = new google.maps.Marker({
        position: center,
        map: map,
        title: "Tacoma!"
    });

    const infoWindow = new google.maps.InfoWindow({
        content: "<b>Welcome to CatchTheFun!</b>"
    });

    marker.addListener("click", () => {
        infoWindow.open(map, marker);
    });
}
