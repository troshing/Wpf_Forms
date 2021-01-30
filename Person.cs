using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_Forms
{
    [Serializable]
    public class Person
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public static ushort idPerson { get; set; }

        public Person()
        {
            //
        }

        public static void AddId()
        {
            idPerson++;
        }

        public static void DeleteId()
        {
            idPerson--;
        }
    }
}
