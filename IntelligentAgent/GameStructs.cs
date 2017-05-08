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

    struct CavesMap
    {
        public CavesMap(int rowsCount, int collsCount)
        {
            m_rowsCount = rowsCount;
            m_collsCount = collsCount;
            m_map = new Cave[rowsCount, collsCount];
        }
        public Cave GetCave(int row, int coll)
        {
            if (row < 0 || row >= m_rowsCount || coll < 0 || coll >= m_collsCount)
            {
                throw new GameException("Cave adress overflow when get from caves map");
            }

            return m_map[row, coll];
        }

        private Cave[,] m_map;
        private int m_rowsCount;
        private int m_collsCount;
    }
}
