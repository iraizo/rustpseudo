using System.IO;
using Network;
using ProtoBuf;
using UnityEngine;

namespace ConVar
{
	[Factory("demo")]
	public class Demo : ConsoleSystem
	{
		public class Header : DemoHeader, IDemoHeader
		{
			long Length
			{
				get
				{
					return base.length;
				}
				set
				{
					base.length = value;
				}
			}

			public void Write(BinaryWriter writer)
			{
				byte[] array = ((DemoHeader)this).ToProtoBytes();
				writer.Write("RUST DEMO FORMAT");
				writer.Write(array.Length);
				writer.Write(array);
				writer.Write('\0');
			}

			public Header()
				: this()
			{
			}
		}

		public static uint Version = 3u;

		[ServerVar]
		public static float splitseconds = 3600f;

		[ServerVar]
		public static float splitmegabytes = 200f;

		[ServerVar(Saved = true)]
		public static string recordlist = "";

		private static int _recordListModeValue = 0;

		[ServerVar(Saved = true, Help = "Controls the behavior of recordlist, 0=whitelist, 1=blacklist")]
		public static int recordlistmode
		{
			get
			{
				return _recordListModeValue;
			}
			set
			{
				_recordListModeValue = Mathf.Clamp(value, 0, 1);
			}
		}

		[ServerVar]
		public static string record(Arg arg)
		{
			BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!Object.op_Implicit((Object)(object)playerOrSleeper) || playerOrSleeper.net == null || playerOrSleeper.net.get_connection() == null)
			{
				return "Player not found";
			}
			if (playerOrSleeper.net.get_connection().get_IsRecording())
			{
				return "Player already recording a demo";
			}
			playerOrSleeper.StartDemoRecording();
			return null;
		}

		[ServerVar]
		public static string stop(Arg arg)
		{
			BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
			if (!Object.op_Implicit((Object)(object)playerOrSleeper) || playerOrSleeper.net == null || playerOrSleeper.net.get_connection() == null)
			{
				return "Player not found";
			}
			if (!playerOrSleeper.net.get_connection().get_IsRecording())
			{
				return "Player not recording a demo";
			}
			playerOrSleeper.StopDemoRecording();
			return null;
		}

		public Demo()
			: this()
		{
		}
	}
}
