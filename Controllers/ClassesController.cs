using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolProject.EFContext;
using SchoolProject.Models;

namespace SchoolProject.Controllers
{
    public class ClassesController : Controller
    {
        private readonly EFSchoolContext _context;

        public ClassesController(EFSchoolContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(int? id, string? name) { 
            if(id==null)
            {
                return RedirectToAction("Index", "Schools");
            }
            ViewBag.SchoolId = id;
            ViewBag.SchoolName = name;
            var classes = _context.Classes.Where(u => u.SchoolId == id).Include(u => u.School);
            return View(await classes.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classesModel = await _context.Classes
                .Include(c => c.School)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classesModel == null)
            {
                return NotFound();
            }

            return RedirectToAction("GetStudentsByClasses", "Students", new { id = classesModel.Id, name = classesModel.Name });
        }

        public IActionResult Create(int? schoolId)
        {
            ViewBag.SchoolId = schoolId;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int SchoolId, [Bind("Id,Name,Info")] ClassesModel classesModel)
        {
            if (ModelState.IsValid)
            {
                classesModel.SchoolId = SchoolId;
                _context.Add(classesModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(classesModel);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classesModel = await _context.Classes.FindAsync(id);
            if (classesModel == null)
            {
                return NotFound();
            }
            ViewData["SchoolId"] = new SelectList(_context.Schools, "Id", "Id", classesModel.SchoolId);
            return View(classesModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SchoolId,Name,Info")] ClassesModel classesModel)
        {
            if (id != classesModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classesModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassesModelExists(classesModel.Id))
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
            ViewData["SchoolId"] = new SelectList(_context.Schools, "Id", "Id", classesModel.SchoolId);
            return View(classesModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classesModel = await _context.Classes
                .Include(c => c.School)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classesModel == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Students", new { id = classesModel.Id, name = classesModel.Name });
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classesModel = await _context.Classes.FindAsync(id);
            _context.Classes.Remove(classesModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassesModelExists(int id)
        {
            return _context.Classes.Any(e => e.Id == id);
        }

        public ActionResult Export(int? id)
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var classes = _context.Classes.Where(s => s.Id == id).FirstOrDefault();
                var worksheet = workbook.Worksheets.Add(classes.Name);
                var allStudentsList = _context.Students.Where(c => c.Class == classes).ToList();

                worksheet.Cell("A1").Value = "Ім'я та прізвище";
                worksheet.Cell("B1").Value = "Дата народження";
                worksheet.Cell("C1").Value = "Клас";
                worksheet.Row(1).Style.Font.Bold = true;

                for (int i = 0; i < allStudentsList.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = allStudentsList[i].Name;
                    worksheet.Cell(i + 2, 2).Value = allStudentsList[i].DateOfBirth;
                    worksheet.Cell(i + 2, 3).Value = allStudentsList[i].Class.Name;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"{classes.Name}_library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }

    }
}
