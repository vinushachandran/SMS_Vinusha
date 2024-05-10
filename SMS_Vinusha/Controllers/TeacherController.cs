using SMS_Vinusha.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace SMS_Vinusha.Controllers
{
    public class TeacherController : Controller
    {
        SMS_DBEntities db=new SMS_DBEntities();
        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult All(int pageNumber, int pageSize, bool? isActive = null)
        {
            db.Configuration.ProxyCreationEnabled = false;

            IQueryable<Teacher> query = db.Teachers;

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsEnable == isActive.Value);
            }

            int skip = (pageNumber - 1) * pageSize;

            var pageData = query.OrderBy(s => s.TeacherID).Skip(skip).Take(pageSize).ToList();

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
        public ActionResult Add(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                bool teacherRegAvailablity = IsTeaReg(teacher.TeacherRegNo);
                if (! teacherRegAvailablity)
                {
                    return Json(new { success = false, message = "Subject code already taken." });
                }

                bool displayNameAbilability = IsDisplayName(teacher.DisplayName);
                if (!displayNameAbilability)
                {
                    return Json(new { success = false, message = "Display name already taken." });
                }

                try
                {
                    db.Teachers.Add(teacher);
                    db.SaveChanges();

                    return Json(new { success = true, message = "Teacher added successfully." });
                }
                catch
                {
                    return Json(new { success = false, message = "Please check the error." });
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
            var teacher = db.Teachers.Find(id);
            ViewBag.IsEditing = true;
            if (teacher == null)
            {
                return HttpNotFound();
            }
            
            return PartialView("_Add", teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                bool isRegAvailable = db.Teachers.Any(s => s.TeacherRegNo == teacher.TeacherRegNo && s.TeacherID!=teacher.TeacherID);
                if (isRegAvailable)
                {
                    return Json(new { success = false, message = "Subject code already taken." });
                }


                bool isDisnameAbailable = db.Teachers.Any(s => s.DisplayName == teacher.DisplayName && s.TeacherID!=teacher.TeacherID);
                if(isDisnameAbailable)
                {
                    return Json(new { success = false, message = "This Display name already taken." });
                }


                if (teacher.IsEnable==false)
                {
                    bool isTeacherUsed = db.Teacher_Subject_Allocation.Any(s => s.TeacherID == teacher.TeacherID);
                    if (isTeacherUsed)
                    {
                        return Json(new { success = false, message = "This Teacher is allocated to subject so you can't disable it!" });
                    }
                }


                try
                {
                    db.Entry(teacher).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Teacher updated successfully." });

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while updating the teacher: " + ex.Message });
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
                var teacher = db.Teachers.Find(id);
                if (teacher == null)
                {
                    return Json(new { success = false, message = "Teacher not found." });
                }

                db.Teachers.Remove(teacher);
                db.SaveChanges();

                return Json(new { success = true, message = "Teacher deleted successfully." });
            }
            catch
            {
                return Json(new { success = false, message = "This Subject Allocated for student or theacher" });
            }
        }


        [HttpPost]
        public ActionResult ToggleTeacher(int id, bool enable)
        {
            try
            {
                var teacher = db.Teachers.Find(id);
                if (teacher == null)
                {
                    return Json(new { success = false, message = "Teacher not found." });
                }

                if(!enable)
                {
                    bool isTeacherUsed = db.Teacher_Subject_Allocation.Any(s => s.TeacherID == id);
                    if (isTeacherUsed)
                    {
                        return Json(new { success = false, message = "This Teacher is allocated to subject so you can't disable it!" });
                    }
                }

                teacher.IsEnable = enable;
                db.SaveChanges();

                return Json(new { success = true, message = enable ? "Teacher enabled successfully." : "Teacher disabled successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }


        //To check Teacher Registration Number already exist
        public JsonResult IsTeaRegAvailable(string teaReg)
        {
            bool isAvailable= IsTeaReg(teaReg);
            return Json(isAvailable, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// To check Teacher Registration Number already exist
        /// </summary>
        /// <param name="teaReg">Teacher Registration</param>
        /// <returns></returns>
        public bool IsTeaReg(string teaReg)
        {
            bool isAvailable = !db.Teachers.Any(s => s.TeacherRegNo == teaReg);
            return isAvailable;
        }


        //To check Teacher Display name already exist
        public JsonResult IsDisNameAvailable(string teaDisName)
        {
            bool isAvailable = !db.Teachers.Any(s => s.DisplayName == teaDisName);
            return Json(isAvailable, JsonRequestBehavior.AllowGet);
        }

        public bool IsDisplayName(string disName)
        {
            bool isAvailable = !db.Teachers.Any(s => s.DisplayName == disName);
            return isAvailable;
        }











    }
}