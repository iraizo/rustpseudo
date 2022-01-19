namespace UnityEngine
{
	public static class ArgEx
	{
		public static BasePlayer Player(this Arg arg)
		{
			if (arg == null || arg.get_Connection() == null)
			{
				return null;
			}
			return arg.get_Connection().player as BasePlayer;
		}

		public static BasePlayer GetPlayer(this Arg arg, int iArgNum)
		{
			string @string = arg.GetString(iArgNum, "");
			if (@string == null)
			{
				return null;
			}
			return BasePlayer.Find(@string);
		}

		public static BasePlayer GetSleeper(this Arg arg, int iArgNum)
		{
			string @string = arg.GetString(iArgNum, "");
			if (@string == null)
			{
				return null;
			}
			return BasePlayer.FindSleeping(@string);
		}

		public static BasePlayer GetPlayerOrSleeper(this Arg arg, int iArgNum)
		{
			string @string = arg.GetString(iArgNum, "");
			if (@string == null)
			{
				return null;
			}
			return BasePlayer.FindAwakeOrSleeping(@string);
		}

		public static BasePlayer GetPlayerOrSleeperOrBot(this Arg arg, int iArgNum)
		{
			uint num = default(uint);
			if (arg.TryGetUInt(iArgNum, ref num))
			{
				return BasePlayer.FindBot(num);
			}
			return arg.GetPlayerOrSleeper(iArgNum);
		}
	}
}
