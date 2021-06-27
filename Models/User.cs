using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLibrary.Models
{
    public class User
    {
        public int id { get; set; }
        [Required]
        [MaxLength(100)]
        public string email { get; set; }
        [Required]
        [MaxLength(100)]
        public string firstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string secondName { get; set; }
        [Required]
        [MaxLength(100)]
        public string password { get; set; }
        public List<RunScore> runScores { get; set; } = new List<RunScore>();
        public List<Plan> plans { get; set; } = new List<Plan>();
    }
}
