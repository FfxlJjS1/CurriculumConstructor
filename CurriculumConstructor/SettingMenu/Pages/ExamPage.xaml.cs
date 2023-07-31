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
    /// Логика взаимодействия для ExamPage.xaml
    /// </summary>
    public partial class ExamPage : Page
    {
        public ExamPage()
        {
            InitializeComponent();
            DataContext = _model;
            Reload();
        }
        private ExamModel _model = ExamModel.Model;
        string _question;

        private void QuestionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestionListBox.SelectedItem == null)
            {
                return;
            }
            _question = QuestionListBox.SelectedItem.ToString();
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
        private void Reload()
        {
            QuestionListBox.ItemsSource = _model.Questions;
            QuestionListBox.Items.Refresh();
        }

        private void TestListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DeleteTestClick(object sender, RoutedEventArgs e)
        {

        }

        private void SaveTestClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
