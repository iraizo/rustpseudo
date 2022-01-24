using Rust;
using UnityEngine;

public class ItemModSound : ItemMod
{
	public enum Type
	{
		OnAttachToWeapon
	}

	public GameObjectRef effect = new GameObjectRef();

	public Type actionType;

	public override void OnParentChanged(Item item)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (!Application.isLoadingSave && actionType == Type.OnAttachToWeapon && item.parentItem != null && item.parentItem.info.category == ItemCategory.Weapon)
		{
			BasePlayer ownerPlayer = item.parentItem.GetOwnerPlayer();
			if (!((Object)(object)ownerPlayer == (Object)null))
			{
				Effect.server.Run(effect.resourcePath, ownerPlayer, 0u, Vector3.get_zero(), Vector3.get_zero());
			}
		}
	}
}
