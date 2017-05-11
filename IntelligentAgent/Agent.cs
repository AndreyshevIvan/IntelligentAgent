using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAgent
{
    enum PassiveAct
    {
        NONE,
        ON_LEFT,
        ON_RIGHT,
        ROLL,
    }

    enum ActiveAct
    {
        NONE,
        GO,
        SHOOT,
        TAKE,
    }

    abstract class Agent
    {
        protected Agent(IMapPhysics mapPhysics)
        {
            m_mapPhysics = mapPhysics;
        }

        public virtual void DoMove()
        {
            HandleNewCave(m_mapPhysics.cave);
            HandleWorld(m_mapPhysics.world);
            Move move = CalculateMove();
            m_mapPhysics.SetMove(move);
        }

        protected abstract void HandleNewCave(Cave newCave);
        protected abstract void HandleWorld(World world);
        protected abstract Move CalculateMove();

        protected IMapPhysics m_mapPhysics;
    }
}
