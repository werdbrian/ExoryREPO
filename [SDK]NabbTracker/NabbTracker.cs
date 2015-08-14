// --------------------------------------------------------------------------------------------------------------------
// <copyright = "NabbHackeR - Exory @ LeagueSharp">
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// <summary>
//     The namespace.
// </summary>
namespace NabbTracker
{    
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK.Core;
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Events;
    using LeagueSharp.SDK.Core.Extensions;
    
    using Menu = LeagueSharp.SDK.Core.UI.IMenu.Menu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Core.Wrappers;
    
    using SharpDX;
    using SharpDX.Direct3D9;

    using Font = SharpDX.Direct3D9.Font;
    using Color = System.Drawing.Color;    
    
    
    /// <summary>
    ///     The main cleanser class.
    /// </summary>
    internal class NabbTracker
    {
        /// <summary>
        ///     Gets the Y coord. of the HPBar screen position.
        /// </summary>
        int X;
        
        /// <summary>
        ///     Gets the Y coord. of the HPBar screen position.
        /// </summary>
        int Y;

        /// <summary>
        ///     Gets the X coord. of the spell-tracker's screen position.
        /// </summary>
        int SpellLevelX;
        
        /// <summary>
        ///     Gets the Y coord. of the spell-tracker's screen position.
        /// </summary>
        int SpellLevelY;
        
        /// <summary>
        ///     Gets the X coord of the Summonerspell-Tracker screen position.
        /// </summary>
        int SummonerSpellX;
        
        /// <summary>
        ///     Gets the Y coord. of the summonerspell-tracker's screen position.
        /// </summary>
        int SummonerSpellY;
        
        /// <summary>
        ///     Gets the Summoner-Spell name.
        /// </summary>
        string GetSummonerSpellName;
        
        /// <summary>
        ///     Gets the player.
        /// </summary>
        public Obj_AI_Hero Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }
        
        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        public Menu Menu { get; set; }
        
        /// <summary>
        ///     The Text fcnt.
        /// </summary>
        Font DisplayTextFont = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Tahoma", 10));
        
        /// <summary>
        ///     The SpellLevel Text font.
        /// </summary>
        Font DisplayLevelFont = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Comic Sans", 14));
        
        /// <summary>
        ///     Gets the Spellslots
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
        ///     The menu.
        /// </summary>
        private void CreateMenu()
        {
            this.Menu = new Menu("Menu", "NabbTracker", true);
            
            //options
            Menu.Add(new MenuBool("display.allies", "Track Allies", true));
            Menu.Add(new MenuBool("display.enemies", "Track Enemies", true));
            Menu.Add(new MenuBool("display.spell_levels", "Track Spell levels", true));
            //
            
            this.Menu.Attach();
        }
        
        /// <summary>
        ///     The load functions.
        /// </summary>
        public void Load()
        {
            this.CreateMenu();
            Drawing.OnDraw += this.OnDraw;
        }

