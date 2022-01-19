using System.Collections.Generic;
using System.Linq;
using System.Net;
using ConVar;

namespace CompanionServer
{
	public class ConnectionLimiter
	{
		private readonly object _sync;

		private readonly Dictionary<IPAddress, int> _addressCounts;

		private int _overallCount;

		public ConnectionLimiter()
		{
			_sync = new object();
			_addressCounts = new Dictionary<IPAddress, int>();
			_overallCount = 0;
		}

		public bool TryAdd(IPAddress address)
		{
			if (address == null)
			{
				return false;
			}
			lock (_sync)
			{
				if (_overallCount >= App.maxconnections)
				{
					return false;
				}
				if (_addressCounts.TryGetValue(address, out var value))
				{
					if (value >= App.maxconnectionsperip)
					{
						return false;
					}
					_addressCounts[address] = value + 1;
				}
				else
				{
					_addressCounts.Add(address, 1);
				}
				_overallCount++;
				return true;
			}
		}

		public void Remove(IPAddress address)
		{
			if (address == null)
			{
				return;
			}
			lock (_sync)
			{
				if (_addressCounts.TryGetValue(address, out var value))
				{
					if (value <= 1)
					{
						_addressCounts.Remove(address);
					}
					else
					{
						_addressCounts[address] = value - 1;
					}
					_overallCount--;
				}
			}
		}

		public void Clear()
		{
			lock (_sync)
			{
				_addressCounts.Clear();
				_overallCount = 0;
			}
		}

		public override string ToString()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			TextTable val = new TextTable();
			val.AddColumns(new string[2] { "IP", "connections" });
			lock (_sync)
			{
				foreach (KeyValuePair<IPAddress, int> item in _addressCounts.OrderByDescending((KeyValuePair<IPAddress, int> t) => t.Value))
				{
					val.AddRow(new string[2]
					{
						item.Key.ToString(),
						item.Value.ToString()
					});
				}
				return $"{val}\n{_overallCount} total";
			}
		}
	}
}
