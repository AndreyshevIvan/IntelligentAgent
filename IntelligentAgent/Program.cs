using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RandomAgent randomAgent = null;
                MapManager mapManager = null;

                InitGame(args, ref randomAgent, ref mapManager);

                while (!mapManager.IsGameEnd())
                {
                    randomAgent.DoMove();
                }

                Console.WriteLine("Game end.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void InitGame(string[] programArgs, ref RandomAgent agent, ref MapManager mapManager)
        {
            if (programArgs.Length < ARGS_COUNT)
            {
                //throw new GameException(EMessage.ARGS_COUNT);
            }

            try
            {
                //int.Parse(programArgs[0]);
                //int.Parse(programArgs[1]);
            }
            catch (Exception)
            {
                throw new GameException(EMessage.INVALID_ID);
            }

            mapManager = MapManager.Create("32", "3568");
            agent = RandomAgent.Create(mapManager);
        }

        private static readonly int ARGS_COUNT = 2;
    }
}
