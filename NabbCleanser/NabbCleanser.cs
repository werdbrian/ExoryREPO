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
                Menu.AddItem(new MenuItem("use.cleansevsignite", "Cleanse enemy Ignite.").SetValue(true));
                Menu.AddItem(new MenuItem("use.info1", "Cleansers = QSS, Dervish, Mercurial, Mikaels"));
                Menu.AddItem(new MenuItem("use.separator1", ""));
                Menu.AddItem(new MenuItem("panic_key_enable", "Only Cleanse when pressed button enable").SetValue(true));
                Menu.AddItem(new MenuItem("use.panic_key", "Only Cleanse when pressed button").SetValue(new KeyBind(32, KeyBindType.Press)));
                /*
                Menu.AddItem(new MenuItem("use.separator2", ""));
                Menu.AddItem(new MenuItem("use.cleansers.second.priority", "Use Cleansers for second-priority ultimates").SetValue(false));
                Menu.AddItem(new MenuItem("use.info2", "Second Priority Utimates = Nocturne and Twisted Fate's Vision Marks."));
                */
            }
            Menu.AddItem(new MenuItem("enable", "Enable").SetValue(true));

            Game.OnUpdate += Game_OnGameUpdate;
        }
        
        bool ShouldUseCleanse()
        {
            return (
                ObjectManager.Player.HasBuffOfType(BuffType.Charm)
             || ObjectManager.Player.HasBuffOfType(BuffType.Fear)
             || ObjectManager.Player.HasBuffOfType(BuffType.Flee)
             || ObjectManager.Player.HasBuffOfType(BuffType.Polymorph)
             || ObjectManager.Player.HasBuffOfType(BuffType.Snare)
             || ObjectManager.Player.HasBuffOfType(BuffType.Stun)
             || ObjectManager.Player.HasBuffOfType(BuffType.Suppression)
             || ObjectManager.Player.HasBuffOfType(BuffType.Taunt)
            )
             && Menu.Item("use.cleanse").GetValue<bool>()
            ; 
        }

        bool ShouldUseCleanser()
        {
            // return "true" if the Player has..
            return (
                // ..Zeds Target Mark (R)..
                ObjectManager.Player.HasBuff("zedulttargetmark")
                
             // ..or Warwicks Suppression (R)..
             || ObjectManager.Player.HasBuff("InfiniteDuress")
             
             // ..or Vladimirs Mark (R)..
             || ObjectManager.Player.HasBuff("VladimirHemoplague")
             
             // ..or Mordekaisers Mark (R)..
             || ObjectManager.Player.HasBuff("MordekaiserChildrenOfTheGrave")
             
             // ..or Poppys Immunity Mark (R)..
             || ObjectManager.Player.HasBuff("PoppyDiplomaticImmunity")
             
             // ..or Fizz's Fish Mark (R)..
             || ObjectManager.Player.HasBuff("FizzMarinerDoom")
            )
             // ..and the relative option is enabled.
             && Menu.Item("use.cleansers").GetValue<bool>()
            ; 
        }
        
        /*
        bool ShouldUseSecondPriorityCleanser()
        {
            // return "true" if the Player has..
            return (
                // ..Twisted Fates vision mark (R)..
                ObjectManager.Player.HasBuff.StartsWith("Twisted")
                
             // ..Nocturnes R (Fog part)..
             || ObjectManager.Player.HasBuff.StartsWith("Nocturne")
            )
             // ..and the relative option is enabled.
             && Menu.Item("use.cleansers.second.priority").GetValue<bool>()
            ; 
        }
        */
        
        bool CanAndShouldCleanseIfIgnited()
        {
            // return "true" if..
            return 
                // ..the player is ignited..
                ObjectManager.Player.HasBuff("summonerdot")
                
             // ..and the relative option is enabled.
             && Menu.Item("use.cleansevsignite").GetValue<bool>()
            ;
        }
        
        private void UseCleanser()
        {
            // if the player has QSS and is able to use it..
            if (Items.HasItem(QSS) && Items.CanUseItem(QSS))
            
                // ..JUST (DO)USE IT!
                Items.UseItem(QSS);
            
            else if (Items.HasItem(Dervish) && Items.CanUseItem(Dervish)) 
            
                // ..JUST (DO)USE IT!
                Items.UseItem(Dervish);
                
            // else if the player has Mikaels and is able to use it..
            else if (Items.HasItem(Mikaels) && Items.CanUseItem(Mikaels)) 
            
                // ..JUST (DO)USE IT!
                Items.UseItem(Mikaels);
                
            // else if the player has Mercurial Blade and is able to use it..
            else if (Items.HasItem(Mercurial) && Items.CanUseItem(Mercurial))
            
                // ..JUST (DO)USE IT!
                Items.UseItem(Mercurial)
            ;
        }
        
        /// <summary>
        ///     Called when the game updates itself.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Game_OnGameUpdate(EventArgs args)
        {
            // Don't use the assembly if the relative option is not enabled.
            if (!Menu.Item("enable").GetValue<bool>()) return;
            
            // If the only-cleanse-if-key-pressed option is enabled and the relative key is being pressed or the only-cleanse-if-key-pressed option is disabled..
            if ((Menu.Item("panic_key_enable").GetValue<bool>() && Menu.Item("use.panic_key").GetValue<KeyBind>().Active) || (!Menu.Item("panic_key_enable").GetValue<bool>())){
            
                cleanse = ObjectManager.Player.GetSpellSlot("summonerboost");
                
                // If you are being affected by movement-empairing or control-denying cctype or you are being affected by summoner Ignite..
                if (ShouldUseCleanse() || CanAndShouldCleanseIfIgnited())
                {
                    var IsCleanseReady = ObjectManager.Player.Spellbook.CanUseSpell(cleanse) == SpellState.Ready;
                    var HasZedTargetMark = ObjectManager.Player.HasBuff("zedulttargetmark");
                    
                    // If the player actually has the summonerspell Cleanse and it is ready to use..
                    if (cleanse != SpellSlot.Unknown && IsCleanseReady)
                    
                        // If the player is being affected by the DeathMark..
                        if (HasZedTargetMark)
                            
                            // ..Cleanse it, but delay the action by 4 seconds.
                            Utility.DelayAction.Add(4000, () => ObjectManager.Player.Spellbook.CastSpell(cleanse, ObjectManager.Player));
                            
                        // ..else..    
                        else
                            // ..JUST (DO)CLEANSE IT!
                            ObjectManager.Player.Spellbook.CastSpell(cleanse, ObjectManager.Player)
                        ;
                        
                    // ..else..    
                    else
                        // ..try to use a cleanser item.
                        UseCleanser()
                    ;
                }
                
                // If the player is being affected by Hard CC or a Second-priority ult mark..
                if (ShouldUseCleanser() /*|| ShouldUseSecondPriorityCleanser()*/)
                {
                    // ..JUST (DO)CLEANSE IT!
                    UseCleanser();
                }
            }    
        }
    }
}

