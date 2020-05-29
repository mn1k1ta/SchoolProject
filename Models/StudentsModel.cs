using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Models
{
    public class StudentsModel
    {
        public int Id { get; set; }
        [Display(Name = "Ім'я")]
        public string Name { get; set; }
        public int ClassId { get; set; }
        [Display(Name = "Дата народження")]
        public DateTime DateOfBirth { get; set; }
        public virtual ClassesModel Class { get; set; }
    }
}
