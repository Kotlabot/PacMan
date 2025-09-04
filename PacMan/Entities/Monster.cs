using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Formats.Asn1.AsnWriter;

namespace PacMan.Entities
{
    /// <summary>
    /// Represents a regular monster that moves randomly and can kill Pacman if he's not in super mode.
    /// </summary>
    internal class Monster : Entity, IUpdateable
    {
        Random Random = new Random();
        private GameManager manager;

        // Minimum time between movement updates, best possible value was found when debugging.
        private int UpdateFreq = 120;

        // Accumulated time since the last movement update.
        private int LastUpdate;

        public Monster()
        {
            if (GameManager.instance != null)
            {
                manager = GameManager.instance;
            }

            // Ensure monster moves immediately on first update.
            LastUpdate = 600;
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
        /// Destroys the monster and removes it from the game grid.
        /// </summary>
        public override void Destroy()
        {
            gameObject.isDestroyed = true;
            manager.toBeRemovedListeners.Remove(Update);

            if (manager.objects[Convert.ToInt32(gameObject.Coordinates.Y), Convert.ToInt32(gameObject.Coordinates.X)].Entity is Pacman)
                return;
            manager.objects[Convert.ToInt32(gameObject.Coordinates.Y), Convert.ToInt32(gameObject.Coordinates.X)] = null;
        }

        /// <summary>
        /// Updates the monster's state - moves randomly at intervals, handles collisions.
        /// </summary>
        public void Update()
        {
            // Do not update when monster is destroyed (in case listener was not yet removed).
            if (gameObject.isDestroyed)
                return;

            // Skip updates until enough time has passed - regulation of monsters movement.
            if (LastUpdate < UpdateFreq)
            {
                LastUpdate += manager.UpdateRate;
                return;
            }
            LastUpdate = 0;

            if (manager == null && gameObject == null)
                return;

            var actualCoordinateX = Convert.ToInt32(gameObject.Coordinates.X);
            var actualCoordinateY = Convert.ToInt32(gameObject.Coordinates.Y);
            int deltaX = Random.Next(-1, 2); // Move random to one of the 9 directions.
            int deltaY = Random.Next(-1, 2); // Move random to one of the 9 directions.
            int sizeRow = manager.objects.GetLength(0);
            int sizeColumn = manager.objects.GetLength(1);

            // Prevent movement outside map boundaries.
            if (actualCoordinateX + deltaX < 0 || actualCoordinateX + deltaX >= sizeColumn || actualCoordinateY + deltaY < 0 || actualCoordinateY + deltaY >= sizeRow)
            {
                return;
            }

            bool shouldMove = true;

            // If there is an object in the target cell, handle collision.
            if (manager.objects[actualCoordinateY + deltaY, actualCoordinateX + deltaX] != null)
                shouldMove = HandleCollision(manager.objects[actualCoordinateY + deltaY, actualCoordinateX + deltaX].Entity);

            // Move monster to the new position if movement is allowed.
            if (shouldMove && !gameObject.isDestroyed)
            {
                manager.objects[actualCoordinateY + deltaY, actualCoordinateX + deltaX] = manager.objects[actualCoordinateY, actualCoordinateX];
                manager.objects[actualCoordinateY, actualCoordinateX] = null;
                gameObject.Coordinates.Y += deltaY;
                gameObject.Coordinates.X += deltaX;
            }

        }

        /// <summary>
        /// Handles collisions between the monster and another entity.
        /// </summary>
        public bool HandleCollision(Entity entity)
        {
            switch (entity)
            {
                case Pacman pacman:
                    // If Pacman is in super mode, destroy monster, otherwise destroy Pacman.
                    if (manager.isSuperMode)
                    {
                        Destroy();
                        return false;
                    }
                    else
                        pacman.Destroy();
                    return false;

                case Monster monster:
                    // Do not move when colliding with another monster.
                    return false;

                case SuperMonster superMonster:
                    // Do not move when colliding with super monster.
                    return false;

                case Cookie cookie:
                    // When colliding with cookie, add cookie to the masked objects and overlay it.
                    manager.maskedObjects.Add(cookie.gameObject);
                    break;

                case SuperCookie superCookie:
                    // When colliding with super cookie, add cookie to the masked objects and overlay it.
                    manager.maskedObjects.Add(superCookie.gameObject);
                    break;

                case Wall wall:
                    return false;
            }
            return true;
        }


    }
}


