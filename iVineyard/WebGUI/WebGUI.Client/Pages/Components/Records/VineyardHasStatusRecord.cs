using Model.Entities.Bookingobjects.Vineyard;

namespace WebGUI.Client.Pages.Components.Records
{
    public record VineyardHasStatusRecord(
        int StatusId,
        DateTime StartDate,
        DateTime? EndDate,
        VineyardStatusTypeRecord VineyardStatusType
    )
    {
        // Parameterloser Konstruktor
        public VineyardHasStatusRecord() : this(
            0,               // Standardwert für StatusId
            DateTime.Now,            // Standardwert für StartDate
            null,            // Standardwert für EndDate
            new VineyardStatusTypeRecord(0, string.Empty)  // Standardwert für VineyardStatusType (mit Defaultwerten)
        )
        {
        }
    }
}