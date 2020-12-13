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

        [HttpPost]
        public IActionResult Create([FromBody]Models.CelestialObject celestialObject)
        {
            _context.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = 0 }, celestialObject); //fix test
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Models.CelestialObject celestialObject)
        {
            var existingCelestialObject = _context.CelestialObjects.Find(id);

            if (existingCelestialObject == null)
                return NotFound();
            else
            {
                existingCelestialObject.Name = celestialObject.Name;
                existingCelestialObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
                existingCelestialObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
                _context.Update(existingCelestialObject);
                _context.SaveChanges();
                return NoContent();
            }
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var existingCelestialObject = _context.CelestialObjects.Find(id);

            if (existingCelestialObject == null)
                return NotFound();
            else
            {
                existingCelestialObject.Name = name;
                _context.Update(existingCelestialObject);
                _context.SaveChanges();
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingCelestialObject = _context.CelestialObjects.Find(id);
            var satelitesOfCelestialObject = GetSatellitesOfCelestialObjectWithId(id);

            if (existingCelestialObject == null && satelitesOfCelestialObject.Count == 0)
                return NotFound();
            else
            {
                var celestialObjectsToRemove = new List<Models.CelestialObject>();
                celestialObjectsToRemove.AddRange(satelitesOfCelestialObject);
                if (existingCelestialObject != null) celestialObjectsToRemove.Add(existingCelestialObject);
                _context.RemoveRange(celestialObjectsToRemove);
                _context.SaveChanges();
                return NoContent();
            }    
        }
    }
}
