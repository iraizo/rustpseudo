using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Skeleton Properties")]
public class SkeletonProperties : ScriptableObject
{
	[Serializable]
	public class BoneProperty
	{
		public GameObject bone;

		public Phrase name;

		public HitArea area;
	}

	public GameObject boneReference;

	[BoneProperty]
	public BoneProperty[] bones;

	[NonSerialized]
	private Dictionary<uint, BoneProperty> quickLookup;

	public void OnValidate()
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		if ((Object)(object)boneReference == (Object)null)
		{
			Debug.LogWarning((object)"boneReference is null", (Object)(object)this);
			return;
		}
		List<BoneProperty> list = Enumerable.ToList<BoneProperty>((IEnumerable<BoneProperty>)bones);
		foreach (Transform child in boneReference.get_transform().GetAllChildren())
		{
			if (Enumerable.All<BoneProperty>((IEnumerable<BoneProperty>)list, (Func<BoneProperty, bool>)((BoneProperty x) => (Object)(object)x.bone != (Object)(object)((Component)child).get_gameObject())))
			{
				list.Add(new BoneProperty
				{
					bone = ((Component)child).get_gameObject(),
					name = new Phrase("", "")
					{
						token = ((Object)child).get_name().ToLower(),
						english = ((Object)child).get_name().ToLower()
					}
				});
			}
		}
		bones = list.ToArray();
	}

	private void BuildDictionary()
	{
		quickLookup = new Dictionary<uint, BoneProperty>();
		BoneProperty[] array = bones;
		foreach (BoneProperty boneProperty in array)
		{
			if (boneProperty == null || (Object)(object)boneProperty.bone == (Object)null || ((Object)boneProperty.bone).get_name() == null)
			{
				Debug.LogWarning((object)("Bone error in SkeletonProperties.BuildDictionary for " + (((Object)(object)boneReference != (Object)null) ? ((Object)boneReference).get_name() : "?")));
				continue;
			}
			uint num = StringPool.Get(((Object)boneProperty.bone).get_name());
			if (!quickLookup.ContainsKey(num))
			{
				quickLookup.Add(num, boneProperty);
				continue;
			}
			string name = ((Object)boneProperty.bone).get_name();
			string name2 = ((Object)quickLookup[num].bone).get_name();
			Debug.LogWarning((object)("Duplicate bone id " + num + " for " + name + " and " + name2));
		}
	}

	public BoneProperty FindBone(uint id)
	{
		if (quickLookup == null)
		{
			BuildDictionary();
		}
		BoneProperty value = null;
		if (!quickLookup.TryGetValue(id, out value))
		{
			return null;
		}
		return value;
	}

	public SkeletonProperties()
		: this()
	{
	}
}
