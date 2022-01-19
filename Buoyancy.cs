using System;
using UnityEngine;

public class Buoyancy : ListComponent<Buoyancy>, IServerComponent
{
	private struct BuoyancyPointData
	{
		public Transform transform;

		public Vector3 localPosition;

		public Vector3 rootToPoint;

		public Vector3 position;
	}

	public BuoyancyPoint[] points;

	public GameObjectRef[] waterImpacts;

	public Rigidbody rigidBody;

	public float buoyancyScale = 1f;

	public bool doEffects = true;

	public float flowMovementScale = 1f;

	public float requiredSubmergedFraction;

	public bool useUnderwaterDrag;

	[Range(0f, 3f)]
	public float underwaterDrag = 2f;

	public Action<bool> SubmergedChanged;

	public BaseEntity forEntity;

	[NonSerialized]
	public float submergedFraction;

	private BuoyancyPointData[] pointData;

	private Vector2[] pointPositionArray;

	private Vector2[] pointPositionUVArray;

	private Vector3[] pointShoreVectorArray;

	private float[] pointTerrainHeightArray;

	private float[] pointWaterHeightArray;

	private float defaultDrag;

	private float defaultAngularDrag;

	private float timeInWater;

	public float? ArtificialHeight;

	public float waveHeightScale = 0.5f;

	public float timeOutOfWater { get; private set; }

	public static string DefaultWaterImpact()
	{
		return "assets/bundled/prefabs/fx/impacts/physics/water-enter-exit.prefab";
	}

	private void Awake()
	{
		((FacepunchBehaviour)this).InvokeRandomized((Action)CheckSleepState, 0.5f, 5f, 1f);
	}

	public void Sleep()
	{
		if ((Object)(object)rigidBody != (Object)null)
		{
			rigidBody.Sleep();
		}
		((Behaviour)this).set_enabled(false);
	}

	public void Wake()
	{
		if ((Object)(object)rigidBody != (Object)null)
		{
			rigidBody.WakeUp();
		}
		((Behaviour)this).set_enabled(true);
	}

