using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CompanionServer;
using ConVar;
using EasyAntiCheat.Server.Cerberus;
using EasyAntiCheat.Server.Hydra;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Facepunch.Rust;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using SilentOrbit.ProtocolBuffers;
using UnityEngine;
using UnityEngine.Assertions;

public class BasePlayer : BaseCombatEntity, LootPanel.IHasLootPanel
{
	public enum CameraMode
	{
		FirstPerson = 0,
		ThirdPerson = 1,
		Eyes = 2,
		FirstPersonWithArms = 3,
		Last = 3
	}

	public enum NetworkQueue
	{
		Update,
		UpdateDistance,
		Count
	}

	private class NetworkQueueList
	{
		public HashSet<BaseNetworkable> queueInternal = new HashSet<BaseNetworkable>();

		public int MaxLength;

		public int Length => queueInternal.get_Count();

		public bool Contains(BaseNetworkable ent)
		{
			return queueInternal.Contains(ent);
		}

		public void Add(BaseNetworkable ent)
		{
			if (!Contains(ent))
			{
				queueInternal.Add(ent);
			}
			MaxLength = Mathf.Max(MaxLength, queueInternal.get_Count());
		}

		public void Add(BaseNetworkable[] ent)
		{
			foreach (BaseNetworkable ent2 in ent)
			{
				Add(ent2);
			}
		}

		public void Clear(Group group)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			TimeWarning val = TimeWarning.New("NetworkQueueList.Clear", 0);
			try
			{
				if (group != null)
				{
					if (group.get_isGlobal())
					{
						return;
					}
					List<BaseNetworkable> list = Pool.GetList<BaseNetworkable>();
					Enumerator<BaseNetworkable> enumerator = queueInternal.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							BaseNetworkable current = enumerator.get_Current();
							if ((Object)(object)current == (Object)null || current.net?.group == null || current.net.group == group)
							{
								list.Add(current);
							}
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					foreach (BaseNetworkable item in list)
					{
						queueInternal.Remove(item);
					}
					Pool.FreeList<BaseNetworkable>(ref list);
					return;
				}
				queueInternal.RemoveWhere((Predicate<BaseNetworkable>)((BaseNetworkable x) => (Object)(object)x == (Object)null || x.net?.group == null || !x.net.group.get_isGlobal()));
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	[Flags]
	public enum PlayerFlags
	{
		Unused1 = 0x1,
		Unused2 = 0x2,
		IsAdmin = 0x4,
		ReceivingSnapshot = 0x8,
		Sleeping = 0x10,
		Spectating = 0x20,
		Wounded = 0x40,
		IsDeveloper = 0x80,
		Connected = 0x100,
		ThirdPersonViewmode = 0x400,
		EyesViewmode = 0x800,
		ChatMute = 0x1000,
		NoSprint = 0x2000,
		Aiming = 0x4000,
		DisplaySash = 0x8000,
		Relaxed = 0x10000,
		SafeZone = 0x20000,
		ServerFall = 0x40000,
		Incapacitated = 0x80000,
		Workbench1 = 0x100000,
		Workbench2 = 0x200000,
		Workbench3 = 0x400000
	}

	public enum MapNoteType
	{
		Death,
		PointOfInterest
	}

	private struct FiredProjectile
	{
		public ItemDefinition itemDef;

		public ItemModProjectile itemMod;

		public Projectile projectilePrefab;

		public float firedTime;

		public float travelTime;

		public float partialTime;

		public AttackEntity weaponSource;

		public AttackEntity weaponPrefab;

		public Projectile.Modifier projectileModifier;

		public Item pickupItem;

		public float integrity;

		public float trajectoryMismatch;

		public Vector3 position;

		public Vector3 velocity;

		public Vector3 initialPosition;

		public Vector3 initialVelocity;

		public Vector3 inheritedVelocity;

		public int protection;

		public int ricochets;

		public int hits;
	}

	public enum TimeCategory
	{
		Wilderness = 1,
		Monument = 2,
		Base = 4,
		Flying = 8,
		Boating = 0x10,
		Swimming = 0x20,
		Driving = 0x40
	}

	public class LifeStoryWorkQueue : ObjectWorkQueue<BasePlayer>
	{
		protected override void RunJob(BasePlayer entity)
		{
			entity.UpdateTimeCategory();
		}

		protected override bool ShouldAdd(BasePlayer entity)
		{
			if (base.ShouldAdd(entity))
			{
				return entity.IsValid();
			}
			return false;
		}
	}

	public class SpawnPoint
	{
		public Vector3 pos;

		public Quaternion rot;
	}

	[Serializable]
	public struct CapsuleColliderInfo
	{
		public float height;

		public float radius;

		public Vector3 center;

		public CapsuleColliderInfo(float height, float radius, Vector3 center)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			this.height = height;
			this.radius = radius;
			this.center = center;
		}
	}

	[NonSerialized]
	public bool isInAir;

	[NonSerialized]
	public bool isOnPlayer;

	[NonSerialized]
	public float violationLevel;

	[NonSerialized]
	public float lastViolationTime;

	[NonSerialized]
	public float lastAdminCheatTime;

	[NonSerialized]
	public AntiHackType lastViolationType;

	[NonSerialized]
	public float vehiclePauseTime;

	[NonSerialized]
	public float speedhackPauseTime;

	[NonSerialized]
	public float speedhackDistance;

	[NonSerialized]
	public float flyhackPauseTime;

	[NonSerialized]
	public float flyhackDistanceVertical;

	[NonSerialized]
	public float flyhackDistanceHorizontal;

	[NonSerialized]
	public TimeAverageValueLookup<uint> rpcHistory = new TimeAverageValueLookup<uint>();

	public ViewModel GestureViewModel;

	private const float drinkRange = 1.5f;

	private const float drinkMovementSpeed = 0.1f;

	[NonSerialized]
	private NetworkQueueList[] networkQueue = new NetworkQueueList[2]
	{
		new NetworkQueueList(),
		new NetworkQueueList()
	};

	[NonSerialized]
	private NetworkQueueList SnapshotQueue = new NetworkQueueList();

	public const string GestureCancelString = "cancel";

	public GestureCollection gestureList;

	private TimeUntil gestureFinishedTime;

	private TimeSince blockHeldInputTimer;

	private GestureConfig currentGesture;

	public ulong currentTeam;

	public static readonly Phrase MaxTeamSizeToast = new Phrase("maxteamsizetip", "Your team is full. Remove a member to invite another player.");

	private bool sentInstrumentTeamAchievement;

	private bool sentSummerTeamAchievement;

	private const int TEAMMATE_INSTRUMENT_COUNT_ACHIEVEMENT = 4;

	private const int TEAMMATE_SUMMER_FLOATING_COUNT_ACHIEVEMENT = 4;

	private const string TEAMMATE_INSTRUMENT_ACHIEVEMENT = "TEAM_INSTRUMENTS";

	private const string TEAMMATE_SUMMER_ACHIEVEMENT = "SUMMER_INFLATABLE";

	private BasePlayer teamLeaderBuffer;

	public List<BaseMission.MissionInstance> missions = new List<BaseMission.MissionInstance>();

	private float thinkEvery = 1f;

	private float timeSinceMissionThink;

	private int _activeMission = -1;

	[NonSerialized]
	public ModelState modelState = new ModelState();

	[NonSerialized]
	private ModelState modelStateTick;

	[NonSerialized]
	private bool wantsSendModelState;

	[NonSerialized]
	private float nextModelStateUpdate;

	[NonSerialized]
	private EntityRef mounted;

	private float nextSeatSwapTime;

	public BaseEntity PetEntity;

	public IPet Pet;

	private float lastPetCommandIssuedTime;

	private bool _playerStateDirty;

	private Dictionary<int, FiredProjectile> firedProjectiles = new Dictionary<int, FiredProjectile>();

	private const int WILDERNESS = 1;

	private const int MONUMENT = 2;

	private const int BASE = 4;

	private const int FLYING = 8;

	private const int BOATING = 16;

	private const int SWIMMING = 32;

	private const int DRIVING = 64;

	[ServerVar]
	[Help("How many milliseconds to budget for processing life story updates per frame")]
	public static float lifeStoryFramebudgetms = 0.25f;

	[NonSerialized]
	public PlayerLifeStory lifeStory;

	[NonSerialized]
	public PlayerLifeStory previousLifeStory;

	private const float TimeCategoryUpdateFrequency = 7f;

	private float nextTimeCategoryUpdate;

	private bool hasSentPresenceState;

	private bool LifeStoryInWilderness;

	private bool LifeStoryInMonument;

	private bool LifeStoryInBase;

	private bool LifeStoryFlying;

	private bool LifeStoryBoating;

	private bool LifeStorySwimming;

	private bool LifeStoryDriving;

	private bool waitingForLifeStoryUpdate;

	public static LifeStoryWorkQueue lifeStoryQueue = new LifeStoryWorkQueue();

	[NonSerialized]
	public PlayerStatistics stats;

	[NonSerialized]
	public uint svActiveItemID;

	[NonSerialized]
	public float NextChatTime;

	[NonSerialized]
	public float nextSuicideTime;

	[NonSerialized]
	public float nextRespawnTime;

	protected Vector3 viewAngles;

	public const int MaxBotIdRange = 10000000;

	private float lastSubscriptionTick;

	private float lastPlayerTick;

	private float sleepStartTime = -1f;

	private float fallTickRate = 0.1f;

	private float lastFallTime;

	private float fallVelocity;

	public static ListHashSet<BasePlayer> activePlayerList = new ListHashSet<BasePlayer>(8);

	public static ListHashSet<BasePlayer> sleepingPlayerList = new ListHashSet<BasePlayer>(8);

	public static ListHashSet<BasePlayer> bots = new ListHashSet<BasePlayer>(8);

	private float cachedCraftLevel;

	private float nextCheckTime;

	private int? cachedAppToken;

	private PersistantPlayer cachedPersistantPlayer;

	private int SpectateOffset = 1000000;

	private string spectateFilter = "";

	private float lastUpdateTime = float.NegativeInfinity;

	private float cachedThreatLevel;

	[NonSerialized]
	public float weaponDrawnDuration;

	public const int serverTickRateDefault = 16;

	public const int clientTickRateDefault = 20;

	public int serverTickRate = 16;

	public int clientTickRate = 20;

	public float serverTickInterval = 0.0625f;

	public float clientTickInterval = 0.05f;

	[NonSerialized]
	private float lastTickTime;

	[NonSerialized]
	private float lastStallTime;

	[NonSerialized]
	private float lastInputTime;

	private PlayerTick lastReceivedTick = new PlayerTick();

	private float tickDeltaTime;

	private bool tickNeedsFinalizing;

	private Vector3 tickViewAngles;

	private TimeAverageValue ticksPerSecond = new TimeAverageValue();

	private TickInterpolator tickInterpolator = new TickInterpolator();

	public Deque<Vector3> eyeHistory = new Deque<Vector3>(8);

	public TickHistory tickHistory = new TickHistory();

	private float nextUnderwearValidationTime;

	private uint lastValidUnderwearSkin;

	private float woundedDuration;

	private float lastWoundedStartTime = float.NegativeInfinity;

	private float healingWhileCrawling;

	private bool woundedByFallDamage;

	private const float INCAPACITATED_HEALTH_MIN = 2f;

	private const float INCAPACITATED_HEALTH_MAX = 6f;

	[Header("BasePlayer")]
	public GameObjectRef fallDamageEffect;

	public GameObjectRef drownEffect;

	[InspectorFlags]
	public PlayerFlags playerFlags;

	[NonSerialized]
	public PlayerEyes eyes;

	[NonSerialized]
	public PlayerInventory inventory;

	[NonSerialized]
	public PlayerBlueprints blueprints;

	[NonSerialized]
	public PlayerMetabolism metabolism;

	[NonSerialized]
	public PlayerModifiers modifiers;

	private CapsuleCollider playerCollider;

	public PlayerBelt Belt;

	private Rigidbody playerRigidbody;

	[NonSerialized]
	public ulong userID;

	[NonSerialized]
	public string UserIDString;

	[NonSerialized]
	public int gamemodeteam = -1;

	[NonSerialized]
	public int reputation;

	protected string _displayName;

	private string _lastSetName;

	public const float crouchSpeed = 1.7f;

	public const float walkSpeed = 2.8f;

	public const float runSpeed = 5.5f;

	public const float crawlSpeed = 0.72f;

	private CapsuleColliderInfo playerColliderStanding;

	private CapsuleColliderInfo playerColliderDucked;

	private CapsuleColliderInfo playerColliderCrawling;

	private CapsuleColliderInfo playerColliderLyingDown;

	private ProtectionProperties cachedProtection;

	private float nextColliderRefreshTime = -1f;

	public bool clothingBlocksAiming;

	public float clothingMoveSpeedReduction;

	public float clothingWaterSpeedBonus;

	public float clothingAccuracyBonus;

	public bool equippingBlocked;

	public float eggVision;

	private PhoneController activeTelephone;

	public BaseEntity designingAIEntity;

	public Phrase LootPanelTitle => Phrase.op_Implicit(displayName);

	public bool IsReceivingSnapshot => HasPlayerFlag(PlayerFlags.ReceivingSnapshot);

	public bool IsAdmin => HasPlayerFlag(PlayerFlags.IsAdmin);

	public bool IsDeveloper => HasPlayerFlag(PlayerFlags.IsDeveloper);

	public bool IsAiming => HasPlayerFlag(PlayerFlags.Aiming);

	public bool IsFlying
	{
		get
		{
			if (modelState == null)
			{
				return false;
			}
			return modelState.get_flying();
		}
	}

	public bool IsConnected
	{
		get
		{
			if (base.isServer)
			{
				if (Net.sv == null)
				{
					return false;
				}
				if (net == null)
				{
					return false;
				}
				if (net.get_connection() == null)
				{
					return false;
				}
				return true;
			}
			return false;
		}
	}

