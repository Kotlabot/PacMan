using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan.Entities
{
    /// <summary>
    /// Represents a wall that blocks movement.
    /// </summary>
    internal class Wall : Entity
    {
        public override void Destroy() { }

        /// <summary>
        /// Creates a visual GameObject for a wall.
        /// </summary>
        public override void CreateGameObject(ScreenElement screenElement)
        {
            gameObject = new GameObject(screenElement, this);
        }

    }
}