        /// <summary>
        ///     Called when the game draws itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnDraw(EventArgs args)
        {
            foreach (var TrackedChar in GameObjects.EnemyHeroes.Where(TrackedChar => (TrackedChar.IsValid)
                && (!TrackedChar.IsMe)
                && (TrackedChar.IsHPBarRendered)
                && ((TrackedChar.IsEnemy && this.Menu["display.enemies"].GetValue<MenuBool>().Value) 
                    || (TrackedChar.IsAlly && this.Menu["display.allies"].GetValue<MenuBool>().Value)))
                ){
                
                for (int Spell = 0; Spell < SpellSlots.Count(); Spell++){
                    X = (int)TrackedChar.HPBarPosition.X + 10 + (Spell * 22);
                    Y = (int)TrackedChar.HPBarPosition.Y + 32;
    
                    var GetSpell = TrackedChar.Spellbook.GetSpell(SpellSlots[Spell]);
                    var GetSpellCD = GetSpell.CooldownExpires - Game.Time;
                    var SpellCDString = string.Format("{0:0}", GetSpellCD);
                    var IsSpellNotLearned = TrackedChar.Spellbook.CanUseSpell(SpellSlots[Spell]) == SpellState.NotLearned;
                    var IsSpellReady = TrackedChar.Spellbook.CanUseSpell(SpellSlots[Spell]) == SpellState.Ready;
                    
                    DisplayTextFont.DrawText(
                        null,
                        GetSpellCD > 0 ?
                        SpellCDString : SpellSlots[Spell].ToString(),
                        
                        X,
                        Y,
                        
						// Grey color if the spell is not learned or not ready to use.
					    IsSpellNotLearned || !IsSpellReady ?
                        SharpDX.Color.Gray :
						
						// Blue color if the target has not enough mana to use the spell.
						GetSpell.ManaCost > TrackedChar.Mana ?
                        SharpDX.Color.Blue :

						// Red color if the Spell CD is <= 4 (almost up), else green.
						GetSpellCD > 0 && GetSpellCD <= 4 ? 
						SharpDX.Color.Red : SharpDX.Color.LightGreen
                    );
                    
                    if (this.Menu["display.spell_levels"].GetValue<MenuBool>().Value){
                    
                        for (int DrawSpellLevel = 0; DrawSpellLevel <= GetSpell.Level - 1; DrawSpellLevel++){
                            SpellLevelX = X + (DrawSpellLevel * 3);
                            SpellLevelY = Y + 5;
                            
                            DisplayLevelFont.DrawText(
                                null,
                                ".",
                                SpellLevelX,
                                SpellLevelY,
                                SharpDX.Color.White    
                            );
                        }
                    }
                }
                
                for (int SummonerSpell = 0; SummonerSpell < SummonerSpellSlots.Count(); SummonerSpell++)
                {
                    SummonerSpellX = (int)TrackedChar.HPBarPosition.X + 10 + (SummonerSpell * 88);
                    SummonerSpellY = (int)TrackedChar.HPBarPosition.Y - 2;
                    
                    var GetSummonerSpell = TrackedChar.Spellbook.GetSpell(SummonerSpellSlots[SummonerSpell]);
                    var GetSummonerSpellCD = GetSummonerSpell.CooldownExpires - Game.Time;
                    var SummonerSpellCDString = string.Format("{0:0}", GetSummonerSpellCD);
                    
                    switch (GetSummonerSpell.Name.ToLower())
                    {
                        case "summonerflash":           GetSummonerSpellName = "Flash";           break;
                        case "summonerdot":             GetSummonerSpellName = "Ignite";          break;
                        case "summonerheal":            GetSummonerSpellName = "Heal";            break;
                        case "summonerteleport":        GetSummonerSpellName = "Teleport";        break;
                        case "summonerexhaust":         GetSummonerSpellName = "Exhaust";         break;
                        case "summonerhaste":           GetSummonerSpellName = "Ghost";           break;
                        case "summonerbarrier":         GetSummonerSpellName = "Barrier";         break;
                        case "summonerboost":           GetSummonerSpellName = "Cleanse";         break;
                        case "summonermana":            GetSummonerSpellName = "Clarity";         break;
                        case "summonerclairvoyance":    GetSummonerSpellName = "Clairvoyance";    break;
                        case "summonerodingarrison":    GetSummonerSpellName = "Garrison";        break;
                        case "summonersnowball":        GetSummonerSpellName = "Mark";            break; // ARAM Only
                        // 
                        default:                        GetSummonerSpellName = "Smite";           break;
                    }
                    
                    DisplayTextFont.DrawText(
                        null,
                        GetSummonerSpellCD > 0 ? 
                        GetSummonerSpellName + ":" + SummonerSpellCDString : GetSummonerSpellName + ": UP ",
                        
                        SummonerSpellX,
                        SummonerSpellY,
                        
                        GetSummonerSpellCD > 0 ?
                        SharpDX.Color.Red : SharpDX.Color.Yellow
                    );
                }
            }
        }
    }
}