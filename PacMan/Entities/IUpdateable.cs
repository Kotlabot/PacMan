using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan.Entities
{
    /// <summary>
    /// Interface for all entities that need to update their state regularly - Pacman, Monster, SuperMonster.
    /// </summary>
    internal interface IUpdateable
    {
        void Update();
    }
}
