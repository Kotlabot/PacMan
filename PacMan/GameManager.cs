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
    /// <summary>
    /// GameManager is the core of the Pacman game.
    /// It handles update cycles, input events, game state, and animation updates.
    /// </summary>
    public class GameManager
    {
        // Delegate for update listeners (methods that should be executed every update cycle).
        public delegate void UpdateDelegate();

        // List of registered update listeners that are invoked every game update.
        public List<UpdateDelegate> onUpdateListeners = new List<UpdateDelegate>();

        // Temporary storage for listeners that should be removed after update (destroyed objects).
        public List<UpdateDelegate> toBeRemovedListeners = new List<UpdateDelegate>();

        // Number of updates per second (game loop frequency), the best possible value was discovered when debugging.
        public int UpdateRate = 50;

        // Whether Pacman is currently in super mode (after eating a super cookie).
        public bool isSuperMode { get; set; }

        // Total number of cookies present on the map (recounted at every start or restart).
        public int CookiesCount { get; set; }

        // Number of cookies already eaten by Pacman (increases when handling collisions).
        public int EatenCookies { get; set; }

        // Dimension of dynamic map (objects), default values are calculated.
        public int MapHeight { get; set; } = 25;
        public int MapWidth { get; set; } = 41;

        // Two-dimensional grid holding all active game objects (dynamic map).
        public GameObject[,] objects; 

        // Queue of keyboard input actions to be processed in the next update cycle.
        public Queue<Key> actionKeys = new Queue<Key>();
        public bool isGameOff = true;
        public bool isGamePaused = false;
        public bool isGameWon = false;

        // Static instance of the GameManager (allows global access).
        public static GameManager instance;

        // Temporary list of objects that are "masked" (cookies or super cookies colliding with monsters).
        public List<GameObject> maskedObjects = new List<GameObject>();

        public GameManager(int updateRate)
        {
            UpdateRate = updateRate; // Initializes the update rate (how many updates per second should be processed).
            instance = this; // Sets the static instance.
            objects = new GameObject[MapHeight, MapWidth]; // Set dimension of dynamic array.
        }

        /// <summary>
        /// Main update loop of the game.
        /// Handles input, invokes update listeners, processes collisions, and updates object states.
        /// </summary>
        public void Update(object sender, EventArgs e)
        {
            if (!isGameOff && !isGamePaused)
            {
                // Process queued keyboard input.
                GetPressedKeys();

                // Execute all registered update listeners.
                foreach (var listeners in onUpdateListeners)
                    listeners.Invoke();

                // Remove listeners marked for removal during update cycle (when handling collisions).
                foreach (var listener in toBeRemovedListeners)
                    onUpdateListeners.Remove(listener);

                // Check all masked objects to determine whether they can be restored
                // (if the object that was masking them is gone - tile is empty) or should remain masked.
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

        /// <summary>
        /// Updates animations for all game objects.
        /// </summary>
        public void UpdateAnimations()
        {
            foreach (var obj in objects)
            {
                if(obj != null)
                    obj.Sprite.UpdateAnimation(); // For animated objects get next frame.
            }
        }

        /// <summary>
        /// Checks for pressed keys and queues them as actions.
        /// This ensures only one key press is registered per update cycle.
        /// </summary>
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

        /// <summary>
        /// Resets the game state back to its initial condition.
        /// Clears listeners, input queues, and object references.
        /// </summary>
        public void Reset()
        {
            onUpdateListeners.Clear();
            toBeRemovedListeners.Clear();
            actionKeys.Clear();
            maskedObjects.Clear();
            isGameWon = false;
            isSuperMode = false;
            objects = new GameObject[MapHeight, MapWidth];
        }

        /// <summary>
        /// Ends the game successfully (when Pacman wins - eats all cookies).
        /// </summary>
        public void SuccesfullyEndGame(int score)
        {
            isGameOff = true;
            isGameWon = true;
            MessageBox.Show($"You win! Your score is {score}.", "Score");
        }

        /// <summary>
        /// Clears all game objects from the dynamic map.
        /// </summary>
        public void ClearMap()
        {
            int rows = objects.GetLength(0);
            int columns = objects.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    objects[i, j] = null;
                }
            }
        }
    }
}
