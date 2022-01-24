using System;
using System.Collections.Generic;
using Facepunch;

namespace CompanionServer
{
	public class SubscriberList<TKey, TTarget, TMessage> where TKey : IEquatable<TKey> where TTarget : class
	{
		private readonly object _syncRoot;

		private readonly Dictionary<TKey, HashSet<TTarget>> _subscriptions;

		private readonly IBroadcastSender<TTarget, TMessage> _sender;

		public SubscriberList(IBroadcastSender<TTarget, TMessage> sender)
		{
			_syncRoot = new object();
			_subscriptions = new Dictionary<TKey, HashSet<TTarget>>();
			_sender = sender;
		}

		public void Add(TKey key, TTarget value)
		{
			lock (_syncRoot)
			{
				if (_subscriptions.TryGetValue(key, out var value2))
				{
					((HashSet<_003F>)(object)value2).Add(value);
					return;
				}
				HashSet<_003F> obj = new HashSet<_003F>();
				obj.Add(value);
				value2 = (HashSet<TTarget>)(object)obj;
				_subscriptions.Add(key, value2);
			}
		}

		public void Remove(TKey key, TTarget value)
		{
			lock (_syncRoot)
			{
				if (_subscriptions.TryGetValue(key, out var value2))
				{
					((HashSet<_003F>)(object)value2).Remove(value);
					if (((HashSet<_003F>)(object)value2).get_Count() == 0)
					{
						_subscriptions.Remove(key);
					}
				}
			}
		}

		public void Clear(TKey key)
		{
			lock (_syncRoot)
			{
				if (_subscriptions.TryGetValue(key, out var value))
				{
					((HashSet<_003F>)(object)value).Clear();
				}
			}
		}

		public unsafe void Send(TKey key, TMessage message)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			List<TTarget> list;
			lock (_syncRoot)
			{
				if (!_subscriptions.TryGetValue(key, out var value))
				{
					return;
				}
				list = Pool.GetList<TTarget>();
				Enumerator<TTarget> enumerator = ((HashSet<_003F>)(object)value).GetEnumerator();
				try
				{
					while (((Enumerator<_003F>*)(&enumerator))->MoveNext())
					{
						TTarget current = ((Enumerator<_003F>*)(&enumerator))->get_Current();
						list.Add(current);
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
			}
			_sender.BroadcastTo(list, message);
			Pool.FreeList<TTarget>(ref list);
		}
	}
}
