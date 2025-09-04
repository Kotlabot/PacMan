using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PacMan.Entities
{
    /// <summary>
    /// Represents a special cookie. Collecting it enables Pacman's super mode.
    /// </summary>
    internal class SuperCookie : Entity
    {
        public int Score { get; set; } = 500;
        private GameManager manager;

        public SuperCookie()
        {
            if (GameManager.instance != null)
            {
                manager = GameManager.instance;
            }
        }

        /// <summary>
        /// Creates a visual GameObject for a super cookie.
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
