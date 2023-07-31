using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CurriculumConstructor.SettingMenu.Model
{
    public class ExamModel : INotifyPropertyChanged
    {
        public static ExamModel Model { get; set; } = new ExamModel();

        
        /// <summary>
        /// Примерные вопросы
        /// </summary>
        public List<string> Questions = new List<string>();

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
