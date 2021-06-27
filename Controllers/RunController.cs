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
    public class RunController : Controller
    {
        private readonly DataContext _context;

        public RunController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/runs")]
        public async Task<ActionResult<IEnumerable<RunScore>>> Index()
        {
            return await _context.runScores.ToListAsync();
        }

        [HttpGet]
        [Route("/runs/user")]
        public async Task<ActionResult> UserRuns(int userId)
        {
            if(userId < 0)
            {
                return BadRequest();
            }

            var user = await _context.users
                .Include(u =>  u.runScores)
                .Where(r => r.id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest();
            }

            return Ok(user.runScores);
        }

        [HttpGet]
        [Route("/runs/details")]
        public async Task<ActionResult> Details(int runScoreId)
        {
            var runScore = await _context.runScores
                .Where(r => r.id == runScoreId)
                .FirstOrDefaultAsync();

            if (runScore == null)
            {
                return BadRequest();
            }

            return Ok(runScore);
        }

        [HttpPost]
        [Route("/runs/add")]
        public async Task<ActionResult> Add([FromBody] RunRequestObj runRequestObj)
        {
            var user = await _context.users
                .Include(u => u.runScores)
                .Where(u => u.id == runRequestObj.userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest();
            }

            user.runScores.Add(runRequestObj.RunScore);
            await _context.SaveChangesAsync();

            return Ok(runRequestObj.RunScore);
        }

        [HttpDelete]
        [Route("/runs/delete")]
        public async Task<ActionResult> Delete( int runScoreId)
        {
            if(runScoreId < 0)
            {
                return BadRequest();
            }

            var run = _context.runScores
                .Where(r => r.id == runScoreId)
                .FirstOrDefault();

            if (run == null)
            {
                return BadRequest();
            }

            _context.runScores.Remove(run);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        [Route("/runs/update")]
        public async Task<ActionResult> Update([FromBody] RunRequestObj runRequestObj)
        {
            var user = await _context.users
                .Include(u => u.runScores)
                .Where(u => u.id ==runRequestObj.userId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest();
            }

            var run = user.runScores
                .Where(r => r.id == runRequestObj.RunScore.id)
                .FirstOrDefault();

            if (run == null)
            {
                return BadRequest();
            }

            run.time = runRequestObj.RunScore.time;
            await _context.SaveChangesAsync();
            run.distance = runRequestObj.RunScore.distance;
            await _context.SaveChangesAsync();

            return Ok(run);
        }
    }
    public class RunRequestObj
    {
        public RunScore RunScore { get; set; }
        public int userId { get; set; }
    }
}
