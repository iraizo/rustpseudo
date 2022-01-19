using System;
using UnityEngine;

[Serializable]
public class PowerLineWireConnection
{
	public Vector3 inOffset = Vector3.get_zero();

	public Vector3 outOffset = Vector3.get_zero();

	public float radius = 0.01f;

	public Transform start;

	public Transform end;
}
