using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLibrary.Models
{
    public class RunScore
    {
        public int id { get; set; }
        [Required]
        public int distance { get; set; }
        [Required]
        public int time { get; set; }
    }
}
