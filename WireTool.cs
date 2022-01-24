using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class WireTool : HeldEntity
{
	public enum WireColour
	{
		Default,
		Red,
		Green,
		Blue,
		Yellow
	}

	public struct PendingPlug_t
	{
		public IOEntity ent;

		public bool input;

		public int index;

		public GameObject tempLine;
	}

	public Sprite InputSprite;

	public Sprite OutputSprite;

	public Sprite ClearSprite;

	public static float maxWireLength = 30f;

	private const int maxLineNodes = 16;

	public GameObjectRef plugEffect;

	public GameObjectRef ioLine;

	public IOEntity.IOType wireType;

	public static Phrase Default = new Phrase("wiretoolcolour.default", "Default");

	public static Phrase DefaultDesc = new Phrase("wiretoolcolour.default.desc", "Default connection color");

	public static Phrase Red = new Phrase("wiretoolcolour.red", "Red");

	public static Phrase RedDesc = new Phrase("wiretoolcolour.red.desc", "Red connection color");

	public static Phrase Green = new Phrase("wiretoolcolour.green", "Green");

	public static Phrase GreenDesc = new Phrase("wiretoolcolour.green.desc", "Green connection color");

	public static Phrase Blue = new Phrase("wiretoolcolour.blue", "Blue");

	public static Phrase BlueDesc = new Phrase("wiretoolcolour.blue.desc", "Blue connection color");

	public static Phrase Yellow = new Phrase("wiretoolcolour.yellow", "Yellow");

	public static Phrase YellowDesc = new Phrase("wiretoolcolour.yellow.desc", "Yellow connection color");

	public PendingPlug_t pending;

	public bool CanChangeColours
	{
		get
		{
			if (wireType != 0)
			{
				return wireType == IOEntity.IOType.Fluidic;
			}
			return true;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("WireTool.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 678101026 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - AddLine "));
				}
				TimeWarning val2 = TimeWarning.New("AddLine", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(678101026u, "AddLine", this, player))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(678101026u, "AddLine", this, player))
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
							AddLine(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AddLine");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 40328523 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - MakeConnection "));
				}
				TimeWarning val2 = TimeWarning.New("MakeConnection", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(40328523u, "MakeConnection", this, player))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(40328523u, "MakeConnection", this, player))
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
							MakeConnection(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in MakeConnection");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2469840259u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RequestClear "));
				}
				TimeWarning val2 = TimeWarning.New("RequestClear", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(2469840259u, "RequestClear", this, player))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(2469840259u, "RequestClear", this, player))
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
							RPCMessage msg4 = rPCMessage;
							RequestClear(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RequestClear");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2596458392u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetPlugged "));
				}
				TimeWarning val2 = TimeWarning.New("SetPlugged", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage plugged = rPCMessage;
						SetPlugged(plugged);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex4)
				{
					Debug.LogException(ex4);
					player.Kick("RPC Error in SetPlugged");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 210386477 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - TryClear "));
				}
				TimeWarning val2 = TimeWarning.New("TryClear", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(210386477u, "TryClear", this, player))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(210386477u, "TryClear", this, player))
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
							RPCMessage msg5 = rPCMessage;
							TryClear(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in TryClear");
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

	public void ClearPendingPlug()
	{
		pending.ent = null;
		pending.index = -1;
	}

	public bool HasPendingPlug()
	{
		if ((Object)(object)pending.ent != (Object)null)
		{
			return pending.index != -1;
		}
		return false;
	}

	public bool PendingPlugIsInput()
	{
		if ((Object)(object)pending.ent != (Object)null && pending.index != -1)
		{
			return pending.input;
		}
		return false;
	}

	public bool PendingPlugIsType(IOEntity.IOType type)
	{
		if ((Object)(object)pending.ent != (Object)null && pending.index != -1)
		{
			if (!pending.input || pending.ent.inputs[pending.index].type != type)
			{
				if (!pending.input)
				{
					return pending.ent.outputs[pending.index].type == type;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public bool PendingPlugIsOutput()
	{
		if ((Object)(object)pending.ent != (Object)null && pending.index != -1)
		{
			return !pending.input;
		}
		return false;
	}

	public Vector3 PendingPlugWorldPos()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)pending.ent == (Object)null || pending.index == -1)
		{
			return Vector3.get_zero();
		}
		if (pending.input)
		{
			return ((Component)pending.ent).get_transform().TransformPoint(pending.ent.inputs[pending.index].handlePosition);
		}
		return ((Component)pending.ent).get_transform().TransformPoint(pending.ent.outputs[pending.index].handlePosition);
	}

	public static bool CanPlayerUseWires(BasePlayer player)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!player.CanBuild())
		{
			return false;
		}
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(player.eyes.position, 0.1f, list, 536870912, (QueryTriggerInteraction)2);
		bool result = Enumerable.All<Collider>((IEnumerable<Collider>)list, (Func<Collider, bool>)((Collider collider) => ((Component)collider).get_gameObject().CompareTag("IgnoreWireCheck")));
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CanModifyEntity(BasePlayer player, BaseEntity ent)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return player.CanBuild(((Component)ent).get_transform().get_position(), ((Component)ent).get_transform().get_rotation(), ent.bounds);
	}

	public bool PendingPlugRoot()
	{
		if ((Object)(object)pending.ent != (Object)null)
		{
			return pending.ent.IsRootEntity();
		}
		return false;
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	[RPC_Server.FromOwner]
	public void TryClear(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		uint uid = msg.read.UInt32();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		IOEntity iOEntity = (((Object)(object)baseNetworkable == (Object)null) ? null : ((Component)baseNetworkable).GetComponent<IOEntity>());
		if (!((Object)(object)iOEntity == (Object)null) && CanPlayerUseWires(player) && CanModifyEntity(player, iOEntity))
		{
			iOEntity.ClearConnections();
			iOEntity.SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	[RPC_Server.FromOwner]
	public void MakeConnection(RPCMessage msg)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!CanPlayerUseWires(player))
		{
			return;
		}
		uint uid = msg.read.UInt32();
		int num = msg.read.Int32();
		uint uid2 = msg.read.UInt32();
		int num2 = msg.read.Int32();
		WireColour wireColour = IntToColour(msg.read.Int32());
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		IOEntity iOEntity = (((Object)(object)baseNetworkable == (Object)null) ? null : ((Component)baseNetworkable).GetComponent<IOEntity>());
		if (!((Object)(object)iOEntity == (Object)null))
		{
			BaseNetworkable baseNetworkable2 = BaseNetworkable.serverEntities.Find(uid2);
			IOEntity iOEntity2 = (((Object)(object)baseNetworkable2 == (Object)null) ? null : ((Component)baseNetworkable2).GetComponent<IOEntity>());
			if (!((Object)(object)iOEntity2 == (Object)null) && !(Vector3.Distance(((Component)baseNetworkable2).get_transform().get_position(), ((Component)baseNetworkable).get_transform().get_position()) > maxWireLength) && num < iOEntity.inputs.Length && num2 < iOEntity2.outputs.Length && !((Object)(object)iOEntity.inputs[num].connectedTo.Get() != (Object)null) && !((Object)(object)iOEntity2.outputs[num2].connectedTo.Get() != (Object)null) && (!iOEntity.inputs[num].rootConnectionsOnly || iOEntity2.IsRootEntity()) && CanModifyEntity(player, iOEntity) && CanModifyEntity(player, iOEntity2))
			{
				iOEntity.inputs[num].connectedTo.Set(iOEntity2);
				iOEntity.inputs[num].connectedToSlot = num2;
				iOEntity.inputs[num].wireColour = wireColour;
				iOEntity.inputs[num].connectedTo.Init();
				iOEntity2.outputs[num2].connectedTo.Set(iOEntity);
				iOEntity2.outputs[num2].connectedToSlot = num;
				iOEntity2.outputs[num2].wireColour = wireColour;
				iOEntity2.outputs[num2].connectedTo.Init();
				iOEntity2.MarkDirtyForceUpdateOutputs();
				iOEntity2.SendNetworkUpdate();
				iOEntity.SendNetworkUpdate();
				iOEntity2.SendChangedToRoot(forceUpdate: true);
			}
		}
	}

	[RPC_Server]
	public void SetPlugged(RPCMessage msg)
	{
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	[RPC_Server.FromOwner]
	public void RequestClear(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!CanPlayerUseWires(player))
		{
			return;
		}
		uint uid = msg.read.UInt32();
		int num = msg.read.Int32();
		bool flag = msg.read.Bit();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		IOEntity iOEntity = (((Object)(object)baseNetworkable == (Object)null) ? null : ((Component)baseNetworkable).GetComponent<IOEntity>());
		if ((Object)(object)iOEntity == (Object)null || !CanModifyEntity(player, iOEntity) || num >= (flag ? iOEntity.inputs.Length : iOEntity.outputs.Length))
		{
			return;
		}
		IOEntity.IOSlot iOSlot = (flag ? iOEntity.inputs[num] : iOEntity.outputs[num]);
		if ((Object)(object)iOSlot.connectedTo.Get() == (Object)null)
		{
			return;
		}
		IOEntity iOEntity2 = iOSlot.connectedTo.Get();
		IOEntity.IOSlot obj = (flag ? iOEntity2.outputs[iOSlot.connectedToSlot] : iOEntity2.inputs[iOSlot.connectedToSlot]);
		if (flag)
		{
			iOEntity.UpdateFromInput(0, num);
		}
		else if (Object.op_Implicit((Object)(object)iOEntity2))
		{
			iOEntity2.UpdateFromInput(0, iOSlot.connectedToSlot);
		}
		iOSlot.Clear();
		obj.Clear();
		iOEntity.MarkDirtyForceUpdateOutputs();
		iOEntity.SendNetworkUpdate();
		if (flag && (Object)(object)iOEntity2 != (Object)null)
		{
			iOEntity2.SendChangedToRoot(forceUpdate: true);
		}
		else if (!flag)
		{
			IOEntity.IOSlot[] inputs = iOEntity.inputs;
			foreach (IOEntity.IOSlot iOSlot2 in inputs)
			{
				if (iOSlot2.mainPowerSlot && Object.op_Implicit((Object)(object)iOSlot2.connectedTo.Get()))
				{
					iOSlot2.connectedTo.Get().SendChangedToRoot(forceUpdate: true);
				}
			}
		}
		iOEntity2.SendNetworkUpdate();
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	[RPC_Server.FromOwner]
	public void AddLine(RPCMessage msg)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!CanPlayerUseWires(player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num > 18)
		{
			return;
		}
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < num; i++)
		{
			Vector3 item = msg.read.Vector3();
			list.Add(item);
		}
		uint uid = msg.read.UInt32();
		int num2 = msg.read.Int32();
		uint uid2 = msg.read.UInt32();
		int num3 = msg.read.Int32();
		WireColour wireColour = IntToColour(msg.read.Int32());
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		IOEntity iOEntity = (((Object)(object)baseNetworkable == (Object)null) ? null : ((Component)baseNetworkable).GetComponent<IOEntity>());
		if (!((Object)(object)iOEntity == (Object)null))
		{
			BaseNetworkable baseNetworkable2 = BaseNetworkable.serverEntities.Find(uid2);
			IOEntity iOEntity2 = (((Object)(object)baseNetworkable2 == (Object)null) ? null : ((Component)baseNetworkable2).GetComponent<IOEntity>());
			if (!((Object)(object)iOEntity2 == (Object)null) && ValidateLine(list, iOEntity, iOEntity2) && num2 < iOEntity.inputs.Length && num3 < iOEntity2.outputs.Length && !((Object)(object)iOEntity.inputs[num2].connectedTo.Get() != (Object)null) && !((Object)(object)iOEntity2.outputs[num3].connectedTo.Get() != (Object)null) && (!iOEntity.inputs[num2].rootConnectionsOnly || iOEntity2.IsRootEntity()) && CanModifyEntity(player, iOEntity2) && CanModifyEntity(player, iOEntity))
			{
				iOEntity2.outputs[num3].linePoints = list.ToArray();
				iOEntity2.outputs[num3].wireColour = wireColour;
				iOEntity2.SendNetworkUpdate();
			}
		}
	}

	private WireColour IntToColour(int i)
	{
		if (i < 0)
		{
			i = 0;
		}
		if (i > 4)
		{
			i = 4;
		}
		WireColour wireColour = (WireColour)i;
		if (wireType == IOEntity.IOType.Fluidic && wireColour == WireColour.Green)
		{
			wireColour = WireColour.Default;
		}
		return wireColour;
	}

	private bool ValidateLine(List<Vector3> lineList, IOEntity inputEntity, IOEntity outputEntity)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (lineList.Count < 2)
		{
			return false;
		}
		if ((Object)(object)inputEntity == (Object)null || (Object)(object)outputEntity == (Object)null)
		{
			return false;
		}
		Vector3 val = lineList[0];
		float num = 0f;
		int count = lineList.Count;
		for (int i = 1; i < count; i++)
		{
			Vector3 val2 = lineList[i];
			num += Vector3.Distance(val, val2);
			if (num > maxWireLength)
			{
				return false;
			}
			val = val2;
		}
		Vector3 val3 = lineList[count - 1];
		Bounds val4 = outputEntity.bounds;
		((Bounds)(ref val4)).Expand(0.5f);
		if (!((Bounds)(ref val4)).Contains(val3))
		{
			return false;
		}
		val3 = ((Component)inputEntity).get_transform().InverseTransformPoint(((Component)outputEntity).get_transform().TransformPoint(lineList[0]));
		Bounds val5 = inputEntity.bounds;
		((Bounds)(ref val5)).Expand(0.5f);
		if (!((Bounds)(ref val5)).Contains(val3))
		{
			return false;
		}
		return true;
	}
}
