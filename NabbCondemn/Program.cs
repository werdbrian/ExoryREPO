﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;

namespace NabbCondemn
{
    class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnLoad;
        }

        private static void Game_OnLoad(EventArgs args)
        {
            Condemn.OnLoad();
        }
    }
}