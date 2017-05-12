using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntelligentAgent
{
    class MapData
    {
        public MapData(JToken mapInfo)
        {
            m_currentCave = mapInfo["currentcave"].ToObject<Cave>();
            m_world = mapInfo["worldinfo"].ToObject<World>();
            m_agent = mapInfo["iagent"].ToObject<AgentInfo>();

            InitCavesMap(mapInfo);
        }

        public bool GetOpenWorld(ref CavesMap cavesMap)
        {
            if (!m_isOpenWorld)
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
            if (caves.Count<JToken>() == 0)
            {
                m_isOpenWorld = false;
                return;
            }

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

        public Cave currentCave { get { return m_currentCave; } }
        public List<Cave> knowCaves { get { return m_cavesMap; } }
        public World currentWorld { get { return m_world; } }
        public AgentInfo currentAgentInfo { get { return m_agent; } }

        private bool m_isOpenWorld = false;
        private Cave m_currentCave;
        private List<Cave> m_cavesMap;
        private World m_world;
        private AgentInfo m_agent;
    }
}
