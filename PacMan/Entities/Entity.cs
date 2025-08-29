using Newtonsoft.Json;
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
        public string ImagePath { get; set; }

        [JsonIgnore]
        public bool handlesCollisions;
        [JsonIgnore]
        public bool isDestroyed;
        [JsonIgnore]
        public GameObject gameObject;

        public abstract void Destroy();
        public abstract void CreateGameObject(ScreenElement screenElement);
    }
}
