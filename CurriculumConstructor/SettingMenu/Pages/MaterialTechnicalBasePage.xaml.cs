using CurriculumConstructor.SettingMenu.Model;
using Microsoft.Office.Interop.Excel;
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
using static CurriculumConstructor.SettingMenu.Model.GeneralModel;

namespace CurriculumConstructor.SettingMenu.Pages
{
    /// <summary>
    /// Логика взаимодействия для MaterialTechnicalBasePage.xaml
    /// </summary>
    public partial class MaterialTechnicalBasePage : System.Windows.Controls.Page
    {
        private GeneralModel generalModel;
        private List<GeneralModel.PlaceTheirEquipmentsClass> placeTheirEquipmentsList;

        private GeneralModel.PlaceTheirEquipmentsClass placeTheirEquipments;
        private bool IsEdit;

        public MaterialTechnicalBasePage(ref GeneralModel generalModel)
        {
            InitializeComponent();

            this.generalModel = generalModel;
            this.placeTheirEquipmentsList = generalModel.PlaceTheirEquipments;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (IsEdit == false)
            {
                placeTheirEquipmentsList.Add(placeTheirEquipments);
            }

            Reload();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            Reload();
        }

        private void DelClick(object sender, RoutedEventArgs e)
        {
            if (PlaceEquipmentsListBox.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать ПО!");
                return;
            }

            placeTheirEquipmentsList.Remove(placeTheirEquipments);
            Reload();
        }
        
        private void AddEquipmentClick(object sender, RoutedEventArgs e)
        {
            if (txtBoxNewEquipment.Text == "")
            {
                MessageBox.Show("Нужно ввести новое оснащение!");
                return;
            }

            placeTheirEquipments.EquipmentsName.Add(txtBoxNewEquipment.Text);

            listBoxEquipments.Items.Refresh();

            txtBoxNewEquipment.Text = string.Empty;
        }
        
        private void DelEquipmentClick(object sender, RoutedEventArgs e)
        {
            if (listBoxEquipments.SelectedItem == null)
            {
                MessageBox.Show("Нужно выбрать оснащение помещения!");
                return;
            }

            placeTheirEquipments.EquipmentsName.Remove(listBoxEquipments.SelectedItem as string);

            listBoxEquipments.Items.Refresh();
        }

        private void PlaceEquipmentsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaceEquipmentsListBox.SelectedItem == null)
            {
                return;
            }

            IsEdit = true;

            GeneralModel.PlaceTheirEquipmentsClass theme = PlaceEquipmentsListBox.SelectedItem as GeneralModel.PlaceTheirEquipmentsClass;

            placeTheirEquipments = theme;
            DataContext = placeTheirEquipments;
        }

        private void Reload()
        {
            PlaceEquipmentsListBox.ItemsSource = placeTheirEquipmentsList;

            placeTheirEquipments= new GeneralModel.PlaceTheirEquipmentsClass();

            IsEdit = false;

            PlaceEquipmentsListBox.SelectedItem = null;
            PlaceEquipmentsListBox.Items.Refresh();

            DataContext = placeTheirEquipments;

            txtBoxNewEquipment.Text = string.Empty;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Reload();
        }
    }
}
