function initMap() {
    // Dark purple Halloween map theme
    const styles = [
        { elementType: 'geometry', stylers: [{ color: '#1a1027' }] },
        { elementType: 'labels.icon', stylers: [{ visibility: 'off' }] },
        { elementType: 'labels.text.fill', stylers: [{ color: '#b8a9d9' }] },
        { elementType: 'labels.text.stroke', stylers: [{ color: '#1a1027' }] },
        { featureType: 'road', elementType: 'geometry', stylers: [{ color: '#2a1c3f' }] },
        { featureType: 'road', elementType: 'geometry.stroke', stylers: [{ color: '#3a2754' }] },
        { featureType: 'water', elementType: 'geometry', stylers: [{ color: '#140b22' }] },
        { featureType: 'poi.park', elementType: 'geometry', stylers: [{ color: '#1e1330' }] },
        { featureType: 'poi', elementType: 'geometry', stylers: [{ color: '#201438' }] }
    ];

    const fallbackCenter = { lat: 47.2529, lng: -122.4443 }; // Tacoma
    const map = new google.maps.Map(document.getElementById("map"), {
        zoom: 13,
        center: fallbackCenter,
        styles,
        mapTypeControl: false,
        streetViewControl: false
    });

    const raw = document.getElementById("location-data")?.textContent?.trim();
    const locations = (raw ? JSON.parse(raw) : null) || [];

    const bounds = new google.maps.LatLngBounds();
    const markers = [];
    const info = new google.maps.InfoWindow();

    // Pumpkin marker icon (SVG) with outline for dark background
    const icon = {
        url: 'https://cdn.jsdelivr.net/gh/twitter/twemoji@14.0.2/assets/svg/1f383.svg',
        scaledSize: new google.maps.Size(28, 28),
        anchor: new google.maps.Point(14, 14)
    };

    let highlightMarker = null;
    const highlightId = (typeof window !== 'undefined' && typeof window.__HIGHLIGHT_ID__ !== 'undefined')
        ? Number(window.__HIGHLIGHT_ID__) : null;

    locations.forEach(loc => {
        const latRaw = loc.Lat ?? loc.lat ?? loc.latitude;
        const lngRaw = loc.Lng ?? loc.lng ?? loc.longitude;
        const lat = typeof latRaw === "string" ? parseFloat(latRaw) : latRaw;
        const lng = typeof lngRaw === "string" ? parseFloat(lngRaw) : lngRaw;
        if (!Number.isFinite(lat) || !Number.isFinite(lng)) return;

        const position = { lat, lng };
        const marker = new google.maps.Marker({ position, map, title: (loc.address ?? loc.Address ?? ""), icon });

        marker.addListener("click", () => {
            const name = loc.name ?? loc.Name ?? "";
            const addr = loc.address ?? loc.Address ?? "";
            const desc = loc.description ?? loc.Description ?? "";
            const photo = loc.photoUrl ?? loc.PhotoUrl ?? "";
            const st = loc.startTime ?? "";
            const et = loc.endTime ?? "";
            const extraRaw = (loc.somethingElse ?? loc.SomethingElse ?? false);
            const hasExtra = (typeof extraRaw === "boolean") ? extraRaw : (typeof extraRaw === "string") ? ["true","1","yes","y","on"].includes(extraRaw.toLowerCase()) : (typeof extraRaw === "number") ? (extraRaw === 1) : false;

            info.setContent(`
                <div style="min-width:240px;line-height:1.35;color:#e9e2f8">
                    ${photo ? `<div style="margin-bottom:6px">
                        <a href="${photo}" target="_blank" rel="noopener noreferrer" title="Open full size">
                            <img src="${photo}" alt="" style="max-width:240px;max-height:160px;border-radius:8px;object-fit:cover;border:1px solid rgba(255,255,255,.12);cursor:zoom-in"/>
                        </a>
                    </div>` : ""}
                    <div style="font-family:'Creepster',cursive;font-size:18px;color:#ff7b00">${name}</div>
                    <div style="font-size:13px;color:#b8a9d9">${addr}</div>
                    ${desc ? `<div style="margin-top:4px;font-size:13px">${desc}</div>` : ""}
                    ${(st || et) ? `<div style="margin-top:6px;font-size:12px;color:#c3a6ff">Hours: ${st}${(st && et) ? " ∼ " : ""}${et}</div>` : ""}
                    <div style="margin-top:6px;font-size:12px">Other treats: <span style="color:${hasExtra ? '#7CFC00' : '#ffb84d'}">${hasExtra ? 'Yes' : 'No'}</span></div>
                </div>
            `);
            try { info.open({ anchor: marker, map }); } catch { info.open(map, marker); }
        });

        // 하이라이트 대상 체크 (id가 전달된 경우 자동 오픈 + 흔들림)
        const idVal = Number(loc.id ?? loc.Id);
        if (Number.isFinite(highlightId) && idVal === highlightId) {
            highlightMarker = marker;
            map.setCenter(position);
            map.setZoom(16);
            setTimeout(() => {
                marker.setAnimation(google.maps.Animation.BOUNCE);
                // 클릭 이벤트 트리거하여 팝업 열기
                google.maps.event.trigger(marker, 'click');
                // 1.4초 후 흔들림 중지
                setTimeout(() => marker.setAnimation(null), 1400);
            }, 350);
        }

        markers.push(marker);
        bounds.extend(position);
    });

    if (markers.length > 0 && !highlightMarker) {
        map.fitBounds(bounds);
    }
}
