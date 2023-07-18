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
    /// Промежутончая аттестация
    /// </summary>
    public class IntermediateCertificationModel : INotifyPropertyChanged
    {
        public static IntermediateCertificationModel IntermediateCertification = new IntermediateCertificationModel();

        private string title;
        private string description;

        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        public string Description
        {
            get { return description; }
            set
            {
                description = value; OnPropertyChanged(nameof(Description));
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
