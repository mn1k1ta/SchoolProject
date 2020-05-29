using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolProject.EFContext;
using SchoolProject.Models;

namespace SchoolProject.Controllers
{
 
    public class SchoolsController : Controller
    {
        private readonly EFSchoolContext _context;

        public SchoolsController(EFSchoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Schools.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schoolsModel = await _context.Schools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schoolsModel == null)
            {
                return NotFound();
            }

            //return View(schoolsModel);
            return RedirectToAction("Index", "Classes", new { id = schoolsModel.Id, name = schoolsModel.Name });
        }

        public async Task<IActionResult> GetTeachers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schoolsModel = await _context.Schools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schoolsModel == null)
            {
                return NotFound();
            }

            //return View(schoolsModel);
            return RedirectToAction("Index", "Teachers", new { id = schoolsModel.Id, name = schoolsModel.Name });
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Address,Name")] SchoolsModel schoolsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(schoolsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(schoolsModel);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schoolsModel = await _context.Schools.FindAsync(id);
            if (schoolsModel == null)
            {
                return NotFound();
            }
            return View(schoolsModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Address,Name")] SchoolsModel schoolsModel)
        {
            if (id != schoolsModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schoolsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchoolsModelExists(schoolsModel.Id))
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
            return View(schoolsModel);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schoolsModel = await _context.Schools
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schoolsModel == null)
            {
                return NotFound();
            }

            return View(schoolsModel);
        }

 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schoolsModel = await _context.Schools.FindAsync(id);
            var teacherModel =  _context.Teachers.Where(t => t.SchoolId == id).FirstOrDefault();
            var classModel = _context.Classes.Where(c => c.SchoolId == id).FirstOrDefault();
            if (classModel!=null){
               
                schoolsModel.Classes.Add(classModel);
            }
            if (teacherModel != null)
            {
                schoolsModel.Teachers.Add(teacherModel);
            }           
            _context.Schools.Remove(schoolsModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

     
      private bool SchoolsModelExists(int id)
        {
            return _context.Schools.Any(e => e.Id == id);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
                        {
                            foreach (IXLWorksheet worksheet in workBook.Worksheets)
                            {
                                SchoolsModel newSchool;

                                var c = _context.Schools.Where(c => c.Name == worksheet.Name).ToList();
                                if (c.Count > 0)
                                {
                                    newSchool = c[0];
                                }
                                else
                                {
                                    newSchool = new SchoolsModel();
                                    newSchool.Name = worksheet.Name;                                   
                                    _context.Schools.Add(newSchool);

                                }
                                await _context.SaveChangesAsync();

                                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                {
                                    try
                                    {
                                        StudentsModel student = new StudentsModel();
                                        student.Name = row.Cell(1).Value.ToString();
                                        student.DateOfBirth = row.Cell(2).GetDateTime();
                                        ClassesModel cl;
                                        var classes = _context.Classes.Where(c => c.Name.Contains(row.Cell(3).Value.ToString()) && c.School==newSchool).FirstOrDefault();
                                        if (classes == null)
                                        {
                                            cl = new ClassesModel();
                                            cl.Name = row.Cell(3).Value.ToString();
                                            cl.School = newSchool;
                                            cl.Info = "Import with EXCEL!";
                                            _context.Classes.Add(cl);
                                            classes = cl;
                                        }
                                        student.Class = classes;
                                        var students = _context.Students.Where(s => s.Name == student.Name && s.DateOfBirth == student.DateOfBirth && s.Class == classes).FirstOrDefault();
                                        if (students == null)
                                        {
                                            _context.Students.Add(student);
                                        }
                                        await _context.SaveChangesAsync();
                                    }
                                    catch (Exception e)
                                    {


                                    }
                                }
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Export(int? id)
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var school = _context.Schools.Where(s => s.Id == id).FirstOrDefault();
                var worksheet = workbook.Worksheets.Add(school.Name);
                var classes = _context.Classes.Where(c => c.SchoolId == id).ToList();
                List<StudentsModel> allStudentsList = new List<StudentsModel>();
                foreach (var item in classes)
                {
                    var students = _context.Students.Where(s => s.Class == item);
                    allStudentsList.AddRange(students);

                }                                 
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
                        FileDownloadName = $"{school.Name}_library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }

    }

}
