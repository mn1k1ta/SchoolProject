using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolProject.EFContext;
using SchoolProject.Models;

namespace SchoolProject.Controllers
{
    public class TeachersController : Controller
    {
        private readonly EFSchoolContext _context;

        public TeachersController(EFSchoolContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Schools");
            }
            ViewBag.SchoolId = id;
            ViewBag.SchoolName = name;
            var  teachers= _context.Teachers.Where(u => u.SchoolId == id).Include(u => u.School);
            return View(await teachers.ToListAsync());
        }

     
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teachersModel = await _context.Teachers
                .Include(t => t.School)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teachersModel == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Teaching", new { id = teachersModel.Id, name = teachersModel.Name });
        }

       
        public IActionResult Create(int? schoolId)
        {
            ViewBag.SchoolId = schoolId;
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int SchoolId, [Bind("Id,Name,DateOfBirth,Speciality,SchoolId")] TeachersModel teachersModel)
        {
            if (ModelState.IsValid)
            {
                teachersModel.SchoolId = SchoolId;
                _context.Add(teachersModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teachersModel);
        }

       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teachersModel = await _context.Teachers.FindAsync(id);
            if (teachersModel == null)
            {
                return NotFound();
            }
            ViewData["SchoolId"] = new SelectList(_context.Schools, "Id", "Id", teachersModel.SchoolId);
            return View(teachersModel);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DateOfBirth,Speciality,SchoolId")] TeachersModel teachersModel)
        {
            if (id != teachersModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teachersModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeachersModelExists(teachersModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SchoolId"] = new SelectList(_context.Schools, "Id", "Id", teachersModel.SchoolId);
            return View(teachersModel);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teachersModel = await _context.Teachers
                .Include(t => t.School)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teachersModel == null)
            {
                return NotFound();
            }

            return View(teachersModel);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teachersModel = await _context.Teachers.FindAsync(id);
            _context.Teachers.Remove(teachersModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeachersModelExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }

        public ActionResult Export(int? id)
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var school = _context.Schools.Where(s => s.Id == id).FirstOrDefault();
                
                var teachers = _context.Teachers.Where(c => c.SchoolId == id).ToList();
                foreach (var item in teachers)
                {
                    var worksheet = workbook.Worksheets.Add(item.Name);
                    worksheet.Cell("A1").Value = "Ім'я та прізвище";
                    worksheet.Cell("B1").Value = "Дата народження";
                    worksheet.Cell("C1").Value = "Клас";
                    worksheet.Cell("D1").Value = "Предмет";
                    worksheet.Row(1).Style.Font.Bold = true;

                    var teaching = _context.Teaching.Where(t => t.Teacher == item).ToList();
                    for (int i = 0; i < teaching.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = teaching[i].Teacher.Name;
                        worksheet.Cell(i + 2, 2).Value = teaching[i].Teacher.DateOfBirth;
                       // var className = _context.Classes.Where(c => c.Id == teaching[i].Teacher.C).FirstOrDefault();
                       // worksheet.Cell(i + 2, 3).Value = className.Name;
                        var subjName = _context.Subjects.Where(c => c.Id == teaching[i].SubjectId).FirstOrDefault();
                        worksheet.Cell(i + 2, 4).Value = subjName.Name;
                    }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"{school.Name}_library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }
    }
}
