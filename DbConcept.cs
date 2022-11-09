
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FaceTracking
{
    public class DbConcept
    {
        private readonly string _connectionString;
        public DbConcept()
        {
            _connectionString = ConfigurationManager.AppSettings["connectionString"];
        }
        public int NewUser(string userName, string firstName, string lastName, string password)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string pwd = Core.Encrypt(password);
                string sql = "insert into [user] (FirstName,LastName,UserName,password) " +
                    "values('" + firstName + "','" + lastName + "','" + userName + "','" + pwd + "');";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    int iResult = command.ExecuteNonQuery();
                    connection.Close();
                    return iResult;
                }
            }
        }

        public UserDto Login(string userName, string password)
        {
            UserDto userDto = new UserDto();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlQuery = "\r\nSelect userId,UserName,firstName,lastName from [user]" +
                    " where userName ='" + userName + "' and password='" + password + "'";
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    userDto.UserId = Convert.ToInt32(rdr["userId"]);
                    userDto.FirstName = rdr["FirstName"].ToString();
                    userDto.LastName = rdr["LastName"].ToString();
                    userDto.UserName = rdr["UserName"].ToString();
                }
                return userDto;
            }
        }

        public StudetnDto FindStudent(string StudetnRollNumber)
        {
            StudetnDto student = new StudetnDto();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlQuery = "Select * From Entrollment Where RollNo ='"+ StudetnRollNumber + "'";
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    student.Entrollmentid = int.Parse( rdr["Entrollmentid"].ToString());
                    student.FirstName = rdr["FirstName"].ToString();
                    student.LastName = rdr["LastName"].ToString();
                    student.Address = rdr["Address"].ToString();
                    student.Contact = rdr["Contact"].ToString();
                    student.Age = int.Parse( rdr["age"].ToString());
                    student.Gender = rdr["Gender"].ToString();
                    student.Photo = (byte[]) rdr["StudentPhoto"];
                    student.Dob = DateTime.Parse(rdr["DOB"].ToString());
                    student.RollNo = rdr["rollno"].ToString();
                }
                return student;
            }
        }

        public string ForgetPassword(string userName)
        {
            string Password = string.Empty;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlQuery = "\r\nSelect password from [user]" +
                    " where userName ='" + userName + "'";
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Password = Core.Decrypt(rdr["password"].ToString());
                }
            }
            return Password;
        }

        public int NewEntrollment(string Rollno, string FirstName, string Lastname, int age, string Contact, string Address,
            string Gender, DateTime DOB, byte[] StudentPhoto, int DepartmentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Entrollment(Rollno,FirstName,Lastname,Age,Contact,Address,Gender,DOB,StudentPhoto,DepartmentId) " +
                     "VALUES(@Rollno,@FirstName,@Lastname,@Age,@Contact,@Address,@Gender,@DOB,@StudentPhoto,@DepartmentId)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Rollno", Rollno);
                    command.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@Lastname", Lastname);
                    command.Parameters.AddWithValue("@Age", age);
                    command.Parameters.AddWithValue("@Contact", Contact);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@Gender", Gender);
                    command.Parameters.AddWithValue("@DOB", DOB);
                    command.Parameters.AddWithValue("@StudentPhoto", StudentPhoto);
                    int iResult = command.ExecuteNonQuery();
                    connection.Close();
                    return iResult;
                }
            }
        }

        public int NewAttendance(string rollNumber, bool isFlag,int userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Attendance(Rollno,Flag,UserId) " +
                     "VALUES(@Rollno,@Flag,@UserId)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Rollno", rollNumber);
                    command.Parameters.AddWithValue("@Flag", isFlag);
                    command.Parameters.AddWithValue("@UserId", userId);
                    int iResult = command.ExecuteNonQuery();
                    connection.Close();
                    return iResult;
                }
            }
        }

        public bool CheckAttendance(string rollNumber)
        {
            int Result = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string sqlQuery = "Select Count(1) as Cnt from (Select CAST(Entrydate as DATE) as EDate , * from Attendance ) as CM where CM.EDate ='" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "' and Rollno ='" + rollNumber + "'";
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Result = int.Parse(rdr["Cnt"].ToString());
                }
            }
            return Result > 0;
        }

        public int UpdateEntrollment(int enrollmentId,string Rollno, string FirstName, string Lastname, int age, string Contact, string Address,
            string Gender, DateTime DOB, byte[] StudentPhoto,int DepartmentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "Update Entrollment Set Rollno=@Rollno,FirstName=@FirstName,Lastname=@Lastname,Age=@Age,Contact=@Contact," +
                    "Address=@Address,Gender=@Gender,DOB=@DOB,StudentPhoto=@StudentPhoto,DepartmentId=@DepartmentId Where Entrollmentid =@Entrollmentid ";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Entrollmentid", enrollmentId);
                    command.Parameters.AddWithValue("@Rollno", Rollno);
                    command.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@Lastname", Lastname);
                    command.Parameters.AddWithValue("@Age", age);
                    command.Parameters.AddWithValue("@Contact", Contact);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@Gender", Gender);
                    command.Parameters.AddWithValue("@DOB", DOB);
                    command.Parameters.AddWithValue("@StudentPhoto", StudentPhoto);
                    int iResult = command.ExecuteNonQuery();
                    connection.Close();
                    return iResult;
                }
            }
        }
        public int DeleteEntrollment(int enrollmentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = " Delete from Entrollment Where Entrollmentid =@Entrollmentid ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Entrollmentid", enrollmentId);
                    int iResult = command.ExecuteNonQuery();
                    connection.Close();
                    return iResult;
                }
            }
        }

        public DataTable GetAllEnrollment()
        {
            string query = "Select depart.DepartmentCode,depart.DepartmentId,enr.Entrollmentid, enr.Rollno,enr.FirstName,enr.Lastname,\r\nenr.Address  , enr.Age,enr.Contact,enr.DOB,enr.Age,enr.StudentPhoto,enr.Gender from  Department depart \r\ninner join Entrollment enr\r\non depart.DepartmentId = enr.DepartmentId";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter(query, con))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public List<AttendanceDto> GetAttendance()
        {
            string query = "Select enr.Rollno,enr.FirstName,enr.Lastname,depart.DepartmentCode , att.EntryDate,\r\nCASE WHEN Flag =0 THEN 'Absence'\r\n               ELSE 'Presence' END AS Flag\r\n   from Department as depart\r\ninner join Entrollment as enr\r\non depart.DepartmentId = enr.DepartmentId\r\ninner join  [Attendance] as att\r\non enr.Rollno = att.Rollno order by enr.rollno";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                List<AttendanceDto> attendances = new List<AttendanceDto>();
                while (rdr.Read())
                {
                    AttendanceDto result = new AttendanceDto();
                    result.RollNumber = rdr["Rollno"].ToString();
                    result.FirstName = rdr["FirstName"].ToString();
                    result.LastName = rdr["LastName"].ToString();
                    result.Type = rdr["Flag"].ToString();
                    result.DepartmentCode = rdr["DepartmentCode"].ToString();
                    result.EntryDate = Convert.ToDateTime(rdr["EntryDate"].ToString());
                    attendances.Add(result);
                }
                return attendances;
            }
        }

        public DataTable GetAllDepartments()
        {
            string query = "SELECT * FROM Department";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter(query, con))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public int NewDepartment(string DepartmentName,string DepartmentCode)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Department(DepartmentName,DepartmentCode) " +
                     "VALUES(@DepartmentName,@DepartmentCode)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@DepartmentName", DepartmentName);
                    command.Parameters.AddWithValue("@DepartmentCode", DepartmentCode);
                    int iResult = command.ExecuteNonQuery();
                    connection.Close();
                    return iResult;
                }
            }
        }

        public int UpdateDepartment(int DepartmentId,string DepartmentName, string DepartmentCode)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "Update Department Set DepartmentName=@DepartmentName,DepartmentCode=@DepartmentCode Where DepartmentId=@DepartmentId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@DepartmentId", DepartmentId);
                    command.Parameters.AddWithValue("@DepartmentName", DepartmentName);
                    command.Parameters.AddWithValue("@DepartmentCode", DepartmentCode);
                    int iResult = command.ExecuteNonQuery();
                    connection.Close();
                    return iResult;
                }
            }
        }

        public int DeleteDepartment(int DepartmentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = " Delete from Department Where DepartmentId=@DepartmentCode ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@DepartmentCode", DepartmentId);
                    int iResult = command.ExecuteNonQuery();
                    connection.Close();
                    return iResult;
                }
            }
        }

        public void UpdateAttendance()
        {
            DataTable allStudent = GetAllEnrollment();
            for (int i = 0; i <= allStudent.Rows.Count - 1; i++)
            {
                string rollNumber = allStudent.Rows[i]["Rollno"].ToString();
                bool isAttendance = CheckAttendance(rollNumber);
                if (!isAttendance)
                {
                    NewAttendance(rollNumber, false, 0);
                }
            }
        }
    }
}