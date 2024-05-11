using SMS_Vinusha.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS_Vinusha.Controllers
{
    public class AllocationController : Controller
    {
        SMS_DBEntities db = new SMS_DBEntities();

        private List<Teacher_Subject_Allocation>subject_Allocations=new List<Teacher_Subject_Allocation>(); 
        // GET: Allocation
        public ActionResult Index()
        {
            ViewBag.Subjects=db.Subjects.Where(s=>s.IsEnable).ToList();
            ViewBag.Teachers=db.Teachers.Where(s=>s.IsEnable).ToList();
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
    }
    
}