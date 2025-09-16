namespace Model.Entities.Bookingobjects;

public class Measurement {
    public int MEASUREMENT_ID { get; set; }
    public int WEATHER_STATION_ID { get; set; }
    public int TEMPERATURE { get; set; }
    public int HUMIDITY { get; set; }
    public int PRESSURE { get; set; }
    public int DEW_POINT { get; set; }
    public string TIME_STAMP { get; set; }
}