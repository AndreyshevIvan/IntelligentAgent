using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace IntelligentAgent
{
    class MapManager : IMapPhysics
    {
        static public MapManager Create(string idGame, string idUser)
        {
            return new MapManager(idGame, idUser);
        }
        public void SetMove(Move move)
        {
            m_passive = move.passive;
            m_active = move.active;
        }
        public bool UpdateMap()
        {
            string actInfo = ToRequest(m_passive, m_active);
            string requestUrl = m_request + actInfo;

            try
            {
                m_data = UpdateData(requestUrl);
            }
            catch (Exception e)
            {
                m_endLog = e.Message;
                return false;
            }

            return true;
        }
        public Cave cave { get { return m_data.currentCave; } }
        public World world { get { return m_data.currentWorld; } }
        public AgentInfo agentInfo { get { return m_data.currentAgentInfo; } }
        public string url { get { return "https://www.mooped.net"; } }
        public string requestRoot
        {
            get { return "/local/its/index.php?module=game&action=agentaction&"; }
        }
        public string endLog { get { return m_endLog; } }

        protected MapManager(string idGame, string idUser)
        {
            string gameInfo = "gameid=" + idGame + "&";
            gameInfo += "userid=" + idUser + "&";
            m_request = url + requestRoot + gameInfo;

            m_passive = PassiveAct.NONE;
            m_active = ActiveAct.NONE;

            UpdateMap();
        }

        private MapData UpdateData(string requestUrl)
        {
            JObject GameInfo = GetMapJson(requestUrl);
            string errorType = GameInfo["error_type"].ToString();

            if (errorType == "notification")
            {
                return m_data;
            }
            else if (errorType == "error")
            {
                throw new GameException(GameInfo["error"].ToString());
            }

            return new MapData(GameInfo["text"]);
        }
        private JObject GetMapJson(string requestUrl)
        {
            WebRequest request = WebRequest.Create(requestUrl);
            Stream objStream = request.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);
            string json = "";
            string sLine = "";

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    json += sLine;
                }
            }

            return JObject.Parse(json);
        }
        private string ToRequest(PassiveAct passive, ActiveAct active)
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

        private PassiveAct m_passive;
        private ActiveAct m_active;
        private MapData m_data;
        private string m_request;
        private string m_endLog;
    }
}
