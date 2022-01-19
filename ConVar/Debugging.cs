using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Facepunch;
using Facepunch.Unity;
using Rust;
using UnityEngine;

namespace ConVar
{
	[Factory("debug")]
	public class Debugging : ConsoleSystem
	{
		[ServerVar]
		[ClientVar]
		public static bool checktriggers = false;

		[ServerVar]
		public static bool checkparentingtriggers = true;

		[ServerVar(Saved = false, Help = "Shows some debug info for dismount attempts.")]
		public static bool DebugDismounts = false;

		[ServerVar(Help = "Do not damage any items")]
		public static bool disablecondition = false;

		[ClientVar]
		[ServerVar]
		public static bool callbacks = false;

		[ServerVar]
		[ClientVar]
		public static bool log
		{
			get
			{
				return Debug.get_unityLogger().get_logEnabled();
			}
			set
			{
				Debug.get_unityLogger().set_logEnabled(value);
			}
		}

		[ServerVar]
		[ClientVar]
		public static void renderinfo(Arg arg)
		{
			RenderInfo.GenerateReport();
		}

		[ClientVar]
		[ServerVar]
		public static void stall(Arg arg)
		{
			float num = Mathf.Clamp(arg.GetFloat(0, 0f), 0f, 1f);
			arg.ReplyWith("Stalling for " + num + " seconds...");
			Thread.Sleep(Mathf.RoundToInt(num * 1000f));
		}

		[ServerVar(Help = "Takes you in and out of your current network group, causing you to delete and then download all entities in your PVS again")]
		public static void flushgroup(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!((Object)(object)basePlayer == (Object)null))
			{
				basePlayer.net.SwitchGroup(BaseNetworkable.LimboNetworkGroup);
				basePlayer.UpdateNetworkGroup();
			}
		}

		[ServerVar(Help = "Break the current held object")]
		public static void breakheld(Arg arg)
		{
			Item activeItem = arg.Player().GetActiveItem();
			activeItem?.LoseCondition(activeItem.condition * 2f);
		}

