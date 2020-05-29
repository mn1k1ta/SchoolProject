using Microsoft.EntityFrameworkCore;
using SchoolProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Repository
{
    public class SubjectRepository:BaseRepository<SubjectsModel>
    {
        public SubjectRepository(DbContext context) : base(context)
        {

        }
    }
}
