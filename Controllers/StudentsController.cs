using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolProject.EFContext;
using SchoolProject.Models;

namespace SchoolProject.Controllers
{
    public class StudentsController : Controller
    {
        private readonly EFSchoolContext _context;

        public StudentsController(EFSchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var eFSchoolContext = _context.Students.Include(s => s.Class);
            return View(await eFSchoolContext.ToListAsync());
        }

        public async Task<IActionResult> GetStudentsByClasses(int? id, string? name)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Classes");
            }
            ViewBag.ClassId = id;
            ViewBag.ClassName = name;
            var classes = _context.Students.Where(u => u.ClassId == id).Include(u => u.Class);
            return View(await classes.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentsModel = await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentsModel == null)
            {
                return NotFound();
            }

            return View(studentsModel);
        }

        public IActionResult Create(int? classId)
        {
            ViewBag.ClassId = classId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int classId,[Bind("Id,Name,DateOfBirth")] StudentsModel studentsModel)
        {
            if (ModelState.IsValid)
            {
                studentsModel.ClassId = classId;
                _context.Add(studentsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetStudentsByClasses));
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id", studentsModel.ClassId);
            return View(studentsModel);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentsModel = await _context.Students.FindAsync(id);
            if (studentsModel == null)
            {
                return NotFound();
            }
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id", studentsModel.ClassId);
            return View(studentsModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ClassId,DateOfBirth")] StudentsModel studentsModel)
        {
            if (id != studentsModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentsModelExists(studentsModel.Id))
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
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id", studentsModel.ClassId);
            return View(studentsModel);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentsModel = await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentsModel == null)
            {
                return NotFound();
            }

            return View(studentsModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentsModel = await _context.Students.FindAsync(id);
            _context.Students.Remove(studentsModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentsModelExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
