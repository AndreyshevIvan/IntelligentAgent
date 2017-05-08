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

    class Agent
    {
        static public RandomAgent CreateRandom()
        {
            return new RandomAgent();
        }

        protected Agent()
        {
        }
    }
}
