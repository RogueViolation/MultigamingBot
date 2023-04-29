namespace Bot.DataObjects
{
    public class OSRSUserBasic
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string GameMode { get; set; }

        public OSRSUserBasic()
        {
            Id = 0;
            UserName = "";
            GameMode = "";
        }
    }
    
}