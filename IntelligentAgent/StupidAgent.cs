using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAgent
{
    class StupidAgent : Agent
    {
        static public StupidAgent Create(IMapPhysics mapPhysics)
        {
            return new StupidAgent(mapPhysics);
        }

        private StupidAgent(IMapPhysics mapPhysics)
            : base(mapPhysics)
        {
        }
        protected override void HandleNewCave(Cave newCave)
        {
            
        }
        protected override void HandleWorld(World world)
        {

        }
        protected override Move CalculateMove()
        {
            return new Move(GetPassive(), GetActive());
        }

        private PassiveAct GetPassive()
        {
            Random rnd = new Random();
            int random = rnd.Next(0, (int)PassiveAct.ROLL + 1);

            return (PassiveAct)random;
        }
        private ActiveAct GetActive()
        {
            ActiveAct action = ActiveAct.GO;
            Cave currentCave = m_mapPhysics.cave;

            if (currentCave.isGold)
            {
                action = (m_info.arrowCount == 0) ? ActiveAct.TAKE : ActiveAct.SHOOT;
            }
            else if (currentCave.isBone && m_info.arrowCount != 0)
            {
                action = ActiveAct.SHOOT;
            }

            return action;
        }
    }
}
