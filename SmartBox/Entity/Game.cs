using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBox.Entity
{
    public class Game
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        [MaxLength(50)]
        public string name { get; set; }
        [MaxLength(255)]
        public string fileName { get; set; }
        public int year { get; set; }
        [MaxLength(50)]
        public string publisher { get; set; }
        public bool installed { get; set; }
        public string installExecutable { get; set; }
        public string installDir { get; set; }        
        public string gameExecutable { get; set; }
        public int templateId { get; set; }

        public override string ToString()
        {
            return fileName;
        }
    }
}
