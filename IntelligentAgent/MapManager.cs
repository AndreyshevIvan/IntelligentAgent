using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

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
            string actInfo = ToRequest(passive, active);
            string requestUrl = m_request + actInfo;
            UpdateData(requestUrl);
            UpdateCave();
            UpdateWorld();
        }
        public bool IsGameEnd()
        {
            return true;
        }
        public Cave cave { get { return m_cave; } }
        public World world { get { return m_world; } }

        protected MapManager(string idGame, string idUser)
        {
            string gameInfo = "gameid=" + idGame + "&";
            gameInfo += "userid=" + idUser + "&";
            m_request = url + requestRoot + gameInfo;

            DoMove(PassiveAct.NONE, ActiveAct.NONE);
        }

        private void UpdateData(string requestUrl)
        {
            WebRequest request = WebRequest.Create(requestUrl);
            Stream objStream = request.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);
            string jsonStr = "";
            string sLine = "";

            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                {
                    jsonStr += sLine;
                    Console.WriteLine(sLine);
                }
            }

            ParseData(jsonStr);
        }
        private void UpdateCave()
        {
            m_cave = new Cave();
        }
        private void UpdateWorld()
        {
            m_world = new World();
        }
        private void ParseData(string jsonStr)
        {
            m_data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonStr);
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

        public string url
        {
            get { return "https://www.mooped.net"; }
        }
        public string requestRoot
        {
            get { return "/local/its/index.php?module=game&action=agentaction&"; }
        }

        private Dictionary<string, dynamic> m_data;
        private string m_request;
        private Cave m_cave;
        private World m_world;
    }
}
