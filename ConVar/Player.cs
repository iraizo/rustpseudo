using System;
using System.Collections.Generic;
using System.Text;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace ConVar
{
	[Factory("player")]
	public class Player : ConsoleSystem
	{
		[ServerVar]
		public static int tickrate_cl = 20;

		[ServerVar]
		public static int tickrate_sv = 16;

		[ServerUserVar]
		[ClientVar(AllowRunFromServer = true)]
		public static void cinematic_play(Arg arg)
		{
			if (!arg.HasArgs(1) || !arg.get_IsServerside())
			{
				return;
			}
			BasePlayer basePlayer = arg.Player();
			if (!((Object)(object)basePlayer == (Object)null))
			{
				string strCommand = string.Empty;
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					strCommand = arg.cmd.FullName + " " + arg.FullString + " " + basePlayer.UserIDString;
				}
				else if (Server.cinematic)
				{
					strCommand = arg.cmd.FullName + " " + arg.GetString(0, "") + " " + basePlayer.UserIDString;
				}
				if (Server.cinematic)
				{
					ConsoleNetwork.BroadcastToAllClients(strCommand);
				}
				else if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					ConsoleNetwork.SendClientCommand(arg.get_Connection(), strCommand);
				}
			}
		}

		[ServerUserVar]
		[ClientVar(AllowRunFromServer = true)]
		public static void cinematic_stop(Arg arg)
		{
			if (!arg.get_IsServerside())
			{
				return;
			}
			BasePlayer basePlayer = arg.Player();
			if (!((Object)(object)basePlayer == (Object)null))
			{
				string strCommand = string.Empty;
				if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					strCommand = arg.cmd.FullName + " " + arg.FullString + " " + basePlayer.UserIDString;
				}
				else if (Server.cinematic)
				{
					strCommand = arg.cmd.FullName + " " + basePlayer.UserIDString;
				}
				if (Server.cinematic)
				{
					ConsoleNetwork.BroadcastToAllClients(strCommand);
				}
				else if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
				{
					ConsoleNetwork.SendClientCommand(arg.get_Connection(), strCommand);
				}
			}
		}

		[ServerUserVar]
		public static void cinematic_gesture(Arg arg)
		{
			if (Server.cinematic)
			{
				string @string = arg.GetString(0, "");
				BasePlayer basePlayer = arg.GetPlayer(1);
				if ((Object)(object)basePlayer == (Object)null)
				{
					basePlayer = arg.Player();
				}
				basePlayer.UpdateActiveItem(0u);
				basePlayer.SignalBroadcast(BaseEntity.Signal.Gesture, @string);
			}
		}

		[ServerUserVar]
		public static void copyrotation(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer.IsAdmin || basePlayer.IsDeveloper || Server.cinematic)
			{
				uint uInt = arg.GetUInt(0, 0u);
				BasePlayer basePlayer2 = BasePlayer.FindByID(uInt);
				if ((Object)(object)basePlayer2 == (Object)null)
				{
					basePlayer2 = BasePlayer.FindBot(uInt);
				}
				if ((Object)(object)basePlayer2 != (Object)null)
				{
					basePlayer2.CopyRotation(basePlayer);
					Debug.Log((object)("Copied rotation of " + basePlayer2.UserIDString));
				}
			}
		}

		[ServerUserVar]
		public static void abandonmission(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer.HasActiveMission())
			{
				basePlayer.AbandonActiveMission();
			}
		}

		[ServerUserVar]
		public static void mount(Arg arg)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint uInt = arg.GetUInt(0, 0u);
			BasePlayer basePlayer2 = BasePlayer.FindByID(uInt);
			if ((Object)(object)basePlayer2 == (Object)null)
			{
				basePlayer2 = BasePlayer.FindBot(uInt);
			}
			RaycastHit hit = default(RaycastHit);
			if (!Object.op_Implicit((Object)(object)basePlayer2) || !Physics.Raycast(basePlayer.eyes.position, basePlayer.eyes.HeadForward(), ref hit, 5f, 10496, (QueryTriggerInteraction)1))
			{
				return;
			}
			BaseEntity entity = hit.GetEntity();
			if (!Object.op_Implicit((Object)(object)entity))
			{
				return;
			}
			BaseMountable baseMountable = ((Component)entity).GetComponent<BaseMountable>();
			if (!Object.op_Implicit((Object)(object)baseMountable))
			{
				BaseVehicle baseVehicle = ((Component)entity).GetComponentInParent<BaseVehicle>();
				if (Object.op_Implicit((Object)(object)baseVehicle))
				{
					if (!baseVehicle.isServer)
					{
						baseVehicle = BaseNetworkable.serverEntities.Find(baseVehicle.net.ID) as BaseVehicle;
					}
					baseVehicle.AttemptMount(basePlayer2);
					return;
				}
			}
			if (Object.op_Implicit((Object)(object)baseMountable) && !baseMountable.isServer)
			{
				baseMountable = BaseNetworkable.serverEntities.Find(baseMountable.net.ID) as BaseMountable;
			}
			if (Object.op_Implicit((Object)(object)baseMountable))
			{
				baseMountable.AttemptMount(basePlayer2);
			}
		}

		[ServerVar]
		public static void gotosleep(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint uInt = arg.GetUInt(0, 0u);
			BasePlayer basePlayer2 = BasePlayer.FindSleeping(uInt.ToString());
			if (!Object.op_Implicit((Object)(object)basePlayer2))
			{
				basePlayer2 = BasePlayer.FindBotClosestMatch(uInt.ToString());
				if (basePlayer2.IsSleeping())
				{
					basePlayer2 = null;
				}
			}
			if (Object.op_Implicit((Object)(object)basePlayer2))
			{
				basePlayer2.StartSleeping();
			}
		}

		[ServerVar]
		public static void dismount(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer.IsAdmin || basePlayer.IsDeveloper || Server.cinematic)
			{
				uint uInt = arg.GetUInt(0, 0u);
				BasePlayer basePlayer2 = BasePlayer.FindByID(uInt);
				if ((Object)(object)basePlayer2 == (Object)null)
				{
					basePlayer2 = BasePlayer.FindBot(uInt);
				}
				if (Object.op_Implicit((Object)(object)basePlayer2) && Object.op_Implicit((Object)(object)basePlayer2) && basePlayer2.isMounted)
				{
					basePlayer2.GetMounted().DismountPlayer(basePlayer2);
				}
			}
		}

		[ServerVar]
		public static void swapseat(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			uint uInt = arg.GetUInt(0, 0u);
			BasePlayer basePlayer2 = BasePlayer.FindByID(uInt);
			if ((Object)(object)basePlayer2 == (Object)null)
			{
				basePlayer2 = BasePlayer.FindBot(uInt);
			}
			if (Object.op_Implicit((Object)(object)basePlayer2))
			{
				int @int = arg.GetInt(1, 0);
				if (Object.op_Implicit((Object)(object)basePlayer2) && basePlayer2.isMounted && Object.op_Implicit((Object)(object)basePlayer2.GetMounted().VehicleParent()))
				{
					basePlayer2.GetMounted().VehicleParent().SwapSeats(basePlayer2, @int);
				}
			}
		}

		[ServerVar]
		public static void wakeup(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer.IsAdmin || basePlayer.IsDeveloper || Server.cinematic)
			{
				BasePlayer basePlayer2 = BasePlayer.FindSleeping(arg.GetUInt(0, 0u).ToString());
				if (Object.op_Implicit((Object)(object)basePlayer2))
				{
					basePlayer2.EndSleeping();
				}
			}
		}

		[ServerVar]
		public static void wakeupall(Arg arg)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = arg.Player();
			if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper && !Server.cinematic)
			{
				return;
			}
			List<BasePlayer> list = Pool.GetList<BasePlayer>();
			Enumerator<BasePlayer> enumerator = BasePlayer.sleepingPlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.get_Current();
					list.Add(current);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			foreach (BasePlayer item in list)
			{
				item.EndSleeping();
			}
			Pool.FreeList<BasePlayer>(ref list);
		}

		[ServerVar]
		public static void printstats(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"{basePlayer.lifeStory.secondsAlive:F1}s alive");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.secondsSleeping:F1}s sleeping");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.secondsSwimming:F1}s swimming");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.secondsInBase:F1}s in base");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.secondsWilderness:F1}s in wilderness");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.secondsInMonument:F1}s in monuments");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.secondsFlying:F1}s flying");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.secondsBoating:F1}s boating");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.secondsDriving:F1}s driving");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.metersRun:F1}m run");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.metersWalked:F1}m walked");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.totalDamageTaken:F1} damage taken");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.totalHealing:F1} damage healed");
			stringBuilder.AppendLine("===");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.killedPlayers} other players killed");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.killedScientists} scientists killed");
			stringBuilder.AppendLine($"{basePlayer.lifeStory.killedAnimals} animals killed");
			stringBuilder.AppendLine("===");
			stringBuilder.AppendLine("Weapon stats:");
			if (basePlayer.lifeStory.weaponStats != null)
			{
				foreach (WeaponStats weaponStat in basePlayer.lifeStory.weaponStats)
				{
					float num = (float)weaponStat.shotsHit / (float)weaponStat.shotsFired;
					num *= 100f;
					stringBuilder.AppendLine($"{weaponStat.weaponName} - shots fired: {weaponStat.shotsFired} shots hit: {weaponStat.shotsHit} accuracy: {num:F1}%");
				}
			}
			stringBuilder.AppendLine("===");
			stringBuilder.AppendLine("Misc stats:");
			if (basePlayer.lifeStory.genericStats != null)
			{
				foreach (GenericStat genericStat in basePlayer.lifeStory.genericStats)
				{
					stringBuilder.AppendLine($"{genericStat.key} = {genericStat.value}");
				}
			}
			arg.ReplyWith(stringBuilder.ToString());
		}

		[ServerVar]
		public static void printpresence(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			bool flag = (basePlayer.currentTimeCategory & 1) != 0;
			bool flag2 = (basePlayer.currentTimeCategory & 4) != 0;
			bool flag3 = (basePlayer.currentTimeCategory & 2) != 0;
			bool flag4 = (basePlayer.currentTimeCategory & 0x20) != 0;
			bool flag5 = (basePlayer.currentTimeCategory & 0x10) != 0;
			bool flag6 = (basePlayer.currentTimeCategory & 8) != 0;
			arg.ReplyWith($"Wilderness:{flag} Base:{flag2} Monument:{flag3} Swimming: {flag4} Boating: {flag5} Flying: {flag6}");
		}

		[ServerVar(Help = "Resets the PlayerState of the given player")]
		public static void resetstate(Arg args)
		{
			BasePlayer playerOrSleeper = args.GetPlayerOrSleeper(0);
			if ((Object)(object)playerOrSleeper == (Object)null)
			{
				args.ReplyWith("Player not found");
				return;
			}
			playerOrSleeper.ResetPlayerState();
			args.ReplyWith("Player state reset");
		}

		[ServerVar(ServerAdmin = true)]
		public static void fillwater(Arg arg)
		{
			bool num = arg.GetString(0, "").ToLower() == "salt";
			BasePlayer basePlayer = arg.Player();
			ItemDefinition liquidType = ItemManager.FindItemDefinition(num ? "water.salt" : "water");
			ItemModContainer itemModContainer = default(ItemModContainer);
			for (int i = 0; i < PlayerBelt.MaxBeltSlots; i++)
			{
				Item itemInSlot = basePlayer.Belt.GetItemInSlot(i);
				BaseLiquidVessel baseLiquidVessel;
				if (itemInSlot != null && (baseLiquidVessel = itemInSlot.GetHeldEntity() as BaseLiquidVessel) != null && baseLiquidVessel.hasLid)
				{
					int amount = 999;
					if (((Component)baseLiquidVessel.GetItem().info).TryGetComponent<ItemModContainer>(ref itemModContainer))
					{
						amount = itemModContainer.maxStackSize;
					}
					baseLiquidVessel.AddLiquid(liquidType, amount);
				}
			}
		}

		[ServerVar]
		public static void createskull(Arg arg)
		{
			string text = arg.GetString(0, "");
			BasePlayer basePlayer = arg.Player();
			if (string.IsNullOrEmpty(text))
			{
				text = RandomUsernames.Get(Random.Range(0, 1000));
			}
			Item item = ItemManager.Create(ItemManager.FindItemDefinition("skull.human"), 1, 0uL);
			item.name = HumanBodyResourceDispenser.CreateSkullName(text);
			basePlayer.inventory.GiveItem(item);
		}

		[ServerVar]
		public static void gesture_radius(Arg arg)
		{
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = arg.Player();
			if ((Object)(object)basePlayer == (Object)null || !basePlayer.IsAdmin)
			{
				return;
			}
			float @float = arg.GetFloat(0, 0f);
			List<string> list = Pool.GetList<string>();
			for (int i = 0; i < 5; i++)
			{
				if (!string.IsNullOrEmpty(arg.GetString(i + 1, "")))
				{
					list.Add(arg.GetString(i + 1, ""));
				}
			}
			if (list.Count == 0)
			{
				arg.ReplyWith("No gestures provided. eg. player.gesture_radius 10f cabbagepatch raiseroof");
				return;
			}
			List<BasePlayer> list2 = Pool.GetList<BasePlayer>();
			global::Vis.Entities(((Component)basePlayer).get_transform().get_position(), @float, list2, 131072, (QueryTriggerInteraction)2);
			foreach (BasePlayer item in list2)
			{
				GestureConfig toPlay = basePlayer.gestureList.StringToGesture(list[Random.Range(0, list.Count)]);
				item.Server_StartGesture(toPlay);
			}
			Pool.FreeList<BasePlayer>(ref list2);
		}

		[ServerVar]
		public static void stopgesture_radius(Arg arg)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = arg.Player();
			if ((Object)(object)basePlayer == (Object)null || !basePlayer.IsAdmin)
			{
				return;
			}
			float @float = arg.GetFloat(0, 0f);
			List<BasePlayer> list = Pool.GetList<BasePlayer>();
			global::Vis.Entities(((Component)basePlayer).get_transform().get_position(), @float, list, 131072, (QueryTriggerInteraction)2);
			foreach (BasePlayer item in list)
			{
				item.Server_CancelGesture();
			}
			Pool.FreeList<BasePlayer>(ref list);
		}

		[ServerVar]
		public static void markhostile(Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if ((Object)(object)basePlayer != (Object)null)
			{
				basePlayer.MarkHostileFor();
			}
		}

		public Player()
			: this()
		{
		}
	}
}
