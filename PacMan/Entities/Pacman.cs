using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PacMan.Entities
{
    internal class Pacman : Entity
    {
        public int Score { get; set; }
        public bool SuperMode { get; set; }

    }
}
