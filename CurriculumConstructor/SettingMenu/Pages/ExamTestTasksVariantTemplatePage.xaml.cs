using CurriculumConstructor.SettingMenu.Model;
using CurriculumConstructor.SettingMenu.Windows;
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
    /// Логика взаимодействия для ExamTestTasksVariantTemplatePage.xaml
    /// </summary>
    public partial class ExamTestTasksVariantTemplatePage : Page
    {
        private GeneralModel generalModel;
        private List<GeneralModel.TestTasksClass.TestTaskLine> _model;

        private int selesterSemesterNumber = -1;

        private List<CompetenciesComboBoxItem> _competenciesComboBoxItems = new List<CompetenciesComboBoxItem>();
        private GeneralModel.TestTasksClass.TestTaskLine _testTaskLine;
        private bool IsEdit;

        public class CompetenciesComboBoxItem
        {
            public string FullCompetenciesString => string.Join(", ", CompetenciesCode);
            public List<string> CompetenciesCode = new List<string>();
        }

        public ExamTestTasksVariantTemplatePage(ref GeneralModel generalModel)
        {
            InitializeComponent();

            this.generalModel = generalModel;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if(_model == null)
            {
                return;
            }

            if (IsEdit == false)
            {
                _model.Add(_testTaskLine);
            }

            Reload();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void DelClick(object sender, RoutedEventArgs e)
        {
            if (TestTasksListBox.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать тестовый вопрос!");
                return;
            }

            _model.Remove(_testTaskLine);
            Reload();
        }

        private void TestTasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestTasksListBox.SelectedItem == null)
            {
                return;
            }

            IsEdit = true;

            GeneralModel.TestTasksClass.TestTaskLine testTask = TestTasksListBox.SelectedItem as GeneralModel.TestTasksClass.TestTaskLine;

            _testTaskLine = testTask;
            DataContext = _testTaskLine;
        }

        private void Reload()
        {
            TestTasksListBox.ItemsSource = _model;

            _testTaskLine = new GeneralModel.TestTasksClass.TestTaskLine();

            IsEdit = false;

            TestTasksListBox.SelectedItem = null;
            TestTasksListBox.Items.Refresh();

            DataContext = _testTaskLine;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxCompetensiesVariantTestsSemester.ItemsSource = generalModel.Semesters.Select(x => x.SemesterNumber).ToList();
            comboBoxCompetensiesVariantTestsSemester.SelectedIndex = 0;

            Reload();
        }

        private void ComboBoxCompetenciesCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ComboBoxCompetenciesCode.SelectedItem == null)
            {
                return;
            }

            var competenciesComboboxItem = ComboBoxCompetenciesCode.SelectedItem as CompetenciesComboBoxItem;

            if (competenciesComboboxItem == null)
                return;

            _model = generalModel.ExamTestTasksVariantTemplate[selesterSemesterNumber][competenciesComboboxItem.CompetenciesCode];

            Reload();
        }

        private void AddCompetenciesList_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedCompetenciesCodeAsItem = new List<string>();

            CompetenciesCodeItemSelectWindow competenciesCodeItemSelect = 
                new CompetenciesCodeItemSelectWindow(generalModel.DisciplineCompetencies, ref selectedCompetenciesCodeAsItem);

            competenciesCodeItemSelect.ShowDialog();

            if(selectedCompetenciesCodeAsItem.Count <= 0 
                || generalModel.ExamTestTasksVariantTemplate[selesterSemesterNumber].Keys.ToList().Contains(selectedCompetenciesCodeAsItem))
            {
                return;
            }

            generalModel.ExamTestTasksVariantTemplate[selesterSemesterNumber].Add(selectedCompetenciesCodeAsItem, new List<GeneralModel.TestTasksClass.TestTaskLine>());

            _competenciesComboBoxItems.Add(new CompetenciesComboBoxItem() { CompetenciesCode = selectedCompetenciesCodeAsItem });

            ComboBoxCompetenciesCode.SelectedItem = selectedCompetenciesCodeAsItem;

            ComboBoxCompetenciesCode.Items.Refresh();

            Reload();
        }

        private void comboBoxCompetensiesVariantTestsSemester_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxCompetensiesVariantTestsSemester.SelectedValue == null)
                return;

            selesterSemesterNumber = (int)comboBoxCompetensiesVariantTestsSemester.SelectedValue;

            _competenciesComboBoxItems.AddRange(generalModel.ExamTestTasksVariantTemplate[selesterSemesterNumber].Select(
                x => new CompetenciesComboBoxItem()
                { CompetenciesCode = x.Key })
            );

            ComboBoxCompetenciesCode.ItemsSource = _competenciesComboBoxItems;
        }
    }
}
