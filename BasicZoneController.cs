using Facepunch.Nexus;

public class BasicZoneController : ZoneController
{
	public BasicZoneController(NexusZoneClient zoneClient)
		: base(zoneClient)
	{
	}

	public override string ChooseSpawnZone(ulong steamId, bool isAlreadyAssignedToThisZone)
	{
		return ZoneClient.get_Zone().get_Name();
	}
}
