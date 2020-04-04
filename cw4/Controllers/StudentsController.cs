using System;
using System.Collections.Generic;
using System.Linq;
using cw3.DAL;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]

    public class StudentsController : ControllerBase
    {

        private readonly IDbService _dbService;

        public StudentsController(IDbService dbservice)
        {
            _dbService = dbservice;
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            var student = new Student();
            using (var client = new SqlConnection("Data Source = db-mssql.pjwstk.edu.pl; Initial Catalog = s16796; Integrated Security = True"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    //indexy od 12 do 20, podawane jako np localhost:44312/api/students/13 da studenta s13
                    command.CommandText = "SELECT * FROM Student st JOIN ENROLLMENT enr ON st.IdEnrollment = enr.IdEnrollment JOIN Studies sts on enr.IdStudy = sts.IdStudy WHERE IndexNumber LIKE '%' + CAST(@id AS varchar)";
                    command.Parameters.AddWithValue("id", id);
                    client.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        student.firstName = reader["FirstName"].ToString();
                        student.lastName = reader["LastName"].ToString();
                        student.BirthDate = Convert.ToDateTime(reader["BirthDate"].ToString());
                        student.indexNumber = reader["IndexNumber"].ToString();
                        student.Study = reader["Name"].ToString();
                        student.Semester = Convert.ToInt32(reader["Semester"].ToString());
                    }
                }
            }
            return Ok(student);
        }

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            var listofstudents = new List<Student>();
            using (var client = new SqlConnection("Data Source = db-mssql.pjwstk.edu.pl; Initial Catalog = s16796; Integrated Security = True"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = client;
                    command.CommandText = "SELECT * FROM Student st JOIN ENROLLMENT enr ON st.IdEnrollment = enr.IdEnrollment JOIN Studies sts on enr.IdStudy = sts.IdStudy";
                    client.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var st = new Student();
                        st.firstName = reader["FirstName"].ToString();
                        st.lastName = reader["LastName"].ToString();
                        st.BirthDate = Convert.ToDateTime(reader["BirthDate"].ToString());
                        st.indexNumber = reader["IndexNumber"].ToString();
                        st.Study = reader["Name"].ToString();
                        st.Semester = Convert.ToInt32(reader["Semester"].ToString());

                        listofstudents.Add(st);
                    }
                }
            }
            return Ok(listofstudents);
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.indexNumber = $"s{new Random().Next(1, 99999)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(int id, Student student)
        {
            return Ok("Aktualizacja studenta nr " + id + " dokończona.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie studenta nr " +id+ " ukończone.");
        }

    }
}