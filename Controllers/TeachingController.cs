using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolProject.EFContext;
using SchoolProject.Models;
using SchoolProject.Repository;

namespace SchoolProject.Controllers
{
    public class TeachingController : Controller
    {
        private readonly EFSchoolContext _context;
  

        public TeachingController(EFSchoolContext context)
        { 
            _context = context;
        }

        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Schools");
            }
            ViewBag.TeacherId = id;
            ViewBag.TeacherName = name;
            var teaching = await _context.Teaching.Where(u => u.TeacherId == id).Include(u => u.Teacher).ToListAsync();
            foreach (var item in teaching)
            {
                var subject = _context.Subjects.Where(u => u.Id == item.SubjectId).FirstOrDefault();
                item.Subject = subject;
            }
            
            return View(teaching);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teachingModel = await _context.Teaching
                .Include(t => t.Subject)
                .Include(t => t.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teachingModel == null)
            {
                return NotFound();
            }

            return View(teachingModel);
        }

     
        public IActionResult Create(int? teacherId)
        {
            ViewBag.TeacherId = teacherId;
            ViewData["ClassId"] = new SelectList(_context.Classes, "Id", "Id");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int teacherId,[Bind("Id,TeacherId,ClassId,SubjectId")] TeachingModel teachingModel)
        {
            teachingModel.TeacherId = teacherId;
            if (ModelState.IsValid)
            {               
                _context.Add(teachingModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
         
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", teachingModel.SubjectId);


            return View(teachingModel);
        }

       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teachingModel = await _context.Teaching.FindAsync(id);
            if (teachingModel == null)
            {
                return NotFound();
            }
          
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", teachingModel.SubjectId);
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "Id", teachingModel.TeacherId);
            return View(teachingModel);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeacherId,ClassId,SubjectId")] TeachingModel teachingModel)
        {
            if (id != teachingModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teachingModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeachingModelExists(teachingModel.Id))
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
           
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", teachingModel.SubjectId);
            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "Id", teachingModel.TeacherId);
            return View(teachingModel);
        }

      
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teachingModel = await _context.Teaching
               
                .Include(t => t.Subject)
                .Include(t => t.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teachingModel == null)
            {
                return NotFound();
            }

            return View(teachingModel);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teachingModel = await _context.Teaching.FindAsync(id);
            _context.Teaching.Remove(teachingModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeachingModelExists(int id)
        {
            return _context.Teaching.Any(e => e.Id == id);
        }
    }
}
