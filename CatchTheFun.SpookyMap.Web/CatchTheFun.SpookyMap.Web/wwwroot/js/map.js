function initMap() {
    // 지도를 초기화할 때 사용할 기본값 (fallback)
    const fallbackCenter = { lat: 47.2529, lng: -122.4443 }; // Tacoma
    const map = new google.maps.Map(document.getElementById("map"), {
        zoom: 13,
        center: fallbackCenter,
    });

    const raw = document.getElementById("location-data")?.textContent?.trim();
    const locations = (raw ? JSON.parse(raw) : null) || [];  // null → []

    const bounds = new google.maps.LatLngBounds();
    const markers = [];

    locations.forEach(loc => {

        const lat = loc.Lat ?? loc.lat ?? loc.latitude;
        const lng = loc.Lng ?? loc.lng ?? loc.longitude;
        if (typeof lat !== "number" || typeof lng !== "number") return;


        const position = { lat, lng };
        const marker = new google.maps.Marker({
            position,
            map,
            title: loc.address ?? loc.Address ?? "",
        });

        markers.push(marker);
        bounds.extend(position);
    });

    if (markers.length > 0) map.fitBounds(bounds);
}

