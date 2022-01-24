using UnityEngine.SceneManagement;

namespace Rust
{
	public static class Generic
	{
		private static Scene _batchingScene;

		public static Scene BatchingScene
		{
			get
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				if (!((Scene)(ref _batchingScene)).IsValid())
				{
					_batchingScene = SceneManager.CreateScene("Batching");
				}
				return _batchingScene;
			}
		}
	}
}
