using System;
using System.IO;

namespace IntelligentAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Agent agent = null;
                MapManager mapManager = null;

                InitGame(ref agent, ref mapManager);

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

        static void InitGame(ref Agent agent, ref MapManager mapManager)
        {
            StreamReader reader = new StreamReader("config.txt");
            int idGame = 0;
            int idUser = 0;

            try
            {
                idGame = int.Parse(reader.ReadLine());
                idUser = int.Parse(reader.ReadLine());
            }
            catch
            {
                throw new GameException(EMessage.INVALID_ID);
            }

            mapManager = MapManager.Create(idGame.ToString(), idUser.ToString());
            agent = StupidAgent.Create(mapManager);
        }
    }
}
