using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Common;
using Common.Enums;
using Twister.Grid;
using Twister.Network;
using Twister.Pages;

namespace Twister.Algorithms
{
    public static class Collisions
    {
        private static GameGridManager _gridManager => GameGridManager.Instance;

        /// <summary>
        /// Checks whether a character will collide into a wall with their movement
        /// </summary>
        /// <param name="characterView">The actual characters view</param>
        /// <param name="movementDirection">The direction the character would move</param>
        /// <param name="canvas">The canvas to draw rectangles on if visualising the collision detection</param>
        /// <returns>Whether the character will collide</returns>
        public static bool WallCollisionDetection(ref GridItem characterView, Direction movementDirection)
        {
            double x, y = 0;
            x = Canvas.GetLeft(characterView);
            y = Canvas.GetTop(characterView);
            int xApprox, yApprox = 0;
            const double half_GRID_ITEM_WIDTH = Constants.GRID_ITEM_WIDTH / 2;
            Queue<GridItem> ItemsToCheckForCollision = new Queue<GridItem>(3);
            //Get approx co-ords
            xApprox = (int) Math.Floor((x + half_GRID_ITEM_WIDTH) / Constants.GRID_ITEM_WIDTH);
            yApprox = (int) Math.Floor((y + half_GRID_ITEM_WIDTH) / Constants.GRID_ITEM_WIDTH);
            switch (movementDirection)
            {
                case Direction.Up:
                    //get three possible collisionable items above
                    if (yApprox <= 0 && y < 0)
                    {
                        return RunWinDialogIfNeeded(ref characterView);
                    }
                    else
                    {
                        ItemsToCheckForCollision = QueuingLocationsToCheck(ref xApprox, ref yApprox, 0 , -1);
                        //Override y to be the moved value so that they can be checked for intersection
                        y -= Constants.KEYPRESS_PX_MOVED;
                    }
                    break;
                case Direction.Down:
                    //get three possible collisionable items below
                    if (yApprox >= Constants.GRID_TILES_XY && y + Constants.GRID_ITEM_WIDTH >= Constants.GRID_WIDTH)
                    {
                        //If outside of map then win
                        return RunWinDialogIfNeeded(ref characterView);
                    }
                    else
                    {
                        ItemsToCheckForCollision = QueuingLocationsToCheck(ref xApprox, ref yApprox, 0 , 1);

                        //Override y to be the moved value so that they can be checked for intersection
                        y += Constants.KEYPRESS_PX_MOVED;
                    }
                    break;
                case Direction.Left:
                    //get three possible collisionable items below
                    if (xApprox <= 0 && x <= 0)
                    {
                        //If they are outside the map
                        return RunWinDialogIfNeeded(ref characterView);
                    }
                    else
                    {
                        ItemsToCheckForCollision = QueuingLocationsToCheck(ref xApprox, ref yApprox, -1 , 0);

                        //Override x to be the moved value so that they can be checked for intersection
                        x -= Constants.KEYPRESS_PX_MOVED;
                    }

                    break;
                case Direction.Right:
                    //get three possible collisionable items below
                    if (xApprox >= Constants.GRID_TILES_XY && (x + Constants.GRID_ITEM_WIDTH) >= Constants.GRID_WIDTH)
                    {
                        return RunWinDialogIfNeeded(ref characterView);
                    }
                    else
                    {
                        ItemsToCheckForCollision = QueuingLocationsToCheck(ref xApprox, ref yApprox, 1, 0);

                        //Override x to be the moved value so that they can be checked for intersection
                        x += Constants.KEYPRESS_PX_MOVED;
                    }
                    break;
                default:
                    throw new NotImplementedException($"Direction value of {nameof(movementDirection)} is not implemented in Level.WallCollisionDetection()");
            }

            //Rectangle variable is to check to see if it intersects
            Rect characterRect = new Rect(x + 1, y + 1, Constants.GRID_ITEM_WIDTH - 2 , Constants.GRID_ITEM_WIDTH - 2);

            //Only display the rectangles if they are wanted
            if (_gridManager.WallCollisionRectangles)
            {
                Canvas canvas = _gridManager.GameCanvas;
                Rectangle charcterRectangle = new Rectangle()
                {
                    Width = characterRect.Width,
                    Height = characterRect.Height,
                    Fill = new SolidColorBrush(Colors.Blue)
                };

                canvas.Children.Add(charcterRectangle);
                Canvas.SetLeft(charcterRectangle, characterRect.Left);
                Canvas.SetTop(charcterRectangle, characterRect.Top);
                _gridManager.DebuggingCanvasLeftovers += ItemsToCheckForCollision.Count + 1;
            }

            bool collision = false;

            while (ItemsToCheckForCollision.Count > 0)
            {
                GridItem tempItem = ItemsToCheckForCollision.Dequeue();
                Rect nonWalkableRect = new Rect(Canvas.GetLeft(tempItem), Canvas.GetTop(tempItem), Constants.GRID_ITEM_WIDTH, Constants.GRID_ITEM_WIDTH);
                if (nonWalkableRect.IntersectsWith(characterRect) || collision /*== true*/)
                {
                    collision = true;
                }

                //Only display rectangles if they are wanted to debug
                if (_gridManager.WallCollisionRectangles)
                {
                    Rectangle nonwalkableRectangle = new Rectangle()
                    {
                        Width = characterRect.Width,
                        Height = characterRect.Height,
                        Fill = new SolidColorBrush(Colors.Blue)
                    };
                    _gridManager.GameCanvas.Children.Add(nonwalkableRectangle);
                    Canvas.SetLeft(nonwalkableRectangle, nonWalkableRect.Left);
                    Canvas.SetTop(nonwalkableRectangle, nonWalkableRect.Top);
                }
            }

            return collision;
        }

