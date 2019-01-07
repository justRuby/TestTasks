﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test_Server_1.Models
{
    public class NoteModel
    {
        public string NoteID { get; set; }

        public string Headline { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Image { get; set; }

        public bool IntegrityCheck()
        {
            if (NoteID != string.Empty &&
               Headline != string.Empty &&
               Description != string.Empty &&
               Date != string.Empty &&
               Image != string.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
