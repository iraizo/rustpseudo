using System;
using UnityEngine;

public class DevMovePlayer : BaseMonoBehaviour
{
	public BasePlayer player;

	public Transform[] Waypoints;

	public bool moveRandomly;

	public Vector3 destination = Vector3.get_zero();

	public Vector3 lookPoint = Vector3.get_zero();

	private int waypointIndex;

	private float randRun;

	public void Awake()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		randRun = Random.Range(5f, 10f);
		player = ((Component)this).GetComponent<BasePlayer>();
		if (Waypoints.Length != 0)
		{
			destination = Waypoints[0].get_position();
		}
		else
		{
			destination = ((Component)this).get_transform().get_position();
		}
		if (!player.isClient)
		{
			if ((Object)(object)player.eyes == (Object)null)
			{
				player.eyes = ((Component)player).GetComponent<PlayerEyes>();
			}
			((FacepunchBehaviour)this).Invoke((Action)LateSpawn, 1f);
		}
	}

	public void LateSpawn()
	{
		Item item = ItemManager.CreateByName("rifle.semiauto", 1, 0uL);
		player.inventory.GiveItem(item, player.inventory.containerBelt);
		player.UpdateActiveItem(item.uid);
		player.health = 100f;
	}

	public void SetWaypoints(Transform[] wps)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Waypoints = wps;
		destination = wps[0].get_position();
	}

	public void Update()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		if (player.isClient || !player.IsAlive() || player.IsWounded())
		{
			return;
		}
		if (Vector3.Distance(destination, ((Component)this).get_transform().get_position()) < 0.25f)
		{
			if (moveRandomly)
			{
				waypointIndex = Random.Range(0, Waypoints.Length);
			}
			else
			{
				waypointIndex++;
			}
			if (waypointIndex >= Waypoints.Length)
			{
				waypointIndex = 0;
			}
		}
		if (Waypoints.Length > waypointIndex)
		{
			destination = Waypoints[waypointIndex].get_position();
			Vector3 val = destination - ((Component)this).get_transform().get_position();
			Vector3 normalized = ((Vector3)(ref val)).get_normalized();
			float running = Mathf.Sin(Time.get_time() + randRun);
			float speed = player.GetSpeed(running, 0f, 0f);
			Vector3 position = ((Component)this).get_transform().get_position();
			float range = 1f;
			LayerMask mask = LayerMask.op_Implicit(1537286401);
			if (TransformUtil.GetGroundInfo(((Component)this).get_transform().get_position() + normalized * speed * Time.get_deltaTime(), out var hitOut, range, mask, ((Component)player).get_transform()))
			{
				position = ((RaycastHit)(ref hitOut)).get_point();
			}
			((Component)this).get_transform().set_position(position);
			val = new Vector3(destination.x, 0f, destination.z) - new Vector3(((Component)player).get_transform().get_position().x, 0f, ((Component)player).get_transform().get_position().z);
			((Vector3)(ref val)).get_normalized();
			player.SendNetworkUpdate();
		}
	}
}
