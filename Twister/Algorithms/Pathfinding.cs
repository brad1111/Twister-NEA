﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Common;
using Common.Grid;
using Twister.Grid;

namespace Twister.Algorithms
{
    public static class Pathfinding
    {
        private static Stack<Rectangle> visualisations = new Stack<Rectangle>();

        private static Guid ThreadUsingVisualisations = Guid.Empty;

        public static void ClearPath()
        {
            //Prevent multiple clicks of the button from locking the program
            Guid thisThread = Guid.NewGuid();
            if (ThreadUsingVisualisations == Guid.Empty)
            {
                //Generate a GUID
                ThreadUsingVisualisations = thisThread;
            }
            else
            {
                return;
            }
            //Clear any paths already shown
            while (ThreadUsingVisualisations == thisThread && visualisations.Count > 0)
            {
                GameGridManager.Instance.GameCanvas.Children.Remove(visualisations.Pop());
            }

            ThreadUsingVisualisations = Guid.Empty; //No longer using stack
        }


        public static void ShowPath()
        {
            //Clear the path before using
            ClearPath();

            //Get enemy & protagonist
            CharacterItem protagonistItem = GameGridManager.Instance.CharactersViews[0];
            CharacterItem enemyItem = GameGridManager.Instance.CharactersViews[1];
            
            //Get their positions
            Position startPos = new Position(Canvas.GetLeft(enemyItem),
                                             Canvas.GetTop(enemyItem));
            Position endPos   = new Position(Canvas.GetLeft(protagonistItem),
                                             Canvas.GetTop(protagonistItem));
            //Get path stack
            Stack<GridItem> pathStack = FindPathInternal(startPos, endPos);
            //Create a rectangles on top of the path
            while (pathStack.Count > 0)
            {
                GridItem currentItem = pathStack.Pop();
                double gridX = Canvas.GetLeft(currentItem);
                double gridY = Canvas.GetTop(currentItem);
                Rectangle rect = new Rectangle()
                {
                    Height = Constants.GRID_ITEM_WIDTH,
                    Width = Constants.GRID_ITEM_WIDTH,
                    Fill = new SolidColorBrush(Colors.Chartreuse)
                };

                Canvas.SetLeft(rect, gridX);
                Canvas.SetTop(rect, gridY);
                GameGridManager.Instance.GameCanvas.Children.Add(rect);
                visualisations.Push(rect);
            }
            
            
        }

        /// <summary>
        /// Gets the shortest path from enemyCharacter to Protagonist
        /// </summary>
        /// <returns>The shortest path as a stack</returns>
        public static Stack<GridItem> FindPath()
        {
            //Gets the characters and their positions and then runs the algorithm internally
            CharacterItem protagonistItem = GameGridManager.Instance.CharactersViews[0];
            CharacterItem enemyItem = GameGridManager.Instance.CharactersViews[1];
            Position startPos = new Position(Canvas.GetLeft(enemyItem),
                Canvas.GetTop(enemyItem));
            Position endPos   = new Position(Canvas.GetLeft(protagonistItem),
                Canvas.GetTop(protagonistItem));
            return FindPathInternal(startPos, endPos);
        }

        /// <summary>
        /// Gets the path from startPos to endPos
        /// </summary>
        /// <param name="startPos">The start position (enemy)</param>
        /// <param name="endPos">The end position (protagonist)</param>
        /// <returns>The path</returns>
        private static Stack<GridItem> FindPathInternal(Position startPos, Position endPos)
        {
            //Clear previous chain

            //Calls A*, should find the last node
            GridItem endingItem = FindPathAStar(startPos, endPos);

            //Creates a path between start and end node by getting parent spaces
            Stack<GridItem> pathFromStartToEnd = new Stack<GridItem>();

            Position startPosGridLocation = new Position((int)startPos.x / 20, (int)startPos.y / 20);

            int count = 0;
            //continue until you get to the beginning
            while (endingItem != null && endingItem.Position != startPosGridLocation && count <= 10000)
            {
                Console.WriteLine("{0}, {1}", endingItem.Position.x, endingItem.Position.y);
                pathFromStartToEnd.Push(endingItem);
                endingItem = endingItem.ParentItem;
                count++;
                
            }
            return pathFromStartToEnd;
        }

