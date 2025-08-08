using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PacMan.Entities
{
    internal class SuperCookie : Entity
    {
        public int Score { get; set; }
        private GameManager manager;

        public SuperCookie()
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
