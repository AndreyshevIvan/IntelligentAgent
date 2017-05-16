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
            foreach (Cave cave in m_cavesMap)
            {
                cavesMap.SetCave(cave);
            }

            return true;
        }
        public bool GetMonsterCave(ref Cave monsterCave)
        {
            foreach(Cave cave in m_cavesMap)
            {
                if (cave.isMonster)
                {
                    monsterCave = cave;
                    return true;
                }
            }
            return false;
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
                searchCave.aviableDir = new List<Direction>();
                foreach (JToken direction in searchCaveDir)
                {
                    Direction searchDir = direction.ToObject<Direction>();
                    searchCave.aviableDir.Add(searchDir);
                }
                m_cavesMap.Add(searchCave);
            }
        }

        public Cave currentCave { get { return m_currentCave; } }
        public World currentWorld { get { return m_world; } }
        public AgentInfo currentAgentInfo { get { return m_agent; } }

        private bool m_isOpenWorld = false;
        private Cave m_currentCave;
        private List<Cave> m_cavesMap;
        private World m_world;
        private AgentInfo m_agent;
    }
}
