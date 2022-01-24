using System.Collections.Generic;
using Facepunch;

namespace CompanionServer
{
	public class PushRequest : IPooled
	{
		public string ServerToken;

		public List<ulong> SteamIds;

		public NotificationChannel Channel;

		public string Title;

		public string Body;

		public Dictionary<string, string> Data;

		public void EnterPool()
		{
			Pool.FreeList<ulong>(ref SteamIds);
			Channel = (NotificationChannel)0;
			Title = null;
			Body = null;
			if (Data != null)
			{
				Data.Clear();
				Pool.Free<Dictionary<string, string>>(ref Data);
			}
		}

		public void LeavePool()
		{
		}
	}
}
