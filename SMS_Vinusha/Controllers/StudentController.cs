using SMS_Vinusha.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS_Vinusha.Controllers
{
    public class StudentController : Controller
    {
        SMS_DBEntities db = new SMS_DBEntities();

        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult All(int pageNumber, int pageSize, bool? isActive = null)
        {
            db.Configuration.ProxyCreationEnabled = false;

            IQueryable<Student> query = db.Students;

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsEnable == isActive.Value);
            }

            int skip = (pageNumber - 1) * pageSize;

            var pageData = query.OrderBy(s => s.StudentID).Skip(skip).Take(pageSize).ToList();

            int totalRecords = query.Count();
            int totalPages = totalRecords / pageSize;



            if (pageData.Count > 0)
            {
                return Json(new { success = true, data = pageData, totalPages = totalPages }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No Data Found", totalPages = totalPages }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Add()
        {
            ViewBag.IsEditing = false;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Student student)
        {
            if (ModelState.IsValid)
            {
                bool studentRegAvailability = IsStudentReg(student.StudentRegNo);
                if (!studentRegAvailability)
                {
                    return Json(new { success = false, message = "Student registration number already taken." });
                }

                bool displayNameAvailability = IsDisplayName(student.DisplayName);
                if (!displayNameAvailability)
                {
                    return Json(new { success = false, message = "Display name already taken." });
                }

                try
                {
                    db.Students.Add(student);
                    db.SaveChanges();

                    return Json(new { success = true, message = "Student added successfully." });
                }
                catch
                {
                    return Json(new { success = false, message = "An error occurred while adding the student." });
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Please fill all details.", errors = errors });
            }
        }

        public ActionResult Edit(int id)
        {
            var student = db.Students.Find(id);
            ViewBag.IsEditing = true;
            if (student == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Add", student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                bool isRegAvailable = db.Students.Any(s => s.StudentRegNo == student.StudentRegNo && s.StudentID != student.StudentID);
                if (isRegAvailable)
                {
                    return Json(new { success = false, message = "This reg no already taken." });
                }


                bool isDisNameAvailable = db.Students.Any(s => s.DisplayName == student.DisplayName && s.StudentID != student.StudentID);
                if (isDisNameAvailable)
                {
                    return Json(new { success = false, message = "This display name already taken." });
                }

                try
                {
                    db.Entry(student).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Student updated successfully." });

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while updating the student: " + ex.Message });
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Validation errors occurred.", errors = errors });
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var student = db.Students.Find(id);
                if (student == null)
                {
                    return Json(new { success = false, message = "Student not found." });
                }

                db.Students.Remove(student);
                db.SaveChanges();

                return Json(new { success = true, message = "Student deleted successfully." });
            }
            catch
            {
                return Json(new { success = false, message = "Error occurred while deleting student." });
            }
        }

        [HttpPost]
        public ActionResult ToggleStudent(int id, bool enable)
        {
            try
            {
                var student = db.Students.Find(id);
                if (student == null)
                {
                    return Json(new { success = false, message = "Student not found." });
                }

                student.IsEnable = enable;
                db.SaveChanges();

                return Json(new { success = true, message = enable ? "Student enabled successfully." : "Student disabled successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        //To check if Student Registration Number already exists
        public JsonResult IsStudentRegAvailable(string studentReg)
        {
            bool isAvailable = !db.Students.Any(s => s.StudentRegNo == studentReg);
            return Json(isAvailable, JsonRequestBehavior.AllowGet);
        }

        public bool IsStudentReg(string studentReg)
        {
            bool isAvailable = !db.Students.Any(s => s.StudentRegNo == studentReg);
            return isAvailable;
        }

        //To check if Display name already exists
        public JsonResult IsDisplayNameAvailable(string displayName)
        {
            bool isAvailable = !db.Students.Any(s => s.DisplayName == displayName);
            return Json(isAvailable, JsonRequestBehavior.AllowGet);
        }

        public bool IsDisplayName(string displayName)
        {
            bool isAvailable = !db.Students.Any(s => s.DisplayName == displayName);
            return isAvailable;
        }
    }
}
