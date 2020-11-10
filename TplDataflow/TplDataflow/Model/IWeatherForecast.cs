using System;

namespace TplDataflow.Model
{
    public interface IWeatherForecast
    {
        DateTime Date { get; set; }
        int TemperatureC { get; set; }
        int TemperatureF { get; }
        string Summary { get; set; }
    }
}
