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
    /// Логика взаимодействия для ProgramListPage.xaml
    /// </summary>
    public partial class ProgramListPage : Page
    {
        private GeneralModel generalModel;
        private List<GeneralModel.SoftwareInfo> _model;

        private GeneralModel.SoftwareInfo _softwareInfo;
        private bool IsEdit;

        public ProgramListPage(ref GeneralModel generalModel)
        {
            InitializeComponent();

            this.generalModel = generalModel;
            this._model = generalModel.SoftwareInfos;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (IsEdit == false)
            {
                _model.Add(_softwareInfo);
            }

            Reload();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void DelClick(object sender, RoutedEventArgs e)
        {
            if (ProgramListListBox.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать ПО!");
                return;
            }

            _model.Remove(_softwareInfo);
            Reload();
        }

        private void ProgramListListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProgramListListBox.SelectedItem == null)
            {
                return;
            }

            IsEdit = true;

            GeneralModel.SoftwareInfo theme = ProgramListListBox.SelectedItem as GeneralModel.SoftwareInfo;

            _softwareInfo = theme;
            DataContext = _softwareInfo;
        }

        private void Reload()
        {
            ProgramListListBox.ItemsSource = _model;

            _softwareInfo = new GeneralModel.SoftwareInfo();

            IsEdit = false;

            ProgramListListBox.SelectedItem = null;
            ProgramListListBox.Items.Refresh();

            DataContext = _softwareInfo;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Reload();
        }
    }
}
