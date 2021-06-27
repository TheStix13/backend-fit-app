using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLibrary.Models
{
    public class ExerciseStatistic
    {
        public int id { get; set; }
        [Required]
        public int series { get; set; }
        [Required]
        public int repetitions { get; set; }
        [Required]
        public DateTime dateTime { get; set; }
    }
}
