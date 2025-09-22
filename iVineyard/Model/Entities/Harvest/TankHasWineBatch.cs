namespace Model.Entities.Harvest;

public class TankHasWineBatch
{
    public Tank Tank { get; set; }
    public int TankId { get; set; }

    public Batch Batch { get; set; }
    public int BatchId { get; set; }
}