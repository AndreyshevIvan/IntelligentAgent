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
            HandleNewCave(m_mapPhysics.cave);
            HandleWorld(m_mapPhysics.world);
            m_info = m_mapPhysics.agentInfo;
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
        protected bool GetWay(Cave from, Cave to, ref List<Direction> way)
        {
            List<string> processHashes = new List<string>();
            Queue<SearchNode> searchQueue = new Queue<SearchNode>();
            searchQueue.Enqueue(new SearchNode(from.row, from.coll));

            while (searchQueue.Count != 0)
            {
                SearchNode top = searchQueue.Dequeue();

                if (top.hash == to.hash)
                {
                    way = top.way;
                    return true;
                }

                if (m_cavesMap.IsExist(top.row - 1, top.coll)) // top
                {
                    SearchNode node = new SearchNode(top.row - 1, top.coll, top.way, Direction.UP);
                    if (!processHashes.Contains(node.hash))
                    {
                        processHashes.Add(node.hash);
                        searchQueue.Enqueue(node);
                    }
                }

                if (m_cavesMap.IsExist(top.row, top.coll + 1)) // right
                {
                    SearchNode node = new SearchNode(top.row, top.coll + 1, top.way, Direction.RIGHT);
                    if (!processHashes.Contains(node.hash))
                    {
                        processHashes.Add(node.hash);
                        searchQueue.Enqueue(node);
                    }
                }

                if (m_cavesMap.IsExist(top.row + 1, top.coll)) // down
                {
                    SearchNode node = new SearchNode(top.row + 1, top.coll, top.way, Direction.DOWN);
                    if (!processHashes.Contains(node.hash))
                    {
                        processHashes.Add(node.hash);
                        searchQueue.Enqueue(node);
                    }
                }

                if (m_cavesMap.IsExist(top.row, top.coll - 1)) // left
                {
                    SearchNode node = new SearchNode(top.row, top.coll - 1, top.way, Direction.LEFT);
                    if (!processHashes.Contains(node.hash))
                    {
                        processHashes.Add(node.hash);
                        searchQueue.Enqueue(node);
                    }
                }
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

        protected IMapPhysics m_mapPhysics;
        protected AgentInfo m_info;
        protected CavesMap m_cavesMap;
        protected World m_world;
    }
}
