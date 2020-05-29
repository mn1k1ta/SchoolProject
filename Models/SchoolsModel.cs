using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Models
{
    public class SchoolsModel
    {
        public int Id { get; set; }  
        [Display(Name="Адреса")]
        public string Address { get; set; }
        [Display(Name = "Назва школи")]
        public string Name { get; set; }
        public virtual ICollection<ClassesModel> Classes { get; set; }
        public virtual ICollection<TeachersModel> Teachers { get; set; }
    }
}
