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
                RequestManager requestManager = null;

                InitGame(args, randomAgent, requestManager);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void InitGame(string[] programArgs, RandomAgent agent, RequestManager netManager)
        {
            if (programArgs.Length < ARGS_COUNT)
            {
                throw new GameException(EMessage.ARGS_COUNT);
            }

            int idGame = int.Parse(programArgs[0]);
            int idUser = int.Parse(programArgs[1]);

            netManager = RequestManager.Create(idGame, idUser);
            agent = Agent.CreateRandom();
        }

        private static readonly int ARGS_COUNT = 2;
    }
}
