namespace WebGUI.Client.Pages.Components.Records;

public record MachineRecord(
    int BookingObjectId,
    string Name,
    BookingObjectRecord BookingObject,
    List<MachineHasStatusRecord> MachineHasStatusList,
    List<WorkInformationRecord> WorkInformation
)
{
    // Parameterloser Konstruktor
    public MachineRecord() : this(
        0,                // Standardwert für BookingObjectId
        string.Empty,     // Standardwert für Name
        new BookingObjectRecord(),
        new List<MachineHasStatusRecord>(), // Leere Liste für MachineStatuses
        new List<WorkInformationRecord>()  // Leere Liste für WorkInformation
    )
    {
    }
}

public record BookingObjectRecord(
    int Id,
    List<InvoiceRecord> Invoice
)
{
    // Parameterloser Konstruktor
    public BookingObjectRecord() : this(
        0,                  // Standardwert für Id
        new List<InvoiceRecord>() // Leere Liste für Invoices
    )
    {
    }
}

public record InvoiceRecord(
    int Id,
    double Price,
    DateTime BoughAt,
    string Description
)
{
    // Parameterloser Konstruktor
    public InvoiceRecord() : this(
        0,               // Standardwert für InvoiceId
        0,                // Standardwert für Price
        DateTime.MinValue, // Standardwert für BoughtAt
        ""
    )
    {
    }
}

public record MachineHasStatusRecord(
    int StatusId,
    DateTime StartDate,
    DateTime? EndDate,
    MachineStatusTypeRecord? MachineStatusType
)
{
    // Parameterloser Konstruktor
    public MachineHasStatusRecord() : this(
        0,                // Standardwert für StatusId
        DateTime.MinValue, // Standardwert für StartDate
        null,             // Standardwert für EndDate
        new MachineStatusTypeRecord() // Standardwert für Status
    )
    {
    }
}

public record MachineStatusTypeRecord(
    int StatusId,
    string Type
)
{
    // Parameterloser Konstruktor
    public MachineStatusTypeRecord() : this(
        0,               // Standardwert für StatusId
        string.Empty     // Standardwert für Type
    )
    {
    }
    public string Type { get; set; } = string.Empty;
}

public record WorkInformationRecord(
    string UserId,
    int VineyardId,
    int? MachineId,
    DateTime? StartedAt,
    DateTime? FinishedAt,
    ApplicationUserRecord ApplicationUser // Hinzufügen der ApplicationUser-Informationen
)
{
    // Parameterloser Konstruktor
    public WorkInformationRecord() : this(
        string.Empty,     // Standardwert für UserId
        0,                // Standardwert für VineyardId
        null,             // Standardwert für MachineId
        null,             // Standardwert für StartedAt
        null,             // Standardwert für FinishedAt
        new ApplicationUserRecord() // Standardwert für ApplicationUser
    )
    {
    }
}

public record ApplicationUserRecord(
    string UserName,
    string Email,
    double Salary // Beispiel für eine Eigenschaft aus ApplicationUser
)
{
    // Parameterloser Konstruktor
    public ApplicationUserRecord() : this(
        string.Empty,  // Standardwert für UserName
        string.Empty,  // Standardwert für Email
        0              // Standardwert für Salary
    )
    {
    }
}
