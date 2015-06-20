namespace NabbCleanser
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
    
    /// <summary>
    ///     The main class.
    /// </summary>
    internal class Cleanse
    {
        #region variables
        public static Menu Menu;
        private SpellSlot cleanse;
        
        int QSS = 3140;
        int Dervish = 3137;
        int Mikaels = 3222;
        int Mercurial = 3139;
        #endregion

        /// <summary>
        ///     The cleanser.
        /// </summary>
        public Cleanse()
        {
            (Menu = new Menu("NabbCleanser", "NabbCleanser", true)).AddToMainMenu();
            {
                Menu.AddItem(new MenuItem("use.cleanse", "Use Cleanse.").SetValue(true));
                Menu.AddItem(new MenuItem("use.cleansers", "Use Cleansers.").SetValue(true));
                Menu.AddItem(new MenuItem("use.info", "Cleansers = QSS, Dervish, Mercurial, Mikaels"));
            }
            Menu.AddItem(new MenuItem("enable", "Enable").SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
        }
        
        bool ShouldUseCleanse()
        {
            return ObjectManager.Player.HasBuffOfType(BuffType.Charm)
             || ObjectManager.Player.HasBuffOfType(BuffType.CombatDehancer)
             || ObjectManager.Player.HasBuffOfType(BuffType.Fear)
             || ObjectManager.Player.HasBuffOfType(BuffType.Knockup)
             || ObjectManager.Player.HasBuffOfType(BuffType.Polymorph)
             || ObjectManager.Player.HasBuffOfType(BuffType.Snare)
             || ObjectManager.Player.HasBuffOfType(BuffType.Stun)
             || ObjectManager.Player.HasBuffOfType(BuffType.Suppression)
             || ObjectManager.Player.HasBuffOfType(BuffType.Taunt)
            ; 
        }

        bool ShouldUseCleanser()
        {
            return ObjectManager.Player.HasBuff("FizzMarinerDoom")
             || ObjectManager.Player.HasBuff("zedulttargetmark")
             || ObjectManager.Player.HasBuff("VladimirHemoplague")
             || ObjectManager.Player.HasBuff("MordekaiserChildrenOfTheGrave")
            ; 
        }
        
        private void UseCleanser()
        {
            if (Items.HasItem(QSS)
             && Items.CanUseItem(QSS))
            
                Items.UseItem(QSS);
                
            else if (Items.HasItem(Dervish)
             && Items.CanUseItem(Dervish))
            
                Items.UseItem(Dervish);
            
            else if (Items.HasItem(Mikaels)
             && Items.CanUseItem(Mikaels))

                Items.UseItem(Mikaels);
            
            else if (Items.HasItem(Mercurial)
             && Items.CanUseItem(Mercurial))
            
                Items.UseItem(Mercurial)
            ;
        }
        
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!Menu.Item("enable").GetValue<bool>()) return;
            
            cleanse = ObjectManager.Player.GetSpellSlot("summonerboost");
            
            if (ShouldUseCleanse() && Menu.Item("use.cleanse").GetValue<bool>())
            {        
                if (cleanse != SpellSlot.Unknown 
                 && ObjectManager.Player.Spellbook.CanUseSpell(cleanse) == SpellState.Ready)
                
                    ObjectManager.Player.Spellbook.CastSpell(cleanse, ObjectManager.Player);
                else 
                    UseCleanser();
                ;
            }
            
            if (ShouldUseCleanser() && Menu.Item("use.cleansers").GetValue<bool>())
            {
                UseCleanser();
            }
        }
    }
}

