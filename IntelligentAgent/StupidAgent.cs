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
            PassiveAct result = PassiveAct.NONE;

            result = GetRollTo(1, 1, Direction.RIGHT, 1, -2);
            result = GetRollTo(1, 1, Direction.RIGHT, 4, 1);
            result = GetRollTo(1, 1, Direction.RIGHT, 1, 4);
            result = GetRollTo(1, 1, Direction.RIGHT, -2, 1);
        }
        protected override void HandleWorld(World world)
        {
            m_world = world;
            m_mapPhysics.GetOpenWorld(ref m_cavesMap);
        }
        protected override Move CalculateMove()
        {
            if (m_world.isMonsterAlive)
            {
                if (!IsOnLineWithMonster())
                {
                    return GetToMonsterMove();
                }
                return GetKillMonsterMove();
            }
            else if (!m_mapPhysics.cave.isGold)
            {
                return GetToGoldMove();
            }
            return GetTakeGoldMove();
        }
        private bool IsOnLineWithMonster()
        {
            Cave monsterCave = new Cave();
            Cave currCave = m_mapPhysics.cave;

            if (!m_mapPhysics.GetMonsterCave(ref monsterCave))
            {
                return false;
            }

            bool isCommonRow = monsterCave.row == currCave.row;
            bool isCommonColl = monsterCave.coll == currCave.coll;

            return isCommonRow || isCommonColl;
        }
        private Move GetKillMonsterMove()
        {
            Direction dir = m_info.currentDir;
            int currRow = m_mapPhysics.cave.row;
            int currColl = m_mapPhysics.cave.coll;
            int monsterRow = m_mapPhysics.monsterRow;
            int monsterColl = m_mapPhysics.monsterColl;

            PassiveAct passive = GetRollTo(currRow, currColl, dir, monsterRow, monsterColl);
            return new Move(passive, ActiveAct.SHOOT);
        }
        private Move GetToMonsterMove()
        {
            return new Move(GetRandomPassive(), GetRandomActive());
        }
        private Move GetToGoldMove()
        {
            return new Move(GetRandomPassive(), GetRandomActive());
        }
        private Move GetTakeGoldMove()
        {
            return new Move(GetRandomPassive(), GetRandomActive());
        }
    }
}
