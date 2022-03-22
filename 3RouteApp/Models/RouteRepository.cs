using GeoCoordinatePortable;
using System.Collections.Generic;
using System.Net.Http;
using Geocoding.Google;
using Newtonsoft.Json.Linq;

namespace _3RouteApp.Models
{
    public class RouteRepository : IRouteRepository
    {
        //private readonly string _conn;
        //public RouteRepository(string conn)
        //{
        //    _conn = conn;
        //}
        public List<MapPoint> GetGeoCoordinatesfromAddress(IEnumerable<AddressModel> addresses)
        {
            var returnList = new List<MapPoint>();
            int addressCount = 1;
            string addressNumber;
            string streetName;
            string streetType;
            string city;
            foreach (var address in addresses)
            {
                addressNumber = address.Street.Split(" ")[0];
                streetName = address.Street.Split(" ")[1];
                streetType = address.Street.Split(" ")[2];
                city = address.City;
                var addressClient = new HttpClient();
                var apiCall = $"http://nominatim.openstreetmap.org/search?q={addressNumber}+{streetName}+{streetType},+{city}&format=json&polygon=1&addressdetails=1";
                //api call needs to be parsed
                var apiResponse = addressClient.GetStringAsync(apiCall).Result;
                var lon = double.Parse(JObject.Parse(apiResponse)["lon"].ToString());
                var lat = double.Parse(JObject.Parse(apiResponse)["lat"].ToString());
                var point = new Point() { Latitude = lon, Longitude = lat };
                var place = new MapPoint() { Name = streetName + addressCount, Location = point };
                returnList.Add(place);
            }
            return returnList;
        }
        public List<MapPoint> CalculateRoute(IEnumerable<MapPoint> mapPointList)
        {
            var locA = new GeoCoordinate();
            var locB = new GeoCoordinate();
            var locC = new GeoCoordinate();
            int counter = 0;
            foreach (var mapPoint in mapPointList)
            {
                switch (counter)
                {
                    case 0:
                        locA.Latitude = mapPoint.Location.Latitude;
                        locA.Longitude = mapPoint.Location.Longitude;
                        break;
                    case 1:
                        locB.Latitude = mapPoint.Location.Latitude;
                        locB.Longitude = mapPoint.Location.Longitude;
                        break;
                    case 2:
                        locC.Latitude = mapPoint.Location.Latitude;
                        locC.Longitude = mapPoint.Location.Longitude;
                        break;
                    default:
                        break ;
                }
                counter++;
            }

            var distanceAtoB = locA.GetDistanceTo(locB);
            var distanceAtoC = locA.GetDistanceTo(locC);
            var distanceBtoC = locB.GetDistanceTo(locC);
            var distanceCtoB = locC.GetDistanceTo(locB);

            var startPoint = new MapPoint();
            var middlePoint = new MapPoint();
            var endPoint = new MapPoint();

            if (distanceAtoB < distanceAtoC)
            {
                //a to b (which is shorter than a to c) + b to c is shorter than a to c + c to b
                if (distanceAtoB + distanceBtoC < distanceAtoC + distanceCtoB)
                {
                    var point1 = new Point() { Latitude = locA.Latitude, Longitude = locA.Longitude };
                    var point2 = new Point() { Latitude = locB.Latitude, Longitude = locB.Longitude };
                    var point3 = new Point() { Latitude = locC.Latitude, Longitude = locC.Longitude };
                    startPoint.Location = point1;
                    middlePoint.Location = point2;
                    endPoint.Location = point3;
                }
                else
                {
                    var point1 = new Point() { Latitude = locA.Latitude, Longitude = locA.Longitude };
                    var point2 = new Point() { Latitude = locC.Latitude, Longitude = locC.Longitude };
                    var point3 = new Point() { Latitude = locB.Latitude, Longitude = locB.Longitude };
                    startPoint.Location = point1;
                    middlePoint.Location = point2;
                    endPoint.Location = point3;
                }
            }
            else //shorter from a to c than a to b
            {
                if (distanceAtoC + distanceCtoB < distanceAtoB + distanceBtoC)
                {
                    var point1 = new Point() { Latitude = locA.Latitude, Longitude = locA.Longitude };
                    var point2 = new Point() { Latitude = locC.Latitude, Longitude = locC.Longitude };
                    var point3 = new Point() { Latitude = locB.Latitude, Longitude = locB.Longitude };
                    startPoint.Location = point1;
                    middlePoint.Location = point2;
                    endPoint.Location = point3;
                }
                else
                {
                    var point1 = new Point() { Latitude = locA.Latitude, Longitude = locA.Longitude };
                    var point2 = new Point() { Latitude = locB.Latitude, Longitude = locB.Longitude };
                    var point3 = new Point() { Latitude = locC.Latitude, Longitude = locC.Longitude };
                    startPoint.Location = point1;
                    middlePoint.Location = point2;
                    endPoint.Location = point3;
                }
            }

            var routelist = new List<MapPoint>() { startPoint, middlePoint, endPoint };
            return routelist;
        }
    }
    public interface IRouteRepository
    {
        public List<MapPoint> GetGeoCoordinatesfromAddress(IEnumerable<AddressModel> addresses);
        public List<MapPoint> CalculateRoute(IEnumerable<MapPoint> mapPointList);
    }
}
