using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAgent
{
    enum PassiveAct
    {
        NONE,
        ON_LEFT,
        ROLL,
        ON_RIGHT,
    }
    enum ActiveAct
    {
        NONE,
        GO,
        SHOOT,
        TAKE,
    }

    abstract class Agent
    {
        protected Agent(IMapPhysics mapPhysics)
        {
            m_mapPhysics = mapPhysics;
            m_mapPhysics.InitCavesMap(ref m_cavesMap);
        }

        public virtual void DoMove()
        {
            HandleNewData();
            Move move = CalculateMove();
            m_mapPhysics.SetMove(move);
        }

        protected abstract void HandleNewData();
        protected abstract Move CalculateMove();

        protected static PassiveAct GetRollTo(int currR, int currC, Direction currDir, int newR, int newC)
        {
            if (currC != newC && currR != newR)
            {
                throw new GameException("GetRollTo: New cell is not on line with current cell");
            }

            Direction newDir = Direction.DOWN;

            if (currR == newR)
            {
                if (currC > newC) newDir = Direction.LEFT;
                else if (currC < newC) newDir = Direction.RIGHT;
            }
            else if (currC == newC)
            {
                if (currR > newR) newDir = Direction.UP;
                else if (currR < newR) newDir = Direction.DOWN;
            }

            return GetRollTo(currDir, newDir);
        }
        protected static PassiveAct GetRollTo(Cave current, Direction currDir, Cave next)
        {
            return GetRollTo(current.row, current.coll, currDir, next.row, next.coll);
        }
        protected static PassiveAct GetRollTo(Direction oldDir, Direction newDir)
        {
            int delta = (int)oldDir - (int)newDir;
            PassiveAct result = (PassiveAct)((delta + 4) % 4);

            return result;
        }
        protected Direction GetDirection(Direction dir, PassiveAct roll)
        {
            if (roll == PassiveAct.NONE)
            {
                return dir;
            }
            return (Direction)(((int)dir + ((int)roll) + 2) % 4);
        }
        protected bool GetBestCave(ref Cave bestCave)
        {
            List<Cave> possibleCaves = new List<Cave>();
            if (GetPossibleCaves(ref possibleCaves))
            {
                bestCave = possibleCaves[0];
                int bestAttention = m_cavesMap.GetAttention(bestCave);

                foreach (Cave cave in possibleCaves)
                {
                    int newAttention = m_cavesMap.GetAttention(cave);
                    if (newAttention < bestAttention)
                    {
                        bestAttention = newAttention;
                        bestCave = cave;
                    }
                }

                m_isScout = true;
                return true;
            }
            return false;
        }
        protected bool GetPossibleCaves(ref List<Cave> possibleCaves)
        {
            Cave start = m_mapPhysics.cave;
            possibleCaves.Clear();
            m_searchQueue.Clear();
            m_processHashes.Clear();
            m_searchQueue.Enqueue(new SearchNode(start.row, start.coll));

            while (m_searchQueue.Count != 0)
            {
                SearchNode queueTop = m_searchQueue.Dequeue();
                AddToPossibleList(queueTop, Direction.UP, ref possibleCaves);
                AddToPossibleList(queueTop, Direction.DOWN, ref possibleCaves);
                AddToPossibleList(queueTop, Direction.LEFT, ref possibleCaves);
                AddToPossibleList(queueTop, Direction.RIGHT, ref possibleCaves);
            }

            return possibleCaves.Count != 0;
        }
        protected bool GetWay(Cave from, Cave to, ref List<Direction> way, int lives)
        {
            if (lives > 0 && GetWay(from, to, ref way, 0))
            {
                return true;
            }

            m_searchQueue.Clear();
            m_processHashes.Clear();
            m_searchQueue.Enqueue(new SearchNode(from.row, from.coll, lives));

            while (m_searchQueue.Count != 0)
            {
                SearchNode queueTop = m_searchQueue.Dequeue();
                if (queueTop.hash == to.hash)
                {
                    way = queueTop.way;
                    return true;
                }
                AddToWaySearch(queueTop, Direction.UP);
                AddToWaySearch(queueTop, Direction.DOWN);
                AddToWaySearch(queueTop, Direction.LEFT);
                AddToWaySearch(queueTop, Direction.RIGHT);
            }

            return false;
        }
        protected bool GetBestWay(Cave from, Cave to, ref List<Direction> way, int lives)
        {
            if (lives > 0 && GetWay(from, to, ref way, 0))
            {
                return true;
            }

            m_bestWayQueue.Clear();
            m_processHashes.Clear();
            m_bestWayQueue.Add(new SearchNode(from.row, from.coll, lives));

            while (m_bestWayQueue.Count != 0)
            {
                SearchNode queueTop = m_bestWayQueue[0];
                Cave cave = m_cavesMap.GetCave(queueTop.row, queueTop.coll);
                int bestChance = m_cavesMap.GetAttention(cave);
                foreach(SearchNode node in m_bestWayQueue)
                {
                    Cave newCave = m_cavesMap.GetCave(queueTop.row, queueTop.coll);
                    int newChance = m_cavesMap.GetAttention(newCave);
                    if (newChance < bestChance)
                    {
                        bestChance = newChance;
                        queueTop = node;
                    }
                }
                m_bestWayQueue.Remove(queueTop);
                if (queueTop.hash == to.hash)
                {
                    way = queueTop.way;
                    return true;
                }
                AddToBestWaySearch(queueTop, Direction.UP);
                AddToBestWaySearch(queueTop, Direction.DOWN);
                AddToBestWaySearch(queueTop, Direction.LEFT);
                AddToBestWaySearch(queueTop, Direction.RIGHT);
            }

            return false;
        }
        protected PassiveAct GetRandomPassive()
        {
            Random rnd = new Random();
            int random = rnd.Next(0, (int)PassiveAct.ROLL + 1);

            return (PassiveAct)random;
        }
        protected ActiveAct GetRandomActive()
        {
            Random rnd = new Random();
            int random = rnd.Next(0, (int)ActiveAct.TAKE + 1);

            return (ActiveAct)random;
        }
        protected Move GetRandomMove()
        {
            return new Move(GetRandomPassive(), GetRandomActive());
        }
        protected virtual bool AddToWayPredicate(SearchNode parent, Cave child)
        {
            return child.IsAvailable(parent.lives);
        }
        protected void CalcCoordinates(ref int row, ref int coll, Direction direction)
        {
            switch (direction)
            {
                case (Direction.UP):
                    row--;
                    break;
                case (Direction.RIGHT):
                    coll++;
                    break;
                case (Direction.DOWN):
                    row++;
                    break;
                case (Direction.LEFT):
                    coll--;
                    break;
                default:
                    break;
            }
        }

        private void AddToPossibleList(SearchNode parent, Direction dir, ref List<Cave> list)
        {
            int row = parent.row;
            int coll = parent.coll;

            CalcCoordinates(ref row, ref coll, dir);

            if (m_cavesMap.IsExist(row, coll))
            {
                string newHash = Utils.CaveHash(row, coll);
                bool isHashValid = !m_processHashes.Contains(newHash);
                Cave nextCave = m_cavesMap.GetCave(row, coll);

                if (isHashValid)
                {
                    m_processHashes.Add(newHash);
                    if (nextCave.isVisible && !nextCave.isHole)
                    {
                        SearchNode node = new SearchNode(row, coll);
                        m_searchQueue.Enqueue(node);
                        return;
                    }

                    list.Add(nextCave);
                }
            }
        }
        private void AddToWaySearch(SearchNode parent, Direction dir)
        {
            int row = parent.row;
            int coll = parent.coll;

            CalcCoordinates(ref row, ref coll, dir);

            if (m_cavesMap.IsExist(row, coll))
            {
                string newHash = Utils.CaveHash(row, coll);
                bool isHashValid = !m_processHashes.Contains(newHash);
                Cave nextCave = m_cavesMap.GetCave(row, coll);

                if (isHashValid && AddToWayPredicate(parent, nextCave))
                {
                    List<Direction> way = parent.way;
                    SearchNode node = new SearchNode(row, coll, way, dir);
                    node.lives = (nextCave.isHole) ? parent.lives - 1 : parent.lives;
                    m_processHashes.Add(node.hash);
                    m_searchQueue.Enqueue(node);
                }
            }
        }
        private void AddToBestWaySearch(SearchNode parent, Direction dir)
        {
            int row = parent.row;
            int coll = parent.coll;

            CalcCoordinates(ref row, ref coll, dir);

            if (m_cavesMap.IsExist(row, coll))
            {
                string newHash = Utils.CaveHash(row, coll);
                bool isHashValid = !m_processHashes.Contains(newHash);
                Cave nextCave = m_cavesMap.GetCave(row, coll);

                if (isHashValid && AddToWayPredicate(parent, nextCave))
                {
                    List<Direction> way = parent.way;
                    SearchNode node = new SearchNode(row, coll, way, dir);
                    node.lives = (nextCave.isHole) ? parent.lives - 1 : parent.lives;
                    m_processHashes.Add(node.hash);
                    m_bestWayQueue.Add(node);
                }
            }
        }

        protected int freeLives
        {
            get
            {
                int lives = m_info.legsCount;
                return (lives <= 1) ? 0 : lives - 1;
            }
        }

        protected IMapPhysics m_mapPhysics;
        protected AgentInfo m_info;
        protected CavesMap m_cavesMap;
        protected World m_world;
        protected bool m_isScout = false;
        Queue<SearchNode> m_searchQueue = new Queue<SearchNode>();
        List<SearchNode> m_bestWayQueue = new List<SearchNode>();
        List<string> m_processHashes = new List<string>();
    }
}
