using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Nexus.Models;
using Network;
using Network.Visibility;
using ProtoBuf.Nexus;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

namespace ConVar
{
	[Factory("global")]
	public class Global : ConsoleSystem
	{
		private static int _developer;

		[ServerVar]
		[ClientVar]
		public static int maxthreads = 8;

		[ServerVar(Saved = true)]
		[ClientVar(Saved = true)]
		public static int perf = 0;

		[ClientVar(ClientInfo = true, Saved = true, Help = "If you're an admin this will enable god mode")]
		public static bool god = false;

		[ClientVar(ClientInfo = true, Saved = true, Help = "If enabled you will be networked when you're spectating. This means that you will hear audio chat, but also means that cheaters will potentially be able to detect you watching them.")]
		public static bool specnet = false;

		[ServerVar]
		[ClientVar]
		public static int developer
		{
			get
			{
				return _developer;
			}
			set
			{
				_developer = value;
			}
		}

		[ServerVar]
		public static void restart(Arg args)
		{
			ServerMgr.RestartServer(args.GetString(1, string.Empty), args.GetInt(0, 300));
		}

		[ClientVar]
		[ServerVar]
		public static void quit(Arg args)
		{
			SingletonComponent<ServerMgr>.Instance.Shutdown();
			Application.isQuitting = true;
			Net.sv.Stop("quit");
			Process.GetCurrentProcess().Kill();
			Debug.Log((object)"Quitting");
			Application.Quit();
		}

		[ServerVar]
		public static void report(Arg args)
		{
			ServerPerformance.DoReport();
		}

		[ServerVar]
		[ClientVar]
		public static void objects(Arg args)
		{
			Object[] array = Object.FindObjectsOfType<Object>();
			string text = "";
			Dictionary<Type, int> dictionary = new Dictionary<Type, int>();
			Dictionary<Type, long> dictionary2 = new Dictionary<Type, long>();
			Object[] array2 = array;
			foreach (Object val in array2)
			{
				int runtimeMemorySize = Profiler.GetRuntimeMemorySize(val);
				if (dictionary.ContainsKey(((object)val).GetType()))
				{
					dictionary[((object)val).GetType()]++;
				}
				else
				{
					dictionary.Add(((object)val).GetType(), 1);
				}
				if (dictionary2.ContainsKey(((object)val).GetType()))
				{
					dictionary2[((object)val).GetType()] += runtimeMemorySize;
				}
				else
				{
					dictionary2.Add(((object)val).GetType(), runtimeMemorySize);
				}
			}
			foreach (KeyValuePair<Type, long> item in dictionary2.OrderByDescending(delegate(KeyValuePair<Type, long> x)
			{
				KeyValuePair<Type, long> keyValuePair = x;
				return keyValuePair.Value;
			}))
			{
				text = string.Concat(text, dictionary[item.Key].ToString().PadLeft(10), " ", NumberExtensions.FormatBytes<long>(item.Value, false).PadLeft(15), "\t", item.Key, "\n");
			}
			args.ReplyWith(text);
		}

		[ServerVar]
		[ClientVar]
		public static void textures(Arg args)
		{
			Texture[] array = Object.FindObjectsOfType<Texture>();
			string text = "";
			Texture[] array2 = array;
			foreach (Texture val in array2)
			{
				string text2 = NumberExtensions.FormatBytes<int>(Profiler.GetRuntimeMemorySize((Object)(object)val), false);
				text = text + ((object)val).ToString().PadRight(30) + ((Object)val).get_name().PadRight(30) + text2 + "\n";
			}
			args.ReplyWith(text);
		}

		[ServerVar]
		[ClientVar]
		public static void colliders(Arg args)
		{
			int num = (from x in Object.FindObjectsOfType<Collider>()
				where x.get_enabled()
				select x).Count();
			int num2 = (from x in Object.FindObjectsOfType<Collider>()
				where !x.get_enabled()
				select x).Count();
			string text = num + " colliders enabled, " + num2 + " disabled";
			args.ReplyWith(text);
		}

