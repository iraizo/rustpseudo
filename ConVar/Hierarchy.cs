using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConVar
{
	[Factory("hierarchy")]
	public class Hierarchy : ConsoleSystem
	{
		private static GameObject currentDir;

		private static Transform[] GetCurrent()
		{
			if ((Object)(object)currentDir == (Object)null)
			{
				return Enumerable.ToArray<Transform>((IEnumerable<Transform>)TransformUtil.GetRootObjects());
			}
			List<Transform> list = new List<Transform>();
			for (int i = 0; i < currentDir.get_transform().get_childCount(); i++)
			{
				list.Add(currentDir.get_transform().GetChild(i));
			}
			return list.ToArray();
		}

		[ServerVar]
		public static void ls(Arg args)
		{
			string text = "";
			string filter = args.GetString(0, "");
			text = ((!Object.op_Implicit((Object)(object)currentDir)) ? (text + "Listing .\n\n") : (text + "Listing " + currentDir.get_transform().GetRecursiveName() + "\n\n"));
			foreach (Transform item in Enumerable.Take<Transform>(Enumerable.Where<Transform>((IEnumerable<Transform>)GetCurrent(), (Func<Transform, bool>)((Transform x) => string.IsNullOrEmpty(filter) || ((Object)x).get_name().Contains(filter))), 40))
			{
				text += $"   {((Object)item).get_name()} [{item.get_childCount()}]\n";
			}
			text += "\n";
			args.ReplyWith(text);
		}

		[ServerVar]
		public static void cd(Arg args)
		{
			if (args.FullString == ".")
			{
				currentDir = null;
				args.ReplyWith("Changed to .");
				return;
			}
			if (args.FullString == "..")
			{
				if (Object.op_Implicit((Object)(object)currentDir))
				{
					currentDir = (Object.op_Implicit((Object)(object)currentDir.get_transform().get_parent()) ? ((Component)currentDir.get_transform().get_parent()).get_gameObject() : null);
				}
				currentDir = null;
				if (Object.op_Implicit((Object)(object)currentDir))
				{
					args.ReplyWith("Changed to " + currentDir.get_transform().GetRecursiveName());
				}
				else
				{
					args.ReplyWith("Changed to .");
				}
				return;
			}
			Transform val = Enumerable.FirstOrDefault<Transform>((IEnumerable<Transform>)GetCurrent(), (Func<Transform, bool>)((Transform x) => ((Object)x).get_name().ToLower() == args.FullString.ToLower()));
			if ((Object)(object)val == (Object)null)
			{
				val = Enumerable.FirstOrDefault<Transform>((IEnumerable<Transform>)GetCurrent(), (Func<Transform, bool>)((Transform x) => ((Object)x).get_name().StartsWith(args.FullString, StringComparison.CurrentCultureIgnoreCase)));
			}
			if (Object.op_Implicit((Object)(object)val))
			{
				currentDir = ((Component)val).get_gameObject();
				args.ReplyWith("Changed to " + currentDir.get_transform().GetRecursiveName());
			}
			else
			{
				args.ReplyWith("Couldn't find \"" + args.FullString + "\"");
			}
		}

		[ServerVar]
		public static void del(Arg args)
		{
			if (!args.HasArgs(1))
			{
				return;
			}
			IEnumerable<Transform> enumerable = Enumerable.Where<Transform>((IEnumerable<Transform>)GetCurrent(), (Func<Transform, bool>)((Transform x) => ((Object)x).get_name().ToLower() == args.FullString.ToLower()));
			if (Enumerable.Count<Transform>(enumerable) == 0)
			{
				enumerable = Enumerable.Where<Transform>((IEnumerable<Transform>)GetCurrent(), (Func<Transform, bool>)((Transform x) => ((Object)x).get_name().StartsWith(args.FullString, StringComparison.CurrentCultureIgnoreCase)));
			}
			if (Enumerable.Count<Transform>(enumerable) == 0)
			{
				args.ReplyWith("Couldn't find  " + args.FullString);
				return;
			}
			foreach (Transform item in enumerable)
			{
				BaseEntity baseEntity = ((Component)item).get_gameObject().ToBaseEntity();
				if (baseEntity.IsValid())
				{
					if (baseEntity.isServer)
					{
						baseEntity.Kill();
					}
				}
				else
				{
					GameManager.Destroy(((Component)item).get_gameObject());
				}
			}
			args.ReplyWith("Deleted " + Enumerable.Count<Transform>(enumerable) + " objects");
		}

		public Hierarchy()
			: this()
		{
		}
	}
}
