using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class MapLayerRenderer : SingletonComponent<MapLayerRenderer>
{
	private int? _underwaterLabFloorCount;

	public Camera renderCamera;

	public CameraEvent cameraEvent;

	public Material renderMaterial;

	private MapLayer? _currentlyRenderedLayer;

	private void RenderTrainLayer()
	{
		CommandBuffer val = BuildCommandBufferTrainTunnels();
		try
		{
			RenderImpl(val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private CommandBuffer BuildCommandBufferTrainTunnels()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer val = new CommandBuffer();
		val.set_name("TrainLayer Render");
		CommandBuffer val2 = val;
		MeshFilter val4 = default(MeshFilter);
		foreach (DungeonGridCell dungeonGridCell in TerrainMeta.Path.DungeonGridCells)
		{
			if (dungeonGridCell.MapRenderers == null || dungeonGridCell.MapRenderers.Length == 0)
			{
				continue;
			}
			MeshRenderer[] mapRenderers = dungeonGridCell.MapRenderers;
			foreach (MeshRenderer val3 in mapRenderers)
			{
				if (!((Object)(object)val3 == (Object)null) && ((Component)val3).TryGetComponent<MeshFilter>(ref val4))
				{
					Mesh sharedMesh = val4.get_sharedMesh();
					int subMeshCount = sharedMesh.get_subMeshCount();
					Matrix4x4 localToWorldMatrix = ((Component)val3).get_transform().get_localToWorldMatrix();
					for (int j = 0; j < subMeshCount; j++)
					{
						val2.DrawMesh(sharedMesh, localToWorldMatrix, renderMaterial, j);
					}
				}
			}
		}
		return val2;
	}

	private void RenderUnderwaterLabs(int floor)
	{
		CommandBuffer val = BuildCommandBufferUnderwaterLabs(floor);
		try
		{
			RenderImpl(val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public int GetUnderwaterLabFloorCount()
	{
		if (_underwaterLabFloorCount.HasValue)
		{
			return _underwaterLabFloorCount.Value;
		}
		List<DungeonBaseInfo> dungeonBaseEntrances = TerrainMeta.Path.DungeonBaseEntrances;
		_underwaterLabFloorCount = ((dungeonBaseEntrances != null && dungeonBaseEntrances.Count > 0) ? dungeonBaseEntrances.Max((DungeonBaseInfo l) => l.Floors.Count) : 0);
		return _underwaterLabFloorCount.Value;
	}

	private CommandBuffer BuildCommandBufferUnderwaterLabs(int floor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer val = new CommandBuffer();
		val.set_name("UnderwaterLabLayer Render");
		CommandBuffer val2 = val;
		MeshFilter val4 = default(MeshFilter);
		foreach (DungeonBaseInfo dungeonBaseEntrance in TerrainMeta.Path.DungeonBaseEntrances)
		{
			if (dungeonBaseEntrance.Floors.Count <= floor)
			{
				continue;
			}
			foreach (DungeonBaseLink link in dungeonBaseEntrance.Floors[floor].Links)
			{
				if (link.MapRenderers == null || link.MapRenderers.Length == 0)
				{
					continue;
				}
				MeshRenderer[] mapRenderers = link.MapRenderers;
				foreach (MeshRenderer val3 in mapRenderers)
				{
					if (!((Object)(object)val3 == (Object)null) && ((Component)val3).TryGetComponent<MeshFilter>(ref val4))
					{
						Mesh sharedMesh = val4.get_sharedMesh();
						int subMeshCount = sharedMesh.get_subMeshCount();
						Matrix4x4 localToWorldMatrix = ((Component)val3).get_transform().get_localToWorldMatrix();
						for (int j = 0; j < subMeshCount; j++)
						{
							val2.DrawMesh(sharedMesh, localToWorldMatrix, renderMaterial, j);
						}
					}
				}
			}
		}
		return val2;
	}

	public void Render(MapLayer layer)
	{
		if (layer == _currentlyRenderedLayer)
		{
			return;
		}
		_currentlyRenderedLayer = layer;
		if (layer >= MapLayer.TrainTunnels)
		{
			if (layer == MapLayer.TrainTunnels)
			{
				RenderTrainLayer();
			}
			else
			{
				RenderUnderwaterLabs((int)(layer - 1));
			}
		}
	}

	private void RenderImpl(CommandBuffer cb)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		double num = (double)World.Size * 1.5;
		renderCamera.set_orthographicSize((float)num / 2f);
		renderCamera.RemoveAllCommandBuffers();
		renderCamera.AddCommandBuffer(cameraEvent, cb);
		renderCamera.Render();
		renderCamera.RemoveAllCommandBuffers();
	}

	public static MapLayerRenderer GetOrCreate()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)SingletonComponent<MapLayerRenderer>.Instance != (Object)null)
		{
			return SingletonComponent<MapLayerRenderer>.Instance;
		}
		return GameManager.server.CreatePrefab("assets/prefabs/engine/maplayerrenderer.prefab", Vector3.get_zero(), Quaternion.get_identity()).GetComponent<MapLayerRenderer>();
	}
}
