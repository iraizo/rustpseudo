using UnityEngine;

public static class EAC
{
	private static bool IsEacEnabled => !Application.get_isEditor();
}
