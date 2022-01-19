using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class MiningQuarry : BaseResourceExtractor
{
	[Serializable]
	public enum QuarryType
	{
		None,
		Basic,
		Sulfur,
		HQM
	}

	[Serializable]
	public class ChildPrefab
	{
		public GameObjectRef prefabToSpawn;

		public GameObject origin;

		public BaseEntity instance;

		public void DoSpawn(MiningQuarry owner)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			if (prefabToSpawn.isValid)
			{
				instance = GameManager.server.CreateEntity(prefabToSpawn.resourcePath, origin.get_transform().get_localPosition(), origin.get_transform().get_localRotation());
				instance.SetParent(owner);
				instance.Spawn();
			}
		}
	}

	public Animator beltAnimator;

	public Renderer beltScrollRenderer;

	public int scrollMatIndex = 3;

	public SoundPlayer[] onSounds;

	public float processRate = 5f;

	public float workToAdd = 15f;

	public GameObjectRef bucketDropEffect;

	public GameObject bucketDropTransform;

	public ChildPrefab engineSwitchPrefab;

	public ChildPrefab hopperPrefab;

	public ChildPrefab fuelStoragePrefab;

	public QuarryType staticType;

	public bool isStatic;

	private ResourceDepositManager.ResourceDeposit _linkedDeposit;

	public bool IsEngineOn()
	{
		return HasFlag(Flags.On);
	}

	private void SetOn(bool isOn)
	{
		SetFlag(Flags.On, isOn);
		engineSwitchPrefab.instance.SetFlag(Flags.On, isOn);
		SendNetworkUpdate();
		engineSwitchPrefab.instance.SendNetworkUpdate();
		if (isOn)
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)ProcessResources, processRate, processRate);
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)ProcessResources);
		}
	}

	public void EngineSwitch(bool isOn)
	{
		if (isOn && FuelCheck())
		{
			SetOn(isOn: true);
		}
		else
		{
			SetOn(isOn: false);
		}
	}

	public override void ServerInit()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (!isStatic)
		{
			ResourceDepositManager.ResourceDeposit resourceDeposit = (_linkedDeposit = ResourceDepositManager.GetOrCreate(((Component)this).get_transform().get_position()));
		}
		else
		{
			UpdateStaticDeposit();
		}
		SpawnChildEntities();
		engineSwitchPrefab.instance.SetFlag(Flags.On, HasFlag(Flags.On));
	}

	public void UpdateStaticDeposit()
	{
		if (isStatic)
		{
			if (_linkedDeposit == null)
			{
				_linkedDeposit = new ResourceDepositManager.ResourceDeposit();
			}
			else
			{
				_linkedDeposit._resources.Clear();
			}
			if (staticType == QuarryType.None)
			{
				_linkedDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 1000, 0.3f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM);
				_linkedDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, 1000, 5f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM);
				_linkedDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, 1000, 7.5f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM);
				_linkedDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, 1000, 75f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM);
			}
			else if (staticType == QuarryType.Basic)
			{
				_linkedDeposit.Add(ItemManager.FindItemDefinition("metal.ore"), 1f, 1000, 2f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM);
				_linkedDeposit.Add(ItemManager.FindItemDefinition("stones"), 1f, 1000, 0.3f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM);
			}
			else if (staticType == QuarryType.Sulfur)
			{
				_linkedDeposit.Add(ItemManager.FindItemDefinition("sulfur.ore"), 1f, 1000, 2f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM);
			}
			else if (staticType == QuarryType.HQM)
			{
				_linkedDeposit.Add(ItemManager.FindItemDefinition("hq.metal.ore"), 1f, 1000, 30f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM);
			}
			_linkedDeposit.Add(ItemManager.FindItemDefinition("crude.oil"), 1f, 1000, 10f, ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM, liquid: true);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		EngineSwitch(HasFlag(Flags.On));
		UpdateStaticDeposit();
	}

	public void SpawnChildEntities()
	{
		engineSwitchPrefab.DoSpawn(this);
		hopperPrefab.DoSpawn(this);
		fuelStoragePrefab.DoSpawn(this);
	}

	public void ProcessResources()
	{
		if (_linkedDeposit == null || (Object)(object)hopperPrefab.instance == (Object)null)
		{
			return;
		}
		foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resource in _linkedDeposit._resources)
		{
			if ((!canExtractLiquid && resource.isLiquid) || (!canExtractSolid && !resource.isLiquid))
			{
				continue;
			}
			resource.workDone += workToAdd;
			if (!(resource.workDone < resource.workNeeded))
			{
				int num = Mathf.FloorToInt(resource.workDone / resource.workNeeded);
				resource.workDone -= (float)num * resource.workNeeded;
				Item item = ItemManager.Create(resource.type, num, 0uL);
				if (!item.MoveToContainer(((Component)hopperPrefab.instance).GetComponent<StorageContainer>().inventory))
				{
					item.Remove();
					SetOn(isOn: false);
				}
			}
		}
		if (!FuelCheck())
		{
			SetOn(isOn: false);
		}
	}

	public bool FuelCheck()
	{
		Item item = ((Component)fuelStoragePrefab.instance).GetComponent<StorageContainer>().inventory.FindItemsByItemName("lowgradefuel");
		if (item != null && item.amount >= 1)
		{
			item.UseItem();
			return true;
		}
		return false;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if ((Object)(object)fuelStoragePrefab.instance == (Object)null || (Object)(object)hopperPrefab.instance == (Object)null)
			{
				Debug.Log((object)"Cannot save mining quary because children were null");
				return;
			}
			info.msg.miningQuarry = Pool.Get<MiningQuarry>();
			info.msg.miningQuarry.extractor = Pool.Get<ResourceExtractor>();
			info.msg.miningQuarry.extractor.fuelContents = ((Component)fuelStoragePrefab.instance).GetComponent<StorageContainer>().inventory.Save();
			info.msg.miningQuarry.extractor.outputContents = ((Component)hopperPrefab.instance).GetComponent<StorageContainer>().inventory.Save();
			info.msg.miningQuarry.staticType = (int)staticType;
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.miningQuarry != null)
		{
			if ((Object)(object)fuelStoragePrefab.instance == (Object)null || (Object)(object)hopperPrefab.instance == (Object)null)
			{
				Debug.Log((object)"Cannot load mining quary because children were null");
				return;
			}
			((Component)fuelStoragePrefab.instance).GetComponent<StorageContainer>().inventory.Load(info.msg.miningQuarry.extractor.fuelContents);
			((Component)hopperPrefab.instance).GetComponent<StorageContainer>().inventory.Load(info.msg.miningQuarry.extractor.outputContents);
			staticType = (QuarryType)info.msg.miningQuarry.staticType;
		}
	}

	public void Update()
	{
	}
}
