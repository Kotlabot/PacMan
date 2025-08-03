using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PacMan
{
    public class GameManager
    {
        public delegate void UpdateDelegate();
        public List<UpdateDelegate> onUpdateListeners = new List<UpdateDelegate>();

        public GameObject[,] objects = new GameObject[25, 41];
        public Queue<Key> actionKeys = new Queue<Key>();
        public bool isGameOff = true;
        public static GameManager gameManager;

        public GameManager() 
        {
            gameManager = this;
        }
        public void RegisterEvent(Key key)
        {
            actionKeys.Enqueue(key);
        }

        public void Update(object sender, EventArgs e)
        {
            if(!isGameOff)
            {
                foreach (var listeners in onUpdateListeners)
                    listeners.Invoke();
            }
            
        }
    }
}
