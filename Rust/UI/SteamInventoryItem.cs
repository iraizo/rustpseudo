using Facepunch.Extend;
using TMPro;
using UnityEngine;

namespace Rust.UI
{
	public class SteamInventoryItem : MonoBehaviour
	{
		public IPlayerItem Item;

		public HttpImage Image;

		public bool Setup(IPlayerItem item)
		{
			Item = item;
			if (PlayerItemExtensions.GetDefinition(item) == null)
			{
				return false;
			}
			((TMP_Text)((Component)TransformEx.FindChildRecursive(((Component)this).get_transform(), "ItemName")).GetComponent<TextMeshProUGUI>()).set_text(PlayerItemExtensions.GetDefinition(item).get_Name());
			return Image.Load(PlayerItemExtensions.GetDefinition(item).get_IconUrl());
		}

		public SteamInventoryItem()
			: this()
		{
		}
	}
}
