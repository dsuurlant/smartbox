using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace SmartBox.Entity
{
    public class Template
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        [MaxLength(50)]
        public string name { get; set; }
        public string dosBoxConfigPath { get; set; }
    }
}
