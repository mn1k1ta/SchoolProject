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
    public class SubjectsController : Controller
    {
        private readonly EFSchoolContext _context;

        public SubjectsController(EFSchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Subjects.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjectsModel = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectsModel == null)
            {
                return NotFound();
            }

            return View(subjectsModel);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Info")] SubjectsModel subjectsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subjectsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subjectsModel);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjectsModel = await _context.Subjects.FindAsync(id);
            if (subjectsModel == null)
            {
                return NotFound();
            }
            return View(subjectsModel);
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Info")] SubjectsModel subjectsModel)
        {
            if (id != subjectsModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subjectsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectsModelExists(subjectsModel.Id))
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
            return View(subjectsModel);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjectsModel = await _context.Subjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectsModel == null)
            {
                return NotFound();
            }

            return View(subjectsModel);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subjectsModel = await _context.Subjects.FindAsync(id);
            _context.Subjects.Remove(subjectsModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectsModelExists(int id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }
    }
}
