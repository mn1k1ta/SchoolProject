using Microsoft.EntityFrameworkCore;
using SchoolProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Repository
{
    public class TeachingRepository : BaseRepository<TeachingModel>
    {
        public TeachingRepository(DbContext context) : base(context)
        {

        }
    }
}
