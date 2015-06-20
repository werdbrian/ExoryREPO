using System;

using LeagueSharp;
using LeagueSharp.Common;

namespace NabbCleanser
{
    class Program
    {
        public static Cleanse Cleanse;

        public static void Main(string[] args){

            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args){
            
            Cleanse = new Cleanse();
            Notifications.AddNotification("NabbCleanser - Loaded", 3000);
        }
    }
}