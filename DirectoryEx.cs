using System;
using System.IO;
using System.Threading;

public static class DirectoryEx
{
	public static void Backup(DirectoryInfo parent, params string[] names)
	{
		for (int i = 0; i < names.Length; i++)
		{
			names[i] = Path.Combine(((FileSystemInfo)parent).get_FullName(), names[i]);
		}
		Backup(names);
	}

	public static bool MoveToSafe(this DirectoryInfo parent, string target, int retries = 10)
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
			DirectoryInfo val = new DirectoryInfo(names[num]);
			DirectoryInfo val2 = new DirectoryInfo(names[num + 1]);
			if (((FileSystemInfo)val).get_Exists())
			{
				if (((FileSystemInfo)val2).get_Exists())
				{
					double totalHours = (DateTime.Now - ((FileSystemInfo)val2).get_LastWriteTime()).TotalHours;
					int num2 = ((num != 0) ? (1 << num - 1) : 0);
					if (totalHours >= (double)num2)
					{
						val2.Delete(true);
						val.MoveToSafe(((FileSystemInfo)val2).get_FullName());
					}
				}
				else
				{
					if (!((FileSystemInfo)val2.get_Parent()).get_Exists())
					{
						val2.get_Parent().Create();
					}
					val.MoveToSafe(((FileSystemInfo)val2).get_FullName());
				}
			}
		}
	}

	public static void CopyAll(string sourceDirectory, string targetDirectory)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		//IL_0013: Expected O, but got Unknown
		DirectoryInfo val = new DirectoryInfo(sourceDirectory);
		DirectoryInfo target = new DirectoryInfo(targetDirectory);
		CopyAll(val, target);
	}

	public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		if (!(((FileSystemInfo)source).get_FullName().ToLower() == ((FileSystemInfo)target).get_FullName().ToLower()) && ((FileSystemInfo)source).get_Exists())
		{
			if (!((FileSystemInfo)target).get_Exists())
			{
				target.Create();
			}
			FileInfo[] files = source.GetFiles();
			foreach (FileInfo val in files)
			{
				FileInfo val2 = new FileInfo(Path.Combine(((FileSystemInfo)target).get_FullName(), ((FileSystemInfo)val).get_Name()));
				val.CopyTo(((FileSystemInfo)val2).get_FullName(), true);
				((FileSystemInfo)val2).set_CreationTime(((FileSystemInfo)val).get_CreationTime());
				((FileSystemInfo)val2).set_LastAccessTime(((FileSystemInfo)val).get_LastAccessTime());
				((FileSystemInfo)val2).set_LastWriteTime(((FileSystemInfo)val).get_LastWriteTime());
			}
			DirectoryInfo[] directories = source.GetDirectories();
			foreach (DirectoryInfo val3 in directories)
			{
				DirectoryInfo val4 = target.CreateSubdirectory(((FileSystemInfo)val3).get_Name());
				CopyAll(val3, val4);
				((FileSystemInfo)val4).set_CreationTime(((FileSystemInfo)val3).get_CreationTime());
				((FileSystemInfo)val4).set_LastAccessTime(((FileSystemInfo)val3).get_LastAccessTime());
				((FileSystemInfo)val4).set_LastWriteTime(((FileSystemInfo)val3).get_LastWriteTime());
			}
		}
	}
}
