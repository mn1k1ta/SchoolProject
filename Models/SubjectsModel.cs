using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Models
{
    public class SubjectsModel
    {
        public int Id { get; set; }
        [Display(Name = "Назва")]
        public string Name { get; set; }
        [Display(Name = "Інформація")]
        public string Info { get; set; }

        public virtual ICollection<TeachingModel> Teaching { get; set; }
    }
}
