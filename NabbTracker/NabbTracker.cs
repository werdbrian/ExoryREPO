namespace NabbTracker
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Drawing;
	using System.IO;

	using LeagueSharp;
	using LeagueSharp.Common;

	using SharpDX;
	using SharpDX.Direct3D9;

	using Font = SharpDX.Direct3D9.Font;
	using Color = System.Drawing.Color;
	
	/// <summary>
    ///     The main class.
    /// </summary>
    internal class Track
    {
		#region variables
		
		/// <summary>
		///     The X coord of the HPBar screen position.
		/// </summary>
		int x;
		
		/// <summary>
		///     The Y coord of the HPBar screen position.
		/// </summary>
		int y;
		
		/// <summary>
		///     The X coord of the Summonerspell-Tracker screen position.
		/// </summary>
		int sumx;
		
		/// <summary>
		///     The Y coord of the Summonerspell-Tracker screen position.
		/// </summary>
		int sumy;
		
		/// <summary>
		///     The X coord of the SpellLevel-Tracker screen position.
		/// </summary>
		int levelx;
		
		/// <summary>
		///     The Y coord of the SpellLevel-Tracker screen position.
		/// </summary>
		int levely;
		
		/// <summary>
		///     The Menu.
		/// </summary>
		Menu Menu;
		
		/// <summary>
		///     The Text fcnt.
		/// </summary>
		Font text;
		
		/// <summary>
		///     The SpellLevel Text font.
		/// </summary>
		Font level;
		
		/// <summary>
		///     The Player.
		/// </summary>
		public static Obj_AI_Hero Player = ObjectManager.Player;
		
		/// <summary>
		///     The Summoner-Spell name.
		/// </summary>
		string summoner;
		#endregion
		
		/// <summary>
		///     The SpellSlots
		/// </summary>
        private SpellSlot[]
            SpellSlots = {
				SpellSlot.Q,
				SpellSlot.W,
				SpellSlot.E,
				SpellSlot.R
			},
			
			SummonerSpellSlots = { 
				SpellSlot.Summoner1,
				SpellSlot.Summoner2
			}
		;

		/// <summary>
		///     The Tracker.
		/// </summary>
        public Track()
        {
			text = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Comic Sans", 10));
			level = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Comic Sans", 14));
	
			(Menu = new Menu("NabbTracker", "NabbTracker", true)).AddToMainMenu();
			{
				Menu.AddItem(new MenuItem("display.me", "Track Me").SetValue(true));
				Menu.AddItem(new MenuItem("display.allies", "Track Allies").SetValue(true));
				Menu.AddItem(new MenuItem("display.enemies", "Track Enemies").SetValue(true));
				Menu.AddItem(new MenuItem("display.spell_levels", "Track Spell levels").SetValue(true));
			}
			Menu.AddItem(new MenuItem("enable", "Enable").SetValue(true));
		
            Drawing.OnDraw += Drawing_OnDraw;
			Notifications.AddNotification("NabbTracker - Loaded", 3000);
        }

		// <summary>
        ///     Drawing_OnDraw event, specifies a drawing callback which is after IDirect3DDevice9::BeginScene and before
        ///     IDirect3DDevice9::EndScene.
        /// </summary>
        private void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Item("enable").GetValue<bool>()) return;
	
			foreach (var hero in HeroManager.AllHeroes.Where(hero => (hero.IsValid) 
				&& (!hero.IsMe || Menu.Item("display.me").GetValue<bool>())
				&& (hero.IsHPBarRendered)
				&& ((hero.IsEnemy && Menu.Item("display.enemies").GetValue<bool>()) || (hero.IsAlly && Menu.Item("display.allies").GetValue<bool>()))
				&& (hero != null))){
				
                for (int k = 0; k < SpellSlots.Count(); k++){
                    x = (int)hero.HPBarPosition.X + 10 + (k * 22);
                    y = (int)hero.HPBarPosition.Y + 32;
					
					levely = y + 5;
	
                    var spell = hero.Spellbook.GetSpell(SpellSlots[k]);
                    var t = spell.CooldownExpires - Game.Time;
                    var s = string.Format("{0:0}", t);
					
                    text.DrawText(
						null,
						(t > 0 && t < 100) ? s : SpellSlots[k].ToString(),
						x,
						y,
						(hero.Spellbook.CanUseSpell(SpellSlots[k]) == SpellState.NotLearned || (t > 0 && t < 100)) ? SharpDX.Color.Gray : SharpDX.Color.LightGreen
                    );
					
					if (Menu.Item("display.spell_levels").GetValue<bool>()){
                        for (int i = 0; i <= spell.Level - 1; i++){
							levelx = x + (i * 3);
							
                            level.DrawText(
                                null,
                                ".",
                                levelx,
                                levely,
                                SharpDX.Color.White
                            );
                        }
                    }
                }
				
				for (int m = 0; m < SummonerSpellSlots.Count(); m++){
					sumx = (int)hero.HPBarPosition.X + 10 + (m * 77);
					sumy = (int)hero.HPBarPosition.Y - 2;
					
					var summonerspell = hero.Spellbook.GetSpell(SummonerSpellSlots[m]);
					var t2 = summonerspell.CooldownExpires - Game.Time;
					var s = string.Format("{0:0}", t2);
					
					
                    switch (summonerspell.Name.ToLower())
                    {
                        case "summonerflash":        summoner = "Flash";    break;
                        case "summonerdot":          summoner = "Ignite";   break;
                        case "summonerheal":         summoner = "Heal";     break;
                        case "summonerteleport":     summoner = "Teleport"; break;
                        case "summonerexhaust":      summoner = "Exhaust";  break;
                        case "summonerhaste":        summoner = "Ghost";    break;
                        case "summonerbarrier":      summoner = "Barrier";  break;
                        case "summonerboost":        summoner = "Cleanse";  break;
                        case "summonermana":         summoner = "Clarity";  break;
                        case "summonerclairvoyance": summoner = "Clairity"; break;
                        case "summonerodingarrison": summoner = "Garrison"; break;
						//
                        default: 					 summoner = "Smite";   break;
                    }
					
					text.DrawText(
						null,
						(t2 > 0 && t2 < 400) ? summoner + ":" + s : summoner + ": UP",
						sumx,
						sumy,
						(t2 > 0 && t2 < 400) ? SharpDX.Color.Red : SharpDX.Color.Yellow
                    );
				}
            }
        }
    }
}

