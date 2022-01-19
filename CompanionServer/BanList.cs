using System;
using System.Collections.Generic;
using Facepunch;
using Network;

namespace CompanionServer
{
	public class BanList<TKey>
	{
		private readonly Dictionary<TKey, double> _bans;

		public BanList()
		{
			_bans = new Dictionary<TKey, double>();
		}

		public void Ban(TKey key, double timeInSeconds)
		{
			lock (_bans)
			{
				double num = TimeEx.get_realtimeSinceStartup() + timeInSeconds;
				if (_bans.TryGetValue(key, out var value))
				{
					num = Math.Max(num, value);
				}
				_bans[key] = num;
			}
		}

		public bool IsBanned(TKey key)
		{
			lock (_bans)
			{
				if (!_bans.TryGetValue(key, out var value))
				{
					return false;
				}
				if (TimeEx.get_realtimeSinceStartup() < value)
				{
					return true;
				}
				_bans.Remove(key);
				return false;
			}
		}

		public void Cleanup()
		{
			double realtimeSinceStartup = TimeEx.get_realtimeSinceStartup();
			List<TKey> list = Pool.GetList<TKey>();
			lock (_bans)
			{
				foreach (KeyValuePair<TKey, double> ban in _bans)
				{
					if (realtimeSinceStartup >= ban.Value)
					{
						list.Add(ban.Key);
					}
				}
				foreach (TKey item in list)
				{
					_bans.Remove(item);
				}
			}
			Pool.FreeList<TKey>(ref list);
		}
	}
}
