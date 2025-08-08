using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Formats.Asn1.AsnWriter;

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

        public override void Destroy()
        {
            isDestroyed = true;
            manager.onUpdateListeners.Remove(Update);
            manager.objects[Convert.ToInt32(gameObject.Coordinates.X), Convert.ToInt32(gameObject.Coordinates.Y)] = null;
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

            if (actualCoordinateX + deltaX < 0 || actualCoordinateX + deltaX >= sizeColumn || actualCoordinateY + deltaY < 0 || actualCoordinateY + deltaY >= sizeRow)
            {
                return;
            }

            bool shouldMove = true;

            if (manager.objects[actualCoordinateY + deltaY, actualCoordinateX + deltaX] != null)
                shouldMove = HandleCollision(manager.objects[actualCoordinateY + deltaY, actualCoordinateX + deltaX].Entity);

            if (shouldMove)
            {
                manager.objects[actualCoordinateY + deltaY, actualCoordinateX + deltaX] = manager.objects[actualCoordinateY, actualCoordinateX];
                manager.objects[actualCoordinateY, actualCoordinateX] = null;
                gameObject.Coordinates.Y += deltaY;
                gameObject.Coordinates.X += deltaX;
            }

        }

        public bool HandleCollision(Entity entity)
        {
            switch (entity)
            {
                case Pacman pacman:
                    if (pacman.SuperMode)
                    {
                        Destroy();
                        return true;
                    }
                    else
                        pacman.Destroy();
                    break;

                case SuperMonster superMonster:
                    break;

                case Cookie cookie:
                    break;

                case SuperCookie superCookie:
                    break;

                case Wall wall:
                    break;
            }
            return false;
        }


    }
}


