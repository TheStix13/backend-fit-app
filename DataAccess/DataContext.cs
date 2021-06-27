using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.DataAccess
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }
        public DbSet<User> users { get; set; }
        public DbSet<Exercise> exercises { get; set; }
        public DbSet<ExerciseStatistic> exerciseStatistics { get; set; }
        public DbSet<Plan> plans { get; set; }
        public DbSet<RunScore> runScores { get; set; }
        public DbSet<PlanExercise> planExercises { get; set; }
    }
}
