using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Models
{
    public class TeachersModel
    {
        public int Id { get; set; }
        [Display(Name = "Ім'я")]
        public string Name { get; set; }
        [Display(Name = "Дата народження")]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Спеціальність")]
        public string Speciality { get; set; }
        public int SchoolId { get; set; }

        public virtual SchoolsModel School { get; set; }
        public  ICollection<TeachingModel> TeachingModel { get; set; }
        public TeachersModel()
        {
            TeachingModel = new List<TeachingModel>();
        }
    }
}
