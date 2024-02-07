using CurriculumConstructor.SettingMenu.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Логика взаимодействия для AssessmentToolsPage.xaml
    /// </summary>
    public partial class AssessmentToolsPage : Page
    {
        private GeneralModel generalModel;
        private GeneralModel.EvaluationCriteriesClass criteries;
        private GeneralModel.EvaluationCriteriesClass.LaboratoryEvaluationClass.QuestionCodeClass questionCode = new GeneralModel.EvaluationCriteriesClass.LaboratoryEvaluationClass.QuestionCodeClass();

        private bool IsEdit;

        public AssessmentToolsPage(ref GeneralModel generalModel)
        {
            InitializeComponent();

            this.generalModel = generalModel;
            criteries = generalModel.EvaluationCriteries;

            comboBoxLaboratoryThemeText.ItemsSource = generalModel.DisciplineThematicPlan.SelectMany(semesterModuleDisciplinePlan =>
                semesterModuleDisciplinePlan.Value.DisciplineThematicPlan.SelectMany(disciplineTheme =>
                    disciplineTheme.ThemeContents
                        .Where(theme => theme.ThemeType == GeneralModel.SemesterModuleData.DisciplineThematicTheme.ThemeContent.ThemeTypeEnum.LaboratoryWork)
                )
            ).ToList();

            comboBoxLabCompetencyCode.ItemsSource = generalModel.DisciplineCompetencies;
            comboBoxPracCompetencyCode.ItemsSource = generalModel.DisciplineCompetencies;

            DataContext = generalModel.EvaluationCriteries;

            if(generalModel.NeedTotalLaboratoryWorkHours <= 0)
            {
                tabItemLaboratoryAssestmToool.Visibility = Visibility.Collapsed;
                tabItemLaboratoryAssestmToool.IsEnabled = false;

                tabControlAssessmentTools.SelectedIndex = 1;
            }

            if(generalModel.NeedTotalPracticeHours <= 0)
            {
                tabItemPracticeAssestmToool.Visibility = Visibility.Collapsed;
                tabItemPracticeAssestmToool.IsEnabled = false;

                tabControlAssessmentTools.SelectedIndex = 0;
            }
        }

        private void QuestionCodeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestionCodeListBox.SelectedItem == null)
                return;

            questionCode = QuestionCodeListBox.SelectedItem as GeneralModel.EvaluationCriteriesClass.LaboratoryEvaluationClass.QuestionCodeClass;

            textBoxLabQuestionText.DataContext = questionCode;

            IsEdit = true;
        }

        private void SaveQuestionToLab(object sender, RoutedEventArgs e)
        {
            if (IsEdit)
                return;

            if (questionCode.Question.Length <= 0)
            {
                MessageBox.Show("Введите вопрос к защите");

                return;
            }

            criteries.laboratory.QuestionsCodeExampleForDefenceLabWork.Add(questionCode);

            QuestionCodeListBox.Items.Refresh();
        }

        private void DelQuestionToLab(object sender, RoutedEventArgs e)
        {
            if (QuestionCodeListBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите вопрос для исключения");

                return;
            }

            var questionForLaboratoryToDel = QuestionCodeListBox.SelectedItem as GeneralModel.EvaluationCriteriesClass.LaboratoryEvaluationClass.QuestionCodeClass;

            criteries.laboratory.QuestionsCodeExampleForDefenceLabWork.Remove(questionForLaboratoryToDel);

            QuestionCodeListBox.Items.Refresh();
        }

        private void AddQuestionToLab(object sender, RoutedEventArgs e)
        {
            questionCode = new GeneralModel.EvaluationCriteriesClass.LaboratoryEvaluationClass.QuestionCodeClass();

            IsEdit = false;
        }
    }
}
