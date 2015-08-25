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
        ///     The Menu.
        /// </summary>
        Menu Menu;
        
        /// <summary>
        ///     The Text fcnt.
        /// </summary>
        Font DisplayTextFont = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Tahoma", 8));
        
        /// <summary>
        ///     The SpellLevel Text font.
        /// </summary>
        Font DisplayLevelFont = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Comic Sans", 14));
     
        /// <summary>
        ///     Gets the Summoner-Spell name.
        /// </summary>
        string GetSummonerSpellName;
        #endregion
        
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
        ///     The tracker.
        /// </summary>
        public Track()
        {
            (Menu = new Menu("NabbTracker", "NabbTracker", true)).AddToMainMenu();
            {
                Menu.AddItem(new MenuItem("display.allies", "Track Allies").SetValue(true));
                Menu.AddItem(new MenuItem("display.enemies", "Track Enemies").SetValue(true));
                Menu.AddItem(new MenuItem("display.spell_levels", "Track Spell levels").SetValue(true));
            }
            Menu.AddItem(new MenuItem("enable", "Enable").SetValue(true));
        
            Drawing.OnDraw += Drawing_OnDraw;
        }
    
        /// <summary>
        ///     Called when the game draws itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Item("enable").GetValue<bool>()) return;
            
            foreach (var PlayingCharacter in HeroManager.AllHeroes.Where(PlayingCharacter => (PlayingCharacter.IsValid)
                && (!PlayingCharacter.IsMe)
                && (PlayingCharacter != null)
                && (PlayingCharacter.IsHPBarRendered)
                && ((PlayingCharacter.IsEnemy && Menu.Item("display.enemies").GetValue<bool>()) 
                    || (PlayingCharacter.IsAlly && Menu.Item("display.allies").GetValue<bool>())))
                ){
                
                for (int Spell = 0; Spell < SpellSlots.Count(); Spell++){
                    X = (int)PlayingCharacter.HPBarPosition.X + 10 + (Spell * 25);
                    Y = (int)PlayingCharacter.HPBarPosition.Y + 35;
    
                    var GetSpell = PlayingCharacter.Spellbook.GetSpell(SpellSlots[Spell]);
                    var GetSpellCD = GetSpell.CooldownExpires - Game.Time;
                    var SpellCDString = string.Format("{0:0}", GetSpellCD);
                    var IsSpellNotLearned = PlayingCharacter.Spellbook.CanUseSpell(SpellSlots[Spell]) == SpellState.NotLearned;
                    var IsSpellSurpressed = PlayingCharacter.Spellbook.CanUseSpell(SpellSlots[Spell]) == SpellState.Surpressed;
                    var IsSpellNoMana = PlayingCharacter.Spellbook.CanUseSpell(SpellSlots[Spell]) == SpellState.NoMana;
                    
                    DisplayTextFont.DrawText(
                        null,
                        GetSpellCD > 0 ?
                        SpellCDString : SpellSlots[Spell].ToString(),
                        
                        X,
                        Y,
                        
                        // Show Grey color if the spell is not learned or not ready to use.
                        IsSpellNotLearned || IsSpellSurpressed ?
                        SharpDX.Color.Gray :
                        
                        // Blue color if the target has not enough mana to use the spell.
                        IsSpellNoMana ?
                        SharpDX.Color.Cyan :

                        // Red color if the Spell CD is <= 4 (almost up),
                        GetSpellCD > 0 && GetSpellCD <= 4 ? 
                        SharpDX.Color.Red : 
                        
                        // Yellow color if the Spell is on CD, else show Grenn
                        GetSpellCD > 0 ?
                        SharpDX.Color.Yellow : SharpDX.Color.LightGreen
                    );
                    
                    if (Menu.Item("display.spell_levels").GetValue<bool>()){
                    
                        for (int DrawSpellLevel = 0; DrawSpellLevel <= GetSpell.Level - 1; DrawSpellLevel++){
                            SpellLevelX = X + (DrawSpellLevel * 3) - 4;
                            SpellLevelY = Y;
                            
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
                    SummonerSpellX = (int)PlayingCharacter.HPBarPosition.X + 10 + (SummonerSpell * 88);
                    SummonerSpellY = (int)PlayingCharacter.HPBarPosition.Y + 4;
                    
                    var GetSummonerSpell = PlayingCharacter.Spellbook.GetSpell(SummonerSpellSlots[SummonerSpell]);
                    var GetSummonerSpellCD = GetSummonerSpell.CooldownExpires - Game.Time;
                    var SummonerSpellCDString = string.Format("{0:0}", GetSummonerSpellCD);
                    
                    switch (GetSummonerSpell.Name.ToLower())
                    {
                        case "summonerflash":    
                            GetSummonerSpellName = "Flash";
                            break;
                            
                        case "summonerdot":
                            GetSummonerSpellName = "Ignite";
                            break;
                        
                        case "summonerheal":
                            GetSummonerSpellName = "Heal";
                            break;
                        
                        case "summonerteleport":
                            GetSummonerSpellName = "Teleport";
                            break;
                        
                        case "summonerexhaust":
                            GetSummonerSpellName = "Exhaust";
                            break;
                        
                        case "summonerhaste":
                            GetSummonerSpellName = "Ghost";
                            break;
                        
                        case "summonerbarrier":
                            GetSummonerSpellName = "Barrier";
                            break;
                        
                        case "summonerboost":
                            GetSummonerSpellName = "Cleanse";
                            break;
                        
                        case "summonermana":
                            GetSummonerSpellName = "Clarity";
                            break;
                        
                        case "summonerclairvoyance":
                            GetSummonerSpellName = "Clairvoyance";
                            break;
                        
                        case "summonerodingarrison":
                            GetSummonerSpellName = "Garrison";
                            break;
                        
                        case "summonersnowball":
                            GetSummonerSpellName = "Mark";
                            break;
                        
                        default:
                            GetSummonerSpellName = "Smite";
                            break
                        ;
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
