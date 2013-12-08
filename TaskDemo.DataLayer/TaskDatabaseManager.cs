using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TaskDemo.Entities;
using Task = TaskDemo.Entities.Task;


namespace TaskDemo.DataLayer
{
    //public class Program
    //{
    //    public static void Main()
    //    {
    //        TaskDatabaseManager mgr = new TaskDatabaseManager(@"Data Source=(local)\SQLEXPRESS;Initial Catalog=TaskDemo;Integrated Security=True");
    //        mgr.AddCategory(new Category { Name = "TestCat" });
    //    }
    //}

    public class TaskDatabaseManager
    {
        private readonly string _connectionString;


        public TaskDatabaseManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddCategory(Category category)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Categories VALUES(@name); SELECT @@Identity";
                command.Parameters.AddWithValue("@name", category.Name);
                connection.Open();
                int id = (int)(decimal)command.ExecuteScalar();
                category.Id = id;
            }
        }

        public IEnumerable<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT 
                                        c.id as CategoryId, 
                                        c.Name as CategoryName, 
                                        t.Description as TaskDescription, 
                                        t.CategoryId as TaskCatId, 
                                        t.Id as TaskID 
                                        FROM Categories c  
                                        LEFT JOIN Tasks t ON c.Id=t.CategoryId";
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int catId = int.Parse(reader["CategoryId"].ToString());
                    Category category = categories.FirstOrDefault(n => n.Id == catId);
                    if (category == null)
                    {
                        category = new Category();
                        category.Name = (string)reader["CategoryName"];
                        category.Id = catId;
                        category.Tasks = new List<Task>();
                        categories.Add(category);
                    }

                    if (reader["TaskID"] != DBNull.Value)
                    {
                        Task task = new Task()
                            {
                                Id = int.Parse(reader["TaskID"].ToString()),
                                Category = category,
                                Description = (string)reader["TaskDescription"]
                            };
                        List<Task> list = (List<Task>)category.Tasks;
                        list.Add(task);
                    }

                    
                }
                return categories;
            }
        }

        public void AddTask(Task task)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "AddTask";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@CategoryId", task.Category.Id);

                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@taskId";
                parameter.DbType = DbType.Int32;
                parameter.Direction = ParameterDirection.Output;
                command.Parameters.Add(parameter);

                connection.Open();
                command.ExecuteNonQuery();
                task.Id = int.Parse(parameter.Value.ToString());
            }
        }

        public void DeleteTask(Task task)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Tasks WHERE Tasks.Id = @Id";
                command.Parameters.AddWithValue("@Id", task.Id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
