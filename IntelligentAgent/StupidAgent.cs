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
                if (!IsOnLineWithMonster(m_mapPhysics.cave))
                {
                    return GetToMonsterLineMove();
                }
                return GetKillMonsterMove();
            }
            else if (!m_mapPhysics.cave.isGold)
            {
                return GetToGoldMove();
            }
            return GetTakeGoldMove();
        }
        private bool IsOnLineWithMonster(Cave currentCave)
        {
            Cave monsterCave = new Cave();
            if (!m_mapPhysics.GetMonsterCave(ref monsterCave))
            {
                return false;
            }

            bool isCommonRow = monsterCave.row == currentCave.row;
            bool isCommonColl = monsterCave.coll == currentCave.coll;

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
        private Move GetToMonsterLineMove()
        {
            foreach (Cave dstCave in m_cavesMap.ToList())
            {
                if (dstCave.isAvailable && IsOnLineWithMonster(dstCave))
                {
                    List<Direction> way = new List<Direction>();
                    if (GetWay(m_mapPhysics.cave, dstCave, ref way) && way.Count != 0)
                    {
                        PassiveAct rotation = GetRollTo(m_info.currentDir, way[0]);
                        return new Move(rotation, ActiveAct.GO);
                    }
                }
            }

            return new Move(GetRandomPassive(), GetRandomActive());
        }
        private Move GetToGoldMove()
        {
            // find cave with gold
            // calculate way
            // if way find return first move
            // else find new cave on line with monster
            // do while not check all caves

            return new Move(GetRandomPassive(), GetRandomActive());
        }
        private Move GetTakeGoldMove()
        {
            return new Move(PassiveAct.NONE, ActiveAct.TAKE);
        }
    }
}
