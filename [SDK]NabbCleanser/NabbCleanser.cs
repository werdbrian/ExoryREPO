// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoonDraven.cs" company="ChewyMoon">
//   Copyright (C) 2015 ChewyMoon
//   
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
namespace NabbCleanser
{    
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows.Forms;

    using LeagueSharp;
    using LeagueSharp.SDK.Core;
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Events;
    using LeagueSharp.SDK.Core.Extensions;
    
    using Menu = LeagueSharp.SDK.Core.UI.IMenu.Menu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Core.Wrappers;
    
    /// <summary>
    ///     The main cleanser class.
    /// </summary>
    internal class NabbCleanser
    {
        /// <summary>
        ///     Gets the cleanse spellslot.
        /// </summary>
        /// <value>
        ///     The cleanse spellslot.
        /// </value>
        private SpellSlot cleanse { get; set; }
        
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
        
        int QSS = (int)3140;
        int Dervish = (int)3137;
        int Mikaels = (int)3222;
        int Mercurial = (int)3139;

        /// <summary>
        ///     The menu.
        /// </summary>
        private void CreateMenu()
        {
            this.Menu = new Menu("Menu", "NabbCleanser", true);
            
            //options
            Menu.Add(new MenuBool("use.cleanse", "Use Cleanse.", true));
            Menu.Add(new MenuBool("use.cleansers", "Use Cleansers.", true));
            Menu.Add(new MenuBool("use.cleansevsignite", "Cleanse enemy Ignite.", true));
            Menu.Add(new MenuBool("panic_key_enable", "Only Cleanse when pressed button enable", false));
            Menu.Add(new MenuKeyBind("use.panic_key", "Only Cleanse when pressed button", Keys.Space, KeyBindType.Press));
            Menu.Add(new MenuBool("use.cleansers.second.priority", "Use Cleansers for second-priority ultimates", false));
            Menu.Add(new MenuSlider("use.delay", "Delay cleanse/cleansers usage by X ms.", 500, 0, 2000));
            //
            
            this.Menu.Attach();
        }
        
        /// <summary>
        ///     The load functions.
        /// </summary>
        public void Load()
        {
            this.CreateMenu();
            Game.OnUpdate += this.OnUpdate;
        }
        
        bool HasNoProtection()
        {
            // return "true" if the Player..
            return 
                
                //..has no SpellShield..
                !this.Player.HasBuffOfType(BuffType.SpellShield)
                
             //..nor SpellImmunity.  
             && !this.Player.HasBuffOfType(BuffType.SpellImmunity)
            ; 
        }
        
        bool ShouldUseCleanse()
        {
            // return "true" if the Player is being affected by..
            return (
                // ..Charms..
                this.Player.HasBuffOfType(BuffType.Charm)
                
             // ..or Fears..
             || this.Player.HasBuffOfType(BuffType.Flee)
             
             // ..or Polymorphs..
             || this.Player.HasBuffOfType(BuffType.Polymorph)
             
             // ..or Snares..
             || this.Player.HasBuffOfType(BuffType.Snare)
             
             // ..or Stuns..
             || this.Player.HasBuffOfType(BuffType.Stun)
             
             // ..or Suppressions..
             || this.Player.HasBuffOfType(BuffType.Suppression)
             
             // ..or Taunts..
             || this.Player.HasBuffOfType(BuffType.Taunt)
             
             // ..or Exhaust..
             || this.Player.HasBuff("summonerexhaust")
             
            )
             //..and, if he has no protection..
             && HasNoProtection()
            
             // ..and the relative option is enabled.
             && this.Menu["use.cleanse"].GetValue<MenuBool>().Value
            ; 
        }

        bool ShouldUseCleanser()
        {
            // return "true" if the Player is being affected by..
            return (
                // ..Zed's Target Mark (R)..
                this.Player.HasBuff("zedulttargetmark")

             // ..or Vladimir's Mark (R)..
             || this.Player.HasBuff("VladimirHemoplague")
             
             // ..or Mordekaiser's Mark (R)..
             || this.Player.HasBuff("MordekaiserChildrenOfTheGrave")
             
             // ..or Poppy's Immunity Mark (R)..
             || this.Player.HasBuff("PoppyDiplomaticImmunity")
             
             // ..or Fizz's Fish Mark (R)..
             || this.Player.HasBuff("FizzMarinerDoom")
             
             // ..or Malzahar's Ultimate..
             || this.Player.HasBuff("AlZaharNetherGrasp")
            )
             //..and, if he has no protection..
             && HasNoProtection()
            
             // ..and the relative option is enabled.
             && this.Menu["use.cleansers"].GetValue<MenuBool>().Value
            ; 
        }

