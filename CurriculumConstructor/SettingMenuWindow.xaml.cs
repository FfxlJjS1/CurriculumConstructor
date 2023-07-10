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

            this.Title = profileNumber + " - " + rowElement.Discipline;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SettingMenu.Items.Clear();
            SettingMenu.ItemsSource = new List<IdName>() { new IdName() { Id = 1, Text = "Предварительный просмотр документа" } };
        }

        private class IdName
        {
            public int Id { get; set; }
            public string Text { get; set; }
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
