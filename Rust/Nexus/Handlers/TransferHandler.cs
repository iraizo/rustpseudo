using System;
using System.Collections.Generic;
using ConVar;
using Network;
using ProtoBuf;
using ProtoBuf.Nexus;
using UnityEngine;

namespace Rust.Nexus.Handlers
{
	public class TransferHandler : BaseNexusRequestHandler<TransferRequest>
	{
		private static readonly Dictionary<uint, uint> UidMapping = new Dictionary<uint, uint>();

		private static readonly Dictionary<BaseEntity, Entity> EntityToSpawn = new Dictionary<BaseEntity, Entity>();

		private static readonly Dictionary<ulong, BasePlayer> SpawnedPlayers = new Dictionary<ulong, BasePlayer>();

		private static readonly List<string> PlayerIds = new List<string>();

		protected override void Handle()
		{
			UidMapping.Clear();
			base.Request.InspectUids((UidInspector<uint>)UpdateWithNewUid);
			PlayerIds.Clear();
			foreach (Entity entity in base.Request.entities)
			{
				if (entity.basePlayer != null)
				{
					ulong userid = entity.basePlayer.userid;
					Debug.Log((object)$"Found player {userid} in transfer");
					PlayerIds.Add(userid.ToString("G"));
					BasePlayer basePlayer = BasePlayer.FindByID(userid) ?? BasePlayer.FindSleeping(userid);
					if ((Object)(object)basePlayer != (Object)null)
					{
						if (basePlayer.IsConnected)
						{
							basePlayer.Kick("Player transfer is overwriting you - contact developers!");
						}
						basePlayer.Kill();
					}
					entity.basePlayer.currentTeam = 0uL;
					if ((entity.basePlayer.playerFlags & 0x10) == 0)
					{
						BasePlayer basePlayer2 = entity.basePlayer;
						basePlayer2.playerFlags |= 0x800000;
					}
					if (entity.basePlayer.loadingTimeout <= 0f || entity.basePlayer.loadingTimeout > ConVar.Nexus.loadingTimeout)
					{
						entity.basePlayer.loadingTimeout = ConVar.Nexus.loadingTimeout;
					}
				}
				if (entity.baseCombat != null && entity.baseEntity != null)
				{
					BaseEntity baseEntity = entity.baseEntity;
					baseEntity.flags |= 0x1000000;
				}
			}
			RepositionEntitiesFromTransfer();
			SpawnedPlayers.Clear();
			SpawnEntities(SpawnedPlayers);
			foreach (PlayerSecondaryData secondaryDatum in base.Request.secondaryData)
			{
				if (!SpawnedPlayers.TryGetValue(secondaryDatum.userId, out var value))
				{
					Debug.LogError((object)$"Got secondary data for {secondaryDatum.userId} but they were not spawned in the transfer");
				}
				else
				{
					value.LoadSecondaryData(secondaryDatum);
				}
			}
			if (PlayerIds.Count > 0)
			{
				Debug.Log((object)("Completing transfers for players: " + string.Join(", ", PlayerIds)));
				CompleteTransfers();
			}
			static void UpdateWithNewUid(UidType type, ref uint prevUid)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Invalid comparison between Unknown and I4
				if ((int)type == 2)
				{
					prevUid = 0u;
				}
				else if (prevUid != 0)
				{
					if (!UidMapping.TryGetValue(prevUid, out var value2))
					{
						value2 = Net.sv.TakeUID();
						UidMapping.Add(prevUid, value2);
					}
					prevUid = value2;
				}
			}
		}

		private static async void CompleteTransfers()
		{
			try
			{
				await NexusServer.ZoneClient.CompleteTransfers((IEnumerable<string>)PlayerIds);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		private void RepositionEntitiesFromTransfer()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			Entity obj = base.Request.entities[0];
			Vector3 pos = obj.baseEntity.pos;
			Quaternion val = Quaternion.Euler(obj.baseEntity.rot);
			(Vector3, Quaternion) tuple = ZoneController.Instance.ChooseTransferDestination(base.FromZone.get_Name(), base.Request.method, base.Request.from, base.Request.to, pos, val);
			Vector3 item = tuple.Item1;
			Quaternion item2 = tuple.Item2;
			Vector3 val2 = item - pos;
			Quaternion val3 = item2 * Quaternion.Inverse(val);
			foreach (Entity entity in base.Request.entities)
			{
				if (entity.baseEntity != null && (entity.parent == null || entity.parent.uid == 0))
				{
					BaseEntity baseEntity = entity.baseEntity;
					baseEntity.pos += val2;
					BaseEntity baseEntity2 = entity.baseEntity;
					Quaternion val4 = Quaternion.Euler(entity.baseEntity.rot) * val3;
					baseEntity2.rot = ((Quaternion)(ref val4)).get_eulerAngles();
				}
			}
		}

		private void SpawnEntities(Dictionary<ulong, BasePlayer> players)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			Application.isLoadingSave = true;
			try
			{
				EntityToSpawn.Clear();
				foreach (Entity entity in base.Request.entities)
				{
					BaseEntity baseEntity = GameManager.server.CreateEntity(StringPool.Get(entity.baseNetworkable.prefabID), entity.baseEntity.pos, Quaternion.Euler(entity.baseEntity.rot));
					if ((Object)(object)baseEntity != (Object)null)
					{
						baseEntity.InitLoad(entity.baseNetworkable.uid);
						EntityToSpawn.Add(baseEntity, entity);
					}
				}
				foreach (KeyValuePair<BaseEntity, Entity> item in EntityToSpawn)
				{
					BaseEntity key = item.Key;
					if (!((Object)(object)key == (Object)null))
					{
						key.Spawn();
						key.Load(new BaseNetworkable.LoadInfo
						{
							fromDisk = true,
							fromTransfer = true,
							msg = item.Value
						});
					}
				}
				foreach (KeyValuePair<BaseEntity, Entity> item2 in EntityToSpawn)
				{
					BaseEntity key2 = item2.Key;
					if (!((Object)(object)key2 == (Object)null))
					{
						key2.PostServerLoad();
						BasePlayer basePlayer;
						if ((basePlayer = key2 as BasePlayer) != null)
						{
							players[basePlayer.userID] = basePlayer;
						}
					}
				}
			}
			finally
			{
				Application.isLoadingSave = false;
			}
		}
	}
}
