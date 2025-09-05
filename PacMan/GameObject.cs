using PacMan.Entities;
using static PacMan.GameManager;

namespace PacMan
{
    /// <summary>
    /// Represents a visual (ScreenElement) and logical (Entity) game object on the map.
    /// </summary>
    public class GameObject
    {
        // The visual representation of an object (image or animation).
        public ScreenElement Sprite { get; private set; }

        // Coordinates of the object in the game grid.
        // Separate from the entity to allow movement independent of the entity object (static map).
        public Coordinates Coordinates {  get; private set; }

        // Reference to the Entity that holds the game logic for this object (collisions).
        public Entity Entity { get; private set; }

        // Delegate method to be executed on each game update (movement or animation).
        public UpdateDelegate UpdateDelegate { get; private set; }

        public bool isDestroyed { get; set; }

        public GameObject(ScreenElement sprite,  Entity entity) 
        {
            Sprite = sprite;
            Coordinates newCoordinates = new Coordinates();

            // Copy coordinates from entity to create independent position tracking.
            newCoordinates.X = entity.Coordinates.X;
            newCoordinates.Y = entity.Coordinates.Y;
            Coordinates = newCoordinates;
            Entity = entity;
        }

        /// <summary>
        /// Sets the update delegate and registers it with the GameManager.
        /// This allows the object's logic to be called automatically on each game update.
        /// </summary>
        public void SetUpdateDelegate(UpdateDelegate updateDelegate)
        {
            UpdateDelegate = updateDelegate;
            if(GameManager.instance != null) 
            {
                GameManager.instance.onUpdateListeners.Add(updateDelegate);
            }
        }
    }
}
