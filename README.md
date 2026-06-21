# Afval Detectie — AI API / Backend (P4AIAPI)

.NET 10 Web API met Entity Framework Core en SQL Server. Dit is het **centrale knooppunt** van
het Afval Detectie-project: het slaat zowel live-detecties als trainingsdata op en is beveiligd
met twee gescheiden API-keys. Zowel de [Frontend](../../P4LU1BackEnd) als de
[Trainingsomgeving](../../YOLOv8%20afval%20detectie) communiceren ermee via HTTP.

## Datamodel (vier tabellen)

| Tabel | Inhoud |
|---|---|
| `Detections` | Live-detecties: label, confidence, timestamp, locatie (X/Y + adres), cameraId, bounding box |
| `TrainingImages` | Trainingsafbeeldingen als blob (`byte[]`) + bestandsnaam en content-type |
| `BoundingBoxes` | Bounding boxes per trainingsafbeelding (YOLO-formaat), via `TrainingImageId` |
| `DetectionImages` | Optionele afbeelding-blob per detectie, in een **aparte** tabel zodat `Detections` klein blijft |

**Ontwerpkeuze:** trainingsafbeeldingen en hun boxes staan in twee aparte tabellen (één-op-veel),
zodat een afbeelding niet per box wordt gedupliceerd. Dezelfde gedachte zit achter
`DetectionImages`: de zware blob staat los van de lichte `Detections`-tabel.

## Endpoints

| Methode + route | Beveiliging | Doel |
|---|---|---|
| `GET /api/detection` | Monitoring-key | Alle detecties ophalen (dashboard) |
| `POST /api/detection/ai` | Training-key | AI-model post een detectie (optioneel met afbeelding) |
| `GET /api/detection/{id}/image` | Monitoring-key | Afbeelding van een detectie ophalen |
| `POST /api/trainingimage/upload` | Training-key | Trainingsafbeelding + bounding boxes opslaan |
| `GET /api/trainingimage` | Training-key | Lijst van trainingsafbeeldingen + boxes (zonder blob) |
| `GET /api/trainingimage/{id}/image` | Training-key | Eén trainingsafbeelding downloaden |

## Beveiliging: twee API-keys

Beide via de header `X-Api-Key`:

- **Training-key** (`ApiKeys:Training`) — uploaden, trainingsdata ophalen, detecties posten.
- **Monitoring-key** (`ApiKeys:Monitoring`) — alleen detecties uitlezen (dashboard).

Het `ApiKeyAttribute`-filter is geparametriseerd, bv. `[ApiKey("ApiKeys:Training")]`. Ontbreekt de
sleutel in de configuratie → **500**; verkeerde/ontbrekende header → **401**.

### Uploadbeveiliging

`POST /api/trainingimage/upload` accepteert alleen **JPG, PNG en HEIC**, maximaal **15 MB**
(zowel Content-Type als extensie worden gecontroleerd).

## Geocoding

Heeft een detectie coordinaten (`LocatieX` = latitude, `LocatieY` = longitude), dan zoekt de
`GeocodingService` via [geocode.maps.co](https://geocode.maps.co) een adres erbij, dat als
`Location` wordt opgeslagen.

## Automatische migraties & Scalar

- Bij het opstarten draait de API automatisch alle openstaande migraties (`db.Database.Migrate()`),
  zodat nieuwe tabellen vanzelf verschijnen na een deploy.
- API-documentatie via **Scalar** op `/scalar/v1` (OpenAPI-JSON op `/openapi/v1.json`). Scalar
  staat bewust buiten de Development-check, zodat het ook live werkt.

## Configuratie (Azure App Settings)

| Instelling | Betekenis |
|---|---|
| `ApiKeys__Training` | Key voor upload/training/detecties posten |
| `ApiKeys__Monitoring` | Key voor detecties uitlezen |
| `Geocoding__ApiKey` | Sleutel voor geocode.maps.co |
| `ConnectionStrings__DefaultConnection` | SQL-connection string (tabblad *Connection strings*) |

> Dubbele underscore (`__`) in Azure komt overeen met de dubbele punt (`:`) in `appsettings.json`.

## Lokaal draaien

```bash
dotnet run                              # API starten
dotnet ef migrations add NaamMigratie   # nieuwe migratie
```
