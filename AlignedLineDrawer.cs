using System;
using UnityEngine;

public class AlignedLineDrawer : MonoBehaviour, IClientComponent
{
	[Serializable]
	public struct LinePoint
	{
		public Vector3 LocalPosition;

		public Vector3 WorldNormal;
	}

	public MeshFilter Filter;

	public MeshRenderer Renderer;

	public float LineWidth = 1f;

	public float SurfaceOffset = 0.001f;

	public float uvTilingFactor = 1f;

	public bool DrawEndCaps;

	public SprayCanSpray_Freehand Spray;

	public AlignedLineDrawer()
		: this()
	{
	}
}
