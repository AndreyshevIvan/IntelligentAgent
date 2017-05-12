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
            return new Move(PassiveAct.NONE, ActiveAct.NONE);
        }
    }
}
