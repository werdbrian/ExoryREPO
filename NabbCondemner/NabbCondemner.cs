namespace NabbCondemner
{    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    using LeagueSharp;
    using LeagueSharp.Common;
    
    using SharpDX;
    using SharpDX.Direct3D9;
    
    /// <summary>
    ///     The main class.
    /// </summary>
    class Condemner
    {
        public static Menu Menu;
        
        public static Spell E;
        

        /// <summary>
        ///    Called when the Assembly is loaded.
        /// </summary>
        public static void OnLoad()
        {
            // ..Don't load the assembly if The player is not a Vayne.
            if (ObjectManager.Player.CharData.BaseSkinName != "Vayne") return;
            
            // Set the condemn.
            E = new Spell(SpellSlot.E, 550f);
            E.SetTargetted(0.25f, 2000f);
            
            // Load the menu.
            LoadMenu();
        }
        
        /// <summary>
        ///    The menu.
        /// </summary>
        private static void LoadMenu()        
        {
            (Menu = new Menu("NabbCondemn", "NabbCondemn", true)).AddToMainMenu();
            {
                Menu.AddItem(new MenuItem("use.key", "Condemn Keybind:").SetValue(new KeyBind(1, KeyBindType.Press)));
            }
            Menu.AddItem(new MenuItem("enable", "Enable").SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
        }
        
        /// <summary>
        ///    Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void Game_OnGameUpdate(EventArgs args)
        {
            // If the player is dead, return.
            if (ObjectManager.Player.IsDead) return;
            
            // If the keybind is pressed and the target is condemnable.
            if (E.IsReady() && Menu.Item("use.key").GetValue<KeyBind>().Active)
            {
                foreach (var target in HeroManager.Enemies.Where(hero => hero.IsValidTarget(E.Range)
                    && !hero.HasBuffOfType(BuffType.SpellShield)
                    && !hero.HasBuffOfType(BuffType.SpellImmunity)
                    && !hero.HasBuffOfType(BuffType.Invulnerability))){
                    
                    E.Cast(target);
                }    
            }
        }
    }
}

