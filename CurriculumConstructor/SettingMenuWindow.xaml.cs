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

        private void btnShowSubElementClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
