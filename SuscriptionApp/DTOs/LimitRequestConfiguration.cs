using System.Reflection.Metadata.Ecma335;

namespace SuscriptionApp.DTOs
{
    public class LimitRequestConfiguration
    {
        public int RequestsByDayForFree { get; set; }
        public string[] WhiteListRoutes { get; set; }
    }
}