		[ServerVar]
		[ClientVar]
		public static void error(Arg args)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((GameObject)null).get_transform().set_position(Vector3.get_zero());
		}

		[ServerVar]
		[ClientVar]
		public static void queue(Arg args)
		{
			string text = "";
			text = text + "stabilityCheckQueue:\t\t" + ((ObjectWorkQueue<StabilityEntity>)StabilityEntity.stabilityCheckQueue).Info() + "\n";
			text = text + "updateSurroundingsQueue:\t" + ((ObjectWorkQueue<Bounds>)StabilityEntity.updateSurroundingsQueue).Info() + "\n";
			args.ReplyWith(text);
		}

		[ServerUserVar]
		public static void setinfo(Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (Object.op_Implicit((Object)(object)basePlayer))
			{
				string @string = args.GetString(0, (string)null);
				string string2 = args.GetString(1, (string)null);
				if (@string != null && string2 != null)
				{
					basePlayer.SetInfo(@string, string2);
				}
			}
		}

		[ServerVar]
		public static void sleep(Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (Object.op_Implicit((Object)(object)basePlayer) && !basePlayer.IsSleeping() && !basePlayer.IsSpectating() && !basePlayer.IsDead())
			{
				basePlayer.StartSleeping();
			}
		}

		[ServerUserVar]
		public static void kill(Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (Object.op_Implicit((Object)(object)basePlayer) && !basePlayer.IsSpectating() && !basePlayer.IsDead())
			{
				if (basePlayer.CanSuicide())
				{
					basePlayer.MarkSuicide();
					basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, useProtection: false);
				}
				else
				{
					basePlayer.ConsoleMessage("You can't suicide again so quickly, wait a while");
				}
			}
		}

		[ServerUserVar]
		public static void respawn(Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				return;
			}
			if (!basePlayer.IsDead() && !basePlayer.IsSpectating())
			{
				if (developer > 0)
				{
					Debug.LogWarning((object)string.Concat(basePlayer, " wanted to respawn but isn't dead or spectating"));
				}
				basePlayer.SendNetworkUpdate();
			}
			else if (basePlayer.CanRespawn())
			{
				basePlayer.MarkRespawn();
				basePlayer.Respawn();
			}
			else
			{
				basePlayer.ConsoleMessage("You can't respawn again so quickly, wait a while");
			}
		}

		[ServerVar]
		public static void injure(Arg args)
		{
			InjurePlayer(args.Player());
		}

		public static void InjurePlayer(BasePlayer ply)
		{
			if ((Object)(object)ply == (Object)null || ply.IsDead())
			{
				return;
			}
			if (Server.woundingenabled && !ply.IsIncapacitated() && !ply.IsSleeping() && !ply.isMounted)
			{
				if (ply.IsCrawling())
				{
					ply.GoToIncapacitated(null);
				}
				else
				{
					ply.BecomeWounded();
				}
			}
			else
			{
				ply.ConsoleMessage("Can't go to wounded state right now.");
			}
		}

		[ServerVar]
		public static void recover(Arg args)
		{
			RecoverPlayer(args.Player());
		}

		public static void RecoverPlayer(BasePlayer ply)
		{
			if (!((Object)(object)ply == (Object)null) && !ply.IsDead())
			{
				ply.StopWounded();
			}
		}

		[ServerVar]
		public static void spectate(Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (Object.op_Implicit((Object)(object)basePlayer))
			{
				if (!basePlayer.IsDead())
				{
					basePlayer.DieInstantly();
				}
				string @string = args.GetString(0, "");
				if (basePlayer.IsDead())
				{
					basePlayer.StartSpectating();
					basePlayer.UpdateSpectateTarget(@string);
				}
			}
		}

		[ServerUserVar]
		public static void respawn_sleepingbag(Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!Object.op_Implicit((Object)(object)basePlayer) || !basePlayer.IsDead())
			{
				return;
			}
			uint uInt = args.GetUInt(0, 0u);
			if (uInt == 0)
			{
				args.ReplyWith("Missing sleeping bag ID");
				return;
			}
			string @string = args.GetString(1, "");
			string errorMessage;
			if (NexusServer.Started && !string.IsNullOrWhiteSpace(@string))
			{
				if (!ZoneController.Instance.CanRespawnAcrossZones(basePlayer))
				{
					args.ReplyWith("You cannot respawn to a different zone");
					return;
				}
				NexusZoneDetails val = List.FindWith<NexusZoneDetails, string>(NexusServer.Zones, (Func<NexusZoneDetails, string>)((NexusZoneDetails z) => z.get_Name()), @string);
				if (val == null)
				{
					args.ReplyWith("Zone was not found");
				}
				else if (!basePlayer.CanRespawn())
				{
					args.ReplyWith("You can't respawn again so quickly, wait a while");
				}
				else
				{
					NexusRespawn(basePlayer, val, uInt);
				}
			}
			else if (!SleepingBag.TrySpawnPlayer(basePlayer, uInt, out errorMessage))
			{
				args.ReplyWith(errorMessage);
			}
			static async void NexusRespawn(BasePlayer player, NexusZoneDetails toZone, uint sleepingBag)
			{
				_ = 1;
				try
				{
					player.nextRespawnTime = float.PositiveInfinity;
					Request val2 = Pool.Get<Request>();
					val2.respawnAtBag = Pool.Get<SleepingBagRespawnRequest>();
					val2.respawnAtBag.userId = player.userID;
					val2.respawnAtBag.sleepingBagId = sleepingBag;
					val2.respawnAtBag.secondaryData = player.SaveSecondaryData();
					Response val3 = await NexusServer.ZoneRpc(toZone.get_Name(), val2);
					try
					{
						if (!val3.status.success)
						{
							if (player.IsConnected)
							{
								player.ConsoleMessage("RespawnAtBag failed: " + val3.status.errorMessage);
							}
							return;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					await NexusServer.ZoneClient.Assign(player.UserIDString, toZone.get_Name());
					if (player.IsConnected)
					{
						ConsoleNetwork.SendClientCommand(player.net.get_connection(), "nexus.redirect", toZone.get_IpAddress(), toZone.get_Port());
						player.Kick("Redirecting to another zone...");
					}
				}
				catch (Exception ex)
				{
					if (player.IsConnected)
					{
						player.ConsoleMessage(ex.ToString());
					}
				}
				finally
				{
					player.MarkRespawn();
				}
			}
		}

		[ServerUserVar]
		public static void respawn_sleepingbag_remove(Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				return;
			}
			uint uInt = args.GetUInt(0, 0u);
			if (uInt == 0)
			{
				args.ReplyWith("Missing sleeping bag ID");
				return;
			}
			string @string = args.GetString(1, "");
			if (NexusServer.Started && !string.IsNullOrWhiteSpace(@string))
			{
				if (ZoneController.Instance.CanRespawnAcrossZones(basePlayer))
				{
					NexusRemoveBag(basePlayer, @string, uInt);
				}
			}
			else
			{
				SleepingBag.DestroyBag(basePlayer.userID, uInt);
			}
			static async void NexusRemoveBag(BasePlayer player, string zoneName, uint sleepingBag)
			{
				try
				{
					Request val = Pool.Get<Request>();
					val.destroyBag = Pool.Get<SleepingBagDestroyRequest>();
					val.destroyBag.userId = player.userID;
					val.destroyBag.sleepingBagId = sleepingBag;
					(await NexusServer.ZoneRpc(zoneName, val)).Dispose();
				}
				catch (Exception ex)
				{
					if (player.IsConnected)
					{
						player.ConsoleMessage(ex.ToString());
					}
				}
			}
		}

		[ServerUserVar]
		public static void status_sv(Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (Object.op_Implicit((Object)(object)basePlayer))
			{
				args.ReplyWith(basePlayer.GetDebugStatus());
			}
		}

		[ClientVar]
		public static void status_cl(Arg args)
		{
		}

		[ServerVar]
		public static void teleport(Arg args)
		{
			if (args.HasArgs(2))
			{
				BasePlayer playerOrSleeperOrBot = args.GetPlayerOrSleeperOrBot(0);
				if (Object.op_Implicit((Object)(object)playerOrSleeperOrBot) && playerOrSleeperOrBot.IsAlive())
				{
					BasePlayer playerOrSleeperOrBot2 = args.GetPlayerOrSleeperOrBot(1);
					if (Object.op_Implicit((Object)(object)playerOrSleeperOrBot2) && playerOrSleeperOrBot2.IsAlive())
					{
						playerOrSleeperOrBot.Teleport(playerOrSleeperOrBot2);
					}
				}
				return;
			}
			BasePlayer basePlayer = args.Player();
			if (Object.op_Implicit((Object)(object)basePlayer) && basePlayer.IsAlive())
			{
				BasePlayer playerOrSleeperOrBot3 = args.GetPlayerOrSleeperOrBot(0);
				if (Object.op_Implicit((Object)(object)playerOrSleeperOrBot3) && playerOrSleeperOrBot3.IsAlive())
				{
					basePlayer.Teleport(playerOrSleeperOrBot3);
				}
			}
		}

		[ServerVar]
		public static void teleport2me(Arg args)
		{
			BasePlayer playerOrSleeperOrBot = args.GetPlayerOrSleeperOrBot(0);
			if ((Object)(object)playerOrSleeperOrBot == (Object)null)
			{
				args.ReplyWith("Player or bot not found");
				return;
			}
			if (!playerOrSleeperOrBot.IsAlive())
			{
				args.ReplyWith("Target is not alive");
				return;
			}
			BasePlayer basePlayer = args.Player();
			if (Object.op_Implicit((Object)(object)basePlayer) && basePlayer.IsAlive())
			{
				playerOrSleeperOrBot.Teleport(basePlayer);
			}
		}

		[ServerVar]
		public static void teleportany(Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (Object.op_Implicit((Object)(object)basePlayer) && basePlayer.IsAlive())
			{
				basePlayer.Teleport(args.GetString(0, ""), playersOnly: false);
			}
		}

		[ServerVar]
		public static void teleportpos(Arg args)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = args.Player();
			if (Object.op_Implicit((Object)(object)basePlayer) && basePlayer.IsAlive())
			{
				basePlayer.Teleport(args.GetVector3(0, Vector3.get_zero()));
			}
		}

		[ServerVar]
		public static void teleportlos(Arg args)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = args.Player();
			if (Object.op_Implicit((Object)(object)basePlayer) && basePlayer.IsAlive())
			{
				Ray val = basePlayer.eyes.HeadRay();
				int @int = args.GetInt(0, 1000);
				RaycastHit val2 = default(RaycastHit);
				if (Physics.Raycast(val, ref val2, (float)@int, 1218652417))
				{
					basePlayer.Teleport(((RaycastHit)(ref val2)).get_point());
				}
				else
				{
					basePlayer.Teleport(((Ray)(ref val)).get_origin() + ((Ray)(ref val)).get_direction() * (float)@int);
				}
			}
		}

		[ServerVar]
		public static void teleport2owneditem(Arg arg)
		{
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = arg.Player();
			BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			ulong result;
			if ((Object)(object)playerOrSleeper != (Object)null)
			{
				result = playerOrSleeper.userID;
			}
			else if (!ulong.TryParse(arg.GetString(0, ""), out result))
			{
				arg.ReplyWith("No player with that id found");
				return;
			}
			string @string = arg.GetString(1, "");
			BaseEntity[] array = BaseEntity.Util.FindTargetsOwnedBy(result, @string);
			if (array.Length == 0)
			{
				arg.ReplyWith("No targets found");
				return;
			}
			int num = Random.Range(0, array.Length);
			arg.ReplyWith($"Teleporting to {array[num].ShortPrefabName} at {((Component)array[num]).get_transform().get_position()}");
			basePlayer.Teleport(((Component)array[num]).get_transform().get_position());
		}

		[ServerVar]
		public static void teleport2autheditem(Arg arg)
		{
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = arg.Player();
			BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			ulong result;
			if ((Object)(object)playerOrSleeper != (Object)null)
			{
				result = playerOrSleeper.userID;
			}
			else if (!ulong.TryParse(arg.GetString(0, ""), out result))
			{
				arg.ReplyWith("No player with that id found");
				return;
			}
			string @string = arg.GetString(1, "");
			BaseEntity[] array = BaseEntity.Util.FindTargetsAuthedTo(result, @string);
			if (array.Length == 0)
			{
				arg.ReplyWith("No targets found");
				return;
			}
			int num = Random.Range(0, array.Length);
			arg.ReplyWith($"Teleporting to {array[num].ShortPrefabName} at {((Component)array[num]).get_transform().get_position()}");
			basePlayer.Teleport(((Component)array[num]).get_transform().get_position());
		}

		[ServerVar]
		public static void teleport2marker(Arg arg)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = arg.Player();
			if (basePlayer.ServerCurrentMapNote == null)
			{
				arg.ReplyWith("You don't have a marker set");
				return;
			}
			Vector3 worldPosition = basePlayer.ServerCurrentMapNote.worldPosition;
			float height = TerrainMeta.HeightMap.GetHeight(worldPosition);
			float height2 = TerrainMeta.WaterMap.GetHeight(worldPosition);
			worldPosition.y = Mathf.Max(height, height2);
			basePlayer.Teleport(worldPosition);
		}

		[ServerVar]
		public static void teleport2death(Arg arg)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = arg.Player();
			if (basePlayer.ServerCurrentDeathNote == null)
			{
				arg.ReplyWith("You don't have a current death note!");
			}
			Vector3 worldPosition = basePlayer.ServerCurrentDeathNote.worldPosition;
			basePlayer.Teleport(worldPosition);
		}

		[ServerVar]
		[ClientVar]
		public static void free(Arg args)
		{
			Pool.clear_prefabs(args);
			Pool.clear_assets(args);
			Pool.clear_memory(args);
			GC.collect();
			GC.unload();
		}

		[ServerVar(ServerUser = true)]
		[ClientVar]
		public static void version(Arg arg)
		{
			arg.ReplyWith($"Protocol: {Protocol.get_printable()}\nBuild Date: {BuildInfo.get_Current().get_BuildDate()}\nUnity Version: {Application.get_unityVersion()}\nChangeset: {BuildInfo.get_Current().get_Scm().get_ChangeId()}\nBranch: {BuildInfo.get_Current().get_Scm().get_Branch()}");
		}

		[ServerVar]
		[ClientVar]
		public static void sysinfo(Arg arg)
		{
			arg.ReplyWith(SystemInfoGeneralText.currentInfo);
		}

		[ServerVar]
		[ClientVar]
		public static void sysuid(Arg arg)
		{
			arg.ReplyWith(SystemInfo.get_deviceUniqueIdentifier());
		}

		[ServerVar]
		public static void breakitem(Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (Object.op_Implicit((Object)(object)basePlayer))
			{
				Item activeItem = basePlayer.GetActiveItem();
				activeItem?.LoseCondition(activeItem.condition);
			}
		}

		[ServerVar]
		public static void breakclothing(Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				return;
			}
			foreach (Item item in basePlayer.inventory.containerWear.itemList)
			{
				item?.LoseCondition(item.condition);
			}
		}

		[ServerVar]
		[ClientVar]
		public static void subscriptions(Arg arg)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			TextTable val = new TextTable();
			val.AddColumn("realm");
			val.AddColumn("group");
			BasePlayer basePlayer = arg.Player();
			if (Object.op_Implicit((Object)(object)basePlayer))
			{
				Enumerator<Group> enumerator = basePlayer.net.subscriber.subscribed.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Group current = enumerator.get_Current();
						val.AddRow(new string[2]
						{
							"sv",
							current.ID.ToString()
						});
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
			}
			arg.ReplyWith(((object)val).ToString());
		}

		public Global()
			: this()
		{
		}
	}
}
