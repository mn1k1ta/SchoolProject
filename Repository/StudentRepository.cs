using Microsoft.EntityFrameworkCore;
using SchoolProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Repository
{
    public class StudentRepository : BaseRepository<StudentsModel>
    {
        public StudentRepository(DbContext context) : base(context)
        {

        }
    }
}
