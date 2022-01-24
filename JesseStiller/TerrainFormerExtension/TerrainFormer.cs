using UnityEngine;

namespace JesseStiller.TerrainFormerExtension
{
	public class TerrainFormer : MonoBehaviour
	{
		private void Awake()
		{
			Object.Destroy((Object)(object)this);
		}

		public TerrainFormer()
			: this()
		{
		}
	}
}