        /// <summary>
        /// Runs the Win Dialog if it is appropriate (charcater 1, not networked, 
        /// </summary>
        private static bool RunWinDialogIfNeeded(ref GridItem characterView)
        {
            if (!WinPage.open && !CommunicationManager.Instance.IsNetworked &&
                (characterView as CharacterItem)?.GetCharacter == GameGridManager.Instance.Characters[0])
            {
                TopFrameManager.Instance.OverlayFrame.Navigate(new WinPage());
                return false;
            }
            else if ((characterView as CharacterItem)?.GetCharacter == GameGridManager.Instance.Characters[1])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a queue of locations to check whether the items intersect
        /// </summary>
        /// <param name="xApprox">Reference the approximate x value the character is currently in</param>
        /// <param name="yApprox">Reference the approximate y value the character is currently in</param>
        /// <param name="xcheck">The x value to check either a fixed value (-1 or 1) or i from a for loop</param>
        /// <param name="ycheck">The y value to check either a fixed value (-1 or 1) or i from a for loop</param>
        /// <returns>A queue of items in that direction that need checking for intersection</returns>
        private static Queue<GridItem> QueuingLocationsToCheck(ref int xApprox, ref int yApprox, int xcheck, int ycheck)
        {
            Queue<GridItem> queue = new Queue<GridItem>(3);
            //If xCheck is 0 then it should be replaced by i, otherwise yCheck should shouldnt
            bool xCheckIsi = xcheck == 0;
            for (int i = -1; i <= 1; i++)
            {
                //If xcheck is the variable value (i.e. the direction you want to check for multiple items, e.g. if you're checking up you want to check for up,
                //up-left, up-right etc so xcheck changes, same with down). Otherwise ycheck is the variable (when moving left or right)
                switch (xCheckIsi)
                {
                       case true:
                           xcheck = i;
                           break;
                       case false:
                           ycheck = i;
                           break;
                }

                //If outside the _gridManager
                if (xApprox + xcheck < 0 || yApprox + ycheck < 0 || xApprox + xcheck > (Constants.GRID_TILES_XY - 1) ||
                    yApprox + ycheck > (Constants.GRID_TILES_XY - 1))
                {
                    break;
                }
                //Check for right-above, right and right-below, and if they are non-walkable or they are a closed exit
                GridItem item = _gridManager.GridItems[yApprox + ycheck, xApprox + xcheck];
                if (item.GetType() == typeof(NonWalkable) || (item.GetType() == typeof(Exitable) && !(item as Exitable).CanExit))
                {
                    //Then add to the queue
                    queue.Enqueue(_gridManager.GridItems[yApprox + ycheck, xApprox + xcheck]);
                }
            }

            return queue;
        }

        /// <summary>
        /// Detects whether the two characters intersect
        /// </summary>
        /// <returns>Whether the enemy has collided with Player 1</returns>
        public static bool EnemyCollisionDetection()
        {

            //Get the characters views
            GridItem characterOneView = GameGridManager.Instance.CharactersViews[0];
            GridItem characterTwoView = GameGridManager.Instance.CharactersViews[1];

            //Get the characters locations
            double char1Left = Canvas.GetLeft(characterOneView);
            double char2Left = Canvas.GetLeft(characterTwoView);
            double char1Top = Canvas.GetTop(characterOneView);
            double char2Top = Canvas.GetTop(characterTwoView);

            bool whetherCollided = Common.Algorithms.Collisions.EnemyCollisionDetectionCommon(char1Left, char1Top, char2Left, char2Top);

            //Visualise the intersection
            if (_gridManager.EnemyCollisionRectangles)
            {
                Canvas canvas = _gridManager.GameCanvas;
                Rectangle char1Rectangle = new Rectangle()
                {
                    Width = Common.Algorithms.Collisions.rectangle1.Width,
                    Height = Common.Algorithms.Collisions.rectangle1.Height,
                    Fill = new SolidColorBrush(Colors.DarkMagenta)
                };
                Rectangle char2Rectangle = new Rectangle()
                {
                    Width = Common.Algorithms.Collisions.rectangle2.Width,
                    Height = Common.Algorithms.Collisions.rectangle2.Height,
                    Fill = new SolidColorBrush(Colors.DarkMagenta)
                };

                canvas.Children.Add(char1Rectangle);
                canvas.Children.Add(char2Rectangle);

                Canvas.SetLeft(char1Rectangle, Common.Algorithms.Collisions.rectangle1.Left);
                Canvas.SetLeft(char2Rectangle, Common.Algorithms.Collisions.rectangle2.Left);

                Canvas.SetTop(char1Rectangle, Common.Algorithms.Collisions.rectangle1.Top);
                Canvas.SetTop(char2Rectangle, Common.Algorithms.Collisions.rectangle2.Top);
                _gridManager.DebuggingCanvasLeftovers += 2;
            }
            //Returns whether they intersect.
            return whetherCollided;
        }
    }
}