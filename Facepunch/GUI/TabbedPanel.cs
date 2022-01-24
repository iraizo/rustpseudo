using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facepunch.GUI
{
	internal class TabbedPanel
	{
		public struct Tab
		{
			public string name;

			public Action drawFunc;
		}

		private int selectedTabID;

		private List<Tab> tabs = new List<Tab>();

		public Tab selectedTab => tabs[selectedTabID];

		public void Add(Tab tab)
		{
			tabs.Add(tab);
		}

		internal void DrawVertical(float width)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Expected O, but got Unknown
			GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.Width(width),
				GUILayout.ExpandHeight(true)
			});
			for (int i = 0; i < tabs.Count; i++)
			{
				if (GUILayout.Toggle(selectedTabID == i, tabs[i].name, new GUIStyle(GUIStyle.op_Implicit("devtab")), Array.Empty<GUILayoutOption>()))
				{
					selectedTabID = i;
				}
			}
			if (GUILayout.Toggle(false, "", new GUIStyle(GUIStyle.op_Implicit("devtab")), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandHeight(true) }))
			{
				selectedTabID = -1;
			}
			GUILayout.EndVertical();
		}

		internal void DrawContents()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			if (selectedTabID >= 0)
			{
				Tab tab = selectedTab;
				GUILayout.BeginVertical(new GUIStyle(GUIStyle.op_Implicit("devtabcontents")), (GUILayoutOption[])(object)new GUILayoutOption[2]
				{
					GUILayout.ExpandHeight(true),
					GUILayout.ExpandWidth(true)
				});
				if (tab.drawFunc != null)
				{
					tab.drawFunc();
				}
				GUILayout.EndVertical();
			}
		}
	}
}