        /// <summary>
        /// Gets the next position
        /// </summary>
        /// <param name="from">The position we're coming from</param>
        /// <param name="to">The postition we're going to</param>
        /// <returns>The next path item</returns>
        private static GridItem FindPathAStar(Position from, Position to)
        {
            List<GridItem> unvisitedItems = new List<GridItem>();
            List<GridItem> visitedItems = new List<GridItem>();

            //The position of the grid position to go to or from
            Position toGridSpaces = new Position((int) to.x / Constants.GRID_ITEM_WIDTH,(int) to.y / Constants.GRID_ITEM_WIDTH);
            Position fromGridSpaces = new Position((int) from.x / Constants.GRID_ITEM_WIDTH,(int) from.y / Constants.GRID_ITEM_WIDTH);

            //Get the location at the start (which will be the destiniation item)
            GridItem startItem = GetApproxGridItem(from);

            //Start at the starting item
            unvisitedItems.Add(startItem);

            //Add all of the neighbours that you want to check
            List<Position> neighbours = new List<Position>();
            neighbours.Add(new Position(-1,0));
            neighbours.Add(new Position(0,1));
            neighbours.Add(new Position(1,0));
            neighbours.Add(new Position(0,-1));

            int maxX = Constants.GRID_TILES_XY - 1;
            int maxY = Constants.GRID_TILES_XY - 1;

            //Continue until you have no unvisited items left
            while (unvisitedItems.Count > 0)
            {
                //Find the current lowest next item
                GridItem current = FindLowestWeight(unvisitedItems);
                if (current.Position == toGridSpaces)
                {
                    //End the loop with destination
                    return current;
                }

                //We've now visited the current item
                unvisitedItems.Remove(current);
                visitedItems.Add(current);

                //Check for the weighting of the neighbours to find next place to go
                foreach (Position neighbour in neighbours)
                {
                    Position nextPos = current.Position + neighbour;
                    //If the next move is invalid then skip
                    if (nextPos.x < 0 || nextPos.y < 0 || nextPos.x >= maxX || nextPos.y >= maxY ||
                        visitedItems.Where(x => x.Position == nextPos).Count() > 0 ||
                        !(GameGridManager.Instance.GridItems[(int) nextPos.y, (int) nextPos.x] is Walkable))
                    {
                        continue;
                    }

                    //Get the next neighbour item to perform calculations on
                    GridItem currentItem = null;
                    if (unvisitedItems.Count > 0)
                    {
                        IEnumerable<GridItem> unvisitedItemsWithPosition =
                            unvisitedItems.Where(x => x.Position == nextPos);
                        if (unvisitedItemsWithPosition.Count() > 0)
                        {
                            currentItem = unvisitedItems.First();
                        }
                    }
                    
                    if (currentItem == null)
                    {
                        //If the item exists then get it from the grid
                        currentItem = GameGridManager.Instance.GridItems[(int) nextPos.y, (int) nextPos.x];

                        //Setup the ParentItem for the pointer system, and the sum, previous and next weights to determine the best path.
                        //No negative weights allowed
                        currentItem.PreviousWeight = (int) (Math.Abs(nextPos.x - fromGridSpaces.x) + Math.Abs(nextPos.y - fromGridSpaces.y));
                        currentItem.NextWeight = (int) (Math.Abs(nextPos.x - toGridSpaces.x) + Math.Abs(nextPos.y - toGridSpaces.y));
                        currentItem.SumWeight = currentItem.PreviousWeight + currentItem.NextWeight;
                        currentItem.ParentItem = current;
                        unvisitedItems.Add(currentItem);
                    }
                    else
                    {
                        //Otherwise use the values you already have
                        int fromWeight = (int) (Math.Abs(nextPos.x - fromGridSpaces.x) + Math.Abs(nextPos.y - fromGridSpaces.y));
                        if (fromWeight < currentItem.PreviousWeight)
                        {
                            currentItem.PreviousWeight = fromWeight;
                            currentItem.SumWeight = currentItem.PreviousWeight + currentItem.NextWeight;
                            currentItem.ParentItem = current;
                        }
                        
                    }
                }
            }
            //Only occurs when a path isn't found
            return null;
        }

        /// <summary>
        /// Finds the lowest weighted items out of a given set of griditems
        /// </summary>
        /// <param name="gridItems">The griditems to check against</param>
        /// <returns>The lowest weighted griditem (aka the next path item to choose)</returns>
        private static GridItem FindLowestWeight(List<GridItem> gridItems)
        {
            GridItem smallestWeighted = gridItems[0];
            //Go through all of the items
            foreach (var item in gridItems)
            {
                //If the next item has a smaller total weight then it is the best choice,
                //also if it has the same total weight but the next item has a lower weight then it is also the best choice
                if (item.SumWeight < smallestWeighted.SumWeight || 
                    (item.SumWeight == smallestWeighted.SumWeight && item.NextWeight < smallestWeighted.NextWeight))
                {
                    smallestWeighted = item;
                }
            }
            return smallestWeighted;
        }

        /// <summary>
        /// Gets the griditem based on the position given
        /// </summary>
        /// <param name="exactPosition">The position the griditem is in</param>
        /// <returns>The griditem object</returns>
        private static GridItem GetApproxGridItem(Position exactPosition)
        {
            int x = (int) Math.Truncate(exactPosition.x / Constants.GRID_ITEM_WIDTH);
            int y = (int) Math.Truncate(exactPosition.y / Constants.GRID_ITEM_WIDTH);
            return GameGridManager.Instance.GridItems[y, x];
        } 
    }
}