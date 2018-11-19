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
        public bool CharactersCollided { get; set; } = false;
        public bool CharactersWon { get; private set; } = false;
        public bool ClientCrashed { get; private set; } = false;
        public bool ClientLeft { get; private set; } = false;
        public bool GameOver => CharactersWon || ClientCrashed || this.ClientLeft;
        public List<bool> ExitsOpen = new List<bool>();

        public int CharactersReady { get; private set; } = 0;

        /// <summary>
        /// State that your charater has been started
        /// </summary>
        public void CharacterReady()
        {
            CharactersReady++;
        }

        /// <summary>
        /// Tell the application that the client has disconnected unexpectidly
        /// </summary>
        public void CharacterCrashed()
        {
            ClientCrashed = true;
        }

        /// <summary>
        /// Tell the application that a client has purposefully disconnected
        /// </summary>
        public void CharacterLeft()
        {
            ClientLeft = true;
        }

        public double currentAngle = 0;

        public string levelJson { get; set; }
    }
}