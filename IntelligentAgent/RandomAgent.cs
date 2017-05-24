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

        protected override void HandleNewData()
        {

        }
        protected override Move CalculateMove()
        {
            return GetRandomMove();
        }
    }
}
