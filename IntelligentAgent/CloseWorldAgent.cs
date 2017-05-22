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
            m_world = m_mapPhysics.world;
            m_info = m_mapPhysics.agentInfo;
            m_cavesMap.AddCaveByProperies(m_mapPhysics.cave);
        }
        protected override Move CalculateMove()
        {
            return new Move(GetRandomPassive(), GetRandomActive());
        }
    }
}
