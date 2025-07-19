using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class GameObject
    {
        public ScreenElement Sprite { get; private set; }
        public Coordinates Coordinates {  get; private set; }

        public GameObject(ScreenElement sprite,  Coordinates coordinates) 
        {
            Sprite = sprite;
            Coordinates = coordinates;
        }
    }
}
