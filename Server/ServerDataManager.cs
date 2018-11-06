using System.Collections.Generic;

namespace Server
{
    public class ServerDataManager
    {
        private ServerDataManager()
        {
            
        }

        public static ServerDataManager Instance { get; } = new ServerDataManager();

        public Character character1 = new Character(1);
        public Character character2 = new Character(2);

        public bool GameStarted => CharactersReady == 2; //Only start the game if both characters are ready
        public bool CharactersCollided = false;
        public bool CharactersWon = false;
        public List<bool> ExitsOpen = new List<bool>();

        public int CharactersReady { get; private set; } = 0;

        /// <summary>
        /// State that your charater has been started
        /// </summary>
        public void CharacterReady()
        {
            CharactersReady++;
        }

        public double currentAngle = 0;

        public string levelJson { get; set; }
    }
}