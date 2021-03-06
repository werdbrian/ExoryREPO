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

            BuildMikaelsMenu(Menu);
            
            Game.OnUpdate += Game_OnGameUpdate;
        }
        
		/// <summary>
        ///		Declares whenever the target unit has no protection.
        /// </summary>
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
        
		/// <summary>
        ///		Called when the player needs to use cleanse.
        /// </summary>
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
             
             // ..or Taunts..
             || ObjectManager.Player.HasBuffOfType(BuffType.Taunt)
             || ObjectManager.Player.HasBuffOfType(BuffType.Suppression)
             || ObjectManager.Player.HasBuffOfType(BuffType.Knockup)
             || ObjectManager.Player.HasBuffOfType(BuffType.Knockback)
             // ..or Exhaust..
             || ObjectManager.Player.HasBuff("summonerexhaust")
             )
            
             //..and, if he has no protection..
             && HasNoProtection()
             
             // ..and the relative option is enabled.
             && Menu.Item("use.cleanse").GetValue<bool>()
            ; 
        }

		/// <summary>
        ///		Called when the player needs to use a cleanser Item.
        /// </summary>
        bool ShouldUseCleanser()
        {
            // return "true" if the Player is being affected by..
            return (
                // ..Zed's Target Mark (R)..
                ObjectManager.Player.HasBuff("ZedR")

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
            
             // ..or Suppressions..
             || ObjectManager.Player.HasBuffOfType(BuffType.Suppression)
             )
             
             //..and, if he has no protection..
             && HasNoProtection()
             
             // ..and the relative option is enabled.
             && Menu.Item("use.cleansers").GetValue<bool>()
            ; 
        }
        
		/// <summary>
        ///		Called when the player needs to use Mikaels Crucible.
        /// </summary>
        bool ShouldUseMikaels(Obj_AI_Hero target)
        {
            // return "true" if the Player is being affected by..
            return (
                // ..Charms..
                target.HasBuffOfType(BuffType.Charm)
                
             // ..or Fears..
             || target.HasBuffOfType(BuffType.Flee)
             
             // ..or Polymorphs..
             || target.HasBuffOfType(BuffType.Polymorph)
             
             // ..or Snares..
             || target.HasBuffOfType(BuffType.Snare)
             
             // ..or Stuns..
             || target.HasBuffOfType(BuffType.Stun)
             
             // ..or Suppressions..
             || target.HasBuffOfType(BuffType.Suppression)
             
             // ..or Taunts..
             || target.HasBuffOfType(BuffType.Taunt)
             
             // ..or Exhaust..
             || target.HasBuff("summonerexhaust")
             
             // ..or Zed's Target Mark (R)..
             || target.HasBuff("ZedR")

             // ..or Vladimir's Mark (R)..
             || target.HasBuff("VladimirHemoplague")
             
             // ..or Mordekaiser's Mark (R)..
             || target.HasBuff("MordekaiserChildrenOfTheGrave")
             
             // ..or Poppy's Immunity Mark (R)..
             || target.HasBuff("PoppyDiplomaticImmunity")
             
             // ..or Fizz's Fish Mark (R)..
             || target.HasBuff("FizzMarinerDoom")
             
             // ..or Malzahar's Ultimate..
             || target.HasBuff("AlZaharNetherGrasp")
             )
             
             //..and, if he has no protection..
             && HasNoProtection()
             
             // ..and the relative option is enabled.
             && Menu.Item("enable.mikaels").GetValue<bool>()
            ; 
        }
        
		/// <summary>
        ///		Called when the player needs to cleanse enemy ignite.
        /// </summary>
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
        
		/// <summary>
        ///		Builds the Mikaels Menu.
        /// </summary>
		/// <param name="Menu">
        ///     The Menu
        /// </param>
        public void BuildMikaelsMenu(Menu Menu)
        {            
            var MikaelsMenu = new Menu("Mikaels Options", "use.mikaelsmenu");
            {
                foreach (var ally in ObjectManager.Get<Obj_AI_Hero>()
                    .Where(h => h.IsAlly)
                    .Select(hero => hero.ChampionName)
                    .ToList()){
                    
                    MikaelsMenu.AddItem(new MenuItem(string.Format("use.mikaels.{0}", ally.ToLowerInvariant()), ally).SetValue(true));
                }
            }
            MikaelsMenu.AddItem(new MenuItem("enable.mikaels", "Enable Mikaels Usage").SetValue(true));

            Menu.AddSubMenu(MikaelsMenu);
        }
		
        /// <summary>
        ///		Called when the player uses a cleanser.
        /// </summary>
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
            // Don't use the assembly if the player is dead.
            if (ObjectManager.Player.IsDead)
			{	
				return;
			}
			
            // Don't use the assembly if the relative option is not enabled.
            if (!Menu.Item("enable").GetValue<bool>())
			{
				return;
            }
			
            // If the only-cleanse-if-key-pressed option is enabled and the relative key is being pressed or the only-cleanse-if-key-pressed option is disabled..
            if ((Menu.Item("panic_key_enable").GetValue<bool>() && Menu.Item("use.panic_key").GetValue<KeyBind>().Active) 
				|| (!Menu.Item("panic_key_enable").GetValue<bool>()))
			{
                cleanse = ObjectManager.Player.GetSpellSlot("summonerboost");
                var IsCleanseReady = ObjectManager.Player.Spellbook.CanUseSpell(cleanse) == SpellState.Ready;

                // For each ally enabled on the menu-option..
                foreach (var ally in ObjectManager.Get<Obj_AI_Hero>()
                    .Where(h => h.IsAlly
                        && Menu.Item(string.Format("use.mikaels.{0}", h.ChampionName.ToLowerInvariant())).GetValue<bool>()
                        && ObjectManager.Player.CountAlliesInRange(500) > 0))
				{

                    // if the player has Mikaels and is able to use it..
                    if (Items.HasItem(Mikaels) && Items.CanUseItem(Mikaels))
                    {
                        // If the ally should be cleansed..
                        if (ShouldUseMikaels(ally))
                        {
                            // ..JUST (DO)CLEANSE HIM!
                            Items.UseItem(Mikaels, ally);
                        }
                    }
                }
                
                // If you are being affected by movement-empairing or control-denying cctype or you are being affected by summoner Ignite..
                if (ShouldUseCleanse() || CanAndShouldCleanseIfIgnited())
                {
                    // If the player actually has the summonerspell Cleanse and it is ready to use..
                    if (cleanse != SpellSlot.Unknown && IsCleanseReady)
                    {
                        // ..JUST (DO)CLEANSE IT!
                        ObjectManager.Player.Spellbook.CastSpell(cleanse, ObjectManager.Player);
                    }    
                }
                
                // If the player is being affected by Hard CC or a Second-priority ult mark..
                if (ShouldUseCleanser())
                {
                    var HasZedUltimate = ObjectManager.Player.HasBuff("ZedR");
					var HasSkarnerUltimate = ObjectManager.Player.HasBuff("SkarnerR");
                    
                    // If the player is being affected by the DeathMark..
                    if (HasZedUltimate)
                    { 
                        // ..Cleanse it, but delay the action by 4 seconds.
                        Utility.DelayAction.Add(3000, () => UseCleanser());
                        return;
                    }
					
					// If the player is being affected by Skarner's R..
					if (HasSkarnerUltimate){
					
						// ..Cleanse it, but delay the action by 1 seconds.
						Utility.DelayAction.Add(1000, () => UseCleanser());
						return;
					}

                    // if the player has Mikaels and is able to use it..
                    if (Items.HasItem(Mikaels) && Items.CanUseItem(Mikaels))
                    {
                        if (ShouldUseCleanser() && ObjectManager.Player.CountAlliesInRange(500) == 0)
                        {
                            Items.UseItem(Mikaels);
                            return;
                        }
                    }

                    // ..JUST (DO)CLEANSE IT!
                    UseCleanser();
                }
                
                // If the player has not cleanse or cleanse is on cooldown and the player is being affected by hard CC..
                if ((cleanse == SpellSlot.Unknown || !IsCleanseReady) && ShouldUseCleanse())
                {
                    // ..JUST (DO)CLEANSE IT!
                    UseCleanser();
                }
            }
        }
    }
}
