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
        private int HunterModeDuration { get; set; } = 5000;
        private int hunterModeTimeLeft = 0;
        private GameManager manager;

        public Pacman() 
        {
            if(GameManager.instance != null)
                manager = GameManager.instance;
        }

        public override void CreateGameObject(ScreenElement screenElement)
        {
            gameObject = new GameObject(screenElement, this);
            gameObject.SetUpdateDelegate(Update);
        }

        public override void Destroy()
        {
            isDestroyed = true;
            manager.toBeRemovedListeners.Remove(Update);
            manager.isGameOff = true;
            MessageBox.Show($"Game Over! Your score is {Score}.", "Score");
        }

        
        public void Update()
        {
            if(hunterModeTimeLeft > HunterModeDuration)
            {
                hunterModeTimeLeft = 0;
                manager.isSuperMode = false;
            }
            else if(manager.isSuperMode)
            {
                hunterModeTimeLeft += manager.UpdateRate;
            }

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

                if (manager.CookiesCount == manager.EatenCookies)
                    manager.SuccesfullyEndGame(Score);
            }


        }

        // Genericka metoda nebyla treba implementovat protoze problem s ruznymi druhy entit (pacman, susenka, prisera...)
        // je vyresen abstraktni tridou Entity, kterou vsechny ostatni dedi.
        public bool HandleCollision(Entity entity)
        {
            switch (entity)
            {
                case Monster monster:
                    if (manager.isSuperMode)
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
                    manager.EatenCookies++;
                    cookie.Destroy();
                    return true;

                case SuperCookie superCookie:
                    manager.isSuperMode = true;
                    Score += superCookie.Score;
                    manager.EatenCookies++;
                    superCookie.Destroy();
                    hunterModeTimeLeft = 0;
                    return true;

                case Wall wall:
                    break;
            }
            return false;
        }
    }
}
