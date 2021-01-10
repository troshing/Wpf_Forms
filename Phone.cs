using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_Forms
{
    public class Phone
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public int Price { get; set; }
        public List<string> Status { get; set; }
        public string StateType { get; set; }
        public static ushort idPhone { get; set; }

        public Phone()
        {
            Status = new List<string>();
            // idPhone++;
        }

        public static void AddId()
        {
            idPhone++;
        }

        public static void DeleteId()
        {
            idPhone--;
        }
    }
}
