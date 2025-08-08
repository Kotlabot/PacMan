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
        public int Score { get; set; }
        private GameManager manager;

        public Cookie()
        {
            manager = GameManager.gameManager;
        }

        public override void Destroy()
        {
            isDestroyed = true;
            manager.objects[Convert.ToInt32(gameObject.Coordinates.X), Convert.ToInt32(gameObject.Coordinates.Y)] = null;
        }
    }
}
