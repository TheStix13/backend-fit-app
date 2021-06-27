using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLibrary.Models
{
    public class Plan
    {
        public int id { get; set; }
        [Required]
        [MaxLength(32)]
        public String planName  { get; set; }
        [Required]
        public String description { get; set; }
        public List<PlanExercise> planExercise { get; set; } = new List<PlanExercise>();
    }
}
