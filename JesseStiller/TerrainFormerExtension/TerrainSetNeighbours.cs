using UnityEngine;

namespace JesseStiller.TerrainFormerExtension
{
	public class TerrainSetNeighbours : MonoBehaviour
	{
		[SerializeField]
		private Terrain leftTerrain;

		[SerializeField]
		private Terrain topTerrain;

		[SerializeField]
		private Terrain rightTerrain;

		[SerializeField]
		private Terrain bottomTerrain;

		private void Awake()
		{
			((Component)this).GetComponent<Terrain>().SetNeighbors(leftTerrain, topTerrain, rightTerrain, bottomTerrain);
			Object.Destroy((Object)(object)this);
		}

		public void SetNeighbours(Terrain leftTerrain, Terrain topTerrain, Terrain rightTerrain, Terrain bottomTerrain)
		{
			this.leftTerrain = leftTerrain;
			this.topTerrain = topTerrain;
			this.rightTerrain = rightTerrain;
			this.bottomTerrain = bottomTerrain;
		}

		public TerrainSetNeighbours()
			: this()
		{
		}
	}
}
