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
				return TransformUtil.GetRootObjects().ToArray();
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
			foreach (Transform item in (from x in GetCurrent()
				where string.IsNullOrEmpty(filter) || ((Object)x).get_name().Contains(filter)
				select x).Take(40))
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
			Transform val = GetCurrent().FirstOrDefault((Transform x) => ((Object)x).get_name().ToLower() == args.FullString.ToLower());
			if ((Object)(object)val == (Object)null)
			{
				val = GetCurrent().FirstOrDefault((Transform x) => ((Object)x).get_name().StartsWith(args.FullString, StringComparison.CurrentCultureIgnoreCase));
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
			IEnumerable<Transform> enumerable = from x in GetCurrent()
				where ((Object)x).get_name().ToLower() == args.FullString.ToLower()
				select x;
			if (enumerable.Count() == 0)
			{
				enumerable = from x in GetCurrent()
					where ((Object)x).get_name().StartsWith(args.FullString, StringComparison.CurrentCultureIgnoreCase)
					select x;
			}
			if (enumerable.Count() == 0)
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
			args.ReplyWith("Deleted " + enumerable.Count() + " objects");
		}

		public Hierarchy()
			: this()
		{
		}
	}
}
