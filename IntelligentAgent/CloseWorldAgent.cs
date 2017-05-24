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
            return ScoutTactic();
        }
        protected override bool AddToWayPredicate(SearchNode parent, Cave child)
        {
            int row = child.row;
            int coll = child.coll;

            Cave wayCave = m_cavesMap.GetCave(row, coll);
            bool isEmpty = wayCave.isVisible && wayCave.isAvailable;
            bool isEasy = m_cavesMap.GetAttention(wayCave) == 0;

            return isEmpty || isEasy;
        }

        private bool IsMonsterVisible()
        {
            return false;
        }
        private bool IsGoldVisible()
        {
            return false;
        }

        private Move KillTactic()
        {
            return GetRandomMove();
        }
        private Move TakeTactic()
        {
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

        private Cave m_scoutCell;
    }
}
