namespace Model.Entities.Harvest;

public class TankMovement
{
    public int MovementId { get; set; }
    public Tank FromTank { get; set; }
    public int FromTakId { get; set; }

    public Tank ToTank { get; set; }
    public int ToTankId { get; set; }

    public DateTime Date { get; set; }
    public double Volume { get; set; }
}