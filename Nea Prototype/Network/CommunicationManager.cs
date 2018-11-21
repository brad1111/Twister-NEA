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
            _connectionTimer.Tick += ConnectionTimer_Tick;
        }

        public static CommunicationManager Instance { get; } = new CommunicationManager();

        public int LocalCharacterNumber { get; private set; }

        public bool IsNetworked { get; private set; } = false;

        //Update the network every second to begin with
        private readonly DispatcherTimer _connectionTimer = new DispatcherTimer(){Interval = new TimeSpan(0,0,0,0,100)};

        /// <summary>
        /// Sets the enemy types required to do things with the network
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="et"></param>
        public bool SetupEnemyTypes(ProtagonistType pt, EnemyType et)
        {
            if (_connectionTimer.IsEnabled)
            {
                //If the timer is running return false because you couldn't update it
                return false;
            }

            if ((pt == ProtagonistType.Remote && et == EnemyType.Remote)
                || (pt != ProtagonistType.Remote && et != EnemyType.Remote))
            {
                throw new Exception("Error in CommunicationManager.SetupEnemyTypes(), only one remote character allowed.");
            }

            //If the enemy is remote, then player 1 is local, if enemy is not remote then player 1 is remote and enemy is local
            LocalCharacterNumber = et == EnemyType.Remote ? 1 : 2;
            //We know its networked
            IsNetworked = true;
            return true;
        }

        public bool ClearEnemyTypes()
        {
            if (_connectionTimer.IsEnabled)
            {
                //Can't change whilst running
                return false;
            }

            LocalCharacterNumber = -1;
            IsNetworked = false;
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
                                            Canvas.GetLeft(GameGridManager.Instance.CharactersViews[LocalCharacterNumber - 1]), //x
                                            Canvas.GetTop(GameGridManager.Instance.CharactersViews[LocalCharacterNumber - 1])); //y
            MessageManager.Instance.SendMessage(serverMsg);
        }

        public void Start()
        {
            if (!_connectionTimer.IsEnabled)
            {
                _connectionTimer.IsEnabled = true;
            }
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop()
        {
            if (_connectionTimer.IsEnabled)
            {
                _connectionTimer.IsEnabled = false;
            }
        }

        /// <summary>
        /// Disconnect everything from the server, assume message handler has been cleared
        /// </summary>
        public void Disconnect()
        {
            _connectionTimer.Stop();
            IsNetworked = false;
            //Also move the topframe back to main menu (on a different thread so just do it)
            TopFrameManager.FrameManager.MainFrame.Dispatcher.Invoke(new Action(() =>
            {
                while (TopFrameManager.FrameManager.MainFrame.CanGoBack)
                {
                    TopFrameManager.FrameManager.MainFrame.GoBack();
                }
            }));
            MessageManager.Instance.ClearServer();
            ClearEnemyTypes();
        }
    }
}