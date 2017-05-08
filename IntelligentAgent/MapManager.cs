using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace IntelligentAgent
{
    class MapManager : IMapPhysics
    {
        static public MapManager Create(string idGame, string idUser)
        {
            return new MapManager(idGame, idUser);
        }
        public void DoMove(PassiveAct passive, ActiveAct active)
        {
            string actInfo = CreateAct(passive, active);
        }
        public bool IsGameEnd()
        {
            return true;
        }

        protected MapManager(string idGame, string idUser)
        {
            m_gameInfo = "gameid=" + idGame + "&";
            m_gameInfo += "userid=" + idUser + "&";

            DoMove(PassiveAct.NONE, ActiveAct.NONE);
        }

        private string CreateAct(PassiveAct passive, ActiveAct active)
        {
            string act = "act=";

            switch (passive)
            {
                case PassiveAct.ON_LEFT:
                    act += "onLeft";
                    break;
                case PassiveAct.ON_RIGHT:
                    act += "onRight";
                    break;
                case PassiveAct.ROLL:
                    act += "upSideDn";
                    break;
                default:
                    act += "noAct";
                    break;
            }

            act += "%20";

            switch (active)
            {
                case ActiveAct.GO:
                    act += "Go";
                    break;
                case ActiveAct.SHOOT:
                    act += "Shoot";
                    break;
                case ActiveAct.TAKE:
                    act += "Take";
                    break;
                default:
                    act += "noAct";
                    break;
            }

            return act;
        }

        private const string m_url = "https://www.mooped.net";
        private const string m_requestRoot = "/local/its/index.php?module=game&action=agentaction&";
        private string m_gameInfo = "gameid=0&userid=0&";
    }
}
