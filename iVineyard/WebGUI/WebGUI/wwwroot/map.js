let map;
let vineyard_map;
let coordinates;

export function MapFunction(dotnetObjectReference) {
    map = L.map("dashboardMap", { center: new L.LatLng(48.361528, 15.370033), zoom: 14, zoomControl: false });
    /* Set Boundaries of Map */
    SetMapBounds(map);
    
    L.tileLayer('http://{s}.google.com/vt?lyrs=s,h&x={x}&y={y}&z={z}', {
        maxZoom: 20,
        minZoom: 2,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3'],
        attribution: "Google Tile Layers",
        detectRetina: true
    }).addTo(map);
    
}
//VineyardsMap
export function MapFunctionVineyards(dotnetObjectReference) {
    let vineyard_map_div = document.getElementById("vineyardMap");
    let appbarHeight = document.getElementsByClassName("mud-appbar")[0].offsetHeight;
    let appbarWidth = document.getElementsByClassName("mud-drawer")[0].offsetWidth;
    vineyard_map_div.style.height = `calc(100vh - ${appbarHeight}px)`;
    vineyard_map_div.style.width = `calc(100vw - ${appbarWidth}px)`;

    vineyard_map = L.map("vineyardMap", { center: new L.LatLng(48.361528, 15.370033), zoom: 14, zoomControl: false })
    /* Set Boundaries of Map */
    SetMapBounds(vineyard_map);

    
    L.tileLayer('http://{s}.google.com/vt?lyrs=s,h&x={x}&y={y}&z={z}', {
            maxZoom: 20,
            minZoom: 2,
            subdomains: ['mt0', 'mt1', 'mt2', 'mt3'],
            attribution: "Google Tile Layers",
            detectRetina: true
        }).addTo(vineyard_map);
        
        //drawer
        L.drawLocal.draw.toolbar.buttons.polygon = 'Start drawing a Vineyard.';
        
            let drawnVineyards = new L.FeatureGroup();
            vineyard_map.addLayer(drawnVineyards);
        
            let drawOptions = new L.Control.Draw({
                position: 'topright',
                draw: {
                    polyline: false,
                    circle: false,
                    circlemarker: false,
                    marker: false,
                    rectangle: false,
                    polygon: {
                        allowIntersection: false, 
                        showArea: true,
                        shapeOptions: {
                            fill: true,
                            color: "#0000ff",
                            fillColor: "#0000ff",
                        },
                    },
                },
                edit: {
                    featureGroup: drawnVineyards
                }
            });
            vineyard_map.addControl(drawOptions);
            
            var result = vineyard_map.on(L.Draw.Event.CREATED, function (event) {
                    var layer = event.layer; // Form that got drawn
                    let area;
                    let coordinates;
                    if (layer instanceof L.Polygon) {
                        var latLngs = layer.getLatLngs()[0];
                        var areaHectares = L.GeometryUtil.geodesicArea(latLngs) / 10000;
                        console.log("Area: " + areaHectares.toFixed(2) + " ha")
                        area = areaHectares.toFixed(2)
                        console.log("Coordinates: " + formatCoordinates(latLngs))
                        coordinates = formatCoordinates(latLngs);

                        // Calculate centroid
                        console.log("Centroid of Polygon: " + calculateCentroid(latLngs));

                        // Split the centroid string to get lat and lng
                        var [centroidLat, centroidLng] = calculateCentroid(latLngs).split(',').map(Number);
                        
                        
                        console.log("COORDINATE" + [centroidLat, centroidLng])
                        console.log("GeoJSON: " + layer.toGeoJSON())
                    }
                    
                    drawnVineyards.addLayer(layer);
                    
                    dotnetObjectReference.invokeMethodAsync('ReceiveVineyardData', area, coordinates,[centroidLat, centroidLng]);

                });
                
}



// Sets the map bounds
function SetMapBounds(map) {
    var southWest = L.LatLng(-90, -180);
    var northEast = L.LatLng(90, 180);
    var bounds = L.latLngBounds(southWest, northEast);
    map.setMaxBounds(bounds);
}

// Calculate Centroid (Middle Point) of Polygon (Vineyard)
function calculateCentroid(latLngs) {
    const n = latLngs.length;
    if (n === 0) {
        return null; // No vertices to calculate the centroid
    }

    let sumLat = 0;
    let sumLng = 0;

    for (let i = 0; i < n; i++) {
        sumLat += latLngs[i].lat; // Extract latitude
        sumLng += latLngs[i].lng; // Extract longitude
    }

    const centroidLat = sumLat / n; // Average latitude
    const centroidLng = sumLng / n; // Average longitude
    return `${centroidLat},${centroidLng}`; // Return as string
}

// Formats the Coordinates in the needed format
function formatCoordinates(latLngs) {
    return latLngs.map(function (latlng) {
        return latlng.lat + ',' + latlng.lng;
    }).join(';');
}
//Display the Weingärten
export function LoadVineyardsFunction(vineyards) {
    // Looping over the vineyards in order to visualize them
    if(vineyards!=null){
        for (let i = 0; i < vineyards.length; i++) {
            coordinates = vineyards[i].coordinates;
            let coordinatePairs = coordinates.split(";");
            
    
            // Convert each coordinate pair to an array of latitude and longitude
            let polygonCoords = coordinatePairs.map(function(pair) {
                let coords = pair.split(",");
                return [parseFloat(coords[0]), parseFloat(coords[1])];
            });
    
            // Create the polygon and add it to the map
            var polygon = L.polygon(polygonCoords).addTo(vineyard_map);
    
            polygon.bindPopup(`<div class="d-flex flex-column justify-content-center"><p class="m-0">Name: ${vineyards[i].name}</p><p class="m-0" >Area: ${vineyards[i].area}ha</p></div>`).openPopup();
        }
    }
}