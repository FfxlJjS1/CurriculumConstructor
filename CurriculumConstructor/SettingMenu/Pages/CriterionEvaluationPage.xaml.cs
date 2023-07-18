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
    /// Логика взаимодействия для CriterionEvaluationPage.xaml
    /// </summary>
    public partial class CriterionEvaluationPage : Page
    {
        public CriterionEvaluationPage(КритерийОценивания критерийОценивания)
        {
            InitializeComponent();
            if (критерийОценивания == КритерийОценивания.Теория)
            {
                _model = CriterionEvaluationModel.CriterionEvaluationLab;
            }
            else if (критерийОценивания == КритерийОценивания.Практика)
            {
                _model = CriterionEvaluationModel.CriterionEvaluationPractic;
            }
            else
            {
                throw new Exception("Магия или расширение?");
            }
            DataContext = _model;
            Reload();
        }
        CriterionEvaluationModel _model;
        string _question;
        private void Reload()
        {
            QuestionListBox.ItemsSource = _model.Questions;
            QuestionListBox.Items.Refresh();
        }
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            _question = QuestionTextBox.Text;
            _model.Questions.Add(_question);
            Reload();
            QuestionTextBox.Text = "";
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (QuestionListBox.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать строку!");
                return;
            }
            _model.Questions.Remove(_question);
            Reload();
        }

        private void QuestionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestionListBox.SelectedItem == null)
            {
                return;
            }
            _question = QuestionListBox.SelectedItem.ToString();    
        }
    }
    // а хренакнука сюда enum, так еще и на русском, а че бы нет?
    public enum КритерийОценивания
    {
        Теория,
        Практика,
    }
}
