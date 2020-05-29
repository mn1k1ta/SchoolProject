using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Models
{
    public class ClassesModel
    {     
        public int Id { get; set; }
        public int SchoolId { get; set; }
        [Display(Name = "Клас")]
        public string Name { get; set; }
        [Display(Name = "Інформація")]
        public string Info { get; set; }     
        public virtual SchoolsModel School { get; set; }
        public virtual ICollection<StudentsModel> Students { get; set; }
        public virtual ICollection<TeachingModel> Teaching { get; set; }
    }
}
