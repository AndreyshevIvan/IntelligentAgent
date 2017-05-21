using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAgent
{
    interface IMapPhysics
    {
        void InitCavesMap(ref CavesMap cavesMap);
        void SetMove(Move move);
        bool GetOpenWorld(ref CavesMap cavesMap);
        bool GetMonsterCave(ref Cave monsterCave);

        int monsterRow { get; }
        int monsterColl { get; }

        Cave cave { get; }
        World world { get; }
        AgentInfo agentInfo { get; }
    }
}
