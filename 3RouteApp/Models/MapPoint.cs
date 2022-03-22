namespace _3RouteApp.Models
{
    public class MapPoint : IMapPointService
    {
        public string Name { get; set; }
        public Point Location { get; set; }
        public int calculated { get; set; }
    }
    public interface IMapPointService
    {
        public string Name { get; set; }
        public Point Location { get; set; }
        public int calculated { get; set; }
    }
}
