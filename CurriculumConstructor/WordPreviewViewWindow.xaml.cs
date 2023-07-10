﻿using System;
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
using System.IO;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using System.Windows.Xps.Packaging;

namespace CurriculumConstructor
{
    /// <summary>
    /// Логика взаимодействия для WordPreviewViewWindow.xaml
    /// </summary>
    public partial class WordPreviewViewWindow : System.Windows.Window
    {
        private string myFilePathName;

        public WordPreviewViewWindow(string filePathName)
        {
            InitializeComponent();

            myFilePathName = filePathName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string newXPSDocumentName = String.Concat(
                System.IO.Path.GetDirectoryName(myFilePathName), "\\", 
                System.IO.Path.GetFileNameWithoutExtension(myFilePathName), ".xps");

            // Set DocumentViewer.Document to XPS document
            documentViewer.Document = ConvertWordDocToXPSDoc(myFilePathName, newXPSDocumentName).GetFixedDocumentSequence();
        }

        private XpsDocument ConvertWordDocToXPSDoc(string wordDocName, string xpsDocName)
        {
            // Create a WordApplication and add Document to it
            Microsoft.Office.Interop.Word.Application

                wordApplication = new Microsoft.Office.Interop.Word.Application();

            wordApplication.Documents.Add(wordDocName);

            Document doc = wordApplication.ActiveDocument;

            // You must ensure you have Microsoft.Office.Interop.Word.Dll version 12.
            // Version 11 or previous versions do not have WdSaveFormat.wdFormatXPS option
            try
            {
                doc.SaveAs(xpsDocName, WdSaveFormat.wdFormatXPS);
                wordApplication.Quit();

                XpsDocument xpsDoc = new XpsDocument(xpsDocName, System.IO.FileAccess.Read);

                return xpsDoc;
            }
            catch (Exception exp)
            {
                string str = exp.Message;
            }

            return null;
        }
    }
}