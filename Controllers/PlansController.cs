using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataLibrary.DataAccess;
using DataLibrary.Models;

namespace FitApp.Controllers
{
    [ApiController]
    public class PlansController : Controller
    {
        private readonly DataContext _context;

        public PlansController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/plans")]
        public async Task<ActionResult<IEnumerable<Plan>>> Index()
        {
            return await _context.plans.ToListAsync();
        }
        
        [HttpGet]
        [Route("/plans/getAllStatistics")]
        public async Task<ActionResult<IEnumerable<Plan>>> getAllStats(int planId)
        {
            var plan = await _context.plans
                .Include(p => p.planExercise)
                .Where(p => p.id == planId)
                .FirstOrDefaultAsync();

            if(plan == null)
            {
                return BadRequest();
            }

            List<PlanStatistics> planStatistics = new List<PlanStatistics>();

            foreach (var p in plan.planExercise)
            {
                PlanStatistics planStat = new PlanStatistics();
                planStat.exerciseStatistics = new List<ExerciseStatistic>();

                var planExercise = await _context.planExercises
                    .Include(pE => pE.exerciseStatistics)
                    .Where(pE => pE.id == p.id)
                    .FirstOrDefaultAsync();

                if (planExercise == null)
                {
                    return BadRequest();
                }

                var exercises = await _context.exercises
                    .Include(e => e.planExercise)
                    .ToListAsync();

                if (exercises == null)
                {
                    return BadRequest();
                }

                Exercise exercise = null;

                foreach (var e in exercises)
                {
                    if(e.planExercise.Contains(planExercise))
                    {
                        exercise = e;
                    }
                }

                if (exercise == null)
                {
                    return BadRequest();
                }

                planStat.exerciseName = exercise.exerciseName;
                planStat.exerciseStatistics.AddRange(planExercise.exerciseStatistics);
                planStatistics.Add(planStat);
            }
            return Ok(planStatistics);
        }

        [HttpGet]
        [Route("/plans/getPlanExercises")]
        public async Task<ActionResult> listAll(int planId)
        {
            var plan = await _context.plans
                .Include(p => p.planExercise)
                .Where(r => r.id == planId)
                .FirstOrDefaultAsync();

            if (plan == null)
            {
                return BadRequest();
            }

            List<PlanExerciseDetails> exercisesList = new List<PlanExerciseDetails>();
            
            foreach (var p in plan.planExercise)
            {
                var exercises = await _context.exercises
                    .Include(e => e.planExercise)
                    .ToListAsync();

                if (exercises == null)
                {
                    return BadRequest();
                }

                Exercise e = null;

                foreach (var exer in exercises)
                {
                    if (exer.planExercise.Contains(p))
                    {
                        e = exer;
                    }
                }

                PlanExerciseDetails details = new PlanExerciseDetails();
                details.planId = plan.id;
                details.planExerciseId = p.id;
                details.description = e.description;
                details.exerciseName = e.exerciseName;
                details.exerciseId = e.id;
                details.series = p.series;
                details.repetitions = p.repetitions;
                
                exercisesList.Add(details);
            }
            return Ok(exercisesList);
        }
        
        [HttpGet]
        [Route("/plans/userPlans")]
        public async Task<ActionResult> UserPlans(int userId)
        {
            var user =  _context.users
                .Include(u => u.plans)
                .Where(u => u.id == userId)
                .FirstOrDefault();

            if(user == null)
            {
                return BadRequest();
            }

            return Ok(user.plans);
        }

        [HttpPost]
        [Route("/plans/add")]
        public async Task<ActionResult> Add([FromBody] PlanToAdd planToAdd)
        {
            var user = _context.users
                .FirstOrDefault(u => u.id == planToAdd.userId);

            if (user == null 
                || planToAdd.Plan.description == "" 
                || planToAdd.Plan.planName == "" 
                || planToAdd.Plan.description == ""
                || planToAdd.Plan.description == null
                || planToAdd.Plan.planName == null)
            {
                return BadRequest();
            }

            user.plans.Add(planToAdd.Plan);
            await _context.SaveChangesAsync();

            return Ok(planToAdd.Plan);
        }

        [HttpDelete]
        [Route("/plans/delete")]
        public async Task<ActionResult> Delete(int planId)
        {
            var plan = await _context.plans
                .Include(p => p.planExercise)
                .Where(p => p.id == planId)
                .FirstOrDefaultAsync();

            if (plan == null)
            {
                return BadRequest();
            }

            foreach (var p in plan.planExercise)
            {
                RedirectToAction("/planExercises/delete", new {p.id});
            }

            _context.plans.Remove(plan);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        [Route("/plans/update")]
        public async Task<ActionResult> Update([FromBody] Plan plan)
        {
            if (plan.id <0 
                || plan.description == ""
                || plan.planName == ""
                || plan.description == ""
                || plan.description == null
                || plan.planName == null)
            {
                return BadRequest();
            }

            var planToChange = _context.plans
                .FirstOrDefault(p => p.id == plan.id);

            if (plan == null)
            {
                return BadRequest();
            }

            planToChange.description = plan.description;
            planToChange.planName = plan.planName;
            await _context.SaveChangesAsync();

            return Ok();
        }

        private class PlanExerciseDetails
        {
            public int planId { get; set; }
            public int planExerciseId { get; set; }
            public int series { get; set; }
            public int repetitions { get; set; }
            public int exerciseId { get; set; }
            public string exerciseName { get; set; }
            public string description { get; set; }
        }

        private class PlanStatistics
        {
            public string exerciseName { get; set; }
            public List<ExerciseStatistic> exerciseStatistics { get; set; }
        }

        public class PlanToAdd
        {
            public Plan Plan { get; set; }
            public int userId { get; set; }
        }
    }
}
