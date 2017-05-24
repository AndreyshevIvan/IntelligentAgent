using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAgent
{
    class CloseWorldAgent : Agent
    {
        static public CloseWorldAgent Create(IMapPhysics mapPhysics)
        {
            return new CloseWorldAgent(mapPhysics);
        }
        private CloseWorldAgent(IMapPhysics mapPhysics)
            : base(mapPhysics)
        {
        }

        protected override void HandleNewData()
        {
            UpdateScoutSystem();
            m_world = m_mapPhysics.world;
            m_info = m_mapPhysics.agentInfo;
            m_cavesMap.AddCave(m_mapPhysics.cave);
            m_cavesMap.UpdateAttentions();
        }
        protected override Move CalculateMove()
        {
            if (m_mapPhysics.cave.isGold)
            {
                return new Move(PassiveAct.NONE, ActiveAct.TAKE);
            }

            if (IsMonsterVisible())
            {
                KillTactic();
            }
            else if (IsGoldVisible())
            {
                TakeTactic();
            }

            return ScoutTactic();
        }
        protected override bool AddToWayPredicate(SearchNode parent, Cave child)
        {
            int row = child.row;
            int coll = child.coll;

            Cave wayCave = m_cavesMap.GetCave(row, coll);
            bool isEmpty = wayCave.isVisible && wayCave.isAvailable;
            bool isEasy = m_cavesMap.GetAttention(wayCave) < 2;

            return isEmpty || isEasy;
        }

        private bool IsGoldVisible()
        {
            foreach (Cave cave in m_cavesMap.ToList())
            {
                if (cave.isGold)
                {
                    return true;
                }
            }

            if (m_cavesMap.closeCount == 1)
            {
                return true;
            }

            return false;
        }
        private Move KillTactic()
        {
            if (IsOnLineWithMonster(m_mapPhysics.cave))
            {
                return GetKillMonsterMove();
            }

            return GetToMonsterLineMove();
        }
        private Move TakeTactic()
        {
            Cave goldCave = new Cave();
            if (m_cavesMap.GetGoldCave(ref goldCave))
            {
                List<Direction> way = new List<Direction>();
                if (GetWay(m_mapPhysics.cave, goldCave, ref way, freeLives))
                {
                    PassiveAct rotation = GetRollTo(m_info.currentDir, way[0]);
                    return new Move(rotation, ActiveAct.GO);
                }
            }

            return GetRandomMove();
        }
        private Move ScoutTactic()
        {
            if (m_isScout || GetBestCave(ref m_scoutCell))
            {
                List<Direction> way = null;
                if (GetWay(m_mapPhysics.cave, m_scoutCell, ref way, freeLives))
                {
                    PassiveAct rotation = GetRollTo(m_info.currentDir, way[0]);
                    return new Move(rotation, ActiveAct.GO);
                }
            }

            Console.WriteLine("Random move");
            return GetRandomMove();
        }
        private void UpdateScoutSystem()
        {
            Console.WriteLine(m_scoutCell.row.ToString() + " " + m_scoutCell.coll.ToString());

            if (!m_isScout)
            {
                return;
            }

            int currRow = m_mapPhysics.cave.row;
            int currColl = m_mapPhysics.cave.coll;

            if (currRow == m_scoutCell.row && currColl == m_scoutCell.coll)
            {
                m_isScout = false;
            }
        }
        private Move GetToMonsterLineMove()
        {
            foreach (Cave dstCave in m_cavesMap.ToList())
            {
                if (dstCave.isVisible && dstCave.isAvailable && IsOnLineWithMonster(dstCave))
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
        private bool IsOnLineWithMonster(Cave currentCave)
        {
            Cave monsterCave = new Cave();
            if (!GetMonsterCave(ref monsterCave))
            {
                return false;
            }

            bool isCommonRow = monsterCave.row == currentCave.row;
            bool isCommonColl = monsterCave.coll == currentCave.coll;

            return isCommonRow || isCommonColl;
        }
        private bool GetMonsterCave(ref Cave monsterCave)
        {
            foreach (Cave cave in m_cavesMap.ToList())
            {
                if (m_cavesMap.GetMonsterChance(cave) >= 2)
                {
                    monsterCave = cave;
                    return true;
                }
            }
            return false;
        }
        private bool IsMonsterVisible()
        {
            Cave cave = new Cave();
            return GetMonsterCave(ref cave);
        }
        private Move GetKillMonsterMove()
        {
            Cave monsterCave = new Cave();
            if (!GetMonsterCave(ref monsterCave))
            {
                throw new GameException("GetKillMonsterMove", "Monster cave not found.");
            }

            Direction dir = m_info.currentDir;
            int currRow = m_mapPhysics.cave.row;
            int currColl = m_mapPhysics.cave.coll;
            int monsterRow = monsterCave.row;
            int monsterColl = monsterCave.coll;

            PassiveAct passive = GetRollTo(currRow, currColl, dir, monsterRow, monsterColl);
            return new Move(passive, ActiveAct.SHOOT);
        }

        private Cave m_scoutCell;
    }
}
