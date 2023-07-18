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
using System.Windows.Shapes;

namespace CurriculumConstructor.SettingMenu.Windows
{
    /// <summary>
    /// Логика взаимодействия для DisciplineContentWindow.xaml
    /// </summary>
    public partial class DisciplineContentWindow : Window
    {
        public DisciplineContentWindow(ThemeDisciplines tD)
        {
            InitializeComponent();
            this.Title = $"{tD.Theme} ({tD.AllHour} ч.)";
            _themeDisciplines = tD ;
            if (tD.disciplinaryModules == null) _themeDisciplines.disciplinaryModules = new List<DisciplinaryModule>();
            Reload();
        }
        private ThemeDisciplines _themeDisciplines;
        private DisciplinaryModule _disciplinaryModule;
        private bool IsEdit;

        private void Reload()
        {
            ThemeListBox.ItemsSource = _themeDisciplines.disciplinaryModules;
            ThemeListBox.Items.Refresh();
            ThemeListBox.SelectedItem = null;
            _disciplinaryModule = new DisciplinaryModule();
            IsEdit = false;
            DataContext = _disciplinaryModule;
        }
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (IsEdit == false)
            {
                _themeDisciplines.disciplinaryModules.Add(_disciplinaryModule);
            }
            Reload();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            _themeDisciplines.disciplinaryModules.Remove(_disciplinaryModule);
            Reload();
        }
        private void AddClick(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void ThemeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeListBox.SelectedItem == null)
            {
                return;
            }
            _disciplinaryModule = ThemeListBox.SelectedItem as DisciplinaryModule;
            DataContext = _disciplinaryModule;
            IsEdit = true;
        }
    }
}
