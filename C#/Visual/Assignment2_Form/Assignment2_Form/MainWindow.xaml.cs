using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Assignment2_Form
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateDateTime();
            LoadStudentData();
        }

        private void LoadStudentData()
        {
            // Simple array of anonymous objects to populate the DataGrid automatically
            StudentsDataGrid.ItemsSource = new[]
            {
                new { ID = "STU-2025-001", Name = "Ali Hassan", Department = "Computer Science", Semester = "5th Semester", Email = "ali.hassan@email.com", Phone = "0300-1234567", Gender = "Male", DOB = "12/03/2003" },
                new { ID = "STU-2025-002", Name = "Sara Khan", Department = "Software Engineering", Semester = "3rd Semester", Email = "sara.khan@email.com", Phone = "0301-2345678", Gender = "Female", DOB = "25/07/2004" },
                new { ID = "STU-2025-003", Name = "Usman Ahmed", Department = "Information Technology", Semester = "7th Semester", Email = "usman.ahmed@email.com", Phone = "0302-3456789", Gender = "Male", DOB = "18/11/2001" },
                new { ID = "STU-2025-004", Name = "Ayesha Malik", Department = "Computer Science", Semester = "1st Semester", Email = "ayesha.malik@email.com", Phone = "0303-4567890", Gender = "Female", DOB = "05/02/2005" },
                new { ID = "STU-2025-005", Name = "Bilal Mustafa", Department = "Artificial Intelligence", Semester = "5th Semester", Email = "bilal.mustafa@email.com", Phone = "0304-5678901", Gender = "Male", DOB = "30/09/2002" }
            };
        }

        private void UpdateDateTime()
        {
            // Format: "Friday, 9 May 2025" and "10:30:45 AM"
            DateTextBlock.Text = System.DateTime.Now.ToString("dddd, d MMMM yyyy");
            TimeTextBlock.Text = System.DateTime.Now.ToString("h:mm:ss tt");
        }
    }
}