using UnityEngine;
using UnityEngine.Rendering;

public static class CommandBufferEx
{
	public static void BlitArray(this CommandBuffer cb, Mesh blitMesh, RenderTargetIdentifier source, Material mat, int slice, int pass = 0)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		cb.SetGlobalTexture("_Source", source);
		cb.SetGlobalFloat("_SourceMip", 0f);
		if (slice >= 0)
		{
			cb.SetGlobalFloat("_SourceSlice", (float)slice);
			cb.SetGlobalInt("_TargetSlice", slice);
		}
		cb.DrawMesh(blitMesh, Matrix4x4.get_identity(), mat, 0, pass);
	}

	public static void BlitArray(this CommandBuffer cb, Mesh blitMesh, RenderTargetIdentifier source, Texture target, Material mat, int slice, int pass = 0)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		cb.SetRenderTarget(RenderTargetIdentifier.op_Implicit(target), 0, (CubemapFace)0, -1);
		cb.SetGlobalTexture("_Source", source);
		cb.SetGlobalFloat("_SourceMip", 0f);
		if (slice >= 0)
		{
			cb.SetGlobalFloat("_SourceSlice", (float)slice);
			cb.SetGlobalInt("_TargetSlice", slice);
		}
		cb.DrawMesh(blitMesh, Matrix4x4.get_identity(), mat, 0, pass);
	}

	public static void BlitArrayMip(this CommandBuffer cb, Mesh blitMesh, Texture source, int sourceMip, int sourceSlice, Texture target, int targetMip, int targetSlice, Material mat, int pass = 0)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		int num = source.get_width() >> sourceMip;
		int num2 = source.get_height() >> sourceMip;
		Vector4 val = default(Vector4);
		((Vector4)(ref val))._002Ector(1f / (float)num, 1f / (float)num2, (float)num, (float)num2);
		int num3 = target.get_width() >> targetMip;
		int num4 = target.get_height() >> targetMip;
		Vector4 val2 = default(Vector4);
		((Vector4)(ref val2))._002Ector(1f / (float)num3, 1f / (float)num4, (float)num3, (float)num4);
		cb.SetGlobalTexture("_Source", RenderTargetIdentifier.op_Implicit(source));
		cb.SetGlobalVector("_Source_TexelSize", val);
		cb.SetGlobalVector("_Target_TexelSize", val2);
		cb.SetGlobalFloat("_SourceMip", (float)sourceMip);
		if (sourceSlice >= 0)
		{
			cb.SetGlobalFloat("_SourceSlice", (float)sourceSlice);
			cb.SetGlobalInt("_TargetSlice", targetSlice);
		}
		cb.SetRenderTarget(RenderTargetIdentifier.op_Implicit(target), targetMip, (CubemapFace)0, -1);
		cb.DrawMesh(blitMesh, Matrix4x4.get_identity(), mat, 0, pass);
	}

	public static void BlitMip(this CommandBuffer cb, Mesh blitMesh, Texture source, Texture target, int mip, int slice, Material mat, int pass = 0)
	{
		cb.BlitArrayMip(blitMesh, source, mip, slice, target, mip, slice, mat, pass);
	}
}
