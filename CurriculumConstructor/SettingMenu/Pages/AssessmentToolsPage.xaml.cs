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
    /// Логика взаимодействия для AssessmentToolsPage.xaml
    /// </summary>
    public partial class AssessmentToolsPage : Page
    {
        private GeneralModel generalModel;
        private GeneralModel.EvaluationCriteriesClass criteries;

        public AssessmentToolsPage(ref GeneralModel generalModel)
        {
            InitializeComponent();

            this.generalModel = generalModel;
            criteries = generalModel.EvaluationCriteries;

            comboBoxLaboratoryThemeText.ItemsSource = generalModel.DisciplineThematicPlan.SelectMany(semesterModuleDisciplinePlan => 
                semesterModuleDisciplinePlan.Value.DisciplineThematicPlan.SelectMany(disciplineTheme => 
                    disciplineTheme.ThemeContents
                        .Where(theme => theme.ThemeType == GeneralModel.SemesterModuleData.DisciplineThematicTheme.ThemeContent.ThemeTypeEnum.LaboratoryWork)
                            .Select(theme => 
                                theme.ThemeText
                            )
                )
            ).ToList();

            comboBoxCompetencies.ItemsSource = generalModel.DisciplineCompetencies.Where(x => !criteries.laboratory.QuestionsCompetencies.Contains(x));

            DataContext = generalModel.EvaluationCriteries;
        }

        private void AddCompetency_Click(object sender, RoutedEventArgs e)
        {
            if(comboBoxCompetencies.SelectedItem == null)
            {
                MessageBox.Show("Выберите компетенцию для добавления");

                return;
            }


            string competencyForSelect = comboBoxCompetencies.SelectedItem.ToString();

            criteries.laboratory.QuestionsCompetencies.Add(competencyForSelect);

            comboBoxCompetencies.ItemsSource = generalModel.DisciplineCompetencies.Where(x => !criteries.laboratory.QuestionsCompetencies.Contains(x));

            listBoxCompetenciesCode.Items.Refresh();
        }

        private void DelCompetency_Click(object sender, RoutedEventArgs e)
        {
            if(listBoxCompetenciesCode.SelectedItem == null)
            {
                MessageBox.Show("Выберите компетенцию для исключения");

                return;
            }

            string competencyCodeForRemove = listBoxCompetenciesCode.SelectedItem.ToString();

            criteries.laboratory.QuestionsCompetencies.Remove(competencyCodeForRemove);

            comboBoxCompetencies.ItemsSource = generalModel.DisciplineCompetencies.Where(x => !criteries.laboratory.QuestionsCompetencies.Contains(x));

            listBoxCompetenciesCode.Items.Refresh();
        }

        private void listBoxQuestionsToLaboratory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(listBoxQuestionsToLaboratory.SelectedItem == null)
            {
                return;
            }

            textBoxQuestionForLab.Text = listBoxQuestionsToLaboratory.SelectedItem.ToString();
        }

        private void AddQuestionToLab(object sender, RoutedEventArgs e)
        {
            if (textBoxQuestionForLab.Text.Length <= 0)
            {
                MessageBox.Show("Введите вопрос к защите");

                return;
            }

            string questionForLabText = textBoxQuestionForLab.Text;

            if(criteries.laboratory.QuestionsExampleForDefenceLabWork.Contains(questionForLabText))
            {
                MessageBox.Show("Данные вопрос уже имеется в списке");

                return;
            }

            criteries.laboratory.QuestionsExampleForDefenceLabWork.Add(questionForLabText);

            listBoxQuestionsToLaboratory.Items.Refresh();
        }

        private void DelQuestionToLab(object sender, RoutedEventArgs e)
        {
            if (listBoxQuestionsToLaboratory.SelectedItem == null)
            {
                MessageBox.Show("Выберите компетенцию для исключения");

                return;
            }

            string questionForLaboratoryToDel = listBoxQuestionsToLaboratory.SelectedItem.ToString();

            criteries.laboratory.QuestionsExampleForDefenceLabWork.Remove(questionForLaboratoryToDel);

            listBoxQuestionsToLaboratory.Items.Refresh();
        }
    }
}
