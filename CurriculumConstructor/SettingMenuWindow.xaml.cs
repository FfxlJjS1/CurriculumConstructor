using CurriculumConstructor.Service;
using CurriculumConstructor.SettingMenu;
using CurriculumConstructor.SettingMenu.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TestWord;
using CurriculumConstructor.SettingMenu.Model;
using System.Text.Json;
using System.IO;
using System.Windows.Forms;

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
        }

        public SettingMenuWindow(GeneralModel _generalModel)
        {
            generalModel = _generalModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = generalModel.ProfileNumber + " - " + generalModel.DisciplineName;

            if (!generalModel.IsExam)
            {
                sampleQuestionsForCertificationMenuButton.Visibility = Visibility.Collapsed;
                sampleQuestionsForCertificationMenuButton.IsEnabled = false;
                examTestTasksVariantTemplateMenuButton.Visibility = Visibility.Collapsed;
                examTestTasksVariantTemplateMenuButton.IsEnabled = false;
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
                this.DragMove();
        }

        private void MeuButton_Checked(object sender, RoutedEventArgs e)
        {
            if(ContentFrame.Content is WordPreview)
            {
                (ContentFrame.Content as WordPreview).RemoveState();
            }

            System.Windows.Controls.RadioButton? radioButton = sender as System.Windows.Controls.RadioButton;

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

                string filePath = System.Windows.Forms.Application.StartupPath + "//" + "shablon_1.docx";

                {
                    var helper = new WordHelper("shablon.docx", ref generalModel);

                    helper.Process(true, filePath);
                }

                ContentFrame.Navigate(new WordPreview(filePath, ref generalModel));
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

        private async void btnSaveArgs_Click(object sender, RoutedEventArgs e)
        {
            // Выбор пути и сохранение
            using (var path_dialog = new SaveFileDialog())
                if (path_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Путь к директории
                    string path = path_dialog.FileName;

                    var options = new JsonSerializerOptions
                    {
                        IncludeFields = true,
                    };

                    string jsonString = JsonSerializer.Serialize(generalModel, options);

                    using (StreamWriter writer = new StreamWriter(path, false))
                    {
                        await writer.WriteLineAsync(jsonString);
                    }

                    System.Windows.MessageBox.Show("Успешное сохранение!");
                };
        }
    }
}
