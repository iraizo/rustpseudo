using System;
using Facepunch;
using UnityEngine;

[Serializable]
public class TokenisedPhrase : Phrase
{
	public override string translated => ReplaceTokens(((Phrase)this).get_translated());

	public static string ReplaceTokens(string str)
	{
		if (!str.Contains("["))
		{
			return str;
		}
		str = str.Replace("[inventory.toggle]", string.Format("[{0}]", Input.GetButtonWithBind("inventory.toggle").ToUpper()));
		str = str.Replace("[inventory.togglecrafting]", string.Format("[{0}]", Input.GetButtonWithBind("inventory.togglecrafting").ToUpper()));
		str = str.Replace("[+map]", string.Format("[{0}]", Input.GetButtonWithBind("+map").ToUpper()));
		str = str.Replace("[inventory.examineheld]", string.Format("[{0}]", Input.GetButtonWithBind("inventory.examineheld").ToUpper()));
		str = str.Replace("[slot2]", string.Format("[{0}]", Input.GetButtonWithBind("+slot2").ToUpper()));
		str = str.Replace("[attack]", string.Format("[{0}]", TranslateMouseButton(Input.GetButtonWithBind("+attack")).ToUpper()));
		str = str.Replace("[attack2]", string.Format("[{0}]", TranslateMouseButton(Input.GetButtonWithBind("+attack2")).ToUpper()));
		str = str.Replace("[+use]", string.Format("[{0}]", TranslateMouseButton(Input.GetButtonWithBind("+use")).ToUpper()));
		str = str.Replace("[+altlook]", string.Format("[{0}]", TranslateMouseButton(Input.GetButtonWithBind("+altlook")).ToUpper()));
		str = str.Replace("[+reload]", string.Format("[{0}]", TranslateMouseButton(Input.GetButtonWithBind("+reload")).ToUpper()));
		str = str.Replace("[+voice]", string.Format("[{0}]", TranslateMouseButton(Input.GetButtonWithBind("+voice")).ToUpper()));
		str = str.Replace("[+lockBreakHealthPercent]", $"{0.15f:0%}");
		str = str.Replace("[+gestures]", string.Format("[{0}]", TranslateMouseButton(Input.GetButtonWithBind("+gestures")).ToUpper()));
		str = str.Replace("[+left]", string.Format("[{0}]", TranslateMouseButton(Input.GetButtonWithBind("+left")).ToUpper()));
		str = str.Replace("[+right]", string.Format("[{0}]", TranslateMouseButton(Input.GetButtonWithBind("+right")).ToUpper()));
		str = str.Replace("[+backward]", string.Format("[{0}]", TranslateMouseButton(Input.GetButtonWithBind("+backward")).ToUpper()));
		str = str.Replace("[+forward]", string.Format("[{0}]", TranslateMouseButton(Input.GetButtonWithBind("+forward")).ToUpper()));
		str = str.Replace("[+sprint]", string.Format("[{0}]", Input.GetButtonWithBind("+sprint")).ToUpper());
		str = str.Replace("[+duck]", string.Format("[{0}]", Input.GetButtonWithBind("+duck")).ToUpper());
		str = str.Replace("[+pets]", string.Format("[{0}]", Input.GetButtonWithBind("+pets")).ToUpper());
		return str;
	}

	public TokenisedPhrase(string t = "", string eng = "")
		: this(t, eng)
	{
	}

	public static string TranslateMouseButton(string mouseButton)
	{
		return mouseButton switch
		{
			"mouse0" => "Left Mouse", 
			"mouse1" => "Right Mouse", 
			"mouse2" => "Center Mouse", 
			_ => mouseButton, 
		};
	}

	private static string GetButtonWithBind(string s)
	{
		if (!Application.get_isPlaying())
		{
			switch (s)
			{
			case "inventory.toggle":
				return "tab";
			case "inventory.togglecrafting":
				return "q";
			case "+map":
				return "g";
			case "inventory.examineheld":
				return "n";
			case "+slot2":
				return "2";
			case "+attack":
				return "mouse0";
			case "+attack2":
				return "mouse1";
			case "+use":
				return "e";
			case "+altlook":
				return "leftalt";
			case "+reload":
				return "r";
			case "+voice":
				return "v";
			}
		}
		return Input.GetButtonWithBind(s);
	}
}