        private void UseCleanser()
        {
            // if the player has QuickSilver Sash and is able to use it..
            if (Items.HasItem(QSS) && Items.CanUseItem(QSS))
            
                // ..JUST (DO)USE IT!
                Items.UseItem(QSS);
            
            // else if the player has Dervish Blade and is able to use it..
            else if (Items.HasItem(Dervish) && Items.CanUseItem(Dervish)) 
            
                // ..JUST (DO)USE IT!
                Items.UseItem(Dervish);
                
            // else if the player has Mikaels Crucible and is able to use it..
            else if (Items.HasItem(Mikaels) && Items.CanUseItem(Mikaels)) 
            
                // ..JUST (DO)USE IT!
                Items.UseItem(Mikaels);
                
            // else if the player has Mercurial Scimitar and is able to use it..
            else if (Items.HasItem(Mercurial) && Items.CanUseItem(Mercurial))
            
                // ..JUST (DO)USE IT!
                Items.UseItem(Mercurial)
            ;
        }
        
        bool ShouldUseSecondPriorityCleanser()
        {
            // return "true" if the Player has..
            return (
                // ..Twisted Fates vision mark (R)..
                this.Player.Buffs.Any(b => b.Name.Contains("Twisted"))
                
             // ..Nocturnes R (Fog part)..
             || this.Player.Buffs.Any(b => b.Name.Contains("Nocturne"))
            )
            
             //..and, if he has no protection..
             && HasNoProtection()
            
            // ..and the relative option is enabled.
            && this.Menu["use.cleansers.second.priority"].GetValue<MenuBool>().Value; 
        }
        
        bool CanAndShouldCleanseIfIgnited()
        {
            // return "true" if..
            return 
                // ..the player is ignited..
                this.Player.HasBuff("summonerdot")
                
            // ..and the relative option is enabled.
            && this.Menu["use.cleansevsignite"].GetValue<MenuBool>().Value;
        }
        
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            // If the only-cleanse-if-key-pressed option is enabled and the relative key is being pressed or the only-cleanse-if-key-pressed option is disabled..
            if ((this.Menu["panic_key_enable"].GetValue<MenuBool>().Value && this.Menu["use.panic_key"].GetValue<MenuKeyBind>().Active) || (!this.Menu["panic_key_enable"].GetValue<MenuBool>().Value)){
            
                cleanse = Player.GetSpellSlot("summonerboost");
                var CleanseDelay = this.Menu["use.delay"].GetValue<MenuSlider>().Value;
                
                // If you are being affected by movement-empairing or control-denying cctype or you are being affected by summoner Ignite..
                if (ShouldUseCleanse() || CanAndShouldCleanseIfIgnited())
                {
                    var IsCleanseReady = Player.Spellbook.CanUseSpell(cleanse) == SpellState.Ready;
                    
                    // If the player actually has the summonerspell Cleanse and it is ready to use..
                    if (cleanse != SpellSlot.Unknown && IsCleanseReady)
                    
                        // ..Use the normal delay.
                        DelayAction.Add(CleanseDelay, () => Player.Spellbook.CastSpell(cleanse, Player))
                    ;
                }
                
                // If the player is being affected by Hard CC or a Second-priority ult mark..
                if (ShouldUseCleanser() || ShouldUseSecondPriorityCleanser())
                {
                    var HasZedTargetMark = this.Player.HasBuff("zedulttargetmark");

                    // If the player is being affected by the DeathMark..
                    if (HasZedTargetMark)
                    
                        // ..Double the delay before cleansing..
                        DelayAction.Add(CleanseDelay*2, () => UseCleanser());
                        
                    //..else..
                    else
                    
                        // ..JUST (DO)CLEANSE IT!
                        DelayAction.Add(CleanseDelay, () => UseCleanser())
                    ;
                }
            }    
        }
    }
}

