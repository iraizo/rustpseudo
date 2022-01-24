using System.Collections.Generic;
using UnityEngine;

public static class AssetNameCache
{
	private static Dictionary<Object, string> mixed = new Dictionary<Object, string>();

	private static Dictionary<Object, string> lower = new Dictionary<Object, string>();

	private static Dictionary<Object, string> upper = new Dictionary<Object, string>();

	private static string LookupName(Object obj)
	{
		if (obj == (Object)null)
		{
			return string.Empty;
		}
		if (!mixed.TryGetValue(obj, out var value))
		{
			value = obj.get_name();
			mixed.Add(obj, value);
		}
		return value;
	}

	private static string LookupNameLower(Object obj)
	{
		if (obj == (Object)null)
		{
			return string.Empty;
		}
		if (!lower.TryGetValue(obj, out var value))
		{
			value = obj.get_name().ToLower();
			lower.Add(obj, value);
		}
		return value;
	}

	private static string LookupNameUpper(Object obj)
	{
		if (obj == (Object)null)
		{
			return string.Empty;
		}
		if (!upper.TryGetValue(obj, out var value))
		{
			value = obj.get_name().ToUpper();
			upper.Add(obj, value);
		}
		return value;
	}

	public static string GetName(this PhysicMaterial mat)
	{
		return LookupName((Object)(object)mat);
	}

	public static string GetNameLower(this PhysicMaterial mat)
	{
		return LookupNameLower((Object)(object)mat);
	}

	public static string GetNameUpper(this PhysicMaterial mat)
	{
		return LookupNameUpper((Object)(object)mat);
	}

	public static string GetName(this Material mat)
	{
		return LookupName((Object)(object)mat);
	}

	public static string GetNameLower(this Material mat)
	{
		return LookupNameLower((Object)(object)mat);
	}

	public static string GetNameUpper(this Material mat)
	{
		return LookupNameUpper((Object)(object)mat);
	}
}
