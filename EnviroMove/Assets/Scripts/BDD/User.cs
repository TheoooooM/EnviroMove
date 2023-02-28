namespace BDD
{
    public class User
    {
        public string name;
        public int gold;

        public User()
        {
            name = "Guest";
            gold = 0;
        }
        
        public User(string name, int gold)
        {
            this.name = name;
            this.gold = gold;
        }
    }
}
