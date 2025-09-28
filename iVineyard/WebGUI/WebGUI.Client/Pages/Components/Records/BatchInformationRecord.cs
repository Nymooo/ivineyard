namespace WebGUI.Client.Pages.Components.Records;

// Root
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

    // unverändert
    public List<WineBatchHasTreatmentRecord>? batchHasTreatmentsList { get; init; }
    public List<InformationsRecord>? InformationsList { get; init; }

    // *** NEU: Tanks dieses Batches (Link-Tabelle) ***
    public List<TankHasWineBatchRecord>? TankList { get; init; }

    // *** NEU: Bewegungen, flach ohne Backrefs (per Zusatz-Query befüllen) ***
    public List<TankMovementRecord>? TankMovements { get; init; }
}

public record WineBatchHasTreatmentRecord
{
    public int BatchId { get; init; }
    public TreatmentRecord Treatment { get; init; } = default!;
    public int TreatementId { get; init; }
    public string Agent { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public string Amount { get; init; } = string.Empty; // falls string in DB
}

public record TreatmentRecord
{
    public int TreatmentId { get; init; }
    public string Type { get; init; } = string.Empty;
    
    public string Notes { get; init; } = string.Empty;
}

public record InformationsRecord
{
    public int InformationId { get; init; }
    public int BatchId { get; init; }
    public DateTime Date { get; init; }
    public string Acidity { get; init; }
    public double PhValue { get; init; }
    public string FurtherSteps { get; init; } = string.Empty;

    public StartingMustRecord? StartingMust { get; init; }
    public YoungWineRecord?   YoungWine { get; init; }
    public WhiteWineRedWineRecord? WhiteWineRedWine { get; init; }
}

public record StartingMustRecord
{
    public int Id { get; init; }          // INFORMATION_ID
    public double KMW_OE { get; init; }
    public string Rebel { get; init; } = string.Empty;
    public string Squeeze { get; init; } = string.Empty;
    public string MashLife { get; init; } = string.Empty;
    
    
}

public record YoungWineRecord
{
    public int Id { get; init; }
    public double Alcohol { get; init; }
    public string ResidualSugar { get; init; }
}

public record WhiteWineRedWineRecord
{
    public int Id { get; init; }
    public double Alcohol { get; init; }
    public string ResidualSugar { get; init; }
    public string Sulfur { get; init; }
}

// ---- Tanks & Bewegungen (zyklusfrei) ----

public record TankHasWineBatchRecord
{
    public int TankId { get; init; }
    public int BatchId { get; init; }
    public TankRecord Tank { get; init; } = default!; // nur Tank; KEIN Batch ⇒ kein Zyklus
}

public record TankRecord
{
    public int TankId { get; init; }
    public string Name { get; init; } = string.Empty;
}

public record TankMovementRecord
{
    public int MovementId { get; init; }
    public int FromTakId { get; init; }   // Schreibweise wie im Entity!
    public int ToTankId  { get; init; }
    public DateTime Date { get; init; }
    public double Volume { get; init; }

    // optional zur Anzeige; keine Backrefs
    public TankRecord? FromTank { get; init; }
    public TankRecord? ToTank   { get; init; }
}
