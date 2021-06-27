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
    public class PlanExerciseController : Controller
    {
        private readonly DataContext _context;

        public PlanExerciseController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/planExercises")]
        public async Task<ActionResult<IEnumerable<PlanExercise>>> Index()
        {
            return await _context.planExercises.ToListAsync();
        }

        [HttpGet]
        [Route("/planExercises/details")]
        public async Task<ActionResult> Details(int planExerciseId)
        {
            var planExercise = await _context.planExercises
                .Where(r => r.id == planExerciseId)
                .FirstOrDefaultAsync();

            if (planExercise == null)
            {
                return BadRequest();
            }

            var exercises = await _context.exercises
                .Include(e => e.planExercise)
                .ToListAsync();

            if(exercises == null)
            {
                return BadRequest();
            }

            Exercise e = null;

            foreach (var exer in exercises)
            {
                if(exer.planExercise.Contains(planExercise))
                {
                    e = exer;
                }
            }

            PlanExerciseDetails details = new PlanExerciseDetails();
            details.planExerciseid = planExercise.id;
            details.description = e.description;
            details.exerciseName = e.exerciseName;
            details.exerciseId = e.id;
            details.series = planExercise.series;
            details.repetitions = planExercise.repetitions;

            return Ok(details);
        }

        [HttpPost]
        [Route("/planExercises/add")]
        public async Task<ActionResult> Add([FromBody] PlanExerciseToAdd planExerciseToAdd)
        {
            if(planExerciseToAdd.planId < 0
                || planExerciseToAdd.planExercise.repetitions < 1
                || planExerciseToAdd.planExercise.series < 1)
            {
                return BadRequest();
            }

            var plan = await _context.plans
                .Include(u => u.planExercise)
                .Where(u => u.id == planExerciseToAdd.planId)
                .FirstOrDefaultAsync();

            if (plan == null)
            {
                return BadRequest();
            }

            var exercise = await _context.exercises
                .Include(e => e.planExercise)
                .Where(e => e.id == planExerciseToAdd.exerciseId)
                .FirstOrDefaultAsync();

            if (exercise == null)
            {
                return BadRequest();
            }

            PlanExercise planExercise = new PlanExercise();
            planExercise.repetitions = planExerciseToAdd.planExercise.repetitions;
            planExercise.series = planExerciseToAdd.planExercise.series;

            PlanExerciseDetails details = new PlanExerciseDetails();
            details.planExerciseid = planExercise.id;
            details.description = exercise.description;
            details.exerciseName = exercise.exerciseName;
            details.exerciseId = exercise.id;
            details.series = planExerciseToAdd.planExercise.series;
            details.repetitions = planExerciseToAdd.planExercise.repetitions;

            exercise.planExercise.Add(planExercise);
            await _context.SaveChangesAsync();
            plan.planExercise.Add(planExercise);
            await _context.SaveChangesAsync();
            details.planExerciseid = planExercise.id;

            return Ok(details);
        }

        [HttpDelete]
        [Route("/planExercises/delete")]
        public async Task<ActionResult> Delete(int planExerciseId)
        {
            var planExercise = await _context.planExercises
                .Include(p => p.exerciseStatistics)
                .Where(u => u.id == planExerciseId)
                .FirstOrDefaultAsync();

            if (planExercise == null)
            {
                return BadRequest();
            }

            planExercise.exerciseStatistics.Clear();
            _context.planExercises.Remove(planExercise);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        [Route("/planExercises/update")]
        public async Task<ActionResult> Update([FromBody] PlanExerciseToAdd planExerciseToUpdate)
        {
            if (planExerciseToUpdate.planExercise.repetitions < 1
                || planExerciseToUpdate.planExercise.series < 1
                || planExerciseToUpdate.planExercise.id < 1
                || planExerciseToUpdate.exerciseId < 1)
            {
                return BadRequest();
            }

            var planExercise = await _context.planExercises
                .Where(u => u.id == planExerciseToUpdate.planExercise.id)
                .FirstOrDefaultAsync();

            var exercises = await _context.exercises
                .Include(e => e.planExercise)
                .ToListAsync();

            if (planExercise == null)
            {
                return BadRequest();
            }

            if (exercises == null)
            {
                return BadRequest();
            }

            foreach (var exer in exercises)
            {
                if(exer.planExercise.Contains(planExercise))
                {
                    exer.planExercise.Remove(planExercise);
                }
            }

            var e = await _context.exercises
                .Include(e => e.planExercise)
                .Where(e => e.id == planExerciseToUpdate.exerciseId)
                .FirstOrDefaultAsync();

            if (e == null)
            {
                return BadRequest();
            }

            planExercise.repetitions = planExerciseToUpdate.planExercise.repetitions;
            planExercise.series = planExerciseToUpdate.planExercise.series;
            e.planExercise.Add(planExercise);
            await _context.SaveChangesAsync();

            PlanExerciseDetails details = new PlanExerciseDetails();
            details.planExerciseid = planExercise.id;
            details.description = e.description;
            details.exerciseName = e.exerciseName;
            details.exerciseId = e.id;
            details.series = planExercise.series;
            details.repetitions = planExercise.repetitions;

            return Ok(details);
        }

        [HttpGet]
        [Route("/planExercises/statistics")]
        public async Task<ActionResult> getStatistic(int planExerciseId)
        {
            var planExercise = _context.planExercises
                .Include(p => p.exerciseStatistics)
                .Where(p => p.id == planExerciseId)
                .FirstOrDefault();

            return Ok(planExercise.exerciseStatistics);
        }

        private class PlanExerciseDetails
        {
            public int planExerciseid { get; set; }
            public int series { get; set; }
            public int repetitions { get; set; }
            public int exerciseId { get; set; }
            public string exerciseName { get; set; }
            public string description { get; set; }
        }

        public class PlanExerciseToAdd
        {
            public PlanExercise planExercise { get; set; }
            public int planId { get; set; }
            public int exerciseId { get; set; }
        }
    }
}
