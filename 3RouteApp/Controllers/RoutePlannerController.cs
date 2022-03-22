using _3RouteApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace _3RouteApp.Controllers
{
    public class RoutePlannerController : Controller
    {
        private IRouteRepository _routeRepository;
        public RoutePlannerController(IRouteRepository routeRepository)
        {
            _routeRepository = routeRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetRoute(IEnumerable<AddressModel> addresses)
        {
            if (addresses == null)
            {
                return View("Error");
            }
            try
            {
                var addressesWithCoordinates = _routeRepository.GetGeoCoordinatesfromAddress(addresses);
                var calculatedroute = _routeRepository.CalculateRoute(addressesWithCoordinates);
                return View(calculatedroute);
            }
            catch (System.Exception)
            {
                return RedirectToAction("Index", "RoutePlanner");
            }
        }
    }
}
