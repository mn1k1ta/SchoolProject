using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolProject.EFContext;

namespace SchoolProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly EFSchoolContext _context;

        public ChartsController(EFSchoolContext _context)
        {
            this._context = _context;
        }

        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var classes = _context.Classes.Include(b => b.Students).ToList();

            List<object> classsesStud = new List<object>();

            classsesStud.Add(new[] { "Класс", "Кількість студентів" });
            foreach (var item in classes)
            {
                classsesStud.Add(new object[] { item.Name, item.Students.Count() });
            }
            return new JsonResult(classsesStud);
        }
    }
}