using System;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

public class TrainBarricade : BaseCombatEntity, ITrainCollidable, TrainTrackSpline.ITrainTrackUser
{
	[FormerlySerializedAs("damagePerMPS")]
	[SerializeField]
	private float trainDamagePerMPS = 10f;

	[SerializeField]
	private float minVelToDestroy = 6f;

	[SerializeField]
	private float velReduction = 2f;

	[SerializeField]
	private GameObjectRef barricadeDamageEffect;

	private BaseTrain hitTrain;

	private TriggerTrainCollisions hitTrainTrigger;

	private TrainTrackSpline track;

	public Vector3 Position => ((Component)this).get_transform().get_position();

	public float FrontWheelSplineDist { get; private set; }

	public bool CustomCollision(BaseTrain train, TriggerTrainCollisions trainTrigger)
	{
		bool result = false;
		if (base.isServer)
		{
			float num = Mathf.Abs(train.TrackSpeed);
			SetHitTrain(train, trainTrigger);
			if (num < minVelToDestroy && !vehicle.cinematictrains)
			{
				((FacepunchBehaviour)this).InvokeRandomized((Action)PushForceTick, 0f, 0.25f, 0.025f);
			}
			else
			{
				result = true;
				((FacepunchBehaviour)this).Invoke((Action)DestroyThisBarrier, 0f);
			}
		}
		return result;
	}

	public override void ServerInit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (TrainTrackSpline.TryFindTrackNearby(((Component)this).get_transform().get_position(), 3f, out var splineResult, out var distResult))
		{
			track = splineResult;
			FrontWheelSplineDist = distResult;
			track.RegisterTrackUser(this);
		}
	}

	internal override void DoServerDestroy()
	{
		if ((Object)(object)track != (Object)null)
		{
			track.DeregisterTrackUser(this);
		}
		base.DoServerDestroy();
	}

	private void SetHitTrain(BaseTrain train, TriggerTrainCollisions trainTrigger)
	{
		hitTrain = train;
		hitTrainTrigger = trainTrigger;
	}

	private void ClearHitTrain()
	{
		SetHitTrain(null, null);
	}

	private void DestroyThisBarrier()
	{
		if (IsDead() || base.IsDestroyed)
		{
			return;
		}
		if ((Object)(object)hitTrain != (Object)null)
		{
			hitTrain.ReduceSpeedBy(velReduction);
			if (vehicle.cinematictrains)
			{
				hitTrain.Hurt(9999f, DamageType.Collision, this, useProtection: false);
			}
			else
			{
				float amount = Mathf.Abs(hitTrain.TrackSpeed) * trainDamagePerMPS;
				hitTrain.Hurt(amount, DamageType.Collision, this, useProtection: false);
			}
		}
		ClearHitTrain();
		Kill(DestroyMode.Gib);
	}

	private void PushForceTick()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)hitTrain == (Object)null || (Object)(object)hitTrainTrigger == (Object)null || hitTrain.IsDead() || hitTrain.IsDestroyed || IsDead())
		{
			ClearHitTrain();
			((FacepunchBehaviour)this).CancelInvoke((Action)PushForceTick);
			return;
		}
		bool flag = true;
		Bounds val = hitTrainTrigger.triggerCollider.get_bounds();
		if (!((Bounds)(ref val)).Intersects(bounds))
		{
			Vector3 val2 = ((hitTrainTrigger.location != 0) ? hitTrainTrigger.owner.GetRearOfTrainPos() : hitTrainTrigger.owner.GetFrontOfTrainPos());
			Vector3 val3 = ((Component)this).get_transform().get_position() + ((Bounds)(ref bounds)).ClosestPoint(val2 - ((Component)this).get_transform().get_position());
			Debug.DrawRay(val3, Vector3.get_up(), Color.get_red(), 10f);
			flag = Vector3.SqrMagnitude(val3 - val2) < 1f;
		}
		if (flag)
		{
			float num = hitTrainTrigger.owner.GetEngineForces();
			if (hitTrainTrigger.location == TriggerTrainCollisions.ColliderLocation.Rear)
			{
				num *= -1f;
			}
			num = Mathf.Max(0f, num);
			Hurt(0.002f * num);
			if (IsDead())
			{
				hitTrain.FreeStaticCollision();
			}
		}
		else
		{
			ClearHitTrain();
			((FacepunchBehaviour)this).CancelInvoke((Action)PushForceTick);
		}
	}
}
