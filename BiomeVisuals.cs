using UnityEngine;

public class BiomeVisuals : MonoBehaviour
{
	public GameObject Arid;

	public GameObject Temperate;

	public GameObject Tundra;

	public GameObject Arctic;

	protected void Start()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		switch (((Object)(object)TerrainMeta.BiomeMap != (Object)null) ? TerrainMeta.BiomeMap.GetBiomeMaxType(((Component)this).get_transform().get_position()) : 2)
		{
		case 1:
			SetChoice(Arid);
			break;
		case 2:
			SetChoice(Temperate);
			break;
		case 4:
			SetChoice(Tundra);
			break;
		case 8:
			SetChoice(Arctic);
			break;
		}
	}

	private void SetChoice(GameObject selection)
	{
		bool shouldDestroy = !((Component)this).get_gameObject().SupportsPoolingInParent();
		ApplyChoice(selection, Arid, shouldDestroy);
		ApplyChoice(selection, Temperate, shouldDestroy);
		ApplyChoice(selection, Tundra, shouldDestroy);
		ApplyChoice(selection, Arctic, shouldDestroy);
		if ((Object)(object)selection != (Object)null)
		{
			selection.SetActive(true);
		}
		GameManager.Destroy((Component)(object)this);
	}

	private void ApplyChoice(GameObject selection, GameObject target, bool shouldDestroy)
	{
		if ((Object)(object)target != (Object)null && (Object)(object)target != (Object)(object)selection)
		{
			if (shouldDestroy)
			{
				GameManager.Destroy(target);
			}
			else
			{
				target.SetActive(false);
			}
		}
	}

	public BiomeVisuals()
		: this()
	{
	}
}
