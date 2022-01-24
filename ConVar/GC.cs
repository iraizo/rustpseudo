using Rust;
using UnityEngine;
using UnityEngine.Scripting;

namespace ConVar
{
	[Factory("gc")]
	public class GC : ConsoleSystem
	{
		[ClientVar]
		public static bool buffer_enabled = true;

		[ClientVar]
		public static int debuglevel = 1;

		private static int m_buffer = 256;

		[ClientVar]
		public static int buffer
		{
			get
			{
				return m_buffer;
			}
			set
			{
				m_buffer = Mathf.Clamp(value, 64, 4096);
			}
		}

		[ServerVar]
		[ClientVar]
		public static bool incremental_enabled
		{
			get
			{
				return GarbageCollector.get_isIncremental();
			}
			set
			{
				Debug.LogWarning((object)"Cannot set gc.incremental as it is read only");
			}
		}

		[ServerVar]
		[ClientVar]
		public static int incremental_milliseconds
		{
			get
			{
				return (int)(GarbageCollector.get_incrementalTimeSliceNanoseconds() / 1000000uL);
			}
			set
			{
				GarbageCollector.set_incrementalTimeSliceNanoseconds(1000000uL * (ulong)Mathf.Max(value, 0));
			}
		}

		[ServerVar]
		[ClientVar]
		public static bool enabled
		{
			get
			{
				return Rust.GC.Enabled;
			}
			set
			{
				Debug.LogWarning((object)"Cannot set gc.enabled as it is read only");
			}
		}

		[ServerVar]
		[ClientVar]
		public static void collect()
		{
			Rust.GC.Collect();
		}

		[ServerVar]
		[ClientVar]
		public static void unload()
		{
			Resources.UnloadUnusedAssets();
		}

		[ServerVar]
		[ClientVar]
		public static void alloc(Arg args)
		{
			byte[] array = new byte[args.GetInt(0, 1048576)];
			args.ReplyWith("Allocated " + array.Length + " bytes");
		}

		public GC()
			: this()
		{
		}
	}
}
