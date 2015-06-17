using System;

using LeagueSharp;
using LeagueSharp.Common;

namespace NabbPotter
{
	class Program
	{
		public static Pot Pot;

		public static void Main(string[] args){

			CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
		}

		private static void Game_OnGameLoad(EventArgs args){

			Pot = new Pot();
		}
	}
}