using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileWeaponMod : BaseEntity
{
	[Serializable]
	public struct Modifier
	{
		public bool enabled;

		[Tooltip("1 means no change. 0.5 is half.")]
		public float scalar;

		[Tooltip("Added after the scalar is applied.")]
		public float offset;
	}

	[Header("Silencer")]
	public GameObjectRef defaultSilencerEffect;

	public bool isSilencer;

	[Header("Weapon Basics")]
	public Modifier repeatDelay;

	public Modifier projectileVelocity;

	public Modifier projectileDamage;

	public Modifier projectileDistance;

	[Header("Recoil")]
	public Modifier aimsway;

	public Modifier aimswaySpeed;

	public Modifier recoil;

	[Header("Aim Cone")]
	public Modifier sightAimCone;

	public Modifier hipAimCone;

	[Header("Light Effects")]
	public bool isLight;

	[Header("MuzzleBrake")]
	public bool isMuzzleBrake;

	[Header("MuzzleBoost")]
	public bool isMuzzleBoost;

	[Header("Scope")]
	public bool isScope;

	public float zoomAmountDisplayOnly;

	public bool needsOnForEffects;

	public override void ServerInit()
	{
		SetFlag(Flags.Disabled, b: true);
		base.ServerInit();
	}

	public override void PostServerLoad()
	{
		base.limitNetworking = HasFlag(Flags.Disabled);
	}

	public static float Mult(BaseEntity parentEnt, Func<ProjectileWeaponMod, Modifier> selector_modifier, Func<Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = GetMods(parentEnt, selector_modifier, selector_value);
		float num = 1f;
		foreach (float item in mods)
		{
			num *= item;
		}
		return num;
	}

	public static float Sum(BaseEntity parentEnt, Func<ProjectileWeaponMod, Modifier> selector_modifier, Func<Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = GetMods(parentEnt, selector_modifier, selector_value);
		if (Enumerable.Count<float>(mods) != 0)
		{
			return Enumerable.Sum(mods);
		}
		return def;
	}

	public static float Average(BaseEntity parentEnt, Func<ProjectileWeaponMod, Modifier> selector_modifier, Func<Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = GetMods(parentEnt, selector_modifier, selector_value);
		if (Enumerable.Count<float>(mods) != 0)
		{
			return Enumerable.Average(mods);
		}
		return def;
	}

	public static float Max(BaseEntity parentEnt, Func<ProjectileWeaponMod, Modifier> selector_modifier, Func<Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = GetMods(parentEnt, selector_modifier, selector_value);
		if (Enumerable.Count<float>(mods) != 0)
		{
			return Enumerable.Max(mods);
		}
		return def;
	}

	public static float Min(BaseEntity parentEnt, Func<ProjectileWeaponMod, Modifier> selector_modifier, Func<Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = GetMods(parentEnt, selector_modifier, selector_value);
		if (Enumerable.Count<float>(mods) != 0)
		{
			return Enumerable.Min(mods);
		}
		return def;
	}

	public static IEnumerable<float> GetMods(BaseEntity parentEnt, Func<ProjectileWeaponMod, Modifier> selector_modifier, Func<Modifier, float> selector_value)
	{
		return Enumerable.Select<Modifier, float>(Enumerable.Where<Modifier>(Enumerable.Select<ProjectileWeaponMod, Modifier>(Enumerable.Where<ProjectileWeaponMod>(Enumerable.Cast<ProjectileWeaponMod>((IEnumerable)parentEnt.children), (Func<ProjectileWeaponMod, bool>)((ProjectileWeaponMod x) => (Object)(object)x != (Object)null && (!x.needsOnForEffects || x.HasFlag(Flags.On)))), selector_modifier), (Func<Modifier, bool>)((Modifier x) => x.enabled)), selector_value);
	}

	public static bool HasBrokenWeaponMod(BaseEntity parentEnt)
	{
		if (parentEnt.children == null)
		{
			return false;
		}
		if (Enumerable.Any<ProjectileWeaponMod>(Enumerable.Cast<ProjectileWeaponMod>((IEnumerable)parentEnt.children), (Func<ProjectileWeaponMod, bool>)((ProjectileWeaponMod x) => (Object)(object)x != (Object)null && x.IsBroken())))
		{
			return true;
		}
		return false;
	}
}
