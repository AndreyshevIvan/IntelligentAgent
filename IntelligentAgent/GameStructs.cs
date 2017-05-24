using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace IntelligentAgent
{
    enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT,
    }

    struct AgentInfo
    {
        [JsonProperty(PropertyName = "arrowcount")]
        public int arrowCount { get; set; }
        [JsonProperty(PropertyName = "aname")]
        public string agentName { get; set; }
        [JsonProperty(PropertyName = "dir")]
        public Direction currentDir { get; set; }
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
        public bool isVisible
        {
            get
            {
                if (m_isVisiable == null)
                {
                    return false;
                }

                return m_isVisiable.ToLower() == "1" || m_isVisiable.ToLower() == "true";
            }
            set
            {
                m_isVisiable = "true";
            }
        }
        public List<Direction> aviableDir { get; set; }
        public bool isAvailable { get { return !isMonster && !isHole; } }
        public string hash { get { return Utils.CaveHash(row, coll); } }
        public Pair<int, int> coord { get { return new Pair<int, int>(row, coll); } }

        public bool IsAvailable(int freeLives = 0)
        {
            if (!isMonster)
            {
                if (isHole)
                {
                    return (freeLives > 0);
                }
                return true;
            }
            return false;
        }

        [JsonProperty(PropertyName = "isVisiable")]
        private string m_isVisiable { get; set; }
    }
    struct World
    {
        [JsonProperty(PropertyName = "newcaveopened")]
        public string caveOpenedCount { get; set; }
        [JsonProperty(PropertyName = "tiktak")]
        public int tiktak { get; set; }
        public bool isMonsterAlive
        {
            get
            {
                return m_isMonsterAlive == "true" || m_isMonsterAlive == "1";
            }
        }
        public bool isGoldFinded { get { return m_isGoldFinded != 0; } }

        [JsonProperty(PropertyName = "ismonsteralive")]
        private string m_isMonsterAlive { get; set; }
        [JsonProperty(PropertyName = "isgoldfinded")]
        private int m_isGoldFinded { get; set; }

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
            m_caves = new Cave[rowsCount, collsCount];
            m_monsterChance = new int[rowsCount, collsCount];
            m_holeChance = new int[rowsCount, collsCount];
            m_checkedBones = new List<Pair<int, int>>();
            m_checkedWings = new List<Pair<int, int>>();

            InitCaves();
            Utils.FillMatrix(ref m_monsterChance, 0);
            Utils.FillMatrix(ref m_holeChance, 0);
        }
        public Cave GetCave(int row, int coll)
        {
            Validate(row, coll);
            return m_caves[row, coll];
        }
        public int GetAttention(Cave cave)
        {
            Validate(cave);
            int row = cave.row;
            int coll = cave.coll;

            int monsterChance = m_monsterChance[row, coll];
            int holeChance = m_holeChance[row, coll];

            return monsterChance + holeChance;
        }
        public void AddCave(Cave cave)
        {
            Validate(cave);
            m_caves[cave.row, cave.coll] = cave;
        }
        public void MarkMonster(Cave bonesCave)
        {
            Validate(bonesCave);
            List<Pair<int, int>> brothersCoords = GetBroCoords(bonesCave);

            foreach (Pair<int, int> coordinate in brothersCoords)
            {
                MarkMonster(coordinate.first, coordinate.second);
            }
        }
        public void ClearMonsterMarks()
        {
            Utils.FillMatrix(ref m_monsterChance, 0);
        }
        public void ClearHoleMarks()
        {
            Utils.FillMatrix(ref m_holeChance, 0);
        }
        public List<Cave> ToList()
        {
            List<Cave> cavesList = new List<Cave>();

            foreach (Cave cave in m_caves)
            {
                cavesList.Add(cave);
            }

            return cavesList;
        }
        public bool IsExist(int row, int coll)
        {
            try
            {
                Validate(row, coll);
            }
            catch(Exception) { return false; }

            return true;
        }
        public void UpdateAttentions()
        {
            UpdateBones();
            UpdateWings();
        }

        private void UpdateBones()
        {
            foreach (Cave cave in m_caves)
            {
                if (cave.isWind && !IsHoleOpen(cave) && !m_checkedWings.Contains(cave.coord))
                {
                    List<Pair<int, int>> brothers = GetBroCoords(cave);
                    foreach (Pair<int, int> coord in brothers)
                    {
                        m_holeChance[coord.first, coord.second]++;
                        m_checkedWings.Add(cave.coord);
                    }
                }
            }
        }
        private void UpdateWings()
        {
            foreach (Cave cave in m_caves)
            {
                if (cave.isBone && !IsMonsterOpen(cave) && !m_checkedBones.Contains(cave.coord))
                {
                    List<Pair<int, int>> brothers = GetBroCoords(cave);
                    foreach (Pair<int, int> coord in brothers)
                    {
                        m_monsterChance[coord.first, coord.second]++;
                        m_checkedBones.Add(cave.coord);
                    }
                }
            }
        }
        private void InitCaves()
        {
            for (int i = 0; i < m_rowsCount; i++)
            {
                for (int j = 0; j < m_collsCount; j++)
                {
                    m_caves[i, j].row = i;
                    m_caves[i, j].coll = j;
                }
            }
        }
        private void Validate(Cave cave)
        {
            int row = cave.row;
            int coll = cave.coll;
            Validate(row, coll);
        }
        private void Validate(int row, int coll)
        {
            if (row < 0 ||
                coll < 0 ||
                row >= m_rowsCount ||
                coll >= m_collsCount)
            {
                throw new GameException(EMessage.CAVE_OVERFLOW);
            }
        }
        private List<Pair<int, int>> GetBroCoords(Cave cave)
        {
            List<Pair<int, int>> result = new List<Pair<int, int>>();

            int row = cave.row;
            int coll = cave.coll;

            if (IsExist(row + 1, coll))
                result.Add(new Pair<int, int>(row + 1, coll));

            if (IsExist(row - 1, coll))
                result.Add(new Pair<int, int>(row - 1, coll));

            if (IsExist(row, coll + 1))
                result.Add(new Pair<int, int>(row, coll + 1));

            if (IsExist(row, coll - 1))
                result.Add(new Pair<int, int>(row, coll - 1));

            return result;
        }
        private void MarkMonster(int row, int coll)
        {
            Validate(row, coll);
            m_monsterChance[row, coll] += 1;
        }
        private bool IsHoleOpen(Cave cave)
        {
            List<Pair<int, int>> brothers = GetBroCoords(cave);
            foreach(Pair<int, int> coord in brothers)
            {
                Cave brother = m_caves[coord.first, coord.second];
                if (brother.isVisible && brother.isHole)
                {
                    return true;
                }
            }

            return false;
        }
        private bool IsMonsterOpen(Cave cave)
        {
            List<Pair<int, int>> brothers = GetBroCoords(cave);
            foreach (Pair<int, int> coord in brothers)
            {
                Cave brother = m_caves[coord.first, coord.second];
                if (brother.isVisible && brother.isMonster)
                {
                    return true;
                }
            }

            return false;
        }

        private Cave[,] m_caves;
        private int[,] m_holeChance;
        private int[,] m_monsterChance;
        private int m_rowsCount;
        private int m_collsCount;
        private List<Pair<int, int>> m_checkedWings;
        private List<Pair<int, int>> m_checkedBones;
    }
    struct SearchNode
    {
        public SearchNode(int row, int coll, int freeLives = 0)
        {
            m_row = row;
            m_coll = coll;
            m_way = new List<Direction>();
            m_hash = Utils.CaveHash(row, coll);
            lives = freeLives;
        }
        public SearchNode(int row, int coll, List<Direction> parentWay, Direction nodeDir)
        {
            m_row = row;
            m_coll = coll;
            m_way = new List<Direction>(parentWay);
            m_way.Add(nodeDir);
            m_hash = Utils.CaveHash(row, coll);
            lives = 0;
        }
        public void AddDirection(Direction direction)
        {
            m_way.Add(direction);
        }

        public List<Direction> way { get { return m_way; } }
        public string hash { get { return m_hash; } }
        public int row { get { return m_row; } }
        public int coll { get { return m_coll; } }
        public int lives { get; set; }

        private List<Direction> m_way;
        private string m_hash;
        private int m_coll;
        private int m_row;
    }
    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            this.first = first;
            this.second = second;
        }

        public T first { get; set; }
        public U second { get; set; }
    };
}
