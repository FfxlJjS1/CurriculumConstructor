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

namespace CurriculumConstructor
{
    /// <summary>
    /// Логика взаимодействия для SettingMenuWindow.xaml
    /// </summary>
    public partial class SettingMenuWindow : Window
    {
        public SettingMenuWindow(string profileNumber, RowElement rowElement)
        {
            InitializeComponent();
            Nav.SettingMenuFrame = ContentFrame;
            this.Title = profileNumber + " - " + rowElement.Discipline;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SettingMenu.Items.Clear();
            List<OptionSettingMenu> optionSettingMenuList = new List<OptionSettingMenu>();
            int id = 0;
            AddOption("Предварительный просмотр документа");
            AddOption("Титульник");
            AddOption("Перечень планируемых результатов");
            AddOption("Тематический план дисциплины");
            AddOption("Промежуточная аттестация");
            AddOption("Тестовые задания для оценки уровня сформированности компетенций");
            AddOption("Критерии оценивания (лабораторные)");
            AddOption("Критерии оценивания (практика)");
            AddOption("Экзамен (вопросы)");
            AddOption("Экзамен (тест)");
            AddOption("Дополнительные баллы");
            SettingMenu.ItemsSource = optionSettingMenuList;
                

            void AddOption(string text)
            {
                OptionSettingMenu optionSettingMenu = new OptionSettingMenu();
                id++;
                optionSettingMenu.Id = id;
                optionSettingMenu.Text = text;  
                optionSettingMenuList.Add(optionSettingMenu);
            }
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void SettingMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OptionSettingMenu optionSettingMenu = SettingMenu.SelectedItem as OptionSettingMenu;
            if (optionSettingMenu == null)
            {
                return;
            }
            int id = optionSettingMenu.Id;
            if (id == 1)
            {

            }
            else if (id == 2)
            {
                ContentFrame.Navigate(new Title());
            }
            else if (id == 3)
            {
                ContentFrame.Navigate(new ListOfResultPage());
            }
            else if (id == 4)
            {
                ContentFrame.Navigate(new PlanOfDisciplinesPage()); 
            }
            else if (id == 5)
            {
                ContentFrame.Navigate(new IntermediateCertificationPage());
            }
            else if (id == 6)
            {
                ContentFrame.Navigate(new TestTaskPage(ТипТеста.модуль));
            }
            else if (id ==7)
            {
                ContentFrame.Navigate(new CriterionEvaluationPage(КритерийОценивания.Теория));
            }
            else if (id == 8)
            {
                ContentFrame.Navigate(new CriterionEvaluationPage(КритерийОценивания.Практика));
            }
            else if (id == 9)
            {
                ContentFrame.Navigate(new ExamPage());
            }
            else if (id == 10)
            {
                ContentFrame.Navigate(new TestTaskPage(ТипТеста.экзамен));
            }
            else if (id == 11)
            {
                ContentFrame.Navigate(new ExtraPointsPage());
            }
            else
            {
                return;
            }
        }
    }
}
