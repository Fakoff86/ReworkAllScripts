
namespace EarthSpirit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
	using System.Threading.Tasks;
	internal class EarthSpirit
	{
		private static readonly Menu menu = new Menu("EarthSpirit", "EarthSpirit", true, "npc_dota_hero_earth_spirit", true);
		
		private static bool loaded;

		private static bool Active, qKey, wKey, eKey;
		private static bool AutoUlt;

		static void Main(string[] args)
		{
			Game.OnUpdate += Game_OnUpdate;

			Print.LogMessage.Success("This beginning marks their end!");
			Print.ConsoleMessage.Success("> |EarthSpirit| This beginning marks their end!");

			menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			menu.AddItem(new MenuItem("qKey", "Q Spell").SetValue(new KeyBind('Q', KeyBindType.Press)));
			menu.AddItem(new MenuItem("wKey", "W Spell").SetValue(new KeyBind('W', KeyBindType.Press)));
			menu.AddItem(new MenuItem("eKey", "E Spell").SetValue(new KeyBind('E', KeyBindType.Press)));

			var Skills = new Dictionary<string, bool>
			{
				{ "earth_spirit_magnetize", true}
			};
			var Items = new Dictionary<string, bool>
			{
				{"item_ethereal_blade", true},
				{"item_blink", true},
				{"item_heavens_halberd", true},
				{"item_orchid", true},
				{"item_urn_of_shadows", true},
				{"item_veil_of_discord", true},
				{"item_abyssal_blade", true},
				{"item_bloodthorn", true},
				{"item_blade_mail", true},
				{"item_black_king_bar", true},
				{"item_medallion_of_courage", true},
				{"item_solar_crest", true}
			};
			var Item = new Dictionary<string, bool>
			{
				{"item_shivas_guard", true},
				{"item_mask_of_madness", true},
				{"item_sheepstick", true},
				{"item_cheese", true},
				{"item_ghost", true},
				{"item_rod_of_atos", true},
				{"item_soul_ring", true},
				{"item_arcane_boots", true},
				{"item_magic_stick", true},
				{"item_magic_wand", true},
				{"item_mjollnir", true},
				{"item_satanic", true}
			};
			menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(Skills)));
			menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(Items)));
			menu.AddItem(
				new MenuItem("Item", "Items:").SetValue(new AbilityToggler(Item)));
			menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
			menu.AddItem(new MenuItem("oneult", "Use AutoUpdate Ultimate Remnant").SetValue(true));
			menu.AddToMainMenu();
		}

		private static Unit GetClosestToRemmnant(List<Unit> units, Hero x)
		{
			Unit closestHero = null;
			foreach (var b in units.Where(v => closestHero == null || closestHero.Distance2D(x) > v.Distance2D(x)))
			{
				closestHero = b;
			}
			return closestHero;
		}
		private static Ability Q, W, E, F, R, D;
		private static Hero e, me;
		private static Item urn,
			dagon,
			ghost,
			soulring,
			atos,
			vail,
			sheep,
			cheese,
			stick,
			arcane,
			halberd,
			mjollnir,
			ethereal,
			orchid,
			abyssal,
			mom,
			Shiva,
			mail,
			bkb,
			satanic,
			medall,
			blink;
		//cyclone

		public static void Game_OnUpdate(EventArgs args)
		{
			me = ObjectManager.LocalHero;


			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_EarthSpirit || Game.IsWatchingGame)
			{
				return;
			}

			Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key);
			qKey = Game.IsKeyDown(menu.Item("qKey").GetValue<KeyBind>().Key);
			wKey = Game.IsKeyDown(menu.Item("wKey").GetValue<KeyBind>().Key);
			eKey = Game.IsKeyDown(menu.Item("eKey").GetValue<KeyBind>().Key);
			AutoUlt = menu.Item("oneult").IsActive();
			if (!menu.Item("enabled").IsActive())
				return;

			e = me.ClosestToMouseTarget(1800);
			if (e == null) return;

			D = me.FindSpell("earth_spirit_stone_caller");
			Q = me.FindSpell("earth_spirit_boulder_smash");
			E = me.Spellbook.SpellE;
			W = me.FindSpell("earth_spirit_rolling_boulder");
			F = me.FindSpell("earth_spirit_petrify");
			R = me.FindSpell("earth_spirit_magnetize");


			ethereal = me.FindItem("item_ethereal_blade");
			mom = me.FindItem("item_mask_of_madness");
			urn = me.FindItem("item_urn_of_shadows");
			dagon =
				me.Inventory.Items.FirstOrDefault(
					item =>
						item.Name.Contains("item_dagon"));
			halberd = me.FindItem("item_heavens_halberd");
			mjollnir = me.FindItem("item_mjollnir");
			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
			abyssal = me.FindItem("item_abyssal_blade");
			mail = me.FindItem("item_blade_mail");
			bkb = me.FindItem("item_black_king_bar");
			satanic = me.FindItem("item_satanic");
			blink = me.FindItem("item_blink");
			medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
			sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
			vail = me.FindItem("item_veil_of_discord");
			cheese = me.FindItem("item_cheese");
			ghost = me.FindItem("item_ghost");
			atos = me.FindItem("item_rod_of_atos");
			soulring = me.FindItem("item_soul_ring");
			arcane = me.FindItem("item_arcane_boots");
			stick = me.FindItem("item_magic_stick") ?? me.FindItem("item_magic_wand");
			Shiva = me.FindItem("item_shivas_guard");


			var ModifEther = e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");
			var stoneModif = e.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
			
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();
			var ModifInv = me.IsInvisible();
			//Adding buff 'modifier_earth_spirit_boulder_smash' index: 280 to 'npc_dota_earth_spirit_stone'.
			//Adding buff 'modifier_stunned' index: 280 to 'npc_dota_hero_axe'.

			//Adding buff 'modifier_earth_spirit_geomagnetic_grip' index: 280 to 'npc_dota_earth_spirit_stone'.
			//Adding buff 'modifier_earth_spirit_geomagnetic_grip_debuff' index: 0 to 'npc_dota_hero_axe'.


			//Adding buff 'modifier_earth_spirit_stone_thinker' index: 1722489760 to 'npc_dota_earth_spirit_stone'.
			if (qKey && me.Distance2D(e) <= 1400 && e != null && e.IsAlive && !ModifInv)
			{

				var remnant = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_earth_spirit_stone").ToList();
				var remnantCount = remnant.Count;

				var Remmnant = GetClosestToRemmnant(remnant, me);
				var RemmnantEnem = GetClosestToRemmnant(remnant, e);
				if (remnant.Count == 0)
				{
					if (
					D.CanBeCasted()
					&& Q.CanBeCasted()
					&& ((!blink.CanBeCasted() || blink == null || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
					|| (blink != null && me.Distance2D(e) <= 450 && blink.CanBeCasted()))
					)
					{
						if (me.Distance2D(e) <= E.CastRange - 50
							&& Utils.SleepCheck("Rem"))
						{
							if (me.NetworkActivity == NetworkActivity.Move)
								me.Stop();
							else
								D.UseAbility(Prediction.InFront(me, 100));
							Utils.Sleep(1000, "Rem");
						}
					}
				}
				for (int i = 0; i < remnantCount; ++i)
				{
					if (
						D.CanBeCasted()
						&& (Q.CanBeCasted())
						   && (me.Distance2D(Remmnant) >= 350
						   && !remnant[i].HasModifier("modifier_earth_spirit_boulder_smash")
						   && !remnant[i].HasModifier("modifier_earth_spirit_geomagnetic_grip"))
						   && ((!blink.CanBeCasted() || blink == null || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
						   || (blink != null && me.Distance2D(e) <= 450 && blink.CanBeCasted()))
						   )
					{
						if (me.Distance2D(e) <= E.CastRange - 50
							&& Utils.SleepCheck("Rem"))
						{
							if (me.NetworkActivity == NetworkActivity.Move)
								me.Stop();
							else
								D.UseAbility(Prediction.InFront(me, 100));
							Utils.Sleep(1000, "Rem");
						}
					}
					if (remnant.Count > 0)
					{

						if (//Q Skill
						   remnant != null
						   && Q.CanBeCasted()
						   && me.CanCast()
						   && me.Distance2D(e) <= E.CastRange - 50
						   && Remmnant.Distance2D(me) <= 350
						   && Utils.SleepCheck(Remmnant.Handle.ToString() + "remnantQ")
						   )
						{
							Q.CastSkillShot(e);
							Utils.Sleep(250, Remmnant.Handle.ToString() + "remnantQ");
						}
					}
				}
			}
			if (wKey)
			{
				var Wmod = me.HasModifier("modifier_earth_spirit_rolling_boulder_caster");
				var remnant = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_earth_spirit_stone").ToList();
				var remnantCount = remnant.Count;

				var Remmnant = GetClosestToRemmnant(remnant, me);
				if (//Q Skill
					remnant != null
					&& W.CanBeCasted()
					&& me.Distance2D(e) <= W.CastRange - 200
					&& Utils.SleepCheck(me.Handle.ToString() + "remnantW")
					)
				{
					W.CastSkillShot(e);
					Utils.Sleep(250, me.Handle.ToString() + "remnantW");
				}
				if (remnant.Count == 0)
				{
					var delay = Task.Delay(350).ContinueWith(_ =>
					{
						if (
							D.CanBeCasted()
							&& Wmod
							&& me.Distance2D(e) >= 600
							&& Utils.SleepCheck("nextAction")
							)
						{
							D.UseAbility(Prediction.InFront(me, 170));
							Utils.Sleep(1800 + D.FindCastPoint(), "nextAction");
						}
					});
				}
				var delay2 = Task.Delay(350).ContinueWith(_ =>
				{
					for (int i = 0; i < remnantCount; ++i)
					{
						if (
							D.CanBeCasted()
							&& Wmod
							&& me.Distance2D(e) >= 600
							&& me.Distance2D(Remmnant) >= 270
							&& Utils.SleepCheck("nextAction")
							)
						{
							D.UseAbility(Prediction.InFront(me, 170));
							Utils.Sleep(1800 + D.FindCastPoint(), "nextAction");
						}
					}
				});
			}
			if (eKey && me.Distance2D(e) <= 1400 && e != null && e.IsAlive && !ModifInv)
			{

				var remnant = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_earth_spirit_stone").ToList();
				var remnantCount = remnant.Count;

				var Remmnant = GetClosestToRemmnant(remnant, me);
				var RemmnantEnem = GetClosestToRemmnant(remnant, e);
				if (remnant.Count == 0)
				{
					if (
					D.CanBeCasted()
					&& E.CanBeCasted()
					)
					{
						if (me.Distance2D(e) <= E.CastRange - 50
							&& Utils.SleepCheck("Rem"))
						{
							if (me.NetworkActivity == NetworkActivity.Move)
								me.Stop();
							else
								D.UseAbility(e.Position);
							Utils.Sleep(1000, "Rem");
						}
					}
				}
				for (int i = 0; i < remnantCount; ++i)
				{
					if (
						D.CanBeCasted()
						&& (E.CanBeCasted())
						   && (e.Distance2D(RemmnantEnem) >= 300
						   && !RemmnantEnem.HasModifier("modifier_earth_spirit_boulder_smash")
						   && !RemmnantEnem.HasModifier("modifier_earth_spirit_geomagnetic_grip"))
						   )
					{
						if (me.Distance2D(e) <= E.CastRange - 50
							&& Utils.SleepCheck("Rem"))
						{
							if (me.NetworkActivity == NetworkActivity.Move)
								me.Stop();
							else
								D.UseAbility(e.Position);
							Utils.Sleep(1000, "Rem");
						}
					}
					if (remnant.Count > 0)
					{


						if (//Q Skill
						   remnant != null
						   && E.CanBeCasted()
						   && remnant[i].HasModifier("modifier_earth_spirit_boulder_smash")
						   && me.Distance2D(e) <= E.CastRange
						   && e.Distance2D(remnant[i]) <= 240
						   && me.CanCast()
						   && Utils.SleepCheck(remnant[i].Handle.ToString() + "remnantE")
						   )
						{
							E.UseAbility(remnant[i].Position);
							Utils.Sleep(220, remnant[i].Handle.ToString() + "remnantE");
						}
						else
						if (//Q Skill
						   remnant != null
						   && E.CanBeCasted()
						   && !remnant[i].HasModifier("modifier_earth_spirit_boulder_smash")
						   && me.Distance2D(e) <= E.CastRange
						   && e.Distance2D(remnant[i]) <= 150
						   && me.CanCast()
						   && Utils.SleepCheck(remnant[i].Handle.ToString() + "remnantE")
						   )
						{
							E.UseAbility(remnant[i].Position);
							Utils.Sleep(220, remnant[i].Handle.ToString() + "remnantE");
						}
						else if (//Q Skill
						  remnant != null
						  && E.CanBeCasted()
						  && RemmnantEnem.HasModifier("modifier_earth_spirit_boulder_smash")
						  && me.Distance2D(e) <= 200
						  && e.Distance2D(RemmnantEnem) >= 50
						  && me.Distance2D(RemmnantEnem) > me.Distance2D(e)
						  && me.CanCast()
						  && Utils.SleepCheck(RemmnantEnem.Handle.ToString() + "remnantE")
						  )
						{
							E.UseAbility(RemmnantEnem.Position);
							Utils.Sleep(220, RemmnantEnem.Handle.ToString() + "remnantE");
						}
					}
				}
			}
			if (Active && me.Distance2D(e) <= 1400 && e != null && e.IsAlive && !ModifInv)
			{

				var remnant = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_earth_spirit_stone").ToList();
				var remnantCount = remnant.Count;

				var Remmnant = GetClosestToRemmnant(remnant, me);
				var RemmnantEnem = GetClosestToRemmnant(remnant, e);
				if (remnant.Count == 0)
				{
					if (
					D.CanBeCasted()
					&& Q.CanBeCasted()
					&& ((!blink.CanBeCasted() || blink == null || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
					|| (blink != null && me.Distance2D(e) <= 450 && blink.CanBeCasted()))
					)
					{
						if (me.Distance2D(e) <= E.CastRange - 50
							&& Utils.SleepCheck("Rem"))
						{
							if (me.NetworkActivity == NetworkActivity.Move)
								me.Stop();
							else
								D.UseAbility(Prediction.InFront(me, 100));
							Utils.Sleep(1000, "Rem");
						}
					}
				}
				for (int i = 0; i < remnantCount; ++i)
				{
					if (
						D.CanBeCasted()
						&& (Q.CanBeCasted() || W.CanBeCasted())
						   && (me.Distance2D(Remmnant) >= 350
						   && !remnant[i].HasModifier("modifier_earth_spirit_boulder_smash")
						   && !remnant[i].HasModifier("modifier_earth_spirit_geomagnetic_grip"))
						   && ((!blink.CanBeCasted() || blink == null || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
						   || (blink != null && me.Distance2D(e) <= 450 && blink.CanBeCasted()))
						   )
					{
						if (me.Distance2D(e) <= E.CastRange - 50
							&& Utils.SleepCheck("Rem"))
						{
							if (me.NetworkActivity == NetworkActivity.Move)
								me.Stop();
							else
								D.UseAbility(Prediction.InFront(me, 100));
							Utils.Sleep(1000, "Rem");
						}
					}
					if (remnant.Count > 0)
					{

						if (//Q Skill
						   remnant != null
						   && Q.CanBeCasted()
						   && me.CanCast()
						   && me.Distance2D(e) <= E.CastRange - 50
						   && Remmnant.Distance2D(me) <= 350
						   && Utils.SleepCheck(Remmnant.Handle.ToString() + "remnantQ")
						   )
						{
							Q.CastSkillShot(e);
							Utils.Sleep(250, Remmnant.Handle.ToString() + "remnantQ");
						}
						else
						if (//Q Skill
						   remnant != null
						   && W.CanBeCasted()
						   && !Q.CanBeCasted()
						   && me.Distance2D(e) <= E.CastRange
						   && Utils.SleepCheck(remnant[i].Handle.ToString() + "remnantW")
						   )
						{
							W.CastSkillShot(e);
							Utils.Sleep(250, remnant[i].Handle.ToString() + "remnantW");
						}
						else if (//Q Skill
						   remnant != null
						   && !Q.CanBeCasted()
						   && !E.CanBeCasted()
						   && !remnant[i].HasModifier("modifier_earth_spirit_boulder_smash")
						   && !remnant[i].HasModifier("modifier_earth_spirit_geomagnetic_grip")
						   && W.CanBeCasted()
						   && me.Distance2D(e) <= E.CastRange - 50
						   && me.CanCast()
						   && Utils.SleepCheck(remnant[i].Handle.ToString() + "remnantW")
						   )
						{
							W.CastSkillShot(e);
							Utils.Sleep(250, remnant[i].Handle.ToString() + "remnantW");
						}
						if (//Q Skill
						   remnant != null
						   && E.CanBeCasted()
						   && remnant[i].HasModifier("modifier_earth_spirit_boulder_smash")
						   && me.Distance2D(e) <= E.CastRange
						   && e.Distance2D(remnant[i]) <= 250
						   && me.CanCast()
						   && Utils.SleepCheck(remnant[i].Handle.ToString() + "remnantE")
						   )
						{
							E.UseAbility(remnant[i].Position);
							Utils.Sleep(220, remnant[i].Handle.ToString() + "remnantE");
						}
						else
						if (//Q Skill
						   remnant != null
						   && E.CanBeCasted()
						   && !remnant[i].HasModifier("modifier_earth_spirit_boulder_smash")
						   && me.Distance2D(e) <= E.CastRange
						   && e.Distance2D(remnant[i]) <= 150
						   && me.CanCast()
						   && Utils.SleepCheck(remnant[i].Handle.ToString() + "remnantE")
						   )
						{
							E.UseAbility(remnant[i].Position);
							Utils.Sleep(220, remnant[i].Handle.ToString() + "remnantE");
						}
						if (//Q Skill
						   remnant != null
						   && E.CanBeCasted()
						   && RemmnantEnem.HasModifier("modifier_earth_spirit_boulder_smash")
						   && me.Distance2D(e) <= 300
						   && e.Distance2D(RemmnantEnem) >= 50
						   && me.Distance2D(RemmnantEnem) > me.Distance2D(e)
						   && me.CanCast()
						   && Utils.SleepCheck(RemmnantEnem.Handle.ToString() + "remnantE")
						   )
						{
							E.UseAbility(RemmnantEnem.Position);
							Utils.Sleep(220, RemmnantEnem.Handle.ToString() + "remnantE");
						}
					}
				}
				var magnetizemod = e.Modifiers.FirstOrDefault(x => x.Name == "modifier_earth_spirit_magnetize");
				if (magnetizemod != null && AutoUlt && magnetizemod.RemainingTime <= 0.5 + Game.Ping && me.Distance2D(e) <= D.CastRange && Utils.SleepCheck("Rem"))
				{
					D.UseAbility(e.Position);
					Utils.Sleep(1000, "Rem");
				}
				var charge = me.Modifiers.FirstOrDefault(y => y.Name == "modifier_earth_spirit_stone_caller_charge_counter");
				if (//W Skill
				   W != null
				   && charge.StackCount == 0
				   && W.CanBeCasted()
				   && me.Distance2D(e) <= 800
				   && me.CanCast()
				   && Utils.SleepCheck(me.Handle.ToString() + "remnantW")
				   )
				{
					W.CastSkillShot(e);
					Utils.Sleep(250, me.Handle.ToString() + "remnantW");
				}
				if ( // MOM
					mom != null
					&& mom.CanBeCasted()
					&& me.CanCast()
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mom.Name)
					&& Utils.SleepCheck("mom")
					&& me.Distance2D(e) <= 700
					)
				{
					mom.UseAbility();
					Utils.Sleep(250, "mom");
				}
				if ( // Hellbard
					halberd != null
					&& halberd.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& (e.NetworkActivity == NetworkActivity.Attack
						|| e.NetworkActivity == NetworkActivity.Crit
						|| e.NetworkActivity == NetworkActivity.Attack2)
					&& Utils.SleepCheck("halberd")
					&& me.Distance2D(e) <= 700
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(halberd.Name)
					)
				{
					halberd.UseAbility(e);
					Utils.Sleep(250, "halberd");
				}
				if ( //Ghost
					ghost != null
					&& ghost.CanBeCasted()
					&& me.CanCast()
					&& ((me.Position.Distance2D(e) < 300
						 && me.Health <= (me.MaximumHealth * 0.7))
						|| me.Health <= (me.MaximumHealth * 0.3))
					&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
					&& Utils.SleepCheck("Ghost"))
				{
					ghost.UseAbility();
					Utils.Sleep(250, "Ghost");
				}
				if ( // Arcane Boots Item
					arcane != null
					&& me.Mana <= W.ManaCost
					&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
					&& arcane.CanBeCasted()
					&& Utils.SleepCheck("arcane")
					)
				{
					arcane.UseAbility();
					Utils.Sleep(250, "arcane");
				} // Arcane Boots Item end
				if ( // Mjollnir
					mjollnir != null
					&& mjollnir.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
					&& Utils.SleepCheck("mjollnir")
					&& me.Distance2D(e) <= 900
					)
				{
					mjollnir.UseAbility(me);
					Utils.Sleep(250, "mjollnir");
				} // Mjollnir Item end
				if (
					// cheese
					cheese != null
					&& cheese.CanBeCasted()
					&& me.Health <= (me.MaximumHealth * 0.3)
					&& me.Distance2D(e) <= 700
					&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
					&& Utils.SleepCheck("cheese")
					)
				{
					cheese.UseAbility();
					Utils.Sleep(200, "cheese");
				} // cheese Item end
				if ( // Medall
					medall != null
					&& medall.CanBeCasted()
					&& Utils.SleepCheck("Medall")
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medall.Name)
					&& me.Distance2D(e) <= 700
					)
				{
					medall.UseAbility(e);
					Utils.Sleep(250, "Medall");
				} // Medall Item end
				if ( //R Skill
					R != null
					&& R.CanBeCasted()
					&& me.CanCast()
					&& me.Distance2D(e) <= 200
					&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
					&& Utils.SleepCheck("R")
					)
				{
					R.UseAbility();
					Utils.Sleep(200, "R");
				} // R Skill end
				if ( // sheep
					sheep != null
					&& sheep.CanBeCasted()
					&& me.CanCast()
					&& !e.IsLinkensProtected()
					&& !e.IsMagicImmune()
					&& me.Distance2D(e) <= 1400
					&& !stoneModif
					&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
					&& Utils.SleepCheck("sheep")
					)
				{
					sheep.UseAbility(e);
					Utils.Sleep(250, "sheep");
				} // sheep Item end
				if ( // Abyssal Blade
					abyssal != null
					&& abyssal.CanBeCasted()
					&& me.CanCast()
					&& !e.IsStunned()
					&& !e.IsHexed()
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name)
					&& Utils.SleepCheck("abyssal")
					&& me.Distance2D(e) <= 400
					)
				{
					abyssal.UseAbility(e);
					Utils.Sleep(250, "abyssal");
				} // Abyssal Item end
				if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e) <= 900
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name) &&
					Utils.SleepCheck("orchid"))
				{
					orchid.UseAbility(e);
					Utils.Sleep(100, "orchid");
				}

				if (Shiva != null && Shiva.CanBeCasted() && me.Distance2D(e) <= 600
					&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(Shiva.Name)
					&& !e.IsMagicImmune() && Utils.SleepCheck("Shiva"))
				{
					Shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
				}
				if ( // ethereal
					ethereal != null
					&& ethereal.CanBeCasted()
					&& me.CanCast()
					&& !e.IsLinkensProtected()
					&& !e.IsMagicImmune()
					&& !stoneModif
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
					&& Utils.SleepCheck("ethereal")
					)
				{
					ethereal.UseAbility(e);
					Utils.Sleep(200, "ethereal");
				} // ethereal Item end
				if (
					blink != null
					&& Remmnant != null
					&& me.CanCast()
					&& blink.CanBeCasted()
					&& me.Distance2D(e) >= 450
					&& me.Distance2D(e) <= 1150
					&& Remmnant.Distance2D(me) >= 300
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
					&& Utils.SleepCheck("blink")
					)
				{
					blink.UseAbility(e.Position);
					Utils.Sleep(250, "blink");
				}
				if (
					blink != null
					&& me.CanCast()
					&& blink.CanBeCasted()
					&& me.Distance2D(e) >= 450
					&& me.Distance2D(e) <= 1150
					&& remnant.Count == 0
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
					&& Utils.SleepCheck("blink")
					)
				{
					blink.UseAbility(e.Position);
					Utils.Sleep(250, "blink");
				}

				if ( // SoulRing Item 
					soulring != null
					&& soulring.CanBeCasted()
					&& me.CanCast()
					&& me.Health >= (me.MaximumHealth * 0.5)
					&& me.Mana <= R.ManaCost
					&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(soulring.Name)
					)
				{
					soulring.UseAbility();
				} // SoulRing Item end
				if ( // Dagon
					me.CanCast()
					&& dagon != null
					&& (ethereal == null
						|| (ModifEther
							|| ethereal.Cooldown < 17))
					&& !e.IsLinkensProtected()
					&& dagon.CanBeCasted()
					&& !e.IsMagicImmune()
					&& !stoneModif
					&& Utils.SleepCheck("dagon")
					)
				{
					dagon.UseAbility(e);
					Utils.Sleep(200, "dagon");
				} // Dagon Item end
				if ( // atos Blade
					atos != null
					&& atos.CanBeCasted()
					&& me.CanCast()
					&& !e.IsLinkensProtected()
					&& !e.IsMagicImmune()
					&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(atos.Name)
					&& me.Distance2D(e) <= 2000
					&& Utils.SleepCheck("atos")
					)
				{
					atos.UseAbility(e);

					Utils.Sleep(250, "atos");
				} // atos Item end
				if (urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0 && me.Distance2D(e) <= 400
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
				{
					urn.UseAbility(e);
					Utils.Sleep(240, "urn");
				}
				if ( // vail
					vail != null
					&& vail.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
					&& me.Distance2D(e) <= 1500
					&& Utils.SleepCheck("vail")
					)
				{
					vail.UseAbility(e.Position);
					Utils.Sleep(250, "vail");
				} // orchid Item end
				if (
					stick != null
					&& stick.CanBeCasted()
					&& stick.CurrentCharges != 0
					&& me.Distance2D(e) <= 700
					&& (me.Health <= (me.MaximumHealth * 0.5)
						|| me.Mana <= (me.MaximumMana * 0.5))
					&& menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(stick.Name))
				{
					stick.UseAbility();
					Utils.Sleep(200, "mana_items");
				}
				if ( // Satanic 
					satanic != null &&
					me.Health <= (me.MaximumHealth * 0.3) &&
					satanic.CanBeCasted() &&
					me.Distance2D(e) <= me.AttackRange + 50
					&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
					&& Utils.SleepCheck("satanic")
					)
				{
					satanic.UseAbility();
					Utils.Sleep(240, "satanic");
				} // Satanic Item end
				if (mail != null && mail.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 650) >=
														   (menu.Item("Heelm").GetValue<Slider>().Value)) &&
					menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name) && Utils.SleepCheck("mail"))
				{
					mail.UseAbility();
					Utils.Sleep(100, "mail");
				}
				if (bkb != null && bkb.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 650) >=
														 (menu.Item("Heel").GetValue<Slider>().Value)) &&
					menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name) && Utils.SleepCheck("bkb"))
				{
					bkb.UseAbility();
					Utils.Sleep(100, "bkb");
				}
			}
		}
	}
	class Print
	{
		public class LogMessage
		{
			public static void Success(string text, params object[] arguments)
			{
				Game.PrintMessage("<font color='#e0007b'>" + text + "</font>", MessageType.LogMessage);
			}
		} // Console class

		public class ConsoleMessage
		{
			public static void Encolored(string text, ConsoleColor color, params object[] arguments)
			{
				var clr = System.Console.ForegroundColor;
				System.Console.ForegroundColor = color;
				System.Console.WriteLine(text, arguments);
				System.Console.ForegroundColor = clr;
			}
			public static void Success(string text, params object[] arguments)
			{
				Encolored(text, ConsoleColor.Red, arguments);
			}
		} // LogMessage class
	}
}
