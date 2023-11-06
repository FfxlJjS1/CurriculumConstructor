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

        public SettingMenuWindow((string, string) Block_Part, TitleData titleData, DisciplineRow disciplineRow)
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
                var helper = new WordHelper("shablon.docx", generalModel);

                helper.Process(true);
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
            else if (senderName == intermediateCertificationMenuButton.Name)
            {
                ContentFrame.Navigate(new IntermediateCertificationPage());
            }
            else if (senderName == testTasksMenuButton.Name)
            {
                ContentFrame.Navigate(new TestTaskPage(ТипТеста.модуль));
            }
            else if (senderName == evaluationCriteriaMenuButton.Name)
            {
                ContentFrame.Navigate(new CriterionEvaluationPage(КритерийОценивания.Теория));
            }
            else if (senderName == "")
            {
                ContentFrame.Navigate(new CriterionEvaluationPage(КритерийОценивания.Практика));
            }
            else if (senderName == examMenuButton.Name)
            {
                ContentFrame.Navigate(new ExamPage());
            }
            else if (senderName == "")
            {
                ContentFrame.Navigate(new TestTaskPage(ТипТеста.экзамен));
            }
            else if (senderName == additionalPointsMenuButton.Name)
            {
                ContentFrame.Navigate(new ExtraPointsPage());
            }
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
