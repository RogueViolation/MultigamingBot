using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.DataAccess.DTO
{
    public class WOMLookupDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public string Build { get; set; }
        public string Country { get; set; }
        public bool Flagged { get; set; }
        public int Exp { get; set; }
        public double Ehp { get; set; }
        public double Ehb { get; set; }
        public double Ttm { get; set; }
        public double Tt200m { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LastChangedAt { get; set; }
        public DateTime LastImportedAt { get; set; }
    }
}
