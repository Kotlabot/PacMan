using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan.Entities
{
    internal class Wall : Entity
    {
        public override void Destroy() { }
        public override void CreateGameObject(ScreenElement screenElement)
        {
            gameObject = new GameObject(screenElement, this);
        }

    }
}
