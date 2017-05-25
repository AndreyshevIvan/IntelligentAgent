using System;
using System.Collections.Generic;

namespace IntelligentAgent
{
    class OpenWorldAgent : Agent
    {
        static public OpenWorldAgent Create(IMapPhysics mapPhysics)
        {
            return new OpenWorldAgent(mapPhysics);
        }
        private OpenWorldAgent(IMapPhysics mapPhysics)
            : base(mapPhysics)
        {
        }

        protected override void HandleNewData()
        {
            if (!m_mapPhysics.GetOpenWorld(ref m_cavesMap))
            {
                throw new GameException(EMessage.OW_AGENT_WORLD_NOT_OPEN);
            }

            m_info = m_mapPhysics.agentInfo;
            m_world = m_mapPhysics.world;
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
            return new Move(PassiveAct.NONE, ActiveAct.TAKE);
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
                    if (GetWay(m_mapPhysics.cave, dstCave, ref way, freeLives))
                    {
                        PassiveAct rotation = GetRollTo(m_info.currentDir, way[0]);
                        return new Move(rotation, ActiveAct.GO);
                    }
                }
            }

            return GetRandomMove();
        }
        private Move GetToGoldMove()
        {
            foreach (Cave dstCave in m_cavesMap.ToList())
            {
                if (dstCave.isGold)
                {
                    List<Direction> way = new List<Direction>();
                    if (GetWay(m_mapPhysics.cave, dstCave, ref way, freeLives))
                    {
                        PassiveAct rotation = GetRollTo(m_info.currentDir, way[0]);
                        return new Move(rotation, ActiveAct.GO);
                    }
                }
            }

            return new Move(GetRandomPassive(), GetRandomActive());
        }
    }
}
