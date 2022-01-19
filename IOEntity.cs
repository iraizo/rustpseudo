using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class IOEntity : BaseCombatEntity
{
	public enum IOType
	{
		Electric,
		Fluidic,
		Kinetic,
		Generic
	}

	[Serializable]
	public class IORef
	{
		public EntityRef entityRef;

		public IOEntity ioEnt;

		public void Init()
		{
			if ((Object)(object)ioEnt != (Object)null && !entityRef.IsValid(serverside: true))
			{
				entityRef.Set(ioEnt);
			}
			if (entityRef.IsValid(serverside: true))
			{
				ioEnt = ((Component)entityRef.Get(serverside: true)).GetComponent<IOEntity>();
			}
		}

		public void InitClient()
		{
			if (entityRef.IsValid(serverside: false) && (Object)(object)ioEnt == (Object)null)
			{
				ioEnt = ((Component)entityRef.Get(serverside: false)).GetComponent<IOEntity>();
			}
		}

		public IOEntity Get(bool isServer = true)
		{
			if ((Object)(object)ioEnt == (Object)null && entityRef.IsValid(isServer))
			{
				ioEnt = ((Component)entityRef.Get(isServer)).GetComponent<IOEntity>();
			}
			return ioEnt;
		}

		public void Clear()
		{
			ioEnt = null;
			entityRef.Set(null);
		}

		public void Set(IOEntity newIOEnt)
		{
			entityRef.Set(newIOEnt);
		}
	}

	[Serializable]
	public class IOSlot
	{
		public string niceName;

		public IOType type;

		public IORef connectedTo;

		public int connectedToSlot;

		public Vector3[] linePoints;

		public float[] slackLevels;

		public ClientIOLine line;

		public Vector3 handlePosition;

		public bool rootConnectionsOnly;

		public bool mainPowerSlot;

		public WireTool.WireColour wireColour;

		public void Clear()
		{
			connectedTo.Clear();
			connectedToSlot = 0;
			linePoints = null;
		}
	}

	[Header("IOEntity")]
	public Transform debugOrigin;

	public ItemDefinition sourceItem;

	[NonSerialized]
	public int lastResetIndex;

	[ServerVar]
	[Help("How many miliseconds to budget for processing io entities per server frame")]
	public static float framebudgetms = 1f;

	[ServerVar]
	public static float responsetime = 0.1f;

	[ServerVar]
	public static int backtracking = 8;

	public const Flags Flag_ShortCircuit = Flags.Reserved7;

	public const Flags Flag_HasPower = Flags.Reserved8;

	public IOSlot[] inputs;

	public IOSlot[] outputs;

	public IOType ioType;

	public static Queue<IOEntity> _processQueue = new Queue<IOEntity>();

	private int cachedOutputsUsed;

	protected int lastPassthroughEnergy;

	private int lastEnergy;

	protected int currentEnergy;

	protected float lastUpdateTime;

	protected int lastUpdateBlockedFrame;

	protected bool ensureOutputsUpdated;

	public virtual bool IsGravitySource => false;

	private bool HasBlockedUpdatedOutputsThisFrame => Time.get_frameCount() == lastUpdateBlockedFrame;

	public virtual bool BlockFluidDraining => false;

	protected virtual float LiquidPassthroughGravityThreshold => 1f;

	protected virtual bool DisregardGravityRestrictionsOnLiquid => false;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("IOEntity.OnRpcMessage", 0);
		try
		{
			if (rpc == 4161541566u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_RequestData "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RequestData", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(4161541566u, "Server_RequestData", this, player, 10uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(4161541566u, "Server_RequestData", this, player, 6f))
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
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.get_read();
							RPCMessage msg2 = rPCMessage;
							Server_RequestData(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_RequestData");
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

	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer)
		{
			lastResetIndex = 0;
			cachedOutputsUsed = 0;
			lastPassthroughEnergy = 0;
			lastEnergy = 0;
			currentEnergy = 0;
			lastUpdateTime = 0f;
			ensureOutputsUpdated = false;
		}
	}

	public string GetDisplayName()
	{
		if ((Object)(object)sourceItem != (Object)null)
		{
			return sourceItem.displayName.get_translated();
		}
		return base.ShortPrefabName;
	}

	public virtual bool IsRootEntity()
	{
		return false;
	}

	public IOEntity FindGravitySource(ref Vector3 worldHandlePosition, int depth, bool ignoreSelf)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (depth <= 0)
		{
			return null;
		}
		if (!ignoreSelf && IsGravitySource)
		{
			worldHandlePosition = ((Component)this).get_transform().TransformPoint(outputs[0].handlePosition);
			return this;
		}
		IOSlot[] array = inputs;
		for (int i = 0; i < array.Length; i++)
		{
			IOEntity iOEntity = array[i].connectedTo.Get(base.isServer);
			if ((Object)(object)iOEntity != (Object)null)
			{
				if (iOEntity.IsGravitySource)
				{
					worldHandlePosition = ((Component)iOEntity).get_transform().TransformPoint(iOEntity.outputs[0].handlePosition);
					return iOEntity;
				}
				iOEntity = iOEntity.FindGravitySource(ref worldHandlePosition, depth - 1, ignoreSelf: false);
				if ((Object)(object)iOEntity != (Object)null)
				{
					worldHandlePosition = ((Component)iOEntity).get_transform().TransformPoint(iOEntity.outputs[0].handlePosition);
					return iOEntity;
				}
			}
		}
		return null;
	}

	public virtual void SetFuelType(ItemDefinition def, IOEntity source)
	{
	}

	public virtual bool WantsPower()
	{
		return true;
	}

	public virtual bool WantsPassthroughPower()
	{
		return WantsPower();
	}

	public virtual int ConsumptionAmount()
	{
		return 1;
	}

	public virtual int MaximalPowerOutput()
	{
		return 0;
	}

	public virtual bool AllowDrainFrom(int outputSlot)
	{
		return true;
	}

	public virtual bool IsPowered()
	{
		return HasFlag(Flags.Reserved8);
	}

	public bool IsConnectedToAnySlot(IOEntity entity, int slot, int depth, bool defaultReturn = false)
	{
		if (depth > 0 && slot < inputs.Length)
		{
			IOEntity iOEntity = inputs[slot].connectedTo.Get();
			if ((Object)(object)iOEntity != (Object)null)
			{
				if ((Object)(object)iOEntity == (Object)(object)entity)
				{
					return true;
				}
				if (ConsiderConnectedTo(entity))
				{
					return true;
				}
				if (iOEntity.IsConnectedTo(entity, depth - 1, defaultReturn))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsConnectedTo(IOEntity entity, int slot, int depth, bool defaultReturn = false)
	{
		if (depth > 0 && slot < inputs.Length)
		{
			IOSlot iOSlot = inputs[slot];
			if (iOSlot.mainPowerSlot)
			{
				IOEntity iOEntity = iOSlot.connectedTo.Get();
				if ((Object)(object)iOEntity != (Object)null)
				{
					if ((Object)(object)iOEntity == (Object)(object)entity)
					{
						return true;
					}
					if (ConsiderConnectedTo(entity))
					{
						return true;
					}
					if (iOEntity.IsConnectedTo(entity, depth - 1, defaultReturn))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool IsConnectedTo(IOEntity entity, int depth, bool defaultReturn = false)
	{
		if (depth > 0)
		{
			for (int i = 0; i < inputs.Length; i++)
			{
				IOSlot iOSlot = inputs[i];
				if (!iOSlot.mainPowerSlot)
				{
					continue;
				}
				IOEntity iOEntity = iOSlot.connectedTo.Get();
				if ((Object)(object)iOEntity != (Object)null)
				{
					if ((Object)(object)iOEntity == (Object)(object)entity)
					{
						return true;
					}
					if (ConsiderConnectedTo(entity))
					{
						return true;
					}
					if (iOEntity.IsConnectedTo(entity, depth - 1, defaultReturn))
					{
						return true;
					}
				}
			}
			return false;
		}
		return defaultReturn;
	}

	protected virtual bool ConsiderConnectedTo(IOEntity entity)
	{
		return false;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(6f)]
	[RPC_Server.CallsPerSecond(10uL)]
	private void Server_RequestData(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		int slot = msg.read.Int32();
		bool input = msg.read.Int32() == 1;
		SendAdditionalData(player, slot, input);
	}

	public virtual void SendAdditionalData(BasePlayer player, int slot, bool input)
	{
		int passthroughAmountForAnySlot = GetPassthroughAmountForAnySlot(slot, input);
		ClientRPCPlayer(null, player, "Client_ReceiveAdditionalData", currentEnergy, passthroughAmountForAnySlot, 0f, 0f);
	}

	protected int GetPassthroughAmountForAnySlot(int slot, bool isInputSlot)
	{
		int result = 0;
		if (isInputSlot)
		{
			if (slot >= 0 && slot < inputs.Length)
			{
				IOSlot iOSlot = inputs[slot];
				IOEntity iOEntity = iOSlot.connectedTo.Get();
				if ((Object)(object)iOEntity != (Object)null && iOSlot.connectedToSlot >= 0 && iOSlot.connectedToSlot < iOEntity.outputs.Length)
				{
					result = iOEntity.GetPassthroughAmount(inputs[slot].connectedToSlot);
				}
			}
		}
		else if (slot >= 0 && slot < outputs.Length)
		{
			result = GetPassthroughAmount(slot);
		}
		return result;
	}

	public static void ProcessQueue()
	{
		float realtimeSinceStartup = Time.get_realtimeSinceStartup();
		float num = framebudgetms / 1000f;
		while (_processQueue.Count > 0 && Time.get_realtimeSinceStartup() < realtimeSinceStartup + num && !_processQueue.Peek().HasBlockedUpdatedOutputsThisFrame)
		{
			IOEntity iOEntity = _processQueue.Dequeue();
			if (iOEntity.IsValid())
			{
				iOEntity.UpdateOutputs();
			}
		}
	}

	public virtual void ResetIOState()
	{
	}

	public virtual void Init()
	{
		for (int i = 0; i < outputs.Length; i++)
		{
			IOSlot iOSlot = outputs[i];
			iOSlot.connectedTo.Init();
			if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null)
			{
				int connectedToSlot = iOSlot.connectedToSlot;
				if (connectedToSlot < 0 || connectedToSlot >= iOSlot.connectedTo.Get().inputs.Length)
				{
					Debug.LogError((object)("Slot IOR Error: " + ((Object)this).get_name() + " setting up inputs for " + ((Object)iOSlot.connectedTo.Get()).get_name() + " slot : " + iOSlot.connectedToSlot));
				}
				else
				{
					iOSlot.connectedTo.Get().inputs[iOSlot.connectedToSlot].connectedTo.Set(this);
					iOSlot.connectedTo.Get().inputs[iOSlot.connectedToSlot].connectedToSlot = i;
					iOSlot.connectedTo.Get().inputs[iOSlot.connectedToSlot].connectedTo.Init();
				}
			}
		}
		UpdateUsedOutputs();
		if (IsRootEntity())
		{
			((FacepunchBehaviour)this).Invoke((Action)MarkDirtyForceUpdateOutputs, Random.Range(1f, 1f));
		}
	}

	internal override void DoServerDestroy()
	{
		if (base.isServer)
		{
			Shutdown();
		}
		base.DoServerDestroy();
	}

	public void ClearConnections()
	{
		List<IOEntity> list = new List<IOEntity>();
		IOSlot[] array = inputs;
		foreach (IOSlot iOSlot in array)
		{
			IOEntity iOEntity = null;
			if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null)
			{
				iOEntity = iOSlot.connectedTo.Get();
				IOSlot[] array2 = iOSlot.connectedTo.Get().outputs;
				foreach (IOSlot iOSlot2 in array2)
				{
					if ((Object)(object)iOSlot2.connectedTo.Get() != (Object)null && iOSlot2.connectedTo.Get().EqualNetID(this))
					{
						iOSlot2.Clear();
					}
				}
			}
			iOSlot.Clear();
			if (Object.op_Implicit((Object)(object)iOEntity))
			{
				iOEntity.SendNetworkUpdate();
			}
		}
		array = outputs;
		foreach (IOSlot iOSlot3 in array)
		{
			if ((Object)(object)iOSlot3.connectedTo.Get() != (Object)null)
			{
				list.Add(iOSlot3.connectedTo.Get());
				IOSlot[] array2 = iOSlot3.connectedTo.Get().inputs;
				foreach (IOSlot iOSlot4 in array2)
				{
					if ((Object)(object)iOSlot4.connectedTo.Get() != (Object)null && iOSlot4.connectedTo.Get().EqualNetID(this))
					{
						iOSlot4.Clear();
					}
				}
			}
			if (Object.op_Implicit((Object)(object)iOSlot3.connectedTo.Get()))
			{
				iOSlot3.connectedTo.Get().UpdateFromInput(0, iOSlot3.connectedToSlot);
			}
			iOSlot3.Clear();
		}
		SendNetworkUpdate();
		foreach (IOEntity item in list)
		{
			if ((Object)(object)item != (Object)null)
			{
				item.MarkDirty();
				item.SendNetworkUpdate();
			}
		}
		for (int k = 0; k < inputs.Length; k++)
		{
			UpdateFromInput(0, k);
		}
	}

	public void Shutdown()
	{
		SendChangedToRoot(forceUpdate: true);
		ClearConnections();
	}

	public void MarkDirtyForceUpdateOutputs()
	{
		ensureOutputsUpdated = true;
		MarkDirty();
	}

	public void UpdateUsedOutputs()
	{
		cachedOutputsUsed = 0;
		IOSlot[] array = outputs;
		for (int i = 0; i < array.Length; i++)
		{
			if ((Object)(object)array[i].connectedTo.Get() != (Object)null)
			{
				cachedOutputsUsed++;
			}
		}
	}

	public virtual void MarkDirty()
	{
		if (!base.isClient)
		{
			UpdateUsedOutputs();
			TouchIOState();
		}
	}

	public virtual int DesiredPower()
	{
		return ConsumptionAmount();
	}

	public virtual int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		return inputAmount;
	}

	public virtual int GetCurrentEnergy()
	{
		return Mathf.Clamp(currentEnergy - ConsumptionAmount(), 0, currentEnergy);
	}

	public virtual int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot < 0 || outputSlot >= outputs.Length)
		{
			return 0;
		}
		if ((Object)(object)outputs[outputSlot].connectedTo.Get() == (Object)null)
		{
			return 0;
		}
		int num = ((cachedOutputsUsed == 0) ? 1 : cachedOutputsUsed);
		return GetCurrentEnergy() / num;
	}

	public virtual void UpdateHasPower(int inputAmount, int inputSlot)
	{
		SetFlag(Flags.Reserved8, inputAmount >= ConsumptionAmount() && inputAmount > 0, recursive: false, networkupdate: false);
	}

	public void TouchInternal()
	{
		int passthroughAmount = GetPassthroughAmount();
		bool num = lastPassthroughEnergy != passthroughAmount;
		lastPassthroughEnergy = passthroughAmount;
		if (num)
		{
			IOStateChanged(currentEnergy, 0);
			ensureOutputsUpdated = true;
		}
		_processQueue.Enqueue(this);
	}

	public virtual void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputs[inputSlot].type != ioType)
		{
			IOStateChanged(inputAmount, inputSlot);
			return;
		}
		UpdateHasPower(inputAmount, inputSlot);
		lastEnergy = currentEnergy;
		currentEnergy = CalculateCurrentEnergy(inputAmount, inputSlot);
		int passthroughAmount = GetPassthroughAmount();
		bool flag = lastPassthroughEnergy != passthroughAmount;
		lastPassthroughEnergy = passthroughAmount;
		if (currentEnergy != lastEnergy || flag)
		{
			IOStateChanged(inputAmount, inputSlot);
			ensureOutputsUpdated = true;
		}
		_processQueue.Enqueue(this);
	}

	public virtual void TouchIOState()
	{
		if (!base.isClient)
		{
			TouchInternal();
		}
	}

	public virtual void SendIONetworkUpdate()
	{
		SendNetworkUpdate_Flags();
	}

	public virtual void IOStateChanged(int inputAmount, int inputSlot)
	{
	}

	public virtual void OnCircuitChanged(bool forceUpdate)
	{
		if (forceUpdate)
		{
			MarkDirtyForceUpdateOutputs();
		}
	}

	public virtual void SendChangedToRoot(bool forceUpdate)
	{
		List<IOEntity> existing = Pool.GetList<IOEntity>();
		SendChangedToRootRecursive(forceUpdate, ref existing);
		Pool.FreeList<IOEntity>(ref existing);
	}

	public virtual void SendChangedToRootRecursive(bool forceUpdate, ref List<IOEntity> existing)
	{
		bool flag = IsRootEntity();
		if (existing.Contains(this))
		{
			return;
		}
		existing.Add(this);
		bool flag2 = false;
		for (int i = 0; i < inputs.Length; i++)
		{
			IOSlot iOSlot = inputs[i];
			if (!iOSlot.mainPowerSlot)
			{
				continue;
			}
			IOEntity iOEntity = iOSlot.connectedTo.Get();
			if (!((Object)(object)iOEntity == (Object)null) && !existing.Contains(iOEntity))
			{
				flag2 = true;
				if (forceUpdate)
				{
					iOEntity.ensureOutputsUpdated = true;
				}
				iOEntity.SendChangedToRootRecursive(forceUpdate, ref existing);
			}
		}
		if (flag)
		{
			forceUpdate = forceUpdate && !flag2;
			OnCircuitChanged(forceUpdate);
		}
	}

	protected bool ShouldUpdateOutputs()
	{
		if (Time.get_realtimeSinceStartup() - lastUpdateTime < responsetime)
		{
			lastUpdateBlockedFrame = Time.get_frameCount();
			_processQueue.Enqueue(this);
			return false;
		}
		lastUpdateTime = Time.get_realtimeSinceStartup();
		SendIONetworkUpdate();
		if (outputs.Length == 0)
		{
			ensureOutputsUpdated = false;
			return false;
		}
		return true;
	}

	public virtual void UpdateOutputs()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (!ShouldUpdateOutputs() || !ensureOutputsUpdated)
		{
			return;
		}
		ensureOutputsUpdated = false;
		TimeWarning val = TimeWarning.New("ProcessIOOutputs", 0);
		try
		{
			for (int i = 0; i < outputs.Length; i++)
			{
				IOSlot iOSlot = outputs[i];
				bool flag = true;
				IOEntity iOEntity = iOSlot.connectedTo.Get();
				if (!((Object)(object)iOEntity != (Object)null))
				{
					continue;
				}
				if (ioType == IOType.Fluidic && !DisregardGravityRestrictionsOnLiquid && !iOEntity.DisregardGravityRestrictionsOnLiquid)
				{
					TimeWarning val2 = TimeWarning.New("FluidOutputProcessing", 0);
					try
					{
						if (!iOEntity.AllowLiquidPassthrough(this, ((Component)this).get_transform().TransformPoint(iOSlot.handlePosition)))
						{
							flag = false;
						}
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
				int passthroughAmount = GetPassthroughAmount(i);
				iOEntity.UpdateFromInput(flag ? passthroughAmount : 0, iOSlot.connectedToSlot);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void Spawn()
	{
		base.Spawn();
		if (!Application.isLoadingSave)
		{
			Init();
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		Init();
	}

	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		Init();
	}

	public override void Save(SaveInfo info)
	{
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.ioEntity = Pool.Get<IOEntity>();
		info.msg.ioEntity.inputs = Pool.GetList<IOConnection>();
		info.msg.ioEntity.outputs = Pool.GetList<IOConnection>();
		IOSlot[] array = inputs;
		foreach (IOSlot iOSlot in array)
		{
			IOConnection val = Pool.Get<IOConnection>();
			val.connectedID = iOSlot.connectedTo.entityRef.uid;
			val.connectedToSlot = iOSlot.connectedToSlot;
			val.niceName = iOSlot.niceName;
			val.type = (int)iOSlot.type;
			val.inUse = val.connectedID != 0;
			val.colour = (int)iOSlot.wireColour;
			info.msg.ioEntity.inputs.Add(val);
		}
		array = outputs;
		foreach (IOSlot iOSlot2 in array)
		{
			IOConnection val2 = Pool.Get<IOConnection>();
			val2.connectedID = iOSlot2.connectedTo.entityRef.uid;
			val2.connectedToSlot = iOSlot2.connectedToSlot;
			val2.niceName = iOSlot2.niceName;
			val2.type = (int)iOSlot2.type;
			val2.inUse = val2.connectedID != 0;
			val2.colour = (int)iOSlot2.wireColour;
			if (iOSlot2.linePoints != null)
			{
				val2.linePointList = Pool.GetList<LineVec>();
				val2.linePointList.Clear();
				for (int j = 0; j < iOSlot2.linePoints.Length; j++)
				{
					Vector3 val3 = iOSlot2.linePoints[j];
					LineVec val4 = Pool.Get<LineVec>();
					val4.vec = Vector4.op_Implicit(val3);
					if (iOSlot2.slackLevels.Length > j)
					{
						val4.vec.w = iOSlot2.slackLevels[j];
					}
					val2.linePointList.Add(val4);
				}
			}
			info.msg.ioEntity.outputs.Add(val2);
		}
	}

	public virtual float IOInput(IOEntity from, IOType inputType, float inputAmount, int slot = 0)
	{
		IOSlot[] array = outputs;
		foreach (IOSlot iOSlot in array)
		{
			if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null)
			{
				inputAmount = iOSlot.connectedTo.Get().IOInput(this, iOSlot.type, inputAmount, iOSlot.connectedToSlot);
			}
		}
		return inputAmount;
	}

	public virtual bool AllowLiquidPassthrough(IOEntity fromSource, Vector3 sourceWorldPosition, bool forPlacement = false)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (fromSource.DisregardGravityRestrictionsOnLiquid || DisregardGravityRestrictionsOnLiquid)
		{
			return true;
		}
		if (inputs.Length == 0)
		{
			return false;
		}
		Vector3 val = ((Component)this).get_transform().TransformPoint(inputs[0].handlePosition);
		float num = sourceWorldPosition.y - val.y;
		if (num > 0f)
		{
			return true;
		}
		if (Mathf.Abs(num) < LiquidPassthroughGravityThreshold)
		{
			return true;
		}
		return false;
	}

	public override void Load(LoadInfo info)
	{
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.ioEntity == null)
		{
			return;
		}
		if (!info.fromDisk && info.msg.ioEntity.inputs != null)
		{
			int count = info.msg.ioEntity.inputs.Count;
			if (inputs.Length != count)
			{
				inputs = new IOSlot[count];
			}
			for (int i = 0; i < count; i++)
			{
				if (inputs[i] == null)
				{
					inputs[i] = new IOSlot();
				}
				IOConnection val = info.msg.ioEntity.inputs[i];
				inputs[i].connectedTo = new IORef();
				inputs[i].connectedTo.entityRef.uid = val.connectedID;
				if (base.isClient)
				{
					inputs[i].connectedTo.InitClient();
				}
				inputs[i].connectedToSlot = val.connectedToSlot;
				inputs[i].niceName = val.niceName;
				inputs[i].type = (IOType)val.type;
				inputs[i].wireColour = (WireTool.WireColour)val.colour;
			}
		}
		if (info.msg.ioEntity.outputs == null)
		{
			return;
		}
		if (!info.fromDisk && base.isClient)
		{
			IOSlot[] array = outputs;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].Clear();
			}
		}
		int count2 = info.msg.ioEntity.outputs.Count;
		IOSlot[] array2 = null;
		if (outputs.Length != count2 && count2 > 0)
		{
			array2 = outputs;
			outputs = new IOSlot[count2];
			for (int k = 0; k < array2.Length; k++)
			{
				if (k < count2)
				{
					outputs[k] = array2[k];
				}
			}
		}
		for (int l = 0; l < count2; l++)
		{
			if (outputs[l] == null)
			{
				outputs[l] = new IOSlot();
			}
			IOConnection val2 = info.msg.ioEntity.outputs[l];
			outputs[l].connectedTo = new IORef();
			outputs[l].connectedTo.entityRef.uid = val2.connectedID;
			if (base.isClient)
			{
				outputs[l].connectedTo.InitClient();
			}
			outputs[l].connectedToSlot = val2.connectedToSlot;
			outputs[l].niceName = val2.niceName;
			outputs[l].type = (IOType)val2.type;
			outputs[l].wireColour = (WireTool.WireColour)val2.colour;
			if (info.fromDisk || base.isClient)
			{
				List<LineVec> linePointList = val2.linePointList;
				if (outputs[l].linePoints == null || outputs[l].linePoints.Length != linePointList.Count)
				{
					outputs[l].linePoints = (Vector3[])(object)new Vector3[linePointList.Count];
				}
				if (outputs[l].slackLevels == null || outputs[l].slackLevels.Length != linePointList.Count)
				{
					outputs[l].slackLevels = new float[linePointList.Count];
				}
				for (int m = 0; m < linePointList.Count; m++)
				{
					outputs[l].linePoints[m] = Vector4.op_Implicit(linePointList[m].vec);
					outputs[l].slackLevels[m] = linePointList[m].vec.w;
				}
			}
		}
	}

	public int GetConnectedInputCount()
	{
		int num = 0;
		IOSlot[] array = inputs;
		for (int i = 0; i < array.Length; i++)
		{
			if ((Object)(object)array[i].connectedTo.Get(base.isServer) != (Object)null)
			{
				num++;
			}
		}
		return num;
	}

	public int GetConnectedOutputCount()
	{
		int num = 0;
		IOSlot[] array = outputs;
		for (int i = 0; i < array.Length; i++)
		{
			if ((Object)(object)array[i].connectedTo.Get(base.isServer) != (Object)null)
			{
				num++;
			}
		}
		return num;
	}

	public bool HasConnections()
	{
		if (GetConnectedInputCount() <= 0)
		{
			return GetConnectedOutputCount() > 0;
		}
		return true;
	}
}
