using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CelestialObjectController(ApplicationDbContext context) => _context = context;

        private List<Models.CelestialObject> GetSatellitesOfCelestialObjectWithId(int id) => _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();

        [HttpGet("{id:int}")]
        //[ActionName("GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null)
                return NotFound();
            else
            {
                celestialObject.Satellites = GetSatellitesOfCelestialObjectWithId(id);
                return Ok(celestialObject);
            }
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name == name).ToList();
            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = GetSatellitesOfCelestialObjectWithId(celestialObject.Id);
            }
            return celestialObjects.Count > 0 ? Ok(celestialObjects) : (IActionResult)NotFound();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = GetSatellitesOfCelestialObjectWithId(celestialObject.Id);
            }
            return celestialObjects.Count > 0 ? Ok(celestialObjects) : (IActionResult)NotFound();
        }
    }
}
