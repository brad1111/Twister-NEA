using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using Common.Enums;
using Nea_Prototype.Grid;

namespace Nea_Prototype.Network
{
    public class CommunicationManager
    {
        /// <summary>
        /// Don't allow external instances
        /// </summary>
        private CommunicationManager()
        {
            connectionTimer.Tick += ConnectionTimer_Tick;
        }

        private CommunicationManager Instance { get; } = new CommunicationManager();

        public ProtagonistType ProtagonistType { get; private set; }
        public EnemyType EnemyType { get; private set; }

        public int LocalCharacterNumber { get; private set; }

        public bool IsNetworked { get; private set; } = false;

        //Update the network every second to begin with
        private readonly DispatcherTimer connectionTimer = new DispatcherTimer(){Interval = new TimeSpan(0,0,1)};

        /// <summary>
        /// Sets 
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="et"></param>
        public bool SetupEnemyTypes(ProtagonistType pt, EnemyType et)
        {
            if (connectionTimer.IsEnabled)
            {
                //If the timer is running return false because you couldn't update it
                return false;
            }

            if ((pt == ProtagonistType.Remote && et == EnemyType.Remote)
                || (pt != ProtagonistType.Remote && et != EnemyType.Remote))
            {
                throw new Exception("Error in CommunicationManager.SetupEnemyTypes(), only one remote character allowed.");
            }

            ProtagonistType = pt;
            EnemyType = et;
            //If the enemy is remote, then player 1 is local, if enemy is not remote then player 1 is remote and enemy is local
            LocalCharacterNumber = et == EnemyType.Remote ? 1 : 2;
            //Start the timer since the enemies have been setup
            connectionTimer.Start();
            IsNetworked = true;
            return true;
        }

        /// <summary>
        /// The logic for communicating to the server
        /// </summary>
        private void ConnectionTimer_Tick(object sender, EventArgs e)
        {
            //Setup the message
            string serverMsg = String.Format("{0},{1},{2},",
                                            LocalCharacterNumber, //character number
                                            Canvas.GetLeft(GameGridManager.GetGameGrid().CharactersViews[LocalCharacterNumber]), //x
                                            Canvas.GetTop(GameGridManager.GetGameGrid().CharactersViews[LocalCharacterNumber])); //y
            MessageManager.Instance.SendMessage(serverMsg);
        }

        public void Start()
        {
            if (!connectionTimer.IsEnabled)
            {
                connectionTimer.IsEnabled = true;
            }
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            if (connectionTimer.IsEnabled)
            {
                connectionTimer.IsEnabled = false;
            }
        }
    }
}