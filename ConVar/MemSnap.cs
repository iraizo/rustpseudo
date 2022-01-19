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
			string path = "profile";
			if (!Directory.Exists(path))
			{
				return Directory.CreateDirectory(path).FullName;
			}
			return new DirectoryInfo(path).FullName;
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