		[ServerVar(Help = "reset all puzzles")]
		public static void puzzlereset(Arg arg)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if (!((Object)(object)arg.Player() == (Object)null))
			{
				PuzzleReset[] array = Object.FindObjectsOfType<PuzzleReset>();
				Debug.Log((object)"iterating...");
				PuzzleReset[] array2 = array;
				foreach (PuzzleReset puzzleReset in array2)
				{
					Debug.Log((object)("resetting puzzle at :" + ((Component)puzzleReset).get_transform().get_position()));
					puzzleReset.DoReset();
					puzzleReset.ResetTimer();
				}
			}
		}

		[ServerVar(EditorOnly = true, Help = "respawn all puzzles from their prefabs")]
		public static void puzzleprefabrespawn(Arg arg)
		{
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			foreach (BaseNetworkable item in BaseNetworkable.serverEntities.Where((BaseNetworkable x) => x is IOEntity && PrefabAttribute.server.Find<Construction>(x.prefabID) == null).ToList())
			{
				item.Kill();
			}
			foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
			{
				GameObject val = GameManager.server.FindPrefab(((Object)((Component)monument).get_gameObject()).get_name());
				if ((Object)(object)val == (Object)null)
				{
					continue;
				}
				Dictionary<IOEntity, IOEntity> dictionary = new Dictionary<IOEntity, IOEntity>();
				IOEntity[] componentsInChildren = val.GetComponentsInChildren<IOEntity>(true);
				foreach (IOEntity iOEntity in componentsInChildren)
				{
					Quaternion rot = ((Component)monument).get_transform().get_rotation() * ((Component)iOEntity).get_transform().get_rotation();
					Vector3 pos = ((Component)monument).get_transform().TransformPoint(((Component)iOEntity).get_transform().get_position());
					BaseEntity newEntity = GameManager.server.CreateEntity(iOEntity.PrefabName, pos, rot);
					IOEntity iOEntity2 = newEntity as IOEntity;
					if (!((Object)(object)iOEntity2 != (Object)null))
					{
						continue;
					}
					dictionary.Add(iOEntity, iOEntity2);
					DoorManipulator doorManipulator = newEntity as DoorManipulator;
					if ((Object)(object)doorManipulator != (Object)null)
					{
						List<Door> list = Pool.GetList<Door>();
						global::Vis.Entities(((Component)newEntity).get_transform().get_position(), 10f, list, -1, (QueryTriggerInteraction)2);
						Door door = list.OrderBy((Door x) => x.Distance(((Component)newEntity).get_transform().get_position())).FirstOrDefault();
						if ((Object)(object)door != (Object)null)
						{
							doorManipulator.targetDoor = door;
						}
						Pool.FreeList<Door>(ref list);
					}
					CardReader cardReader = newEntity as CardReader;
					if ((Object)(object)cardReader != (Object)null)
					{
						CardReader cardReader2 = iOEntity as CardReader;
						if ((Object)(object)cardReader2 != (Object)null)
						{
							cardReader.accessLevel = cardReader2.accessLevel;
							cardReader.accessDuration = cardReader2.accessDuration;
						}
					}
					TimerSwitch timerSwitch = newEntity as TimerSwitch;
					if ((Object)(object)timerSwitch != (Object)null)
					{
						TimerSwitch timerSwitch2 = iOEntity as TimerSwitch;
						if ((Object)(object)timerSwitch2 != (Object)null)
						{
							timerSwitch.timerLength = timerSwitch2.timerLength;
						}
					}
				}
				foreach (KeyValuePair<IOEntity, IOEntity> item2 in dictionary)
				{
					IOEntity key = item2.Key;
					IOEntity value = item2.Value;
					for (int j = 0; j < key.outputs.Length; j++)
					{
						if (!((Object)(object)key.outputs[j].connectedTo.ioEnt == (Object)null))
						{
							value.outputs[j].connectedTo.ioEnt = dictionary[key.outputs[j].connectedTo.ioEnt];
							value.outputs[j].connectedToSlot = key.outputs[j].connectedToSlot;
						}
					}
				}
				foreach (IOEntity value2 in dictionary.Values)
				{
					value2.Spawn();
				}
			}
		}

		[ServerVar(Help = "Break all the items in your inventory whose name match the passed string")]
		public static void breakitem(Arg arg)
		{
			string @string = arg.GetString(0, "");
			foreach (Item item in arg.Player().inventory.containerMain.itemList)
			{
				if (StringEx.Contains(item.info.shortname, @string, CompareOptions.IgnoreCase) && item.hasCondition)
				{
					item.LoseCondition(item.condition * 2f);
				}
			}
		}

		[ServerVar]
		public static void refillvitals(Arg arg)
		{
			AdjustHealth(arg.Player(), 1000f);
			AdjustCalories(arg.Player(), 1000f);
			AdjustHydration(arg.Player(), 1000f);
		}

		[ServerVar]
		public static void heal(Arg arg)
		{
			AdjustHealth(arg.Player(), arg.GetInt(0, 1));
		}

		[ServerVar]
		public static void hurt(Arg arg)
		{
			AdjustHealth(arg.Player(), -arg.GetInt(0, 1), arg.GetString(1, string.Empty));
		}

		[ServerVar]
		public static void eat(Arg arg)
		{
			AdjustCalories(arg.Player(), arg.GetInt(0, 1), arg.GetInt(1, 1));
		}

		[ServerVar]
		public static void drink(Arg arg)
		{
			AdjustHydration(arg.Player(), arg.GetInt(0, 1), arg.GetInt(1, 1));
		}

		private static void AdjustHealth(BasePlayer player, float amount, string bone = null)
		{
			HitInfo hitInfo = new HitInfo(player, player, DamageType.Bullet, 0f - amount);
			if (!string.IsNullOrEmpty(bone))
			{
				hitInfo.HitBone = StringPool.Get(bone);
			}
			player.OnAttacked(hitInfo);
		}

		private static void AdjustCalories(BasePlayer player, float amount, float time = 1f)
		{
			player.metabolism.ApplyChange(MetabolismAttribute.Type.Calories, amount, time);
		}

		private static void AdjustHydration(BasePlayer player, float amount, float time = 1f)
		{
			player.metabolism.ApplyChange(MetabolismAttribute.Type.Hydration, amount, time);
		}

		[ServerVar]
		public static void ResetSleepingBagTimers(Arg arg)
		{
			SleepingBag.ResetTimersForPlayer(arg.Player());
		}

		public Debugging()
			: this()
		{
		}
	}
}
