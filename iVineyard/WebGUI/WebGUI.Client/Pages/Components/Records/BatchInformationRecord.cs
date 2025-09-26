using Model.Entities.Harvest;

namespace WebGUI.Client.Pages.Components.Records;

// Root: entspricht VineyardHasBatch (+ geladene Navigationen)
public record BatchInformationRecord
{
    public VineyardRecord Vineyard { get; init; } = default!;
    public int VineyardId { get; init; }

    public BatchRecord Batch { get; init; } = default!;
    public int BatchId { get; init; }
}

// ---- Teil-Records ----

public record BatchRecord
{
    public int BatchId { get; init; }
    public string Variety { get; init; } = string.Empty;
    public string Amount { get; init; }
    public DateTime Date { get; init; }
    public string Maturity_Health { get; init; } = string.Empty;
    public string Weather { get; init; } = string.Empty;

    // Namen exakt wie in Entity belassen, damit Mapster out-of-the-box mapped
    public List<WineBatchHasTreatmentRecord>? batchHasTreatmentsList { get; init; }
    public List<InformationsRecord>? InformationsList { get; init; }
}

public record WineBatchHasTreatmentRecord
{
    public int BatchId { get; init; }
    public TreatmentRecord Treatment { get; init; } = default!;
    public int TreatementId { get; init; }   // Schreibweise wie im Entity
    public string Amount { get; init; }
    public string Agent { get; init; } = string.Empty;
    public DateTime Date { get; init; }
}

public record TreatmentRecord
{
    public int TreatmentId { get; init; }
    public string Type { get; init; } = string.Empty;
}

public record InformationsRecord
{
    public int InformationId { get; init; }
    public int BatchId { get; init; }
    public DateTime Date { get; init; }
    public double Acidity { get; init; }
    public double PhValue { get; init; }
    public string FurtherSteps { get; init; } = string.Empty;

    public StartingMustRecord? StartingMust { get; init; }
    public YoungWineRecord? YoungWine { get; init; }
    public WhiteWineRedWineRecord? WhiteWineRedWine { get; init; }
}

public record StartingMustRecord
{
    public int Id { get; init; }            // INFORMATION_ID
    public double KMW_OE { get; init; }
    public string Rebel { get; init; } = string.Empty;
    public string Squeeze { get; init; } = string.Empty;
    public string MashLife { get; init; } = string.Empty;
}

public record YoungWineRecord
{
    public int Id { get; init; }
    public double Alcohol { get; init; }
    public double ResidualSugar { get; init; }
}

public record WhiteWineRedWineRecord
{
    public int Id { get; init; }
    public double Alcohol { get; init; }
    public double ResidualSugar { get; init; }
    public double Sulfur { get; init; }
}
