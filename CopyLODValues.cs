using UnityEngine;

public class CopyLODValues : MonoBehaviour
{
	[SerializeField]
	private LODGroup source;

	[SerializeField]
	private LODGroup destination;

	[Tooltip("Is false, exact values are copied. If true, values are scaled based on LODGroup size, so the changeover point will match.")]
	[SerializeField]
	private bool scale = true;

	public bool CanCopy()
	{
		if ((Object)(object)source != (Object)null)
		{
			return (Object)(object)destination != (Object)null;
		}
		return false;
	}

	public void Copy()
	{
		if (!CanCopy())
		{
			return;
		}
		LOD[] lODs = source.GetLODs();
		if (scale)
		{
			float num = destination.get_size() / source.get_size();
			for (int i = 0; i < lODs.Length; i++)
			{
				lODs[i].screenRelativeTransitionHeight *= num;
			}
		}
		LOD[] lODs2 = destination.GetLODs();
		for (int j = 0; j < lODs2.Length; j++)
		{
			if (j < lODs.Length)
			{
				lODs2[j].screenRelativeTransitionHeight = lODs[j].screenRelativeTransitionHeight;
				Debug.Log((object)$"Set destination LOD {j} to {lODs2[j].screenRelativeTransitionHeight}");
			}
		}
		destination.SetLODs(lODs2);
	}

	public CopyLODValues()
		: this()
	{
	}
}
