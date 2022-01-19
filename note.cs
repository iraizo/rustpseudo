using Facepunch.Extend;
using UnityEngine;

[Factory("note")]
public class note : ConsoleSystem
{
	[ServerUserVar]
	public static void update(Arg arg)
	{
		uint uInt = arg.GetUInt(0, 0u);
		string @string = arg.GetString(1, "");
		Item item = arg.Player().inventory.FindItemUID(uInt);
		if (item != null)
		{
			item.text = StringExtensions.Truncate(@string, 1024, (string)null);
			item.MarkDirty();
		}
	}

	public note()
		: this()
	{
	}
}
