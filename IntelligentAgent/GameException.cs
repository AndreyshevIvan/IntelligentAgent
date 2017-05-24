using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAgent
{
    class GameException : Exception
    {
        public GameException(string message)
            : base(message)
        {
        }

        public GameException(string source, string message)
            : base(source + " :" + message)
        {
        }

        public GameException(string source, string message, int code)
            : base(source + " :" + message + " Code: " + code.ToString())
        {
        }
    }

    static class EMessage
    {
        static public string ARGS_COUNT = "Invalid arguments count.\nUse: <program> <GAMEID> <USERID>\n";
        static public string INVALID_ID = "Invalid id! Use id from mooped.net";
        static public string CAVE_OVERFLOW = "Cave adress overflow when get from caves map";
        static public string OW_AGENT_WORLD_NOT_OPEN = "OpenWorldAgent can not work in close world.";
        static public string ERROR_PARSE_HASH = "ParseHash: Invalid hash.";
    }
}
