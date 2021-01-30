using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_Forms
{
    [Serializable]
    public class SerialezerXML
    {
        public List<Phone> phones { get; set; }
        public List<Person> persons { get; set; }


        public SerialezerXML()
        {
            phones = new List<Phone>();
            persons = new List<Person>();
        }
    }
}
