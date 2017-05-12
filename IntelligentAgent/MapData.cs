using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntelligentAgent
{
    class MapData
    {
        public MapData(string mapJson)
        {
            JObject JsonGameInfo = JObject.Parse(mapJson);

            JToken mapInfo = JsonGameInfo["text"];
            string errorType = JsonGameInfo["error_type"].ToString();

            if (errorType == "error")
            {
                throw new GameException(JsonGameInfo["error"].ToString());
            }

            m_currentCave = JsonGameInfo["text"]["currentcave"].ToObject<Cave>();
            m_world = JsonGameInfo["text"]["worldinfo"].ToObject<World>();
            m_agent = JsonGameInfo["text"]["iagent"].ToObject<AgentInfo>();
            InitCavesMap(mapInfo);
        }

        public bool GetOpenWorld(ref CavesMap cavesMap)
        {
            if (!m_isOpenWorld || !isValid)
            {
                return false;
            }

            cavesMap = new CavesMap(4, 4);

            // Add caves about which we know
            foreach (Cave cave in knowCaves)
            {
                cavesMap.SetCave(cave);
            }
            return true;
        }

        private void InitCavesMap(JToken mapInfo)
        {
            JToken caves = mapInfo["iagent"]["knowCaves"];
            List<JToken> JsonKnowCaves = caves.Children().Children().ToList();
            m_cavesMap = new List<Cave>();

            foreach (JToken cave in JsonKnowCaves)
            {
                Cave searchCave = cave.ToObject<Cave>();
                List<JToken> searchCaveDir = cave["dirList"].ToList();
                searchCave.aviableDir = new List<DirList>();
                foreach (JToken direction in searchCaveDir)
                {
                    DirList searchDir = direction.ToObject<DirList>();
                    searchCave.aviableDir.Add(searchDir);
                }
                m_cavesMap.Add(searchCave);
            }
        }

        public bool isValid { get { return m_isValid; } }
        public Cave currentCave { get { return m_currentCave; } }
        public List<Cave> knowCaves { get { return m_cavesMap; } }
        public World currentWorld { get { return m_world; } }
        public AgentInfo currentAgentInfo { get { return m_agent; } }

        private bool m_isValid = true;
        private bool m_isOpenWorld = false;
        private Cave m_currentCave;
        private List<Cave> m_cavesMap;
        private World m_world;
        private AgentInfo m_agent;
    }
}
