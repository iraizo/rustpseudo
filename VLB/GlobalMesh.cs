using UnityEngine;

namespace VLB
{
	public static class GlobalMesh
	{
		private static Mesh ms_Mesh;

		public static Mesh mesh
		{
			get
			{
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)ms_Mesh == (Object)null)
				{
					ms_Mesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, cap: true);
					((Object)ms_Mesh).set_hideFlags(Consts.ProceduralObjectsHideFlags);
				}
				return ms_Mesh;
			}
		}
	}
}
