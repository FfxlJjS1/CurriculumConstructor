using CurriculumConstructor.Service;
using CurriculumConstructor.SettingMenu;
using CurriculumConstructor.SettingMenu.Pages;
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
using TestWord;
using CurriculumConstructor.SettingMenu.Model;

namespace CurriculumConstructor
{
    /// <summary>
    /// Логика взаимодействия для SettingMenuWindow.xaml
    /// </summary>
    public partial class SettingMenuWindow : Window
    {
        GeneralModel generalModel;

        public SettingMenuWindow((string, string) Block_Part, TitleDataClass titleData, DisciplineRow disciplineRow)
        {
            InitializeComponent();
            Nav.SettingMenuFrame = ContentFrame;

            generalModel = new GeneralModel(Block_Part, titleData, disciplineRow);

            this.Title = generalModel.ProfileNumber + " - " + generalModel.DisciplineName;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
                this.DragMove();
        }

        private void MeuButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton? radioButton = sender as RadioButton;

            if(radioButton == null)
                return;

            var senderName = radioButton.Name;

            if (senderName == previewViewMenuButton.Name)
            {
                if (!generalModel.CheckModelForCorrect())
                {
                    previewViewMenuButton.IsChecked = false;

                    return;
                }

                var helper = new WordHelper("shablon.docx", ref generalModel);

                helper.Process(true);

                ContentFrame.Navigate(new WordPreview("shablon_1.docx", ref generalModel));
            }
            else if (senderName == titleMenuButton.Name)
            {
                ContentFrame.Navigate(new TitlePage(ref generalModel));
            }
            else if (senderName == planningResultMenuButton.Name)
            {
                ContentFrame.Navigate(new ListOfResultPage(ref generalModel));
            }
            else if (senderName == thematicPlanMenuButton.Name)
            {
                ContentFrame.Navigate(new PlanOfDisciplinesPage(ref generalModel));
            }
            else if(senderName == assessmentToolsMenuButton.Name)
            {
                ContentFrame.Navigate(new AssessmentToolsPage(ref generalModel));
            }
            else if(senderName == testTasksForEvaulationCompetenciesMenuButton.Name)
            {
                ContentFrame.Navigate(new TestTasksForDetermineTheLevelOfCompetenciesPage(ref generalModel));
            }
            else if(senderName == sampleQuestionsForCertificationMenuButton.Name)
            {
                ContentFrame.Navigate(new SampleQuestionsForCertificationPage(ref generalModel));
            }
            else if(senderName == examTestTasksVariantTemplateMenuButton.Name)
            {
                ContentFrame.Navigate(new ExamTestTasksVariantTemplatePage(ref generalModel));
            }
            else if (senderName == educationLiteratureMenuButton.Name)
            {
                ContentFrame.Navigate(new EducationLiteraturePage(ref generalModel));
            }
            else if (senderName == proffesionalBaseMenuButton.Name)
            {
                ContentFrame.Navigate(new ProffecionalDatabasePage(ref generalModel));
            }
            else if(senderName == programListMenuButton.Name)
            {
                ContentFrame.Navigate(new ProgramListPage(ref generalModel));
            }
            else if (senderName == materialTechnicalBaseMenuButton.Name)
            {
                ContentFrame.Navigate(new MaterialTechnicalBasePage(ref generalModel));
            }
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
