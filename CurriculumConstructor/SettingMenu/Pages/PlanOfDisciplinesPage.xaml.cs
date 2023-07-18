using CurriculumConstructor.SettingMenu.Model;
using CurriculumConstructor.SettingMenu.Windows;
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
    /// Логика взаимодействия для PlanOfDisciplinesPage.xaml
    /// </summary>
    public partial class PlanOfDisciplinesPage : Page
    {
        public PlanOfDisciplinesPage(PlanOfDisciplinesModel model)
        {
            InitializeComponent();
            if (model == null)
            {
                _model = new PlanOfDisciplinesModel();
            }
            else
            {
                _model = model;
            }
           
        }
        private PlanOfDisciplinesModel _model;
        private ThemeDisciplines _themeDisciplines;
        private bool IsEdit;
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (IsEdit == false)
            {
                _model.ThemeDisciplines.Add(_themeDisciplines);
            }
            
            Reload();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void DelClick(object sender, RoutedEventArgs e)
        {
            if (ThemeDisciplinesListBox.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать тему!");
                return;
            }
            _model.ThemeDisciplines.Remove(_themeDisciplines);
            Reload();
        }

        private void ThemeDisciplinesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeDisciplinesListBox.SelectedItem == null)
            {
                return;
            }
            IsEdit = true;
            ThemeDisciplines theme = ThemeDisciplinesListBox.SelectedItem as ThemeDisciplines;
            _themeDisciplines = theme;
            DataContext = _themeDisciplines;
        }
        private void Reload()
        {
            ThemeDisciplinesListBox.ItemsSource = _model.ThemeDisciplines;
            _themeDisciplines = new ThemeDisciplines();
            IsEdit = false;
            ThemeDisciplinesListBox.SelectedItem = null;
            ThemeDisciplinesListBox.Items.Refresh();
            DataContext = _themeDisciplines;
           
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void ContentClick(object sender, RoutedEventArgs e)
        {
            if (ThemeDisciplinesListBox.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать тему!");
                return;
            }
            DisciplineContentWindow disciplineContentWindow = new DisciplineContentWindow(_themeDisciplines);
            disciplineContentWindow.ShowDialog();
        }
    }
}
