using System;
using UnityEngine;

namespace Rust.Ai
{
	[DefaultExecutionOrder(-102)]
	public class AiManagedAgent : FacepunchBehaviour, IServerComponent
	{
		[Tooltip("TODO: Replace with actual agent type id on the NavMeshAgent when we upgrade to 5.6.1 or above.")]
		public int AgentTypeIndex;

		[NonSerialized]
		[ReadOnly]
		public Vector2i NavmeshGridCoord;

		private bool isRegistered;

		private void OnEnable()
		{
			isRegistered = false;
			if ((Object)(object)SingletonComponent<AiManager>.Instance == (Object)null || !((Behaviour)SingletonComponent<AiManager>.Instance).get_enabled() || AiManager.nav_disable)
			{
				((Behaviour)this).set_enabled(false);
			}
		}

		private void DelayedRegistration()
		{
			if (!isRegistered)
			{
				isRegistered = true;
			}
		}

		private void OnDisable()
		{
			if (!Application.isQuitting && !((Object)(object)SingletonComponent<AiManager>.Instance == (Object)null) && ((Behaviour)SingletonComponent<AiManager>.Instance).get_enabled())
			{
				_ = isRegistered;
			}
		}

		public AiManagedAgent()
			: this()
		{
		}
	}
}
