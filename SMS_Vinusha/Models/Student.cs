//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SMS_Vinusha.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Student
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Student()
        {
            this.Student_Subject_Teacher_Allocation = new HashSet<Student_Subject_Teacher_Allocation>();
        }
    
        public long StudentID { get; set; }
        [Required(ErrorMessage = "Student registration number is required")]
        [DisplayName("Registration No")]
        public string StudentRegNo { get; set; }
        [Required(ErrorMessage = "Student First Name is required")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Student Last Name is required")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Student Display Name is required")]
        [DisplayName("Display Name")]
        public string DisplayName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [DisplayName("Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        [DisplayName("Gender")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Date of Birth is required")]
        [DisplayName("Date Of Birth")]
        public System.DateTime DOB { get; set; }
        [Required(ErrorMessage = "Address is required")]
        [DisplayName("Address")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Contact No is required")]
        [DisplayName("Contact No")]
        public string ContactNo { get; set; }
        [DisplayName("Status")]
        public bool IsEnable { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Student_Subject_Teacher_Allocation> Student_Subject_Teacher_Allocation { get; set; }
    }
}