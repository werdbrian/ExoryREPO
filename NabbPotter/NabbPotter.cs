namespace NabbPotter
{    
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    
    /// <summary>
    ///     The main class.
    /// </summary>
    internal class Pot
    {
        #region variables
        /// <summary>
        ///     The Menu.
        /// </summary>
        Menu Menu;
        
        int HealthPot = (int)2003;
        int ManaPot = (int)2004;
        int Biscuit = (int)2010;
        int Flask = (int)2041;
        #endregion
        
        public Pot()
        {
            (Menu = new Menu("NabbPotter", "NabbPotter", true)).AddToMainMenu();
            {
                Menu.AddItem(new MenuItem("use.hp_potion", "Use Health Potions").SetValue(true));
                Menu.AddItem(new MenuItem("use.mana_potion", "Use Mana Potions").SetValue(true));
                Menu.AddItem(new MenuItem("use.biscuit", "Use Biscuits").SetValue(true));
                Menu.AddItem(new MenuItem("use.flask", "Use Flasks").SetValue(true));
                Menu.AddItem(new MenuItem("use.on_health_percent", "Use Health Pots if Health < x%").SetValue(new Slider(50, 0, 100)));
                Menu.AddItem(new MenuItem("use.on_mana_percent", "Use Mana Pots if Mana < x%").SetValue(new Slider(50, 0, 100)));
                
            }
            Menu.AddItem(new MenuItem("enable", "Enable").SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
        }
        
        bool IsPotRunning()
        {
            return
                // Biscuit
                ObjectManager.Player.HasBuff("ItemMiniRegenPotion")
                // Flask
             || ObjectManager.Player.HasBuff("ItemCrystalFlask")
                // HealthPot
             || ObjectManager.Player.HasBuff("RegenerationPotion")
                // ManaPot
             || ObjectManager.Player.HasBuff("FlaskOfCrystalWater")
            ;
        }
        
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Game_OnGameUpdate(EventArgs args)
        {
            // Don't use potions if not enabled..
            if (!Menu.Item("enable").GetValue<bool>()) return;
            
            // ..or a potion is already being used..
            if (IsPotRunning()) return;
            
            // ..or the player is in base.
            if (ObjectManager.Player.InFountain()) return;
            
            // If Health is lower than the config value..
            if (ObjectManager.Player.HealthPercent <= Menu.Item("use.on_health_percent").GetValue<Slider>().Value)
            {
                // and the player has an Health Potion...
                if (Items.HasItem(HealthPot))
                
                    // ..use it.
                    Items.UseItem(HealthPot);
                
                // but if the player has not Health Potions left..
                else
                    
                    // ..use the biscuit.
                    Items.UseItem(Biscuit);
                
            }
            
            // If Mana is lower than the config value..
            if (ObjectManager.Player.ManaPercent <= Menu.Item("use.on_mana_percent").GetValue<Slider>().Value)
            {
                // ..use the Mana Potion.
                Items.UseItem(ManaPot);
                Items.UseItem(Biscuit);
            }
            
            // If both Health and Mana are lower than the config value..
            if ((ObjectManager.Player.HealthPercent <= Menu.Item("use.on_health_percent").GetValue<Slider>().Value
             && ObjectManager.Player.ManaPercent <= Menu.Item("use.on_mana_percent").GetValue<Slider>().Value)
             
             // or the player Health is half the percent on the config (supposed to be very low, like 25%)..
             || ObjectManager.Player.HealthPercent <= (Menu.Item("use.on_health_percent").GetValue<Slider>().Value / 2)
             
             // or the player Mana is half the percent on the config (supposed to be very low, like 25%)..
             || ObjectManager.Player.ManaPercent <= (Menu.Item("use.on_mana_percent").GetValue<Slider>().Value / 2))
            {
                //..use the Flask.
                Items.UseItem(Flask);
            } 
        }
    }
}

