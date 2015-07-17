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
namespace NabbPotter
{    
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK.Core;
    using LeagueSharp.SDK.Core.Events;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Core.Wrappers;
    
    using Menu = LeagueSharp.SDK.Core.UI.IMenu.Menu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    
    /// <summary>
    ///     The main class.
    /// </summary>
    internal class NabbPotter
    {
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
        
        int HealthPot = (int)2003;
        int ManaPot = (int)2004;
        int Biscuit = (int)2010;
        int Flask = (int)2041;

        
        private void CreateMenu()
        {
            this.Menu = new Menu("Menu", "NabbPotter", true);
            
            //options
            Menu.Add(new MenuBool("use.hp_potion", "Use Health Potions", true));
            Menu.Add(new MenuBool("use.mana_potion", "Use Mana Potions", true));
            Menu.Add(new MenuBool("use.biscuit", "Use Biscuits", true));
            Menu.Add(new MenuBool("use.flask", "Use Flasks", true));
            Menu.Add(new MenuSlider("use.on_health_percent", "Use Health Pots if Health < x%", 50, 0, 100));
            Menu.Add(new MenuSlider("use.on_mana_percent", "Use Mana Pots if Mana < x%", 50, 0, 100));
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
        
        bool IsPotRunning()
        {
            return
                // Biscuit
                this.Player.HasBuff("ItemMiniRegenPotion")
                // Flask
             || this.Player.HasBuff("ItemCrystalFlask")
                // HealthPot
             || this.Player.HasBuff("RegenerationPotion")
                // ManaPot
             || this.Player.HasBuff("FlaskOfCrystalWater")
            ;
        }
        
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnUpdate(EventArgs args)
        {
            // don't use another potion if a potion is already being used.
            if (IsPotRunning()) return;
            
            // ..or the player is in base.
            if (this.Player.InFountain()) return;
            
            // If Health is lower than the config value..
            if (this.Player.HealthPercent <= Menu["use.on_health_percent"].GetValue<MenuSlider>().Value)
            {
                // and the player has an Health Potion...
                if (Items.HasItem(HealthPot))
                
                    // ..use it.
                    Items.UseItem(HealthPot);
                
                // but if the player has not Health Potions left..
                else
                    
                    // ..use the biscuit.
                    Items.UseItem(Biscuit)
                ;
            }
            
            // If Mana is lower than the config value..
            if (this.Player.ManaPercent <= this.Menu["use.on_mana_percent"].GetValue<MenuSlider>().Value)
            {
                // ..use the Mana Potion.
                Items.UseItem(ManaPot);
            }
            
            // If both Health and Mana are lower than the config value..
            if ((this.Player.HealthPercent <= this.Menu["use.on_health_percent"].GetValue<MenuSlider>().Value
             && this.Player.ManaPercent <= this.Menu["use.on_mana_percent"].GetValue<MenuSlider>().Value)
             
             // or the player Health is half the percent on the config (supposed to be very low, like 25%)..
             || this.Player.HealthPercent <= (this.Menu["use.on_health_percent"].GetValue<MenuSlider>().Value / 2)
             
             // or the player Mana is half the percent on the config (supposed to be very low, like 25%)..
             || this.Player.ManaPercent <= (this.Menu["use.on_mana_percent"].GetValue<MenuSlider>().Value / 2))
            {
                //..use the Flask.
                Items.UseItem(Flask);
            } 
        }
    }
}

