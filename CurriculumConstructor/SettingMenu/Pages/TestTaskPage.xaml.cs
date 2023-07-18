using CurriculumConstructor.SettingMenu.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurriculumConstructor.SettingMenu.Pages
{
    /// <summary>
    /// Логика взаимодействия для TestTaskPage.xaml
    /// </summary>
    public partial class TestTaskPage : Page
    {
        public TestTaskPage()
        {
            InitializeComponent();
            _model = TestTasksModel.TestTasks;
            Reload();
        }

        private List<TestTasksModel> _model;
        private TestTasksModel _task;
        private bool IsEdit;
        private void Reload()
        {
            _task = new TestTasksModel();
            IsEdit = false;
            DataContext = _task;
            TestTaskListBox.ItemsSource = _model;
            TestTaskListBox.SelectedItem = null;
            TestTaskListBox.Items.Refresh();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (TestTaskListBox.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать запись!");
                return;
            }
            _model.Remove(_task);
            Reload();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (IsEdit == false)
            {
                _model.Add(_task);
            }
            Reload();
        }

        private void TestTaskListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestTaskListBox.SelectedItem == null)
            {
                return;
            }
            _task = TestTaskListBox.SelectedItem as TestTasksModel;
            DataContext = _task;
            IsEdit = true;

        }
    }
}
