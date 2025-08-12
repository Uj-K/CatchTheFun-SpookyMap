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

    const info = new google.maps.InfoWindow(); // 말풍선 하나 재사용

    locations.forEach(loc => {
        // 다양한 키 대응 + 문자열일 수도 있어 숫자로 변환
        const latRaw = loc.Lat ?? loc.lat ?? loc.latitude;
        const lngRaw = loc.Lng ?? loc.lng ?? loc.longitude;
        const lat = typeof latRaw === "string" ? parseFloat(latRaw) : latRaw;
        const lng = typeof lngRaw === "string" ? parseFloat(lngRaw) : lngRaw;
        if (!Number.isFinite(lat) || !Number.isFinite(lng)) return;

        const position = { lat, lng };
        const marker = new google.maps.Marker({
            position,
            map,
            title: (loc.address ?? loc.Address ?? ""),
        });

        //  마커 클릭 시 정보 표시되는곳
        marker.addListener("click", () => {
            const name = loc.name ?? loc.Name ?? "";
            const addr = loc.address ?? loc.Address ?? "";
            const desc = loc.description ?? loc.Description ?? "";
            info.setContent(`
                <div style="min-width:220px">
                  <strong>${name}</strong><br>
                  ${addr}${desc ? `<br><small>${desc}</small>` : ""}
                </div>
            `);
            info.open({ anchor: marker, map });
        });

        markers.push(marker);
        bounds.extend(position);
    });

    if (markers.length > 0) map.fitBounds(bounds);
}
