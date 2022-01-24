using Facepunch;
using UnityEngine;

public class UIPrefab : MonoBehaviour
{
	public GameObject prefabSource;

	internal GameObject createdGameObject;

	private void Awake()
	{
		if (!((Object)(object)prefabSource == (Object)null) && !((Object)(object)createdGameObject != (Object)null))
		{
			createdGameObject = Instantiate.GameObject(prefabSource, (Transform)null);
			((Object)createdGameObject).set_name(((Object)prefabSource).get_name());
			createdGameObject.get_transform().SetParent(((Component)this).get_transform(), false);
			createdGameObject.Identity();
		}
	}

	public void SetVisible(bool visible)
	{
		if (!((Object)(object)createdGameObject == (Object)null) && createdGameObject.get_activeSelf() != visible)
		{
			createdGameObject.SetActive(visible);
		}
	}

	public UIPrefab()
		: this()
	{
	}
}
