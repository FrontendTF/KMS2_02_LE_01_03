﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagment
{
    /// <summary>
    /// Klasse Buch 
    /// </summary>
    public class Book
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Status { get; set; }
    }
}
