using UnityEngine;

public class EffectAudioPerspectiveSwitcher : MonoBehaviour, IEffect
{
	[HideInInspector]
	public EffectParentToWeaponBone parentToWeaponComponent;

	public EffectAudioPerspectiveSwitcher()
		: this()
	{
	}
}
