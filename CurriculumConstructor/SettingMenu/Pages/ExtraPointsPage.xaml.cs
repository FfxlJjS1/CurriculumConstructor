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
    /// Логика взаимодействия для ExtraPointsPage.xaml
    /// </summary>
    public partial class ExtraPointsPage : Page
    {
        public ExtraPointsPage()
        {
            InitializeComponent();
            TextBoxExtraPointText.Text = ExtraPointText;
        }
        public static string ExtraPointText { get; set; }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            ExtraPointText = TextBoxExtraPointText.Text;
        }
    }
}