	public void CheckSleepState()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)((Component)this).get_transform() == (Object)null) && !((Object)(object)rigidBody == (Object)null))
		{
			bool flag = BaseNetworkable.HasCloseConnections(((Component)this).get_transform().get_position(), 100f);
			if (((Behaviour)this).get_enabled() && (rigidBody.IsSleeping() || (!flag && timeInWater > 6f)))
			{
				((FacepunchBehaviour)this).Invoke((Action)Sleep, 0f);
			}
			else if (!((Behaviour)this).get_enabled() && (!rigidBody.IsSleeping() || (flag && timeInWater > 0f)))
			{
				((FacepunchBehaviour)this).Invoke((Action)Wake, 0f);
			}
		}
	}

	protected void DoCycle()
	{
		bool num = submergedFraction > 0f;
		BuoyancyFixedUpdate();
		bool flag = submergedFraction > 0f;
		if (num == flag)
		{
			return;
		}
		if (useUnderwaterDrag && (Object)(object)rigidBody != (Object)null)
		{
			if (flag)
			{
				defaultDrag = rigidBody.get_drag();
				defaultAngularDrag = rigidBody.get_angularDrag();
				rigidBody.set_drag(underwaterDrag);
				rigidBody.set_angularDrag(underwaterDrag);
			}
			else
			{
				rigidBody.set_drag(defaultDrag);
				rigidBody.set_angularDrag(defaultAngularDrag);
			}
		}
		if (SubmergedChanged != null)
		{
			SubmergedChanged(flag);
		}
	}

	public static void Cycle()
	{
		Buoyancy[] buffer = ListComponent<Buoyancy>.InstanceList.get_Values().get_Buffer();
		int count = ListComponent<Buoyancy>.InstanceList.get_Count();
		for (int i = 0; i < count; i++)
		{
			buffer[i].DoCycle();
		}
	}

	public Vector3 GetFlowDirection(Vector2 posUV)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.WaterMap == (Object)null)
		{
			return Vector3.get_zero();
		}
		Vector3 normalFast = TerrainMeta.WaterMap.GetNormalFast(posUV);
		float num = Mathf.Clamp01(Mathf.Abs(normalFast.y));
		normalFast.y = 0f;
		Vector3Ex.FastRenormalize(normalFast, num);
		return normalFast;
	}

	public void EnsurePointsInitialized()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		if (points == null || points.Length == 0)
		{
			Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
			if ((Object)(object)component != (Object)null)
			{
				GameObject val = new GameObject("BuoyancyPoint");
				val.get_transform().set_parent(((Component)component).get_gameObject().get_transform());
				val.get_transform().set_localPosition(component.get_centerOfMass());
				BuoyancyPoint buoyancyPoint = val.AddComponent<BuoyancyPoint>();
				buoyancyPoint.buoyancyForce = component.get_mass() * (0f - Physics.get_gravity().y);
				buoyancyPoint.buoyancyForce *= 1.32f;
				buoyancyPoint.size = 0.2f;
				points = new BuoyancyPoint[1];
				points[0] = buoyancyPoint;
			}
		}
		if (pointData == null || pointData.Length != points.Length)
		{
			pointData = new BuoyancyPointData[points.Length];
			pointPositionArray = (Vector2[])(object)new Vector2[points.Length];
			pointPositionUVArray = (Vector2[])(object)new Vector2[points.Length];
			pointShoreVectorArray = (Vector3[])(object)new Vector3[points.Length];
			pointTerrainHeightArray = new float[points.Length];
			pointWaterHeightArray = new float[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				Transform transform = ((Component)points[i]).get_transform();
				Transform parent = transform.get_parent();
				transform.SetParent(((Component)this).get_transform());
				Vector3 localPosition = transform.get_localPosition();
				transform.SetParent(parent);
				pointData[i].transform = transform;
				pointData[i].localPosition = transform.get_localPosition();
				pointData[i].rootToPoint = localPosition;
			}
		}
	}

	public void BuoyancyFixedUpdate()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.WaterMap == (Object)null)
		{
			return;
		}
		EnsurePointsInitialized();
		if ((Object)(object)rigidBody == (Object)null)
		{
			return;
		}
		if (buoyancyScale == 0f)
		{
			((FacepunchBehaviour)this).Invoke((Action)Sleep, 0f);
			return;
		}
		float time = Time.get_time();
		float x = TerrainMeta.Position.x;
		float z = TerrainMeta.Position.z;
		float x2 = TerrainMeta.OneOverSize.x;
		float z2 = TerrainMeta.OneOverSize.z;
		Matrix4x4 localToWorldMatrix = ((Component)this).get_transform().get_localToWorldMatrix();
		for (int i = 0; i < pointData.Length; i++)
		{
			_ = points[i];
			Vector3 val = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint3x4(pointData[i].rootToPoint);
			pointData[i].position = val;
			float num = (val.x - x) * x2;
			float num2 = (val.z - z) * z2;
			pointPositionArray[i] = new Vector2(val.x, val.z);
			pointPositionUVArray[i] = new Vector2(num, num2);
		}
		WaterSystem.GetHeightArray(pointPositionArray, pointPositionUVArray, pointShoreVectorArray, pointTerrainHeightArray, pointWaterHeightArray);
		int num3 = 0;
		Vector3 val2 = default(Vector3);
		Vector3 val3 = default(Vector3);
		for (int j = 0; j < points.Length; j++)
		{
			BuoyancyPoint buoyancyPoint = points[j];
			Vector3 position = pointData[j].position;
			Vector3 localPosition = pointData[j].localPosition;
			Vector2 posUV = pointPositionUVArray[j];
			float terrainHeight = pointTerrainHeightArray[j];
			float waterHeight = pointWaterHeightArray[j];
			if (ArtificialHeight.HasValue)
			{
				waterHeight = ArtificialHeight.Value;
			}
			bool doDeepwaterChecks = !ArtificialHeight.HasValue;
			WaterLevel.WaterInfo buoyancyWaterInfo = WaterLevel.GetBuoyancyWaterInfo(position, posUV, terrainHeight, waterHeight, doDeepwaterChecks, forEntity);
			bool flag = false;
			if (position.y < buoyancyWaterInfo.surfaceLevel && buoyancyWaterInfo.isValid)
			{
				flag = true;
				num3++;
				float currentDepth = buoyancyWaterInfo.currentDepth;
				float num4 = Mathf.InverseLerp(0f, buoyancyPoint.size, currentDepth);
				float num5 = 1f + Mathf.PerlinNoise(buoyancyPoint.randomOffset + time * buoyancyPoint.waveFrequency, 0f) * buoyancyPoint.waveScale;
				float num6 = buoyancyPoint.buoyancyForce * buoyancyScale;
				((Vector3)(ref val2))._002Ector(0f, num5 * num4 * num6, 0f);
				Vector3 flowDirection = GetFlowDirection(posUV);
				if (flowDirection.y < 0.9999f && flowDirection != Vector3.get_up())
				{
					num6 *= 0.25f;
					val2.x += flowDirection.x * num6 * flowMovementScale;
					val2.y += flowDirection.y * num6 * flowMovementScale;
					val2.z += flowDirection.z * num6 * flowMovementScale;
				}
				rigidBody.AddForceAtPosition(val2, position, (ForceMode)0);
			}
			if (buoyancyPoint.doSplashEffects && ((!buoyancyPoint.wasSubmergedLastFrame && flag) || (!flag && buoyancyPoint.wasSubmergedLastFrame)) && doEffects)
			{
				Vector3 relativePointVelocity = rigidBody.GetRelativePointVelocity(localPosition);
				if (((Vector3)(ref relativePointVelocity)).get_magnitude() > 1f)
				{
					string strName = ((waterImpacts != null && waterImpacts.Length != 0 && waterImpacts[0].isValid) ? waterImpacts[0].resourcePath : DefaultWaterImpact());
					((Vector3)(ref val3))._002Ector(Random.Range(-0.25f, 0.25f), 0f, Random.Range(-0.25f, 0.25f));
					Effect.server.Run(strName, position + val3, Vector3.get_up());
					buoyancyPoint.nexSplashTime = Time.get_time() + 0.25f;
				}
			}
			buoyancyPoint.wasSubmergedLastFrame = flag;
		}
		if (points.Length != 0)
		{
			submergedFraction = (float)num3 / (float)points.Length;
		}
		if (submergedFraction > requiredSubmergedFraction)
		{
			timeInWater += Time.get_fixedDeltaTime();
			timeOutOfWater = 0f;
		}
		else
		{
			timeOutOfWater += Time.get_fixedDeltaTime();
			timeInWater = 0f;
		}
	}
}
