using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLibrary.Models
{
    public class Exercise
    {
        public int id { get; set; }
        [Required]
        [MaxLength(32)]
        public string exerciseName { get; set; }
        [Required]
        public string description { get; set; }
        public List<PlanExercise> planExercise { get; set; } = new List<PlanExercise>();
    }
}
