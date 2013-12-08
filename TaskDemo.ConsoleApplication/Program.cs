using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskDemo.DataLayer;
using TaskDemo.Entities;
using Task = TaskDemo.Entities.Task;

namespace TaskDemo.ConsoleApplication
{
    class Program
    {

        private static TaskDatabaseManager _manager =
            new TaskDatabaseManager(Settings.Default.ConnectionString);
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("Hello, Hit 1 for Categories Menu, 2 for Tasks Menu");
                if (Console.ReadLine() == "1")
                {
                    ShowCategoriesMenu();
                }
                else
                {
                    ShowTasksMenu();
                }
                Console.WriteLine("Hit q to quit");
            } while (Console.ReadKey().Key != ConsoleKey.Q);

        }

        private static void ShowCategoriesMenu()
        {
            Console.WriteLine("Press 1 to add a new Category");
            string response = Console.ReadLine();
            if (response == "1")
            {
                Console.WriteLine("Enter a category name:");
                string name = Console.ReadLine();
                Category category = new Category { Name = name };
                _manager.AddCategory(category);
                Console.WriteLine("New category Added, Id: " + category.Id);
            }
        }

        private static void ShowTasksMenu()
        {
            Console.WriteLine("Press 1 to add a task, 2 to delete a task");
            string response = Console.ReadLine();
            if (response == "1")
            {
                Category theCategory = GetCategory();
                Console.WriteLine("Enter a task description:");
                string description = Console.ReadLine();
                Task task = new Task { Description = description, Category = theCategory };
                _manager.AddTask(task);
                Console.WriteLine("Task created, Id: " + task.Id);
            }
            else if (response == "2")
            {
                Category category = GetCategory();
                int i = 1;
                foreach (Task task in category.Tasks)
                {
                    Console.WriteLine("{0}. {1}", i, task.Description);
                    i++;
                }

                Console.WriteLine("Choose a task to delete from the list above");
                int selectedId = int.Parse(Console.ReadLine());
                Task selectedTask = category.Tasks.ElementAt(selectedId - 1);
                _manager.DeleteTask(selectedTask);
            }
        }

        private static Category GetCategory()
        {
            IEnumerable<Category> categories = _manager.GetCategories();
            int i = 1;
            foreach (Category category in categories)
            {
                Console.WriteLine("{0}. {1}", i, category.Name);
                i++;
            }
            Console.WriteLine("Choose a category from the list above");
            int categoryChosen = int.Parse(Console.ReadLine());
            Category theCategory = categories.ElementAt(categoryChosen - 1);
            return theCategory;
        }
    }
}
