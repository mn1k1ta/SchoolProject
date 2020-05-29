
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolProject.Models;

namespace SchoolProject.EFContext
{
    public class EFSchoolContext : IdentityDbContext<UserProfileModel>
    {
        public EFSchoolContext(DbContextOptions<EFSchoolContext> options)
            : base(options)
        {

        }

        public DbSet<ClassesModel> Classes { get; set; }
        public DbSet<SchoolsModel> Schools { get; set; }
        public DbSet<StudentsModel> Students { get; set; }
        public DbSet<SubjectsModel> Subjects { get; set; }
        public DbSet<TeachersModel> Teachers { get; set; }
        public DbSet<TeachingModel> Teaching { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
    


        }

    }
}
