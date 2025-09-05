using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PacMan.Entities
{
    /// <summary>
    /// Represents a stronger monster that moves randomly and cannot be killed by Pacman.
    /// </summary>
    internal class SuperMonster : Entity, IUpdateable
    {
        Random Random = new Random();
        private GameManager manager;

        // Minimum time between movement updates, best possible value was found when debugging.
        private int UpdateFreq = 120;

        // Accumulated time since the last movement update.
        private int LastUpdate;

        public SuperMonster()
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

        public override void Destroy() { }

        /// <summary>
        /// Updates the super monster's state - moves randomly at intervals, handles collisions.
        /// </summary>
        public void Update()
        {
            // Skip updates until enough time has passed - regulation of super monsters movement.
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
            int deltaX = Random.Next(-1, 2); // Move random to one of the 4 directions.
            int deltaY = Random.Next(-1, 2); // Move random to one of the 4 directions.
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

            // Move super monster to the new position if movement is allowed.
            if (shouldMove && !gameObject.isDestroyed)
            {
                manager.objects[actualCoordinateY + deltaY, actualCoordinateX + deltaX] = manager.objects[actualCoordinateY, actualCoordinateX];
                manager.objects[actualCoordinateY, actualCoordinateX] = null;
                gameObject.Coordinates.Y += deltaY;
                gameObject.Coordinates.X += deltaX;
            }

        }

        /// <summary>
        /// Handles collisions between the super monster and another entity.
        /// </summary>
        public bool HandleCollision(Entity entity)
        {
            switch (entity)
            {
                case Pacman pacman:
                    // Destroy pacman even if he's in super mode - super monster is indestructible.
                    pacman.Destroy();
                    return false;

                case Monster monster:
                    // Do not move when colliding with monster.
                    return false;

                case SuperMonster superMonster:
                    // Do not move when colliding with another super monster.
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
