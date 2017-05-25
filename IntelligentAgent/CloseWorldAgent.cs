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
            UpdateKillSystem();
            m_world = m_mapPhysics.world;
            m_info = m_mapPhysics.agentInfo;
            m_cavesMap.AddCave(m_mapPhysics.cave);
            m_cavesMap.UpdateAttentions();
        }
        protected override Move CalculateMove()
        {
            if (IsMonsterVisible())
            {
                return KillTactic();
            }
            else if (IsGoldVisible())
            {
                return TakeTactic();
            }

            return ScoutTactic();
        }
        protected override bool AddToWayPredicate(SearchNode parent, Cave child)
        {
            Cave wayCave = m_cavesMap.GetCave(child.row, child.coll);
            if (wayCave.isVisible && !wayCave.isHole)
            {
                return true;
            }
            bool isEmpty = wayCave.isVisible && wayCave.isAvailable;
            bool isEasy = m_cavesMap.GetHoleChance(wayCave) < 2;

            return isEmpty || isEasy;
        }
        private void UpdateScoutSystem()
        {
            if (!m_isScout)
            {
                return;
            }

            int currRow = m_mapPhysics.cave.row;
            int currColl = m_mapPhysics.cave.coll;
            int scoutRow = m_scoutCell.row;
            int scoutColl = m_scoutCell.coll;

            if (currRow == scoutRow && currColl == scoutColl ||
                m_cavesMap.GetMonsterChance(scoutRow, scoutColl) > 1)
            {
                m_isScout = false;
            }
        }
        private void UpdateKillSystem()
        {
            if (m_world.isMonsterAlive) return;
            m_cavesMap.ClearMonsterMarks();
        }
        private Move KillTactic()
        {
            if (!IsOnLineWithMonster(m_mapPhysics.cave))
            {
                return GetToMonsterLineMove();
            }

            Move killMove = GetKillMonsterMove();
            Direction newDir = GetDirection(m_info.currentDir, killMove.passive);
            m_cavesMap.ClearMonsterLine(m_mapPhysics.cave, newDir);
            return killMove;
        }
        private Move TakeTactic()
        {
            if (m_mapPhysics.cave.isGold)
            {
                Move move = new Move(PassiveAct.NONE, ActiveAct.TAKE);
                return TryDoFinalShoot(move);
            }

            Cave goldCave = new Cave();
            List<Direction> way = new List<Direction>();

            if (m_cavesMap.GetGoldCave(ref goldCave) &&
                GetWay(m_mapPhysics.cave, goldCave, ref way, freeLives))
            {
                PassiveAct rotation = GetRollTo(m_info.currentDir, way[0]);
                return new Move(rotation, ActiveAct.GO);
            }

            return GetRandomMove();
        }
        private Move TryDoFinalShoot(Move finalMove)
        {
            Cave currCave = m_mapPhysics.cave;
            List<Cave> caves = m_cavesMap.GetSemilineCaves(currCave);
            Cave monsterCave = caves[0];

            foreach (Cave cave in caves)
            {
                int currentChance = m_cavesMap.GetMonsterChance(cave);
                int newChance = m_cavesMap.GetMonsterChance(cave);
                Direction dir = m_info.currentDir;

                if (newChance != 0 && currentChance >= newChance && !cave.isVisible)
                {
                    PassiveAct roll = GetRollTo(currCave, dir, cave);
                    finalMove = new Move(roll, ActiveAct.SHOOT);
                }
            }

            return finalMove;
        }
        private Move ScoutTactic()
        {
            Cave cave = m_mapPhysics.cave;
            Direction dir = m_info.currentDir;
            List<Direction> way = null;

            if ((m_isScout || GetBestCave(ref m_scoutCell)) &&
                GetWay(cave, m_scoutCell, ref way, freeLives))
            {
                PassiveAct rotation = GetRollTo(dir, way[0]);
                return new Move(rotation, ActiveAct.GO);
            }

            try
            {
                PassiveAct scoutPass = GetRollTo(cave, dir, m_scoutCell);
                return new Move(scoutPass, ActiveAct.GO);
            } catch {}

            return GetRandomMove();
        }
        private Move GetToMonsterLineMove()
        {
            List<Direction> way = new List<Direction>();
            foreach (Cave dstCave in m_cavesMap.ToList())
            {
                bool isCaveValid = dstCave.isVisible && dstCave.isAvailable;
                if (isCaveValid && IsOnLineWithMonster(dstCave) &&
                    GetWay(m_mapPhysics.cave, dstCave, ref way, freeLives))
                {
                    PassiveAct roll = GetRollTo(m_info.currentDir, way[0]);
                    return new Move(roll, ActiveAct.GO);
                }
            }

            return GetRandomMove();
        }
        private Move GetKillMonsterMove()
        {
            Cave monsterCave = new Cave();
            if (!GetMonsterCave(ref monsterCave))
            {
                throw new GameException("GetKillMonsterMove", "Monster cave not found.");
            }

            Direction dir = m_info.currentDir;
            int row = m_mapPhysics.cave.row;
            int coll = m_mapPhysics.cave.coll;
            int mRow = monsterCave.row;
            int mColl = monsterCave.coll;

            PassiveAct roll = GetRollTo(row, coll, dir, mRow, mColl);
            return new Move(roll, ActiveAct.SHOOT);
        }
        private bool IsGoldVisible()
        {
            if (m_cavesMap.closeCount == 1)
            {
                return true;
            }

            foreach (Cave cave in m_cavesMap.ToList())
            {
                if (cave.isGold) return true;
            }

            return false;
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
                if (m_cavesMap.GetMonsterChance(cave) < 2 || cave.isVisible)
                {
                    continue;
                }

                monsterCave = cave;
                return true;
            }
            return false;
        }
        private bool IsMonsterVisible()
        {
            Cave cave = new Cave();
            bool isALive = m_world.isMonsterAlive;
            bool isArrowsExist = m_info.arrowCount > 0;

            return isALive && isArrowsExist && GetMonsterCave(ref cave);
        }

        private Cave m_scoutCell;
    }
}
