using ConVar;
using UnityEngine;

public class ItemModSummerSunglassesEquip : ItemMod
{
	public float SunsetTime;

	public float SunriseTime;

	public string AchivementName;

	public override void DoAction(Item item, BasePlayer player)
	{
		base.DoAction(item, player);
		if ((Object)(object)player != (Object)null && !string.IsNullOrEmpty(AchivementName) && player.inventory.containerWear.FindItemByUID(item.uid) != null)
		{
			float time = Env.time;
			if (time < SunriseTime || time > SunsetTime)
			{
				player.GiveAchievement(AchivementName);
			}
		}
	}
}
