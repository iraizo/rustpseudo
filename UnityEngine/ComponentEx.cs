using Facepunch;

namespace UnityEngine
{
	public static class ComponentEx
	{
		public static T Instantiate<T>(this T component) where T : Component
		{
			return Instantiate.GameObject(((Component)component).get_gameObject(), (Transform)null).GetComponent<T>();
		}

		public static bool HasComponent<T>(this Component component) where T : Component
		{
			return (Object)(object)component.GetComponent<T>() != (Object)null;
		}
	}
}
