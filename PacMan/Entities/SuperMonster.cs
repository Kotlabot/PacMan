using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PacMan.Entities
{
    internal class SuperMonster : Entity, IUpdateable
    {
        Random Random = new Random();
        private GameManager manager;
        private int UpdateFreq = 300;
        private int LastUpdate;

        public SuperMonster()
        {
            if (GameManager.instance != null)
            {
                manager = GameManager.instance;
            }

            LastUpdate = 600;
        }

        public override void CreateGameObject(ScreenElement screenElement)
        {
            gameObject = new GameObject(screenElement, this);
            gameObject.SetUpdateDelegate(Update);
        }

        public override void Destroy() { }

        public void Update()
        {
            if (LastUpdate < UpdateFreq)
            {
                LastUpdate += manager.UpdateRate;
                return;
            }
            LastUpdate = 0;

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
                    pacman.Destroy();
                    return false;

                case Monster monster:
                    //manager.maskedObjects.Add(monster.gameObject);
                    //break;
                    return false;

                case SuperMonster superMonster:
                    //manager.maskedObjects.Add(superMonster.gameObject);
                    //break;
                    return false;

                case Cookie cookie:
                    manager.maskedObjects.Add(cookie.gameObject);
                    break;

                case SuperCookie superCookie:
                    manager.maskedObjects.Add(superCookie.gameObject);
                    break;

                case Wall wall:
                    return false;
            }
            return true;
        }

    }
}
