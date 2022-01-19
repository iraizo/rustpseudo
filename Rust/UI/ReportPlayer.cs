using UnityEngine;

namespace Rust.UI
{
	public class ReportPlayer : UIDialog
	{
		public GameObject FindPlayer;

		public GameObject GetInformation;

		public GameObject Finished;

		public GameObject RecentlyReported;

		public Dropdown ReasonDropdown;

		public RustInput Subject;

		public RustInput Message;

		public RustButton ReportButton;

		public SteamUserButton SteamUserButton;

		public RustIcon ProgressIcon;

		public RustText ProgressText;

		public static Option[] ReportReasons = (Option[])(object)new Option[5]
		{
			new Option(new Phrase("report.reason.none", "Select an option"), "none", false, (Icons)61641),
			new Option(new Phrase("report.reason.abuse", "Racism/Sexism/Abusive"), "abusive", false, (Icons)62806),
			new Option(new Phrase("report.reason.cheat", "Cheating"), "cheat", false, (Icons)61531),
			new Option(new Phrase("report.reason.spam", "Spamming"), "spam", false, (Icons)61601),
			new Option(new Phrase("report.reason.name", "Offensive Name"), "name", false, (Icons)63417)
		};
	}
}
