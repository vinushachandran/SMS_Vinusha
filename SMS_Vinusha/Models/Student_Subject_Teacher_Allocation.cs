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
    
    public partial class Student_Subject_Teacher_Allocation
    {
        public long StudentAllocationID { get; set; }
        public long StudentID { get; set; }
        public long SubjectAllocationID { get; set; }
    
        public virtual Student Student { get; set; }
        public virtual Teacher_Subject_Allocation Teacher_Subject_Allocation { get; set; }
    }
}
