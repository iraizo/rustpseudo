using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

public class PhoneController : EntityComponent<BaseEntity>
{
	private PhoneController activeCallTo;

	public int PhoneNumber;

	public string PhoneName;

	public bool CanModifyPhoneName = true;

	public bool CanSaveNumbers = true;

	public bool RequirePower = true;

	public bool RequireParent;

	public float CallWaitingTime = 12f;

	public bool AppendGridToName;

	public bool IsMobile;

	public bool CanSaveVoicemail;

	public GameObjectRef PhoneDialog;

	public VoiceProcessor VProcessor;

	public PreloadedCassetteContent PreloadedContent;

	public SoundDefinition DialToneSfx;

	public SoundDefinition RingingSfx;

	public SoundDefinition ErrorSfx;

	public SoundDefinition CallIncomingWhileBusySfx;

	public SoundDefinition PickupHandsetSfx;

	public SoundDefinition PutDownHandsetSfx;

	public SoundDefinition FailedWrongNumber;

	public SoundDefinition FailedNoAnswer;

	public SoundDefinition FailedNetworkBusy;

	public SoundDefinition FailedEngaged;

	public SoundDefinition FailedRemoteHangUp;

	public SoundDefinition FailedSelfHangUp;

	public Light RingingLight;

	public float RingingLightFrequency = 0.4f;

	public AudioSource answeringMachineSound;

	public EntityRef currentPlayerRef;

	public List<VoicemailEntry> savedVoicemail;

	public Telephone.CallState serverState { get; set; }

	public uint AnsweringMessageId
	{
		get
		{
			Telephone telephone;
			if ((telephone = base.baseEntity as Telephone) == null)
			{
				return 0u;
			}
			return telephone.AnsweringMessageId;
		}
	}

	public int MaxVoicemailSlots
	{
		get
		{
			if (!((Object)(object)cachedCassette != (Object)null))
			{
				return 0;
			}
			return cachedCassette.MaximumVoicemailSlots;
		}
	}

	public BasePlayer currentPlayer
	{
		get
		{
			if (currentPlayerRef.IsValid(isServer))
			{
				return currentPlayerRef.Get(isServer).ToPlayer();
			}
			return null;
		}
		set
		{
			currentPlayerRef.Set(value);
		}
	}

	private bool isServer
	{
		get
		{
			if ((Object)(object)base.baseEntity != (Object)null)
			{
				return base.baseEntity.isServer;
			}
			return false;
		}
	}

	public int lastDialedNumber { get; set; }

	public PhoneDirectory savedNumbers { get; set; }

	public BaseEntity ParentEntity => base.baseEntity;

	private Cassette cachedCassette
	{
		get
		{
			Telephone telephone;
			if (!((Object)(object)base.baseEntity != (Object)null) || (telephone = base.baseEntity as Telephone) == null)
			{
				return null;
			}
			return telephone.cachedCassette;
		}
	}

