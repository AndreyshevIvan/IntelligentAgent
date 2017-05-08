using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAgent
{
    struct Cave
    {
        /*
        public int row;
        public int coll;
        public bool isGold;
        public bool isMonster;
        public bool isHole;
        public bool isWind;
        public bool isBones;
        */
    }

    struct World
    {

    }

    struct Move
    {
        public Move(PassiveAct passive, ActiveAct active)
        {
            this.passive = passive;
            this.active = active;
        }

        public PassiveAct passive;
        public ActiveAct active;
    }
}
