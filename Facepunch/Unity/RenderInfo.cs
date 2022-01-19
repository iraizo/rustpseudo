using System;
using System.Collections.Generic;
using System.IO;
using Facepunch.Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace Facepunch.Unity
{
	public static class RenderInfo
	{
		public struct RendererInstance
		{
			public bool IsVisible;

			public bool CastShadows;

			public bool Enabled;

			public bool RecieveShadows;

			public float Size;

			public float Distance;

			public int BoneCount;

			public int MaterialCount;

			public int VertexCount;

			public int TriangleCount;

			public int SubMeshCount;

			public int BlendShapeCount;

			public string RenderType;

			public string MeshName;

			public string ObjectName;

			public string EntityName;

			public uint EntityId;

			public bool UpdateWhenOffscreen;

			public int ParticleCount;

			public static RendererInstance From(Renderer renderer)
			{
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Invalid comparison between Unknown and I4
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				RendererInstance result = default(RendererInstance);
				result.IsVisible = renderer.get_isVisible();
				result.CastShadows = (int)renderer.get_shadowCastingMode() > 0;
				result.RecieveShadows = renderer.get_receiveShadows();
				result.Enabled = renderer.get_enabled() && ((Component)renderer).get_gameObject().get_activeInHierarchy();
				Bounds bounds = renderer.get_bounds();
				Vector3 size = ((Bounds)(ref bounds)).get_size();
				result.Size = ((Vector3)(ref size)).get_magnitude();
				bounds = renderer.get_bounds();
				result.Distance = Vector3.Distance(((Bounds)(ref bounds)).get_center(), ((Component)Camera.get_main()).get_transform().get_position());
				result.MaterialCount = renderer.get_sharedMaterials().Length;
				result.RenderType = ((object)renderer).GetType().Name;
				BaseEntity baseEntity = ((Component)renderer).get_gameObject().ToBaseEntity();
				if (Object.op_Implicit((Object)(object)baseEntity))
				{
					result.EntityName = baseEntity.PrefabName;
					if (baseEntity.net != null)
					{
						result.EntityId = baseEntity.net.ID;
					}
				}
				else
				{
					result.ObjectName = ((Component)renderer).get_transform().GetRecursiveName();
				}
				if (renderer is MeshRenderer)
				{
					result.BoneCount = 0;
					MeshFilter component = ((Component)renderer).GetComponent<MeshFilter>();
					if (Object.op_Implicit((Object)(object)component))
					{
						result.ReadMesh(component.get_sharedMesh());
					}
				}
				if (renderer is SkinnedMeshRenderer)
				{
					SkinnedMeshRenderer val = (SkinnedMeshRenderer)(object)((renderer is SkinnedMeshRenderer) ? renderer : null);
					result.ReadMesh(val.get_sharedMesh());
					result.UpdateWhenOffscreen = val.get_updateWhenOffscreen();
				}
				if (renderer is ParticleSystemRenderer)
				{
					ParticleSystem component2 = ((Component)renderer).GetComponent<ParticleSystem>();
					if (Object.op_Implicit((Object)(object)component2))
					{
						result.MeshName = ((Object)component2).get_name();
						result.ParticleCount = component2.get_particleCount();
					}
				}
				return result;
			}

			public void ReadMesh(Mesh mesh)
			{
				if ((Object)(object)mesh == (Object)null)
				{
					MeshName = "<NULL>";
					return;
				}
				VertexCount = mesh.get_vertexCount();
				SubMeshCount = mesh.get_subMeshCount();
				BlendShapeCount = mesh.get_blendShapeCount();
				MeshName = ((Object)mesh).get_name();
			}
		}

		public static void GenerateReport()
		{
			Renderer[] array = Object.FindObjectsOfType<Renderer>();
			List<RendererInstance> list = new List<RendererInstance>();
			Renderer[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RendererInstance item = RendererInstance.From(array2[i]);
				list.Add(item);
			}
			string text = string.Format(Application.get_dataPath() + "/../RenderInfo-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
			string contents = JsonConvert.SerializeObject((object)list, (Formatting)1);
			File.WriteAllText(text, contents);
			string text2 = Application.get_streamingAssetsPath() + "/RenderInfo.exe";
			string text3 = "\"" + text + "\"";
			Debug.Log((object)("Launching " + text2 + " " + text3));
			Os.StartProcess(text2, text3);
		}
	}
}
