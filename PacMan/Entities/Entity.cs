using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan.Entities
{
    /// <summary>
    /// Abstract base class for all game entities (pacman, cookies...).
    /// Contains game logic and cross-reference to GameObject for rendering.
    /// </summary>
    public abstract class Entity
    {
        public Coordinates Coordinates { get; set; }

        // Path to the image or animation directory for an entity.
        public string ImagePath { get; set; }

        [JsonIgnore]
        public GameObject gameObject;

        /// <summary>
        /// Abstract method to destroy an entity. Implementation is specific to the entity type.
        /// </summary>
        public abstract void Destroy();

        /// <summary>
        /// Abstract method to create a GameObject (visual) from this entity.
        /// </summary>
        public abstract void CreateGameObject(ScreenElement screenElement);
    }
}
