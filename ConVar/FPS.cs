using UnityEngine;

namespace ConVar
{
	[Factory("fps")]
	public class FPS : ConsoleSystem
	{
		private static int m_graph;

		[ClientVar(Saved = true)]
		[ServerVar(Saved = true)]
		public static int limit
		{
			get
			{
				return Application.get_targetFrameRate();
			}
			set
			{
				Application.set_targetFrameRate(value);
			}
		}

		[ClientVar]
		public static int graph
		{
			get
			{
				return m_graph;
			}
			set
			{
				m_graph = value;
				if (Object.op_Implicit((Object)(object)MainCamera.mainCamera))
				{
					FPSGraph component = ((Component)MainCamera.mainCamera).GetComponent<FPSGraph>();
					if (Object.op_Implicit((Object)(object)component))
					{
						component.Refresh();
					}
				}
			}
		}

		public FPS()
			: this()
		{
		}
	}
}
