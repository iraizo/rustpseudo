using System;
using UnityEngine;

public class Deployable : PrefabAttribute
{
	public Mesh guideMesh;

	public Vector3 guideMeshScale = Vector3.get_one();

	public bool guideLights = true;

	public bool wantsInstanceData;

	public bool copyInventoryFromItem;

	public bool setSocketParent;

	public bool toSlot;

	public BaseEntity.Slot slot;

	public GameObjectRef placeEffect;

	[NonSerialized]
	public Bounds bounds;

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		bounds = rootObj.GetComponent<BaseEntity>().bounds;
	}

	protected override Type GetIndexedType()
	{
		return typeof(Deployable);
	}
}
