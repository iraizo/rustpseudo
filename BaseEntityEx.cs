using UnityEngine;

public static class BaseEntityEx
{
	public static bool IsValid(this BaseEntity ent)
	{
		if ((Object)(object)ent == (Object)null)
		{
			return false;
		}
		if (ent.net == null)
		{
			return false;
		}
		return true;
	}

	public static bool IsRealNull(this BaseEntity ent)
	{
		return ent == null;
	}

	public static bool IsValidEntityReference<T>(this T obj) where T : class
	{
		return (Object)(object)(obj as BaseEntity) != (Object)null;
	}

	public static bool HasEntityInParents(this BaseEntity ent, BaseEntity toFind)
	{
		if ((Object)(object)ent == (Object)null || (Object)(object)toFind == (Object)null)
		{
			return false;
		}
		if ((Object)(object)ent == (Object)(object)toFind || ent.EqualNetID(toFind))
		{
			return true;
		}
		BaseEntity parentEntity = ent.GetParentEntity();
		while ((Object)(object)parentEntity != (Object)null)
		{
			if ((Object)(object)parentEntity == (Object)(object)toFind || parentEntity.EqualNetID(toFind))
			{
				return true;
			}
			parentEntity = parentEntity.GetParentEntity();
		}
		return false;
	}
}
