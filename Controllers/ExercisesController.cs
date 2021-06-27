using DataLibrary.DataAccess;
using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitApp.Controllers
{
    public class ExercisesController : Controller
    {
        private readonly DataContext _context;
        public ExercisesController(DataContext context)
        {
            _context = context;

            if (_context.exercises.Count() == 0)
            {
                string file = System.IO.File.ReadAllText("data.json");
                var exercises = JsonSerializer.Deserialize<List<Exercise>>(file);
                _context.AddRangeAsync(exercises);
                _context.SaveChangesAsync();
            }
        }

        [HttpGet]
        [Route("/exercises")]
        public async Task<ActionResult<IEnumerable<Exercise>>> Index()
        {
            return await _context.exercises.ToListAsync();
        }
        [HttpGet]
        [Route("/exercises/details")]
        public async Task<ActionResult> Details(int exerciseId)
        {
            var exercise = await _context.exercises
                .Where(e => e.id == exerciseId)
                .FirstOrDefaultAsync();

            if(exercise == null)
            {
                return BadRequest();
            }

            return Ok(exercise);
        }
    }
}