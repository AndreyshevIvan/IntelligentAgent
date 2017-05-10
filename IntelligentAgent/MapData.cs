using Newtonsoft.Json;

namespace IntelligentAgent
{
    class MapData
    {
        public MapData(string mapJson)
        {
            // TODO: parse string with Newtonsoft.Json to class properties
            // Ready to start

            m_cave = new Cave(); // Only for debug
            m_world = new World(); // Only for debug
        }
        public bool GetOpenWorld(ref CavesMap cavesMap)
        {
            if (!m_isOpenWorld || !isValid)
            {
                return false;
            }

            cavesMap = new CavesMap(4, 4);
            return true;
        }

        public bool isValid { get { return m_isValid; } }
        public string endLog { get { return m_endLog; } }
        public Cave currentCave { get { return m_cave; } }
        public World currentWorld { get { return m_world; } }

        private bool m_isValid = true;
        private bool m_isOpenWorld = false;
        private string m_endLog = "";
        private Cave m_cave;
        private World m_world;
    }
}
