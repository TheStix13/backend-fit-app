using DataLibrary.DataAccess;
using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitApp.Controllers
{
    [ApiController]
    public class ExerciseStatisticController : Controller
    {
        private readonly DataContext _context;

        public ExerciseStatisticController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/exerciseStatistics")]
        public async Task<ActionResult<IEnumerable<ExerciseStatistic>>> Index()
        {
            return await _context.exerciseStatistics.ToListAsync();
        }

        [HttpGet]
        [Route("/exerciseStatistics/details")]
        public async Task<ActionResult> Details(int exerciseStatisticId)
        {
            var exerciseStatistics = await _context.exerciseStatistics
                .Where(e => e.id == exerciseStatisticId)
                .FirstOrDefaultAsync();

            if(exerciseStatistics == null)
            {
                return BadRequest();
            }

            return Ok(exerciseStatistics);
        }

        [HttpPost]
        [Route("/exerciseStatistics/add")]
        public async Task<ActionResult> Add([FromBody] ExerciseStatisticToAdd exerciseStatisticToAdd)
        {
            if (exerciseStatisticToAdd.planExerciseId < 1
                || exerciseStatisticToAdd.exerciseStatistic.repetitions < 1
                || exerciseStatisticToAdd.exerciseStatistic.series < 1)
            {
                return BadRequest();
            }

            var planExercise = await _context.planExercises
                .Where(u => u.id == exerciseStatisticToAdd.planExerciseId)
                .FirstOrDefaultAsync();

            if (planExercise == null)
            {
                return BadRequest();
            }

            var exercise = await _context.exercises
                .Include(e => e.planExercise)
                .Where(e => e.planExercise.Contains(planExercise))
                .FirstOrDefaultAsync();

            if (exercise == null)
            {
                return BadRequest();
            }

            ExerciseStatistic stats = new ExerciseStatistic();
            stats.dateTime = DateTime.Now;
            stats.repetitions = exerciseStatisticToAdd.exerciseStatistic.repetitions;
            stats.series = exerciseStatisticToAdd.exerciseStatistic.series;

            planExercise.exerciseStatistics.Add(stats);
            await _context.SaveChangesAsync();

            return Ok(stats);
        }

        [HttpDelete]
        [Route("/exerciseStatistics/delete")]
        public async Task<ActionResult> Delete(int exerciseStatisticId)
        {
            if (exerciseStatisticId < 1) return BadRequest();

            var exerciseStatistic = await _context.exerciseStatistics
                .Where(u => u.id == exerciseStatisticId)
                .FirstOrDefaultAsync();

            if (exerciseStatistic == null)
            {
                return BadRequest();
            }

            _context.exerciseStatistics.Remove(exerciseStatistic);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        [Route("/exerciseStatistics/update")]
        public async Task<ActionResult> Update([FromBody] ExerciseStatisticToAdd exerciseStatisticToUpdate)
        {
            if(exerciseStatisticToUpdate.exerciseStatistic.id < 1
                || exerciseStatisticToUpdate.exerciseStatistic.repetitions <1
                || exerciseStatisticToUpdate.exerciseStatistic.series <1)
            {
                return BadRequest();
            }
            var exerciseStatistic = await _context.exerciseStatistics
                .Where(u => u.id == exerciseStatisticToUpdate.exerciseStatistic.id)
                .FirstOrDefaultAsync();

            if (exerciseStatistic == null)
            {
                return BadRequest();
            }

            exerciseStatistic.repetitions = exerciseStatisticToUpdate.exerciseStatistic.repetitions;
            exerciseStatistic.series = exerciseStatisticToUpdate.exerciseStatistic.series;
            await _context.SaveChangesAsync();

            return Ok(exerciseStatistic);
        }
    }

    public class ExerciseStatisticToAdd
    {
        public ExerciseStatistic exerciseStatistic { get; set; }
        public int planExerciseId { get; set; }
    }
}