	public bool InGesture
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)currentGesture != (Object)null)
			{
				if (!(TimeUntil.op_Implicit(gestureFinishedTime) > 0f))
				{
					return currentGesture.animationType == GestureConfig.AnimationType.Loop;
				}
				return true;
			}
			return false;
		}
	}

	private bool CurrentGestureBlocksMovement
	{
		get
		{
			if (InGesture)
			{
				return currentGesture.movementMode == GestureConfig.MovementCapabilities.NoMovement;
			}
			return false;
		}
	}

	public bool CurrentGestureIsDance
	{
		get
		{
			if (InGesture)
			{
				return currentGesture.actionType == GestureConfig.GestureActionType.DanceAchievement;
			}
			return false;
		}
	}

	public bool CurrentGestureIsFullBody
	{
		get
		{
			if (InGesture)
			{
				return currentGesture.playerModelLayer == GestureConfig.PlayerModelLayer.FullBody;
			}
			return false;
		}
	}

	private bool InGestureCancelCooldown => TimeSince.op_Implicit(blockHeldInputTimer) < 0.5f;

	public RelationshipManager.PlayerTeam Team
	{
		get
		{
			if ((Object)(object)RelationshipManager.ServerInstance == (Object)null)
			{
				return null;
			}
			return RelationshipManager.ServerInstance.FindTeam(currentTeam);
		}
	}

	public MapNote ServerCurrentMapNote
	{
		get
		{
			return State.pointOfInterest;
		}
		set
		{
			State.pointOfInterest = value;
		}
	}

	public MapNote ServerCurrentDeathNote
	{
		get
		{
			return State.deathMarker;
		}
		set
		{
			State.deathMarker = value;
		}
	}

	public bool isMounted => mounted.IsValid(base.isServer);

	public bool isMountingHidingWeapon
	{
		get
		{
			if (isMounted)
			{
				return GetMounted().CanHoldItems();
			}
			return false;
		}
	}

	public PlayerState State
	{
		get
		{
			if (userID == 0L)
			{
				throw new InvalidOperationException("Cannot get player state without a SteamID");
			}
			return SingletonComponent<ServerMgr>.Instance.playerStateManager.Get(userID);
		}
	}

	public bool hasPreviousLife => previousLifeStory != null;

	public int currentTimeCategory { get; private set; }

	public virtual BaseNpc.AiStatistics.FamilyEnum Family => BaseNpc.AiStatistics.FamilyEnum.Player;

	protected override float PositionTickRate => -1f;

	public Vector3 estimatedVelocity { get; private set; }

	public float estimatedSpeed { get; private set; }

	public float estimatedSpeed2D { get; private set; }

	public int secondsConnected { get; private set; }

	public float desyncTimeRaw { get; private set; }

	public float desyncTimeClamped { get; private set; }

	public float secondsSleeping
	{
		get
		{
			if (sleepStartTime == -1f || !IsSleeping())
			{
				return 0f;
			}
			return Time.get_time() - sleepStartTime;
		}
	}

	public static IEnumerable<BasePlayer> allPlayerList
	{
		get
		{
			Enumerator<BasePlayer> enumerator = sleepingPlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					yield return enumerator.get_Current();
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			enumerator = activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					yield return enumerator.get_Current();
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
	}

	public float currentCraftLevel
	{
		get
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			if (triggers == null)
			{
				return 0f;
			}
			if (nextCheckTime > Time.get_realtimeSinceStartup())
			{
				return cachedCraftLevel;
			}
			nextCheckTime = Time.get_realtimeSinceStartup() + Random.Range(0.4f, 0.5f);
			float num = 0f;
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerWorkbench triggerWorkbench = triggers[i] as TriggerWorkbench;
				if (!((Object)(object)triggerWorkbench == (Object)null) && !((Object)(object)triggerWorkbench.parentBench == (Object)null) && triggerWorkbench.parentBench.IsVisible(eyes.position))
				{
					float num2 = triggerWorkbench.WorkbenchLevel();
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			cachedCraftLevel = num;
			return num;
		}
	}

	public float currentComfort
	{
		get
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			if (isMounted)
			{
				num = GetMounted().GetComfort();
			}
			if (triggers == null)
			{
				return num;
			}
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerComfort triggerComfort = triggers[i] as TriggerComfort;
				if (!((Object)(object)triggerComfort == (Object)null))
				{
					float num2 = triggerComfort.CalculateComfort(((Component)this).get_transform().get_position(), this);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}
	}

	public float currentSafeLevel
	{
		get
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			if (triggers == null)
			{
				return num;
			}
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerSafeZone triggerSafeZone = triggers[i] as TriggerSafeZone;
				if (!((Object)(object)triggerSafeZone == (Object)null))
				{
					float safeLevel = triggerSafeZone.GetSafeLevel(((Component)this).get_transform().get_position());
					if (safeLevel > num)
					{
						num = safeLevel;
					}
				}
			}
			return num;
		}
	}

	public int appToken
	{
		get
		{
			if (cachedAppToken.HasValue)
			{
				return cachedAppToken.Value;
			}
			int orGenerateAppToken = SingletonComponent<ServerMgr>.Instance.persistance.GetOrGenerateAppToken(userID);
			cachedAppToken = orGenerateAppToken;
			return orGenerateAppToken;
		}
	}

	public PersistantPlayer PersistantPlayerInfo
	{
		get
		{
			if (cachedPersistantPlayer == null)
			{
				cachedPersistantPlayer = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerInfo(userID);
			}
			return cachedPersistantPlayer;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			cachedPersistantPlayer = value;
			SingletonComponent<ServerMgr>.Instance.persistance.SetPlayerInfo(userID, value);
		}
	}

	public bool IsBeingSpectated { get; private set; }

	public InputState serverInput { get; private set; } = new InputState();


	public float timeSinceLastTick
	{
		get
		{
			if (lastTickTime == 0f)
			{
				return 0f;
			}
			return Time.get_time() - lastTickTime;
		}
	}

	public float IdleTime
	{
		get
		{
			if (lastInputTime == 0f)
			{
				return 0f;
			}
			return Time.get_time() - lastInputTime;
		}
	}

	public bool isStalled
	{
		get
		{
			if (IsDead())
			{
				return false;
			}
			if (IsSleeping())
			{
				return false;
			}
			return timeSinceLastTick > 1f;
		}
	}

	public bool wasStalled
	{
		get
		{
			if (isStalled)
			{
				lastStallTime = Time.get_time();
			}
			return Time.get_time() - lastStallTime < 1f;
		}
	}

	public int tickHistoryCapacity => Mathf.Max(1, Mathf.CeilToInt((float)ticksPerSecond.Calculate() * ConVar.AntiHack.tickhistorytime));

	public Matrix4x4 tickHistoryMatrix
	{
		get
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (!Object.op_Implicit((Object)(object)((Component)this).get_transform().get_parent()))
			{
				return Matrix4x4.get_identity();
			}
			return ((Component)this).get_transform().get_parent().get_localToWorldMatrix();
		}
	}

	public float TimeSinceWoundedStarted => Time.get_realtimeSinceStartup() - lastWoundedStartTime;

	public Connection Connection
	{
		get
		{
			if (net != null)
			{
				return net.get_connection();
			}
			return null;
		}
	}

	public string displayName
	{
		get
		{
			return _displayName;
		}
		set
		{
			if (!(_lastSetName == value))
			{
				_lastSetName = value;
				_displayName = SanitizePlayerNameString(value, userID);
			}
		}
	}

	public override TraitFlag Traits => base.Traits | TraitFlag.Human | TraitFlag.Food | TraitFlag.Meat | TraitFlag.Alive;

	public bool HasActiveTelephone => (Object)(object)activeTelephone != (Object)null;

	public bool IsDesigningAI => (Object)(object)designingAIEntity != (Object)null;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BasePlayer.OnRpcMessage", 0);
		try
		{
			RPCMessage rPCMessage;
			if (rpc == 935768323 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ClientKeepConnectionAlive "));
				}
				TimeWarning val2 = TimeWarning.New("ClientKeepConnectionAlive", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(935768323u, "ClientKeepConnectionAlive", this, player))
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
							ClientKeepConnectionAlive(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ClientKeepConnectionAlive");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3782818894u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ClientLoadingComplete "));
				}
				TimeWarning val2 = TimeWarning.New("ClientLoadingComplete", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(3782818894u, "ClientLoadingComplete", this, player))
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
							ClientLoadingComplete(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ClientLoadingComplete");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1497207530 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - IssuePetCommand "));
				}
				TimeWarning val2 = TimeWarning.New("IssuePetCommand", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg4 = rPCMessage;
						IssuePetCommand(msg4);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex3)
				{
					Debug.LogException(ex3);
					player.Kick("RPC Error in IssuePetCommand");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2041023702 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - IssuePetCommandRaycast "));
				}
				TimeWarning val2 = TimeWarning.New("IssuePetCommandRaycast", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg5 = rPCMessage;
						IssuePetCommandRaycast(msg5);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex4)
				{
					Debug.LogException(ex4);
					player.Kick("RPC Error in IssuePetCommandRaycast");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1998170713 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - OnPlayerLanded "));
				}
				TimeWarning val2 = TimeWarning.New("OnPlayerLanded", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(1998170713u, "OnPlayerLanded", this, player))
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
							RPCMessage msg6 = rPCMessage;
							OnPlayerLanded(msg6);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in OnPlayerLanded");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2147041557 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - OnPlayerReported "));
				}
				TimeWarning val2 = TimeWarning.New("OnPlayerReported", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2147041557u, "OnPlayerReported", this, player, 1uL))
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
							RPCMessage msg7 = rPCMessage;
							OnPlayerReported(msg7);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in OnPlayerReported");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 363681694 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - OnProjectileAttack "));
				}
				TimeWarning val2 = TimeWarning.New("OnProjectileAttack", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(363681694u, "OnProjectileAttack", this, player))
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
							RPCMessage msg8 = rPCMessage;
							OnProjectileAttack(msg8);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in OnProjectileAttack");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1500391289 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - OnProjectileRicochet "));
				}
				TimeWarning val2 = TimeWarning.New("OnProjectileRicochet", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(1500391289u, "OnProjectileRicochet", this, player))
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
							RPCMessage msg9 = rPCMessage;
							OnProjectileRicochet(msg9);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex8)
					{
						Debug.LogException(ex8);
						player.Kick("RPC Error in OnProjectileRicochet");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2324190493u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - OnProjectileUpdate "));
				}
				TimeWarning val2 = TimeWarning.New("OnProjectileUpdate", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(2324190493u, "OnProjectileUpdate", this, player))
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
							RPCMessage msg10 = rPCMessage;
							OnProjectileUpdate(msg10);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex9)
					{
						Debug.LogException(ex9);
						player.Kick("RPC Error in OnProjectileUpdate");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3167788018u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - PerformanceReport "));
				}
				TimeWarning val2 = TimeWarning.New("PerformanceReport", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3167788018u, "PerformanceReport", this, player, 1uL))
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
							RPCMessage msg11 = rPCMessage;
							PerformanceReport(msg11);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex10)
					{
						Debug.LogException(ex10);
						player.Kick("RPC Error in PerformanceReport");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 52352806 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RequestRespawnInformation "));
				}
				TimeWarning val2 = TimeWarning.New("RequestRespawnInformation", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(52352806u, "RequestRespawnInformation", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(52352806u, "RequestRespawnInformation", this, player))
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
							RPCMessage msg12 = rPCMessage;
							RequestRespawnInformation(msg12);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex11)
					{
						Debug.LogException(ex11);
						player.Kick("RPC Error in RequestRespawnInformation");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 970468557 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Assist "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Assist", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(970468557u, "RPC_Assist", this, player, 3f))
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
							RPCMessage msg13 = rPCMessage;
							RPC_Assist(msg13);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex12)
					{
						Debug.LogException(ex12);
						player.Kick("RPC Error in RPC_Assist");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3263238541u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_KeepAlive "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_KeepAlive", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3263238541u, "RPC_KeepAlive", this, player, 3f))
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
							RPCMessage msg14 = rPCMessage;
							RPC_KeepAlive(msg14);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex13)
					{
						Debug.LogException(ex13);
						player.Kick("RPC Error in RPC_KeepAlive");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3692395068u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_LootPlayer "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_LootPlayer", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3692395068u, "RPC_LootPlayer", this, player, 3f))
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
							RPCMessage msg15 = rPCMessage;
							RPC_LootPlayer(msg15);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex14)
					{
						Debug.LogException(ex14);
						player.Kick("RPC Error in RPC_LootPlayer");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1539133504 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_StartClimb "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_StartClimb", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg16 = rPCMessage;
						RPC_StartClimb(msg16);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex15)
				{
					Debug.LogException(ex15);
					player.Kick("RPC Error in RPC_StartClimb");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3047177092u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_AddMarker "));
				}
				TimeWarning val2 = TimeWarning.New("Server_AddMarker", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(3047177092u, "Server_AddMarker", this, player))
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
							RPCMessage msg17 = rPCMessage;
							Server_AddMarker(msg17);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex16)
					{
						Debug.LogException(ex16);
						player.Kick("RPC Error in Server_AddMarker");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1005040107 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_CancelGesture "));
				}
				TimeWarning val2 = TimeWarning.New("Server_CancelGesture", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1005040107u, "Server_CancelGesture", this, player, 10uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(1005040107u, "Server_CancelGesture", this, player))
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
							Server_CancelGesture();
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex17)
					{
						Debug.LogException(ex17);
						player.Kick("RPC Error in Server_CancelGesture");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 706157120 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_ClearMapMarkers "));
				}
				TimeWarning val2 = TimeWarning.New("Server_ClearMapMarkers", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(706157120u, "Server_ClearMapMarkers", this, player))
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
							RPCMessage msg18 = rPCMessage;
							Server_ClearMapMarkers(msg18);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex18)
					{
						Debug.LogException(ex18);
						player.Kick("RPC Error in Server_ClearMapMarkers");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 31713840 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_RemovePointOfInterest "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RemovePointOfInterest", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(31713840u, "Server_RemovePointOfInterest", this, player))
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
							RPCMessage msg19 = rPCMessage;
							Server_RemovePointOfInterest(msg19);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex19)
					{
						Debug.LogException(ex19);
						player.Kick("RPC Error in Server_RemovePointOfInterest");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2567683804u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_RequestMarkers "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RequestMarkers", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(2567683804u, "Server_RequestMarkers", this, player))
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
							RPCMessage msg20 = rPCMessage;
							Server_RequestMarkers(msg20);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex20)
					{
						Debug.LogException(ex20);
						player.Kick("RPC Error in Server_RequestMarkers");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1572722245 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_StartGesture "));
				}
				TimeWarning val2 = TimeWarning.New("Server_StartGesture", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1572722245u, "Server_StartGesture", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(1572722245u, "Server_StartGesture", this, player))
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
							RPCMessage msg21 = rPCMessage;
							Server_StartGesture(msg21);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex21)
					{
						Debug.LogException(ex21);
						player.Kick("RPC Error in Server_StartGesture");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3635568749u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerRPC_UnderwearChange "));
				}
				TimeWarning val2 = TimeWarning.New("ServerRPC_UnderwearChange", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg22 = rPCMessage;
						ServerRPC_UnderwearChange(msg22);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex22)
				{
					Debug.LogException(ex22);
					player.Kick("RPC Error in ServerRPC_UnderwearChange");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 970114602 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SV_Drink "));
				}
				TimeWarning val2 = TimeWarning.New("SV_Drink", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.get_read();
						RPCMessage msg23 = rPCMessage;
						SV_Drink(msg23);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex23)
				{
					Debug.LogException(ex23);
					player.Kick("RPC Error in SV_Drink");
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

	public bool TriggeredAntiHack(float seconds = 1f, float score = float.PositiveInfinity)
	{
		if (!(Time.get_realtimeSinceStartup() - lastViolationTime < seconds))
		{
			return violationLevel > score;
		}
		return true;
	}

	public bool UsedAdminCheat(float seconds = 2f)
	{
		return Time.get_realtimeSinceStartup() - lastAdminCheatTime < seconds;
	}

	public void PauseVehicleNoClipDetection(float seconds = 1f)
	{
		vehiclePauseTime = Mathf.Max(vehiclePauseTime, seconds);
	}

	public void PauseFlyHackDetection(float seconds = 1f)
	{
		flyhackPauseTime = Mathf.Max(flyhackPauseTime, seconds);
	}

	public void PauseSpeedHackDetection(float seconds = 1f)
	{
		speedhackPauseTime = Mathf.Max(speedhackPauseTime, seconds);
	}

	public int GetAntiHackKicks()
	{
		return AntiHack.GetKickRecord(this);
	}

	public void ResetAntiHack()
	{
		violationLevel = 0f;
		lastViolationTime = 0f;
		lastAdminCheatTime = 0f;
		speedhackPauseTime = 0f;
		speedhackDistance = 0f;
		flyhackPauseTime = 0f;
		flyhackDistanceVertical = 0f;
		flyhackDistanceHorizontal = 0f;
		rpcHistory.Clear();
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		if ((Object)(object)player == (Object)(object)this)
		{
			return false;
		}
		if (!IsWounded())
		{
			return IsSleeping();
		}
		return true;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_LootPlayer(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (Object.op_Implicit((Object)(object)player) && player.CanInteract() && CanBeLooted(player) && player.inventory.loot.StartLootingEntity(this))
		{
			player.inventory.loot.AddContainer(inventory.containerMain);
			player.inventory.loot.AddContainer(inventory.containerWear);
			player.inventory.loot.AddContainer(inventory.containerBelt);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer(null, player, "RPC_OpenLootPanel", "player_corpse");
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_Assist(RPCMessage msg)
	{
		if (msg.player.CanInteract() && !((Object)(object)msg.player == (Object)(object)this) && IsWounded())
		{
			StopWounded(msg.player);
			msg.player.stats.Add("wounded_assisted", 1, (Stats)5);
			stats.Add("wounded_healed", 1);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_KeepAlive(RPCMessage msg)
	{
		if (msg.player.CanInteract() && !((Object)(object)msg.player == (Object)(object)this) && IsWounded())
		{
			ProlongWounding(10f);
		}
	}

	[RPC_Server]
	private void SV_Drink(RPCMessage msg)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		Vector3 val = msg.read.Vector3();
		if (Vector3Ex.IsNaNOrInfinity(val) || !Object.op_Implicit((Object)(object)player) || !player.metabolism.CanConsume() || Vector3.Distance(((Component)player).get_transform().get_position(), val) > 5f || !WaterLevel.Test(val, waves: true, this) || (isMounted && !GetMounted().canDrinkWhileMounted))
		{
			return;
		}
		ItemDefinition atPoint = WaterResource.GetAtPoint(val);
		if (!((Object)(object)atPoint == (Object)null))
		{
			ItemModConsumable component = ((Component)atPoint).GetComponent<ItemModConsumable>();
			Item item = ItemManager.Create(atPoint, component.amountToConsume, 0uL);
			ItemModConsume component2 = ((Component)item.info).GetComponent<ItemModConsume>();
			if (component2.CanDoAction(item, player))
			{
				component2.DoAction(item, player);
			}
			item?.Remove();
			player.metabolism.MarkConsumption();
		}
	}

	[RPC_Server]
	public void RPC_StartClimb(RPCMessage msg)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		bool flag = msg.read.Bit();
		Vector3 val = msg.read.Vector3();
		uint num = msg.read.UInt32();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(num);
		Vector3 val2 = (flag ? ((Component)baseNetworkable).get_transform().TransformPoint(val) : val);
		if (!player.isMounted || player.Distance(val2) > 5f || !GamePhysics.LineOfSight(player.eyes.position, val2, 1218519041) || !GamePhysics.LineOfSight(val2, val2 + player.eyes.offset, 1218519041))
		{
			return;
		}
		Vector3 val3 = val2 - player.eyes.position;
		Vector3 end = val2 - ((Vector3)(ref val3)).get_normalized() * 0.25f;
		if (!GamePhysics.CheckCapsule(player.eyes.position, end, 0.25f, 1218519041, (QueryTriggerInteraction)0) && !AntiHack.TestNoClipping(player, val2 + player.NoClipOffset(), val2 + player.NoClipOffset(), player.NoClipRadius(ConVar.AntiHack.noclip_margin), ConVar.AntiHack.noclip_backtracking, sphereCast: true))
		{
			player.EnsureDismounted();
			((Component)player).get_transform().set_position(val2);
			Collider component = ((Component)player).GetComponent<Collider>();
			component.set_enabled(false);
			component.set_enabled(true);
			player.ForceUpdateTriggers();
			if (flag)
			{
				player.ClientRPCPlayer<Vector3, uint>(null, player, "ForcePositionToParentOffset", val, num);
			}
			else
			{
				player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", val2);
			}
		}
	}

	public int GetQueuedUpdateCount(NetworkQueue queue)
	{
		return networkQueue[(int)queue].Length;
	}

	public void SendSnapshots(ListHashSet<Networkable> ents)
	{
		TimeWarning val = TimeWarning.New("SendSnapshots", 0);
		try
		{
			int count = ents.get_Values().get_Count();
			Networkable[] buffer = ents.get_Values().get_Buffer();
			for (int i = 0; i < count; i++)
			{
				SnapshotQueue.Add(buffer[i].handler as BaseNetworkable);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void QueueUpdate(NetworkQueue queue, BaseNetworkable ent)
	{
		if (!IsConnected)
		{
			return;
		}
		switch (queue)
		{
		case NetworkQueue.Update:
			networkQueue[0].Add(ent);
			break;
		case NetworkQueue.UpdateDistance:
			if (!IsReceivingSnapshot && !networkQueue[1].Contains(ent) && !networkQueue[0].Contains(ent))
			{
				NetworkQueueList networkQueueList = networkQueue[1];
				if (Distance(ent as BaseEntity) < 20f)
				{
					QueueUpdate(NetworkQueue.Update, ent);
				}
				else
				{
					networkQueueList.Add(ent);
				}
			}
			break;
		}
	}

	public void SendEntityUpdate()
	{
		TimeWarning val = TimeWarning.New("SendEntityUpdate", 0);
		try
		{
			SendEntityUpdates(SnapshotQueue);
			SendEntityUpdates(networkQueue[0]);
			SendEntityUpdates(networkQueue[1]);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void ClearEntityQueue(Group group = null)
	{
		SnapshotQueue.Clear(group);
		networkQueue[0].Clear(group);
		networkQueue[1].Clear(group);
	}

	private void SendEntityUpdates(NetworkQueueList queue)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (queue.queueInternal.get_Count() == 0)
		{
			return;
		}
		int num = (IsReceivingSnapshot ? ConVar.Server.updatebatchspawn : ConVar.Server.updatebatch);
		List<BaseNetworkable> list = Pool.GetList<BaseNetworkable>();
		TimeWarning val = TimeWarning.New("SendEntityUpdates.SendEntityUpdates", 0);
		try
		{
			int num2 = 0;
			Enumerator<BaseNetworkable> enumerator = queue.queueInternal.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BaseNetworkable current = enumerator.get_Current();
					SendEntitySnapshot(current);
					list.Add(current);
					num2++;
					if (num2 > num)
					{
						break;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (num > queue.queueInternal.get_Count())
		{
			queue.queueInternal.Clear();
		}
		else
		{
			val = TimeWarning.New("SendEntityUpdates.Remove", 0);
			try
			{
				for (int i = 0; i < list.Count; i++)
				{
					queue.queueInternal.Remove(list[i]);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (queue.queueInternal.get_Count() == 0 && queue.MaxLength > 2048)
		{
			queue.queueInternal.Clear();
			queue.queueInternal = new HashSet<BaseNetworkable>();
			queue.MaxLength = 0;
		}
		Pool.FreeList<BaseNetworkable>(ref list);
	}

	private void SendEntitySnapshot(BaseNetworkable ent)
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("SendEntitySnapshot", 0);
		try
		{
			if (!((Object)(object)ent == (Object)null) && ent.net != null && ent.ShouldNetworkTo(this) && ((BaseNetwork)Net.sv).get_write().Start())
			{
				net.get_connection().validate.entityUpdates++;
				SaveInfo saveInfo = default(SaveInfo);
				saveInfo.forConnection = net.get_connection();
				saveInfo.forDisk = false;
				SaveInfo saveInfo2 = saveInfo;
				((BaseNetwork)Net.sv).get_write().PacketID((Type)5);
				((BaseNetwork)Net.sv).get_write().UInt32(net.get_connection().validate.entityUpdates);
				ent.ToStreamForNetwork((Stream)(object)((BaseNetwork)Net.sv).get_write(), saveInfo2);
				((BaseNetwork)Net.sv).get_write().Send(new SendInfo(net.get_connection()));
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public bool HasPlayerFlag(PlayerFlags f)
	{
		return (playerFlags & f) == f;
	}

	public void SetPlayerFlag(PlayerFlags f, bool b)
	{
		if (b)
		{
			if (HasPlayerFlag(f))
			{
				return;
			}
			playerFlags |= f;
		}
		else
		{
			if (!HasPlayerFlag(f))
			{
				return;
			}
			playerFlags &= ~f;
		}
		SendNetworkUpdate();
	}

	public void LightToggle(bool mask = true)
	{
		Item activeItem = GetActiveItem();
		if (activeItem != null)
		{
			BaseEntity heldEntity = activeItem.GetHeldEntity();
			if ((Object)(object)heldEntity != (Object)null)
			{
				HeldEntity component = ((Component)heldEntity).GetComponent<HeldEntity>();
				if (Object.op_Implicit((Object)(object)component))
				{
					((Component)component).SendMessage("SetLightsOn", (object)(mask && !component.LightsOn()), (SendMessageOptions)1);
				}
			}
		}
		foreach (Item item in inventory.containerWear.itemList)
		{
			ItemModWearable component2 = ((Component)item.info).GetComponent<ItemModWearable>();
			if (Object.op_Implicit((Object)(object)component2) && component2.emissive)
			{
				item.SetFlag(Item.Flag.IsOn, mask && !item.HasFlag(Item.Flag.IsOn));
				item.MarkDirty();
			}
		}
		if (isMounted)
		{
			GetMounted().LightToggle(this);
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(1uL)]
	private void Server_StartGesture(RPCMessage msg)
	{
		if (!InGesture && !IsGestureBlocked())
		{
			uint id = msg.read.UInt32();
			GestureConfig toPlay = gestureList.IdToGesture(id);
			Server_StartGesture(toPlay);
		}
	}

	public void Server_StartGesture(GestureConfig toPlay)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)toPlay != (Object)null) || !toPlay.IsOwnedBy(this) || !toPlay.CanBeUsedBy(this))
		{
			return;
		}
		if (toPlay.animationType == GestureConfig.AnimationType.OneShot)
		{
			((FacepunchBehaviour)this).Invoke((Action)TimeoutGestureServer, toPlay.duration);
		}
		else if (toPlay.animationType == GestureConfig.AnimationType.Loop)
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)MonitorLoopingGesture, 0f, 0f);
		}
		ClientRPC(null, "Client_StartGesture", toPlay.gestureId);
		gestureFinishedTime = TimeUntil.op_Implicit(toPlay.duration);
		currentGesture = toPlay;
		if (toPlay.actionType == GestureConfig.GestureActionType.DanceAchievement)
		{
			TriggerDanceAchievement triggerDanceAchievement = FindTrigger<TriggerDanceAchievement>();
			if ((Object)(object)triggerDanceAchievement != (Object)null)
			{
				triggerDanceAchievement.NotifyDanceStarted();
			}
		}
	}

	private void TimeoutGestureServer()
	{
		currentGesture = null;
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(10uL)]
	public void Server_CancelGesture()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		currentGesture = null;
		blockHeldInputTimer = TimeSince.op_Implicit(0f);
		ClientRPC(null, "Client_RemoteCancelledGesture");
		((FacepunchBehaviour)this).CancelInvoke((Action)MonitorLoopingGesture);
	}

	private void MonitorLoopingGesture()
	{
		if (modelState.get_ducked() || modelState.get_sleeping() || IsWounded() || IsSwimming() || IsDead() || (isMounted && GetMounted().allowedGestures == BaseMountable.MountGestureType.UpperBody && currentGesture.playerModelLayer == GestureConfig.PlayerModelLayer.FullBody) || (isMounted && GetMounted().allowedGestures == BaseMountable.MountGestureType.None))
		{
			Server_CancelGesture();
		}
	}

	private void NotifyGesturesNewItemEquipped()
	{
		if (InGesture)
		{
			Server_CancelGesture();
		}
	}

	private bool IsGestureBlocked()
	{
		if (isMounted && GetMounted().allowedGestures == BaseMountable.MountGestureType.None)
		{
			return true;
		}
		if (Object.op_Implicit((Object)(object)GetHeldEntity()) && GetHeldEntity().BlocksGestures())
		{
			return true;
		}
		if (!IsWounded() && !((Object)(object)currentGesture != (Object)null) && !IsDead())
		{
			return IsSleeping();
		}
		return true;
	}

	public void DelayedTeamUpdate()
	{
		UpdateTeam(currentTeam);
	}

	public void TeamUpdate()
	{
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		if (!RelationshipManager.TeamsEnabled() || !IsConnected || currentTeam == 0L)
		{
			return;
		}
		RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindTeam(currentTeam);
		if (playerTeam == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		PlayerTeam val = Pool.Get<PlayerTeam>();
		try
		{
			val.teamLeader = playerTeam.teamLeader;
			val.teamID = playerTeam.teamID;
			val.teamName = playerTeam.teamName;
			val.members = Pool.GetList<TeamMember>();
			val.teamLifetime = playerTeam.teamLifetime;
			foreach (ulong member in playerTeam.members)
			{
				BasePlayer basePlayer = RelationshipManager.FindByID(member);
				TeamMember val2 = Pool.Get<TeamMember>();
				val2.displayName = (((Object)(object)basePlayer != (Object)null) ? basePlayer.displayName : (SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(member) ?? "DEAD"));
				val2.healthFraction = (((Object)(object)basePlayer != (Object)null) ? basePlayer.healthFraction : 0f);
				val2.position = (((Object)(object)basePlayer != (Object)null) ? ((Component)basePlayer).get_transform().get_position() : Vector3.get_zero());
				val2.online = (Object)(object)basePlayer != (Object)null && !basePlayer.IsSleeping();
				if ((!sentInstrumentTeamAchievement || !sentSummerTeamAchievement) && (Object)(object)basePlayer != (Object)null)
				{
					if (Object.op_Implicit((Object)(object)basePlayer.GetHeldEntity()) && basePlayer.GetHeldEntity().IsInstrument())
					{
						num++;
					}
					if (basePlayer.isMounted)
					{
						if (basePlayer.GetMounted().IsInstrument())
						{
							num++;
						}
						if (basePlayer.GetMounted().IsSummerDlcVehicle)
						{
							num2++;
						}
					}
					if (num >= 4 && !sentInstrumentTeamAchievement)
					{
						GiveAchievement("TEAM_INSTRUMENTS");
						sentInstrumentTeamAchievement = true;
					}
					if (num2 >= 4)
					{
						GiveAchievement("SUMMER_INFLATABLE");
						sentSummerTeamAchievement = true;
					}
				}
				val2.userID = member;
				val.members.Add(val2);
			}
			teamLeaderBuffer = FindByID(playerTeam.teamLeader);
			if ((Object)(object)teamLeaderBuffer != (Object)null)
			{
				val.mapNote = teamLeaderBuffer.ServerCurrentMapNote;
			}
			ClientRPCPlayerAndSpectators<PlayerTeam>(null, this, "CLIENT_ReceiveTeamInfo", val);
			val.mapNote = null;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void UpdateTeam(ulong newTeam)
	{
		currentTeam = newTeam;
		SendNetworkUpdate();
		if (RelationshipManager.ServerInstance.FindTeam(newTeam) == null)
		{
			ClearTeam();
		}
		else
		{
			TeamUpdate();
		}
	}

	public void ClearTeam()
	{
		currentTeam = 0uL;
		ClientRPCPlayerAndSpectators(null, this, "CLIENT_ClearTeam");
		SendNetworkUpdate();
	}

	public void ClearPendingInvite()
	{
		ClientRPCPlayer(null, this, "CLIENT_PendingInvite", "", 0);
	}

	public HeldEntity GetHeldEntity()
	{
		if (base.isServer)
		{
			Item activeItem = GetActiveItem();
			if (activeItem == null)
			{
				return null;
			}
			return activeItem.GetHeldEntity() as HeldEntity;
		}
		return null;
	}

	public bool IsHoldingEntity<T>()
	{
		HeldEntity heldEntity = GetHeldEntity();
		if ((Object)(object)heldEntity == (Object)null)
		{
			return false;
		}
		return heldEntity is T;
	}

	public bool IsHostileItem(Item item)
	{
		if (!item.info.isHoldable)
		{
			return false;
		}
		ItemModEntity component = ((Component)item.info).GetComponent<ItemModEntity>();
		if ((Object)(object)component == (Object)null)
		{
			return false;
		}
		GameObject val = component.entityPrefab.Get();
		if ((Object)(object)val == (Object)null)
		{
			return false;
		}
		AttackEntity component2 = val.GetComponent<AttackEntity>();
		if ((Object)(object)component2 == (Object)null)
		{
			return false;
		}
		return component2.hostile;
	}

	public bool IsItemHoldRestricted(Item item)
	{
		if (IsNpc)
		{
			return false;
		}
		if (InSafeZone() && item != null && IsHostileItem(item))
		{
			return true;
		}
		return false;
	}

	public void Server_LogDeathMarker(Vector3 position)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!IsNpc)
		{
			if (ServerCurrentDeathNote == null)
			{
				ServerCurrentDeathNote = Pool.Get<MapNote>();
				ServerCurrentDeathNote.noteType = 0;
			}
			ServerCurrentDeathNote.worldPosition = position;
			ClientRPCPlayer<MapNote>(null, this, "Client_AddNewDeathMarker", ServerCurrentDeathNote);
			DirtyPlayerState();
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void Server_AddMarker(RPCMessage msg)
	{
		MapNote serverCurrentMapNote = ServerCurrentMapNote;
		if (serverCurrentMapNote != null)
		{
			serverCurrentMapNote.Dispose();
		}
		ServerCurrentMapNote = MapNote.Deserialize((Stream)(object)msg.read);
		DirtyPlayerState();
		TeamUpdate();
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void Server_RemovePointOfInterest(RPCMessage msg)
	{
		if (ServerCurrentMapNote != null)
		{
			ServerCurrentMapNote.Dispose();
			ServerCurrentMapNote = null;
			DirtyPlayerState();
			TeamUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void Server_RequestMarkers(RPCMessage msg)
	{
		SendMarkersToClient();
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void Server_ClearMapMarkers(RPCMessage msg)
	{
		MapNote serverCurrentDeathNote = ServerCurrentDeathNote;
		if (serverCurrentDeathNote != null)
		{
			serverCurrentDeathNote.Dispose();
		}
		ServerCurrentDeathNote = null;
		MapNote serverCurrentMapNote = ServerCurrentMapNote;
		if (serverCurrentMapNote != null)
		{
			serverCurrentMapNote.Dispose();
		}
		ServerCurrentMapNote = null;
		DirtyPlayerState();
		TeamUpdate();
	}

	private void SendMarkersToClient()
	{
		MapNoteList val = Pool.Get<MapNoteList>();
		try
		{
			val.notes = Pool.GetList<MapNote>();
			if (ServerCurrentDeathNote != null)
			{
				val.notes.Add(ServerCurrentDeathNote);
			}
			if (ServerCurrentMapNote != null)
			{
				val.notes.Add(ServerCurrentMapNote);
			}
			ClientRPCPlayer<MapNoteList>(null, this, "Client_ReceiveMarkers", val);
			val.notes.Clear();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public bool HasAttemptedMission(uint missionID)
	{
		foreach (BaseMission.MissionInstance mission in missions)
		{
			if (mission.missionID == missionID)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanAcceptMission(uint missionID)
	{
		if (HasActiveMission())
		{
			return false;
		}
		if (!BaseMission.missionsenabled)
		{
			return false;
		}
		BaseMission fromID = MissionManifest.GetFromID(missionID);
		if (fromID == null)
		{
			Debug.LogError((object)("MISSION NOT FOUND IN MANIFEST, ID :" + missionID));
			return false;
		}
		if (fromID.acceptDependancies != null && fromID.acceptDependancies.Length != 0)
		{
			BaseMission.MissionDependancy[] acceptDependancies = fromID.acceptDependancies;
			foreach (BaseMission.MissionDependancy missionDependancy in acceptDependancies)
			{
				if (missionDependancy.everAttempted)
				{
					continue;
				}
				bool flag = false;
				foreach (BaseMission.MissionInstance mission in missions)
				{
					if (mission.missionID == missionDependancy.targetMissionID && mission.status == missionDependancy.targetMissionDesiredStatus)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
		}
		if (IsMissionActive(missionID))
		{
			return false;
		}
		if (fromID.isRepeatable)
		{
			bool num = HasCompletedMission(missionID);
			bool flag2 = HasFailedMission(missionID);
			if (num && fromID.repeatDelaySecondsSuccess == -1)
			{
				return false;
			}
			if (flag2 && fromID.repeatDelaySecondsFailed == -1)
			{
				return false;
			}
			foreach (BaseMission.MissionInstance mission2 in missions)
			{
				if (mission2.missionID == missionID)
				{
					float num2 = 0f;
					if (mission2.status == BaseMission.MissionStatus.Completed)
					{
						num2 = fromID.repeatDelaySecondsSuccess;
					}
					else if (mission2.status == BaseMission.MissionStatus.Failed)
					{
						num2 = fromID.repeatDelaySecondsFailed;
					}
					float endTime = mission2.endTime;
					if (Time.get_time() - endTime < num2)
					{
						return false;
					}
				}
			}
		}
		BaseMission.PositionGenerator[] positionGenerators = fromID.positionGenerators;
		for (int i = 0; i < positionGenerators.Length; i++)
		{
			if (!positionGenerators[i].Validate(this))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsMissionActive(uint missionID)
	{
		foreach (BaseMission.MissionInstance mission in missions)
		{
			if (mission.missionID == missionID && (mission.status == BaseMission.MissionStatus.Active || mission.status == BaseMission.MissionStatus.Accomplished))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasCompletedMission(uint missionID)
	{
		foreach (BaseMission.MissionInstance mission in missions)
		{
			if (mission.missionID == missionID && mission.status == BaseMission.MissionStatus.Completed)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasFailedMission(uint missionID)
	{
		foreach (BaseMission.MissionInstance mission in missions)
		{
			if (mission.missionID == missionID && mission.status == BaseMission.MissionStatus.Failed)
			{
				return true;
			}
		}
		return false;
	}

	private void WipeMissions()
	{
		if (missions.Count > 0)
		{
			for (int num = missions.Count - 1; num >= 0; num--)
			{
				BaseMission.MissionInstance missionInstance = missions[num];
				if (missionInstance != null)
				{
					missionInstance.GetMission().MissionFailed(missionInstance, this);
					Pool.Free<BaseMission.MissionInstance>(ref missionInstance);
				}
			}
		}
		missions.Clear();
		SetActiveMission(-1);
		MissionDirty();
	}

	public void AbandonActiveMission()
	{
		if (HasActiveMission())
		{
			int activeMission = GetActiveMission();
			if (activeMission != -1 && activeMission < missions.Count)
			{
				BaseMission.MissionInstance missionInstance = missions[activeMission];
				missionInstance.GetMission().MissionFailed(missionInstance, this);
			}
		}
	}

	public void AddMission(BaseMission.MissionInstance instance)
	{
		missions.Add(instance);
		MissionDirty();
	}

	public void ThinkMissions(float delta)
	{
		if (!BaseMission.missionsenabled)
		{
			return;
		}
		if (timeSinceMissionThink < thinkEvery)
		{
			timeSinceMissionThink += delta;
			return;
		}
		foreach (BaseMission.MissionInstance mission in missions)
		{
			mission.Think(this, timeSinceMissionThink);
		}
		timeSinceMissionThink = 0f;
	}

	public void ClearMissions()
	{
		missions.Clear();
		State.missions = SaveMissions();
		DirtyPlayerState();
	}

	public void MissionDirty(bool shouldSendNetworkUpdate = true)
	{
		if (BaseMission.missionsenabled)
		{
			State.missions = SaveMissions();
			DirtyPlayerState();
			if (shouldSendNetworkUpdate)
			{
				SendNetworkUpdate();
			}
		}
	}

	public void ProcessMissionEvent(BaseMission.MissionEventType type, string identifier, float amount)
	{
		if (!BaseMission.missionsenabled)
		{
			return;
		}
		foreach (BaseMission.MissionInstance mission in missions)
		{
			mission.ProcessMissionEvent(this, type, identifier, amount);
		}
	}

	private Missions SaveMissions()
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		Missions val = Pool.Get<Missions>();
		val.missions = Pool.GetList<MissionInstance>();
		val.activeMission = GetActiveMission();
		val.protocol = 220;
		val.seed = World.Seed;
		val.saveCreatedTime = Epoch.FromDateTime(SaveRestore.SaveCreatedTime);
		foreach (BaseMission.MissionInstance mission in missions)
		{
			MissionInstance val2 = Pool.Get<MissionInstance>();
			val2.providerID = mission.providerID;
			val2.missionID = mission.missionID;
			val2.missionStatus = (uint)mission.status;
			val2.completionScale = mission.completionScale;
			val2.startTime = Time.get_realtimeSinceStartup() - mission.startTime;
			val2.endTime = mission.endTime;
			val2.missionLocation = mission.missionLocation;
			val2.missionPoints = Pool.GetList<MissionPoint>();
			foreach (KeyValuePair<string, Vector3> missionPoint in mission.missionPoints)
			{
				MissionPoint val3 = Pool.Get<MissionPoint>();
				val3.identifier = missionPoint.Key;
				val3.location = missionPoint.Value;
				val2.missionPoints.Add(val3);
			}
			val2.objectiveStatuses = Pool.GetList<ObjectiveStatus>();
			BaseMission.MissionInstance.ObjectiveStatus[] objectiveStatuses = mission.objectiveStatuses;
			foreach (BaseMission.MissionInstance.ObjectiveStatus objectiveStatus in objectiveStatuses)
			{
				ObjectiveStatus val4 = Pool.Get<ObjectiveStatus>();
				val4.completed = objectiveStatus.completed;
				val4.failed = objectiveStatus.failed;
				val4.started = objectiveStatus.started;
				val4.genericFloat1 = objectiveStatus.genericFloat1;
				val4.genericInt1 = objectiveStatus.genericInt1;
				val2.objectiveStatuses.Add(val4);
			}
			val2.createdEntities = Pool.GetList<uint>();
			if (mission.createdEntities != null)
			{
				foreach (MissionEntity createdEntity in mission.createdEntities)
				{
					if (!((Object)(object)createdEntity == (Object)null))
					{
						BaseEntity entity = createdEntity.GetEntity();
						if (Object.op_Implicit((Object)(object)entity))
						{
							val2.createdEntities.Add(entity.net.ID);
						}
					}
				}
			}
			if (mission.rewards != null && mission.rewards.Length != 0)
			{
				val2.rewards = Pool.GetList<MissionReward>();
				ItemAmount[] rewards = mission.rewards;
				foreach (ItemAmount itemAmount in rewards)
				{
					MissionReward val5 = Pool.Get<MissionReward>();
					val5.itemID = itemAmount.itemid;
					val5.itemAmount = Mathf.FloorToInt(itemAmount.amount);
					val2.rewards.Add(val5);
				}
			}
			val.missions.Add(val2);
		}
		return val;
	}

	public void SetActiveMission(int index)
	{
		_activeMission = index;
	}

	public int GetActiveMission()
	{
		return _activeMission;
	}

	public bool HasActiveMission()
	{
		return GetActiveMission() != -1;
	}

	private void LoadMissions(Missions loadedMissions)
	{
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		if (missions.Count > 0)
		{
			for (int num = missions.Count - 1; num >= 0; num--)
			{
				BaseMission.MissionInstance missionInstance = missions[num];
				if (missionInstance != null)
				{
					Pool.Free<BaseMission.MissionInstance>(ref missionInstance);
				}
			}
		}
		missions.Clear();
		if (base.isServer && loadedMissions != null)
		{
			int protocol = loadedMissions.protocol;
			uint seed = loadedMissions.seed;
			int saveCreatedTime = loadedMissions.saveCreatedTime;
			int num2 = Epoch.FromDateTime(SaveRestore.SaveCreatedTime);
			if (220 != protocol || World.Seed != seed || num2 != saveCreatedTime)
			{
				Debug.Log((object)"Missions were from old protocol or different seed, or not from a loaded save clearing");
				loadedMissions.activeMission = -1;
				SetActiveMission(-1);
				State.missions = SaveMissions();
				return;
			}
		}
		if (loadedMissions != null && loadedMissions.missions.Count > 0)
		{
			foreach (MissionInstance mission in loadedMissions.missions)
			{
				BaseMission.MissionInstance missionInstance2 = Pool.Get<BaseMission.MissionInstance>();
				missionInstance2.providerID = mission.providerID;
				missionInstance2.missionID = mission.missionID;
				missionInstance2.status = (BaseMission.MissionStatus)mission.missionStatus;
				missionInstance2.completionScale = mission.completionScale;
				missionInstance2.startTime = Time.get_realtimeSinceStartup() - mission.startTime;
				missionInstance2.endTime = mission.endTime;
				missionInstance2.missionLocation = mission.missionLocation;
				if (mission.missionPoints != null)
				{
					foreach (MissionPoint missionPoint in mission.missionPoints)
					{
						missionInstance2.missionPoints.Add(missionPoint.identifier, missionPoint.location);
					}
				}
				missionInstance2.objectiveStatuses = new BaseMission.MissionInstance.ObjectiveStatus[mission.objectiveStatuses.Count];
				for (int i = 0; i < mission.objectiveStatuses.Count; i++)
				{
					ObjectiveStatus val = mission.objectiveStatuses[i];
					BaseMission.MissionInstance.ObjectiveStatus objectiveStatus = new BaseMission.MissionInstance.ObjectiveStatus();
					objectiveStatus.completed = val.completed;
					objectiveStatus.failed = val.failed;
					objectiveStatus.started = val.started;
					objectiveStatus.genericInt1 = val.genericInt1;
					objectiveStatus.genericFloat1 = val.genericFloat1;
					missionInstance2.objectiveStatuses[i] = objectiveStatus;
				}
				if (mission.createdEntities != null)
				{
					if (missionInstance2.createdEntities == null)
					{
						missionInstance2.createdEntities = Pool.GetList<MissionEntity>();
					}
					foreach (uint createdEntity in mission.createdEntities)
					{
						BaseNetworkable baseNetworkable = null;
						if (base.isServer)
						{
							baseNetworkable = BaseNetworkable.serverEntities.Find(createdEntity);
						}
						if ((Object)(object)baseNetworkable != (Object)null)
						{
							MissionEntity component = ((Component)baseNetworkable).GetComponent<MissionEntity>();
							if (Object.op_Implicit((Object)(object)component))
							{
								missionInstance2.createdEntities.Add(component);
							}
						}
					}
				}
				if (mission.rewards != null && mission.rewards.Count > 0)
				{
					missionInstance2.rewards = new ItemAmount[mission.rewards.Count];
					for (int j = 0; j < mission.rewards.Count; j++)
					{
						MissionReward val2 = mission.rewards[j];
						ItemAmount itemAmount = new ItemAmount();
						ItemDefinition itemDefinition = ItemManager.FindItemDefinition(val2.itemID);
						if ((Object)(object)itemDefinition == (Object)null)
						{
							Debug.LogError((object)"MISSION LOAD UNABLE TO FIND REWARD ITEM, HUGE ERROR!");
						}
						itemAmount.itemDef = itemDefinition;
						itemAmount.amount = val2.itemAmount;
						missionInstance2.rewards[j] = itemAmount;
					}
				}
				missions.Add(missionInstance2);
			}
			SetActiveMission(loadedMissions.activeMission);
		}
		else
		{
			SetActiveMission(-1);
		}
	}

	private void UpdateModelState()
	{
		if (!IsDead() && !IsSpectating())
		{
			wantsSendModelState = true;
		}
	}

	public void SendModelState(bool force = false)
	{
		if (force || (wantsSendModelState && !(nextModelStateUpdate > Time.get_time())))
		{
			wantsSendModelState = false;
			nextModelStateUpdate = Time.get_time() + 0.1f;
			if (!IsDead() && !IsSpectating())
			{
				modelState.set_sleeping(IsSleeping());
				modelState.set_mounted(isMounted);
				modelState.set_relaxed(IsRelaxed());
				modelState.set_onPhone(HasActiveTelephone && !activeTelephone.IsMobile);
				modelState.set_crawling(IsCrawling());
				ClientRPC<ModelState>(null, "OnModelState", modelState);
			}
		}
	}

	public BaseMountable GetMounted()
	{
		return mounted.Get(base.isServer) as BaseMountable;
	}

	public BaseVehicle GetMountedVehicle()
	{
		BaseMountable baseMountable = GetMounted();
		if (!baseMountable.IsValid())
		{
			return null;
		}
		return baseMountable.VehicleParent();
	}

	public void MarkSwapSeat()
	{
		nextSeatSwapTime = Time.get_time() + 0.75f;
	}

	public bool SwapSeatCooldown()
	{
		return Time.get_time() < nextSeatSwapTime;
	}

	public bool CanMountMountablesNow()
	{
		if (!IsDead())
		{
			return !IsWounded();
		}
		return false;
	}

	public void MountObject(BaseMountable mount, int desiredSeat = 0)
	{
		mounted.Set(mount);
		SendNetworkUpdate();
	}

	public void EnsureDismounted()
	{
		if (isMounted)
		{
			GetMounted().DismountPlayer(this);
		}
	}

	public virtual void DismountObject()
	{
		mounted.Set(null);
		SendNetworkUpdate();
		PauseSpeedHackDetection(5f);
		PauseVehicleNoClipDetection(5f);
	}

	public void HandleMountedOnLoad()
	{
		if (!mounted.IsValid(base.isServer))
		{
			return;
		}
		BaseMountable baseMountable = mounted.Get(base.isServer) as BaseMountable;
		if ((Object)(object)baseMountable != (Object)null)
		{
			baseMountable.MountPlayer(this);
			if (!baseMountable.allowSleeperMounting)
			{
				baseMountable.DismountPlayer(this);
			}
		}
		else
		{
			mounted.Set(null);
		}
	}

	public void ClearClientPetLink()
	{
		ClientRPCPlayer(null, this, "CLIENT_SetPetPrefabID", 0, 0);
	}

	public void SendClientPetLink()
	{
		if ((Object)(object)PetEntity == (Object)null && BasePet.ActivePetByOwnerID.TryGetValue(userID, out var value) && (Object)(object)value.Brain != (Object)null)
		{
			value.Brain.SetOwningPlayer(this);
		}
		ClientRPCPlayer(null, this, "CLIENT_SetPetPrefabID", ((Object)(object)PetEntity != (Object)null) ? PetEntity.prefabID : 0u, ((Object)(object)PetEntity != (Object)null) ? PetEntity.net.ID : 0u);
		if ((Object)(object)PetEntity != (Object)null)
		{
			SendClientPetStateIndex();
		}
	}

	public void SendClientPetStateIndex()
	{
		BasePet basePet = PetEntity as BasePet;
		if (!((Object)(object)basePet == (Object)null))
		{
			ClientRPCPlayer(null, this, "CLIENT_SetPetPetLoadedStateIndex", basePet.Brain.LoadedDesignIndex());
		}
	}

	[RPC_Server]
	private void IssuePetCommand(RPCMessage msg)
	{
		ParsePetCommand(msg, raycast: false);
	}

	[RPC_Server]
	private void IssuePetCommandRaycast(RPCMessage msg)
	{
		ParsePetCommand(msg, raycast: true);
	}

	private void ParsePetCommand(RPCMessage msg, bool raycast)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (Time.get_time() - lastPetCommandIssuedTime <= 1f)
		{
			return;
		}
		lastPetCommandIssuedTime = Time.get_time();
		if (!((Object)(object)msg.player == (Object)null) && Pet != null && Pet.IsOwnedBy(msg.player))
		{
			int cmd = msg.read.Int32();
			int param = msg.read.Int32();
			if (raycast)
			{
				Ray value = msg.read.Ray();
				Pet.IssuePetCommand((PetCommandType)cmd, param, value);
			}
			else
			{
				Pet.IssuePetCommand((PetCommandType)cmd, param, null);
			}
		}
	}

	public void DirtyPlayerState()
	{
		_playerStateDirty = true;
	}

	public void SavePlayerState()
	{
		if (_playerStateDirty)
		{
			_playerStateDirty = false;
			SingletonComponent<ServerMgr>.Instance.playerStateManager.Save(userID);
		}
	}

	public void ResetPlayerState()
	{
		SingletonComponent<ServerMgr>.Instance.playerStateManager.Reset(userID);
		ClientRPCPlayer(null, this, "SetHostileLength", 0f);
		SendMarkersToClient();
		WipeMissions();
		MissionDirty();
	}

	public bool IsSleeping()
	{
		return HasPlayerFlag(PlayerFlags.Sleeping);
	}

	public bool IsSpectating()
	{
		return HasPlayerFlag(PlayerFlags.Spectating);
	}

	public bool IsRelaxed()
	{
		return HasPlayerFlag(PlayerFlags.Relaxed);
	}

	public bool IsServerFalling()
	{
		return HasPlayerFlag(PlayerFlags.ServerFall);
	}

	public bool CanBuild()
	{
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return true;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool CanBuild(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(new OBB(position, rotation, bounds));
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return true;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool CanBuild(OBB obb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(obb);
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return true;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingBlocked()
	{
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		return !buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingBlocked(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(new OBB(position, rotation, bounds));
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		return !buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingBlocked(OBB obb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(obb);
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		return !buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingAuthed()
	{
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingAuthed(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(new OBB(position, rotation, bounds));
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingAuthed(OBB obb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(obb);
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool CanPlaceBuildingPrivilege()
	{
		return (Object)(object)GetBuildingPrivilege() == (Object)null;
	}

	public bool CanPlaceBuildingPrivilege(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return (Object)(object)GetBuildingPrivilege(new OBB(position, rotation, bounds)) == (Object)null;
	}

	public bool CanPlaceBuildingPrivilege(OBB obb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return (Object)(object)GetBuildingPrivilege(obb) == (Object)null;
	}

	public bool IsNearEnemyBase()
	{
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		if (!buildingPrivilege.IsAuthed(this))
		{
			return buildingPrivilege.AnyAuthed();
		}
		return false;
	}

	public bool IsNearEnemyBase(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(new OBB(position, rotation, bounds));
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		if (!buildingPrivilege.IsAuthed(this))
		{
			return buildingPrivilege.AnyAuthed();
		}
		return false;
	}

	public bool IsNearEnemyBase(OBB obb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(obb);
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		if (!buildingPrivilege.IsAuthed(this))
		{
			return buildingPrivilege.AnyAuthed();
		}
		return false;
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void OnProjectileAttack(RPCMessage msg)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eaf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eda: Unknown result type (might be due to invalid IL or missing references)
		//IL_1058: Unknown result type (might be due to invalid IL or missing references)
		//IL_105d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1065: Unknown result type (might be due to invalid IL or missing references)
		//IL_106a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1138: Unknown result type (might be due to invalid IL or missing references)
		//IL_1143: Unknown result type (might be due to invalid IL or missing references)
		//IL_114d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1152: Unknown result type (might be due to invalid IL or missing references)
		//IL_1157: Unknown result type (might be due to invalid IL or missing references)
		PlayerProjectileAttack val = PlayerProjectileAttack.Deserialize((Stream)(object)msg.read);
		if (val == null)
		{
			return;
		}
		PlayerAttack playerAttack = val.playerAttack;
		HitInfo hitInfo = new HitInfo();
		hitInfo.LoadFromAttack(playerAttack.attack, serverSide: true);
		hitInfo.Initiator = this;
		hitInfo.ProjectileID = playerAttack.projectileID;
		hitInfo.ProjectileDistance = val.hitDistance;
		hitInfo.ProjectileVelocity = val.hitVelocity;
		hitInfo.Predicted = msg.connection;
		if (hitInfo.IsNaNOrInfinity() || float.IsNaN(val.travelTime) || float.IsInfinity(val.travelTime))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + playerAttack.projectileID + ")");
			val.ResetToPool();
			val = null;
			stats.combat.Log(hitInfo, "projectile_nan");
			return;
		}
		if (!firedProjectiles.TryGetValue(playerAttack.projectileID, out var value))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Missing ID (" + playerAttack.projectileID + ")");
			val.ResetToPool();
			val = null;
			stats.combat.Log(hitInfo, "projectile_invalid");
			return;
		}
		hitInfo.ProjectileHits = value.hits;
		hitInfo.ProjectileIntegrity = value.integrity;
		hitInfo.ProjectileTravelTime = value.travelTime;
		hitInfo.ProjectileTrajectoryMismatch = value.trajectoryMismatch;
		if (value.integrity <= 0f)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Integrity is zero (" + playerAttack.projectileID + ")");
			val.ResetToPool();
			val = null;
			stats.combat.Log(hitInfo, "projectile_integrity");
			return;
		}
		if (value.firedTime < Time.get_realtimeSinceStartup() - 8f)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Lifetime is zero (" + playerAttack.projectileID + ")");
			val.ResetToPool();
			val = null;
			stats.combat.Log(hitInfo, "projectile_lifetime");
			return;
		}
		if (value.ricochets > 0)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile is ricochet (" + playerAttack.projectileID + ")");
			val.ResetToPool();
			val = null;
			stats.combat.Log(hitInfo, "projectile_ricochet");
			return;
		}
		hitInfo.Weapon = value.weaponSource;
		hitInfo.WeaponPrefab = value.weaponPrefab;
		hitInfo.ProjectilePrefab = value.projectilePrefab;
		hitInfo.damageProperties = value.projectilePrefab.damageProperties;
		Vector3 position = value.position;
		Vector3 velocity = value.velocity;
		float partialTime = value.partialTime;
		float travelTime = value.travelTime;
		float num = Mathf.Clamp(val.travelTime, 0f, 8f);
		Vector3 gravity = Physics.get_gravity() * value.projectilePrefab.gravityModifier;
		float drag = value.projectilePrefab.drag;
		int layerMask = (ConVar.AntiHack.projectile_terraincheck ? 10551296 : 2162688);
		BaseEntity hitEntity = hitInfo.HitEntity;
		BasePlayer basePlayer = hitEntity as BasePlayer;
		bool flag = (Object)(object)basePlayer != (Object)null;
		bool flag2 = flag && basePlayer.IsSleeping();
		bool flag3 = flag && basePlayer.IsWounded();
		bool flag4 = flag && basePlayer.isMounted;
		bool flag5 = flag && basePlayer.HasParent();
		bool flag6 = (Object)(object)hitEntity != (Object)null;
		bool flag7 = flag6 && hitEntity.IsNpc;
		bool flag8 = hitInfo.HitMaterial == Projectile.WaterMaterialID();
		if (value.protection > 0)
		{
			bool flag9 = true;
			float num2 = 1f + ConVar.AntiHack.projectile_forgiveness;
			float projectile_clientframes = ConVar.AntiHack.projectile_clientframes;
			float projectile_serverframes = ConVar.AntiHack.projectile_serverframes;
			float num3 = Mathx.Decrement(value.firedTime);
			float num4 = Mathf.Clamp(Mathx.Increment(Time.get_realtimeSinceStartup()) - num3, 0f, 8f);
			float num5 = num;
			float num6 = Mathf.Abs(num4 - num5);
			float num7 = Mathf.Min(num4, num5);
			float num8 = projectile_clientframes / 60f;
			float num9 = projectile_serverframes * Mathx.Max(Time.get_deltaTime(), Time.get_smoothDeltaTime(), Time.get_fixedDeltaTime());
			float num10 = (desyncTimeClamped + num7 + num8 + num9) * num2;
			float num11 = ((value.protection >= 6) ? ((desyncTimeClamped + num8 + num9) * num2) : num10);
			if (flag && hitInfo.boneArea == (HitArea)(-1))
			{
				string name = ((Object)hitInfo.ProjectilePrefab).get_name();
				string text = (flag6 ? hitEntity.ShortPrefabName : "world");
				AntiHack.Log(this, AntiHackType.ProjectileHack, "Bone is invalid (" + name + " on " + text + " bone " + hitInfo.HitBone + ")");
				stats.combat.Log(hitInfo, "projectile_bone");
				flag9 = false;
			}
			if (flag8)
			{
				if (flag6)
				{
					string name2 = ((Object)hitInfo.ProjectilePrefab).get_name();
					string text2 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile water hit on entity (" + name2 + " on " + text2 + ")");
					stats.combat.Log(hitInfo, "water_entity");
					flag9 = false;
				}
				if (!WaterLevel.Test(hitInfo.HitPositionWorld - 0.5f * Vector3.get_up(), waves: false, this))
				{
					string name3 = ((Object)hitInfo.ProjectilePrefab).get_name();
					string text3 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile water level (" + name3 + " on " + text3 + ")");
					stats.combat.Log(hitInfo, "water_level");
					flag9 = false;
				}
			}
			if (value.protection >= 2)
			{
				Vector3 parentVelocity;
				if (flag6)
				{
					float num12 = hitEntity.MaxVelocity();
					parentVelocity = hitEntity.GetParentVelocity();
					float num13 = num12 + ((Vector3)(ref parentVelocity)).get_magnitude();
					float num14 = hitEntity.BoundsPadding() + num11 * num13;
					float num15 = hitEntity.Distance(hitInfo.HitPositionWorld);
					if (num15 > num14)
					{
						string name4 = ((Object)hitInfo.ProjectilePrefab).get_name();
						string shortPrefabName = hitEntity.ShortPrefabName;
						AntiHack.Log(this, AntiHackType.ProjectileHack, "Entity too far away (" + name4 + " on " + shortPrefabName + " with " + num15 + "m > " + num14 + "m in " + num11 + "s)");
						stats.combat.Log(hitInfo, "entity_distance");
						flag9 = false;
					}
				}
				if (value.protection >= 6 && flag9 && flag && !flag7 && !flag2 && !flag3 && !flag4 && !flag5)
				{
					parentVelocity = basePlayer.GetParentVelocity();
					float magnitude = ((Vector3)(ref parentVelocity)).get_magnitude();
					float num16 = basePlayer.BoundsPadding() + num11 * magnitude + ConVar.AntiHack.tickhistoryforgiveness;
					float num17 = basePlayer.tickHistory.Distance(basePlayer, hitInfo.HitPositionWorld);
					if (num17 > num16)
					{
						string name5 = ((Object)hitInfo.ProjectilePrefab).get_name();
						string shortPrefabName2 = basePlayer.ShortPrefabName;
						AntiHack.Log(this, AntiHackType.ProjectileHack, "Player too far away (" + name5 + " on " + shortPrefabName2 + " with " + num17 + "m > " + num16 + "m in " + num11 + "s)");
						stats.combat.Log(hitInfo, "player_distance");
						flag9 = false;
					}
				}
			}
			if (value.protection >= 1)
			{
				float magnitude2 = ((Vector3)(ref value.initialVelocity)).get_magnitude();
				float num18 = hitInfo.ProjectilePrefab.initialDistance + num10 * magnitude2;
				float num19 = hitInfo.ProjectileDistance + 1f;
				float num20 = Vector3.Distance(value.initialPosition, hitInfo.HitPositionWorld);
				if (num20 > num18)
				{
					string name6 = ((Object)hitInfo.ProjectilePrefab).get_name();
					string text4 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile too fast (" + name6 + " on " + text4 + " with " + num20 + "m > " + num18 + "m in " + num10 + "s)");
					stats.combat.Log(hitInfo, "projectile_speed");
					flag9 = false;
				}
				if (num20 > num19)
				{
					string name7 = ((Object)hitInfo.ProjectilePrefab).get_name();
					string text5 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile too far away (" + name7 + " on " + text5 + " with " + num20 + "m > " + num19 + "m in " + num10 + "s)");
					stats.combat.Log(hitInfo, "projectile_distance");
					flag9 = false;
				}
				if (num6 > ConVar.AntiHack.projectile_desync)
				{
					string name8 = ((Object)hitInfo.ProjectilePrefab).get_name();
					string text6 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile desync (" + name8 + " on " + text6 + " with " + num6 + "s > " + ConVar.AntiHack.projectile_desync + "s)");
					stats.combat.Log(hitInfo, "projectile_desync");
					flag9 = false;
				}
			}
			if (value.protection >= 3)
			{
				Vector3 position2 = value.position;
				Vector3 pointStart = hitInfo.PointStart;
				Vector3 val2 = hitInfo.HitPositionWorld;
				Vector3 val3 = hitInfo.PositionOnRay(val2);
				if (!flag8)
				{
					val2 += ((Vector3)(ref hitInfo.HitNormalWorld)).get_normalized() * 0.001f;
				}
				bool num21 = GamePhysics.LineOfSight(position2, pointStart, val3, val2, layerMask);
				if (!num21)
				{
					stats.Add("hit_" + (flag6 ? hitEntity.Categorize() : "world") + "_indirect_los", 1, Stats.Server);
				}
				else
				{
					stats.Add("hit_" + (flag6 ? hitEntity.Categorize() : "world") + "_direct_los", 1, Stats.Server);
				}
				if (!num21)
				{
					string name9 = ((Object)hitInfo.ProjectilePrefab).get_name();
					string text7 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Line of sight (", name9, " on ", text7, ") ", position2, " ", pointStart, " ", val3, " ", val2));
					stats.combat.Log(hitInfo, "projectile_los");
					flag9 = false;
				}
				if (flag9 && flag && !flag7)
				{
					Vector3 val4 = hitInfo.HitPositionWorld;
					Vector3 position3 = basePlayer.eyes.position;
					Vector3 val5 = basePlayer.CenterPoint();
					if (!flag8)
					{
						val4 += ((Vector3)(ref hitInfo.HitNormalWorld)).get_normalized() * 0.001f;
					}
					if ((!GamePhysics.LineOfSight(val4, position3, layerMask, 0f, ConVar.AntiHack.projectile_losforgiveness) || !GamePhysics.LineOfSight(position3, val4, layerMask, ConVar.AntiHack.projectile_losforgiveness, 0f)) && (!GamePhysics.LineOfSight(val4, val5, layerMask, 0f, ConVar.AntiHack.projectile_losforgiveness) || !GamePhysics.LineOfSight(val5, val4, layerMask, ConVar.AntiHack.projectile_losforgiveness, 0f)))
					{
						string name10 = ((Object)hitInfo.ProjectilePrefab).get_name();
						string text8 = (flag6 ? hitEntity.ShortPrefabName : "world");
						AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Line of sight (", name10, " on ", text8, ") ", val4, " ", position3, " or ", val4, " ", val5));
						stats.combat.Log(hitInfo, "projectile_los");
						flag9 = false;
					}
				}
			}
			if (value.protection >= 4)
			{
				SimulateProjectile(ref position, ref velocity, ref partialTime, num - travelTime, gravity, drag, out var prevPosition, out var prevVelocity);
				Vector3 val6 = prevVelocity * 0.03125f;
				Line val7 = default(Line);
				((Line)(ref val7))._002Ector(prevPosition - val6, position + val6);
				float num22 = ((Line)(ref val7)).Distance(hitInfo.PointStart);
				float num23 = ((Line)(ref val7)).Distance(hitInfo.HitPositionWorld);
				if (num22 > ConVar.AntiHack.projectile_trajectory)
				{
					string name11 = ((Object)value.projectilePrefab).get_name();
					string text9 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Start position trajectory (" + name11 + " on " + text9 + " with " + num22 + "m > " + ConVar.AntiHack.projectile_trajectory + "m)");
					stats.combat.Log(hitInfo, "trajectory_start");
					flag9 = false;
				}
				if (num23 > ConVar.AntiHack.projectile_trajectory)
				{
					string name12 = ((Object)value.projectilePrefab).get_name();
					string text10 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "End position trajectory (" + name12 + " on " + text10 + " with " + num23 + "m > " + ConVar.AntiHack.projectile_trajectory + "m)");
					stats.combat.Log(hitInfo, "trajectory_end");
					flag9 = false;
				}
				hitInfo.ProjectileVelocity = velocity;
				if (val.hitVelocity != Vector3.get_zero() && velocity != Vector3.get_zero())
				{
					float num24 = Vector3.Angle(val.hitVelocity, velocity);
					float num25 = ((Vector3)(ref val.hitVelocity)).get_magnitude() / ((Vector3)(ref velocity)).get_magnitude();
					if (num24 > ConVar.AntiHack.projectile_anglechange)
					{
						string name13 = ((Object)value.projectilePrefab).get_name();
						string text11 = (flag6 ? hitEntity.ShortPrefabName : "world");
						AntiHack.Log(this, AntiHackType.ProjectileHack, "Trajectory angle change (" + name13 + " on " + text11 + " with " + num24 + "deg > " + ConVar.AntiHack.projectile_anglechange + "deg)");
						stats.combat.Log(hitInfo, "angle_change");
						flag9 = false;
					}
					if (num25 > ConVar.AntiHack.projectile_velocitychange)
					{
						string name14 = ((Object)value.projectilePrefab).get_name();
						string text12 = (flag6 ? hitEntity.ShortPrefabName : "world");
						AntiHack.Log(this, AntiHackType.ProjectileHack, "Trajectory velocity change (" + name14 + " on " + text12 + " with " + num25 + " > " + ConVar.AntiHack.projectile_velocitychange + ")");
						stats.combat.Log(hitInfo, "velocity_change");
						flag9 = false;
					}
				}
			}
			if (!flag9)
			{
				AntiHack.AddViolation(this, AntiHackType.ProjectileHack, ConVar.AntiHack.projectile_penalty);
				val.ResetToPool();
				val = null;
				return;
			}
		}
		value.position = hitInfo.HitPositionWorld;
		value.velocity = val.hitVelocity;
		value.travelTime = num;
		value.partialTime = partialTime;
		value.hits++;
		hitInfo.ProjectilePrefab.CalculateDamage(hitInfo, value.projectileModifier, value.integrity);
		if (value.integrity < 1f)
		{
			value.integrity = 0f;
		}
		else if (flag8)
		{
			value.integrity = Mathf.Clamp01(value.integrity - 0.1f);
		}
		else if (hitInfo.ProjectilePrefab.penetrationPower <= 0f || !flag6)
		{
			value.integrity = 0f;
		}
		else
		{
			float num26 = hitEntity.PenetrationResistance(hitInfo) / hitInfo.ProjectilePrefab.penetrationPower;
			value.integrity = Mathf.Clamp01(value.integrity - num26);
			ref Vector3 position4 = ref value.position;
			position4 += ((Vector3)(ref val.hitVelocity)).get_normalized() * 0.001f;
		}
		if (flag6)
		{
			stats.Add(value.itemMod.category + "_hit_" + hitEntity.Categorize(), 1);
		}
		if (value.integrity <= 0f)
		{
			if (value.hits <= 1)
			{
				value.itemMod.ServerProjectileHit(hitInfo);
			}
			if (hitInfo.ProjectilePrefab.remainInWorld)
			{
				CreateWorldProjectile(hitInfo, value.itemDef, value.itemMod, hitInfo.ProjectilePrefab, value.pickupItem);
			}
		}
		firedProjectiles[playerAttack.projectileID] = value;
		if (flag6)
		{
			if (value.hits <= 2)
			{
				hitEntity.OnAttacked(hitInfo);
			}
			else
			{
				stats.combat.Log(hitInfo, "ricochet");
			}
		}
		hitInfo.DoHitEffects = hitInfo.ProjectilePrefab.doDefaultHitEffects;
		Effect.server.ImpactEffect(hitInfo);
		val.ResetToPool();
		val = null;
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void OnProjectileRicochet(RPCMessage msg)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		PlayerProjectileRicochet val = PlayerProjectileRicochet.Deserialize((Stream)(object)msg.read);
		if (val != null)
		{
			FiredProjectile value;
			if (Vector3Ex.IsNaNOrInfinity(val.hitPosition) || Vector3Ex.IsNaNOrInfinity(val.inVelocity) || Vector3Ex.IsNaNOrInfinity(val.outVelocity) || Vector3Ex.IsNaNOrInfinity(val.hitNormal) || float.IsNaN(val.travelTime) || float.IsInfinity(val.travelTime))
			{
				AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + val.projectileID + ")");
				val.ResetToPool();
				val = null;
			}
			else if (!firedProjectiles.TryGetValue(val.projectileID, out value))
			{
				AntiHack.Log(this, AntiHackType.ProjectileHack, "Missing ID (" + val.projectileID + ")");
				val.ResetToPool();
				val = null;
			}
			else if (value.firedTime < Time.get_realtimeSinceStartup() - 8f)
			{
				AntiHack.Log(this, AntiHackType.ProjectileHack, "Lifetime is zero (" + val.projectileID + ")");
				val.ResetToPool();
				val = null;
			}
			else
			{
				value.ricochets++;
				firedProjectiles[val.projectileID] = value;
				val.ResetToPool();
				val = null;
			}
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void OnProjectileUpdate(RPCMessage msg)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		PlayerProjectileUpdate val = PlayerProjectileUpdate.Deserialize((Stream)(object)msg.read);
		if (val == null)
		{
			return;
		}
		if (Vector3Ex.IsNaNOrInfinity(val.curPosition) || Vector3Ex.IsNaNOrInfinity(val.curVelocity) || float.IsNaN(val.travelTime) || float.IsInfinity(val.travelTime))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + val.projectileID + ")");
			val.ResetToPool();
			val = null;
			return;
		}
		if (!firedProjectiles.TryGetValue(val.projectileID, out var value))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Missing ID (" + val.projectileID + ")");
			val.ResetToPool();
			val = null;
			return;
		}
		if (value.firedTime < Time.get_realtimeSinceStartup() - 8f)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Lifetime is zero (" + val.projectileID + ")");
			val.ResetToPool();
			val = null;
			return;
		}
		if (value.ricochets > 0)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile is ricochet (" + val.projectileID + ")");
			val.ResetToPool();
			val = null;
			return;
		}
		Vector3 position = value.position;
		Vector3 velocity = value.velocity;
		float num = value.trajectoryMismatch;
		float partialTime = value.partialTime;
		float travelTime = Mathf.Clamp(val.travelTime - value.travelTime, 0f, 8f);
		Vector3 gravity = Physics.get_gravity() * value.projectilePrefab.gravityModifier;
		float drag = value.projectilePrefab.drag;
		int layerMask = (ConVar.AntiHack.projectile_terraincheck ? 10551296 : 2162688);
		if (value.protection >= 3)
		{
			Vector3 position2 = value.position;
			Vector3 curPosition = val.curPosition;
			if (!GamePhysics.LineOfSight(position2, curPosition, layerMask))
			{
				string name = ((Object)value.projectilePrefab).get_name();
				AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Line of sight (", name, " on update) ", position2, " ", curPosition));
				val.ResetToPool();
				val = null;
				return;
			}
			if (ConVar.AntiHack.projectile_backtracking > 0f)
			{
				Vector3 val2 = curPosition - position2;
				Vector3 val3 = ((Vector3)(ref val2)).get_normalized() * ConVar.AntiHack.projectile_backtracking;
				if (!GamePhysics.LineOfSight(position2, curPosition + val3, layerMask))
				{
					string name2 = ((Object)value.projectilePrefab).get_name();
					AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Line of sight (", name2, " backtracking on update) ", position2, " ", curPosition));
					val.ResetToPool();
					val = null;
					return;
				}
			}
		}
		if (value.protection >= 4)
		{
			SimulateProjectile(ref position, ref velocity, ref partialTime, travelTime, gravity, drag, out var prevPosition, out var prevVelocity);
			Vector3 val4 = prevVelocity * 0.03125f;
			Line val5 = default(Line);
			((Line)(ref val5))._002Ector(prevPosition - val4, position + val4);
			num += ((Line)(ref val5)).Distance(val.curPosition);
			if (num > ConVar.AntiHack.projectile_trajectory)
			{
				string name3 = ((Object)value.projectilePrefab).get_name();
				AntiHack.Log(this, AntiHackType.ProjectileHack, "Update position trajectory (" + name3 + " on update with " + num + "m > " + ConVar.AntiHack.projectile_trajectory + "m)");
				val.ResetToPool();
				val = null;
				return;
			}
		}
		if (value.protection >= 5)
		{
			if (value.inheritedVelocity != Vector3.get_zero())
			{
				Vector3 curVelocity = value.inheritedVelocity + velocity;
				Vector3 curVelocity2 = val.curVelocity;
				if (((Vector3)(ref curVelocity2)).get_magnitude() > 2f * ((Vector3)(ref curVelocity)).get_magnitude())
				{
					val.curVelocity = curVelocity;
				}
				value.inheritedVelocity = Vector3.get_zero();
			}
			else
			{
				val.curVelocity = velocity;
			}
		}
		value.position = val.curPosition;
		value.velocity = val.curVelocity;
		value.travelTime = val.travelTime;
		value.partialTime = partialTime;
		value.trajectoryMismatch = num;
		firedProjectiles[val.projectileID] = value;
		val.ResetToPool();
		val = null;
	}

	private void SimulateProjectile(ref Vector3 position, ref Vector3 velocity, ref float partialTime, float travelTime, Vector3 gravity, float drag, out Vector3 prevPosition, out Vector3 prevVelocity)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.03125f;
		prevPosition = position;
		prevVelocity = velocity;
		if (partialTime > Mathf.Epsilon)
		{
			float num2 = num - partialTime;
			if (travelTime < num2)
			{
				prevPosition = position;
				prevVelocity = velocity;
				position += velocity * travelTime;
				partialTime += travelTime;
				return;
			}
			prevPosition = position;
			prevVelocity = velocity;
			position += velocity * num2;
			velocity += gravity * num;
			velocity -= velocity * drag * num;
			travelTime -= num2;
		}
		int num3 = Mathf.FloorToInt(travelTime / num);
		for (int i = 0; i < num3; i++)
		{
			prevPosition = position;
			prevVelocity = velocity;
			position += velocity * num;
			velocity += gravity * num;
			velocity -= velocity * drag * num;
		}
		partialTime = travelTime - num * (float)num3;
		if (partialTime > Mathf.Epsilon)
		{
			prevPosition = position;
			prevVelocity = velocity;
			position += velocity * partialTime;
		}
	}

	protected virtual void CreateWorldProjectile(HitInfo info, ItemDefinition itemDef, ItemModProjectile itemMod, Projectile projectilePrefab, Item recycleItem)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		Vector3 projectileVelocity = info.ProjectileVelocity;
		Item item = ((recycleItem != null) ? recycleItem : ItemManager.Create(itemDef, 1, 0uL));
		BaseEntity baseEntity = null;
		if (!info.DidHit)
		{
			baseEntity = item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3)(ref projectileVelocity)).get_normalized()));
			baseEntity.Kill(DestroyMode.Gib);
			return;
		}
		if (projectilePrefab.breakProbability > 0f && Random.get_value() <= projectilePrefab.breakProbability)
		{
			baseEntity = item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3)(ref projectileVelocity)).get_normalized()));
			baseEntity.Kill(DestroyMode.Gib);
			return;
		}
		if (projectilePrefab.conditionLoss > 0f)
		{
			item.LoseCondition(projectilePrefab.conditionLoss * 100f);
			if (item.isBroken)
			{
				baseEntity = item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3)(ref projectileVelocity)).get_normalized()));
				baseEntity.Kill(DestroyMode.Gib);
				return;
			}
		}
		if (projectilePrefab.stickProbability > 0f && Random.get_value() <= projectilePrefab.stickProbability)
		{
			baseEntity = (((Object)(object)info.HitEntity == (Object)null) ? item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3)(ref projectileVelocity)).get_normalized())) : ((info.HitBone != 0) ? item.CreateWorldObject(info.HitPositionLocal, Quaternion.LookRotation(info.HitNormalLocal * -1f), info.HitEntity, info.HitBone) : item.CreateWorldObject(info.HitPositionLocal, Quaternion.LookRotation(((Component)info.HitEntity).get_transform().InverseTransformDirection(((Vector3)(ref projectileVelocity)).get_normalized())), info.HitEntity)));
			((Component)baseEntity).GetComponent<Rigidbody>().set_isKinematic(true);
			return;
		}
		baseEntity = item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3)(ref projectileVelocity)).get_normalized()));
		Rigidbody component = ((Component)baseEntity).GetComponent<Rigidbody>();
		component.AddForce(((Vector3)(ref projectileVelocity)).get_normalized() * 200f);
		component.WakeUp();
	}

	public void CleanupExpiredProjectiles()
	{
		foreach (KeyValuePair<int, FiredProjectile> item in Enumerable.ToList<KeyValuePair<int, FiredProjectile>>(Enumerable.Where<KeyValuePair<int, FiredProjectile>>((IEnumerable<KeyValuePair<int, FiredProjectile>>)firedProjectiles, (Func<KeyValuePair<int, FiredProjectile>, bool>)((KeyValuePair<int, FiredProjectile> x) => x.Value.firedTime < Time.get_realtimeSinceStartup() - 8f - 1f))))
		{
			firedProjectiles.Remove(item.Key);
		}
	}

	public bool HasFiredProjectile(int id)
	{
		return firedProjectiles.ContainsKey(id);
	}

	public void NoteFiredProjectile(int projectileid, Vector3 startPos, Vector3 startVel, AttackEntity attackEnt, ItemDefinition firedItemDef, Item pickupItem = null)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		BaseProjectile baseProjectile = attackEnt as BaseProjectile;
		ItemModProjectile component = ((Component)firedItemDef).GetComponent<ItemModProjectile>();
		Projectile component2 = component.projectileObject.Get().GetComponent<Projectile>();
		int projectile_protection = ConVar.AntiHack.projectile_protection;
		Vector3 inheritedVelocity = (((Object)(object)attackEnt != (Object)null) ? attackEnt.GetInheritedVelocity(this) : Vector3.get_zero());
		if (Vector3Ex.IsNaNOrInfinity(startPos) || Vector3Ex.IsNaNOrInfinity(startVel))
		{
			string name = ((Object)component2).get_name();
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + name + ")");
			stats.combat.Log(baseProjectile, "projectile_nan");
			return;
		}
		if (projectile_protection >= 1)
		{
			float num = 1f + ConVar.AntiHack.projectile_forgiveness;
			float magnitude = ((Vector3)(ref startVel)).get_magnitude();
			float num2 = component.GetMaxVelocity();
			BaseProjectile baseProjectile2 = attackEnt as BaseProjectile;
			if (Object.op_Implicit((Object)(object)baseProjectile2))
			{
				num2 *= baseProjectile2.GetProjectileVelocityScale(getMax: true);
			}
			num2 *= num;
			if (magnitude > num2)
			{
				string name2 = ((Object)component2).get_name();
				AntiHack.Log(this, AntiHackType.ProjectileHack, "Velocity (" + name2 + " with " + magnitude + " > " + num2 + ")");
				stats.combat.Log(baseProjectile, "projectile_velocity");
				return;
			}
		}
		FiredProjectile firedProjectile = default(FiredProjectile);
		firedProjectile.itemDef = firedItemDef;
		firedProjectile.itemMod = component;
		firedProjectile.projectilePrefab = component2;
		firedProjectile.firedTime = Time.get_realtimeSinceStartup();
		firedProjectile.travelTime = 0f;
		firedProjectile.weaponSource = attackEnt;
		firedProjectile.weaponPrefab = (((Object)(object)attackEnt == (Object)null) ? null : GameManager.server.FindPrefab(StringPool.Get(attackEnt.prefabID)).GetComponent<AttackEntity>());
		firedProjectile.projectileModifier = (((Object)(object)baseProjectile == (Object)null) ? Projectile.Modifier.Default : baseProjectile.GetProjectileModifier());
		firedProjectile.pickupItem = pickupItem;
		firedProjectile.integrity = 1f;
		firedProjectile.position = startPos;
		firedProjectile.velocity = startVel;
		firedProjectile.initialPosition = startPos;
		firedProjectile.initialVelocity = startVel;
		firedProjectile.inheritedVelocity = inheritedVelocity;
		firedProjectile.protection = projectile_protection;
		firedProjectile.ricochets = 0;
		firedProjectile.hits = 0;
		FiredProjectile value = firedProjectile;
		firedProjectiles.Add(projectileid, value);
	}

	public void ServerNoteFiredProjectile(int projectileid, Vector3 startPos, Vector3 startVel, AttackEntity attackEnt, ItemDefinition firedItemDef, Item pickupItem = null)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		BaseProjectile baseProjectile = attackEnt as BaseProjectile;
		ItemModProjectile component = ((Component)firedItemDef).GetComponent<ItemModProjectile>();
		Projectile component2 = component.projectileObject.Get().GetComponent<Projectile>();
		int protection = 0;
		Vector3 zero = Vector3.get_zero();
		if (!Vector3Ex.IsNaNOrInfinity(startPos) && !Vector3Ex.IsNaNOrInfinity(startVel))
		{
			FiredProjectile firedProjectile = default(FiredProjectile);
			firedProjectile.itemDef = firedItemDef;
			firedProjectile.itemMod = component;
			firedProjectile.projectilePrefab = component2;
			firedProjectile.firedTime = Time.get_realtimeSinceStartup();
			firedProjectile.travelTime = 0f;
			firedProjectile.weaponSource = attackEnt;
			firedProjectile.weaponPrefab = (((Object)(object)attackEnt == (Object)null) ? null : GameManager.server.FindPrefab(StringPool.Get(attackEnt.prefabID)).GetComponent<AttackEntity>());
			firedProjectile.projectileModifier = (((Object)(object)baseProjectile == (Object)null) ? Projectile.Modifier.Default : baseProjectile.GetProjectileModifier());
			firedProjectile.pickupItem = pickupItem;
			firedProjectile.integrity = 1f;
			firedProjectile.trajectoryMismatch = 0f;
			firedProjectile.position = startPos;
			firedProjectile.velocity = startVel;
			firedProjectile.initialPosition = startPos;
			firedProjectile.initialVelocity = startVel;
			firedProjectile.inheritedVelocity = zero;
			firedProjectile.protection = protection;
			firedProjectile.ricochets = 0;
			firedProjectile.hits = 0;
			FiredProjectile value = firedProjectile;
			firedProjectiles.Add(projectileid, value);
		}
	}

	public override bool CanUseNetworkCache(Connection connection)
	{
		if (net == null)
		{
			return true;
		}
		if (net.get_connection() != connection)
		{
			return true;
		}
		return false;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		HandleMountedOnLoad();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		bool flag = net != null && net.get_connection() == info.forConnection;
		info.msg.basePlayer = Pool.Get<BasePlayer>();
		info.msg.basePlayer.userid = userID;
		info.msg.basePlayer.name = displayName;
		info.msg.basePlayer.playerFlags = (int)playerFlags;
		info.msg.basePlayer.currentTeam = currentTeam;
		info.msg.basePlayer.heldEntity = svActiveItemID;
		info.msg.basePlayer.reputation = reputation;
		if (!info.forDisk && (Object)(object)currentGesture != (Object)null && currentGesture.animationType == GestureConfig.AnimationType.Loop)
		{
			info.msg.basePlayer.loopingGesture = currentGesture.gestureId;
		}
		if (IsConnected && (IsAdmin || IsDeveloper))
		{
			info.msg.basePlayer.skinCol = net.get_connection().info.GetFloat("global.skincol", -1f);
			info.msg.basePlayer.skinTex = net.get_connection().info.GetFloat("global.skintex", -1f);
			info.msg.basePlayer.skinMesh = net.get_connection().info.GetFloat("global.skinmesh", -1f);
		}
		else
		{
			info.msg.basePlayer.skinCol = -1f;
			info.msg.basePlayer.skinTex = -1f;
			info.msg.basePlayer.skinMesh = -1f;
		}
		info.msg.basePlayer.underwear = GetUnderwearSkin();
		if (info.forDisk || flag)
		{
			info.msg.basePlayer.metabolism = metabolism.Save();
			info.msg.basePlayer.modifiers = null;
			if ((Object)(object)modifiers != (Object)null)
			{
				info.msg.basePlayer.modifiers = modifiers.Save();
			}
		}
		if (!info.forDisk && !flag)
		{
			BasePlayer basePlayer = info.msg.basePlayer;
			basePlayer.playerFlags &= -5;
			BasePlayer basePlayer2 = info.msg.basePlayer;
			basePlayer2.playerFlags &= -129;
		}
		info.msg.basePlayer.inventory = inventory.Save(info.forDisk || flag);
		modelState.set_sleeping(IsSleeping());
		modelState.set_relaxed(IsRelaxed());
		modelState.set_crawling(IsCrawling());
		info.msg.basePlayer.modelState = modelState.Copy();
		if (info.forDisk)
		{
			BaseEntity baseEntity = mounted.Get(base.isServer);
			if (baseEntity.IsValid())
			{
				if (baseEntity.enableSaving)
				{
					info.msg.basePlayer.mounted = mounted.uid;
				}
				else
				{
					BaseVehicle mountedVehicle = GetMountedVehicle();
					if (mountedVehicle.IsValid() && mountedVehicle.enableSaving)
					{
						info.msg.basePlayer.mounted = mountedVehicle.net.ID;
					}
				}
			}
		}
		else
		{
			info.msg.basePlayer.mounted = mounted.uid;
		}
		if (flag)
		{
			info.msg.basePlayer.persistantData = PersistantPlayerInfo.Copy();
			if (!info.forDisk && State.missions != null)
			{
				info.msg.basePlayer.missions = State.missions.Copy();
			}
		}
		if (info.forDisk)
		{
			info.msg.basePlayer.currentLife = lifeStory;
			info.msg.basePlayer.previousLife = previousLifeStory;
		}
		if (info.forDisk)
		{
			SavePlayerState();
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.basePlayer != null)
		{
			BasePlayer basePlayer = info.msg.basePlayer;
			userID = basePlayer.userid;
			UserIDString = userID.ToString();
			if (basePlayer.name != null)
			{
				displayName = basePlayer.name;
			}
			playerFlags = (PlayerFlags)basePlayer.playerFlags;
			currentTeam = basePlayer.currentTeam;
			reputation = basePlayer.reputation;
			if (basePlayer.metabolism != null)
			{
				metabolism.Load(basePlayer.metabolism);
			}
			if (basePlayer.modifiers != null && (Object)(object)modifiers != (Object)null)
			{
				modifiers.Load(basePlayer.modifiers);
			}
			if (basePlayer.inventory != null)
			{
				inventory.Load(basePlayer.inventory);
			}
			if (basePlayer.modelState != null)
			{
				if (modelState != null)
				{
					modelState.ResetToPool();
					modelState = null;
				}
				modelState = basePlayer.modelState;
				basePlayer.modelState = null;
			}
		}
		if (info.fromDisk)
		{
			lifeStory = info.msg.basePlayer.currentLife;
			if (lifeStory != null)
			{
				lifeStory.ShouldPool = false;
			}
			previousLifeStory = info.msg.basePlayer.previousLife;
			if (previousLifeStory != null)
			{
				previousLifeStory.ShouldPool = false;
			}
			SetPlayerFlag(PlayerFlags.Sleeping, b: false);
			StartSleeping();
			SetPlayerFlag(PlayerFlags.Connected, b: false);
			if (lifeStory == null && IsAlive())
			{
				LifeStoryStart();
			}
			mounted.uid = info.msg.basePlayer.mounted;
			if (IsWounded())
			{
				Die();
			}
		}
	}

	internal void LifeStoryStart()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		if (lifeStory != null)
		{
			Debug.LogError((object)"Stomping old lifeStory");
			lifeStory = null;
		}
		lifeStory = new PlayerLifeStory
		{
			ShouldPool = false
		};
		lifeStory.timeBorn = (uint)Epoch.get_Current();
		hasSentPresenceState = false;
	}

	internal void LifeStoryEnd()
	{
		SingletonComponent<ServerMgr>.Instance.persistance.AddLifeStory(userID, lifeStory);
		previousLifeStory = lifeStory;
		lifeStory = null;
	}

	internal void LifeStoryUpdate(float deltaTime, float moveSpeed)
	{
		if (lifeStory != null)
		{
			PlayerLifeStory obj = lifeStory;
			obj.secondsAlive += deltaTime;
			nextTimeCategoryUpdate -= deltaTime * ((moveSpeed > 0.1f) ? 1f : 0.25f);
			if (nextTimeCategoryUpdate <= 0f && !waitingForLifeStoryUpdate)
			{
				nextTimeCategoryUpdate = 7f + 7f * Random.Range(0.2f, 1f);
				waitingForLifeStoryUpdate = true;
				((ObjectWorkQueue<BasePlayer>)lifeStoryQueue).Add(this);
			}
			if (LifeStoryInWilderness)
			{
				PlayerLifeStory obj2 = lifeStory;
				obj2.secondsWilderness += deltaTime;
			}
			if (LifeStoryInMonument)
			{
				PlayerLifeStory obj3 = lifeStory;
				obj3.secondsInMonument += deltaTime;
			}
			if (LifeStoryInBase)
			{
				PlayerLifeStory obj4 = lifeStory;
				obj4.secondsInBase += deltaTime;
			}
			if (LifeStoryFlying)
			{
				PlayerLifeStory obj5 = lifeStory;
				obj5.secondsFlying += deltaTime;
			}
			if (LifeStoryBoating)
			{
				PlayerLifeStory obj6 = lifeStory;
				obj6.secondsBoating += deltaTime;
			}
			if (LifeStorySwimming)
			{
				PlayerLifeStory obj7 = lifeStory;
				obj7.secondsSwimming += deltaTime;
			}
			if (LifeStoryDriving)
			{
				PlayerLifeStory obj8 = lifeStory;
				obj8.secondsDriving += deltaTime;
			}
			if (IsSleeping())
			{
				PlayerLifeStory obj9 = lifeStory;
				obj9.secondsSleeping += deltaTime;
			}
			else if (IsRunning())
			{
				PlayerLifeStory obj10 = lifeStory;
				obj10.metersRun += moveSpeed * deltaTime;
			}
			else
			{
				PlayerLifeStory obj11 = lifeStory;
				obj11.metersWalked += moveSpeed * deltaTime;
			}
		}
	}

	public void UpdateTimeCategory()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("UpdateTimeCategory", 0);
		try
		{
			waitingForLifeStoryUpdate = false;
			int num = currentTimeCategory;
			currentTimeCategory = 1;
			if (IsBuildingAuthed())
			{
				currentTimeCategory = 4;
			}
			Vector3 position = ((Component)this).get_transform().get_position();
			if ((Object)(object)TerrainMeta.TopologyMap != (Object)null && ((uint)TerrainMeta.TopologyMap.GetTopology(position) & 0x400u) != 0)
			{
				foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
				{
					if (monument.shouldDisplayOnMap && monument.IsInBounds(position))
					{
						currentTimeCategory = 2;
						break;
					}
				}
			}
			if (IsSwimming())
			{
				currentTimeCategory |= 32;
			}
			BaseMountable baseMountable2;
			if (isMounted)
			{
				BaseMountable baseMountable = GetMounted();
				if (baseMountable.mountTimeStatType == BaseMountable.MountStatType.Boating)
				{
					currentTimeCategory |= 16;
				}
				else if (baseMountable.mountTimeStatType == BaseMountable.MountStatType.Flying)
				{
					currentTimeCategory |= 8;
				}
				else if (baseMountable.mountTimeStatType == BaseMountable.MountStatType.Driving)
				{
					currentTimeCategory |= 64;
				}
			}
			else if (HasParent() && (baseMountable2 = GetParentEntity() as BaseMountable) != null)
			{
				if (baseMountable2.mountTimeStatType == BaseMountable.MountStatType.Boating)
				{
					currentTimeCategory |= 16;
				}
				else if (baseMountable2.mountTimeStatType == BaseMountable.MountStatType.Flying)
				{
					currentTimeCategory |= 8;
				}
				else if (baseMountable2.mountTimeStatType == BaseMountable.MountStatType.Driving)
				{
					currentTimeCategory |= 64;
				}
			}
			if (num != currentTimeCategory || !hasSentPresenceState)
			{
				LifeStoryInWilderness = (1 & currentTimeCategory) != 0;
				LifeStoryInMonument = (2 & currentTimeCategory) != 0;
				LifeStoryInBase = (4 & currentTimeCategory) != 0;
				LifeStoryFlying = (8 & currentTimeCategory) != 0;
				LifeStoryBoating = (0x10 & currentTimeCategory) != 0;
				LifeStorySwimming = (0x20 & currentTimeCategory) != 0;
				LifeStoryDriving = (0x40 & currentTimeCategory) != 0;
				ClientRPCPlayer(null, this, "UpdateRichPresenceState", currentTimeCategory);
				hasSentPresenceState = true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void LifeStoryShotFired(BaseEntity withWeapon)
	{
		if (lifeStory == null)
		{
			return;
		}
		if (lifeStory.weaponStats == null)
		{
			lifeStory.weaponStats = Pool.GetList<WeaponStats>();
		}
		foreach (WeaponStats weaponStat in lifeStory.weaponStats)
		{
			if (weaponStat.weaponName == withWeapon.ShortPrefabName)
			{
				weaponStat.shotsFired++;
				return;
			}
		}
		WeaponStats val = Pool.Get<WeaponStats>();
		val.weaponName = withWeapon.ShortPrefabName;
		val.shotsFired++;
		lifeStory.weaponStats.Add(val);
	}

	public void LifeStoryShotHit(BaseEntity withWeapon)
	{
		if (lifeStory == null || (Object)(object)withWeapon == (Object)null)
		{
			return;
		}
		if (lifeStory.weaponStats == null)
		{
			lifeStory.weaponStats = Pool.GetList<WeaponStats>();
		}
		foreach (WeaponStats weaponStat in lifeStory.weaponStats)
		{
			if (weaponStat.weaponName == withWeapon.ShortPrefabName)
			{
				weaponStat.shotsHit++;
				return;
			}
		}
		WeaponStats val = Pool.Get<WeaponStats>();
		val.weaponName = withWeapon.ShortPrefabName;
		val.shotsHit++;
		lifeStory.weaponStats.Add(val);
	}

	public void LifeStoryKill(BaseCombatEntity killed)
	{
		if (lifeStory != null)
		{
			if (killed is ScientistNPC)
			{
				PlayerLifeStory obj = lifeStory;
				obj.killedScientists++;
			}
			else if (killed is BasePlayer)
			{
				PlayerLifeStory obj2 = lifeStory;
				obj2.killedPlayers++;
			}
			else if (killed is BaseAnimalNPC)
			{
				PlayerLifeStory obj3 = lifeStory;
				obj3.killedAnimals++;
			}
		}
	}

	public void LifeStoryGenericStat(string key, int value)
	{
		if (lifeStory == null)
		{
			return;
		}
		if (lifeStory.genericStats == null)
		{
			lifeStory.genericStats = Pool.GetList<GenericStat>();
		}
		foreach (GenericStat genericStat in lifeStory.genericStats)
		{
			if (genericStat.key == key)
			{
				genericStat.value += value;
				return;
			}
		}
		GenericStat val = Pool.Get<GenericStat>();
		val.key = key;
		val.value = value;
		lifeStory.genericStats.Add(val);
	}

	public void LifeStoryHurt(float amount)
	{
		if (lifeStory != null)
		{
			PlayerLifeStory obj = lifeStory;
			obj.totalDamageTaken += amount;
		}
	}

	public void LifeStoryHeal(float amount)
	{
		if (lifeStory != null)
		{
			PlayerLifeStory obj = lifeStory;
			obj.totalHealing += amount;
		}
	}

	internal void LifeStoryLogDeath(HitInfo deathBlow, DamageType lastDamage)
	{
		if (lifeStory == null)
		{
			return;
		}
		lifeStory.timeDied = (uint)Epoch.get_Current();
		DeathInfo val = Pool.Get<DeathInfo>();
		val.lastDamageType = (int)lastDamage;
		if (deathBlow != null)
		{
			if ((Object)(object)deathBlow.Initiator != (Object)null)
			{
				deathBlow.Initiator.AttackerInfo(val);
				val.attackerDistance = Distance(deathBlow.Initiator);
			}
			if ((Object)(object)deathBlow.WeaponPrefab != (Object)null)
			{
				val.inflictorName = deathBlow.WeaponPrefab.ShortPrefabName;
			}
			if (deathBlow.HitBone != 0)
			{
				val.hitBone = StringPool.Get(deathBlow.HitBone);
			}
			else
			{
				val.hitBone = "";
			}
		}
		else if (base.SecondsSinceAttacked <= 60f && (Object)(object)lastAttacker != (Object)null)
		{
			lastAttacker.AttackerInfo(val);
		}
		lifeStory.deathInfo = val;
	}

	internal override void OnParentRemoved()
	{
		if (IsNpc)
		{
			base.OnParentRemoved();
		}
		else
		{
			SetParent(null, worldPositionStays: true, sendImmediate: true);
		}
	}

	public override void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)oldParent != (Object)null)
		{
			TransformState(((Component)oldParent).get_transform().get_localToWorldMatrix());
		}
		if ((Object)(object)newParent != (Object)null)
		{
			TransformState(((Component)newParent).get_transform().get_worldToLocalMatrix());
		}
	}

	private void TransformState(Matrix4x4 matrix)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		tickInterpolator.TransformEntries(matrix);
		tickHistory.TransformEntries(matrix);
		Quaternion rotation = ((Matrix4x4)(ref matrix)).get_rotation();
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(0f, ((Quaternion)(ref rotation)).get_eulerAngles().y, 0f);
		eyes.bodyRotation = Quaternion.Euler(val) * eyes.bodyRotation;
	}

	public bool CanSuicide()
	{
		if (IsAdmin || IsDeveloper)
		{
			return true;
		}
		return Time.get_realtimeSinceStartup() > nextSuicideTime;
	}

	public void MarkSuicide()
	{
		nextSuicideTime = Time.get_realtimeSinceStartup() + 60f;
	}

	public bool CanRespawn()
	{
		return Time.get_realtimeSinceStartup() > nextRespawnTime;
	}

	public void MarkRespawn()
	{
		nextRespawnTime = Time.get_realtimeSinceStartup() + 5f;
	}

	public Item GetActiveItem()
	{
		if (svActiveItemID == 0)
		{
			return null;
		}
		if (IsDead())
		{
			return null;
		}
		if ((Object)(object)inventory == (Object)null || inventory.containerBelt == null)
		{
			return null;
		}
		return inventory.containerBelt.FindItemByUID(svActiveItemID);
	}

	public void MovePosition(Vector3 newPos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).get_transform().set_position(newPos);
		tickInterpolator.Reset(newPos);
		ticksPerSecond.Increment();
		tickHistory.AddPoint(newPos, tickHistoryCapacity);
		NetworkPositionTick();
	}

	public void OverrideViewAngles(Vector3 newAng)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		viewAngles = newAng;
	}

	public override void ServerInit()
	{
		stats = new PlayerStatistics(this);
		if (userID == 0L)
		{
			userID = (ulong)Random.Range(0, 10000000);
			UserIDString = userID.ToString();
			displayName = UserIDString;
			bots.Add(this);
		}
		EnablePlayerCollider();
		SetPlayerRigidbodyState(!IsSleeping());
		base.ServerInit();
		Query.Server.AddPlayer(this);
		inventory.ServerInit(this);
		metabolism.ServerInit(this);
		if ((Object)(object)modifiers != (Object)null)
		{
			modifiers.ServerInit(this);
		}
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		Query.Server.RemovePlayer(this);
		if (Object.op_Implicit((Object)(object)inventory))
		{
			inventory.DoDestroy();
		}
		sleepingPlayerList.Remove(this);
		SavePlayerState();
		if (cachedPersistantPlayer != null)
		{
			Pool.Free<PersistantPlayer>(ref cachedPersistantPlayer);
		}
	}

	protected void ServerUpdate(float deltaTime)
	{
		if (!Net.sv.IsConnected())
		{
			return;
		}
		LifeStoryUpdate(deltaTime, IsOnGround() ? estimatedSpeed : 0f);
		FinalizeTick(deltaTime);
		ThinkMissions(deltaTime);
		desyncTimeRaw = Mathf.Max(timeSinceLastTick - deltaTime, 0f);
		desyncTimeClamped = Mathf.Min(desyncTimeRaw, ConVar.AntiHack.maxdesync);
		if (clientTickRate != Player.tickrate_cl)
		{
			clientTickRate = Player.tickrate_cl;
			clientTickInterval = 1f / (float)clientTickRate;
			ClientRPCPlayer(null, this, "UpdateClientTickRate", clientTickRate);
		}
		if (serverTickRate != Player.tickrate_sv)
		{
			serverTickRate = Player.tickrate_sv;
			serverTickInterval = 1f / (float)serverTickRate;
		}
		if (ConVar.AntiHack.terrain_protection > 0 && Time.get_frameCount() % ConVar.AntiHack.terrain_timeslice == (long)net.ID % (long)ConVar.AntiHack.terrain_timeslice && !AntiHack.ShouldIgnore(this) && AntiHack.IsInsideTerrain(this))
		{
			AntiHack.AddViolation(this, AntiHackType.InsideTerrain, ConVar.AntiHack.terrain_penalty);
			if (ConVar.AntiHack.terrain_kill)
			{
				Hurt(1000f, DamageType.Suicide, this, useProtection: false);
				return;
			}
		}
		if (!(Time.get_realtimeSinceStartup() < lastPlayerTick + serverTickInterval))
		{
			if (lastPlayerTick < Time.get_realtimeSinceStartup() - serverTickInterval * 100f)
			{
				lastPlayerTick = Time.get_realtimeSinceStartup() - Random.Range(0f, serverTickInterval);
			}
			while (lastPlayerTick < Time.get_realtimeSinceStartup())
			{
				lastPlayerTick += serverTickInterval;
			}
			if (IsConnected)
			{
				ConnectedPlayerUpdate(serverTickInterval);
			}
		}
	}

	private void ServerUpdateBots(float deltaTime)
	{
		RefreshColliderSize(forced: false);
	}

	private void ConnectedPlayerUpdate(float deltaTime)
	{
		if (IsReceivingSnapshot)
		{
			net.UpdateSubscriptions(int.MaxValue, int.MaxValue);
		}
		else if (Time.get_realtimeSinceStartup() > lastSubscriptionTick + ConVar.Server.entitybatchtime && net.UpdateSubscriptions(ConVar.Server.entitybatchsize * 2, ConVar.Server.entitybatchsize))
		{
			lastSubscriptionTick = Time.get_realtimeSinceStartup();
		}
		SendEntityUpdate();
		if (IsReceivingSnapshot)
		{
			if (SnapshotQueue.Length == 0 && EACServer.IsAuthenticated(net.get_connection()))
			{
				EnterGame();
			}
			return;
		}
		if (IsAlive())
		{
			metabolism.ServerUpdate(this, deltaTime);
			if ((Object)(object)modifiers != (Object)null && !IsReceivingSnapshot)
			{
				modifiers.ServerUpdate(this);
			}
			if (InSafeZone())
			{
				float num = 0f;
				HeldEntity heldEntity = GetHeldEntity();
				if (Object.op_Implicit((Object)(object)heldEntity) && heldEntity.hostile)
				{
					num = deltaTime;
				}
				if (num == 0f)
				{
					MarkWeaponDrawnDuration(0f);
				}
				else
				{
					AddWeaponDrawnDuration(num);
				}
				if (weaponDrawnDuration >= 5f)
				{
					MarkHostileFor(30f);
				}
			}
			else
			{
				MarkWeaponDrawnDuration(0f);
			}
			if (timeSinceLastTick > (float)ConVar.Server.playertimeout)
			{
				lastTickTime = 0f;
				Kick("Unresponsive");
				return;
			}
		}
		int num2 = (int)net.get_connection().GetSecondsConnected();
		int num3 = num2 - secondsConnected;
		if (num3 > 0)
		{
			stats.Add("time", num3, Stats.Server);
			secondsConnected = num2;
		}
		RefreshColliderSize(forced: false);
		SendModelState();
	}

	private void EnterGame()
	{
		SetPlayerFlag(PlayerFlags.ReceivingSnapshot, b: false);
		ClientRPCPlayer(null, this, "FinishLoading");
		((FacepunchBehaviour)this).Invoke((Action)DelayedTeamUpdate, 1f);
		LoadMissions(State.missions);
		MissionDirty();
		double num = State.unHostileTimestamp - TimeEx.get_currentTimestamp();
		if (num > 0.0)
		{
			ClientRPCPlayer(null, this, "SetHostileLength", (float)num);
		}
		if ((Object)(object)modifiers != (Object)null)
		{
			modifiers.ResetTicking();
		}
		if (net != null)
		{
			EACServer.OnFinishLoading(net.get_connection());
		}
		Debug.Log((object)$"{this} has spawned");
		if ((Demo.recordlistmode == 0) ? Demo.recordlist.Contains(UserIDString) : (!Demo.recordlist.Contains(UserIDString)))
		{
			StartDemoRecording();
		}
		SendClientPetLink();
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	private void ClientKeepConnectionAlive(RPCMessage msg)
	{
		lastTickTime = Time.get_time();
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	private void ClientLoadingComplete(RPCMessage msg)
	{
	}

	public void PlayerInit(Connection c)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("PlayerInit", 10);
		try
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)base.KillMessage);
			SetPlayerFlag(PlayerFlags.Connected, b: true);
			activePlayerList.Add(this);
			bots.Remove(this);
			userID = c.userid;
			UserIDString = userID.ToString();
			displayName = c.username;
			c.player = (MonoBehaviour)(object)this;
			currentTeam = RelationshipManager.ServerInstance.FindPlayersTeam(userID)?.teamID ?? 0;
			SingletonComponent<ServerMgr>.Instance.persistance.SetPlayerName(userID, displayName);
			tickInterpolator.Reset(((Component)this).get_transform().get_position());
			tickHistory.Reset(((Component)this).get_transform().get_position());
			eyeHistory.Clear();
			lastTickTime = 0f;
			lastInputTime = 0f;
			SetPlayerFlag(PlayerFlags.ReceivingSnapshot, b: true);
			stats.Init();
			((FacepunchBehaviour)this).InvokeRandomized((Action)StatSave, Random.Range(5f, 10f), 30f, Random.Range(0f, 6f));
			previousLifeStory = SingletonComponent<ServerMgr>.Instance.persistance.GetLastLifeStory(userID);
			SetPlayerFlag(PlayerFlags.IsAdmin, c.authLevel != 0);
			SetPlayerFlag(PlayerFlags.IsDeveloper, DeveloperList.IsDeveloper(this));
			if (IsDead() && net.SwitchGroup(BaseNetworkable.LimboNetworkGroup))
			{
				SendNetworkGroupChange();
			}
			net.OnConnected(c);
			net.StartSubscriber();
			SendAsSnapshot(net.get_connection());
			ClientRPCPlayer(null, this, "StartLoading");
			if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(serverside: true)))
			{
				BaseGameMode.GetActiveGameMode(serverside: true).OnPlayerConnected(this);
			}
			if (net != null)
			{
				EACServer.OnStartLoading(net.get_connection());
			}
			if (IsAdmin)
			{
				if (ConVar.AntiHack.noclip_protection <= 0)
				{
					ChatMessage("antihack.noclip_protection is disabled!");
				}
				if (ConVar.AntiHack.speedhack_protection <= 0)
				{
					ChatMessage("antihack.speedhack_protection is disabled!");
				}
				if (ConVar.AntiHack.flyhack_protection <= 0)
				{
					ChatMessage("antihack.flyhack_protection is disabled!");
				}
				if (ConVar.AntiHack.projectile_protection <= 0)
				{
					ChatMessage("antihack.projectile_protection is disabled!");
				}
				if (ConVar.AntiHack.melee_protection <= 0)
				{
					ChatMessage("antihack.melee_protection is disabled!");
				}
				if (ConVar.AntiHack.eye_protection <= 0)
				{
					ChatMessage("antihack.eye_protection is disabled!");
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void StatSave()
	{
		if (stats != null)
		{
			stats.Save();
		}
	}

	public void SendDeathInformation()
	{
		ClientRPCPlayer(null, this, "OnDied");
	}

	public void SendRespawnOptions()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		RespawnInformation val = Pool.Get<RespawnInformation>();
		try
		{
			val.spawnOptions = Pool.Get<List<SpawnOptions>>();
			SleepingBag[] array = SleepingBag.FindForPlayer(userID, ignoreTimers: true);
			foreach (SleepingBag sleepingBag in array)
			{
				SpawnOptions val2 = Pool.Get<SpawnOptions>();
				val2.id = sleepingBag.net.ID;
				val2.name = sleepingBag.niceName;
				val2.worldPosition = ((Component)sleepingBag).get_transform().get_position();
				val2.type = sleepingBag.RespawnType;
				val2.unlockSeconds = sleepingBag.GetUnlockSeconds(userID);
				val2.occupied = sleepingBag.IsOccupied();
				val.spawnOptions.Add(val2);
			}
			val.previousLife = previousLifeStory;
			val.fadeIn = previousLifeStory != null && previousLifeStory.timeDied > Epoch.get_Current() - 5;
			ClientRPCPlayer<RespawnInformation>(null, this, "OnRespawnInformation", val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(1uL)]
	private void RequestRespawnInformation(RPCMessage msg)
	{
		SendRespawnOptions();
	}

	public void ScheduledDeath()
	{
		Kill();
	}

	public virtual void StartSleeping()
	{
		if (!IsSleeping())
		{
			if (InSafeZone() && !((FacepunchBehaviour)this).IsInvoking((Action)ScheduledDeath))
			{
				((FacepunchBehaviour)this).Invoke((Action)ScheduledDeath, NPCAutoTurret.sleeperhostiledelay);
			}
			BaseMountable baseMountable = GetMounted();
			if ((Object)(object)baseMountable != (Object)null && !baseMountable.allowSleeperMounting)
			{
				EnsureDismounted();
			}
			SetPlayerFlag(PlayerFlags.Sleeping, b: true);
			sleepStartTime = Time.get_time();
			sleepingPlayerList.Add(this);
			bots.Remove(this);
			((FacepunchBehaviour)this).CancelInvoke((Action)InventoryUpdate);
			((FacepunchBehaviour)this).CancelInvoke((Action)TeamUpdate);
			inventory.loot.Clear();
			inventory.crafting.CancelAll(returnItems: true);
			inventory.containerMain.OnChanged();
			inventory.containerBelt.OnChanged();
			inventory.containerWear.OnChanged();
			TurnOffAllLights();
			EnablePlayerCollider();
			RemovePlayerRigidbody();
			SetServerFall(wantsOn: true);
		}
	}

	private void TurnOffAllLights()
	{
		LightToggle(mask: false);
		HeldEntity heldEntity = GetHeldEntity();
		if ((Object)(object)heldEntity != (Object)null)
		{
			TorchWeapon component = ((Component)heldEntity).GetComponent<TorchWeapon>();
			if ((Object)(object)component != (Object)null)
			{
				component.SetIsOn(isOn: false);
			}
		}
	}

	private void OnPhysicsNeighbourChanged()
	{
		if (IsSleeping() || IsIncapacitated())
		{
			((FacepunchBehaviour)this).Invoke((Action)DelayedServerFall, 0.05f);
		}
	}

	private void DelayedServerFall()
	{
		SetServerFall(wantsOn: true);
	}

	public void SetServerFall(bool wantsOn)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (wantsOn && ConVar.Server.playerserverfall)
		{
			if (!((FacepunchBehaviour)this).IsInvoking((Action)ServerFall))
			{
				SetPlayerFlag(PlayerFlags.ServerFall, b: true);
				lastFallTime = Time.get_time() - fallTickRate;
				((FacepunchBehaviour)this).InvokeRandomized((Action)ServerFall, 0f, fallTickRate, fallTickRate * 0.1f);
				fallVelocity = estimatedVelocity.y;
			}
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)ServerFall);
			SetPlayerFlag(PlayerFlags.ServerFall, b: false);
		}
	}

	public void ServerFall()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		if (IsDead() || HasParent() || (!IsIncapacitated() && !IsSleeping()))
		{
			SetServerFall(wantsOn: false);
			return;
		}
		float num = Time.get_time() - lastFallTime;
		lastFallTime = Time.get_time();
		float radius = GetRadius();
		float num2 = GetHeight(ducked: true) * 0.5f;
		float num3 = 2.5f;
		float num4 = 0.5f;
		fallVelocity += Physics.get_gravity().y * num3 * num4 * num;
		float num5 = Mathf.Abs(fallVelocity * num);
		Vector3 val = ((Component)this).get_transform().get_position() + Vector3.get_up() * (radius + num2);
		Vector3 position = ((Component)this).get_transform().get_position();
		Vector3 val2 = ((Component)this).get_transform().get_position();
		RaycastHit val3 = default(RaycastHit);
		if (Physics.SphereCast(val, radius, Vector3.get_down(), ref val3, num5 + num2, 1537286401, (QueryTriggerInteraction)1))
		{
			SetServerFall(wantsOn: false);
			if (((RaycastHit)(ref val3)).get_distance() > num2)
			{
				val2 += Vector3.get_down() * (((RaycastHit)(ref val3)).get_distance() - num2);
			}
			ApplyFallDamageFromVelocity(fallVelocity);
			UpdateEstimatedVelocity(val2, val2, num);
			fallVelocity = 0f;
		}
		else if (Physics.Raycast(val, Vector3.get_down(), ref val3, num5 + radius + num2, 1537286401, (QueryTriggerInteraction)1))
		{
			SetServerFall(wantsOn: false);
			if (((RaycastHit)(ref val3)).get_distance() > num2 - radius)
			{
				val2 += Vector3.get_down() * (((RaycastHit)(ref val3)).get_distance() - num2 - radius);
			}
			ApplyFallDamageFromVelocity(fallVelocity);
			UpdateEstimatedVelocity(val2, val2, num);
			fallVelocity = 0f;
		}
		else
		{
			val2 += Vector3.get_down() * num5;
			UpdateEstimatedVelocity(position, val2, num);
			if (WaterLevel.Test(val2, waves: true, this) || AntiHack.TestInsideTerrain(val2))
			{
				SetServerFall(wantsOn: false);
			}
		}
		MovePosition(val2);
	}

	public void DelayedRigidbodyDisable()
	{
		RemovePlayerRigidbody();
	}

	public virtual void EndSleeping()
	{
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		if (!IsSleeping())
		{
			return;
		}
		SetPlayerFlag(PlayerFlags.Sleeping, b: false);
		sleepStartTime = -1f;
		sleepingPlayerList.Remove(this);
		if (userID < 10000000 && !bots.Contains(this))
		{
			bots.Add(this);
		}
		((FacepunchBehaviour)this).CancelInvoke((Action)ScheduledDeath);
		((FacepunchBehaviour)this).InvokeRepeating((Action)InventoryUpdate, 1f, 0.1f * Random.Range(0.99f, 1.01f));
		if (RelationshipManager.TeamsEnabled())
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)TeamUpdate, 1f, 4f, 1f);
		}
		EnablePlayerCollider();
		AddPlayerRigidbody();
		SetServerFall(wantsOn: false);
		if (HasParent())
		{
			SetParent(null, worldPositionStays: true);
			ForceUpdateTriggers();
		}
		inventory.containerMain.OnChanged();
		inventory.containerBelt.OnChanged();
		inventory.containerWear.OnChanged();
		if (EACServer.playerTracker != null && net.get_connection() != null)
		{
			TimeWarning val = TimeWarning.New("playerTracker.LogPlayerSpawn", 0);
			try
			{
				Client client = EACServer.GetClient(net.get_connection());
				EACServer.playerTracker.LogPlayerSpawn(client, 0, 0);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public virtual void EndLooting()
	{
		if (Object.op_Implicit((Object)(object)inventory.loot))
		{
			inventory.loot.Clear();
		}
	}

	public virtual void OnDisconnected()
	{
		stats.Save();
		EndLooting();
		ClearDesigningAIEntity();
		if (IsAlive() || IsSleeping())
		{
			StartSleeping();
		}
		else
		{
			((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0f);
		}
		activePlayerList.Remove(this);
		SetPlayerFlag(PlayerFlags.Connected, b: false);
		StopDemoRecording();
		if (net != null)
		{
			net.OnDisconnected();
		}
		ResetAntiHack();
		RefreshColliderSize(forced: true);
		clientTickRate = 20;
		clientTickInterval = 0.05f;
		if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(serverside: true)))
		{
			BaseGameMode.GetActiveGameMode(serverside: true).OnPlayerDisconnected(this);
		}
		BaseMission.PlayerDisconnected(this);
	}

	private void InventoryUpdate()
	{
		if (IsConnected && !IsDead())
		{
			inventory.ServerUpdate(0.1f);
		}
	}

	public void ApplyFallDamageFromVelocity(float velocity)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.InverseLerp(-15f, -100f, velocity);
		if (num != 0f)
		{
			metabolism.bleeding.Add(num * 0.5f);
			float num2 = num * 500f;
			Hurt(num2, DamageType.Fall);
			if (num2 > 20f && fallDamageEffect.isValid)
			{
				Effect.server.Run(fallDamageEffect.resourcePath, ((Component)this).get_transform().get_position(), Vector3.get_zero());
			}
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	private void OnPlayerLanded(RPCMessage msg)
	{
		float num = msg.read.Float();
		if (!float.IsNaN(num) && !float.IsInfinity(num))
		{
			ApplyFallDamageFromVelocity(num);
			fallVelocity = 0f;
		}
	}

	public void SendGlobalSnapshot()
	{
		TimeWarning val = TimeWarning.New("SendGlobalSnapshot", 10);
		try
		{
			EnterVisibility(Net.sv.visibility.Get(0u));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void SendFullSnapshot()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("SendFullSnapshot", 0);
		try
		{
			Enumerator<Group> enumerator = net.subscriber.subscribed.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Group current = enumerator.get_Current();
					if (current.ID != 0)
					{
						EnterVisibility(current);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void OnNetworkGroupLeave(Group group)
	{
		base.OnNetworkGroupLeave(group);
		LeaveVisibility(group);
	}

	private void LeaveVisibility(Group group)
	{
		ServerMgr.OnLeaveVisibility(net.get_connection(), group);
		ClearEntityQueue(group);
	}

	public override void OnNetworkGroupEnter(Group group)
	{
		base.OnNetworkGroupEnter(group);
		EnterVisibility(group);
	}

	private void EnterVisibility(Group group)
	{
		ServerMgr.OnEnterVisibility(net.get_connection(), group);
		SendSnapshots(group.networkables);
	}

	public void CheckDeathCondition(HitInfo info = null)
	{
		Assert.IsTrue(base.isServer, "CheckDeathCondition called on client!");
		if (!IsSpectating() && !IsDead() && metabolism.ShouldDie())
		{
			Die(info);
		}
	}

	public virtual BaseCorpse CreateCorpse()
	{
		TimeWarning val = TimeWarning.New("Create corpse", 0);
		try
		{
			PlayerCorpse playerCorpse = DropCorpse("assets/prefabs/player/player_corpse.prefab") as PlayerCorpse;
			if (Object.op_Implicit((Object)(object)playerCorpse))
			{
				playerCorpse.SetFlag(Flags.Reserved5, HasPlayerFlag(PlayerFlags.DisplaySash));
				playerCorpse.TakeFrom(inventory.containerMain, inventory.containerWear, inventory.containerBelt);
				playerCorpse.playerName = displayName;
				playerCorpse.playerSteamID = userID;
				playerCorpse.underwearSkin = GetUnderwearSkin();
				playerCorpse.Spawn();
				playerCorpse.TakeChildren(this);
				ResourceDispenser component = ((Component)playerCorpse).GetComponent<ResourceDispenser>();
				int num = 2;
				if (lifeStory != null)
				{
					num += Mathf.Clamp(Mathf.FloorToInt(lifeStory.secondsAlive / 180f), 0, 20);
				}
				component.containedItems.Add(new ItemAmount(ItemManager.FindItemDefinition("fat.animal"), num));
				return playerCorpse;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return null;
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		SetPlayerFlag(PlayerFlags.Unused2, b: false);
		SetPlayerFlag(PlayerFlags.Unused1, b: false);
		EnsureDismounted();
		EndSleeping();
		EndLooting();
		stats.Add("deaths", 1, Stats.All);
		if (info != null && (Object)(object)info.InitiatorPlayer != (Object)null && !info.InitiatorPlayer.IsNpc && !IsNpc)
		{
			RelationshipManager.ServerInstance.SetSeen(info.InitiatorPlayer, this);
			RelationshipManager.ServerInstance.SetSeen(this, info.InitiatorPlayer);
			RelationshipManager.ServerInstance.SetRelationship(this, info.InitiatorPlayer, RelationshipManager.RelationshipType.Enemy);
		}
		if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(serverside: true)))
		{
			BasePlayer instigator = info?.InitiatorPlayer;
			BaseGameMode.GetActiveGameMode(serverside: true).OnPlayerDeath(instigator, this, info);
		}
		BaseMission.PlayerKilled(this);
		DisablePlayerCollider();
		RemovePlayerRigidbody();
		StopWounded();
		inventory.crafting.CancelAll(returnItems: true);
		TimeWarning val;
		if (EACServer.playerTracker != null && net.get_connection() != null)
		{
			BasePlayer basePlayer = ((info != null && (Object)(object)info.Initiator != (Object)null) ? info.Initiator.ToPlayer() : null);
			if ((Object)(object)basePlayer != (Object)null && basePlayer.net.get_connection() != null)
			{
				val = TimeWarning.New("playerTracker.LogPlayerKill", 0);
				try
				{
					Client client = EACServer.GetClient(basePlayer.net.get_connection());
					Client client2 = EACServer.GetClient(net.get_connection());
					EACServer.playerTracker.LogPlayerKill(client2, client);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			else
			{
				val = TimeWarning.New("playerTracker.LogPlayerDespawn", 0);
				try
				{
					Client client3 = EACServer.GetClient(net.get_connection());
					EACServer.playerTracker.LogPlayerDespawn(client3);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
		}
		BaseCorpse baseCorpse = CreateCorpse();
		if ((Object)(object)baseCorpse != (Object)null && info != null)
		{
			Rigidbody component = ((Component)baseCorpse).GetComponent<Rigidbody>();
			if ((Object)(object)component != (Object)null)
			{
				Vector3 val2 = info.attackNormal + Vector3.get_up() * 0.5f;
				component.AddForce(((Vector3)(ref val2)).get_normalized() * 1f, (ForceMode)2);
			}
		}
		inventory.Strip();
		if (lastDamage == DamageType.Fall)
		{
			stats.Add("death_fall", 1);
		}
		string text = "";
		string text2 = "";
		if (info != null)
		{
			if (Object.op_Implicit((Object)(object)info.Initiator))
			{
				if ((Object)(object)info.Initiator == (Object)(object)this)
				{
					text = ((object)this).ToString() + " was suicide by " + lastDamage;
					text2 = "You died: suicide by " + lastDamage;
					if (lastDamage == DamageType.Suicide)
					{
						Analytics.Death("suicide");
						stats.Add("death_suicide", 1, Stats.All);
					}
					else
					{
						Analytics.Death("selfinflicted");
						stats.Add("death_selfinflicted", 1);
					}
				}
				else if (info.Initiator is BasePlayer)
				{
					BasePlayer basePlayer2 = info.Initiator.ToPlayer();
					text = ((object)this).ToString() + " was killed by " + ((object)basePlayer2).ToString();
					text2 = "You died: killed by " + basePlayer2.displayName + " (" + basePlayer2.userID + ")";
					basePlayer2.stats.Add("kill_player", 1, Stats.All);
					basePlayer2.LifeStoryKill(this);
					if ((Object)(object)info.WeaponPrefab != (Object)null)
					{
						Analytics.Death(info.WeaponPrefab.ShortPrefabName);
					}
					else
					{
						Analytics.Death("player");
					}
					if (lastDamage == DamageType.Fun_Water)
					{
						basePlayer2.GiveAchievement("SUMMER_LIQUIDATOR");
						LiquidWeapon liquidWeapon = basePlayer2.GetHeldEntity() as LiquidWeapon;
						if ((Object)(object)liquidWeapon != (Object)null && liquidWeapon.RequiresPumping && liquidWeapon.PressureFraction <= liquidWeapon.MinimumPressureFraction)
						{
							basePlayer2.GiveAchievement("SUMMER_NO_PRESSURE");
						}
					}
				}
				else
				{
					text = ((object)this).ToString() + " was killed by " + info.Initiator.ShortPrefabName + " (" + info.Initiator.Categorize() + ")";
					text2 = "You died: killed by " + info.Initiator.Categorize();
					stats.Add("death_" + info.Initiator.Categorize(), 1);
					Analytics.Death(info.Initiator.Categorize());
				}
			}
			else if (lastDamage == DamageType.Fall)
			{
				text = ((object)this).ToString() + " was killed by fall!";
				text2 = "You died: killed by fall!";
				Analytics.Death("fall");
			}
			else
			{
				text = ((object)this).ToString() + " was killed by " + info.damageTypes.GetMajorityDamageType();
				text2 = "You died: " + info.damageTypes.GetMajorityDamageType();
			}
		}
		else
		{
			text = string.Concat(((object)this).ToString(), " died (", lastDamage, ")");
			text2 = "You died: " + lastDamage;
		}
		val = TimeWarning.New("LogMessage", 0);
		try
		{
			DebugEx.Log((object)text, (StackTraceLogType)0);
			ConsoleMessage(text2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (net.get_connection() == null && (Object)(object)info?.Initiator != (Object)null && (Object)(object)info.Initiator != (Object)(object)this)
		{
			CompanionServer.Util.SendDeathNotification(this, info.Initiator);
		}
		SendNetworkUpdateImmediate();
		LifeStoryLogDeath(info, lastDamage);
		Server_LogDeathMarker(((Component)this).get_transform().get_position());
		LifeStoryEnd();
		if (net.get_connection() == null)
		{
			((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0f);
			return;
		}
		SendRespawnOptions();
		SendDeathInformation();
		stats.Save();
	}

	public void RespawnAt(Vector3 position, Quaternion rotation)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if (!Object.op_Implicit((Object)(object)activeGameMode) || activeGameMode.CanPlayerRespawn(this))
		{
			SetPlayerFlag(PlayerFlags.Wounded, b: false);
			SetPlayerFlag(PlayerFlags.Unused2, b: false);
			SetPlayerFlag(PlayerFlags.Unused1, b: false);
			SetPlayerFlag(PlayerFlags.ReceivingSnapshot, b: true);
			SetPlayerFlag(PlayerFlags.DisplaySash, b: false);
			ServerPerformance.spawns++;
			SetParent(null, worldPositionStays: true);
			((Component)this).get_transform().SetPositionAndRotation(position, rotation);
			tickInterpolator.Reset(position);
			tickHistory.Reset(position);
			eyeHistory.Clear();
			lastTickTime = 0f;
			StopWounded();
			ResetWoundingVars();
			StopSpectating();
			UpdateNetworkGroup();
			EnablePlayerCollider();
			RemovePlayerRigidbody();
			StartSleeping();
			LifeStoryStart();
			metabolism.Reset();
			if ((Object)(object)modifiers != (Object)null)
			{
				modifiers.RemoveAll();
			}
			InitializeHealth(StartHealth(), StartMaxHealth());
			inventory.GiveDefaultItems();
			SendNetworkUpdateImmediate();
			ClientRPCPlayer(null, this, "StartLoading");
			if (Object.op_Implicit((Object)(object)activeGameMode))
			{
				BaseGameMode.GetActiveGameMode(serverside: true).OnPlayerRespawn(this);
			}
			if (net != null)
			{
				EACServer.OnStartLoading(net.get_connection());
			}
		}
	}

	public void Respawn()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		SpawnPoint spawnPoint = ServerMgr.FindSpawnPoint(this);
		RespawnAt(spawnPoint.pos, spawnPoint.rot);
	}

	public bool IsImmortalTo(HitInfo info)
	{
		if (IsGod())
		{
			return true;
		}
		if (WoundingCausingImmortality(info))
		{
			return true;
		}
		BaseVehicle mountedVehicle = GetMountedVehicle();
		if ((Object)(object)mountedVehicle != (Object)null && mountedVehicle.ignoreDamageFromOutside)
		{
			BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if ((Object)(object)initiatorPlayer != (Object)null && (Object)(object)initiatorPlayer.GetMountedVehicle() != (Object)(object)mountedVehicle)
			{
				return true;
			}
		}
		return false;
	}

	public float TimeAlive()
	{
		return lifeStory.secondsAlive;
	}

	public override void Hurt(HitInfo info)
	{
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		if (IsDead() || (IsImmortalTo(info) && info.damageTypes.Total() >= 0f))
		{
			return;
		}
		if (ConVar.Server.pve && Object.op_Implicit((Object)(object)info.Initiator) && info.Initiator is BasePlayer && (Object)(object)info.Initiator != (Object)(object)this)
		{
			(info.Initiator as BasePlayer).Hurt(info.damageTypes.Total(), DamageType.Generic);
			return;
		}
		if (info.damageTypes.Has(DamageType.Fun_Water))
		{
			bool flag = true;
			Item activeItem = GetActiveItem();
			if (activeItem != null && (activeItem.info.shortname == "gun.water" || activeItem.info.shortname == "pistol.water"))
			{
				float value = metabolism.wetness.value;
				metabolism.wetness.Add(ConVar.Server.funWaterWetnessGain);
				bool flag2 = metabolism.wetness.value >= ConVar.Server.funWaterDamageThreshold;
				flag = !flag2;
				if ((Object)(object)info.InitiatorPlayer != (Object)null)
				{
					if (flag2 && value < ConVar.Server.funWaterDamageThreshold)
					{
						info.InitiatorPlayer.GiveAchievement("SUMMER_SOAKED");
					}
					if (metabolism.radiation_level.Fraction() > 0.2f && !string.IsNullOrEmpty("SUMMER_RADICAL"))
					{
						info.InitiatorPlayer.GiveAchievement("SUMMER_RADICAL");
					}
				}
			}
			if (flag)
			{
				info.damageTypes.Scale(DamageType.Fun_Water, 0f);
			}
		}
		if (info.damageTypes.Get(DamageType.Drowned) > 5f && drownEffect.isValid)
		{
			Effect.server.Run(drownEffect.resourcePath, this, StringPool.Get("head"), Vector3.get_zero(), Vector3.get_zero());
		}
		if ((Object)(object)modifiers != (Object)null)
		{
			if (info.damageTypes.Has(DamageType.Radiation))
			{
				info.damageTypes.Scale(DamageType.Radiation, 1f - Mathf.Clamp01(modifiers.GetValue(Modifier.ModifierType.Radiation_Resistance)));
			}
			if (info.damageTypes.Has(DamageType.RadiationExposure))
			{
				info.damageTypes.Scale(DamageType.RadiationExposure, 1f - Mathf.Clamp01(modifiers.GetValue(Modifier.ModifierType.Radiation_Exposure_Resistance)));
			}
		}
		metabolism.pending_health.Subtract(info.damageTypes.Total() * 10f);
		BasePlayer initiatorPlayer = info.InitiatorPlayer;
		if (Object.op_Implicit((Object)(object)initiatorPlayer) && (Object)(object)initiatorPlayer != (Object)(object)this)
		{
			if (initiatorPlayer.InSafeZone() || InSafeZone())
			{
				initiatorPlayer.MarkHostileFor(300f);
			}
			if (initiatorPlayer.IsNpc && initiatorPlayer.Family == BaseNpc.AiStatistics.FamilyEnum.Murderer && info.damageTypes.Get(DamageType.Explosion) > 0f)
			{
				info.damageTypes.ScaleAll(Halloween.scarecrow_beancan_vs_player_dmg_modifier);
			}
		}
		base.Hurt(info);
		if (EACServer.playerTracker != null && (Object)(object)info.Initiator != (Object)null && info.Initiator is BasePlayer)
		{
			BasePlayer basePlayer = info.Initiator.ToPlayer();
			if (net.get_connection() != null && basePlayer.net.get_connection() != null)
			{
				Client client = EACServer.GetClient(net.get_connection());
				Client client2 = EACServer.GetClient(basePlayer.net.get_connection());
				PlayerTakeDamage val = default(PlayerTakeDamage);
				val.DamageTaken = (int)info.damageTypes.Total();
				val.HitBoneID = (int)info.HitBone;
				val.WeaponID = 0;
				val.DamageFlags = (PlayerTakeDamageFlags)(info.isHeadshot ? 1 : 0);
				if ((Object)(object)info.Weapon != (Object)null)
				{
					Item item = info.Weapon.GetItem();
					if (item != null)
					{
						val.WeaponID = item.info.itemid;
					}
				}
				Vector3 position = basePlayer.eyes.position;
				Quaternion rotation = basePlayer.eyes.rotation;
				Vector3 position2 = eyes.position;
				Quaternion rotation2 = eyes.rotation;
				val.AttackerPosition = new Vector3(position.x, position.y, position.z);
				val.AttackerViewRotation = new Quaternion(rotation.w, rotation.x, rotation.y, rotation.z);
				val.VictimPosition = new Vector3(position2.x, position2.y, position2.z);
				val.VictimViewRotation = new Quaternion(rotation2.w, rotation2.x, rotation2.y, rotation2.z);
				EACServer.playerTracker.LogPlayerTakeDamage(client, client2, val);
			}
		}
		metabolism.SendChangesToClient();
		if (info.PointStart != Vector3.get_zero() && info.damageTypes.Total() >= 0f)
		{
			ClientRPCPlayerAndSpectators<Vector3, int>(null, this, "DirectionalDamage", info.PointStart, (int)info.damageTypes.GetMajorityDamageType());
		}
	}

	public override void Heal(float amount)
	{
		if (IsCrawling())
		{
			float num = base.health;
			base.Heal(amount);
			healingWhileCrawling += base.health - num;
		}
		else
		{
			base.Heal(amount);
		}
	}

	public static BasePlayer FindBot(ulong userId)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<BasePlayer> enumerator = bots.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.get_Current();
				if (current.userID == userId)
				{
					return current;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return FindBotClosestMatch(userId.ToString());
	}

	public static BasePlayer FindBotClosestMatch(string name)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		Enumerator<BasePlayer> enumerator = bots.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.get_Current();
				if (current.displayName.Contains(name))
				{
					return current;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return null;
	}

	public static BasePlayer FindByID(ulong userID)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("BasePlayer.FindByID", 0);
		try
		{
			Enumerator<BasePlayer> enumerator = activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.get_Current();
					if (current.userID == userID)
					{
						return current;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return null;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool TryFindByID(ulong userID, out BasePlayer basePlayer)
	{
		basePlayer = FindByID(userID);
		return (Object)(object)basePlayer != (Object)null;
	}

	public static BasePlayer FindSleeping(ulong userID)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("BasePlayer.FindSleeping", 0);
		try
		{
			Enumerator<BasePlayer> enumerator = sleepingPlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.get_Current();
					if (current.userID == userID)
					{
						return current;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return null;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void Command(string strCommand, params object[] arguments)
	{
		if (net.get_connection() != null)
		{
			ConsoleNetwork.SendClientCommand(net.get_connection(), strCommand, arguments);
		}
	}

	public override void OnInvalidPosition()
	{
		if (!IsDead())
		{
			Die();
		}
	}

	private static BasePlayer Find(string strNameOrIDOrIP, IEnumerable<BasePlayer> list)
	{
		BasePlayer basePlayer = Enumerable.FirstOrDefault<BasePlayer>(list, (Func<BasePlayer, bool>)((BasePlayer x) => x.UserIDString == strNameOrIDOrIP));
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			return basePlayer;
		}
		BasePlayer basePlayer2 = Enumerable.FirstOrDefault<BasePlayer>(list, (Func<BasePlayer, bool>)((BasePlayer x) => x.displayName.StartsWith(strNameOrIDOrIP, StringComparison.CurrentCultureIgnoreCase)));
		if (Object.op_Implicit((Object)(object)basePlayer2))
		{
			return basePlayer2;
		}
		BasePlayer basePlayer3 = Enumerable.FirstOrDefault<BasePlayer>(list, (Func<BasePlayer, bool>)((BasePlayer x) => x.net != null && x.net.get_connection() != null && x.net.get_connection().ipaddress == strNameOrIDOrIP));
		if (Object.op_Implicit((Object)(object)basePlayer3))
		{
			return basePlayer3;
		}
		return null;
	}

	public static BasePlayer Find(string strNameOrIDOrIP)
	{
		return Find(strNameOrIDOrIP, (IEnumerable<BasePlayer>)activePlayerList);
	}

	public static BasePlayer FindSleeping(string strNameOrIDOrIP)
	{
		return Find(strNameOrIDOrIP, (IEnumerable<BasePlayer>)sleepingPlayerList);
	}

	public static BasePlayer FindAwakeOrSleeping(string strNameOrIDOrIP)
	{
		return Find(strNameOrIDOrIP, allPlayerList);
	}

	public void SendConsoleCommand(string command, params object[] obj)
	{
		ConsoleNetwork.SendClientCommand(net.get_connection(), command, obj);
	}

	public void UpdateRadiation(float fAmount)
	{
		metabolism.radiation_level.Increase(fAmount);
	}

	public override float RadiationExposureFraction()
	{
		float num = Mathf.Clamp(baseProtection.amounts[17], 0f, 1f);
		return 1f - num;
	}

	public override float RadiationProtection()
	{
		return baseProtection.amounts[17] * 100f;
	}

	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		base.OnHealthChanged(oldvalue, newvalue);
		if (base.isServer)
		{
			if (oldvalue > newvalue)
			{
				LifeStoryHurt(oldvalue - newvalue);
			}
			else
			{
				LifeStoryHeal(newvalue - oldvalue);
			}
			metabolism.isDirty = true;
		}
	}

	public void SV_ClothingChanged()
	{
		UpdateProtectionFromClothing();
		UpdateMoveSpeedFromClothing();
	}

	public bool IsNoob()
	{
		return !HasPlayerFlag(PlayerFlags.DisplaySash);
	}

	public bool HasHostileItem()
	{
		TimeWarning val = TimeWarning.New("BasePlayer.HasHostileItem", 0);
		try
		{
			foreach (Item item in inventory.containerBelt.itemList)
			{
				if (IsHostileItem(item))
				{
					return true;
				}
			}
			foreach (Item item2 in inventory.containerMain.itemList)
			{
				if (IsHostileItem(item2))
				{
					return true;
				}
			}
			return false;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void GiveItem(Item item, GiveItemReason reason = GiveItemReason.Generic)
	{
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		if (reason == GiveItemReason.ResourceHarvested)
		{
			stats.Add($"harvest.{item.info.shortname}", item.amount, (Stats)6);
		}
		if (reason == GiveItemReason.ResourceHarvested || reason == GiveItemReason.Crafted)
		{
			ProcessMissionEvent(BaseMission.MissionEventType.HARVEST, item.info.shortname, item.amount);
		}
		int amount = item.amount;
		if (inventory.GiveItem(item))
		{
			if (!string.IsNullOrEmpty(item.name))
			{
				Command("note.inv", item.info.itemid, amount, item.name, (int)reason);
			}
			else
			{
				Command("note.inv", item.info.itemid, amount, string.Empty, (int)reason);
			}
		}
		else
		{
			item.Drop(inventory.containerMain.dropPosition, inventory.containerMain.dropVelocity);
		}
	}

	public override void AttackerInfo(DeathInfo info)
	{
		info.attackerName = displayName;
		info.attackerSteamID = userID;
	}

	public virtual bool ShouldDropActiveItem()
	{
		return true;
	}

	public override void Die(HitInfo info = null)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("Player.Die", 0);
		try
		{
			if (!IsDead())
			{
				if (Belt != null && ShouldDropActiveItem())
				{
					Vector3 val2 = default(Vector3);
					((Vector3)(ref val2))._002Ector(Random.Range(-2f, 2f), 0.2f, Random.Range(-2f, 2f));
					Belt.DropActive(GetDropPosition(), GetInheritedDropVelocity() + ((Vector3)(ref val2)).get_normalized() * 3f);
				}
				if (!WoundInsteadOfDying(info))
				{
					base.Die(info);
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void Kick(string reason)
	{
		if (IsConnected)
		{
			Net.sv.Kick(net.get_connection(), reason, false);
		}
	}

	public override Vector3 GetDropPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return eyes.position;
	}

	public override Vector3 GetDropVelocity()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		return GetInheritedDropVelocity() + eyes.BodyForward() * 4f + Vector3Ex.Range(-0.5f, 0.5f);
	}

	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = GetParentEntity();
		if ((Object)(object)baseEntity != (Object)null)
		{
			ClientRPCPlayer<Vector3, uint>(null, this, "SetInheritedVelocity", ((Component)baseEntity).get_transform().InverseTransformDirection(velocity), baseEntity.net.ID);
		}
		else
		{
			ClientRPCPlayer<Vector3>(null, this, "SetInheritedVelocity", velocity);
		}
		PauseSpeedHackDetection();
	}

	public virtual void SetInfo(string key, string val)
	{
		if (IsConnected)
		{
			net.get_connection().info.Set(key, val);
		}
	}

	public virtual int GetInfoInt(string key, int defaultVal)
	{
		if (!IsConnected)
		{
			return defaultVal;
		}
		return net.get_connection().info.GetInt(key, defaultVal);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(1uL)]
	public void PerformanceReport(RPCMessage msg)
	{
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		float num3 = msg.read.Float();
		int num4 = msg.read.Int32();
		bool flag = msg.read.Bit();
		string text = (num + "MB").PadRight(9);
		string text2 = (num2 + "MB").PadRight(9);
		string text3 = (num3.ToString("0") + "FPS").PadRight(8);
		string text4 = NumberExtensions.FormatSeconds((long)num4).PadRight(9);
		string text5 = UserIDString.PadRight(20);
		string text6 = flag.ToString().PadRight(7);
		DebugEx.Log((object)(text + text2 + text3 + text4 + text6 + text5 + displayName), (StackTraceLogType)0);
	}

	public override bool ShouldNetworkTo(BasePlayer player)
	{
		if (IsSpectating() && (Object)(object)player != (Object)(object)this && !player.net.get_connection().info.GetBool("global.specnet", false))
		{
			return false;
		}
		return base.ShouldNetworkTo(player);
	}

	internal void GiveAchievement(string name)
	{
		if (GameInfo.HasAchievements)
		{
			ClientRPCPlayer(null, this, "RecieveAchievement", name);
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(1uL)]
	public void OnPlayerReported(RPCMessage msg)
	{
		string text = msg.read.String(256);
		string message = msg.read.StringMultiLine(2048);
		string type = msg.read.String(256);
		string text2 = msg.read.String(256);
		string text3 = msg.read.String(256);
		DebugEx.Log((object)$"[PlayerReport] {this} reported {text3}[{text2}] - \"{text}\"", (StackTraceLogType)0);
		RCon.Broadcast(RCon.LogType.Report, new
		{
			PlayerId = UserIDString,
			PlayerName = displayName,
			TargetId = text2,
			TargetName = text3,
			Subject = text,
			Message = message,
			Type = type
		});
	}

	public void StartDemoRecording()
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (net != null && net.get_connection() != null && !net.get_connection().get_IsRecording())
		{
			string text = $"demos/{UserIDString}/{DateTime.Now:yyyy-MM-dd-hhmmss}.dem";
			Debug.Log((object)(((object)this).ToString() + " recording started: " + text));
			net.get_connection().StartRecording(text, (IDemoHeader)(object)new Demo.Header
			{
				version = Demo.Version,
				level = Application.get_loadedLevelName(),
				levelSeed = World.Seed,
				levelSize = World.Size,
				checksum = World.Checksum,
				localclient = userID,
				position = eyes.position,
				rotation = eyes.HeadForward(),
				levelUrl = World.Url,
				recordedTime = DateTime.Now.ToBinary()
			});
			SendNetworkUpdateImmediate();
			SendGlobalSnapshot();
			SendFullSnapshot();
			ServerMgr.SendReplicatedVars(net.get_connection());
			((FacepunchBehaviour)this).InvokeRepeating((Action)MonitorDemoRecording, 10f, 10f);
		}
	}

	public void StopDemoRecording()
	{
		if (net != null && net.get_connection() != null && net.get_connection().get_IsRecording())
		{
			Debug.Log((object)(((object)this).ToString() + " recording stopped: " + net.get_connection().get_RecordFilename()));
			net.get_connection().StopRecording();
			((FacepunchBehaviour)this).CancelInvoke((Action)MonitorDemoRecording);
		}
	}

	public void MonitorDemoRecording()
	{
		if (net != null && net.get_connection() != null && net.get_connection().get_IsRecording() && (net.get_connection().get_RecordTimeElapsed().TotalSeconds >= (double)Demo.splitseconds || (float)net.get_connection().get_RecordFilesize() >= Demo.splitmegabytes * 1024f * 1024f))
		{
			StopDemoRecording();
			StartDemoRecording();
		}
	}

	public bool IsPlayerVisibleToUs(BasePlayer otherPlayer, int layerMask)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)otherPlayer == (Object)null)
		{
			return false;
		}
		Vector3 val = (isMounted ? eyes.worldMountedPosition : (IsDucked() ? eyes.worldCrouchedPosition : ((!IsCrawling()) ? eyes.worldStandingPosition : eyes.worldCrawlingPosition)));
		if (!otherPlayer.IsVisibleSpecificLayers(val, otherPlayer.CenterPoint(), layerMask) && !otherPlayer.IsVisibleSpecificLayers(val, ((Component)otherPlayer).get_transform().get_position(), layerMask) && !otherPlayer.IsVisibleSpecificLayers(val, otherPlayer.eyes.position, layerMask))
		{
			return false;
		}
		if (!IsVisibleSpecificLayers(otherPlayer.CenterPoint(), val, layerMask) && !IsVisibleSpecificLayers(((Component)otherPlayer).get_transform().get_position(), val, layerMask) && !IsVisibleSpecificLayers(otherPlayer.eyes.position, val, layerMask))
		{
			return false;
		}
		return true;
	}

	private void Tick_Spectator()
	{
		int num = 0;
		if (serverInput.WasJustPressed(BUTTON.JUMP))
		{
			num++;
		}
		if (serverInput.WasJustPressed(BUTTON.DUCK))
		{
			num--;
		}
		if (num != 0)
		{
			SpectateOffset += num;
			TimeWarning val = TimeWarning.New("UpdateSpectateTarget", 0);
			try
			{
				UpdateSpectateTarget(spectateFilter);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public void UpdateSpectateTarget(string strName)
	{
		spectateFilter = strName;
		IEnumerable<BaseEntity> enumerable = null;
		if (spectateFilter.StartsWith("@"))
		{
			string filter = spectateFilter.Substring(1);
			enumerable = Enumerable.Cast<BaseEntity>((IEnumerable)Enumerable.Where<BaseNetworkable>(Enumerable.Where<BaseNetworkable>((IEnumerable<BaseNetworkable>)BaseNetworkable.serverEntities, (Func<BaseNetworkable, bool>)((BaseNetworkable x) => StringEx.Contains(((Object)x).get_name(), filter, CompareOptions.IgnoreCase))), (Func<BaseNetworkable, bool>)((BaseNetworkable x) => (Object)(object)x != (Object)(object)this)));
		}
		else
		{
			IEnumerable<BasePlayer> enumerable2 = Enumerable.Where<BasePlayer>((IEnumerable<BasePlayer>)activePlayerList, (Func<BasePlayer, bool>)((BasePlayer x) => !x.IsSpectating() && !x.IsDead() && !x.IsSleeping()));
			if (strName.Length > 0)
			{
				enumerable2 = Enumerable.Where<BasePlayer>(Enumerable.Where<BasePlayer>(enumerable2, (Func<BasePlayer, bool>)((BasePlayer x) => StringEx.Contains(x.displayName, spectateFilter, CompareOptions.IgnoreCase) || x.UserIDString.Contains(spectateFilter))), (Func<BasePlayer, bool>)((BasePlayer x) => (Object)(object)x != (Object)(object)this));
			}
			enumerable2 = (IEnumerable<BasePlayer>)Enumerable.OrderBy<BasePlayer, string>(enumerable2, (Func<BasePlayer, string>)((BasePlayer x) => x.displayName));
			enumerable = Enumerable.Cast<BaseEntity>((IEnumerable)enumerable2);
		}
		BaseEntity[] array = Enumerable.ToArray<BaseEntity>(enumerable);
		if (array.Length == 0)
		{
			ChatMessage("No valid spectate targets!");
			return;
		}
		BaseEntity baseEntity = array[SpectateOffset % array.Length];
		if ((Object)(object)baseEntity != (Object)null)
		{
			if (baseEntity is BasePlayer)
			{
				ChatMessage("Spectating: " + (baseEntity as BasePlayer).displayName);
			}
			else
			{
				ChatMessage("Spectating: " + ((object)baseEntity).ToString());
			}
			TimeWarning val = TimeWarning.New("SendEntitySnapshot", 0);
			try
			{
				SendEntitySnapshot(baseEntity);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			((Component)this).get_gameObject().Identity();
			val = TimeWarning.New("SetParent", 0);
			try
			{
				SetParent(baseEntity);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public void StartSpectating()
	{
		if (!IsSpectating())
		{
			SetPlayerFlag(PlayerFlags.Spectating, b: true);
			((Component)this).get_gameObject().SetLayerRecursive(10);
			((FacepunchBehaviour)this).CancelInvoke((Action)InventoryUpdate);
			ChatMessage("Becoming Spectator");
			UpdateSpectateTarget(spectateFilter);
		}
	}

	public void StopSpectating()
	{
		if (IsSpectating())
		{
			SetParent(null);
			SetPlayerFlag(PlayerFlags.Spectating, b: false);
			((Component)this).get_gameObject().SetLayerRecursive(17);
		}
	}

	public void Teleport(BasePlayer player)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Teleport(((Component)player).get_transform().get_position());
	}

	public void Teleport(string strName, bool playersOnly)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity[] array = Util.FindTargets(strName, playersOnly);
		if (array != null && array.Length != 0)
		{
			BaseEntity baseEntity = array[Random.Range(0, array.Length)];
			Teleport(((Component)baseEntity).get_transform().get_position());
		}
	}

	public void Teleport(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		MovePosition(position);
		ClientRPCPlayer<Vector3>(null, this, "ForcePositionTo", position);
	}

	public void CopyRotation(BasePlayer player)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		viewAngles = player.viewAngles;
		SendNetworkUpdate_Position();
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (child is BasePlayer)
		{
			IsBeingSpectated = true;
		}
	}

	protected override void OnChildRemoved(BaseEntity child)
	{
		base.OnChildRemoved(child);
		if (!(child is BasePlayer))
		{
			return;
		}
		IsBeingSpectated = false;
		foreach (BaseEntity child2 in children)
		{
			if (child2 is BasePlayer)
			{
				IsBeingSpectated = true;
			}
		}
	}

	public void ClientRPCPlayerAndSpectators(Connection sourceConnection, BasePlayer player, string funcName)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.sv.IsConnected() || net == null || player.net.get_connection() == null)
		{
			return;
		}
		ClientRPCEx(new SendInfo(player.net.get_connection()), sourceConnection, funcName);
		if (!IsBeingSpectated || children == null)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			BasePlayer player2;
			if ((player2 = child as BasePlayer) != null)
			{
				ClientRPCPlayer(sourceConnection, player2, funcName);
			}
		}
	}

	public void ClientRPCPlayerAndSpectators<T1>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.sv.IsConnected() || net == null || player.net.get_connection() == null)
		{
			return;
		}
		ClientRPCEx(new SendInfo(player.net.get_connection()), sourceConnection, funcName, arg1);
		if (!IsBeingSpectated || children == null)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			BasePlayer player2;
			if ((player2 = child as BasePlayer) != null)
			{
				ClientRPCPlayer(sourceConnection, player2, funcName, arg1);
			}
		}
	}

	public void ClientRPCPlayerAndSpectators<T1, T2>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1, T2 arg2)
	{
		if (!Net.sv.IsConnected() || net == null || player.net.get_connection() == null)
		{
			return;
		}
		ClientRPCPlayer(sourceConnection, player, funcName, arg1, arg2);
		if (!IsBeingSpectated || children == null)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			BasePlayer player2;
			if ((player2 = child as BasePlayer) != null)
			{
				ClientRPCPlayer(sourceConnection, player2, funcName, arg1, arg2);
			}
		}
	}

	public override float GetThreatLevel()
	{
		EnsureUpdated();
		return cachedThreatLevel;
	}

	public void EnsureUpdated()
	{
		if (Time.get_realtimeSinceStartup() - lastUpdateTime < 30f)
		{
			return;
		}
		lastUpdateTime = Time.get_realtimeSinceStartup();
		cachedThreatLevel = 0f;
		if (IsSleeping())
		{
			return;
		}
		if (inventory.containerWear.itemList.Count > 2)
		{
			cachedThreatLevel += 1f;
		}
		foreach (Item item in inventory.containerBelt.itemList)
		{
			BaseEntity heldEntity = item.GetHeldEntity();
			if (Object.op_Implicit((Object)(object)heldEntity) && heldEntity is BaseProjectile && !(heldEntity is BowWeapon))
			{
				cachedThreatLevel += 2f;
				break;
			}
		}
	}

	public override bool IsHostile()
	{
		return State.unHostileTimestamp > TimeEx.get_currentTimestamp();
	}

	public virtual float GetHostileDuration()
	{
		return Mathf.Clamp((float)(State.unHostileTimestamp - TimeEx.get_currentTimestamp()), 0f, float.PositiveInfinity);
	}

	public override void MarkHostileFor(float duration = 60f)
	{
		double currentTimestamp = TimeEx.get_currentTimestamp();
		double val = currentTimestamp + (double)duration;
		State.unHostileTimestamp = Math.Max(State.unHostileTimestamp, val);
		DirtyPlayerState();
		double num = Math.Max(State.unHostileTimestamp - currentTimestamp, 0.0);
		ClientRPCPlayer(null, this, "SetHostileLength", (float)num);
	}

	public void MarkWeaponDrawnDuration(float newDuration)
	{
		float num = weaponDrawnDuration;
		weaponDrawnDuration = newDuration;
		if ((float)Mathf.FloorToInt(newDuration) != num)
		{
			ClientRPCPlayer(null, this, "SetWeaponDrawnDuration", weaponDrawnDuration);
		}
	}

	public void AddWeaponDrawnDuration(float duration)
	{
		MarkWeaponDrawnDuration(weaponDrawnDuration + duration);
	}

	public void OnReceivedTick(Stream stream)
	{
		TimeWarning val = TimeWarning.New("OnReceiveTickFromStream", 0);
		try
		{
			PlayerTick val2 = null;
			TimeWarning val3 = TimeWarning.New("PlayerTick.Deserialize", 0);
			try
			{
				val2 = PlayerTick.Deserialize(stream, lastReceivedTick, true);
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			val3 = TimeWarning.New("RecordPacket", 0);
			try
			{
				net.get_connection().RecordPacket((byte)15, (IProto)(object)val2);
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			val3 = TimeWarning.New("PlayerTick.Copy", 0);
			try
			{
				lastReceivedTick = val2.Copy();
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			val3 = TimeWarning.New("OnReceiveTick", 0);
			try
			{
				OnReceiveTick(val2, wasStalled);
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			lastTickTime = Time.get_time();
			val2.Dispose();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void OnReceivedVoice(byte[] data)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).get_write().Start())
		{
			((BaseNetwork)Net.sv).get_write().PacketID((Type)21);
			((BaseNetwork)Net.sv).get_write().UInt32(net.ID);
			((BaseNetwork)Net.sv).get_write().BytesWithSize(data);
			NetWrite write = ((BaseNetwork)Net.sv).get_write();
			SendInfo val = default(SendInfo);
			((SendInfo)(ref val))._002Ector(BaseNetworkable.GetConnectionsWithin(((Component)this).get_transform().get_position(), 100f));
			val.priority = (Priority)0;
			write.Send(val);
		}
		if ((Object)(object)activeTelephone != (Object)null)
		{
			activeTelephone.OnReceivedVoiceFromUser(data);
		}
	}

	public void ResetInputIdleTime()
	{
		lastInputTime = Time.get_time();
	}

	private void EACStateUpdate()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		if (net == null || net.get_connection() == null || EACServer.playerTracker == null || IsReceivingSnapshot)
		{
			return;
		}
		Vector3 position = eyes.position;
		Quaternion rotation = eyes.rotation;
		Client client = EACServer.GetClient(net.get_connection());
		PlayerTick val = default(PlayerTick);
		val.Position = new Vector3(position.x, position.y, position.z);
		val.ViewRotation = new Quaternion(rotation.w, rotation.x, rotation.y, rotation.z);
		if (IsDucked())
		{
			ref PlayerTickFlags tickFlags = ref val.TickFlags;
			tickFlags = (PlayerTickFlags)((uint)tickFlags | 1u);
		}
		if (isMounted)
		{
			ref PlayerTickFlags tickFlags2 = ref val.TickFlags;
			tickFlags2 = (PlayerTickFlags)((uint)tickFlags2 | 4u);
		}
		if (IsWounded())
		{
			ref PlayerTickFlags tickFlags3 = ref val.TickFlags;
			tickFlags3 = (PlayerTickFlags)((uint)tickFlags3 | 8u);
		}
		if (IsSwimming())
		{
			ref PlayerTickFlags tickFlags4 = ref val.TickFlags;
			tickFlags4 = (PlayerTickFlags)((uint)tickFlags4 | 0x10u);
		}
		if (!IsOnGround())
		{
			ref PlayerTickFlags tickFlags5 = ref val.TickFlags;
			tickFlags5 = (PlayerTickFlags)((uint)tickFlags5 | 0x20u);
		}
		if (OnLadder())
		{
			ref PlayerTickFlags tickFlags6 = ref val.TickFlags;
			tickFlags6 = (PlayerTickFlags)((uint)tickFlags6 | 0x40u);
		}
		TimeWarning val2 = TimeWarning.New("playerTracker.LogPlayerState", 0);
		try
		{
			EACServer.playerTracker.LogPlayerTick(client, val);
		}
		catch (Exception ex)
		{
			Debug.LogWarning((object)"Disabling EAC Logging due to exception");
			EACServer.playerTracker = null;
			Debug.LogException(ex);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void OnReceiveTick(PlayerTick msg, bool wasPlayerStalled)
	{
		if (msg.inputState != null)
		{
			serverInput.Flip(msg.inputState);
		}
		if (serverInput.current.buttons != serverInput.previous.buttons)
		{
			ResetInputIdleTime();
		}
		if (IsReceivingSnapshot)
		{
			return;
		}
		if (IsSpectating())
		{
			TimeWarning val = TimeWarning.New("Tick_Spectator", 0);
			try
			{
				Tick_Spectator();
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		else
		{
			if (IsDead())
			{
				return;
			}
			if (IsSleeping())
			{
				if (serverInput.WasJustPressed(BUTTON.FIRE_PRIMARY) || serverInput.WasJustPressed(BUTTON.FIRE_SECONDARY) || serverInput.WasJustPressed(BUTTON.JUMP) || serverInput.WasJustPressed(BUTTON.DUCK))
				{
					EndSleeping();
					SendNetworkUpdateImmediate();
				}
				UpdateActiveItem(0u);
				return;
			}
			UpdateActiveItem(msg.activeItem);
			UpdateModelStateFromTick(msg);
			if (!IsIncapacitated())
			{
				if (isMounted)
				{
					GetMounted().PlayerServerInput(serverInput, this);
				}
				UpdatePositionFromTick(msg, wasPlayerStalled);
				UpdateRotationFromTick(msg);
			}
		}
	}

	public void UpdateActiveItem(uint itemID)
	{
		Assert.IsTrue(base.isServer, "Realm should be server!");
		if (svActiveItemID == itemID)
		{
			return;
		}
		if (equippingBlocked)
		{
			itemID = 0u;
		}
		Item item = inventory.containerBelt.FindItemByUID(itemID);
		if (IsItemHoldRestricted(item))
		{
			itemID = 0u;
		}
		Item activeItem = GetActiveItem();
		svActiveItemID = 0u;
		if (activeItem != null)
		{
			HeldEntity heldEntity = activeItem.GetHeldEntity() as HeldEntity;
			if ((Object)(object)heldEntity != (Object)null)
			{
				heldEntity.SetHeld(bHeld: false);
			}
		}
		svActiveItemID = itemID;
		SendNetworkUpdate();
		Item activeItem2 = GetActiveItem();
		if (activeItem2 != null)
		{
			HeldEntity heldEntity2 = activeItem2.GetHeldEntity() as HeldEntity;
			if ((Object)(object)heldEntity2 != (Object)null)
			{
				heldEntity2.SetHeld(bHeld: true);
			}
			NotifyGesturesNewItemEquipped();
		}
		inventory.UpdatedVisibleHolsteredItems();
	}

	internal void UpdateModelStateFromTick(PlayerTick tick)
	{
		if (tick.modelState != null && !ModelState.Equal(modelStateTick, tick.modelState))
		{
			if (modelStateTick != null)
			{
				modelStateTick.ResetToPool();
			}
			modelStateTick = tick.modelState;
			tick.modelState = null;
			tickNeedsFinalizing = true;
		}
	}

	internal void UpdatePositionFromTick(PlayerTick tick, bool wasPlayerStalled)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3Ex.IsNaNOrInfinity(tick.position) || Vector3Ex.IsNaNOrInfinity(tick.eyePos))
		{
			Kick("Kicked: Invalid Position");
		}
		else
		{
			if (tick.parentID != parentEntity.uid || isMounted || (modelState != null && modelState.get_mounted()) || (modelStateTick != null && modelStateTick.get_mounted()))
			{
				return;
			}
			if (wasPlayerStalled)
			{
				float num = Vector3.Distance(tick.position, tickInterpolator.EndPoint);
				if (num > 0.01f)
				{
					AntiHack.ResetTimer(this);
				}
				if (num > 0.5f)
				{
					ClientRPCPlayer<Vector3, uint>(null, this, "ForcePositionToParentOffset", tickInterpolator.EndPoint, parentEntity.uid);
				}
			}
			else if ((modelState == null || !modelState.get_flying() || (!IsAdmin && !IsDeveloper)) && Vector3.Distance(tick.position, tickInterpolator.EndPoint) > 5f)
			{
				AntiHack.ResetTimer(this);
				ClientRPCPlayer<Vector3, uint>(null, this, "ForcePositionToParentOffset", tickInterpolator.EndPoint, parentEntity.uid);
			}
			else
			{
				tickInterpolator.AddPoint(tick.position);
				tickNeedsFinalizing = true;
			}
		}
	}

	internal void UpdateRotationFromTick(PlayerTick tick)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (tick.inputState != null)
		{
			if (Vector3Ex.IsNaNOrInfinity(tick.inputState.aimAngles))
			{
				Kick("Kicked: Invalid Rotation");
				return;
			}
			tickViewAngles = tick.inputState.aimAngles;
			tickNeedsFinalizing = true;
		}
	}

	public void UpdateEstimatedVelocity(Vector3 lastPos, Vector3 currentPos, float deltaTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		estimatedVelocity = (currentPos - lastPos) / deltaTime;
		Vector3 val = estimatedVelocity;
		estimatedSpeed = ((Vector3)(ref val)).get_magnitude();
		estimatedSpeed2D = Vector3Ex.Magnitude2D(estimatedVelocity);
		if (estimatedSpeed < 0.01f)
		{
			estimatedSpeed = 0f;
		}
		if (estimatedSpeed2D < 0.01f)
		{
			estimatedSpeed2D = 0f;
		}
	}

	private void FinalizeTick(float deltaTime)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		tickDeltaTime += deltaTime;
		if (IsReceivingSnapshot || !tickNeedsFinalizing)
		{
			return;
		}
		tickNeedsFinalizing = false;
		TimeWarning val = TimeWarning.New("ModelState", 0);
		try
		{
			if (modelStateTick != null)
			{
				if (modelStateTick.get_flying() && !IsAdmin && !IsDeveloper)
				{
					AntiHack.NoteAdminHack(this);
				}
				if (modelStateTick.inheritedVelocity != Vector3.get_zero() && (Object)(object)FindTrigger<TriggerForce>() == (Object)null)
				{
					modelStateTick.inheritedVelocity = Vector3.get_zero();
				}
				if (modelState != null)
				{
					if (ConVar.AntiHack.modelstate && TriggeredAntiHack())
					{
						modelStateTick.set_ducked(modelState.get_ducked());
					}
					modelState.ResetToPool();
					modelState = null;
				}
				modelState = modelStateTick;
				modelStateTick = null;
				UpdateModelState();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("Transform", 0);
		try
		{
			UpdateEstimatedVelocity(tickInterpolator.StartPoint, tickInterpolator.EndPoint, tickDeltaTime);
			bool flag = tickInterpolator.StartPoint != tickInterpolator.EndPoint;
			bool flag2 = tickViewAngles != viewAngles;
			if (flag)
			{
				if (AntiHack.ValidateMove(this, tickInterpolator, tickDeltaTime))
				{
					((Component)this).get_transform().set_localPosition(tickInterpolator.EndPoint);
					ticksPerSecond.Increment();
					tickHistory.AddPoint(tickInterpolator.EndPoint, tickHistoryCapacity);
					AntiHack.FadeViolations(this, tickDeltaTime);
				}
				else
				{
					flag = false;
					if (ConVar.AntiHack.forceposition)
					{
						ClientRPCPlayer<Vector3, uint>(null, this, "ForcePositionToParentOffset", ((Component)this).get_transform().get_localPosition(), parentEntity.uid);
					}
				}
			}
			tickInterpolator.Reset(((Component)this).get_transform().get_localPosition());
			if (flag2)
			{
				viewAngles = tickViewAngles;
				((Component)this).get_transform().set_rotation(Quaternion.get_identity());
				((Component)this).get_transform().set_hasChanged(true);
			}
			if (flag || flag2)
			{
				eyes.NetworkUpdate(Quaternion.Euler(viewAngles));
				NetworkPositionTick();
			}
			AntiHack.ValidateEyeHistory(this);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("ModelState", 0);
		try
		{
			if (modelState != null)
			{
				modelState.waterLevel = WaterFactor();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("EACStateUpdate", 0);
		try
		{
			EACStateUpdate();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("AntiHack.EnforceViolations", 0);
		try
		{
			AntiHack.EnforceViolations(this);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		tickDeltaTime = 0f;
	}

	public uint GetUnderwearSkin()
	{
		uint infoInt = (uint)GetInfoInt("client.underwearskin", 0);
		if (infoInt != lastValidUnderwearSkin && Time.get_time() > nextUnderwearValidationTime)
		{
			UnderwearManifest underwearManifest = UnderwearManifest.Get();
			nextUnderwearValidationTime = Time.get_time() + 0.2f;
			Underwear underwear = underwearManifest.GetUnderwear(infoInt);
			if ((Object)(object)underwear == (Object)null)
			{
				lastValidUnderwearSkin = 0u;
			}
			else if (Underwear.Validate(underwear, this))
			{
				lastValidUnderwearSkin = infoInt;
			}
		}
		return lastValidUnderwearSkin;
	}

	[RPC_Server]
	public void ServerRPC_UnderwearChange(RPCMessage msg)
	{
		if (!((Object)(object)msg.player != (Object)(object)this))
		{
			uint num = lastValidUnderwearSkin;
			uint underwearSkin = GetUnderwearSkin();
			if (num != underwearSkin)
			{
				SendNetworkUpdate();
			}
		}
	}

	public bool IsWounded()
	{
		return HasPlayerFlag(PlayerFlags.Wounded);
	}

	public bool IsCrawling()
	{
		if (HasPlayerFlag(PlayerFlags.Wounded))
		{
			return !HasPlayerFlag(PlayerFlags.Incapacitated);
		}
		return false;
	}

	public bool IsIncapacitated()
	{
		return HasPlayerFlag(PlayerFlags.Incapacitated);
	}

	private bool WoundInsteadOfDying(HitInfo info)
	{
		if (!EligibleForWounding(info))
		{
			return false;
		}
		BecomeWounded(info);
		return true;
	}

	private void ResetWoundingVars()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)WoundingTick);
		woundedDuration = 0f;
		lastWoundedStartTime = float.NegativeInfinity;
		healingWhileCrawling = 0f;
		woundedByFallDamage = false;
	}

	public virtual bool EligibleForWounding(HitInfo info)
	{
		if (!ConVar.Server.woundingenabled)
		{
			return false;
		}
		if (IsWounded())
		{
			return false;
		}
		if (IsSleeping())
		{
			return false;
		}
		if (isMounted)
		{
			return false;
		}
		if (info == null)
		{
			return false;
		}
		if (!IsWounded() && Time.get_realtimeSinceStartup() - lastWoundedStartTime < ConVar.Server.rewounddelay)
		{
			return false;
		}
		if (triggers != null)
		{
			for (int i = 0; i < triggers.Count; i++)
			{
				if (triggers[i] is IHurtTrigger)
				{
					return false;
				}
			}
		}
		if (info.WeaponPrefab is BaseMelee)
		{
			return true;
		}
		if (info.WeaponPrefab is BaseProjectile)
		{
			return !info.isHeadshot;
		}
		return info.damageTypes.GetMajorityDamageType() switch
		{
			DamageType.Suicide => false, 
			DamageType.Fall => true, 
			DamageType.Bite => true, 
			DamageType.Bleeding => true, 
			DamageType.Hunger => true, 
			DamageType.Thirst => true, 
			DamageType.Poison => true, 
			_ => false, 
		};
	}

	public void BecomeWounded(HitInfo info = null)
	{
		if (IsWounded())
		{
			return;
		}
		bool flag = info != null && info.damageTypes.GetMajorityDamageType() == DamageType.Fall;
		if (IsCrawling())
		{
			woundedByFallDamage |= flag;
			GoToIncapacitated(info);
			return;
		}
		woundedByFallDamage = flag;
		if (flag)
		{
			GoToIncapacitated(info);
		}
		else
		{
			GoToCrawling(info);
		}
	}

	public void StopWounded(BasePlayer source = null)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (!IsWounded())
		{
			return;
		}
		RecoverFromWounded();
		((FacepunchBehaviour)this).CancelInvoke((Action)WoundingTick);
		if (EACServer.playerTracker != null && net.get_connection() != null && (Object)(object)source != (Object)null && source.net.get_connection() != null)
		{
			TimeWarning val = TimeWarning.New("playerTracker.LogPlayerRevive", 0);
			try
			{
				Client client = EACServer.GetClient(net.get_connection());
				Client client2 = EACServer.GetClient(source.net.get_connection());
				EACServer.playerTracker.LogPlayerRevive(client, client2);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public void ProlongWounding(float delay)
	{
		woundedDuration = Mathf.Max(woundedDuration, Mathf.Min(TimeSinceWoundedStarted + delay, woundedDuration + delay));
	}

	private void WoundingTick()
	{
		TimeWarning val = TimeWarning.New("WoundingTick", 0);
		try
		{
			if (IsDead())
			{
				return;
			}
			if (TimeSinceWoundedStarted >= woundedDuration)
			{
				float num = (IsIncapacitated() ? ConVar.Server.incapacitatedrecoverchance : ConVar.Server.woundedrecoverchance);
				float num2 = (metabolism.hydration.Fraction() + metabolism.calories.Fraction()) / 2f;
				float num3 = Mathf.Lerp(0f, ConVar.Server.woundedmaxfoodandwaterbonus, num2);
				float num4 = Mathf.Clamp01(num + num3);
				if (Random.get_value() < num4)
				{
					RecoverFromWounded();
					return;
				}
				if (woundedByFallDamage)
				{
					Die();
					return;
				}
				ItemDefinition itemDefinition = ItemManager.FindItemDefinition("largemedkit");
				Item item = inventory.containerBelt.FindItemByItemID(itemDefinition.itemid);
				if (item != null)
				{
					item.UseItem();
					RecoverFromWounded();
				}
				else
				{
					Die();
				}
			}
			else
			{
				if (IsSwimming() && IsCrawling())
				{
					GoToIncapacitated(null);
				}
				((FacepunchBehaviour)this).Invoke((Action)WoundingTick, 1f);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void GoToCrawling(HitInfo info)
	{
		base.health = Random.Range(ConVar.Server.crawlingminimumhealth, ConVar.Server.crawlingmaximumhealth);
		metabolism.bleeding.value = 0f;
		healingWhileCrawling = 0f;
		WoundedStartSharedCode(info);
		StartWoundedTick(40, 50);
		SendNetworkUpdateImmediate();
	}

	public void GoToIncapacitated(HitInfo info)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (!IsWounded())
		{
			WoundedStartSharedCode(info);
		}
		base.health = Random.Range(2f, 6f);
		metabolism.bleeding.value = 0f;
		healingWhileCrawling = 0f;
		SetPlayerFlag(PlayerFlags.Incapacitated, b: true);
		SetServerFall(wantsOn: true);
		BasePlayer basePlayer = info?.InitiatorPlayer;
		if (EACServer.playerTracker != null && net.get_connection() != null && (Object)(object)basePlayer != (Object)null && basePlayer.net.get_connection() != null)
		{
			TimeWarning val = TimeWarning.New("playerTracker.LogPlayerDowned", 0);
			try
			{
				Client client = EACServer.GetClient(net.get_connection());
				Client client2 = EACServer.GetClient(basePlayer.net.get_connection());
				EACServer.playerTracker.LogPlayerDowned(client, client2);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		StartWoundedTick(10, 25);
		SendNetworkUpdateImmediate();
	}

	private void WoundedStartSharedCode(HitInfo info)
	{
		stats.Add("wounded", 1, (Stats)5);
		SetPlayerFlag(PlayerFlags.Wounded, b: true);
		if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(base.isServer)))
		{
			BaseGameMode.GetActiveGameMode(base.isServer).OnPlayerWounded(info.InitiatorPlayer, this, info);
		}
	}

	private void StartWoundedTick(int minTime, int maxTime)
	{
		woundedDuration = Random.Range(minTime, maxTime + 1);
		lastWoundedStartTime = Time.get_realtimeSinceStartup();
		((FacepunchBehaviour)this).Invoke((Action)WoundingTick, 1f);
	}

	private void RecoverFromWounded()
	{
		if (IsCrawling())
		{
			base.health = Random.Range(2f, 6f) + healingWhileCrawling;
		}
		healingWhileCrawling = 0f;
		SetPlayerFlag(PlayerFlags.Wounded, b: false);
		SetPlayerFlag(PlayerFlags.Incapacitated, b: false);
		if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(base.isServer)))
		{
			BaseGameMode.GetActiveGameMode(base.isServer).OnPlayerRevived(null, this);
		}
	}

	private bool WoundingCausingImmortality(HitInfo info)
	{
		if (!IsWounded())
		{
			return false;
		}
		if (TimeSinceWoundedStarted > 0.25f)
		{
			return false;
		}
		if (info != null && info.damageTypes.GetMajorityDamageType() == DamageType.Fall)
		{
			return false;
		}
		return true;
	}

	public override BasePlayer ToPlayer()
	{
		return this;
	}

	public static string SanitizePlayerNameString(string playerName, ulong userId)
	{
		playerName = StringEx.EscapeRichText(StringEx.ToPrintable(playerName, 32)).Trim();
		if (string.IsNullOrWhiteSpace(playerName))
		{
			playerName = userId.ToString();
		}
		return playerName;
	}

	public bool IsGod()
	{
		if (base.isServer && (IsAdmin || IsDeveloper) && IsConnected && net.get_connection() != null && net.get_connection().info.GetBool("global.god", false))
		{
			return true;
		}
		return false;
	}

	public override Quaternion GetNetworkRotation()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			return Quaternion.Euler(viewAngles);
		}
		return Quaternion.get_identity();
	}

	public bool CanInteract()
	{
		return CanInteract(usableWhileCrawling: false);
	}

	public bool CanInteract(bool usableWhileCrawling)
	{
		if (!IsDead() && !IsSleeping() && !IsSpectating() && (usableWhileCrawling ? (!IsIncapacitated()) : (!IsWounded())))
		{
			return !HasActiveTelephone;
		}
		return false;
	}

	public override float StartHealth()
	{
		return Random.Range(50f, 60f);
	}

	public override float StartMaxHealth()
	{
		return 100f;
	}

	public override float MaxHealth()
	{
		return 100f * (1f + (((Object)(object)modifiers != (Object)null) ? modifiers.GetValue(Modifier.ModifierType.Max_Health) : 0f));
	}

	public override float MaxVelocity()
	{
		if (IsSleeping())
		{
			return 0f;
		}
		if (isMounted)
		{
			return GetMounted().MaxVelocity();
		}
		return GetMaxSpeed();
	}

	public Vector3 GetMountVelocity()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		BaseMountable baseMountable = GetMounted();
		if (!((Object)(object)baseMountable != (Object)null))
		{
			return Vector3.get_zero();
		}
		return baseMountable.GetWorldVelocity();
	}

	public override Vector3 GetInheritedProjectileVelocity()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		BaseMountable baseMountable = GetMounted();
		if (!Object.op_Implicit((Object)(object)baseMountable))
		{
			return base.GetInheritedProjectileVelocity();
		}
		return baseMountable.GetInheritedProjectileVelocity();
	}

	public override Vector3 GetInheritedThrowVelocity()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		BaseMountable baseMountable = GetMounted();
		if (!Object.op_Implicit((Object)(object)baseMountable))
		{
			return base.GetInheritedThrowVelocity();
		}
		return baseMountable.GetInheritedThrowVelocity();
	}

	public override Vector3 GetInheritedDropVelocity()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		BaseMountable baseMountable = GetMounted();
		if (!Object.op_Implicit((Object)(object)baseMountable))
		{
			return base.GetInheritedDropVelocity();
		}
		return baseMountable.GetInheritedDropVelocity();
	}

	public override void PreInitShared()
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		base.PreInitShared();
		cachedProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		baseProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		inventory = ((Component)this).GetComponent<PlayerInventory>();
		blueprints = ((Component)this).GetComponent<PlayerBlueprints>();
		metabolism = ((Component)this).GetComponent<PlayerMetabolism>();
		modifiers = ((Component)this).GetComponent<PlayerModifiers>();
		playerCollider = ((Component)this).GetComponent<CapsuleCollider>();
		eyes = ((Component)this).GetComponent<PlayerEyes>();
		playerColliderStanding = new CapsuleColliderInfo(playerCollider.get_height(), playerCollider.get_radius(), playerCollider.get_center());
		playerColliderDucked = new CapsuleColliderInfo(1.5f, playerCollider.get_radius(), Vector3.get_up() * 0.75f);
		playerColliderCrawling = new CapsuleColliderInfo(playerCollider.get_radius(), playerCollider.get_radius(), Vector3.get_up() * playerCollider.get_radius());
		playerColliderLyingDown = new CapsuleColliderInfo(0.4f, playerCollider.get_radius(), Vector3.get_up() * 0.2f);
		Belt = new PlayerBelt(this);
	}

	public override void DestroyShared()
	{
		Object.Destroy((Object)(object)cachedProtection);
		Object.Destroy((Object)(object)baseProtection);
		base.DestroyShared();
	}

	public static void ServerCycle(float deltaTime)
	{
		for (int i = 0; i < activePlayerList.get_Values().get_Count(); i++)
		{
			if ((Object)(object)activePlayerList.get_Values().get_Item(i) == (Object)null)
			{
				activePlayerList.RemoveAt(i--);
			}
		}
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		for (int j = 0; j < activePlayerList.get_Count(); j++)
		{
			list.Add(activePlayerList.get_Item(j));
		}
		for (int k = 0; k < list.Count; k++)
		{
			if (!((Object)(object)list[k] == (Object)null))
			{
				list[k].ServerUpdate(deltaTime);
			}
		}
		for (int l = 0; l < bots.get_Count(); l++)
		{
			if (!((Object)(object)bots.get_Item(l) == (Object)null))
			{
				bots.get_Item(l).ServerUpdateBots(deltaTime);
			}
		}
		if (ConVar.Server.idlekick > 0 && ((ServerMgr.AvailableSlots <= 0 && ConVar.Server.idlekickmode == 1) || ConVar.Server.idlekickmode == 2))
		{
			for (int m = 0; m < list.Count; m++)
			{
				if (!(list[m].IdleTime < (float)(ConVar.Server.idlekick * 60)) && (!list[m].IsAdmin || ConVar.Server.idlekickadmins != 0) && (!list[m].IsDeveloper || ConVar.Server.idlekickadmins != 0))
				{
					list[m].Kick("Idle for " + ConVar.Server.idlekick + " minutes");
				}
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
	}

	public bool InSafeZone()
	{
		if (base.isServer)
		{
			return currentSafeLevel > 0f;
		}
		return false;
	}

	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		if (baseEntity.InSafeZone() && baseEntity.userID != userID)
		{
			return false;
		}
		if ((Object)(object)RelationshipManager.ServerInstance != (Object)null)
		{
			if ((IsSleeping() || IsIncapacitated()) && !RelationshipManager.ServerInstance.HasRelations(baseEntity.userID, userID))
			{
				RelationshipManager.ServerInstance.SetRelationship(baseEntity, this, RelationshipManager.RelationshipType.Acquaintance);
			}
			RelationshipManager.ServerInstance.SetSeen(baseEntity, this);
		}
		if (IsCrawling())
		{
			GoToIncapacitated(null);
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	public Bounds GetBounds(bool ducked)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return new Bounds(((Component)this).get_transform().get_position() + GetOffset(ducked), GetSize(ducked));
	}

	public Bounds GetBounds()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return GetBounds(modelState.get_ducked());
	}

	public Vector3 GetCenter(bool ducked)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().get_position() + GetOffset(ducked);
	}

	public Vector3 GetCenter()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return GetCenter(modelState.get_ducked());
	}

	public Vector3 GetOffset(bool ducked)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (ducked)
		{
			return new Vector3(0f, 0.55f, 0f);
		}
		return new Vector3(0f, 0.9f, 0f);
	}

	public Vector3 GetOffset()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return GetOffset(modelState.get_ducked());
	}

	public Vector3 GetSize(bool ducked)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (ducked)
		{
			return new Vector3(1f, 1.1f, 1f);
		}
		return new Vector3(1f, 1.8f, 1f);
	}

	public Vector3 GetSize()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return GetSize(modelState.get_ducked());
	}

	public float GetHeight(bool ducked)
	{
		if (ducked)
		{
			return 1.1f;
		}
		return 1.8f;
	}

	public float GetHeight()
	{
		return GetHeight(modelState.get_ducked());
	}

	public float GetRadius()
	{
		return 0.5f;
	}

	public float GetJumpHeight()
	{
		return 1.5f;
	}

	public override Vector3 TriggerPoint()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).get_transform().get_position() + NoClipOffset();
	}

	public Vector3 NoClipOffset()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(0f, GetHeight(ducked: true) - GetRadius(), 0f);
	}

	public float NoClipRadius(float margin)
	{
		return GetRadius() - margin;
	}

	public float MaxDeployDistance(Item item)
	{
		return 8f;
	}

	public float GetMinSpeed()
	{
		return GetSpeed(0f, 0f, 1f);
	}

	public float GetMaxSpeed()
	{
		return GetSpeed(1f, 0f, 0f);
	}

	public float GetSpeed(float running, float ducking, float crawling)
	{
		float num = 1f;
		num -= clothingMoveSpeedReduction;
		if (IsSwimming())
		{
			num += clothingWaterSpeedBonus;
		}
		if (crawling > 0f)
		{
			return Mathf.Lerp(2.8f, 0.72f, crawling) * num;
		}
		return Mathf.Lerp(Mathf.Lerp(2.8f, 5.5f, running), 1.7f, ducking) * num;
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		float health_old = base.health;
		if (InSafeZone() && !IsHostile() && (Object)(object)info.Initiator != (Object)null && (Object)(object)info.Initiator != (Object)(object)this)
		{
			info.damageTypes.ScaleAll(0f);
		}
		if (base.isServer)
		{
			HitArea boneArea = info.boneArea;
			if (boneArea != (HitArea)(-1))
			{
				List<Item> list = Pool.GetList<Item>();
				list.AddRange(inventory.containerWear.itemList);
				for (int i = 0; i < list.Count; i++)
				{
					Item item = list[i];
					if (item != null)
					{
						ItemModWearable component = ((Component)item.info).GetComponent<ItemModWearable>();
						if (!((Object)(object)component == (Object)null) && component.ProtectsArea(boneArea))
						{
							item.OnAttacked(info);
						}
					}
				}
				Pool.FreeList<Item>(ref list);
				inventory.ServerUpdate(0f);
			}
		}
		base.OnAttacked(info);
		if (base.isServer && base.isServer && info.hasDamage)
		{
			if (!info.damageTypes.Has(DamageType.Bleeding) && info.damageTypes.IsBleedCausing() && !IsWounded() && !IsImmortalTo(info))
			{
				metabolism.bleeding.Add(info.damageTypes.Total() * 0.2f);
			}
			if (isMounted)
			{
				GetMounted().MounteeTookDamage(this, info);
			}
			CheckDeathCondition(info);
			if (net != null && net.get_connection() != null)
			{
				Effect effect = new Effect();
				effect.Init(Effect.Type.Generic, ((Component)this).get_transform().get_position(), ((Component)this).get_transform().get_forward());
				effect.pooledString = "assets/bundled/prefabs/fx/takedamage_hit.prefab";
				EffectNetwork.Send(effect, net.get_connection());
			}
			string text = StringPool.Get(info.HitBone);
			Vector3 val = info.PointEnd - info.PointStart;
			bool flag = ((Vector3.Dot(((Vector3)(ref val)).get_normalized(), eyes.BodyForward()) > 0.4f) ? true : false);
			BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (Object.op_Implicit((Object)(object)initiatorPlayer) && !info.damageTypes.IsMeleeType())
			{
				initiatorPlayer.LifeStoryShotHit(info.Weapon);
			}
			if (info.isHeadshot)
			{
				if (flag)
				{
					SignalBroadcast(Signal.Flinch_RearHead, string.Empty);
				}
				else
				{
					SignalBroadcast(Signal.Flinch_Head, string.Empty);
				}
				Effect.server.Run("assets/bundled/prefabs/fx/headshot.prefab", this, 0u, new Vector3(0f, 2f, 0f), Vector3.get_zero(), ((Object)(object)initiatorPlayer != (Object)null) ? initiatorPlayer.net.get_connection() : null);
				if (Object.op_Implicit((Object)(object)initiatorPlayer))
				{
					initiatorPlayer.stats.Add("headshot", 1, (Stats)5);
					if (initiatorPlayer.IsBeingSpectated)
					{
						foreach (BaseEntity child in initiatorPlayer.children)
						{
							BasePlayer basePlayer;
							if ((basePlayer = child as BasePlayer) != null)
							{
								basePlayer.ClientRPCPlayer(null, basePlayer, "SpectatedPlayerHeadshot");
							}
						}
					}
				}
			}
			else if (flag)
			{
				SignalBroadcast(Signal.Flinch_RearTorso, string.Empty);
			}
			else if (text == "spine" || text == "spine2")
			{
				SignalBroadcast(Signal.Flinch_Stomach, string.Empty);
			}
			else
			{
				SignalBroadcast(Signal.Flinch_Chest, string.Empty);
			}
		}
		if (stats != null)
		{
			if (IsWounded())
			{
				stats.combat.Log(info, health_old, base.health, "wounded");
			}
			else if (IsDead())
			{
				stats.combat.Log(info, health_old, base.health, "killed");
			}
			else
			{
				stats.combat.Log(info, health_old, base.health);
			}
		}
	}

	private void EnablePlayerCollider()
	{
		if (!((Collider)playerCollider).get_enabled())
		{
			RefreshColliderSize(forced: true);
			((Collider)playerCollider).set_enabled(true);
		}
	}

	private void DisablePlayerCollider()
	{
		if (((Collider)playerCollider).get_enabled())
		{
			RemoveFromTriggers();
			((Collider)playerCollider).set_enabled(false);
		}
	}

	private void RefreshColliderSize(bool forced)
	{
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		if (forced || (((Collider)playerCollider).get_enabled() && !(Time.get_time() < nextColliderRefreshTime)))
		{
			nextColliderRefreshTime = Time.get_time() + 0.25f + Random.Range(-0.05f, 0.05f);
			BaseMountable baseMountable = GetMounted();
			CapsuleColliderInfo capsuleColliderInfo = (((Object)(object)baseMountable != (Object)null && baseMountable.IsValid()) ? ((!baseMountable.modifiesPlayerCollider) ? playerColliderStanding : baseMountable.customPlayerCollider) : ((IsIncapacitated() || IsSleeping()) ? playerColliderLyingDown : (IsCrawling() ? playerColliderCrawling : ((!modelState.get_ducked()) ? playerColliderStanding : playerColliderDucked))));
			if (playerCollider.get_height() != capsuleColliderInfo.height || playerCollider.get_radius() != capsuleColliderInfo.radius || playerCollider.get_center() != capsuleColliderInfo.center)
			{
				playerCollider.set_height(capsuleColliderInfo.height);
				playerCollider.set_radius(capsuleColliderInfo.radius);
				playerCollider.set_center(capsuleColliderInfo.center);
			}
		}
	}

	private void SetPlayerRigidbodyState(bool isEnabled)
	{
		if (isEnabled)
		{
			AddPlayerRigidbody();
		}
		else
		{
			RemovePlayerRigidbody();
		}
	}

	private void AddPlayerRigidbody()
	{
		if ((Object)(object)playerRigidbody == (Object)null)
		{
			playerRigidbody = ((Component)this).get_gameObject().GetComponent<Rigidbody>();
		}
		if ((Object)(object)playerRigidbody == (Object)null)
		{
			playerRigidbody = ((Component)this).get_gameObject().AddComponent<Rigidbody>();
			playerRigidbody.set_useGravity(false);
			playerRigidbody.set_isKinematic(true);
			playerRigidbody.set_mass(1f);
			playerRigidbody.set_interpolation((RigidbodyInterpolation)0);
			playerRigidbody.set_collisionDetectionMode((CollisionDetectionMode)0);
		}
	}

	private void RemovePlayerRigidbody()
	{
		if ((Object)(object)playerRigidbody == (Object)null)
		{
			playerRigidbody = ((Component)this).get_gameObject().GetComponent<Rigidbody>();
		}
		if ((Object)(object)playerRigidbody != (Object)null)
		{
			RemoveFromTriggers();
			Object.DestroyImmediate((Object)(object)playerRigidbody);
			playerRigidbody = null;
		}
	}

	public bool IsEnsnared()
	{
		if (triggers == null)
		{
			return false;
		}
		for (int i = 0; i < triggers.Count; i++)
		{
			if (triggers[i] is TriggerEnsnare)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAttacking()
	{
		HeldEntity heldEntity = GetHeldEntity();
		if ((Object)(object)heldEntity == (Object)null)
		{
			return false;
		}
		AttackEntity attackEntity = heldEntity as AttackEntity;
		if ((Object)(object)attackEntity == (Object)null)
		{
			return false;
		}
		return attackEntity.NextAttackTime - Time.get_time() > attackEntity.repeatDelay - 1f;
	}

	public bool CanAttack()
	{
		HeldEntity heldEntity = GetHeldEntity();
		if ((Object)(object)heldEntity == (Object)null)
		{
			return false;
		}
		bool flag = IsSwimming();
		bool flag2 = heldEntity.CanBeUsedInWater();
		if (modelState.get_onLadder())
		{
			return false;
		}
		if (!flag && !modelState.get_onground())
		{
			return false;
		}
		if (flag && !flag2)
		{
			return false;
		}
		if (IsEnsnared())
		{
			return false;
		}
		return true;
	}

	public bool OnLadder()
	{
		if (modelState.get_onLadder())
		{
			return Object.op_Implicit((Object)(object)FindTrigger<TriggerLadder>());
		}
		return false;
	}

	public bool IsSwimming()
	{
		return WaterFactor() >= 0.65f;
	}

	public bool IsHeadUnderwater()
	{
		return WaterFactor() > 0.75f;
	}

	public virtual bool IsOnGround()
	{
		return modelState.get_onground();
	}

	public bool IsRunning()
	{
		if (modelState != null)
		{
			return modelState.get_sprinting();
		}
		return false;
	}

	public bool IsDucked()
	{
		if (modelState != null)
		{
			return modelState.get_ducked();
		}
		return false;
	}

	public void ShowToast(int style, Phrase phrase)
	{
		if (base.isServer)
		{
			SendConsoleCommand("gametip.showtoast_translated", style, phrase.token, phrase.english);
		}
	}

	public void ChatMessage(string msg)
	{
		if (base.isServer)
		{
			SendConsoleCommand("chat.add", 2, 0, msg);
		}
	}

	public void ConsoleMessage(string msg)
	{
		if (base.isServer)
		{
			SendConsoleCommand("echo " + msg);
		}
	}

	public override float PenetrationResistance(HitInfo info)
	{
		return 100f;
	}

	public override void ScaleDamage(HitInfo info)
	{
		if (isMounted)
		{
			GetMounted().ScaleDamageForPlayer(this, info);
		}
		if (info.UseProtection)
		{
			HitArea boneArea = info.boneArea;
			if (boneArea != (HitArea)(-1))
			{
				cachedProtection.Clear();
				cachedProtection.Add(inventory.containerWear.itemList, boneArea);
				cachedProtection.Multiply(DamageType.Arrow, ConVar.Server.arrowarmor);
				cachedProtection.Multiply(DamageType.Bullet, ConVar.Server.bulletarmor);
				cachedProtection.Multiply(DamageType.Slash, ConVar.Server.meleearmor);
				cachedProtection.Multiply(DamageType.Blunt, ConVar.Server.meleearmor);
				cachedProtection.Multiply(DamageType.Stab, ConVar.Server.meleearmor);
				cachedProtection.Multiply(DamageType.Bleeding, ConVar.Server.bleedingarmor);
				cachedProtection.Scale(info.damageTypes);
			}
			else
			{
				baseProtection.Scale(info.damageTypes);
			}
		}
		if (Object.op_Implicit((Object)(object)info.damageProperties))
		{
			info.damageProperties.ScaleDamage(info);
		}
	}

	private void UpdateMoveSpeedFromClothing()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		bool flag = false;
		bool flag2 = false;
		float num4 = 0f;
		eggVision = 0f;
		base.Weight = 0f;
		foreach (Item item in inventory.containerWear.itemList)
		{
			ItemModWearable component = ((Component)item.info).GetComponent<ItemModWearable>();
			if (Object.op_Implicit((Object)(object)component))
			{
				if (component.blocksAiming)
				{
					flag = true;
				}
				if (component.blocksEquipping)
				{
					flag2 = true;
				}
				num4 += component.accuracyBonus;
				eggVision += component.eggVision;
				base.Weight += component.weight;
				if ((Object)(object)component.movementProperties != (Object)null)
				{
					num2 += component.movementProperties.speedReduction;
					num = Mathf.Max(num, component.movementProperties.minSpeedReduction);
					num3 += component.movementProperties.waterSpeedBonus;
				}
			}
		}
		clothingAccuracyBonus = num4;
		clothingMoveSpeedReduction = Mathf.Max(num2, num);
		clothingBlocksAiming = flag;
		clothingWaterSpeedBonus = num3;
		equippingBlocked = flag2;
		if (base.isServer && equippingBlocked)
		{
			UpdateActiveItem(0u);
		}
	}

	public virtual void UpdateProtectionFromClothing()
	{
		baseProtection.Clear();
		baseProtection.Add(inventory.containerWear.itemList);
		float num = 355f / (678f * (float)Math.PI);
		for (int i = 0; i < baseProtection.amounts.Length; i++)
		{
			switch (i)
			{
			case 22:
				baseProtection.amounts[i] = 1f;
				break;
			default:
				baseProtection.amounts[i] *= num;
				break;
			case 17:
				break;
			}
		}
	}

	public override string Categorize()
	{
		return "player";
	}

	public override string ToString()
	{
		if (_name == null)
		{
			if (base.isServer)
			{
				_name = $"{displayName}[{userID}]";
			}
			else
			{
				_name = base.ShortPrefabName;
			}
		}
		return _name;
	}

	public string GetDebugStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("Entity: {0}\n", ((object)this).ToString());
		stringBuilder.AppendFormat("Name: {0}\n", displayName);
		stringBuilder.AppendFormat("SteamID: {0}\n", userID);
		foreach (PlayerFlags value in Enum.GetValues(typeof(PlayerFlags)))
		{
			stringBuilder.AppendFormat("{1}: {0}\n", HasPlayerFlag(value), value);
		}
		return stringBuilder.ToString();
	}

	public override Item GetItem(uint itemId)
	{
		if ((Object)(object)inventory == (Object)null)
		{
			return null;
		}
		return inventory.FindItemUID(itemId);
	}

	public override float WaterFactor()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (GetMounted().IsValid())
		{
			return GetMounted().WaterFactorForPlayer(this);
		}
		if ((Object)(object)GetParentEntity() != (Object)null && GetParentEntity().BlocksWaterFor(this))
		{
			return 0f;
		}
		float radius = playerCollider.get_radius();
		float num = playerCollider.get_height() * 0.5f;
		Vector3 start = ((Component)playerCollider).get_transform().get_position() + ((Component)playerCollider).get_transform().get_rotation() * (playerCollider.get_center() - Vector3.get_up() * (num - radius));
		Vector3 end = ((Component)playerCollider).get_transform().get_position() + ((Component)playerCollider).get_transform().get_rotation() * (playerCollider.get_center() + Vector3.get_up() * (num - radius));
		return WaterLevel.Factor(start, end, radius, this);
	}

	public override float AirFactor()
	{
		float num = ((WaterFactor() > 0.85f) ? 0f : 1f);
		BaseMountable baseMountable = GetMounted();
		if (baseMountable.IsValid() && baseMountable.BlocksWaterFor(this))
		{
			float num2 = baseMountable.AirFactor();
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	public float GetOxygenTime(out ItemModGiveOxygen.AirSupplyType airSupplyType)
	{
		BaseVehicle mountedVehicle = GetMountedVehicle();
		IAirSupply airSupply;
		if (mountedVehicle.IsValid() && (airSupply = mountedVehicle as IAirSupply) != null)
		{
			float airTimeRemaining = airSupply.GetAirTimeRemaining();
			if (airTimeRemaining > 0f)
			{
				airSupplyType = airSupply.AirType;
				return airTimeRemaining;
			}
		}
		foreach (Item item in inventory.containerWear.itemList)
		{
			IAirSupply componentInChildren = ((Component)item.info).GetComponentInChildren<IAirSupply>();
			if (componentInChildren != null)
			{
				float airTimeRemaining2 = componentInChildren.GetAirTimeRemaining();
				if (airTimeRemaining2 > 0f)
				{
					airSupplyType = componentInChildren.AirType;
					return airTimeRemaining2;
				}
			}
		}
		airSupplyType = ItemModGiveOxygen.AirSupplyType.Lungs;
		if (metabolism.oxygen.value > 0.5f)
		{
			float num = Mathf.InverseLerp(0.5f, 1f, metabolism.oxygen.value);
			return 5f * num;
		}
		return 0f;
	}

	public override bool ShouldInheritNetworkGroup()
	{
		return IsSpectating();
	}

	public static bool AnyPlayersVisibleToEntity(Vector3 pos, float radius, BaseEntity source, Vector3 entityEyePos, bool ignorePlayersWithPriv = false)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		List<BasePlayer> list2 = Pool.GetList<BasePlayer>();
		Vis.Entities(pos, radius, list2, 131072, (QueryTriggerInteraction)2);
		bool flag = false;
		foreach (BasePlayer item in list2)
		{
			if (item.IsSleeping() || !item.IsAlive() || (item.IsBuildingAuthed() && ignorePlayersWithPriv))
			{
				continue;
			}
			list.Clear();
			Vector3 position = item.eyes.position;
			Vector3 val = entityEyePos - item.eyes.position;
			GamePhysics.TraceAll(new Ray(position, ((Vector3)(ref val)).get_normalized()), 0f, list, 9f, 1218519297, (QueryTriggerInteraction)0);
			for (int i = 0; i < list.Count; i++)
			{
				BaseEntity entity = list[i].GetEntity();
				if ((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)source || entity.EqualNetID(source)))
				{
					flag = true;
					break;
				}
				if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
				{
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		Pool.FreeList<BasePlayer>(ref list2);
		return flag;
	}

	public bool IsStandingOnEntity(BaseEntity standingOn, int layerMask)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOnGround())
		{
			return false;
		}
		RaycastHit hit = default(RaycastHit);
		if (Physics.SphereCast(((Component)this).get_transform().get_position() + Vector3.get_up() * (0.25f + GetRadius()), GetRadius() * 0.95f, Vector3.get_down(), ref hit, 4f, layerMask))
		{
			BaseEntity entity = hit.GetEntity();
			if ((Object)(object)entity != (Object)null)
			{
				if (entity.EqualNetID(standingOn))
				{
					return true;
				}
				BaseEntity baseEntity = entity.GetParentEntity();
				if ((Object)(object)baseEntity != (Object)null && baseEntity.EqualNetID(standingOn))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SetActiveTelephone(PhoneController t)
	{
		activeTelephone = t;
	}

	public void ClearDesigningAIEntity()
	{
		if (IsDesigningAI)
		{
			((Component)designingAIEntity).GetComponent<IAIDesign>()?.StopDesigning();
		}
		designingAIEntity = null;
	}
}
