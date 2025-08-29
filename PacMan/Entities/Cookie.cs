using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PacMan.Entities
{
    internal class Cookie : Entity
    {
        public int Score { get; set; } = 100;
        private GameManager manager;

        public Cookie()
        {
            manager = GameManager.instance;
        }
        public override void CreateGameObject(ScreenElement screenElement)
        {
            gameObject = new GameObject(screenElement, this);
        }

        public override void Destroy()
        {
            isDestroyed = true;
            manager.objects[Convert.ToInt32(gameObject.Coordinates.Y), Convert.ToInt32(gameObject.Coordinates.X)] = null;
        }
    }
}
