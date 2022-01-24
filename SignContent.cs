using Facepunch;
using ProtoBuf;

public class SignContent : ImageStorageEntity
{
	private uint[] textureIDs = new uint[1];

	protected override uint CrcToLoad => textureIDs[0];

	protected override FileStorage.Type StorageType => FileStorage.Type.png;

	public void CopyInfoFromSign(ISignage s)
	{
		uint[] textureCRCs = s.GetTextureCRCs();
		textureIDs = new uint[textureCRCs.Length];
		textureCRCs.CopyTo(textureIDs, 0);
		FileStorage.server.ReassignEntityId(s.NetworkID, net.ID);
	}

	public void CopyInfoToSign(ISignage s)
	{
		FileStorage.server.ReassignEntityId(net.ID, s.NetworkID);
		s.SetTextureCRCs(textureIDs);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.paintableSign == null)
		{
			info.msg.paintableSign = Pool.Get<PaintableSign>();
		}
		info.msg.paintableSign.crcs = Pool.GetList<uint>();
		uint[] array = textureIDs;
		foreach (uint item in array)
		{
			info.msg.paintableSign.crcs.Add(item);
		}
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		FileStorage.server.RemoveAllByEntity(net.ID);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.paintableSign != null)
		{
			textureIDs = new uint[info.msg.paintableSign.crcs.Count];
			for (int i = 0; i < info.msg.paintableSign.crcs.Count; i++)
			{
				textureIDs[i] = info.msg.paintableSign.crcs[i];
			}
		}
	}
}
