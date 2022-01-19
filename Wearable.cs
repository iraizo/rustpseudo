using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

public class Wearable : MonoBehaviour, IItemSetup, IPrefabPreProcess
{
	[Flags]
	public enum RemoveSkin
	{
		Torso = 0x1,
		Feet = 0x2,
		Hands = 0x4,
		Legs = 0x8,
		Head = 0x10
	}

	[Flags]
	public enum RemoveHair
	{
		Head = 0x1,
		Eyebrow = 0x2,
		Facial = 0x4,
		Armpit = 0x8,
		Pubic = 0x10
	}

	[Flags]
	public enum DeformHair
	{
		None = 0x0,
		BaseballCap = 0x1,
		BoonieHat = 0x2,
		CandleHat = 0x3,
		MinersHat = 0x4,
		WoodHelmet = 0x5
	}

	[Flags]
	public enum OccupationSlots
	{
		HeadTop = 0x1,
		Face = 0x2,
		HeadBack = 0x4,
		TorsoFront = 0x8,
		TorsoBack = 0x10,
		LeftShoulder = 0x20,
		RightShoulder = 0x40,
		LeftArm = 0x80,
		RightArm = 0x100,
		LeftHand = 0x200,
		RightHand = 0x400,
		Groin = 0x800,
		Bum = 0x1000,
		LeftKnee = 0x2000,
		RightKnee = 0x4000,
		LeftLeg = 0x8000,
		RightLeg = 0x10000,
		LeftFoot = 0x20000,
		RightFoot = 0x40000,
		Mouth = 0x80000,
		Eyes = 0x100000
	}

	[InspectorFlags]
	public RemoveSkin removeSkin;

	[InspectorFlags]
	public RemoveSkin removeSkinFirstPerson;

	[InspectorFlags]
	public RemoveHair removeHair;

	[InspectorFlags]
	public DeformHair deformHair;

	[InspectorFlags]
	public OccupationSlots occupationUnder;

	[InspectorFlags]
	public OccupationSlots occupationOver;

	public bool showCensorshipCube;

	public bool showCensorshipCubeBreasts;

	public bool forceHideCensorshipBreasts;

	public string followBone;

	public bool disableRigStripping;

	public bool overrideDownLimit;

	public float downLimit = 70f;

	[HideInInspector]
	public PlayerModelHair playerModelHair;

	[HideInInspector]
	public PlayerModelHairCap playerModelHairCap;

	[HideInInspector]
	public WearableReplacementByRace wearableReplacementByRace;

	[HideInInspector]
	public List<Renderer> renderers = new List<Renderer>();

	[HideInInspector]
	public List<PlayerModelSkin> playerModelSkins = new List<PlayerModelSkin>();

	[HideInInspector]
	public List<BoneRetarget> boneRetargets = new List<BoneRetarget>();

	[HideInInspector]
	public List<SkinnedMeshRenderer> skinnedRenderers = new List<SkinnedMeshRenderer>();

	[HideInInspector]
	public List<SkeletonSkin> skeletonSkins = new List<SkeletonSkin>();

	[HideInInspector]
	public List<ComponentInfo> componentInfos = new List<ComponentInfo>();

	public bool HideInEyesView;

	[Header("First Person Legs")]
	[Tooltip("If this is true, we'll hide this item in the first person view. Usually done for items that you definitely won't see in first person view, like facemasks and hats.")]
	public bool HideInFirstPerson;

	[Tooltip("Use this if the clothing item clips into the player view. It'll push the chest legs model backwards.")]
	[Range(0f, 5f)]
	public float ExtraLeanBack;

	[Tooltip("Enable this to check for BoneRetargets which need to be preserved in first person view")]
	public bool PreserveBones;

	public Renderer[] RenderersLod0;

	public Renderer[] RenderersLod1;

	public Renderer[] RenderersLod2;

	public Renderer[] RenderersLod3;

	public Renderer[] SkipInFirstPersonLegs;

	private static LOD[] emptyLOD = (LOD[])(object)new LOD[1];

	public void OnItemSetup(Item item)
	{
	}

	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		LODGroup[] componentsInChildren = ((Component)this).GetComponentsInChildren<LODGroup>(true);
		foreach (LODGroup val in componentsInChildren)
		{
			val.SetLODs(emptyLOD);
			preProcess.RemoveComponent((Component)(object)val);
		}
	}

	public void CacheComponents()
	{
		playerModelHairCap = ((Component)this).GetComponent<PlayerModelHairCap>();
		playerModelHair = ((Component)this).GetComponent<PlayerModelHair>();
		wearableReplacementByRace = ((Component)this).GetComponent<WearableReplacementByRace>();
		((Component)this).GetComponentsInChildren<Renderer>(true, renderers);
		((Component)this).GetComponentsInChildren<PlayerModelSkin>(true, playerModelSkins);
		((Component)this).GetComponentsInChildren<BoneRetarget>(true, boneRetargets);
		((Component)this).GetComponentsInChildren<SkinnedMeshRenderer>(true, skinnedRenderers);
		((Component)this).GetComponentsInChildren<SkeletonSkin>(true, skeletonSkins);
		((Component)this).GetComponentsInChildren<ComponentInfo>(true, componentInfos);
		RenderersLod0 = renderers.Where((Renderer x) => ((Object)((Component)x).get_gameObject()).get_name().EndsWith("0")).ToArray();
		RenderersLod1 = renderers.Where((Renderer x) => ((Object)((Component)x).get_gameObject()).get_name().EndsWith("1")).ToArray();
		RenderersLod2 = renderers.Where((Renderer x) => ((Object)((Component)x).get_gameObject()).get_name().EndsWith("2")).ToArray();
		RenderersLod3 = renderers.Where((Renderer x) => ((Object)((Component)x).get_gameObject()).get_name().EndsWith("3")).ToArray();
		foreach (Renderer renderer in renderers)
		{
			((Component)renderer).get_gameObject().AddComponent<ObjectMotionVectorFix>();
			renderer.set_motionVectorGenerationMode((MotionVectorGenerationMode)2);
		}
	}

	public void StripRig(IPrefabProcessor preProcess, SkinnedMeshRenderer skinnedMeshRenderer)
	{
		if (disableRigStripping)
		{
			return;
		}
		Transform val = skinnedMeshRenderer.FindRig();
		if (!((Object)(object)val != (Object)null))
		{
			return;
		}
		List<Transform> list = Pool.GetList<Transform>();
		((Component)val).GetComponentsInChildren<Transform>(list);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (preProcess != null)
			{
				preProcess.NominateForDeletion(((Component)list[num]).get_gameObject());
			}
			else
			{
				Object.DestroyImmediate((Object)(object)((Component)list[num]).get_gameObject());
			}
		}
		Pool.FreeList<Transform>(ref list);
	}

	public void SetupRendererCache(IPrefabProcessor preProcess)
	{
	}

	public Wearable()
		: this()
	{
	}
}
