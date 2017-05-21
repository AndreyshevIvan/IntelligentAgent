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
                StreamWriter log = new StreamWriter("out_log.txt");
                log.WriteLine(e.Message);
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
            agent = CloseWorldAgent.Create(mapManager);
            //ChooseAgent(ref agent, ref mapManager);
        }
        static void ChooseAgent(ref Agent agent, ref MapManager mapManager)
        {
            while (true)
            {
                string command = Console.ReadLine();

                if (command == "1")
                {

                }
                else if (command == "2")
                {

                }
                else if (command == "3")
                {

                }
                else if (command == "help")
                {

                }
                else
                {

                }
            }

        }
        static void WriteMenu(int idGame, int idUser)
        {
            Console.WriteLine("Game id: ", idGame, ", user id: ", idUser, ".");
            Console.WriteLine("Choose agent to mission:");
            Console.WriteLine("1 - Random agent");
            Console.WriteLine("2 - OpenWorld agent");
            Console.WriteLine("3 - CloseWorld agent");
        }
    }
}
