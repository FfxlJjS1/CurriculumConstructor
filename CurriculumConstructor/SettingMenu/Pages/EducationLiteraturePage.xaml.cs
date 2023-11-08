using CurriculumConstructor.SettingMenu.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для EducationLiteraturePage.xaml
    /// </summary>
    public partial class EducationLiteraturePage : Page
    {
        private GeneralModel generalModel;
        private List<GeneralModel.EducationLiteratureModelComplex.EducationLiteratureModel> _model;

        private GeneralModel.EducationLiteratureModelComplex.EducationLiteratureModel _educationLiterature;
        private bool IsEdit;

        public EducationLiteraturePage(ref GeneralModel generalModel)
        {
            InitializeComponent();

            this.generalModel = generalModel;

            tabControlEducLiters.SelectedIndex = 0;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Reload();
        }
        
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (IsEdit == false)
            {
                switch (tabControlEducLiters.SelectedIndex)
                {
                    case 0:
                        generalModel.EducationLiteraturesComplex.MainLiteratures.Add(_educationLiterature);
                        break;
                    case 1:
                        generalModel.EducationLiteraturesComplex.AdditionalLiteratures.Add(_educationLiterature);
                        break;
                    case 2:
                        generalModel.EducationLiteraturesComplex.EducationMethodicalLiteratures.Add(_educationLiterature);
                        break;
                    default:
                        return;
                        break;
                }
            }

            Reload();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void DelClick(object sender, RoutedEventArgs e)
        {
            if (EducationLiteratureListBox.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать ПО!");
                return;
            }

            switch (tabControlEducLiters.SelectedIndex)
            {
                case 0:
                    generalModel.EducationLiteraturesComplex.MainLiteratures.Remove(_educationLiterature);
                    break;
                case 1:
                    generalModel.EducationLiteraturesComplex.AdditionalLiteratures.Remove(_educationLiterature);
                    break;
                case 2:
                    generalModel.EducationLiteraturesComplex.EducationMethodicalLiteratures.Remove(_educationLiterature);
                    break;
                default:
                    return;
                    break;
            }

            Reload();
        }

        private void EducationLiteratureListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EducationLiteratureListBox.SelectedItem == null)
            {
                return;
            }

            IsEdit = true;

            GeneralModel.EducationLiteratureModelComplex.EducationLiteratureModel theme = 
                EducationLiteratureListBox.SelectedItem as GeneralModel.EducationLiteratureModelComplex.EducationLiteratureModel;

            _educationLiterature = theme;
            DataContext = _educationLiterature;
        }

        private void Reload()
        {
            switch (tabControlEducLiters.SelectedIndex)
            {
                case 0:
                    EducationLiteratureListBox.ItemsSource = generalModel.EducationLiteraturesComplex.MainLiteratures;
                    break;
                case 1:
                    EducationLiteratureListBox.ItemsSource = generalModel.EducationLiteraturesComplex.AdditionalLiteratures;
                    break;
                case 2:
                    EducationLiteratureListBox.ItemsSource = generalModel.EducationLiteraturesComplex.EducationMethodicalLiteratures;
                    break;
                default:
                    return;
                    break;
            }

            _educationLiterature = new GeneralModel.EducationLiteratureModelComplex.EducationLiteratureModel();

            IsEdit = false;

            EducationLiteratureListBox.SelectedItem = null;
            EducationLiteratureListBox.Items.Refresh();

            DataContext = _educationLiterature;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Reload();
        }
    }
}
