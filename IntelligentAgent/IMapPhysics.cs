using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAgent
{
    interface IMapPhysics
    {
        void DoMove(PassiveAct passive, ActiveAct active);
        //Cave GetCaveInfo();
        //World GetWorldInfo();
    }
}
