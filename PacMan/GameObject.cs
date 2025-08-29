using PacMan.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PacMan.GameManager;

namespace PacMan
{
    public class GameObject
    {
        public ScreenElement Sprite { get; private set; }
        public Coordinates Coordinates {  get; private set; }
        public Entity Entity { get; private set; }

        public UpdateDelegate UpdateDelegate { get; private set; }

        public GameObject(ScreenElement sprite,  Entity entity) 
        {
            Sprite = sprite;
            Coordinates newCoordinates = new Coordinates();
            newCoordinates.X = entity.Coordinates.X;
            newCoordinates.Y = entity.Coordinates.Y;
            Coordinates = newCoordinates;
            Entity = entity;
        }

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
