﻿using System;
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
    }

    static class EMessage
    {
        static public string ARGS_COUNT = "Invalid arguments count.\nUse: <program> <GAMEID> <USERID>\n";
        static public string INVALID_ID = "Invalid id! Use id from mooped.net";
    }
}
