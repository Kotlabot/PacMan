using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PacMan
{
    public class GameManager
    {
        public delegate void UpdateDelegate();
        public List<UpdateDelegate> onUpdateListeners = new List<UpdateDelegate>();
        public List<UpdateDelegate> toBeRemovedListeners = new List<UpdateDelegate>();
        public int UpdateRate = 20;
        public bool isSuperMode { get; set; }
        public int CookiesCount { get; set; }
        public int EatenCookies { get; set; }

        public GameObject[,] objects = new GameObject[25, 41];
        public Queue<Key> actionKeys = new Queue<Key>();
        public bool isGameOff = true;
        public bool isGamePaused = false;
        public bool isGameWon = false;
        public static GameManager instance;
        public List<GameObject> maskedObjects = new List<GameObject>();

        public GameManager(int updateRate)
        {
            UpdateRate = updateRate;
            instance = this;
        }
        public void RegisterEvent(Key key)
        {
            actionKeys.Enqueue(key);
        }

        public void Update(object sender, EventArgs e)
        {
            if (!isGameOff && !isGamePaused)
            {
                GetPressedKeys();
                foreach (var listeners in onUpdateListeners)
                    listeners.Invoke();

                // Pri odstranovani v kolizi pridat delegaty listener do toBeRemovedListeners a zde pak odstranit (nelze odstranit primo z cyklu)
                foreach (var listener in toBeRemovedListeners)
                    onUpdateListeners.Remove(listener);

                List<GameObject> objectsToBeRemoved = new List<GameObject>();
                foreach (var obj in maskedObjects)
                {
                    if (objects[Convert.ToInt32(obj.Coordinates.Y), Convert.ToInt32(obj.Coordinates.X)] == null)
                    {
                        objects[Convert.ToInt32(obj.Coordinates.Y), Convert.ToInt32(obj.Coordinates.X)] = obj;
                        objectsToBeRemoved.Add(obj);
                    }
                }

                foreach (var obj in objectsToBeRemoved)
                {
                    maskedObjects.Remove(obj);
                }
            }

        }

        public void UpdateAnimations()
        {
            foreach (var obj in objects)
            {
                if(obj != null)
                    obj.Sprite.UpdateAnimation();
            }
        }

        // Vyreseni prerusovani, kontrola zde a pridavani do actionKeys
        private void GetPressedKeys()
        {
            if (Keyboard.IsKeyDown(Key.A))
                actionKeys.Enqueue(Key.A);
            else if (Keyboard.IsKeyDown(Key.S))
                actionKeys.Enqueue(Key.S);
            else if (Keyboard.IsKeyDown(Key.D))
                actionKeys.Enqueue(Key.D);
            else if (Keyboard.IsKeyDown(Key.W))
                actionKeys.Enqueue(Key.W);
        }

        public void Reset()
        {
            onUpdateListeners.Clear();
            toBeRemovedListeners.Clear();
            actionKeys.Clear();
            maskedObjects.Clear();
            isGameWon = false;
            isSuperMode = false;
            objects = new GameObject[25, 41];
        }

        public void SuccesfullyEndGame(int score)
        {
            isGameOff = true;
            isGameWon = true;
            MessageBox.Show($"You win! Your score is {score}.", "Score");
        }
    }
}
