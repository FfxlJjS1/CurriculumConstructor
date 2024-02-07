using CurriculumConstructor.SettingMenu.Model;
using CurriculumConstructor.SettingMenu.Windows;
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
using static System.Net.Mime.MediaTypeNames;

namespace CurriculumConstructor.SettingMenu.Pages
{
    /// <summary>
    /// Логика взаимодействия для ListOfResultPage.xaml
    /// </summary>
    public partial class ListOfResultPage : Page
    {
        private GeneralModel generalModel;
        public ListOfResultPage(ref GeneralModel generalModel)
        {
            InitializeComponent();

            this.generalModel = generalModel;
        }

        private void ComboboxOPKs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var competencyPlanning = generalModel.competencyPlanningResults[(sender as ComboBox).SelectedIndex];

            this.DataContext = competencyPlanning;

            listBoxEnteredCompetencyIndicator.ItemsSource = competencyPlanning.CompetencyAchivmentIndicators
                .Select(x => new CodeIndicatorViewData { Code = x.Key, FullText = competencyPlanning.Code + "." + x.Key.ToString() + ". " + x.Value });
        }

        private void btnAddCodeIndicator_Click(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as GeneralModel.CompetencyPlanningResult;

            if (model == null)
            {
                return;
            }

            string CodeText = txtBoxCodeIndicatorCode.Text;

            int Code;
            string Text = txtBoxCodeIndicatorText.Text;

            if (!int.TryParse(CodeText, out Code))
            {
                MessageBox.Show("Введите числоой номер индикатора для его добавления");

                return;
            }

            if (Text.Length <= 0)
            {
                MessageBox.Show("Введите текст индикатора дисциплины");
                return;
            }

            if (model.CompetencyAchivmentIndicators.ContainsKey(Code))
            {
                MessageBox.Show("Индикатор с таким номером уже существует");

                return;
            }

            model.CompetencyAchivmentIndicators.Add(Code, Text);

            listBoxEnteredCompetencyIndicator.ItemsSource = model.CompetencyAchivmentIndicators
                .Select(x => new CodeIndicatorViewData { Code = x.Key, FullText = model.Code + "." + x.Key.ToString() + ". " + x.Value });

            listBoxEnteredCompetencyIndicator.Items.Refresh();
        }

        private class CodeIndicatorViewData
        {
            public int Code { get; set; }
            public string FullText { get; set; } = "";
        }

        private void btnRemoveCodeIndicator_Click(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as GeneralModel.CompetencyPlanningResult;

            if (model == null)
            {
                return;
            }

            var selectedItem = listBoxEnteredCompetencyIndicator.SelectedItem as CodeIndicatorViewData;

            if (selectedItem == null)
            {
                MessageBox.Show("Выберите индикатор для удаления");

                return;
            }

            model.CompetencyAchivmentIndicators.Remove(selectedItem.Code);

            listBoxEnteredCompetencyIndicator.ItemsSource = model.CompetencyAchivmentIndicators
                .Select(x => new CodeIndicatorViewData { Code = x.Key, FullText = model.Code + "." + x.Key.ToString() + ". " + x.Value });

            listBoxEnteredCompetencyIndicator.Items.Refresh();
        }

        private void btnAddToKnow_Click(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as GeneralModel.CompetencyPlanningResult;

            if (model == null)
            {
                return;
            }

            string text = txtBoxToKnow.Text;

            if (text.Length <= 0)
            {
                MessageBox.Show("Введите текст результата (знать)");

                return;
            }

            if (model.ToKnowResult.Contains(text))
            {
                MessageBox.Show("Результат (знать) с таким текстом уже существует");

                return;
            }

            model.ToKnowResult.Add(text);

            listBoxEnteredToKnow.Items.Refresh();
        }

        private void btnRemoveToKnow_Click(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as GeneralModel.CompetencyPlanningResult;

            if (model == null)
            {
                return;
            }

            var selectedItem = listBoxEnteredToKnow.SelectedItem as string;

            if (selectedItem == null)
            {
                MessageBox.Show("Выберите результат (знать) для удаления");

                return;
            }

            model.ToKnowResult.Remove(selectedItem);

            listBoxEnteredToKnow.Items.Refresh();
        }

        private void btnAddToAble_Click(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as GeneralModel.CompetencyPlanningResult;

            if (model == null)
            {
                return;
            }

            string text = txtBoxToAble.Text;

            if (text.Length <= 0)
            {
                MessageBox.Show("Введите текст результата (уметь)");
                return;
            }

            if (model.ToAbilityResult.Contains(text))
            {
                MessageBox.Show("Результат (уметь) с таким текстом уже существует");

                return;
            }

            model.ToAbilityResult.Add(text);

            listBoxEnteredToAble.Items.Refresh();
        }

        private void btnRemoveToAble_Click(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as GeneralModel.CompetencyPlanningResult;

            if (model == null)
            {
                return;
            }

            var selectedItem = listBoxEnteredToAble.SelectedItem as string;

            if (selectedItem == null)
            {
                MessageBox.Show("Выберите результат (уметь) для удаления");

                return;
            }

            model.ToAbilityResult.Remove(selectedItem);

            listBoxEnteredToAble.Items.Refresh();
        }

        private void btnAddToOwn_Click(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as GeneralModel.CompetencyPlanningResult;

            if (model == null)
            {
                return;
            }

            string text = txtBoxToOwn.Text;

            if (text.Length <= 0)
            {
                MessageBox.Show("Введите текст результата (владеть)");
                return;
            }

            if (model.ToOwnResult.Contains(text))
            {
                MessageBox.Show("Результат (владеть) с таким текстом уже существует");

                return;
            }

            model.ToOwnResult.Add(text);

            listBoxEnteredToOwn.Items.Refresh();
        }

        private void btnRemoveToOwn_Click(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as GeneralModel.CompetencyPlanningResult;

            if (model == null)
            {
                return;
            }

            var selectedItem = listBoxEnteredToOwn.SelectedItem as string;

            if (selectedItem == null)
            {
                MessageBox.Show("Выберите результат (владеть) для удаления");

                return;
            }

            model.ToOwnResult.Remove(selectedItem);

            listBoxEnteredToOwn.Items.Refresh();
        }

        private void btnModifyEvaluationCriteria_Click(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as GeneralModel.CompetencyPlanningResult;

            if (model == null)
            {
                return;
            }

            string senderName = (sender as Button).Name;
            int btnIdentificator =
                senderName == btnModifyEvaluationCriteriaToKnow.Name
                ? 1
                : senderName == btnModifyEvaluationCriteriaToAble.Name
                ? 2
                : senderName == btnModifyEvaluationCriteriaToOwn.Name
                ? 3
                : 0;

            if (btnIdentificator == 0)
            {
                return;
            }

            EvaluationCriteriesSetWindow evaluationCriteriesSetWindow = new EvaluationCriteriesSetWindow(ref model, btnIdentificator);

            evaluationCriteriesSetWindow.ShowDialog();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ComboboxOPKs.ItemsSource = generalModel.competencyPlanningResults.Select(x => x.Code).ToList();

            if (ComboboxOPKs.ItemsSource != null)
                ComboboxOPKs.SelectedIndex = 0;
        }
    }
}
