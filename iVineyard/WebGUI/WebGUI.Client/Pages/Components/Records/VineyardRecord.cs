using Model.Entities.Bookingobjects.Vineyard;

namespace WebGUI.Client.Pages.Components.Records;

public record VineyardRecord(
    int BookingObjectId,
    string Name,
    string Coordinates,
    string MidCoordinate,
    float Area,
    int CompanyId,
    string? CompanyName, // Optional
    List<VineyardHasStatusRecord> StatusList
)
{
    // Parameterloser Konstruktor
    public VineyardRecord() : this(
        0,                // Standardwert für BookingObjectId
        string.Empty,     // Standardwert für Name
        string.Empty,     // Standardwert für Coordinates
        string.Empty,     // Standardwert für MidCoordinate
        0f,               // Standardwert für Area
        0,                // Standardwert für CompanyId
        null,             // Standardwert für CompanyName
        new List<VineyardHasStatusRecord>() // Leere Liste für StatusList
    )
    {
    }
}