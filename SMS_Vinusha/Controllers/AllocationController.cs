using SMS_Vinusha.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS_Vinusha.Controllers
{
    public class AllocationController : Controller
    {
        SMS_DBEntities db = new SMS_DBEntities();

        private List<Teacher_Subject_Allocation>subject_Allocations=new List<Teacher_Subject_Allocation>();

        public AllocationController()
        {

            ViewBag.Subjects = db.Subjects.Where(s => s.IsEnable == true)
               .Select(s => new { SubjectID = s.SubjectID, Name = s.SubjectCode + " - " + s.Name })
               .ToList();
            ViewBag.Teachers = db.Teachers.Where(s => s.IsEnable == true)
                 .Select(s => new { TeacherID = s.TeacherID, DisplayName = s.TeacherRegNo + " - " + s.DisplayName })
                .ToList();

            ViewBag.Students = db.Students.Where(s => s.IsEnable == true)
                 .Select(s => new { StudentID = s.StudentID, DisplayName = s.StudentRegNo + " - " + s.DisplayName })
                .ToList();

            var allocatedSubjects = db.Teacher_Subject_Allocation.Select(a => new { SubjectID = a.SubjectID, Name = a.Subject.SubjectCode + " - " + a.Subject.Name }).Distinct().ToList();


            ViewBag.AllocatedSubjects = allocatedSubjects;

        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AllSubjectTeacherAllocation()
        {
            db.Configuration.ProxyCreationEnabled = false;

            var query = db.Teacher_Subject_Allocation.Include("Subject").Include("Teacher");

            var subjectTeacherAllocation=query.ToList();

            if(subjectTeacherAllocation.Count > 0)
            {
                var data = subjectTeacherAllocation.Select(item => new
                {
                    SubjectAllocationID = item.SubjectAllocationID,
                    SubjectCode = item.Subject.SubjectCode,
                    SubjectName = item.Subject.Name,
                    TeacherRegNo = item.Teacher.TeacherRegNo,
                    TeacherName = item.Teacher.DisplayName
                });

                return Json(new {success=true, data=data},JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new {success=false,message="Data Not Found"},JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult AddTeacherSubjectAllocation()
        {
            ViewBag.IsEditing = false;
            return View();
        }


        public ActionResult AddTeacherSubjectAllocation(Teacher_Subject_Allocation teacher_Subject_Allocation)
        {
            if (ModelState.IsValid)
            {
               

                try
                {
                    db.Teacher_Subject_Allocation.Add(teacher_Subject_Allocation);
                    db.SaveChanges();

                    return Json(new { success = true, message = "Teacher subject allocation added successfully." });
                }
                catch
                {
                    return Json(new { success = false, message = "This Teachet already allocated for this subject!" });
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

        public ActionResult DeleteTeacherSubjectAllocation(long id)
        {
            try
            {
                var teacher_Subject_Allocation = db.Teacher_Subject_Allocation.Find(id);
                if (teacher_Subject_Allocation == null)
                {
                    return Json(new { success = false, message = "Teacher Subject Allocation not found." });
                }

                db.Teacher_Subject_Allocation.Remove(teacher_Subject_Allocation);
                db.SaveChanges();

                return Json(new { success = true, message = "Teacher Subject Allocation deleted successfully." });
            }
            catch
            {
                return Json(new { success = false, message = "This Data Allocated for student " });
            }
        }


        public ActionResult Edit_TeacherSubjectAllocation(int id)
        {
            var teacher_subject_allocation = db.Teacher_Subject_Allocation.Find(id);
            ViewBag.IsEditing = true;
            if (teacher_subject_allocation == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Add", teacher_subject_allocation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_TeacherSubjectAllocation(Teacher_Subject_Allocation teacher_Subject_Allocation)
        {
            if (ModelState.IsValid)
            {
                bool isAllocated = db.Student_Subject_Teacher_Allocation.Any(s => s.SubjectAllocationID == teacher_Subject_Allocation.SubjectAllocationID);

                if (isAllocated)
                {
                    return Json(new { success = false, message = "The students are following this subject to the teacher." });
                }

                bool alreadyExists = db.Teacher_Subject_Allocation.Any(s => s.TeacherID == teacher_Subject_Allocation.TeacherID && s.SubjectID == teacher_Subject_Allocation.SubjectID);

                if (alreadyExists)
                {
                    return Json(new { success = false, message = "This teacher is already teaching this subject" });
                }

                try
                {
                    db.Entry(teacher_Subject_Allocation).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Teacher subject allocation updated successfully." });

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while updating the teacher subject allocation : " + ex.Message });
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


        //Students

        public ActionResult AllStudentAllocation()
        {

            db.Configuration.ProxyCreationEnabled = false;

            var query = db.Student_Subject_Teacher_Allocation
                          .Include("Teacher_Subject_Allocation.Subject")
                          .Include("Teacher_Subject_Allocation.Teacher")
                          .Include("Student")
                          .Where(s => s.Student.IsEnable == true);


            var studentsAllocations = query.ToList();

            if (studentsAllocations.Count > 0)
            {
                var data = studentsAllocations.Select(item => new
                {
                    studentAllocationID = item.StudentAllocationID,
                    StudentID = item.StudentID,
                    StudentName = item.Student.DisplayName,
                    studentRegNo = item.Student.StudentRegNo,
                    SubjectID = item.Teacher_Subject_Allocation.SubjectID,
                    subjectCode = item.Teacher_Subject_Allocation.Subject.SubjectCode,
                    SubjectName = item.Teacher_Subject_Allocation.Subject.Name,
                    TeacherID = item.Teacher_Subject_Allocation.TeacherID,
                    teacherRegNo = item.Teacher_Subject_Allocation.Teacher.TeacherRegNo,
                    TeacherName = item.Teacher_Subject_Allocation.Teacher.DisplayName
                }); ; ; ;

                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No Data Found" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStudentAllocation(Student_Subject_Teacher_Allocation teacherAllocation)
        {
            ViewBag.IsEditing = false;


            if (ModelState.IsValid)
            {
                try
                {
                    bool isAllocated = db.Student_Subject_Teacher_Allocation.Any(a => a.StudentID == teacherAllocation.StudentID && a.SubjectAllocationID == teacherAllocation.SubjectAllocationID);

                    if (isAllocated)
                    {

                        return Json(new { success = false, message = "This Student is already study this subject to this teacher " });

                    }
                    // Save the subject allocation
                    db.Student_Subject_Teacher_Allocation.Add(teacherAllocation);
                    db.SaveChanges();

                    return Json(new { success = true, message = "Teacher Subject Allocation added successfully." });
                }
                catch
                {
                    return Json(new { success = false, message = "Invalid Subject Allocation ID " });
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = errors, errors = errors });
            }

        }


        public JsonResult GetTeachersBySubject(int subjectId)
        {
            var teachers = db.Teacher_Subject_Allocation
                .Where(tsa => tsa.SubjectID == subjectId)
                .Select(tsa => new
                {
                    Value = tsa.TeacherID,
                    Text = tsa.Teacher.DisplayName
                })
                .ToList();

            return Json(teachers, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetAllocationID(long subjectId, long teacherId)
        {
            var allocationId = db.Teacher_Subject_Allocation
                .Where(allocation => allocation.SubjectID == subjectId && allocation.TeacherID == teacherId)
                .Select(allocation => allocation.SubjectAllocationID)
                .FirstOrDefault();

            return Json(allocationId, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllocatedSubject()
        {
            var data = db.Teacher_Subject_Allocation.Select(a => new { SubjectID = a.SubjectID, Name = a.Subject.SubjectCode + " - " + a.Subject.Name }).Distinct().ToList();

            if (data.Count > 0)
            {
                return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No Data Found" }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult DeleteStudentAllocation(long id)
        {
            try
            {
                var student_Allocation = db.Student_Subject_Teacher_Allocation.Find(id);
                if (student_Allocation == null)
                {
                    return Json(new { success = false, message = "Students Allocation not found." });
                }

                db.Student_Subject_Teacher_Allocation.Remove(student_Allocation);
                db.SaveChanges();

                return Json(new { success = true, message = "Student Allocation deleted successfully." });
            }
            catch
            {
                return Json(new { success = false, message = "This Data Allocated for student " });
            }
        }

        public ActionResult EditStudentAllocation(long id)
        {
            ViewBag.IsEditing = true;

            var studentAllocation = db.Student_Subject_Teacher_Allocation.Find(id);


            if (studentAllocation == null)
            {
                return HttpNotFound();
            }

            ViewBag.SelectedSubjectID = studentAllocation.StudentID;

            return PartialView("_AddStudentAllocation", studentAllocation);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudentAllocation(Student_Subject_Teacher_Allocation studentAllocation)
        {
            if (ModelState.IsValid)
            {
                bool isAllocated = db.Student_Subject_Teacher_Allocation.Any(s => s.SubjectAllocationID == studentAllocation.SubjectAllocationID && s.StudentID == studentAllocation.StudentID);


                if (isAllocated)
                {
                    return Json(new { success = false, message = "The student is already following this subject to the teacher." });
                }



                try
                {
                    db.Entry(studentAllocation).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Student Allocation updated successfully." });

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
                return Json(new { success = false, message = "Please fill all details.", errors = errors });
            }
        }
    }

}