using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseFishingRod : HeldEntity
{
	public class UpdateFishingRod : ObjectWorkQueue<BaseFishingRod>
	{
		protected override void RunJob(BaseFishingRod entity)
		{
			if (((ObjectWorkQueue<BaseFishingRod>)this).ShouldAdd(entity))
			{
				entity.CatchProcessBudgeted();
			}
		}

		protected override bool ShouldAdd(BaseFishingRod entity)
		{
			if (base.ShouldAdd(entity))
			{
				return entity.IsValid();
			}
			return false;
		}
	}

	public enum CatchState
	{
		None,
		Aiming,
		Waiting,
		Catching,
		Caught
	}

	[Flags]
	public enum FishState
	{
		PullingLeft = 0x1,
		PullingRight = 0x2,
		PullingBack = 0x4
	}

	public enum FailReason
	{
		UserRequested,
		BadAngle,
		TensionBreak,
		Unequipped,
		TimeOut,
		Success,
		NoWaterFound,
		Obstructed,
		NoLure,
		TooShallow,
		TooClose,
		TooFarAway,
		PlayerMoved
	}

	public static UpdateFishingRod updateFishingRodQueue = new UpdateFishingRod();

	private FishLookup fishLookup;

	private TimeUntil nextFishStateChange;

	private TimeSince fishCatchDuration;

	private float strainTimer;

	private const float strainMax = 6f;

	private TimeSince lastStrainUpdate;

	private TimeUntil catchTime;

	private TimeSince lastSightCheck;

	private Vector3 playerStartPosition;

	private WaterBody surfaceBody;

	private ItemDefinition lureUsed;

	private ItemDefinition currentFishTarget;

	private ItemModFishable fishableModifier;

	private ItemModFishable lastFish;

	private bool inQueue;

	[ServerVar]
	public static bool ForceSuccess = false;

	[ServerVar]
	public static bool ForceFail = false;

	[ServerVar]
	public static bool ImmediateHook = false;

	public GameObjectRef FishingBobberRef;

	public float FishCatchDistance = 0.5f;

	public LineRenderer ReelLineRenderer;

	public Transform LineRendererWorldStartPos;

	private FishState currentFishState;

	private EntityRef<FishingBobber> currentBobber;

	public float ConditionLossOnSuccess = 0.02f;

	public float ConditionLossOnFail = 0.04f;

	public float GlobalStrainSpeedMultiplier = 1f;

	public float MaxCastDistance = 10f;

	public const Flags Straining = Flags.Reserved1;

	public ItemModFishable ForceFish;

	public static Flags PullingLeftFlag = Flags.Reserved6;

	public static Flags PullingRightFlag = Flags.Reserved7;

	public static Flags ReelingInFlag = Flags.Reserved8;

	public GameObjectRef BobberPreview;

	public SoundDefinition onLineSoundDef;

	public SoundDefinition strainSoundDef;

	public AnimationCurve strainGainCurve;

	public SoundDefinition tensionBreakSoundDef;

	public CatchState CurrentState { get; private set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseFishingRod.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 4237324865u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_Cancel "));
				}
				TimeWarning val2 = TimeWarning.New("Server_Cancel", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(4237324865u, "Server_Cancel", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg2 = rPCMessage;
							Server_Cancel(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_Cancel");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4238539495u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_RequestCast "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RequestCast", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(4238539495u, "Server_RequestCast", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg3 = rPCMessage;
							Server_RequestCast(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Server_RequestCast");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void Server_RequestCast(RPCMessage msg)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pos = msg.read.Vector3();
		BasePlayer ownerPlayer = GetOwnerPlayer();
		Item currentLure = GetCurrentLure();
		if (currentLure == null)
		{
			FailedCast(FailReason.NoLure);
			return;
		}
		if (!EvaluateFishingPosition(ref pos, ownerPlayer, out var reason, out surfaceBody))
		{
			FailedCast(reason);
			return;
		}
		FishingBobber component = ((Component)base.gameManager.CreateEntity(FishingBobberRef.resourcePath, ((Component)this).get_transform().get_position() + Vector3.get_up() * 2.8f + ownerPlayer.eyes.BodyForward() * 1.8f, GetOwnerPlayer().ServerRotation)).GetComponent<FishingBobber>();
		((Component)component).get_transform().set_forward(GetOwnerPlayer().eyes.BodyForward());
		component.Spawn();
		component.InitialiseBobber(ownerPlayer, surfaceBody, pos);
		lureUsed = currentLure.info;
		currentLure.UseItem();
		if (fishLookup == null)
		{
			fishLookup = PrefabAttribute.server.Find<FishLookup>(prefabID);
		}
		currentFishTarget = fishLookup.GetFish(((Component)component).get_transform().get_position(), surfaceBody, lureUsed, out fishableModifier, lastFish);
		lastFish = fishableModifier;
		currentBobber.Set(component);
		ClientRPC(null, "Client_ReceiveCastPoint", component.net.ID);
		ownerPlayer.SignalBroadcast(Signal.Attack);
		catchTime = TimeUntil.op_Implicit(ImmediateHook ? 0f : Random.Range(10f, 20f));
		catchTime = TimeUntil.op_Implicit(TimeUntil.op_Implicit(catchTime) * fishableModifier.CatchWaitTimeMultiplier);
		ItemModCompostable itemModCompostable = default(ItemModCompostable);
		float num = (((Component)lureUsed).TryGetComponent<ItemModCompostable>(ref itemModCompostable) ? itemModCompostable.BaitValue : 0f);
		num = Mathx.RemapValClamped(num, 0f, 20f, 1f, 10f);
		catchTime = TimeUntil.op_Implicit(Mathf.Clamp(TimeUntil.op_Implicit(catchTime) - num, 3f, 20f));
		playerStartPosition = ((Component)ownerPlayer).get_transform().get_position();
		SetFlag(Flags.Busy, b: true);
		CurrentState = CatchState.Waiting;
		((FacepunchBehaviour)this).InvokeRepeating((Action)CatchProcess, 0f, 0f);
		inQueue = false;
	}

	private void FailedCast(FailReason reason)
	{
		CurrentState = CatchState.None;
		ClientRPC(null, "Client_ResetLine", (int)reason);
	}

	private void CatchProcess()
	{
		if (!inQueue)
		{
			inQueue = true;
			((ObjectWorkQueue<BaseFishingRod>)updateFishingRodQueue).Add(this);
		}
	}

	private void CatchProcessBudgeted()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		inQueue = false;
		FishingBobber fishingBobber = currentBobber.Get(serverside: true);
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if ((Object)(object)ownerPlayer == (Object)null || ownerPlayer.IsSleeping() || ownerPlayer.IsWounded() || ownerPlayer.IsDead() || (Object)(object)fishingBobber == (Object)null)
		{
			Server_Cancel(FailReason.UserRequested);
			return;
		}
		Vector3 position = ((Component)ownerPlayer).get_transform().get_position();
		Vector3 val = Vector3Ex.WithY(((Component)fishingBobber).get_transform().get_position(), 0f) - Vector3Ex.WithY(position, 0f);
		float num = Vector3.Angle(((Vector3)(ref val)).get_normalized(), Vector3Ex.WithY(ownerPlayer.eyes.HeadForward(), 0f));
		float num2 = Vector3.Distance(position, Vector3Ex.WithY(((Component)fishingBobber).get_transform().get_position(), position.y));
		if (num > ((num2 > 1.2f) ? 60f : 180f))
		{
			Server_Cancel(FailReason.BadAngle);
			return;
		}
		if (num2 > 1.2f && TimeSince.op_Implicit(lastSightCheck) > 0.4f)
		{
			if (!GamePhysics.LineOfSight(ownerPlayer.eyes.position, ((Component)fishingBobber).get_transform().get_position(), 1218511105))
			{
				Server_Cancel(FailReason.Obstructed);
				return;
			}
			lastSightCheck = TimeSince.op_Implicit(0f);
		}
		if (Vector3.Distance(position, ((Component)fishingBobber).get_transform().get_position()) > MaxCastDistance * 2f)
		{
			Server_Cancel(FailReason.TooFarAway);
			return;
		}
		if (Vector3.Distance(playerStartPosition, position) > 1f)
		{
			Server_Cancel(FailReason.PlayerMoved);
			return;
		}
		if (CurrentState == CatchState.Waiting)
		{
			if (TimeUntil.op_Implicit(catchTime) < 0f)
			{
				ClientRPC(null, "Client_HookedSomething");
				CurrentState = CatchState.Catching;
				fishingBobber.SetFlag(Flags.Reserved1, b: true);
				nextFishStateChange = TimeUntil.op_Implicit(0f);
				fishCatchDuration = TimeSince.op_Implicit(0f);
				strainTimer = 0f;
			}
			return;
		}
		FishState fishState = currentFishState;
		if (TimeUntil.op_Implicit(nextFishStateChange) < 0f)
		{
			float num3 = Mathx.RemapValClamped(fishingBobber.TireAmount, 0f, 20f, 0f, 1f);
			if (currentFishState != 0)
			{
				currentFishState = (FishState)0;
				nextFishStateChange = TimeUntil.op_Implicit(Random.Range(2f, 4f) * (num3 + 1f));
			}
			else
			{
				nextFishStateChange = TimeUntil.op_Implicit(Random.Range(3f, 7f) * (1f - num3));
				if (Random.Range(0, 100) < 50)
				{
					currentFishState = FishState.PullingLeft;
				}
				else
				{
					currentFishState = FishState.PullingRight;
				}
				if (Random.Range(0, 100) > 60 && Vector3.Distance(((Component)fishingBobber).get_transform().get_position(), ((Component)ownerPlayer).get_transform().get_position()) < MaxCastDistance - 2f)
				{
					currentFishState |= FishState.PullingBack;
				}
			}
		}
		if (TimeSince.op_Implicit(fishCatchDuration) > 120f)
		{
			Server_Cancel(FailReason.TimeOut);
			return;
		}
		bool flag = ownerPlayer.serverInput.IsDown(BUTTON.RIGHT);
		bool flag2 = ownerPlayer.serverInput.IsDown(BUTTON.LEFT);
		bool flag3 = HasReelInInput(ownerPlayer.serverInput);
		if (flag2 && flag)
		{
			flag2 = (flag = false);
		}
		UpdateFlags(flag2, flag, flag3);
		if (CurrentState == CatchState.Waiting)
		{
			flag = (flag2 = (flag3 = false));
		}
		if (flag2 && !AllowPullInDirection(-ownerPlayer.eyes.HeadRight(), ((Component)fishingBobber).get_transform().get_position()))
		{
			flag2 = false;
		}
		if (flag && !AllowPullInDirection(ownerPlayer.eyes.HeadRight(), ((Component)fishingBobber).get_transform().get_position()))
		{
			flag = false;
		}
		fishingBobber.ServerMovementUpdate(flag2, flag, flag3, ref currentFishState, position, fishableModifier);
		bool flag4 = false;
		float num4 = 0f;
		if (flag3 || flag2 || flag)
		{
			flag4 = true;
			num4 = 0.5f;
		}
		if (currentFishState != 0 && flag4)
		{
			if (currentFishState.Contains(FishState.PullingBack) && flag3)
			{
				num4 = 1.5f;
			}
			else if ((currentFishState.Contains(FishState.PullingLeft) || currentFishState.Contains(FishState.PullingRight)) && flag3)
			{
				num4 = 1.2f;
			}
			else if (currentFishState.Contains(FishState.PullingLeft) && flag)
			{
				num4 = 0.8f;
			}
			else if (currentFishState.Contains(FishState.PullingRight) && flag2)
			{
				num4 = 0.8f;
			}
		}
		if (flag3 && currentFishState != 0)
		{
			num4 += 1f;
		}
		num4 *= fishableModifier.StrainModifier * GlobalStrainSpeedMultiplier;
		if (flag4)
		{
			strainTimer += Time.get_deltaTime() * num4;
		}
		else
		{
			strainTimer = Mathf.MoveTowards(strainTimer, 0f, Time.get_deltaTime() * 1.5f);
		}
		float num5 = strainTimer / 6f;
		SetFlag(Flags.Reserved1, flag4 && num5 > 0.25f);
		if (TimeSince.op_Implicit(lastStrainUpdate) > 0.4f || fishState != currentFishState)
		{
			ClientRPC(null, "Client_UpdateFishState", (int)currentFishState, num5);
			lastStrainUpdate = TimeSince.op_Implicit(0f);
		}
		if (strainTimer > 7f || ForceFail)
		{
			Server_Cancel(FailReason.TensionBreak);
		}
		else
		{
			if (!(num2 <= FishCatchDistance) && !ForceSuccess)
			{
				return;
			}
			CurrentState = CatchState.Caught;
			if ((Object)(object)currentFishTarget != (Object)null)
			{
				Item item = ItemManager.Create(currentFishTarget, 1, 0uL);
				ownerPlayer.GiveItem(item, GiveItemReason.Crafted);
				if (currentFishTarget.shortname == "skull.human")
				{
					item.name = RandomUsernames.Get(Random.Range(0, 1000));
				}
			}
			ClientRPC(null, "Client_OnCaughtFish", currentFishTarget.itemid);
			ownerPlayer.SignalBroadcast(Signal.Alt_Attack);
			((FacepunchBehaviour)this).Invoke((Action)ResetLine, 6f);
			fishingBobber.Kill();
			currentBobber.Set(null);
			((FacepunchBehaviour)this).CancelInvoke((Action)CatchProcess);
		}
	}

	private void ResetLine()
	{
		Server_Cancel(FailReason.Success);
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void Server_Cancel(RPCMessage msg)
	{
		if (CurrentState != CatchState.Caught)
		{
			Server_Cancel(FailReason.UserRequested);
		}
	}

	private void Server_Cancel(FailReason reason)
	{
		if (GetItem() != null)
		{
			GetItem().LoseCondition((reason == FailReason.Success) ? ConditionLossOnSuccess : ConditionLossOnFail);
		}
		SetFlag(Flags.Busy, b: false);
		UpdateFlags();
		((FacepunchBehaviour)this).CancelInvoke((Action)CatchProcess);
		CurrentState = CatchState.None;
		SetFlag(Flags.Reserved1, b: false);
		FishingBobber fishingBobber = currentBobber.Get(serverside: true);
		if ((Object)(object)fishingBobber != (Object)null)
		{
			fishingBobber.Kill();
			currentBobber.Set(null);
		}
		ClientRPC(null, "Client_ResetLine", (int)reason);
	}

	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (CurrentState != 0)
		{
			Server_Cancel(FailReason.Unequipped);
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (currentBobber.IsSet && info.msg.simpleUID == null)
		{
			info.msg.simpleUID = Pool.Get<SimpleUID>();
			info.msg.simpleUID.uid = currentBobber.uid;
		}
	}

	private void UpdateFlags(bool inputLeft = false, bool inputRight = false, bool back = false)
	{
		SetFlag(PullingLeftFlag, CurrentState == CatchState.Catching && inputLeft);
		SetFlag(PullingRightFlag, CurrentState == CatchState.Catching && inputRight);
		SetFlag(ReelingInFlag, CurrentState == CatchState.Catching && back);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if ((!base.isServer || !info.fromDisk) && info.msg.simpleUID != null)
		{
			currentBobber.uid = info.msg.simpleUID.uid;
		}
	}

	public override bool BlocksGestures()
	{
		return CurrentState != CatchState.None;
	}

	private bool AllowPullInDirection(Vector3 worldDirection, Vector3 bobberPosition)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).get_transform().get_position();
		Vector3 val = Vector3Ex.WithY(bobberPosition, position.y);
		Vector3 val2 = val - position;
		return Vector3.Dot(worldDirection, ((Vector3)(ref val2)).get_normalized()) < 0f;
	}

	private bool EvaluateFishingPosition(ref Vector3 pos, BasePlayer ply, out FailReason reason, out WaterBody waterBody)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		waterBody = null;
		bool flag = false;
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, 1f, list, 16, (QueryTriggerInteraction)2);
		if (list.Count > 0)
		{
			foreach (Collider item in list)
			{
				((Component)item).TryGetComponent<WaterBody>(ref waterBody);
				if ((Object)(object)waterBody == (Object)null)
				{
					waterBody = ((Component)((Component)item).get_transform().get_parent()).GetComponentInChildren<WaterBody>();
				}
				if (!((Object)(object)waterBody != (Object)null) || waterBody.FishingType == (WaterBody.FishingTag)0)
				{
					continue;
				}
				flag = true;
				if (GamePhysics.Trace(new Ray(pos + Vector3.get_up(), -Vector3.get_up()), 0f, out var hitInfo, 1.5f, 16, (QueryTriggerInteraction)2))
				{
					pos.y = ((RaycastHit)(ref hitInfo)).get_point().y;
				}
				else
				{
					pos.y = ((Component)list[0]).get_transform().get_position().y;
				}
				if (!waterBody.IsOcean)
				{
					if ((Object)(object)waterBody.Renderer != (Object)null && (waterBody.FishingType & WaterBody.FishingTag.MoonPool) == WaterBody.FishingTag.MoonPool)
					{
						pos.y = ((Component)waterBody.Renderer).get_transform().get_position().y;
					}
					break;
				}
			}
		}
		Pool.FreeList<Collider>(ref list);
		if (!flag)
		{
			reason = FailReason.NoWaterFound;
			return false;
		}
		if (Vector3.Distance(Vector3Ex.WithY(((Component)ply).get_transform().get_position(), pos.y), pos) < 5f)
		{
			reason = FailReason.TooClose;
			return false;
		}
		if (!GamePhysics.LineOfSight(ply.eyes.position, pos, 1218652417))
		{
			reason = FailReason.Obstructed;
			return false;
		}
		Vector3 p = pos + Vector3.get_up() * 2f;
		if (!GamePhysics.LineOfSight(ply.eyes.position, p, 1218652417))
		{
			reason = FailReason.Obstructed;
			return false;
		}
		Vector3 position = ((Component)ply).get_transform().get_position();
		position.y = pos.y;
		float num = Vector3.Distance(pos, position);
		Vector3 val = pos;
		Vector3 val2 = position - pos;
		Vector3 p2 = val + ((Vector3)(ref val2)).get_normalized() * (num - FishCatchDistance);
		if (!GamePhysics.LineOfSight(pos, p2, 1218652417))
		{
			reason = FailReason.Obstructed;
			return false;
		}
		if (WaterLevel.GetOverallWaterDepth(Vector3.Lerp(pos, Vector3Ex.WithY(((Component)ply).get_transform().get_position(), pos.y), 0.95f), waves: true, null, noEarlyExit: true) < 0.1f && ply.eyes.position.y > 0f)
		{
			reason = FailReason.TooShallow;
			return false;
		}
		if (WaterLevel.GetOverallWaterDepth(pos, waves: true, null, noEarlyExit: true) < 0.3f && ply.eyes.position.y > 0f)
		{
			reason = FailReason.TooShallow;
			return false;
		}
		Vector3 p3 = Vector3.MoveTowards(Vector3Ex.WithY(((Component)ply).get_transform().get_position(), pos.y), pos, 1f);
		if (!GamePhysics.LineOfSight(ply.eyes.position, p3, 1218652417))
		{
			reason = FailReason.Obstructed;
			return false;
		}
		reason = FailReason.Success;
		return true;
	}

	private Item GetCurrentLure()
	{
		if (GetItem() == null)
		{
			return null;
		}
		if (GetItem().contents == null)
		{
			return null;
		}
		return GetItem().contents.GetSlot(0);
	}

	private bool HasReelInInput(InputState state)
	{
		if (!state.IsDown(BUTTON.BACKWARD))
		{
			return state.IsDown(BUTTON.FIRE_PRIMARY);
		}
		return true;
	}
}
