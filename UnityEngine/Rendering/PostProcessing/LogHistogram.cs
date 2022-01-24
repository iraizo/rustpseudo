namespace UnityEngine.Rendering.PostProcessing
{
	internal sealed class LogHistogram
	{
		public const int rangeMin = -9;

		public const int rangeMax = 9;

		private const int k_Bins = 128;

		public ComputeBuffer data { get; private set; }

		public void Generate(PostProcessRenderContext context)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			if (data == null)
			{
				data = new ComputeBuffer(128, 4);
			}
			Vector4 histogramScaleOffsetRes = GetHistogramScaleOffsetRes(context);
			ComputeShader exposureHistogram = context.resources.computeShaders.exposureHistogram;
			CommandBuffer command = context.command;
			command.BeginSample("LogHistogram");
			int num = exposureHistogram.FindKernel("KEyeHistogramClear");
			command.SetComputeBufferParam(exposureHistogram, num, "_HistogramBuffer", data);
			uint num2 = default(uint);
			uint num3 = default(uint);
			uint num4 = default(uint);
			exposureHistogram.GetKernelThreadGroupSizes(num, ref num2, ref num3, ref num4);
			command.DispatchCompute(exposureHistogram, num, Mathf.CeilToInt(128f / (float)num2), 1, 1);
			num = exposureHistogram.FindKernel("KEyeHistogram");
			command.SetComputeBufferParam(exposureHistogram, num, "_HistogramBuffer", data);
			command.SetComputeTextureParam(exposureHistogram, num, "_Source", context.source);
			command.SetComputeVectorParam(exposureHistogram, "_ScaleOffsetRes", histogramScaleOffsetRes);
			exposureHistogram.GetKernelThreadGroupSizes(num, ref num2, ref num3, ref num4);
			command.DispatchCompute(exposureHistogram, num, Mathf.CeilToInt(histogramScaleOffsetRes.z / 2f / (float)num2), Mathf.CeilToInt(histogramScaleOffsetRes.w / 2f / (float)num3), 1);
			command.EndSample("LogHistogram");
		}

		public Vector4 GetHistogramScaleOffsetRes(PostProcessRenderContext context)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			float num = 18f;
			float num2 = 1f / num;
			float num3 = 9f * num2;
			return new Vector4(num2, num3, (float)context.width, (float)context.height);
		}

		public void Release()
		{
			if (data != null)
			{
				data.Release();
			}
			data = null;
		}
	}
}
