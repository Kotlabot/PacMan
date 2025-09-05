using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PacMan.Entities
{
    /// <summary>
    /// Represents the player character (Pacman). Handles input, scoring, collisions, and game-over logic.
    /// </summary>
    internal class Pacman : Entity, IUpdateable
    {
        // Total score accumulated by Pacman.
        public int Score { get; set; }

        // Duration of the hunter (super) mode in milliseconds.
        private int HunterModeDuration { get; set; } = 5000;

        // How long Pacman has already been in hunter mode.
        private int hunterModeTimeLeft = 0;

        // Reference to the global game manager to access game state, objects, and settings.
        private GameManager manager;

        public Pacman() 
        {
            if(GameManager.instance != null)
                manager = GameManager.instance;
        }

        /// <summary>
        /// Creates a visual game object from an entity and registers its update method.
        /// </summary>
        public override void CreateGameObject(ScreenElement screenElement)
        {
            gameObject = new GameObject(screenElement, this);
            gameObject.SetUpdateDelegate(Update);
        }

        /// <summary>
        /// Destroys Pacman (when eaten by a monster or super monster) and ends the game and shows the final score.
        /// </summary>
        public override void Destroy()
        {
            gameObject.isDestroyed = true;
            manager.toBeRemovedListeners.Remove(Update);
            manager.isGameOff = true;
            MessageBox.Show($"Game Over! Your score is {Score}.", "Score");
        }

        /// <summary>
        /// Updates Pacman's state - manages super mode timer, processes input,
        /// handles collisions, moves Pacman, and checks for game win.
        /// </summary>
        public void Update()
        {
            // Check hunter mode duration and reset when expired.
            if (hunterModeTimeLeft > HunterModeDuration)
            {
                hunterModeTimeLeft = 0;
                manager.isSuperMode = false;
            }
            else if(manager.isSuperMode)
            {
                hunterModeTimeLeft += manager.UpdateRate;
            }

            if (manager == null && gameObject == null)
                return;

            // Process queued input actions.
            if (manager.actionKeys.Count > 0)
            {
                Key actualKey = manager.actionKeys.Dequeue();
                var actualCoordinateX = Convert.ToInt32(gameObject.Coordinates.X);
                var actualCoordinateY = Convert.ToInt32(gameObject.Coordinates.Y);
                int deltaX = 0;
                int deltaY = 0;
                int sizeRow = manager.objects.GetLength(0);
                int sizeColumn = manager.objects.GetLength(1);

                switch (actualKey.ToString())
                {
                    case "A":
                        deltaX--;
                        break;
                    case "S":
                        deltaY++;
                        break;
                    case "D":
                        deltaX++;
                        break;
                    case "W":
                        deltaY--;
                        break;
                }

                // Prevent movement outside map boundaries.
                if (actualCoordinateX + deltaX < 0 || actualCoordinateX + deltaX >= sizeColumn || actualCoordinateY + deltaY < 0 || actualCoordinateY + deltaY >= sizeRow)
                {
                    return;
                }

                bool shouldMove = true;

                // If there is an object in the target cell, handle collision.
                if (manager.objects[actualCoordinateY + deltaY, actualCoordinateX + deltaX] != null)
                    shouldMove = HandleCollision(manager.objects[actualCoordinateY + deltaY, actualCoordinateX + deltaX].Entity);

                // Move Pacman to the new position if movement is allowed.
                if (shouldMove)
                {
                    manager.objects[actualCoordinateY + deltaY, actualCoordinateX + deltaX] = manager.objects[actualCoordinateY, actualCoordinateX];
                    manager.objects[actualCoordinateY, actualCoordinateX] = null;
                    gameObject.Coordinates.Y += deltaY;
                    gameObject.Coordinates.X += deltaX;
                }

                // Check if Pacman ate all cookies (completed the level).
                if (manager.CookiesCount == manager.EatenCookies)
                    manager.SuccesfullyEndGame(Score);
            }


        }

        /// <summary>
        /// Handles collision between Pacman and another entity.
        /// Returns true if Pacman can move into the entity's cell, false otherwise.
        /// It was not necessary to implement a generic method because the problem with 
        /// different types of entities (Pacman, cookie, monster...) is solved by the 
        /// abstract class Entity, which all the others inherit from.
        /// </summary>
        public bool HandleCollision(Entity entity)
        {
            switch (entity)
            {
                case Monster monster:
                    if (manager.isSuperMode)
                    {
                        monster.Destroy();
                        return true;
                    }
                    else
                        Destroy();
                    break;

                case SuperMonster superMonster:
                    Destroy();
                    break;

                case Cookie cookie:                    
                    Score += cookie.Score;
                    manager.EatenCookies++;
                    cookie.Destroy();
                    return true;

                case SuperCookie superCookie:
                    manager.isSuperMode = true;
                    Score += superCookie.Score;
                    manager.EatenCookies++;
                    superCookie.Destroy();
                    hunterModeTimeLeft = 0;
                    return true;

                case Wall wall:
                    break;
            }
            return false;
        }
    }
}
