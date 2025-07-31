using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan.Entities
{
    public abstract class Entity
    {
        public Coordinates Coordinates { get; set; }
        public bool HandlesCollisions {  get; set; }
        public bool IsDestroyed { get; set; }
        public string ImagePath { get; set; }

        public GameObject GameObject;
    }
}
