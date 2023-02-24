using QuizWithDatabase.models;
using Telegram.Bot.Types;
using File = System.IO.File;



namespace QuizWithDatabase.services
{
    class UserService
    {
        static string location = "../../../users.txt";

        public List<UserX> Users;

        public UserService() 
        {
            Read();
        }
        public UserX AddUser(long Id)
        {
            var user = new UserX()
            {
                ChatId = Id,
            };
            Users.Add(user);
            return user;
        }
        public UserX GetUser(long Id)
        {
            var user = Users.FirstOrDefault(u => u.ChatId== Id);
            if (user == null)
            {
                user = AddUser(Id);
                Save();
            }
            return user;
        }

        public bool CheckuserLogged(long id)
        {
            return Users.Any(U => U.ChatId == id );
        }

        public bool IsUserSigned(string login, string password)
        {
            return Users.Any(u => u.Login == login || u.Password == password);
        }


        public void Save()
        {
            Read();
            var lines = "";

            foreach (var user in Users)
            {
                lines += $"{user.Login},{user.Password},{user.ChatId}\n";
            }

            System.IO.File.WriteAllText(location, lines);
        }

        private void Read()
        {
            Users = new List<UserX>();

            if (File.Exists(location))
            {
                List<string> lines = System.IO.File.ReadAllLines(location).ToList();

                foreach (var line in lines)
                {
                    string[] elements = line.Split(',');

                    var user = new UserX()
                    {
                        Login = elements[0],
                        Password = elements[1],
                        ChatId = Convert.ToInt64(elements[2])
                    };

                    Users.Add(user);
                }
            }
        } 
    }
}
