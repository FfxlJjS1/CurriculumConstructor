using CurriculumConstructor.SettingMenu.Model;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Xps.Packaging;
using TestWord;

namespace CurriculumConstructor.SettingMenu.Pages
{
    /// <summary>
    /// Логика взаимодействия для WordPreview.xaml
    /// </summary>
    public partial class WordPreview : System.Windows.Controls.Page
    {
        private GeneralModel generalModel;

        private string myFilePathName;
        private string newXPSDocumentName;
        private XpsDocument xpsDoc;

        public WordPreview(string filePathName, ref GeneralModel generalModel)
        {
            InitializeComponent();

            myFilePathName = filePathName;

            this.generalModel = generalModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            newXPSDocumentName = myFilePathName + ".xps";

            {
                FileInfo fileInf = new FileInfo(newXPSDocumentName);

                if (fileInf.Exists)
                    fileInf.Delete();
            }

            // Set DocumentViewer.Document to XPS document
            documentViewer.Document = ConvertWordDocToXPSDoc(myFilePathName, newXPSDocumentName).GetFixedDocumentSequence();
        }

        private XpsDocument ConvertWordDocToXPSDoc(string wordDocName, string xpsDocName)
        {
            // Create a WordApplication and add Document to it
            Microsoft.Office.Interop.Word.Application wordApplication = new Microsoft.Office.Interop.Word.Application();

            // wordApplication.Documents.Open(wordDocName, ReadOnly: true);
            wordApplication.Documents.Add(wordDocName);

            Document doc = wordApplication.ActiveDocument;

            // You must ensure you have Microsoft.Office.Interop.Word.Dll version 12.
            // Version 11 or previous versions do not have WdSaveFormat.wdFormatXPS option
            try
            {
                doc.SaveAs(xpsDocName, WdSaveFormat.wdFormatXPS);
                wordApplication.Quit();

                xpsDoc = new XpsDocument(xpsDocName, System.IO.FileAccess.Read);

                return xpsDoc;
            }
            catch (Exception exp)
            {
                string str = exp.Message;
            }

            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var helper = new WordHelper("shablon.docx", ref generalModel);

            helper.Process(false);
        }

        public void RemoveState()
        {
            xpsDoc.Close();

            FileInfo fileInf = new FileInfo(newXPSDocumentName);

            if (fileInf.Exists)
                fileInf.Delete();

            fileInf = new FileInfo(myFilePathName);

            if (fileInf.Exists)
                fileInf.Delete();
        }
    }
}
