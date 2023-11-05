﻿using CurriculumConstructor.SettingMenu.Model;
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
    /// Логика взаимодействия для ListOfResultPage.xaml
    /// </summary>
    public partial class ListOfResultPage : Page
    {
        private GeneralModel generalModel;
        public ListOfResultPage(ref GeneralModel generalModel)
        {
            InitializeComponent();

            this.generalModel = generalModel;

            ComboboxOPKs.ItemsSource = generalModel.competencyPlanningResults.Select(x => new
            {
                x.Code,
                x.CodeName
            });
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboboxOPKs_Selected(object sender, RoutedEventArgs e)
        {
            this.DataContext = generalModel.competencyPlanningResults[(sender as ComboBox).SelectedIndex];
        }
    }
}
