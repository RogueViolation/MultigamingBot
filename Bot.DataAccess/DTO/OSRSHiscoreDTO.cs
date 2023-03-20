using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.DataAccess.DTO
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public int Score { get; set; }
    }

    public class OSRSHisccoreDTO
    {
        public List<Skill> Skills { get; set; }
        public List<Activity> Activities { get; set; }
    }

    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public int Level { get; set; }
        public int Xp { get; set; }
    }
}

