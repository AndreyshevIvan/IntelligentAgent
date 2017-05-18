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
        }

        public virtual void DoMove()
        {
            m_mapPhysics.GetOpenWorld(ref m_cavesMap);
            m_info = m_mapPhysics.agentInfo;
            HandleNewCave(m_mapPhysics.cave);
            HandleWorld(m_mapPhysics.world);
            Move move = CalculateMove();
            m_mapPhysics.SetMove(move);
        }

        protected abstract void HandleNewCave(Cave newCave);
        protected abstract void HandleWorld(World world);
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
        protected static PassiveAct GetRollTo(Direction oldDir, Direction newDir)
        {
            int delta = (int)oldDir - (int)newDir;
            PassiveAct result = (PassiveAct)((delta + 4) % 4);

            return result;
        }
        protected bool GetWay(Cave from, Cave to, ref List<Direction> way, int lives)
        {
            if (lives > 0 && GetWay(from, to, ref way, 0))
            {
                return true;
            }

            m_processHashes = new List<string>();
            m_searchQueue = new Queue<SearchNode>();
            m_searchQueue.Enqueue(new SearchNode(from.row, from.coll, lives));

            while (m_searchQueue.Count != 0)
            {
                SearchNode queueTop = m_searchQueue.Dequeue();
                if (queueTop.hash == to.hash)
                {
                    way = queueTop.way;
                    return true;
                }
                TryAddToQueue(queueTop, Direction.UP);
                TryAddToQueue(queueTop, Direction.DOWN);
                TryAddToQueue(queueTop, Direction.LEFT);
                TryAddToQueue(queueTop, Direction.RIGHT);
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

        private void TryAddToQueue(SearchNode parent, Direction direction)
        {
            int row = parent.row;
            int coll = parent.coll;

            CalcCoordinates(ref row, ref coll, direction);

            if (m_cavesMap.IsExist(row, coll))
            {
                string newHash = Utils.CaveHash(row, coll);
                bool isHashValid = !m_processHashes.Contains(newHash);
                Cave nextCave = m_cavesMap.GetCave(row, coll);

                if (isHashValid && nextCave.IsAvailable(parent.lives))
                {
                    List<Direction> way = parent.way;
                    SearchNode node = new SearchNode(row, coll, way, direction);
                    node.lives = (nextCave.isHole) ? parent.lives - 1 : parent.lives;
                    m_processHashes.Add(node.hash);
                    m_searchQueue.Enqueue(node);
                }
            }
        }
        private void CalcCoordinates(ref int row, ref int coll, Direction direction)
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

        private List<string> m_processHashes;
        private Queue<SearchNode> m_searchQueue;
    }
}
