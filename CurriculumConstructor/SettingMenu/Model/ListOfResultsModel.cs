using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CurriculumConstructor.SettingMenu.Model
{
    /// <summary>
    /// 1.Перечень планируемых результатов обучения по дисциплине, соотнесенных с планируемыми результатами освоения образовательной программы
    /// </summary>
    public class ListOfResultsModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Оцениваемые компетенции  (код, наименование)
        /// </summary>
        private string col1;
        /// <summary>
        /// Код и наименование индикатора (индикаторов) достижения компетенции
        /// </summary>
        private string col2;
        /// <summary>
        /// Результаты освоения компетенции (знать)
        /// </summary>
        private string row1;
        private string row2;
        private string row3;
        public string Col1
        {
            get { return col1; }
            set
            {
                col1 = value;
                OnPropertyChanged(nameof(Col1));
            }
        }
        public string Col2
        {
            get { return col2; }
            set
            {
                col2 = value; OnPropertyChanged(nameof(Col2));
            }
        }
        public string Row1
        {
            get => row1;
            set
            {
                row1 = value; OnPropertyChanged(nameof(Row1)); 
            }
        }
        public string Row2
        {
            get => row2;
            set
            {
                row2 = value; OnPropertyChanged(nameof(Row2));
            }
        }
        public string Row3
        {
            get => row3;
            set
            {
                row3 = value; OnPropertyChanged(nameof(Row3));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