	public void ServerInit()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (PhoneNumber == 0 && !Application.isLoadingSave)
		{
			PhoneNumber = TelephoneManager.GetUnusedTelephoneNumber();
			if (AppendGridToName & !string.IsNullOrEmpty(PhoneName))
			{
				PhoneName = PhoneName + " " + PositionToGridCoord(((Component)this).get_transform().get_position());
			}
			TelephoneManager.RegisterTelephone(this);
		}
	}

	public void PostServerLoad()
	{
		currentPlayer = null;
		base.baseEntity.SetFlag(BaseEntity.Flags.Busy, b: false);
		TelephoneManager.RegisterTelephone(this);
	}

	public void DoServerDestroy()
	{
		TelephoneManager.DeregisterTelephone(this);
	}

	public void ClearCurrentUser(BaseEntity.RPCMessage msg)
	{
		ClearCurrentUser();
	}

	public void ClearCurrentUser()
	{
		if ((Object)(object)currentPlayer != (Object)null)
		{
			currentPlayer.SetActiveTelephone(null);
			currentPlayer = null;
		}
		base.baseEntity.SetFlag(BaseEntity.Flags.Busy, b: false);
	}

	public void SetCurrentUser(BaseEntity.RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)currentPlayer == (Object)(object)player))
		{
			UpdateServerPlayer(player);
			if (serverState == Telephone.CallState.Dialing || serverState == Telephone.CallState.Ringing || serverState == Telephone.CallState.InProcess)
			{
				ServerHangUp(default(BaseEntity.RPCMessage));
			}
		}
	}

	private void UpdateServerPlayer(BasePlayer newPlayer)
	{
		if (!((Object)(object)currentPlayer == (Object)(object)newPlayer))
		{
			if ((Object)(object)currentPlayer != (Object)null)
			{
				currentPlayer.SetActiveTelephone(null);
			}
			currentPlayer = newPlayer;
			base.baseEntity.SetFlag(BaseEntity.Flags.Busy, (Object)(object)currentPlayer != (Object)null);
			if ((Object)(object)currentPlayer != (Object)null)
			{
				currentPlayer.SetActiveTelephone(this);
			}
		}
	}

	public void InitiateCall(BaseEntity.RPCMessage msg)
	{
		if (!((Object)(object)msg.player != (Object)(object)currentPlayer))
		{
			int number = msg.read.Int32();
			CallPhone(number);
		}
	}

	public void CallPhone(int number)
	{
		if (number == PhoneNumber)
		{
			OnDialFailed(Telephone.DialFailReason.CallSelf);
			return;
		}
		if (TelephoneManager.GetCurrentActiveCalls() + 1 > TelephoneManager.MaxConcurrentCalls)
		{
			OnDialFailed(Telephone.DialFailReason.NetworkBusy);
			return;
		}
		PhoneController telephone = TelephoneManager.GetTelephone(number);
		if ((Object)(object)telephone != (Object)null)
		{
			if (telephone.serverState == Telephone.CallState.Idle && telephone.CanReceiveCall())
			{
				SetPhoneState(Telephone.CallState.Dialing);
				lastDialedNumber = number;
				activeCallTo = telephone;
				activeCallTo.ReceiveCallFrom(this);
			}
			else
			{
				OnDialFailed(Telephone.DialFailReason.Engaged);
				telephone.OnIncomingCallWhileBusy();
			}
		}
		else
		{
			OnDialFailed(Telephone.DialFailReason.WrongNumber);
		}
	}

	private bool CanReceiveCall()
	{
		if (RequirePower && !IsPowered())
		{
			return false;
		}
		if (RequireParent && !base.baseEntity.HasParent())
		{
			return false;
		}
		return true;
	}

	public void AnswerPhone(BaseEntity.RPCMessage msg)
	{
		if (((FacepunchBehaviour)this).IsInvoking((Action)TimeOutDialing))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)TimeOutDialing);
		}
		if (!((Object)(object)activeCallTo == (Object)null))
		{
			BasePlayer player = msg.player;
			UpdateServerPlayer(player);
			BeginCall();
			activeCallTo.BeginCall();
		}
	}

	public void ReceiveCallFrom(PhoneController t)
	{
		activeCallTo = t;
		SetPhoneState(Telephone.CallState.Ringing);
		((FacepunchBehaviour)this).Invoke((Action)TimeOutDialing, CallWaitingTime);
	}

	private void TimeOutDialing()
	{
		if ((Object)(object)activeCallTo != (Object)null)
		{
			activeCallTo.ServerPlayAnsweringMessage(this);
		}
		SetPhoneState(Telephone.CallState.Idle);
	}

	public void OnDialFailed(Telephone.DialFailReason reason)
	{
		SetPhoneState(Telephone.CallState.Idle);
		base.baseEntity.ClientRPC(null, "ClientOnDialFailed", (int)reason);
		activeCallTo = null;
		if (((FacepunchBehaviour)this).IsInvoking((Action)TimeOutCall))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)TimeOutCall);
		}
		if (((FacepunchBehaviour)this).IsInvoking((Action)TriggerTimeOut))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)TriggerTimeOut);
		}
		if (((FacepunchBehaviour)this).IsInvoking((Action)TimeOutDialing))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)TimeOutDialing);
		}
	}

	public void ServerPlayAnsweringMessage(PhoneController fromPhone)
	{
		uint num = 0u;
		uint num2 = 0u;
		uint arg = 0u;
		if ((Object)(object)activeCallTo != (Object)null && (Object)(object)activeCallTo.cachedCassette != (Object)null)
		{
			num = activeCallTo.cachedCassette.net.ID;
			num2 = activeCallTo.cachedCassette.AudioId;
			if (num2 == 0)
			{
				arg = StringPool.Get(((Object)activeCallTo.cachedCassette.PreloadedAudio).get_name());
			}
		}
		if (num != 0)
		{
			base.baseEntity.ClientRPC(null, "ClientPlayAnsweringMessage", num, num2, arg, fromPhone.HasVoicemailSlot() ? 1 : 0, activeCallTo.PhoneNumber);
			((FacepunchBehaviour)this).Invoke((Action)TriggerTimeOut, activeCallTo.cachedCassette.MaxCassetteLength);
		}
		else
		{
			OnDialFailed(Telephone.DialFailReason.TimedOut);
		}
	}

	private void TriggerTimeOut()
	{
		OnDialFailed(Telephone.DialFailReason.TimedOut);
	}

	public void SetPhoneStateWithPlayer(Telephone.CallState state)
	{
		serverState = state;
		base.baseEntity.ClientRPC(null, "SetClientState", (int)serverState, ((Object)(object)activeCallTo != (Object)null) ? activeCallTo.PhoneNumber : 0);
		MobilePhone mobilePhone;
		if ((mobilePhone = base.baseEntity as MobilePhone) != null)
		{
			mobilePhone.ToggleRinging(state == Telephone.CallState.Ringing);
		}
	}

	private void SetPhoneState(Telephone.CallState state)
	{
		if (state == Telephone.CallState.Idle && (Object)(object)currentPlayer == (Object)null)
		{
			base.baseEntity.SetFlag(BaseEntity.Flags.Busy, b: false);
		}
		serverState = state;
		base.baseEntity.ClientRPC(null, "SetClientState", (int)serverState, ((Object)(object)activeCallTo != (Object)null) ? activeCallTo.PhoneNumber : 0);
		Telephone telephone;
		if ((telephone = base.baseEntity as Telephone) != null)
		{
			telephone.MarkDirtyForceUpdateOutputs();
		}
		MobilePhone mobilePhone;
		if ((mobilePhone = base.baseEntity as MobilePhone) != null)
		{
			mobilePhone.ToggleRinging(state == Telephone.CallState.Ringing);
		}
	}

	public void BeginCall()
	{
		if (IsMobile && (Object)(object)activeCallTo != (Object)null && !activeCallTo.RequirePower)
		{
			_ = (Object)(object)currentPlayer != (Object)null;
		}
		SetPhoneStateWithPlayer(Telephone.CallState.InProcess);
		((FacepunchBehaviour)this).Invoke((Action)TimeOutCall, (float)TelephoneManager.MaxCallLength);
	}

	public void ServerHangUp(BaseEntity.RPCMessage msg)
	{
		if (!((Object)(object)msg.player != (Object)(object)currentPlayer))
		{
			ServerHangUp();
		}
	}

	public void ServerHangUp()
	{
		if ((Object)(object)activeCallTo != (Object)null)
		{
			activeCallTo.RemoteHangUp();
		}
		SelfHangUp();
	}

	private void SelfHangUp()
	{
		OnDialFailed(Telephone.DialFailReason.SelfHangUp);
	}

	private void RemoteHangUp()
	{
		OnDialFailed(Telephone.DialFailReason.RemoteHangUp);
	}

	private void TimeOutCall()
	{
		OnDialFailed(Telephone.DialFailReason.TimeOutDuringCall);
	}

	public void OnReceivedVoiceFromUser(byte[] data)
	{
		if ((Object)(object)activeCallTo != (Object)null)
		{
			activeCallTo.OnReceivedDataFromConnectedPhone(data);
		}
	}

	public void OnReceivedDataFromConnectedPhone(byte[] data)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity obj = base.baseEntity;
		SendInfo sendInfo = default(SendInfo);
		((SendInfo)(ref sendInfo))._002Ector(BaseNetworkable.GetConnectionsWithin(((Component)this).get_transform().get_position(), 15f));
		sendInfo.priority = (Priority)0;
		obj.ClientRPCEx(sendInfo, null, "OnReceivedVoice", data.Length, data);
	}

	public void OnIncomingCallWhileBusy()
	{
		base.baseEntity.ClientRPC(null, "OnIncomingCallDuringCall");
	}

	public void DestroyShared()
	{
		if (isServer && serverState != 0 && (Object)(object)activeCallTo != (Object)null)
		{
			activeCallTo.RemoteHangUp();
		}
	}

	public void UpdatePhoneName(BaseEntity.RPCMessage msg)
	{
		if (!((Object)(object)msg.player != (Object)(object)currentPlayer))
		{
			string text = msg.read.String(256);
			if (text.Length > 20)
			{
				text = text.Substring(0, 20);
			}
			PhoneName = text;
			base.baseEntity.SendNetworkUpdate();
		}
	}

	public void Server_RequestPhoneDirectory(BaseEntity.RPCMessage msg)
	{
		if (!((Object)(object)msg.player != (Object)(object)currentPlayer))
		{
			int page = msg.read.Int32();
			PhoneDirectory val = Pool.Get<PhoneDirectory>();
			try
			{
				TelephoneManager.GetPhoneDirectory(PhoneNumber, page, 12, val);
				base.baseEntity.ClientRPC<PhoneDirectory>(null, "ReceivePhoneDirectory", val);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public void Server_AddSavedNumber(BaseEntity.RPCMessage msg)
	{
		if (!((Object)(object)msg.player != (Object)(object)currentPlayer))
		{
			if (savedNumbers == null)
			{
				savedNumbers = Pool.Get<PhoneDirectory>();
			}
			if (savedNumbers.entries == null)
			{
				savedNumbers.entries = Pool.GetList<DirectoryEntry>();
			}
			int num = msg.read.Int32();
			string text = msg.read.String(256);
			if (IsSavedContactValid(text, num) && savedNumbers.entries.Count < 10)
			{
				DirectoryEntry val = Pool.Get<DirectoryEntry>();
				val.phoneName = text;
				val.phoneNumber = num;
				val.ShouldPool = false;
				savedNumbers.ShouldPool = false;
				savedNumbers.entries.Add(val);
				base.baseEntity.SendNetworkUpdate();
			}
		}
	}

	public void Server_RemoveSavedNumber(BaseEntity.RPCMessage msg)
	{
		if (!((Object)(object)msg.player != (Object)(object)currentPlayer))
		{
			uint number = msg.read.UInt32();
			if (savedNumbers.entries.RemoveAll((DirectoryEntry p) => p.phoneNumber == number) > 0)
			{
				base.baseEntity.SendNetworkUpdate();
			}
		}
	}

	public string GetDirectoryName()
	{
		return PhoneName;
	}

	private static string PositionToGridCoord(Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = new Vector2(TerrainMeta.NormalizeX(position.x), TerrainMeta.NormalizeZ(position.z));
		float num = TerrainMeta.Size.x / 1024f;
		int num2 = 7;
		Vector2 val2 = val * num * (float)num2;
		float num3 = Mathf.Floor(val2.x) + 1f;
		float num4 = Mathf.Floor(num * (float)num2 - val2.y);
		string text = string.Empty;
		float num5 = num3 / 26f;
		float num6 = num3 % 26f;
		if (num6 == 0f)
		{
			num6 = 26f;
		}
		if (num5 > 1f)
		{
			text += Convert.ToChar(64 + (int)num5);
		}
		text += Convert.ToChar(64 + (int)num6);
		return $"{text}{num4}";
	}

	public void WatchForDisconnects()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if ((Object)(object)currentPlayer != (Object)null)
		{
			if (currentPlayer.IsSleeping())
			{
				flag = true;
			}
			if (currentPlayer.IsDead())
			{
				flag = true;
			}
			if (Vector3.Distance(((Component)this).get_transform().get_position(), ((Component)currentPlayer).get_transform().get_position()) > 5f)
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			ServerHangUp();
			ClearCurrentUser();
		}
	}

	public void OnParentChanged(BaseEntity newParent)
	{
		if ((Object)(object)newParent != (Object)null && newParent is BasePlayer)
		{
			TelephoneManager.RegisterTelephone(this, checkPhoneNumber: true);
		}
		else
		{
			TelephoneManager.DeregisterTelephone(this);
		}
	}

	private bool HasVoicemailSlot()
	{
		return MaxVoicemailSlots > 0;
	}

	public void ServerSendVoicemail(BaseEntity.RPCMessage msg)
	{
		if (!((Object)(object)msg.player == (Object)null))
		{
			byte[] data = msg.read.BytesWithSize(10485760u);
			PhoneController telephone = TelephoneManager.GetTelephone(msg.read.Int32());
			if (!((Object)(object)telephone == (Object)null) && Cassette.IsOggValid(data, telephone.cachedCassette))
			{
				telephone.SaveVoicemail(data, msg.player.displayName);
			}
		}
	}

	public void SaveVoicemail(byte[] data, string playerName)
	{
		uint audioId = FileStorage.server.Store(data, FileStorage.Type.ogg, base.baseEntity.net.ID);
		if (savedVoicemail == null)
		{
			savedVoicemail = Pool.GetList<VoicemailEntry>();
		}
		VoicemailEntry val = Pool.Get<VoicemailEntry>();
		val.audioId = audioId;
		val.timestamp = DateTime.Now.ToBinary();
		val.userName = playerName;
		val.ShouldPool = false;
		savedVoicemail.Add(val);
		while (savedVoicemail.Count > MaxVoicemailSlots)
		{
			FileStorage.server.Remove(savedVoicemail[0].audioId, FileStorage.Type.ogg, base.baseEntity.net.ID);
			savedVoicemail.RemoveAt(0);
		}
		base.baseEntity.SendNetworkUpdate();
	}

	public void ServerPlayVoicemail(BaseEntity.RPCMessage msg)
	{
		base.baseEntity.ClientRPC(null, "ClientToggleVoicemail", 1, msg.read.UInt32());
	}

	public void ServerStopVoicemail(BaseEntity.RPCMessage msg)
	{
		base.baseEntity.ClientRPC(null, "ClientToggleVoicemail", 0, msg.read.UInt32());
	}

	public void ServerDeleteVoicemail(BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		for (int i = 0; i < savedVoicemail.Count; i++)
		{
			if (savedVoicemail[i].audioId == num)
			{
				VoicemailEntry val = savedVoicemail[i];
				FileStorage.server.Remove(val.audioId, FileStorage.Type.ogg, base.baseEntity.net.ID);
				val.ShouldPool = true;
				Pool.Free<VoicemailEntry>(ref val);
				savedVoicemail.RemoveAt(i);
				base.baseEntity.SendNetworkUpdate();
				break;
			}
		}
	}

	public void DeleteAllVoicemail()
	{
		if (savedVoicemail == null)
		{
			return;
		}
		foreach (VoicemailEntry item in savedVoicemail)
		{
			item.ShouldPool = true;
			FileStorage.server.Remove(item.audioId, FileStorage.Type.ogg, base.baseEntity.net.ID);
		}
		Pool.FreeList<VoicemailEntry>(ref savedVoicemail);
	}

	private bool IsPowered()
	{
		IOEntity iOEntity;
		if ((Object)(object)base.baseEntity != (Object)null && (iOEntity = base.baseEntity as IOEntity) != null)
		{
			return iOEntity.IsPowered();
		}
		return false;
	}

	public bool IsSavedContactValid(string contactName, int contactNumber)
	{
		if (contactName.Length <= 0 || contactName.Length > 20)
		{
			return false;
		}
		if (contactNumber < 10000000 || contactNumber >= 100000000)
		{
			return false;
		}
		return true;
	}

	public void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
	}
}
