using Facepunch;
using ProtoBuf;
using UnityEngine;

public class VehicleVendor : NPCTalking
{
	public EntityRef spawnerRef;

	public VehicleSpawner vehicleSpawner;

	public override string GetConversationStartSpeech(BasePlayer player)
	{
		if (ProviderBusy())
		{
			return "startbusy";
		}
		return "intro";
	}

	public VehicleSpawner GetVehicleSpawner()
	{
		if (!spawnerRef.IsValid(base.isServer))
		{
			return null;
		}
		return ((Component)spawnerRef.Get(base.isServer)).GetComponent<VehicleSpawner>();
	}

	public override void UpdateFlags()
	{
		base.UpdateFlags();
		VehicleSpawner vehicleSpawner = GetVehicleSpawner();
		bool b = (Object)(object)vehicleSpawner != (Object)null && vehicleSpawner.IsPadOccupied();
		SetFlag(Flags.Reserved1, b);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (spawnerRef.IsValid(serverside: true) && (Object)(object)vehicleSpawner == (Object)null)
		{
			vehicleSpawner = GetVehicleSpawner();
		}
		else if ((Object)(object)vehicleSpawner != (Object)null && !spawnerRef.IsValid(serverside: true))
		{
			spawnerRef.Set(vehicleSpawner);
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.vehicleVendor = Pool.Get<VehicleVendor>();
		info.msg.vehicleVendor.spawnerRef = spawnerRef.uid;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.vehicleVendor != null)
		{
			spawnerRef.id_cached = info.msg.vehicleVendor.spawnerRef;
		}
	}

	public override ConversationData GetConversationFor(BasePlayer player)
	{
		return conversations[0];
	}
}
