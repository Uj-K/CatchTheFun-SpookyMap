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

            // ✅ JS에서 DTO 읽기 
            const photo = loc.photoUrl ?? loc.PhotoUrl ?? "";
            const st = loc.startTime ?? ""; // "18:00" (Index.cshtml에서 문자열로 내려옴)
            const et = loc.endTime ?? ""; // "21:00"

            // ✅ somethingElse 안전 변환 
            const extraRaw = (loc.somethingElse ?? loc.SomethingElse ?? false);
            const hasExtra = (typeof extraRaw === "boolean")
                ? extraRaw
                : (typeof extraRaw === "string")
                    ? ["true", "1", "yes", "y", "on"].includes(extraRaw.toLowerCase())
                    : (typeof extraRaw === "number")
                        ? extraRaw === 1
                        : false;

            info.setContent(`
                <div style="min-width:240px;line-height:1.3">
                ${photo ? `<div style="margin-bottom:6px">
                    <img src="${photo}" alt="" style="max-width:240px;max-height:160px;border-radius:8px;object-fit:cover"/>
                </div>` : ""}
            <strong>${name}</strong><br>${addr}
            ${desc ? `<br><small>${desc}</small>` : ""}
            ${(st || et) ? `<div style="margin-top:6px"><small>Hours: ${st}${(st && et) ? "∼" : ""}${et}</small></div>` : ""}
            <div style="margin-top:6px"><small>Other treats: ${hasExtra ? "Yes" : "No"}</small></div>
            </div>
        `);

            try { info.open({ anchor: marker, map }); } catch { info.open(map, marker); }
        });


        markers.push(marker);
        bounds.extend(position);
    });

    if (markers.length > 0) map.fitBounds(bounds);
}
