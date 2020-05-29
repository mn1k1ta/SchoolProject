using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Models
{
    public class TeachingModel
    {
        public int Id { get; set; }
        public int? TeacherId { get; set; }
  
        public int? SubjectId { get; set; }

        public virtual SubjectsModel Subject { get; set; }
        public  TeachersModel Teacher { get; set; }
    }
}
