using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PacMan.Entities
{
    internal class Monster : Entity, IUpdateable
    {
        Random Random = new Random();
        private GameManager manager;

        public Monster()
        {
            manager = GameManager.gameManager;
            if (manager != null)
            {
                manager.onUpdateListeners.Add(Update);
            }
        }
        public void Update()
        {
            if (manager == null && gameObject == null)
                return;

            var actualCoordinateX = Convert.ToInt32(gameObject.Coordinates.X);
            var actualCoordinateY = Convert.ToInt32(gameObject.Coordinates.Y);
            int deltaX = Random.Next(-1, 2);
            int deltaY = Random.Next(-1, 2);
            int sizeRow = manager.objects.GetLength(0);
            int sizeColumn = manager.objects.GetLength(1);

            if (actualCoordinateX + deltaX < 0 || actualCoordinateX + deltaX >= sizeRow || actualCoordinateY + deltaY < 0 || actualCoordinateY + deltaY >= sizeColumn)
            {
                return;
            }

            manager.objects[actualCoordinateX + deltaX, actualCoordinateY + deltaY] = manager.objects[actualCoordinateX, actualCoordinateY];
            manager.objects[actualCoordinateX, actualCoordinateY] = null;
            gameObject.Coordinates.Y += deltaY;
            gameObject.Coordinates.X += deltaX;
        }


    }
}


