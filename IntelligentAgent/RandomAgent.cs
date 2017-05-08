using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAgent
{
    class RandomAgent : Agent
    {
        static public RandomAgent Create(IMapPhysics mapPhysics)
        {
            return new RandomAgent(mapPhysics);
        }

        public override void DoMove()
        {
            m_mapPhysics.DoMove(PassiveAct.NONE, ActiveAct.NONE);
        }

        private RandomAgent(IMapPhysics mapPhysics)
            : base(mapPhysics)
        {
        }
    }
}
