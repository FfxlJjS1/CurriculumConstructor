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
        public SettingMenuWindow(DisciplineRow disciplineRow)
        {
            InitializeComponent();
            Nav.SettingMenuFrame = ContentFrame;

            TitleModel.Title.Index = disciplineRow.Index;
            TitleModel.Title.Discipline = disciplineRow.DisciplineName;

            this.Title = TitleModel.Title.ProfileNumber + " - " + disciplineRow.DisciplineName;
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
            OptionSettingMenu? optionSettingMenu = SettingMenu.SelectedItem as OptionSettingMenu;
            if (optionSettingMenu == null)
            {
                return;
            }
            int id = optionSettingMenu.Id;
            if (id == 1)
            {
                var helper = new WordHelper("shablon.docx");

                var items = new Dictionary<string, string>
                {
                    //EXCEL ИЛИ ПРОГРАММНО РАССЧИТАТЬ
                    //0-1
                    {"<YEAR>", "2023" },
                    {"<INDEX>", TitleModel.Title.Index },
                    {"<DISCIPLINE>", TitleModel.Title.Discipline }, //6, 6.3.1.1 (ЛАБЫ), 6.4, 11, аннотация
                    {"<DIRECTION>", TitleModel.Title.ProfileNumber + " " +  TitleModel.Title.ProfileName }, //2, 6.4, 12, аннотация
                    {"<PROFILE>", TitleModel.Title.ProfileName }, //2, 12, аннотация
                    {"<QUALIFICATION>", TitleModel.Title.Qualification },
                    {"<FORM_STUDY>", TitleModel.Title.EducationForm },
                    {"<LANGUAGE_STUDY>", "русский" },
                    {"<YEAR_START>", TitleModel.Title.StartYear },
                    //2
                    {"<BLOCK_1>", "Блока 1 \"Дисциплины (модули)\""},
                    {"<BLOCK_2>", "обязательной части"},
                    {"<COURSE_SEMESTER>", " 2 курсе в 4 семестре"},
                    //3
                    {"<TOILSOMENESS>", "4 зачетных единиц, 144 часов"},
                    {"<WORK>", "Контактная работа обучающихся с преподавателем - 58 часов:\r\n- лекции 16 ч.;\r\n- практические занятия 18 ч.;\r\n- лабораторные работы 18 ч.\r\nСамостоятельная работа 20ч.\r\nКонтроль (экзамен) 36 ч."},
                    {"<ATTESTATION>", "экзамен в 4 семестре"}, //6.4 //зачет с оценкой в 1, 2, 3 семестрах, экзамен в 4 семестре
                    //6
                    {"<ATTESTATION_2>", "экзамена"}, //зачета с оценкой (1, 2, 3 семестры) и экзамена (4 семестр)
                    //

                    //ВВОДИМЫЕ ДАННЫЕ
                    {"<AUTHOR>", TitleModel.Title.Author },
                    {"<REVIEWER>", TitleModel.Title.Reviewer },
                    {"<DEPARTMENT_CHAIR>", TitleModel.Title.Department_chair },
                    //5
                    //{"<METHOD_BOOK>", "Ситдикова И.П., Ахметзянов Р.Р. Метрология, стандартизация и сертификация: методические указания для выполнения лабораторных работ и организации самостоятельной работы по дисциплине «Метрология, стандартизация и сертификация» для бакалавров направления подготовки 15.03.04 «Автоматизация технологических процессов и производств» очной формы обучения. – Альметьевск: АГНИ, 2021г." }

                };

                helper.Process(items);
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
            else if (id == 7)
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
