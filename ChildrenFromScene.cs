using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChildrenFromScene : MonoBehaviour
{
	public string SceneName;

	public bool StartChildrenDisabled;

	private IEnumerator Start()
	{
		Debug.LogWarning((object)("WARNING: CHILDRENFROMSCENE(" + SceneName + ") - WE SHOULDN'T BE USING THIS SHITTY COMPONENT NOW WE HAVE AWESOME PREFABS"), (Object)(object)((Component)this).get_gameObject());
		Scene sceneByName = SceneManager.GetSceneByName(SceneName);
		if (!((Scene)(ref sceneByName)).get_isLoaded())
		{
			yield return SceneManager.LoadSceneAsync(SceneName, (LoadSceneMode)1);
		}
		sceneByName = SceneManager.GetSceneByName(SceneName);
		GameObject[] rootGameObjects = ((Scene)(ref sceneByName)).GetRootGameObjects();
		foreach (GameObject val in rootGameObjects)
		{
			val.get_transform().SetParent(((Component)this).get_transform(), false);
			val.Identity();
			Transform transform = val.get_transform();
			RectTransform val2 = (RectTransform)(object)((transform is RectTransform) ? transform : null);
			if (Object.op_Implicit((Object)(object)val2))
			{
				val2.set_pivot(Vector2.get_zero());
				val2.set_anchoredPosition(Vector2.get_zero());
				val2.set_anchorMin(Vector2.get_zero());
				val2.set_anchorMax(Vector2.get_one());
				val2.set_sizeDelta(Vector2.get_one());
			}
			SingletonComponent[] componentsInChildren = val.GetComponentsInChildren<SingletonComponent>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].SingletonSetup();
			}
			if (StartChildrenDisabled)
			{
				val.SetActive(false);
			}
		}
		SceneManager.UnloadSceneAsync(sceneByName);
	}

	public ChildrenFromScene()
		: this()
	{
	}
}
