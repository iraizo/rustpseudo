using Rust.Workshop;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/ItemSkin")]
public class ItemSkin : SteamInventoryItem
{
	public Skinnable Skinnable;

	public Material[] Materials;

	[Tooltip("If set, whenever we make an item with this skin, we'll spawn this item without a skin instead")]
	public ItemDefinition Redirect;

	public SteamInventoryItem UnlockedViaSteamItem;

	public void ApplySkin(GameObject obj)
	{
		if (!((Object)(object)Skinnable == (Object)null))
		{
			Skin.Apply(obj, Skinnable, Materials);
		}
	}

	public override bool HasUnlocked(ulong playerId)
	{
		if ((Object)(object)Redirect != (Object)null && (Object)(object)Redirect.isRedirectOf != (Object)null && (Object)(object)Redirect.isRedirectOf.steamItem != (Object)null)
		{
			BasePlayer basePlayer = BasePlayer.FindByID(playerId);
			if ((Object)(object)basePlayer != (Object)null && basePlayer.blueprints.CheckSkinOwnership(Redirect.isRedirectOf.steamItem.id, basePlayer.userID))
			{
				return true;
			}
		}
		if ((Object)(object)UnlockedViaSteamItem != (Object)null)
		{
			BasePlayer basePlayer2 = BasePlayer.FindByID(playerId);
			if ((Object)(object)basePlayer2 != (Object)null && basePlayer2.blueprints.CheckSkinOwnership(UnlockedViaSteamItem.id, basePlayer2.userID))
			{
				return true;
			}
		}
		return base.HasUnlocked(playerId);
	}
}
