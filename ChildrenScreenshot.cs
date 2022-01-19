using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public class ChildrenScreenshot : MonoBehaviour
{
	public Vector3 offsetAngle = new Vector3(0f, 0f, 1f);

	public int width = 512;

	public int height = 512;

	public float fieldOfView = 70f;

	[Tooltip("0 = full recursive name, 1 = object name")]
	public string folder = "screenshots/{0}.png";

	[ContextMenu("Create Screenshots")]
	public void CreateScreenshots()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		RenderTexture val = new RenderTexture(width, height, 0);
		GameObject val2 = new GameObject();
		Camera val3 = val2.AddComponent<Camera>();
		val3.set_targetTexture(val);
		val3.set_orthographic(false);
		val3.set_fieldOfView(fieldOfView);
		val3.set_nearClipPlane(0.1f);
		val3.set_farClipPlane(2000f);
		val3.set_cullingMask(LayerMask.GetMask(new string[1] { "TransparentFX" }));
		val3.set_clearFlags((CameraClearFlags)2);
		val3.set_backgroundColor(new Color(0f, 0f, 0f, 0f));
		val3.set_renderingPath((RenderingPath)3);
		Texture2D val4 = new Texture2D(((Texture)val).get_width(), ((Texture)val).get_height(), (TextureFormat)5, false);
		foreach (Transform item in ((IEnumerable)((Component)this).get_transform()).Cast<Transform>())
		{
			PositionCamera(val3, ((Component)item).get_gameObject());
			int layer = ((Component)item).get_gameObject().get_layer();
			((Component)item).get_gameObject().SetLayerRecursive(1);
			val3.Render();
			((Component)item).get_gameObject().SetLayerRecursive(layer);
			string recursiveName = item.GetRecursiveName();
			recursiveName = recursiveName.Replace('/', '.');
			RenderTexture.set_active(val);
			val4.ReadPixels(new Rect(0f, 0f, (float)((Texture)val).get_width(), (float)((Texture)val).get_height()), 0, 0, false);
			RenderTexture.set_active((RenderTexture)null);
			byte[] bytes = ImageConversion.EncodeToPNG(val4);
			string path = string.Format(folder, recursiveName, ((Object)item).get_name());
			string directoryName = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllBytes(path, bytes);
		}
		Object.DestroyImmediate((Object)(object)val4, true);
		Object.DestroyImmediate((Object)(object)val, true);
		Object.DestroyImmediate((Object)(object)val2, true);
	}

	public void PositionCamera(Camera cam, GameObject obj)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = default(Bounds);
		((Bounds)(ref bounds))._002Ector(obj.get_transform().get_position(), Vector3.get_zero() * 0.1f);
		bool flag = true;
		Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
		foreach (Renderer val in componentsInChildren)
		{
			if (flag)
			{
				bounds = val.get_bounds();
				flag = false;
			}
			else
			{
				((Bounds)(ref bounds)).Encapsulate(val.get_bounds());
			}
		}
		Vector3 size = ((Bounds)(ref bounds)).get_size();
		float num = ((Vector3)(ref size)).get_magnitude() * 0.5f / Mathf.Tan(cam.get_fieldOfView() * 0.5f * ((float)Math.PI / 180f));
		((Component)cam).get_transform().set_position(((Bounds)(ref bounds)).get_center() + obj.get_transform().TransformVector(((Vector3)(ref offsetAngle)).get_normalized()) * num);
		((Component)cam).get_transform().LookAt(((Bounds)(ref bounds)).get_center());
	}

	public ChildrenScreenshot()
		: this()
	{
	}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
	//IL_0015: Unknown result type (might be due to invalid IL or missing references)

}
