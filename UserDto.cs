using System;

namespace FaceTracking
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
    }

    public class StudetnDto
    {
        public int Entrollmentid { get; set; }
        public string RollNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }

        public string Contact { get; set; }
        public DateTime Dob { get; set; }
        public int Age { get; set; }
        public byte[] Photo { get; set; }
    }


    public class AttendanceDto
    {
        public string  RollNumber { get; set; }
        public string  FirstName { get; set; }
        public string LastName { get; set; }
        public string  DepartmentCode { get; set; }
        public DateTime EntryDate { get; set; }
        public string Type { set; get; }
    }
}
