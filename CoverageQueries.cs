using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
public class CoverageQueries : MonoBehaviour
{
	public class BufferSet
	{
		public int width;

		public int height;

		public Texture2D inputTexture;

		public RenderTexture resultTexture;

		public Color[] inputData = (Color[])(object)new Color[0];

		public Color32[] resultData = (Color32[])(object)new Color32[0];

		private Material coverageMat;

		private const int MaxAsyncGPUReadbackRequests = 10;

		private Queue<AsyncGPUReadbackRequest> asyncRequests = new Queue<AsyncGPUReadbackRequest>();

		public void Attach(Material coverageMat)
		{
			this.coverageMat = coverageMat;
		}

		public void Dispose(bool data = true)
		{
			if ((Object)(object)inputTexture != (Object)null)
			{
				Object.DestroyImmediate((Object)(object)inputTexture);
				inputTexture = null;
			}
			if ((Object)(object)resultTexture != (Object)null)
			{
				RenderTexture.set_active((RenderTexture)null);
				resultTexture.Release();
				Object.DestroyImmediate((Object)(object)resultTexture);
				resultTexture = null;
			}
			if (data)
			{
				inputData = (Color[])(object)new Color[0];
				resultData = (Color32[])(object)new Color32[0];
			}
		}

		public bool CheckResize(int count)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected O, but got Unknown
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Expected O, but got Unknown
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			if (count > inputData.Length || ((Object)(object)resultTexture != (Object)null && !resultTexture.IsCreated()))
			{
				Dispose(data: false);
				width = Mathf.CeilToInt(Mathf.Sqrt((float)count));
				height = Mathf.CeilToInt((float)count / (float)width);
				inputTexture = new Texture2D(width, height, (TextureFormat)20, false, true);
				((Object)inputTexture).set_name("_Input");
				((Texture)inputTexture).set_filterMode((FilterMode)0);
				((Texture)inputTexture).set_wrapMode((TextureWrapMode)1);
				resultTexture = new RenderTexture(width, height, 0, (RenderTextureFormat)0, (RenderTextureReadWrite)1);
				((Object)resultTexture).set_name("_Result");
				((Texture)resultTexture).set_filterMode((FilterMode)0);
				((Texture)resultTexture).set_wrapMode((TextureWrapMode)1);
				resultTexture.set_useMipMap(false);
				resultTexture.Create();
				int num = resultData.Length;
				int num2 = width * height;
				Array.Resize(ref inputData, num2);
				Array.Resize(ref resultData, num2);
				Color32 val = default(Color32);
				((Color32)(ref val))._002Ector(byte.MaxValue, (byte)0, (byte)0, (byte)0);
				for (int i = num; i < num2; i++)
				{
					resultData[i] = val;
				}
				return true;
			}
			return false;
		}

		public void UploadData()
		{
			if (inputData.Length != 0)
			{
				inputTexture.SetPixels(inputData);
				inputTexture.Apply();
			}
		}

		public void Dispatch(int count)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			if (inputData.Length != 0)
			{
				RenderBuffer activeColorBuffer = Graphics.get_activeColorBuffer();
				RenderBuffer activeDepthBuffer = Graphics.get_activeDepthBuffer();
				coverageMat.SetTexture("_Input", (Texture)(object)inputTexture);
				Graphics.Blit((Texture)(object)inputTexture, resultTexture, coverageMat, 0);
				Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
			}
		}

		public void IssueRead()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (asyncRequests.get_Count() < 10)
			{
				asyncRequests.Enqueue(AsyncGPUReadback.Request((Texture)(object)resultTexture, 0, (Action<AsyncGPUReadbackRequest>)null));
			}
		}

		public void GetResults()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if (resultData.Length == 0)
			{
				return;
			}
			while (asyncRequests.get_Count() > 0)
			{
				AsyncGPUReadbackRequest val = asyncRequests.Peek();
				if (((AsyncGPUReadbackRequest)(ref val)).get_hasError())
				{
					asyncRequests.Dequeue();
					continue;
				}
				if (((AsyncGPUReadbackRequest)(ref val)).get_done())
				{
					NativeArray<Color32> data = ((AsyncGPUReadbackRequest)(ref val)).GetData<Color32>(0);
					for (int i = 0; i < data.get_Length(); i++)
					{
						resultData[i] = data.get_Item(i);
					}
					asyncRequests.Dequeue();
					continue;
				}
				break;
			}
		}
	}

	public enum RadiusSpace
	{
		ScreenNormalized,
		World
	}

	public class Query
	{
		public struct Input
		{
			public Vector3 position;

			public RadiusSpace radiusSpace;

			public float radius;

			public int sampleCount;

			public float smoothingSpeed;
		}

		public struct Internal
		{
			public int id;

			public void Reset()
			{
				id = -1;
			}
		}

		public struct Result
		{
			public int passed;

			public float coverage;

			public float smoothCoverage;

			public float weightedCoverage;

			public float weightedSmoothCoverage;

			public bool originOccluded;

			public int frame;

			public float originVisibility;

			public float originSmoothVisibility;

			public void Reset()
			{
				passed = 0;
				coverage = 0f;
				smoothCoverage = 0f;
				weightedCoverage = 0f;
				weightedSmoothCoverage = 0f;
				originOccluded = true;
				frame = -1;
				originVisibility = 0f;
				originSmoothVisibility = 0f;
			}
		}

		public Input input;

		public Internal intern;

		public Result result;

		public bool IsRegistered => intern.id >= 0;
	}

	public bool debug;

	public float depthBias = -0.1f;

	public CoverageQueries()
		: this()
	{
	}
}
