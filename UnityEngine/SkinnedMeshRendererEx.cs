namespace UnityEngine
{
	public static class SkinnedMeshRendererEx
	{
		public static Transform FindRig(this SkinnedMeshRenderer renderer)
		{
			Transform parent = ((Component)renderer).get_transform().get_parent();
			Transform val = renderer.get_rootBone();
			while ((Object)(object)val != (Object)null && (Object)(object)val.get_parent() != (Object)null && (Object)(object)val.get_parent() != (Object)(object)parent)
			{
				val = val.get_parent();
			}
			return val;
		}
	}
}
