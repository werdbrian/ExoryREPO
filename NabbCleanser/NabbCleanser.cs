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
        
        bool HasNoProtection()
        {
            // return "true" if the Player..
            return 
                
                //..has no SpellShield..
                !ObjectManager.Player.HasBuffOfType(BuffType.SpellShield)
                
             //..nor SpellImmunity.  
             && !ObjectManager.Player.HasBuffOfType(BuffType.SpellImmunity)
            ; 
        }
        
        bool ShouldUseCleanse()
        {
            // return "true" if the Player is being affected by..
            return (
                // ..Charms..
                ObjectManager.Player.HasBuffOfType(BuffType.Charm)
                
             // ..or Fears..
             || ObjectManager.Player.HasBuffOfType(BuffType.Flee)
             
             // ..or Polymorphs..
             || ObjectManager.Player.HasBuffOfType(BuffType.Polymorph)
             
             // ..or Snares..
             || ObjectManager.Player.HasBuffOfType(BuffType.Snare)
             
             // ..or Stuns..
             || ObjectManager.Player.HasBuffOfType(BuffType.Stun)
             
             // ..or Suppressions..
             || ObjectManager.Player.HasBuffOfType(BuffType.Suppression)
             
             // ..or Taunts..
             || ObjectManager.Player.HasBuffOfType(BuffType.Taunt)
             
             // ..or Exhaust..
             || ObjectManager.Player.HasBuff("summonerexhaust")
             )
            
             //..and, if he has no protection..
             && HasNoProtection()
             
             // ..and the relative option is enabled.
             && Menu.Item("use.cleanse").GetValue<bool>()
            ; 
        }

        bool ShouldUseCleanser()
        {
            // return "true" if the Player is being affected by..
            return (
                // ..Zed's Target Mark (R)..
                ObjectManager.Player.HasBuff("zedulttargetmark")

             // ..or Vladimir's Mark (R)..
             || ObjectManager.Player.HasBuff("VladimirHemoplague")
             
             // ..or Mordekaiser's Mark (R)..
             || ObjectManager.Player.HasBuff("MordekaiserChildrenOfTheGrave")
             
             // ..or Poppy's Immunity Mark (R)..
             || ObjectManager.Player.HasBuff("PoppyDiplomaticImmunity")
             
             // ..or Fizz's Fish Mark (R)..
             || ObjectManager.Player.HasBuff("FizzMarinerDoom")
             
             // ..or Malzahar's Ultimate..
             || ObjectManager.Player.HasBuff("AlZaharNetherGrasp")
             )
             
             //..and, if he has no protection..
             && HasNoProtection()
             
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
            
             //..and, if he has no protection..
             && HasNoProtection()
            
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
                
             // ..or the player has an invulnerability source..
             && !ObjectManager.Player.HasBuffOfType(BuffType.Invulnerability)
                
             // ..and the relative option is enabled.
             && Menu.Item("use.cleansevsignite").GetValue<bool>()
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
				var IsCleanseReady = ObjectManager.Player.Spellbook.CanUseSpell(cleanse) == SpellState.Ready;
                
                // If you are being affected by movement-empairing or control-denying cctype or you are being affected by summoner Ignite..
                if (ShouldUseCleanse() || CanAndShouldCleanseIfIgnited())
                {
                    // If the player actually has the summonerspell Cleanse and it is ready to use..
                    if (cleanse != SpellSlot.Unknown && IsCleanseReady)
                    
                        // ..JUST (DO)CLEANSE IT!
                        ObjectManager.Player.Spellbook.CastSpell(cleanse, ObjectManager.Player)
                    ;
                }
                
                // If the player is being affected by Hard CC or a Second-priority ult mark..
                if (ShouldUseCleanser() /*|| ShouldUseSecondPriorityCleanser()*/)
                {
                    var HasZedTargetMark = ObjectManager.Player.HasBuff("zedulttargetmark");
                    
                    // If the player is being affected by the DeathMark..
                    if (HasZedTargetMark)
                        
                        // ..Cleanse it, but delay the action by 4 seconds.
                        Utility.DelayAction.Add(4000, () => UseCleanser());
                        
                    // ..else..
                    else
                    
                        // ..JUST (DO)CLEANSE IT!
                        UseCleanser()
                    ;
                }
				
				// If the player has not cleanse or cleanse is on cooldown and the player is being affected by hard CC..
				if ((cleanse == SpellSlot.Unknown || !IsCleanseReady) && ShouldUseCleanse())
				
					// ..JUST (DO)CLEANSE IT!
					UseCleanser()
				;
            }
        }
    }
}

