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
                StupidAgent agent = null;
                MapManager mapManager = null;

                InitGame(args, ref agent, ref mapManager);

                while (mapManager.UpdateMap())
                {
                    agent.DoMove();
                }

                Console.WriteLine(mapManager.endLog);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void InitGame(string[] programArgs, ref StupidAgent agent, ref MapManager mapManager)
        {
            if (programArgs.Length < ARGS_COUNT)
            {
                // Commented only for debug
                //throw new GameException(EMessage.ARGS_COUNT);
            }

            try
            {
                // Commented only for debug
                //int.Parse(programArgs[0]);
                //int.Parse(programArgs[1]);
            }
            catch (Exception)
            {
                throw new GameException(EMessage.INVALID_ID);
            }

            // Use args[0] and args[1] as arguments if start with arguments
            mapManager = MapManager.Create("32", "3568");
            agent = StupidAgent.Create(mapManager);
        }

        private static readonly int ARGS_COUNT = 2;
    }
}
