function initMap() {
    // 지도를 초기화할 때 사용할 기본값 (fallback)
    const fallbackCenter = { lat: 47.2529, lng: -122.4443 }; // Tacoma
    const map = new google.maps.Map(document.getElementById("map"), {
        zoom: 13,
        center: fallbackCenter,
    });

    const locations = JSON.parse(document.getElementById("location-data").textContent);

    const bounds = new google.maps.LatLngBounds();
    const markers = [];

    locations.forEach(loc => {
        const position = { lat: loc.latitude, lng: loc.longitude };
        const marker = new google.maps.Marker({
            position,
            map,
            title: loc.address
        });

        markers.push(marker);
        bounds.extend(position);
    });

    if (locations.length > 0) {
        map.fitBounds(bounds); // 모든 마커가 보이도록 자동 확대/중심 설정
    }

    // Marker Clusterer 적용
    new markerClusterer.MarkerClusterer({ map, markers });
}

