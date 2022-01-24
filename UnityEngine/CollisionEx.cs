namespace UnityEngine
{
	public static class CollisionEx
	{
		public static BaseEntity GetEntity(this Collision col)
		{
			return col.get_transform().ToBaseEntity();
		}
	}
}
