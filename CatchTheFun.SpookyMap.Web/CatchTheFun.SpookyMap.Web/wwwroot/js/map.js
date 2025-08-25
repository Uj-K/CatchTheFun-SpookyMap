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

    // --- Weather box helpers ---
    const weatherBox = document.getElementById('weather-box');
    let lastWeather = { lat: null, lng: null, ts: 0 };

    function nearlySame(a, b, eps = 0.01) { // ~1km
        return a !== null && b !== null && Math.abs(a - b) < eps;
    }

    async function loadWeather(lat, lng) {
        if (!weatherBox) return;
        try {
            weatherBox.innerHTML = '<div class="small">Center of map</div><div class="small">Loading...</div>';
            const url = `https://api.open-meteo.com/v1/forecast?latitude=${lat.toFixed(4)}&longitude=${lng.toFixed(4)}&current=temperature_2m,precipitation,weather_code,wind_speed_10m&temperature_unit=fahrenheit&wind_speed_unit=mph&precipitation_unit=inch`;
            const res = await fetch(url);
            if (!res.ok) throw new Error('Weather fetch failed');
            const data = await res.json();
            const cur = data.current || {};
            const t = cur.temperature_2m;
            const w = cur.wind_speed_10m;
            const p = cur.precipitation;
            const code = cur.weather_code;
            const icon = weatherIcon(code);
            weatherBox.innerHTML = `
                <div class="d-flex align-items-center gap-2">
                    <span style="font-size:1.1rem">${icon}</span>
                    <div>
                        <div class="small">Center of map</div>
                        <div class="small">Temp: ${t ?? '?'}°F, Wind: ${w ?? '?'} mph</div>
                        <div class="small">Precip: ${p ?? 0} in</div>
                    </div>
                </div>`;
        } catch (e) {
            weatherBox.innerHTML = '<div class="small text-danger">Weather unavailable</div>';
        }
    }

    function weatherIcon(code) {
        // Minimal mapping for demo. See Open-Meteo weather codes.
        if (code === 0) return '☀️';
        if ([1,2].includes(code)) return '🌤️';
        if (code === 3) return '☁️';
        if ([45,48].includes(code)) return '🌫️';
        if ([51,53,55,56,57].includes(code)) return '🌦️';
        if ([61,63,65,66,67,80,81,82].includes(code)) return '🌧️';
        if ([71,73,75,77,85,86].includes(code)) return '❄️';
        if ([95,96,99].includes(code)) return '⛈️';
        return '🌡️';
    }

    function updateWeather() {
        if (!weatherBox) return;
        const c = map.getCenter();
        const lat = c?.lat();
        const lng = c?.lng();
        const now = Date.now();
        if (nearlySame(lat, lastWeather.lat) && nearlySame(lng, lastWeather.lng) && (now - lastWeather.ts) < 60_000) {
            return; // throttle ~60s for same area
        }
        lastWeather = { lat, lng, ts: now };
        loadWeather(lat, lng);
    }

    // Initial weather and when map settles after user interactions
    google.maps.event.addListenerOnce(map, 'idle', updateWeather);
    map.addListener('idle', updateWeather);

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
