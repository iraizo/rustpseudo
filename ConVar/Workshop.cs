namespace ConVar
{
	[Factory("workshop")]
	public class Workshop : ConsoleSystem
	{
		[ServerVar]
		public static void print_approved_skins(Arg arg)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			if (!PlatformService.Instance.get_IsValid() || PlatformService.Instance.get_ItemDefinitions() == null)
			{
				return;
			}
			TextTable val = new TextTable();
			val.AddColumn("name");
			val.AddColumn("itemshortname");
			val.AddColumn("workshopid");
			val.AddColumn("workshopdownload");
			foreach (IPlayerItemDefinition itemDefinition in PlatformService.Instance.get_ItemDefinitions())
			{
				string name = itemDefinition.get_Name();
				string itemShortName = itemDefinition.get_ItemShortName();
				string text = itemDefinition.get_WorkshopId().ToString();
				string text2 = itemDefinition.get_WorkshopDownload().ToString();
				val.AddRow(new string[4] { name, itemShortName, text, text2 });
			}
			arg.ReplyWith(((object)val).ToString());
		}

		public Workshop()
			: this()
		{
		}
	}
}
