let map;
let vineyard_map;
let coordinates;

export function MapFunction(dotnetObjectReference) {
    map = L.map("dashboardMap", { center: new L.LatLng(48.361528, 15.370033), zoom: 14, zoomControl: false });
    SetMapBounds(map);

    L.tileLayer('http://{s}.google.com/vt?lyrs=s,h&x={x}&y={y}&z={z}', {
        maxZoom: 20,
        minZoom: 2,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3'],
        attribution: "Google Tile Layers",
        detectRetina: true
    }).addTo(map);
}

// ---------- Vineyards Map ----------
export function MapFunctionVineyards(dotnetObjectReference) {
    const mapDiv = document.getElementById("vineyardMap");
    const appbar = document.getElementsByClassName("mud-appbar")[0];
    const drawer = document.getElementsByClassName("mud-drawer")[0];
    const main = document.getElementsByClassName("mud-main-content")[0];

    function setSize() {
        const appbarHeight = appbar ? appbar.offsetHeight : 0;
        // WICHTIG: Breite immer 100% des Content-Bereichs – keine Drawer-Abzüge!
        mapDiv.style.width = "100%";
        mapDiv.style.height = `calc(100vh - ${appbarHeight}px)`;
        // Leaflet über Größenänderung informieren
        if (vineyard_map) {
            // minimal verzögert, damit CSS-Layout fertig ist
            requestAnimationFrame(() => vineyard_map.invalidateSize());
        }
    }

    // Initiale Größe
    setSize();

    vineyard_map = L.map("vineyardMap", {
        center: new L.LatLng(48.361528, 15.370033),
        zoom: 14,
        zoomControl: false
    });

    SetMapBounds(vineyard_map);

    L.tileLayer('http://{s}.google.com/vt?lyrs=s,h&x={x}&y={y}&z={z}', {
        maxZoom: 20,
        minZoom: 2,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3'],
        attribution: "Google Tile Layers",
        detectRetina: true
    }).addTo(vineyard_map);

    // --- Responsives Re-Layout ---
    // 1) Fenstergröße
    window.addEventListener("resize", setSize);

    // 2) Wenn Drawer auf/zu geht (Breite ändert sich)
    if (drawer) {
        const drawerResize = new ResizeObserver(setSize);
        drawerResize.observe(drawer);
        drawer.addEventListener("transitionend", setSize, true);
    }

    // 3) Falls der Content-Bereich seine Größe ändert
    if (main) {
        const mainResize = new ResizeObserver(setSize);
        mainResize.observe(main);
    }

    // Leaflet.Draw Texte
    L.drawLocal.draw.toolbar.buttons.polygon = "Start drawing a Vineyard.";

    const drawnVineyards = new L.FeatureGroup();
    vineyard_map.addLayer(drawnVineyards);

    const drawOptions = new L.Control.Draw({
        position: "topright",
        draw: {
            polyline: false,
            circle: false,
            circlemarker: false,
            marker: false,
            rectangle: false,
            polygon: {
                allowIntersection: false,
                showArea: true,
                shapeOptions: { fill: true, color: "#0000ff", fillColor: "#0000ff" },
            },
        },
        edit: { featureGroup: drawnVineyards }
    });
    vineyard_map.addControl(drawOptions);

    vineyard_map.on(L.Draw.Event.CREATED, function (event) {
        const layer = event.layer;
        let area, coords, centroidLat, centroidLng;

        if (layer instanceof L.Polygon) {
            const latLngs = layer.getLatLngs()[0];
            const areaHectares = L.GeometryUtil.geodesicArea(latLngs) / 10000;
            area = areaHectares.toFixed(2);
            coords = formatCoordinates(latLngs);

            const centroid = calculateCentroid(latLngs);
            [centroidLat, centroidLng] = centroid.split(",").map(Number);
        }

        drawnVineyards.addLayer(layer);
        dotnetObjectReference.invokeMethodAsync("ReceiveVineyardData", area, coords, [centroidLat, centroidLng]);
    });
}

// Sets the map bounds
function SetMapBounds(map) {
    const southWest = L.LatLng(-90, -180);
    const northEast = L.LatLng(90, 180);
    const bounds = L.latLngBounds(southWest, northEast);
    map.setMaxBounds(bounds);
}

// Calculate Centroid of Polygon
function calculateCentroid(latLngs) {
    const n = latLngs.length;
    if (n === 0) return null;

    let sumLat = 0, sumLng = 0;
    for (let i = 0; i < n; i++) {
        sumLat += latLngs[i].lat;
        sumLng += latLngs[i].lng;
    }
    const centroidLat = sumLat / n;
    const centroidLng = sumLng / n;
    return `${centroidLat},${centroidLng}`;
}

// Formats the coordinates
function formatCoordinates(latLngs) {
    return latLngs.map(ll => `${ll.lat},${ll.lng}`).join(";");
}

// Display the vineyards
export function LoadVineyardsFunction(vineyards) {
    if (!vineyards) return;

    for (let i = 0; i < vineyards.length; i++) {
        coordinates = vineyards[i].coordinates;
        const coordinatePairs = coordinates.split(";");

        const polygonCoords = coordinatePairs.map(pair => {
            const coords = pair.split(",");
            return [parseFloat(coords[0]), parseFloat(coords[1])];
        });

        const polygon = L.polygon(polygonCoords).addTo(vineyard_map);
        polygon.bindPopup(
            `<div class="d-flex flex-column justify-content-center">
               <p class="m-0">Name: ${vineyards[i].name}</p>
               <p class="m-0">Area: ${vineyards[i].area}ha</p>
             </div>`
        );
    }

    // sicherheitshalber neu layouten
    requestAnimationFrame(() => vineyard_map.invalidateSize());
}
