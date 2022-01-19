using System;
using UnityEngine;

namespace Facepunch.GUI
{
	public static class Controls
	{
		public static float labelWidth = 100f;

		public static float FloatSlider(string strLabel, float value, float low, float high, string format = "0.00")
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strLabel, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(labelWidth) });
			float num = float.Parse(GUILayout.TextField(value.ToString(format), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandWidth(true) }));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			float result = GUILayout.HorizontalSlider(num, low, high, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		public static int IntSlider(string strLabel, int value, int low, int high, string format = "0")
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strLabel, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(labelWidth) });
			int num = int.Parse(GUILayout.TextField(value.ToString(format), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandWidth(true) }));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			int result = (int)GUILayout.HorizontalSlider((float)num, (float)low, (float)high, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		public static string TextArea(string strName, string value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strName, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(labelWidth) });
			string result = GUILayout.TextArea(value, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		public static bool Checkbox(string strName, bool value)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(strName, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(labelWidth) });
			bool result = GUILayout.Toggle(value, "", Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}

		public static bool Button(string strName)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			bool result = GUILayout.Button(strName, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			return result;
		}
	}
}
