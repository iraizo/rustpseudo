using System;
using UnityEngine;

public class TriggerParent : TriggerBase, IServerComponent
{
	[Tooltip("Deparent if the parented entity clips into an obstacle")]
	[SerializeField]
	private bool doClippingCheck;

	[Tooltip("If deparenting via clipping, this will be used (if assigned) to also move the entity to a valid dismount position")]
	public BaseMountable associatedMountable;

	[Tooltip("Needed if the player might dismount inside the trigger and the trigger might be moving. Being mounting inside the trigger lets them dismount in local trigger-space, which means client and server will sync up.Otherwise the client/server delay can have them dismounting into invalid space.")]
	public bool parentMountedPlayers;

	public bool ParentNPCPlayers;

	[Tooltip("If the player is already parented to something else, they'll switch over to another parent only if this is true")]
	public bool overrideOtherTriggers;

	public const int CLIP_CHECK_MASK = 1218511105;

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return ((Component)baseEntity).get_gameObject();
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		if (!(ent is NPCPlayer) || ParentNPCPlayers)
		{
			if (ShouldParent(ent))
			{
				Parent(ent);
			}
			base.OnEntityEnter(ent);
			if (entityContents != null && entityContents.Count == 1)
			{
				((FacepunchBehaviour)this).InvokeRepeating((Action)OnTick, 0f, 0f);
			}
		}
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (entityContents == null || entityContents.Count == 0)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)OnTick);
		}
		BasePlayer basePlayer = ent.ToPlayer();
		if (!((Object)(object)basePlayer != (Object)null) || !basePlayer.IsSleeping())
		{
			Unparent(ent);
		}
	}

	protected virtual bool ShouldParent(BaseEntity ent)
	{
		BaseEntity parentEntity = ent.GetParentEntity();
		if (!overrideOtherTriggers && parentEntity.IsValid() && (Object)(object)parentEntity != (Object)(object)((Component)this).get_gameObject().ToBaseEntity())
		{
			return false;
		}
		if ((Object)(object)ent.FindTrigger<TriggerParentExclusion>() != (Object)null)
		{
			return false;
		}
		if (doClippingCheck && IsClipping(ent))
		{
			return false;
		}
		if (!parentMountedPlayers)
		{
			BasePlayer basePlayer = ent.ToPlayer();
			if ((Object)(object)basePlayer != (Object)null && basePlayer.isMounted)
			{
				return false;
			}
		}
		return true;
	}

	protected void Parent(BaseEntity ent)
	{
		if (!((Object)(object)ent.GetParentEntity() == (Object)(object)((Component)this).get_gameObject().ToBaseEntity()))
		{
			ent.SetParent(((Component)this).get_gameObject().ToBaseEntity(), worldPositionStays: true, sendImmediate: true);
		}
	}

	protected void Unparent(BaseEntity ent)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)ent.GetParentEntity() != (Object)(object)((Component)this).get_gameObject().ToBaseEntity()))
		{
			ent.SetParent(null, worldPositionStays: true, sendImmediate: true);
			BasePlayer basePlayer = ent.ToPlayer();
			if ((Object)(object)basePlayer != (Object)null)
			{
				basePlayer.PauseFlyHackDetection(5f);
				basePlayer.PauseSpeedHackDetection(5f);
				basePlayer.PauseVehicleNoClipDetection(5f);
			}
			BasePlayer basePlayer2;
			if ((Object)(object)associatedMountable != (Object)null && doClippingCheck && IsClipping(ent) && (basePlayer2 = ent as BasePlayer) != null && associatedMountable.GetDismountPosition(basePlayer2, out var res))
			{
				basePlayer2.MovePosition(res);
				basePlayer2.SendNetworkUpdateImmediate();
				basePlayer2.ClientRPCPlayer<Vector3>(null, basePlayer2, "ForcePositionTo", res);
			}
		}
	}

	private void OnTick()
	{
		if (entityContents == null)
		{
			return;
		}
		BaseEntity baseEntity = ((Component)this).get_gameObject().ToBaseEntity();
		if (!baseEntity.IsValid() || baseEntity.IsDestroyed)
		{
			return;
		}
		foreach (BaseEntity entityContent in entityContents)
		{
			if (entityContent.IsValid() && !entityContent.IsDestroyed)
			{
				if (ShouldParent(entityContent))
				{
					Parent(entityContent);
				}
				else
				{
					Unparent(entityContent);
				}
			}
		}
	}

	private bool IsClipping(BaseEntity ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GamePhysics.CheckOBB(ent.WorldSpaceBounds(), 1218511105, (QueryTriggerInteraction)1);
	}
}
