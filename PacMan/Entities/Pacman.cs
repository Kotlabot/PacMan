using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PacMan.Entities
{
    internal class Pacman : Entity, IUpdateable
    {
        public int Score { get; set; }
        public bool SuperMode { get; set; }
        private GameManager manager;

        public Pacman() 
        {
            manager = GameManager.gameManager;
            if(manager != null)
            {
                manager.onUpdateListeners.Add(Update);
            }
        }
        public void Update()
        {
            if (manager == null && gameObject == null)
                return;

            if(manager.actionKeys.Count > 0)
            {
                Key actualKey = manager.actionKeys.Dequeue();
                var actualCoordinateX = Convert.ToInt32(gameObject.Coordinates.X);
                var actualCoordinateY = Convert.ToInt32(gameObject.Coordinates.Y);
                int deltaX = 0;
                int deltaY = 0;
                int sizeRow = manager.objects.GetLength(0);
                int sizeColumn = manager.objects.GetLength(1);

                switch (actualKey.ToString())
                {
                    case "A":
                        deltaX--;
                        break;
                    case "S":
                        deltaY++;
                        break;
                    case "D":
                        deltaX++;
                        break;
                    case "W":
                        deltaY--;
                        break;
                }
                if(actualCoordinateX+deltaX < 0 || actualCoordinateX+deltaX >= sizeRow || actualCoordinateY+deltaY < 0 || actualCoordinateY+deltaY >= sizeColumn)
                {
                    return;
                }

                manager.objects[actualCoordinateX+deltaX, actualCoordinateY+deltaY] = manager.objects[actualCoordinateX, actualCoordinateY];
                manager.objects[actualCoordinateX, actualCoordinateY] = null;
                gameObject.Coordinates.Y += deltaY;
                gameObject.Coordinates.X += deltaX;
            }


        }
    }
}
