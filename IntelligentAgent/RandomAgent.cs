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
        private RandomAgent(IMapPhysics mapPhysics)
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
            Random rnd = new Random();
            int randomAct = rnd.Next(0, (int)PassiveAct.ROLL + 1);
            PassiveAct passive = (PassiveAct)randomAct;

            randomAct = rnd.Next(0, (int)ActiveAct.TAKE + 1);
            ActiveAct active = (ActiveAct)randomAct;

            return new Move(passive, active);
        }
    }
}
