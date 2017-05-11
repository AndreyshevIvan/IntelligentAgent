using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IntelligentAgent
{

    enum DirList
    {
        Up, Right, Down, Left
    }

    struct AgentInfo
    {
        [JsonProperty(PropertyName = "arrowcount")]
        public int arrowCount { get; set; }

        [JsonProperty(PropertyName = "aname")]
        public string agentName { get; set; }

        [JsonProperty(PropertyName = "dir")]
        public DirList currentDir { get; set; }

        [JsonProperty(PropertyName = "legscount")]
        public int legsCount { get; set; }

        [JsonProperty(PropertyName = "isagentalive")]
        public string isAgentAlive { get; set; }

        [JsonProperty(PropertyName = "havegold")]
        public string haveGold { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int id { get; set; }

        [JsonProperty(PropertyName = "gaid")]
        public int gameId { get; set; }
    }

    struct Cave
    {
        [JsonProperty(PropertyName = "rowN")]
        public int row { get; set; }

        [JsonProperty(PropertyName = "colN")]
        public int coll { get; set; }

        [JsonProperty(PropertyName = "isGold")]
        public bool isGold { get; set; }

        [JsonProperty(PropertyName = "isMonster")]
        public bool isMonster { get; set; }

        [JsonProperty(PropertyName = "isHole")]
        public bool isHole { get; set; }

        [JsonProperty(PropertyName = "isWind")]
        public bool isWind { get; set; }

        [JsonProperty(PropertyName = "isBones")]
        public bool isBone { get; set; }

        [JsonProperty(PropertyName = "isVisiable")]
        public string isVisiable { get; set; }

        public List<DirList> aviableDir { get; set; }
    }


    // ПРОБЛЕМА
    // в твоей архитектуре я не вижу обновления состояний Агента, исключительно обновление Мира
    // есть вариант впихнуть knowCaves в структуру World, тупо добавить public IList<Cave> knowCaves { get; set; }
    // и в MapData.cs прописать m_world.knowCaves = knowCaves
    struct World
    {
        [JsonProperty(PropertyName = "newcaveopened")]
        public string caveOpenedCount { get; set; }

        [JsonProperty(PropertyName = "isgoldfinded")]
        public string isGoldFinded { get; set; }

        [JsonProperty(PropertyName = "ismonsteralive")]
        public string isMonsterAlive { get; set; }

        [JsonProperty(PropertyName = "tiktak")]
        public int tiktak { get; set; }
    }

    struct Move
    {
        public Move(PassiveAct passive, ActiveAct active)
        {
            this.passive = passive;
            this.active = active;
        }

        public PassiveAct passive;
        public ActiveAct active;
    }

    struct CavesMap
    {
        public CavesMap(int rowsCount, int collsCount)
        {
            m_rowsCount = rowsCount;
            m_collsCount = collsCount;
            m_map = new Cave[rowsCount, collsCount];
        }
        public Cave GetCave(int row, int coll)
        {
            if (row < 0 || row >= m_rowsCount || coll < 0 || coll >= m_collsCount)
            {
                throw new GameException("Cave adress overflow when get from caves map");
            }

            return m_map[row, coll];
        }

        public void SetCave(Cave cave)
        {
            if (cave.row < 0 || cave.row >= m_rowsCount || cave.coll < 0 || cave.coll >= m_collsCount)
            {
                throw new GameException("Cave adress overflow when get from caves map");
            }
            m_map[cave.row, cave.coll] = cave;
        }

        private Cave[,] m_map;
        private int m_rowsCount;
        private int m_collsCount;
    }
}
