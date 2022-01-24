using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

public class StagedResourceEntity : ResourceEntity
{
	[Serializable]
	public class ResourceStage
	{
		public float health;

		public GameObject instance;
	}

	public List<ResourceStage> stages = new List<ResourceStage>();

	public int stage;

	public GameObjectRef changeStageEffect;

	public GameObject gibSourceTest;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("StagedResourceEntity.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource != null)
		{
			int num = info.msg.resource.stage;
			if (info.fromDisk && base.isServer)
			{
				health = startHealth;
				num = 0;
			}
			if (num != stage)
			{
				stage = num;
				UpdateStage();
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.resource == null)
		{
			info.msg.resource = Pool.Get<BaseResource>();
		}
		info.msg.resource.health = Health();
		info.msg.resource.stage = stage;
	}

	protected override void OnHealthChanged()
	{
		((FacepunchBehaviour)this).Invoke((Action)UpdateNetworkStage, 0.1f);
	}

	protected virtual void UpdateNetworkStage()
	{
		if (FindBestStage() != stage)
		{
			stage = FindBestStage();
			SendNetworkUpdate();
			UpdateStage();
		}
	}

	private int FindBestStage()
	{
		float num = Mathf.InverseLerp(0f, MaxHealth(), Health());
		for (int i = 0; i < stages.Count; i++)
		{
			if (num >= stages[i].health)
			{
				return i;
			}
		}
		return stages.Count - 1;
	}

	public T GetStageComponent<T>() where T : Component
	{
		return stages[stage].instance.GetComponentInChildren<T>();
	}

	private void UpdateStage()
	{
		if (stages.Count != 0)
		{
			for (int i = 0; i < stages.Count; i++)
			{
				stages[i].instance.SetActive(i == stage);
			}
			GroundWatch.PhysicsChanged(((Component)this).get_gameObject());
		}
	}
}
