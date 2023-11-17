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
using static CurriculumConstructor.SettingMenu.Model.GeneralModel.TestTasksClass;

namespace CurriculumConstructor.SettingMenu.Pages
{
    /// <summary>
    /// Логика взаимодействия для SampleQuestionsForCertificationPage.xaml
    /// </summary>
    public partial class SampleQuestionsForCertificationPage : Page
    {
        private GeneralModel generalModel;
        private List<GeneralModel.QuestionCodesClass> questions;

        private GeneralModel.QuestionCodesClass _model;
        private bool IsEdit;

        public SampleQuestionsForCertificationPage(ref GeneralModel generalModel)
        {
            InitializeComponent();

            this.generalModel = generalModel;
            questions = generalModel.QuestionCodes;

            listBoxAvailableCompetencies.ItemsSource = generalModel.DisciplineCompetencies;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (IsEdit == false)
            {
                questions.Add(_model);
            }

            Reload();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void DelClick(object sender, RoutedEventArgs e)
        {
            if (listBoxSampleQuestionsToExap.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать вопрос");
                return;
            }

            questions.Remove(_model);
            Reload();
        }

        private void Reload()
        {
            listBoxSampleQuestionsToExap.ItemsSource = questions;

            _model = new GeneralModel.QuestionCodesClass();

            IsEdit = false;

            listBoxSampleQuestionsToExap.SelectedItem = null;
            listBoxSampleQuestionsToExap.Items.Refresh();

            DataContext = _model;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void SelectCompetency_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxAvailableCompetencies.SelectedItem == null)
            {
                MessageBox.Show("Выберите компетенцию для добавления");

                return;
            }

            string selectedCompetency = listBoxAvailableCompetencies.SelectedItem as string;

            _model.Competencies.Add(selectedCompetency);

            listBoxAvailableCompetencies.ItemsSource = generalModel.DisciplineCompetencies.Where(x => !_model.Competencies.Contains(x)).ToList();

            listBoxAvailableCompetencies.Items.Refresh();
            listBoxSelectedCompetencies.Items.Refresh();
        }

        private void UnselectCompetency_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxSelectedCompetencies.SelectedItem == null)
            {
                MessageBox.Show("Выберите компетенции для исключения");

                return;
            }

            string competencyForUnselect = listBoxSelectedCompetencies.SelectedItem as string;

            _model.Competencies.Remove(competencyForUnselect);

            listBoxAvailableCompetencies.ItemsSource = generalModel.DisciplineCompetencies.Where(x => !_model.Competencies.Contains(x)).ToList();

            listBoxAvailableCompetencies.Items.Refresh();
            listBoxSelectedCompetencies.Items.Refresh();
        }

        private void listBoxSampleQuestionsToExap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxSampleQuestionsToExap.SelectedItem == null)
            {
                return;
            }

            IsEdit = true;

            GeneralModel.QuestionCodesClass questionCodes = listBoxSampleQuestionsToExap.SelectedItem as GeneralModel.QuestionCodesClass;

            _model = questionCodes;
            DataContext = _model;
        }
    }
}
