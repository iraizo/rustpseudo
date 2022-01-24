using System;
using System.IO;
using UnityEngine.Profiling.Memory.Experimental;

namespace ConVar
{
	[Factory("memsnap")]
	public class MemSnap : ConsoleSystem
	{
		private static string NeedProfileFolder()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			string text = "profile";
			if (!Directory.Exists(text))
			{
				return ((FileSystemInfo)Directory.CreateDirectory(text)).get_FullName();
			}
			return ((FileSystemInfo)new DirectoryInfo(text)).get_FullName();
		}

		[ClientVar]
		[ServerVar]
		public static void managed(Arg arg)
		{
			MemoryProfiler.TakeSnapshot(NeedProfileFolder() + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap", (Action<string, bool>)null, (CaptureFlags)1);
		}

		[ClientVar]
		[ServerVar]
		public static void native(Arg arg)
		{
			MemoryProfiler.TakeSnapshot(NeedProfileFolder() + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap", (Action<string, bool>)null, (CaptureFlags)2);
		}

		[ClientVar]
		[ServerVar]
		public static void full(Arg arg)
		{
			MemoryProfiler.TakeSnapshot(NeedProfileFolder() + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap", (Action<string, bool>)null, (CaptureFlags)31);
		}

		public MemSnap()
			: this()
		{
		}
	}
}
