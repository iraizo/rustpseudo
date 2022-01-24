using System;
using System.IO;
using System.Threading;

public static class FileEx
{
	public static void Backup(DirectoryInfo parent, params string[] names)
	{
		for (int i = 0; i < names.Length; i++)
		{
			names[i] = Path.Combine(((FileSystemInfo)parent).get_FullName(), names[i]);
		}
		Backup(names);
	}

	public static bool MoveToSafe(this FileInfo parent, string target, int retries = 10)
	{
		for (int i = 0; i < retries; i++)
		{
			try
			{
				parent.MoveTo(target);
			}
			catch (Exception)
			{
				Thread.Sleep(5);
				continue;
			}
			return true;
		}
		return false;
	}

	public static void Backup(params string[] names)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		for (int num = names.Length - 2; num >= 0; num--)
		{
			FileInfo val = new FileInfo(names[num]);
			FileInfo val2 = new FileInfo(names[num + 1]);
			if (((FileSystemInfo)val).get_Exists())
			{
				if (((FileSystemInfo)val2).get_Exists())
				{
					double totalHours = (DateTime.Now - ((FileSystemInfo)val2).get_LastWriteTime()).TotalHours;
					int num2 = ((num != 0) ? (1 << num - 1) : 0);
					if (totalHours >= (double)num2)
					{
						((FileSystemInfo)val2).Delete();
						val.MoveToSafe(((FileSystemInfo)val2).get_FullName());
					}
				}
				else
				{
					if (!((FileSystemInfo)val2.get_Directory()).get_Exists())
					{
						val2.get_Directory().Create();
					}
					val.MoveToSafe(((FileSystemInfo)val2).get_FullName());
				}
			}
		}
	}
}
