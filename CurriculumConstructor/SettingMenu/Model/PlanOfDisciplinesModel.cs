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
    /// Тематический план дисциплины
    /// </summary>
    public class PlanOfDisciplinesModel
    {
        public PlanOfDisciplinesModel() 
        {
            ThemeDisciplines = new List<ThemeDisciplines>();
        }   
        /// <summary>
        /// Список тем дисциплин
        /// </summary>
        public List<ThemeDisciplines> ThemeDisciplines
        {
           get; set;
        }

        /// <summary>
        /// Сумма часов
        /// </summary>
        public int Lection
        {
            get
            {
               return ThemeDisciplines.Sum(q => q.Lecture);
            }
        }
        public int Practice
        {
            get
            {
                return ThemeDisciplines.Sum(q => q.Practice);
            }
        }
        public int Lab
        {
            get
            {
                return ThemeDisciplines.Sum(q => q.Lab);
            }
        }
        public int Crc
        {
            get
            {
                return ThemeDisciplines.Sum(q => q.Crc);
            }
        }

        
    }
}
