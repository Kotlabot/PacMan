using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PacMan.Entities
{
    /// <summary>
    /// Represents a basic cookie. Collecting it increases the Pacman score.
    /// </summary>
    internal class Cookie : Entity
    {
        public int Score { get; set; } = 100;
        private GameManager manager;

        public Cookie()
        {
            manager = GameManager.instance;
        }

        /// <summary>
        /// Creates a visual GameObject for a cookie.
        /// </summary>
        public override void CreateGameObject(ScreenElement screenElement)
        {
            gameObject = new GameObject(screenElement, this);
        }

        /// <summary>
        /// Destroys the cookie and removes it from the game grid.
        /// </summary>
        public override void Destroy()
        {
            gameObject.isDestroyed = true;
            manager.objects[Convert.ToInt32(gameObject.Coordinates.Y), Convert.ToInt32(gameObject.Coordinates.X)] = null;
        }
    }
}
