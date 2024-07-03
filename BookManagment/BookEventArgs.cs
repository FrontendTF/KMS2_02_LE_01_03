using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagment
{
    public class BookEventArgs : EventArgs
    {
        public Book Book { get; set; }
    }
}
