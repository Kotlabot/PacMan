using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            if (manager != null)
            {
                manager.onUpdateListeners.Add(Update);
            }
        }

        public override void Destroy()
        {
            isDestroyed = true;
            manager.onUpdateListeners.Remove(Update);
            manager.isGameOff = true;
            MessageBox.Show("Game Over! Your score is {0}", Score.ToString());
        }
        public void Update()
        {
            if (manager == null && gameObject == null)
                return;

            if (manager.actionKeys.Count > 0)
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


        }

        public bool HandleCollision(Entity entity)
        {
            switch (entity)
            {
                case Monster monster:
                    if (SuperMode)
                    {
                        monster.Destroy();
                        return true;
                    }
                    else
                        Destroy();
                    break;

                case SuperMonster superMonster:
                    Destroy();
                    break;

                case Cookie cookie:
                    Score += cookie.Score;
                    cookie.Destroy();
                    return true;

                case SuperCookie superCookie:
                    Score += superCookie.Score;
                    superCookie.Destroy();
                    return true;

                case Wall wall:
                    break;
            }
            return false;
        }
    }
}
