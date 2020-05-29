using Microsoft.EntityFrameworkCore;
using SchoolProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Repository
{
    public class SchoolRepository : BaseRepository<SchoolsModel>
    {
        public SchoolRepository(DbContext context) : base(context)
        {

        }
    }
}
