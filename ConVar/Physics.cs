using UnityEngine;

namespace ConVar
{
	[Factory("physics")]
	public class Physics : ConsoleSystem
	{
		private const float baseGravity = -9.81f;

		[ServerVar(Help = "The collision detection mode that dropped items and corpses should use")]
		public static int droppedmode = 2;

		[ServerVar(Help = "Send effects to clients when physics objects collide")]
		public static bool sendeffects = true;

		[ServerVar]
		public static bool groundwatchdebug = false;

		[ServerVar]
		public static int groundwatchfails = 1;

		[ServerVar]
		public static float groundwatchdelay = 0.1f;

		[ClientVar]
		[ServerVar]
		public static bool batchsynctransforms = true;

		[ServerVar]
		public static float bouncethreshold
		{
			get
			{
				return Physics.get_bounceThreshold();
			}
			set
			{
				Physics.set_bounceThreshold(value);
			}
		}

		[ServerVar]
		public static float sleepthreshold
		{
			get
			{
				return Physics.get_sleepThreshold();
			}
			set
			{
				Physics.set_sleepThreshold(value);
			}
		}

		[ServerVar(Help = "The default solver iteration count permitted for any rigid bodies (default 7). Must be positive")]
		public static int solveriterationcount
		{
			get
			{
				return Physics.get_defaultSolverIterations();
			}
			set
			{
				Physics.set_defaultSolverIterations(value);
			}
		}

		[ServerVar(Help = "Gravity multiplier")]
		public static float gravity
		{
			get
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				return Physics.get_gravity().y / -9.81f;
			}
			set
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				Physics.set_gravity(new Vector3(0f, value * -9.81f, 0f));
			}
		}

		[ClientVar(ClientAdmin = true)]
		[ServerVar(Help = "The amount of physics steps per second")]
		public static float steps
		{
			get
			{
				return 1f / Time.get_fixedDeltaTime();
			}
			set
			{
				if (value < 10f)
				{
					value = 10f;
				}
				if (value > 60f)
				{
					value = 60f;
				}
				Time.set_fixedDeltaTime(1f / value);
			}
		}

		[ClientVar(ClientAdmin = true)]
		[ServerVar(Help = "The slowest physics steps will operate")]
		public static float minsteps
		{
			get
			{
				return 1f / Time.get_maximumDeltaTime();
			}
			set
			{
				if (value < 1f)
				{
					value = 1f;
				}
				if (value > 60f)
				{
					value = 60f;
				}
				Time.set_maximumDeltaTime(1f / value);
			}
		}

		[ClientVar]
		[ServerVar]
		public static bool autosynctransforms
		{
			get
			{
				return Physics.get_autoSyncTransforms();
			}
			set
			{
				Physics.set_autoSyncTransforms(value);
			}
		}

		internal static void ApplyDropped(Rigidbody rigidBody)
		{
			if (droppedmode <= 0)
			{
				rigidBody.set_collisionDetectionMode((CollisionDetectionMode)0);
			}
			if (droppedmode == 1)
			{
				rigidBody.set_collisionDetectionMode((CollisionDetectionMode)1);
			}
			if (droppedmode == 2)
			{
				rigidBody.set_collisionDetectionMode((CollisionDetectionMode)2);
			}
			if (droppedmode >= 3)
			{
				rigidBody.set_collisionDetectionMode((CollisionDetectionMode)3);
			}
		}

		public Physics()
			: this()
		{
		}
	}
}
