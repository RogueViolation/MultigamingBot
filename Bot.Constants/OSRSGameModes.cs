namespace Bot.Constants
{
    public class OSRSGameModes
    {
        private readonly Dictionary<string, string> _gameModes;
        public OSRSGameModes()
        {
            _gameModes = new Dictionary<string, string>
                {
                { "main", "hiscore_oldschool" },
                { "ironman", "hiscore_oldschool_ironman" },
                { "ultimate", "hiscore_oldschool_ultimate" },
                { "hardcore", "hiscore_oldschool_hardcore_ironman" },
                { "deadman", "hiscore_oldschool_deadman" },
                { "seasonal", "hiscore_oldschool_seasonal" },
                { "tournament", "hiscore_oldschool_tournament" }
                };
        }
        public string GetGameMode(string gamemode) 
        {
            return _gameModes[gamemode];
        }
    }
}