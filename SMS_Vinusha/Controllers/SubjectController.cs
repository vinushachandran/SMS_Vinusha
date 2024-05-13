using SMS_Vinusha.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace SMS_Vinusha.Controllers
{
    public class SubjectController : Controller
    {
        SMS_DBEntities db = new SMS_DBEntities();
        // GET: Subject
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult All(int pageNumber, int pageSize, bool? isActive=null)
        {
            db.Configuration.ProxyCreationEnabled = false;

            IQueryable<Subject> query = db.Subjects;

            if(isActive.HasValue)
            {
                query=query.Where(s =>s.IsEnable== isActive.Value);
            }

            int skip=(pageNumber-1)*pageSize;

            var pageData=query.OrderBy(s=>s.SubjectID).Skip(skip).Take(pageSize).ToList();

            int totalRecords=query.Count();
            int totalPages=totalRecords/pageSize;

            

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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Subject subject)
        {
            if (ModelState.IsValid)
            {
                bool subjectCodeAvailablity=IsSubCode(subject.SubjectCode);
                if (!subjectCodeAvailablity) {
                    return Json(new { success = false, message = "Subject code already taken." });
                }
                try
                {
                    db.Subjects.Add(subject);
                    db.SaveChanges();

                    return Json(new { success = true, message = "Subject added successfully." });
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




        //For Delete
        public ActionResult Delete(int id)
        {
            try
            {
                var subject = db.Subjects.Find(id);
                if (subject == null)
                {
                    return Json(new { success = false, message = "Subject not found." });
                }

                db.Subjects.Remove(subject);
                db.SaveChanges();

                return Json(new { success = true, message = "Subject deleted successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "This Subject allocated for teacher so you can not delete it. "});
            }
        }


        public ActionResult Edit(int id)
        {
            ViewBag.isEditing = true;

            var subject = db.Subjects.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Add", subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Subject subject)
        {
            if (ModelState.IsValid)
            {
                bool enable = db.Subjects.Any(s => s.IsEnable == subject.IsEnable);
                if (!enable)
                {
                    bool isSubjectUsed = db.Teacher_Subject_Allocation.Any(s => s.SubjectID == subject.SubjectID );
                    if (isSubjectUsed)
                    {
                        return Json(new { success = false, message = "This Subject  is allocated to Teacher so you can't disable it!" });
                    }
                }

                try
                {
                    // Check if the subject code already exists, excluding the current subject being edited
                    if (db.Subjects.Any(s => s.SubjectCode == subject.SubjectCode && s.SubjectID != subject.SubjectID))
                    {
                        ModelState.AddModelError("SubjectCode", "Subject code already exists.");
                        return PartialView("_Add", subject);
                    }

                    db.Entry(subject).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Subject updated successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while updating the subject: " + ex.Message });
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





        //To check Subject Code already exist
        public JsonResult IsSubCodeAvailable(string subCode)
        {
            bool isAvailable = !db.Subjects.Any(s => s.SubjectCode == subCode);
            return Json(isAvailable, JsonRequestBehavior.AllowGet);
        }

        public bool IsSubCode(string subCode)
        {
            bool isAvailable = !db.Subjects.Any(s => s.SubjectCode == subCode);
            return isAvailable;
        }



        [HttpPost]
        public ActionResult ToggleSubject(int id, bool enable)
        {
            try
            {
                var subject = db.Subjects.Find(id);
                if (subject == null)
                {
                    return Json(new { success = false, message = "Subject not found." });
                }

               if (!enable)
                {
                    bool isSubjectUsed = db.Teacher_Subject_Allocation.Any(s => s.SubjectID == id);
                    if (isSubjectUsed)
                    {
                        return Json(new { success = false, message = "This subject is allocated to teacher so you can't disable it!" });
                    }
                }

                subject.IsEnable = enable;
                db.SaveChanges();

                return Json(new { success = true, message = enable ? "Subject enabled successfully." : "Subject disabled successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }





        [HttpGet]
        public ActionResult Search(string query, string criteria)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<Subject> searchResults;

            if (criteria == "SubjectCode")
            {
                searchResults = db.Subjects.Where(s => s.SubjectCode.ToString().Contains(query)).ToList();
            }
            else
            {
                searchResults = db.Subjects.Where(s => s.Name.Contains(query)).ToList();
            }

            if (searchResults.Count > 0)
            {
                return PartialView("_SearchResults", searchResults);
            }
            else
            {
                return PartialView("_SearchResults", null);
            }
        }


       

    }
}