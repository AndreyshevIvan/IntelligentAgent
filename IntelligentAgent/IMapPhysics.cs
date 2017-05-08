using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAgent
{
    interface IMapPhysics
    {
        void SetMove(Move move);

        Cave cave { get; }
        World world { get; }
    }
}
