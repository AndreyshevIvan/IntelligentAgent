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

            IList<JToken> JsonKnowCaves = JsonGameInfo["text"]["iagent"]["knowCaves"].Children().Children().ToList();
            knowCaves = new List<Cave>();
            foreach(JToken cave in JsonKnowCaves)
            {
                Cave searchCave = cave.ToObject<Cave>();
                IList<JToken> searchCaveDir = cave["dirList"].ToList();
                searchCave.aviableDir = new List<DirList>();
                foreach (JToken direction in searchCaveDir)
                {
                    DirList searchDir = direction.ToObject<DirList>();
                    searchCave.aviableDir.Add(searchDir);
                }
                knowCaves.Add(searchCave);
            }

            m_world = JsonGameInfo["text"]["worldinfo"].ToObject<World>();
            m_cave = JsonGameInfo["text"]["currentcave"].ToObject<Cave>();
            m_agent = JsonGameInfo["text"]["iagent"].ToObject<AgentInfo>();
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

        public bool isValid { get { return m_isValid; } }
        public Cave currentCave { get { return m_cave; } }
        public IList<Cave> knowCaves { get; set; }
        public World currentWorld { get { return m_world; } }
        public AgentInfo currentAgentInfo { get { return m_agent; } }

        private bool m_isValid = true;
        private bool m_isOpenWorld = false;
        private Cave m_cave;
        private World m_world;
        private AgentInfo m_agent;
    }
}
