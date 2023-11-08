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
    /// Логика взаимодействия для ProffecionalDatabasePage.xaml
    /// </summary>
    public partial class ProffecionalDatabasePage : Page
    {
        private GeneralModel generalModel;
        private List<GeneralModel.LiteratureModel> _model;

        private GeneralModel.LiteratureModel _literatureModel;
        private bool IsEdit;

        public ProffecionalDatabasePage(ref GeneralModel generalModel)
        {
            InitializeComponent();

            this.generalModel = generalModel;
            this._model = generalModel.SiteList;
        }
        
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (IsEdit == false)
            {
                _model.Add(_literatureModel);
            }

            Reload();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void DelClick(object sender, RoutedEventArgs e)
        {
            if (ProffecionalDatabaseListBox.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать ПО!");
                return;
            }

            _model.Remove(_literatureModel);
            Reload();
        }

        private void ProffecionalDatabaseListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProffecionalDatabaseListBox.SelectedItem == null)
            {
                return;
            }

            IsEdit = true;

            GeneralModel.LiteratureModel theme = ProffecionalDatabaseListBox.SelectedItem as GeneralModel.LiteratureModel;

            _literatureModel = theme;
            DataContext = _literatureModel;
        }

        private void Reload()
        {
            ProffecionalDatabaseListBox.ItemsSource = _model;

            _literatureModel = new GeneralModel.LiteratureModel();

            IsEdit = false;

            ProffecionalDatabaseListBox.SelectedItem = null;
            ProffecionalDatabaseListBox.Items.Refresh();

            DataContext = _literatureModel;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Reload();
        }
    }
}
