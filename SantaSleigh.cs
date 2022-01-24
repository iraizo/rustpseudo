using System;
using Network;
using UnityEngine;

public class SantaSleigh : BaseEntity
{
	public GameObjectRef prefabDrop;

	public SpawnFilter filter;

	public Transform dropOrigin;

	[ServerVar]
	public static float altitudeAboveTerrain = 50f;

	[ServerVar]
	public static float desiredAltitude = 60f;

	public Light bigLight;

	public SoundPlayer hohoho;

	public float hohohospacing = 4f;

	public float hohoho_additional_spacing = 2f;

	private Vector3 startPos;

	private Vector3 endPos;

	private float secondsToTake;

	private float secondsTaken;

	private bool dropped;

	private Vector3 dropPosition = Vector3.get_zero();

	public Vector3 swimScale;

	public Vector3 swimSpeed;

	private float swimRandom;

	public float appliedSwimScale = 1f;

	public float appliedSwimRotation = 20f;

	private const string path = "assets/prefabs/misc/xmas/sleigh/santasleigh.prefab";

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SantaSleigh.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override float GetNetworkTime()
	{
		return Time.get_fixedTime();
	}

	public void InitDropPosition(Vector3 newDropPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		dropPosition = newDropPosition;
		dropPosition.y = 0f;
	}

	public override void ServerInit()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (dropPosition == Vector3.get_zero())
		{
			dropPosition = RandomDropPosition();
		}
		UpdateDropPosition(dropPosition);
		((FacepunchBehaviour)this).Invoke((Action)SendHoHoHo, 0f);
	}

	public void SendHoHoHo()
	{
		((FacepunchBehaviour)this).Invoke((Action)SendHoHoHo, hohohospacing + Random.Range(0f, hohoho_additional_spacing));
		ClientRPC(null, "ClientPlayHoHoHo");
	}

	public Vector3 RandomDropPosition()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.get_zero();
		float num = 100f;
		float x = TerrainMeta.Size.x;
		do
		{
			val = Vector3Ex.Range(0f - x / 3f, x / 3f);
		}
		while (filter.GetFactor(val) == 0f && (num -= 1f) > 0f);
		val.y = 0f;
		return val;
	}

	public void UpdateDropPosition(Vector3 newDropPosition)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		float x = TerrainMeta.Size.x;
		float y = altitudeAboveTerrain;
		startPos = Vector3Ex.Range(-1f, 1f);
		startPos.y = 0f;
		((Vector3)(ref startPos)).Normalize();
		startPos *= x * 1.25f;
		startPos.y = y;
		endPos = startPos * -1f;
		endPos.y = startPos.y;
		startPos += newDropPosition;
		endPos += newDropPosition;
		secondsToTake = Vector3.Distance(startPos, endPos) / 25f;
		secondsToTake *= Random.Range(0.95f, 1.05f);
		((Component)this).get_transform().SetPositionAndRotation(startPos, Quaternion.LookRotation(endPos - startPos));
		dropPosition = newDropPosition;
	}

	private void FixedUpdate()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer)
		{
			return;
		}
		Vector3 position = ((Component)this).get_transform().get_position();
		Quaternion rotation = ((Component)this).get_transform().get_rotation();
		secondsTaken += Time.get_deltaTime();
		float num = Mathf.InverseLerp(0f, secondsToTake, secondsTaken);
		if (!dropped && num >= 0.5f)
		{
			dropped = true;
			BaseEntity baseEntity = GameManager.server.CreateEntity(prefabDrop.resourcePath, ((Component)dropOrigin).get_transform().get_position());
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.globalBroadcast = true;
				baseEntity.Spawn();
			}
		}
		position = Vector3.Lerp(startPos, endPos, num);
		Vector3 val = endPos - startPos;
		Vector3 normalized = ((Vector3)(ref val)).get_normalized();
		Vector3 val2 = Vector3.get_zero();
		if (swimScale != Vector3.get_zero())
		{
			if (swimRandom == 0f)
			{
				swimRandom = Random.Range(0f, 20f);
			}
			float num2 = Time.get_time() + swimRandom;
			((Vector3)(ref val2))._002Ector(Mathf.Sin(num2 * swimSpeed.x) * swimScale.x, Mathf.Cos(num2 * swimSpeed.y) * swimScale.y, Mathf.Sin(num2 * swimSpeed.z) * swimScale.z);
			val2 = ((Component)this).get_transform().InverseTransformDirection(val2);
			position += val2 * appliedSwimScale;
		}
		rotation = Quaternion.LookRotation(normalized) * Quaternion.Euler(Mathf.Cos(Time.get_time() * swimSpeed.y) * appliedSwimRotation, 0f, Mathf.Sin(Time.get_time() * swimSpeed.x) * appliedSwimRotation);
		Vector3 val3 = position;
		float height = TerrainMeta.HeightMap.GetHeight(val3 + ((Component)this).get_transform().get_forward() * 30f);
		float height2 = TerrainMeta.HeightMap.GetHeight(val3);
		float num3 = Mathf.Max(height, height2);
		val3.y = Mathf.Max(desiredAltitude, num3 + altitudeAboveTerrain);
		position = val3;
		((Component)this).get_transform().set_hasChanged(true);
		if (num >= 1f)
		{
			Kill();
		}
		((Component)this).get_transform().SetPositionAndRotation(position, rotation);
	}

	[ServerVar]
	public static void drop(Arg arg)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			Debug.Log((object)"Santa Inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/misc/xmas/sleigh/santasleigh.prefab");
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				((Component)baseEntity).GetComponent<SantaSleigh>().InitDropPosition(((Component)basePlayer).get_transform().get_position() + new Vector3(0f, 10f, 0f));
				baseEntity.Spawn();
			}
		}
	}
}
