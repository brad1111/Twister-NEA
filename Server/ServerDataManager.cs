using System.Collections.Generic;

namespace Server
{
    public class ServerDataManager
    {
        private ServerDataManager()
        {
            
        }

        //The instance of this signleton
        public static ServerDataManager Instance { get; } = new ServerDataManager();

        //The storage of chracters
        public Character character1 = null;
        public Character character2 = null;

        
        public bool GameStarted => CharactersReady == 2; //Only start the game if both characters are ready
        public bool CharactersCollided { get; set; } = false; //Stores whether characters have collided
        public bool CharactersWon { get; set; } = false; //Stores whether characters have won
        public bool ClientCrashed { get; private set; } = false; //Stores whether someone has crashed
        public bool ClientLeft { get; private set; } = false; //Stores whether someone has quit (i.e. properly exited the game)
        public bool GameOver => CharactersWon || ClientCrashed || ClientLeft; //The game is over is someone has won/crashed/left
        public List<bool> ExitsOpen = new List<bool>(); //The storage for whether exits are open
        public Level.Level Level { get; set; } //Stores the level file

        public int CharactersReady { get; private set; } = 0; //The number of characters ready to play the game

        /// <summary>
        /// State that your charater has been started
        /// </summary>
        public void CharacterReady()
        {
            CharactersReady++;
        }

        /// <summary>
        /// Resets the fact that the clients are ready
        /// </summary>
        public void ResetGame()
        {
            CharactersReady = 0;
            CharactersCollided = false;
            CharactersWon = false;
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
        //The current angle of rotation
        public double currentAngle = 0;

        //The storage for the JSON serialized level text
        public string levelJson { get; set; }
    }
}