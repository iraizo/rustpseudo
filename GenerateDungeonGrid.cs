using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateDungeonGrid : ProceduralComponent
{
	private class PathNode
	{
		public MonumentInfo monument;

		public PathFinder.Node node;
	}

	private class PathSegment
	{
		public PathFinder.Node start;

		public PathFinder.Node end;
	}

	private class PathLink
	{
		public PathLinkSide downwards;

		public PathLinkSide upwards;
	}

	private class PathLinkSide
	{
		public PathLinkSegment origin;

		public List<PathLinkSegment> segments;

		public PathLinkSegment prevSegment
		{
			get
			{
				if (segments.Count <= 0)
				{
					return origin;
				}
				return segments[segments.Count - 1];
			}
		}
	}

	private class PathLinkSegment
	{
		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;

		public Prefab<DungeonGridLink> prefab;

		public DungeonGridLink link;

		public Transform downSocket => link.DownSocket;

		public Transform upSocket => link.UpSocket;

		public DungeonGridLinkType downType => link.DownType;

		public DungeonGridLinkType upType => link.UpType;
	}

	public string TunnelFolder = string.Empty;

	public string StationFolder = string.Empty;

	public string TransitionFolder = string.Empty;

	public string LinkFolder = string.Empty;

	public InfrastructureType ConnectionType = InfrastructureType.Tunnel;

	public int CellSize = 216;

	public float LinkHeight = 1.5f;

	public float LinkRadius = 3f;

	public float LinkTransition = 9f;

	private const int MaxDepth = 100000;

	public override bool RunOnCache => true;

	public override void Process(uint seed)
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_0725: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0824: Unknown result type (might be due to invalid IL or missing references)
		//IL_0829: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0836: Unknown result type (might be due to invalid IL or missing references)
		//IL_083d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1005: Unknown result type (might be due to invalid IL or missing references)
		//IL_100a: Unknown result type (might be due to invalid IL or missing references)
		//IL_100c: Unknown result type (might be due to invalid IL or missing references)
		//IL_101d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1022: Unknown result type (might be due to invalid IL or missing references)
		//IL_1024: Unknown result type (might be due to invalid IL or missing references)
		//IL_1026: Unknown result type (might be due to invalid IL or missing references)
		//IL_1059: Unknown result type (might be due to invalid IL or missing references)
		//IL_105b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1060: Unknown result type (might be due to invalid IL or missing references)
		//IL_1076: Unknown result type (might be due to invalid IL or missing references)
		//IL_1078: Unknown result type (might be due to invalid IL or missing references)
		//IL_107a: Unknown result type (might be due to invalid IL or missing references)
		//IL_107f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1081: Unknown result type (might be due to invalid IL or missing references)
		//IL_1086: Unknown result type (might be due to invalid IL or missing references)
		//IL_108b: Unknown result type (might be due to invalid IL or missing references)
		//IL_109f: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_10af: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1106: Unknown result type (might be due to invalid IL or missing references)
		//IL_1108: Unknown result type (might be due to invalid IL or missing references)
		//IL_1131: Unknown result type (might be due to invalid IL or missing references)
		//IL_1136: Unknown result type (might be due to invalid IL or missing references)
		//IL_1138: Unknown result type (might be due to invalid IL or missing references)
		//IL_113d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1183: Unknown result type (might be due to invalid IL or missing references)
		//IL_1185: Unknown result type (might be due to invalid IL or missing references)
		//IL_118a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1193: Unknown result type (might be due to invalid IL or missing references)
		//IL_1195: Unknown result type (might be due to invalid IL or missing references)
		//IL_1197: Unknown result type (might be due to invalid IL or missing references)
		//IL_119c: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_12fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1301: Unknown result type (might be due to invalid IL or missing references)
		//IL_13de: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_13fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1400: Unknown result type (might be due to invalid IL or missing references)
		//IL_1409: Unknown result type (might be due to invalid IL or missing references)
		//IL_140b: Unknown result type (might be due to invalid IL or missing references)
		//IL_140d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1412: Unknown result type (might be due to invalid IL or missing references)
		//IL_1417: Unknown result type (might be due to invalid IL or missing references)
		//IL_1483: Unknown result type (might be due to invalid IL or missing references)
		//IL_1494: Unknown result type (might be due to invalid IL or missing references)
		//IL_14aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_14bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_14cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_14dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_1504: Unknown result type (might be due to invalid IL or missing references)
		//IL_1515: Unknown result type (might be due to invalid IL or missing references)
		//IL_151a: Unknown result type (might be due to invalid IL or missing references)
		//IL_151f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1524: Unknown result type (might be due to invalid IL or missing references)
		//IL_1529: Unknown result type (might be due to invalid IL or missing references)
		//IL_152b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1530: Unknown result type (might be due to invalid IL or missing references)
		//IL_1549: Unknown result type (might be due to invalid IL or missing references)
		//IL_154e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1564: Unknown result type (might be due to invalid IL or missing references)
		//IL_1569: Unknown result type (might be due to invalid IL or missing references)
		//IL_157c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1581: Unknown result type (might be due to invalid IL or missing references)
		//IL_15af: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_15bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_15db: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_15fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1604: Unknown result type (might be due to invalid IL or missing references)
		//IL_1609: Unknown result type (might be due to invalid IL or missing references)
		//IL_160d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1619: Unknown result type (might be due to invalid IL or missing references)
		//IL_161e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1623: Unknown result type (might be due to invalid IL or missing references)
		//IL_1682: Unknown result type (might be due to invalid IL or missing references)
		//IL_1693: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_16b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_16f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_16f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_16fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1704: Unknown result type (might be due to invalid IL or missing references)
		//IL_1709: Unknown result type (might be due to invalid IL or missing references)
		//IL_170e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1720: Unknown result type (might be due to invalid IL or missing references)
		//IL_172c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1731: Unknown result type (might be due to invalid IL or missing references)
		//IL_1736: Unknown result type (might be due to invalid IL or missing references)
		//IL_1742: Unknown result type (might be due to invalid IL or missing references)
		//IL_1744: Unknown result type (might be due to invalid IL or missing references)
		//IL_1758: Unknown result type (might be due to invalid IL or missing references)
		//IL_175a: Unknown result type (might be due to invalid IL or missing references)
		//IL_175c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1761: Unknown result type (might be due to invalid IL or missing references)
		//IL_1766: Unknown result type (might be due to invalid IL or missing references)
		//IL_1768: Unknown result type (might be due to invalid IL or missing references)
		//IL_176a: Unknown result type (might be due to invalid IL or missing references)
		//IL_176c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1771: Unknown result type (might be due to invalid IL or missing references)
		//IL_1773: Unknown result type (might be due to invalid IL or missing references)
		//IL_1775: Unknown result type (might be due to invalid IL or missing references)
		//IL_1777: Unknown result type (might be due to invalid IL or missing references)
		//IL_177c: Unknown result type (might be due to invalid IL or missing references)
		//IL_177e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1780: Unknown result type (might be due to invalid IL or missing references)
		//IL_1789: Unknown result type (might be due to invalid IL or missing references)
		//IL_178e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1793: Unknown result type (might be due to invalid IL or missing references)
		//IL_1798: Unknown result type (might be due to invalid IL or missing references)
		//IL_179a: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_17af: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_17be: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_17cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_17cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_17cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_17fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1810: Unknown result type (might be due to invalid IL or missing references)
		//IL_1812: Unknown result type (might be due to invalid IL or missing references)
		//IL_1859: Unknown result type (might be due to invalid IL or missing references)
		//IL_1865: Unknown result type (might be due to invalid IL or missing references)
		//IL_187c: Unknown result type (might be due to invalid IL or missing references)
		//IL_188f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1896: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_18cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_18de: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1903: Unknown result type (might be due to invalid IL or missing references)
		//IL_191a: Unknown result type (might be due to invalid IL or missing references)
		//IL_192d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1934: Unknown result type (might be due to invalid IL or missing references)
		//IL_194d: Unknown result type (might be due to invalid IL or missing references)
		//IL_195e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1975: Unknown result type (might be due to invalid IL or missing references)
		//IL_1988: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_19be: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a08: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1afc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1afe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b00: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b02: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b15: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b17: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b35: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b37: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b43: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b72: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b74: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b76: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b85: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b93: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ba7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bac: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bba: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bda: Unknown result type (might be due to invalid IL or missing references)
		//IL_1be1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bed: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c01: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c05: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c11: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c16: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1caf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ce2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cec: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d01: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d06: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d18: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d24: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d29: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d50: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d52: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d54: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d59: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d60: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d62: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d64: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d69: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d74: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d76: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d78: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d81: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d86: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d90: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d92: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1da7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dac: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dba: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1de0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1de2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1de4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1de9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1deb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ded: Unknown result type (might be due to invalid IL or missing references)
		//IL_1def: Unknown result type (might be due to invalid IL or missing references)
		//IL_1df4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e08: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e51: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e74: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e87: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ea0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eac: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ed6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1edd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eef: Unknown result type (might be due to invalid IL or missing references)
		//IL_1efb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f12: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f25: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f45: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f56: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f80: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1feb: Unknown result type (might be due to invalid IL or missing references)
		//IL_2000: Unknown result type (might be due to invalid IL or missing references)
		//IL_202f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2052: Unknown result type (might be due to invalid IL or missing references)
		//IL_2082: Unknown result type (might be due to invalid IL or missing references)
		//IL_20a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_20d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_20da: Unknown result type (might be due to invalid IL or missing references)
		//IL_20ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_20ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_20f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_20f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_20f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_20fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_210d: Unknown result type (might be due to invalid IL or missing references)
		//IL_210f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2124: Unknown result type (might be due to invalid IL or missing references)
		//IL_2126: Unknown result type (might be due to invalid IL or missing references)
		//IL_212d: Unknown result type (might be due to invalid IL or missing references)
		//IL_212f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2136: Unknown result type (might be due to invalid IL or missing references)
		//IL_213b: Unknown result type (might be due to invalid IL or missing references)
		//IL_216a: Unknown result type (might be due to invalid IL or missing references)
		//IL_216c: Unknown result type (might be due to invalid IL or missing references)
		//IL_216e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2173: Unknown result type (might be due to invalid IL or missing references)
		//IL_2213: Unknown result type (might be due to invalid IL or missing references)
		//IL_221a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2221: Unknown result type (might be due to invalid IL or missing references)
		//IL_2270: Unknown result type (might be due to invalid IL or missing references)
		//IL_2277: Unknown result type (might be due to invalid IL or missing references)
		//IL_227e: Unknown result type (might be due to invalid IL or missing references)
		if (World.Cached)
		{
			TerrainMeta.Path.DungeonGridRoot = HierarchyUtil.GetRoot("Dungeon");
			return;
		}
		if (World.Networked)
		{
			World.Spawn("Dungeon");
			TerrainMeta.Path.DungeonGridRoot = HierarchyUtil.GetRoot("Dungeon");
			return;
		}
		Prefab<DungeonGridCell>[] array = Prefab.Load<DungeonGridCell>("assets/bundled/prefabs/autospawn/" + TunnelFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		Prefab<DungeonGridCell>[] array2 = Prefab.Load<DungeonGridCell>("assets/bundled/prefabs/autospawn/" + StationFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true);
		if (array2 == null || array2.Length == 0)
		{
			return;
		}
		Prefab<DungeonGridCell>[] array3 = Prefab.Load<DungeonGridCell>("assets/bundled/prefabs/autospawn/" + TransitionFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true);
		if (array3 == null)
		{
			return;
		}
		Prefab<DungeonGridLink>[] array4 = Prefab.Load<DungeonGridLink>("assets/bundled/prefabs/autospawn/" + LinkFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true);
		if (array4 == null)
		{
			return;
		}
		array4 = array4.OrderByDescending((Prefab<DungeonGridLink> x) => x.Component.Priority).ToArray();
		List<DungeonGridInfo> list = (Object.op_Implicit((Object)(object)TerrainMeta.Path) ? TerrainMeta.Path.DungeonGridEntrances : null);
		WorldSpaceGrid<Prefab<DungeonGridCell>> val = new WorldSpaceGrid<Prefab<DungeonGridCell>>(TerrainMeta.Size.x * 2f, (float)CellSize);
		int[,] array5 = new int[val.CellCount, val.CellCount];
		DungeonGridConnectionHash[,] hashmap = new DungeonGridConnectionHash[val.CellCount, val.CellCount];
		PathFinder pathFinder = new PathFinder(array5, diagonals: false);
		int cellCount = val.CellCount;
		int num = 0;
		int num2 = val.CellCount - 1;
		for (int i = 0; i < cellCount; i++)
		{
			for (int j = 0; j < cellCount; j++)
			{
				array5[j, i] = 1;
			}
		}
		List<PathSegment> list2 = new List<PathSegment>();
		List<PathLink> list3 = new List<PathLink>();
		List<PathNode> list4 = new List<PathNode>();
		List<PathNode> unconnectedNodeList = new List<PathNode>();
		List<PathNode> secondaryNodeList = new List<PathNode>();
		List<PathFinder.Point> list5 = new List<PathFinder.Point>();
		List<PathFinder.Point> list6 = new List<PathFinder.Point>();
		List<PathFinder.Point> list7 = new List<PathFinder.Point>();
		foreach (DungeonGridInfo item2 in list)
		{
			DungeonGridInfo entrance = item2;
			TerrainPathConnect[] componentsInChildren = ((Component)entrance).GetComponentsInChildren<TerrainPathConnect>(true);
			foreach (TerrainPathConnect terrainPathConnect in componentsInChildren)
			{
				if (terrainPathConnect.Type != ConnectionType)
				{
					continue;
				}
				Vector2i val2 = val.WorldToGridCoords(((Component)terrainPathConnect).get_transform().get_position());
				if (array5[val2.x, val2.y] == int.MaxValue)
				{
					continue;
				}
				PathFinder.Node stationNode = pathFinder.FindClosestWalkable(new PathFinder.Point(val2.x, val2.y), 1);
				if (stationNode == null)
				{
					continue;
				}
				Prefab<DungeonGridCell> prefab = ((val2.x > num) ? val.get_Item(val2.x - 1, val2.y) : null);
				Prefab<DungeonGridCell> prefab2 = ((val2.x < num2) ? val.get_Item(val2.x + 1, val2.y) : null);
				Prefab<DungeonGridCell> prefab3 = ((val2.y > num) ? val.get_Item(val2.x, val2.y - 1) : null);
				Prefab<DungeonGridCell> prefab4 = ((val2.y < num2) ? val.get_Item(val2.x, val2.y + 1) : null);
				Prefab<DungeonGridCell> prefab5 = null;
				float num3 = float.MaxValue;
				array2.Shuffle(ref seed);
				Prefab<DungeonGridCell>[] array6 = array2;
				foreach (Prefab<DungeonGridCell> prefab6 in array6)
				{
					if ((prefab != null && prefab6.Component.West != prefab.Component.East) || (prefab2 != null && prefab6.Component.East != prefab2.Component.West) || (prefab3 != null && prefab6.Component.South != prefab3.Component.North) || (prefab4 != null && prefab6.Component.North != prefab4.Component.South))
					{
						continue;
					}
					DungeonVolume componentInChildren = prefab6.Object.GetComponentInChildren<DungeonVolume>();
					DungeonVolume componentInChildren2 = ((Component)entrance).GetComponentInChildren<DungeonVolume>();
					OBB bounds = componentInChildren.GetBounds(val.GridToWorldCoords(val2), Quaternion.get_identity());
					OBB bounds2 = componentInChildren2.GetBounds(((Component)entrance).get_transform().get_position(), Quaternion.get_identity());
					if (!((OBB)(ref bounds)).Intersects2D(bounds2))
					{
						DungeonGridLink componentInChildren3 = prefab6.Object.GetComponentInChildren<DungeonGridLink>();
						Vector3 val3 = val.GridToWorldCoords(new Vector2i(val2.x, val2.y)) + componentInChildren3.UpSocket.get_localPosition();
						float num4 = Vector3Ex.Magnitude2D(((Component)terrainPathConnect).get_transform().get_position() - val3);
						if (!(num3 < num4))
						{
							prefab5 = prefab6;
							num3 = num4;
						}
					}
				}
				bool isStartPoint;
				if (prefab5 != null)
				{
					val.set_Item(val2.x, val2.y, prefab5);
					array5[val2.x, val2.y] = int.MaxValue;
					isStartPoint = secondaryNodeList.Count == 0;
					secondaryNodeList.RemoveAll((PathNode x) => x.node.point == stationNode.point);
					unconnectedNodeList.RemoveAll((PathNode x) => x.node.point == stationNode.point);
					if (prefab5.Component.West != 0)
					{
						AddNode(val2.x - 1, val2.y);
					}
					if (prefab5.Component.East != 0)
					{
						AddNode(val2.x + 1, val2.y);
					}
					if (prefab5.Component.South != 0)
					{
						AddNode(val2.x, val2.y - 1);
					}
					if (prefab5.Component.North != 0)
					{
						AddNode(val2.x, val2.y + 1);
					}
					PathLink pathLink = new PathLink();
					DungeonGridLink componentInChildren4 = ((Component)entrance).get_gameObject().GetComponentInChildren<DungeonGridLink>();
					Vector3 position = ((Component)entrance).get_transform().get_position();
					Quaternion rotation = ((Component)entrance).get_transform().get_rotation();
					Vector3 eulerAngles = ((Quaternion)(ref rotation)).get_eulerAngles();
					DungeonGridLink componentInChildren5 = prefab5.Object.GetComponentInChildren<DungeonGridLink>();
					Vector3 position2 = val.GridToWorldCoords(new Vector2i(val2.x, val2.y));
					Vector3 zero = Vector3.get_zero();
					pathLink.downwards = new PathLinkSide();
					pathLink.downwards.origin = new PathLinkSegment();
					pathLink.downwards.origin.position = position;
					pathLink.downwards.origin.rotation = Quaternion.Euler(eulerAngles);
					pathLink.downwards.origin.scale = Vector3.get_one();
					pathLink.downwards.origin.link = componentInChildren4;
					pathLink.downwards.segments = new List<PathLinkSegment>();
					pathLink.upwards = new PathLinkSide();
					pathLink.upwards.origin = new PathLinkSegment();
					pathLink.upwards.origin.position = position2;
					pathLink.upwards.origin.rotation = Quaternion.Euler(zero);
					pathLink.upwards.origin.scale = Vector3.get_one();
					pathLink.upwards.origin.link = componentInChildren5;
					pathLink.upwards.segments = new List<PathLinkSegment>();
					list3.Add(pathLink);
				}
				void AddNode(int x, int y)
				{
					//IL_0059: Unknown result type (might be due to invalid IL or missing references)
					PathFinder.Node node8 = pathFinder.FindClosestWalkable(new PathFinder.Point(x, y), 1);
					if (node8 != null)
					{
						PathNode item = new PathNode
						{
							monument = (Object.op_Implicit((Object)(object)TerrainMeta.Path) ? TerrainMeta.Path.FindClosest(TerrainMeta.Path.Monuments, ((Component)entrance).get_transform().get_position()) : ((Component)((Component)entrance).get_transform()).GetComponentInParent<MonumentInfo>()),
							node = node8
						};
						if (isStartPoint)
						{
							secondaryNodeList.Add(item);
						}
						else
						{
							unconnectedNodeList.Add(item);
						}
						DungeonGridConnectionHash dungeonGridConnectionHash4 = hashmap[node8.point.x, node8.point.y];
						DungeonGridConnectionHash dungeonGridConnectionHash5 = hashmap[stationNode.point.x, stationNode.point.y];
						if (node8.point.x > stationNode.point.x)
						{
							dungeonGridConnectionHash4.West = true;
							dungeonGridConnectionHash5.East = true;
						}
						if (node8.point.x < stationNode.point.x)
						{
							dungeonGridConnectionHash4.East = true;
							dungeonGridConnectionHash5.West = true;
						}
						if (node8.point.y > stationNode.point.y)
						{
							dungeonGridConnectionHash4.South = true;
							dungeonGridConnectionHash5.North = true;
						}
						if (node8.point.y < stationNode.point.y)
						{
							dungeonGridConnectionHash4.North = true;
							dungeonGridConnectionHash5.South = true;
						}
						hashmap[node8.point.x, node8.point.y] = dungeonGridConnectionHash4;
						hashmap[stationNode.point.x, stationNode.point.y] = dungeonGridConnectionHash5;
					}
				}
			}
		}
		while (unconnectedNodeList.Count != 0 || secondaryNodeList.Count != 0)
		{
			if (unconnectedNodeList.Count == 0)
			{
				PathNode node3 = secondaryNodeList[0];
				unconnectedNodeList.AddRange(secondaryNodeList.Where((PathNode x) => (Object)(object)x.monument == (Object)(object)node3.monument));
				secondaryNodeList.RemoveAll((PathNode x) => (Object)(object)x.monument == (Object)(object)node3.monument);
				Vector2i val4 = val.WorldToGridCoords(((Component)node3.monument).get_transform().get_position());
				pathFinder.PushPoint = new PathFinder.Point(val4.x, val4.y);
				pathFinder.PushRadius = 2;
				pathFinder.PushDistance = 2;
				pathFinder.PushMultiplier = 4;
			}
			list7.Clear();
			list7.AddRange(unconnectedNodeList.Select((PathNode x) => x.node.point));
			list6.Clear();
			list6.AddRange(list4.Select((PathNode x) => x.node.point));
			list6.AddRange(secondaryNodeList.Select((PathNode x) => x.node.point));
			list6.AddRange(list5);
			PathFinder.Node node4 = pathFinder.FindPathUndirected(list6, list7, 100000);
			if (node4 == null)
			{
				PathNode node2 = unconnectedNodeList[0];
				secondaryNodeList.AddRange(unconnectedNodeList.Where((PathNode x) => (Object)(object)x.monument == (Object)(object)node2.monument));
				unconnectedNodeList.RemoveAll((PathNode x) => (Object)(object)x.monument == (Object)(object)node2.monument);
				secondaryNodeList.Remove(node2);
				list4.Add(node2);
				continue;
			}
			PathSegment segment = new PathSegment();
			for (PathFinder.Node node5 = node4; node5 != null; node5 = node5.next)
			{
				if (node5 == node4)
				{
					segment.start = node5;
				}
				if (node5.next == null)
				{
					segment.end = node5;
				}
			}
			list2.Add(segment);
			PathNode node = unconnectedNodeList.Find((PathNode x) => x.node.point == segment.start.point || x.node.point == segment.end.point);
			secondaryNodeList.AddRange(unconnectedNodeList.Where((PathNode x) => (Object)(object)x.monument == (Object)(object)node.monument));
			unconnectedNodeList.RemoveAll((PathNode x) => (Object)(object)x.monument == (Object)(object)node.monument);
			secondaryNodeList.Remove(node);
			list4.Add(node);
			PathNode pathNode = secondaryNodeList.Find((PathNode x) => x.node.point == segment.start.point || x.node.point == segment.end.point);
			if (pathNode != null)
			{
				secondaryNodeList.Remove(pathNode);
				list4.Add(pathNode);
			}
			for (PathFinder.Node node6 = node4; node6 != null; node6 = node6.next)
			{
				if (node6 != node4 && node6.next != null)
				{
					list5.Add(node6.point);
				}
			}
		}
		foreach (PathSegment item3 in list2)
		{
			PathFinder.Node node7 = item3.start;
			while (node7 != null && node7.next != null)
			{
				DungeonGridConnectionHash dungeonGridConnectionHash = hashmap[node7.point.x, node7.point.y];
				DungeonGridConnectionHash dungeonGridConnectionHash2 = hashmap[node7.next.point.x, node7.next.point.y];
				if (node7.point.x > node7.next.point.x)
				{
					dungeonGridConnectionHash.West = true;
					dungeonGridConnectionHash2.East = true;
				}
				if (node7.point.x < node7.next.point.x)
				{
					dungeonGridConnectionHash.East = true;
					dungeonGridConnectionHash2.West = true;
				}
				if (node7.point.y > node7.next.point.y)
				{
					dungeonGridConnectionHash.South = true;
					dungeonGridConnectionHash2.North = true;
				}
				if (node7.point.y < node7.next.point.y)
				{
					dungeonGridConnectionHash.North = true;
					dungeonGridConnectionHash2.South = true;
				}
				hashmap[node7.point.x, node7.point.y] = dungeonGridConnectionHash;
				hashmap[node7.next.point.x, node7.next.point.y] = dungeonGridConnectionHash2;
				node7 = node7.next;
			}
		}
		for (int m = 0; m < val.CellCount; m++)
		{
			for (int n = 0; n < val.CellCount; n++)
			{
				if (array5[m, n] == int.MaxValue)
				{
					continue;
				}
				DungeonGridConnectionHash dungeonGridConnectionHash3 = hashmap[m, n];
				if (dungeonGridConnectionHash3.Value == 0)
				{
					continue;
				}
				array.Shuffle(ref seed);
				Prefab<DungeonGridCell>[] array6 = array;
				foreach (Prefab<DungeonGridCell> prefab7 in array6)
				{
					Prefab<DungeonGridCell> prefab8 = ((m > num) ? val.get_Item(m - 1, n) : null);
					if (((prefab8 != null) ? ((prefab7.Component.West == prefab8.Component.East) ? 1 : 0) : (dungeonGridConnectionHash3.West ? ((int)prefab7.Component.West) : ((prefab7.Component.West == DungeonGridConnectionType.None) ? 1 : 0))) == 0)
					{
						continue;
					}
					Prefab<DungeonGridCell> prefab9 = ((m < num2) ? val.get_Item(m + 1, n) : null);
					if (((prefab9 != null) ? ((prefab7.Component.East == prefab9.Component.West) ? 1 : 0) : (dungeonGridConnectionHash3.East ? ((int)prefab7.Component.East) : ((prefab7.Component.East == DungeonGridConnectionType.None) ? 1 : 0))) == 0)
					{
						continue;
					}
					Prefab<DungeonGridCell> prefab10 = ((n > num) ? val.get_Item(m, n - 1) : null);
					if (((prefab10 != null) ? ((prefab7.Component.South == prefab10.Component.North) ? 1 : 0) : (dungeonGridConnectionHash3.South ? ((int)prefab7.Component.South) : ((prefab7.Component.South == DungeonGridConnectionType.None) ? 1 : 0))) == 0)
					{
						continue;
					}
					Prefab<DungeonGridCell> prefab11 = ((n < num2) ? val.get_Item(m, n + 1) : null);
					if (((prefab11 != null) ? (prefab7.Component.North == prefab11.Component.South) : (dungeonGridConnectionHash3.North ? ((byte)prefab7.Component.North != 0) : (prefab7.Component.North == DungeonGridConnectionType.None))) && (prefab7.Component.West == DungeonGridConnectionType.None || prefab8 == null || !prefab7.Component.ShouldAvoid(prefab8.ID)) && (prefab7.Component.East == DungeonGridConnectionType.None || prefab9 == null || !prefab7.Component.ShouldAvoid(prefab9.ID)) && (prefab7.Component.South == DungeonGridConnectionType.None || prefab10 == null || !prefab7.Component.ShouldAvoid(prefab10.ID)) && (prefab7.Component.North == DungeonGridConnectionType.None || prefab11 == null || !prefab7.Component.ShouldAvoid(prefab11.ID)))
					{
						val.set_Item(m, n, prefab7);
						bool num5 = prefab8 == null || prefab7.Component.WestVariant == prefab8.Component.EastVariant;
						bool flag = prefab10 == null || prefab7.Component.SouthVariant == prefab10.Component.NorthVariant;
						if (num5 && flag)
						{
							break;
						}
					}
				}
			}
		}
		Vector3 zero2 = Vector3.get_zero();
		Vector3 zero3 = Vector3.get_zero();
		Vector3 val5 = Vector3.get_up() * 10f;
		Vector3 val6 = Vector3.get_up() * (LinkTransition + 1f);
		Vector2i val7 = default(Vector2i);
		do
		{
			zero3 = zero2;
			for (int num6 = 0; num6 < val.CellCount; num6++)
			{
				for (int num7 = 0; num7 < val.CellCount; num7++)
				{
					Prefab<DungeonGridCell> prefab12 = val.get_Item(num6, num7);
					if (prefab12 != null)
					{
						((Vector2i)(ref val7))._002Ector(num6, num7);
						Vector3 val8 = val.GridToWorldCoords(val7);
						while (!prefab12.CheckEnvironmentVolumesInsideTerrain(zero2 + val8 + val5, Quaternion.get_identity(), Vector3.get_one(), EnvironmentType.Underground) || prefab12.CheckEnvironmentVolumes(zero2 + val8 + val6, Quaternion.get_identity(), Vector3.get_one(), EnvironmentType.Underground | EnvironmentType.Building) || prefab12.CheckEnvironmentVolumes(zero2 + val8, Quaternion.get_identity(), Vector3.get_one(), EnvironmentType.Underground | EnvironmentType.Building))
						{
							zero2.y -= 9f;
						}
					}
				}
			}
		}
		while (zero2 != zero3);
		foreach (PathLink item4 in list3)
		{
			PathLinkSegment origin = item4.upwards.origin;
			origin.position += zero2;
		}
		Vector2i val9 = default(Vector2i);
		for (int num8 = 0; num8 < val.CellCount; num8++)
		{
			for (int num9 = 0; num9 < val.CellCount; num9++)
			{
				Prefab<DungeonGridCell> prefab13 = val.get_Item(num8, num9);
				if (prefab13 != null)
				{
					((Vector2i)(ref val9))._002Ector(num8, num9);
					Vector3 val10 = val.GridToWorldCoords(val9);
					World.AddPrefab("Dungeon", prefab13, zero2 + val10, Quaternion.get_identity(), Vector3.get_one());
				}
			}
		}
		Vector2i val11 = default(Vector2i);
		Vector2i val13 = default(Vector2i);
		for (int num10 = 0; num10 < val.CellCount - 1; num10++)
		{
			for (int num11 = 0; num11 < val.CellCount - 1; num11++)
			{
				Prefab<DungeonGridCell> prefab14 = val.get_Item(num10, num11);
				Prefab<DungeonGridCell> prefab15 = val.get_Item(num10 + 1, num11);
				Prefab<DungeonGridCell> prefab16 = val.get_Item(num10, num11 + 1);
				Prefab<DungeonGridCell>[] array6;
				if (prefab14 != null && prefab15 != null && prefab14.Component.EastVariant != prefab15.Component.WestVariant)
				{
					array3.Shuffle(ref seed);
					array6 = array3;
					foreach (Prefab<DungeonGridCell> prefab17 in array6)
					{
						if (prefab17.Component.West == prefab14.Component.East && prefab17.Component.East == prefab15.Component.West && prefab17.Component.WestVariant == prefab14.Component.EastVariant && prefab17.Component.EastVariant == prefab15.Component.WestVariant)
						{
							((Vector2i)(ref val11))._002Ector(num10, num11);
							Vector3 val12 = val.GridToWorldCoords(val11) + new Vector3(val.CellSizeHalf, 0f, 0f);
							World.AddPrefab("Dungeon", prefab17, zero2 + val12, Quaternion.get_identity(), Vector3.get_one());
							break;
						}
					}
				}
				if (prefab14 == null || prefab16 == null || prefab14.Component.NorthVariant == prefab16.Component.SouthVariant)
				{
					continue;
				}
				array3.Shuffle(ref seed);
				array6 = array3;
				foreach (Prefab<DungeonGridCell> prefab18 in array6)
				{
					if (prefab18.Component.South == prefab14.Component.North && prefab18.Component.North == prefab16.Component.South && prefab18.Component.SouthVariant == prefab14.Component.NorthVariant && prefab18.Component.NorthVariant == prefab16.Component.SouthVariant)
					{
						((Vector2i)(ref val13))._002Ector(num10, num11);
						Vector3 val14 = val.GridToWorldCoords(val13) + new Vector3(0f, 0f, val.CellSizeHalf);
						World.AddPrefab("Dungeon", prefab18, zero2 + val14, Quaternion.get_identity(), Vector3.get_one());
						break;
					}
				}
			}
		}
		foreach (PathLink item5 in list3)
		{
			Vector3 val15 = item5.upwards.origin.position + item5.upwards.origin.rotation * Vector3.Scale(item5.upwards.origin.upSocket.get_localPosition(), item5.upwards.origin.scale);
			Vector3 val16 = item5.downwards.origin.position + item5.downwards.origin.rotation * Vector3.Scale(item5.downwards.origin.downSocket.get_localPosition(), item5.downwards.origin.scale) - val15;
			Vector3[] array7 = (Vector3[])(object)new Vector3[2]
			{
				new Vector3(0f, 1f, 0f),
				new Vector3(1f, 1f, 1f)
			};
			foreach (Vector3 val17 in array7)
			{
				int num12 = 0;
				int num13 = 0;
				while (((Vector3)(ref val16)).get_magnitude() > 1f && (num12 < 8 || num13 < 8))
				{
					bool flag2 = num12 > 2 && num13 > 2;
					bool flag3 = num12 > 4 && num13 > 4;
					Prefab<DungeonGridLink> prefab19 = null;
					Vector3 val18 = Vector3.get_zero();
					int num14 = int.MinValue;
					Vector3 position3 = Vector3.get_zero();
					Quaternion rotation2 = Quaternion.get_identity();
					PathLinkSegment prevSegment = item5.downwards.prevSegment;
					Vector3 val19 = prevSegment.position + prevSegment.rotation * Vector3.Scale(prevSegment.scale, prevSegment.downSocket.get_localPosition());
					Quaternion val20 = prevSegment.rotation * prevSegment.downSocket.get_localRotation();
					Prefab<DungeonGridLink>[] array8 = array4;
					foreach (Prefab<DungeonGridLink> prefab20 in array8)
					{
						float num15 = SeedRandom.Value(ref seed);
						DungeonGridLink component = prefab20.Component;
						if (prevSegment.downType != component.UpType)
						{
							continue;
						}
						switch (component.DownType)
						{
						case DungeonGridLinkType.Elevator:
							if (flag2 || val17.x != 0f || val17.z != 0f)
							{
								continue;
							}
							break;
						case DungeonGridLinkType.Transition:
							if (val17.x != 0f || val17.z != 0f)
							{
								continue;
							}
							break;
						}
						int num16 = ((!flag2) ? component.Priority : 0);
						if (num14 > num16)
						{
							continue;
						}
						Quaternion val21 = val20 * Quaternion.Inverse(component.UpSocket.get_localRotation());
						Quaternion val22 = val21 * component.DownSocket.get_localRotation();
						PathLinkSegment prevSegment2 = item5.upwards.prevSegment;
						Quaternion val23 = prevSegment2.rotation * prevSegment2.upSocket.get_localRotation();
						if (component.Rotation > 0)
						{
							if (Quaternion.Angle(val23, val22) > (float)component.Rotation)
							{
								continue;
							}
							Quaternion val24 = val23 * Quaternion.Inverse(val22);
							val21 *= val24;
							val22 *= val24;
						}
						Vector3 val25 = val19 - val21 * component.UpSocket.get_localPosition();
						Vector3 val26 = val21 * (component.DownSocket.get_localPosition() - component.UpSocket.get_localPosition());
						Vector3 val27 = val16 + val18;
						Vector3 val28 = val16 + val26;
						float magnitude = ((Vector3)(ref val27)).get_magnitude();
						float magnitude2 = ((Vector3)(ref val28)).get_magnitude();
						Vector3 val29 = Vector3.Scale(val27, val17);
						Vector3 val30 = Vector3.Scale(val28, val17);
						float magnitude3 = ((Vector3)(ref val29)).get_magnitude();
						float magnitude4 = ((Vector3)(ref val30)).get_magnitude();
						if (val18 != Vector3.get_zero())
						{
							if (magnitude3 < magnitude4 || (magnitude3 == magnitude4 && magnitude < magnitude2) || (magnitude3 == magnitude4 && magnitude == magnitude2 && num15 < 0.5f))
							{
								continue;
							}
						}
						else if (magnitude3 <= magnitude4)
						{
							continue;
						}
						if (Mathf.Abs(val30.x) - Mathf.Abs(val29.x) > 0.01f || (Mathf.Abs(val30.x) > 0.01f && val27.x * val28.x < 0f) || Mathf.Abs(val30.y) - Mathf.Abs(val29.y) > 0.01f || (Mathf.Abs(val30.y) > 0.01f && val27.y * val28.y < 0f) || Mathf.Abs(val30.z) - Mathf.Abs(val29.z) > 0.01f || (Mathf.Abs(val30.z) > 0.01f && val27.z * val28.z < 0f) || (flag2 && val17.x == 0f && val17.z == 0f && component.DownType == DungeonGridLinkType.Default && ((Mathf.Abs(val28.x) > 0.01f && Mathf.Abs(val28.x) < LinkRadius * 2f - 0.1f) || (Mathf.Abs(val28.z) > 0.01f && Mathf.Abs(val28.z) < LinkRadius * 2f - 0.1f))))
						{
							continue;
						}
						num14 = num16;
						if (val17.x == 0f && val17.z == 0f)
						{
							if (!flag2 && Mathf.Abs(val28.y) < LinkTransition - 0.1f)
							{
								continue;
							}
						}
						else if ((!flag2 && magnitude4 > 0.01f && (Mathf.Abs(val28.x) < LinkRadius * 2f - 0.1f || Mathf.Abs(val28.z) < LinkRadius * 2f - 0.1f)) || (!flag3 && magnitude4 > 0.01f && (Mathf.Abs(val28.x) < LinkRadius * 1f - 0.1f || Mathf.Abs(val28.z) < LinkRadius * 1f - 0.1f)))
						{
							continue;
						}
						if (!flag2 || !(magnitude4 < 0.01f) || !(magnitude2 < 0.01f) || !(Quaternion.Angle(val23, val22) > 10f))
						{
							prefab19 = prefab20;
							val18 = val26;
							num14 = num16;
							position3 = val25;
							rotation2 = val21;
						}
					}
					if (val18 != Vector3.get_zero())
					{
						PathLinkSegment pathLinkSegment = new PathLinkSegment();
						pathLinkSegment.position = position3;
						pathLinkSegment.rotation = rotation2;
						pathLinkSegment.scale = Vector3.get_one();
						pathLinkSegment.prefab = prefab19;
						pathLinkSegment.link = prefab19.Component;
						item5.downwards.segments.Add(pathLinkSegment);
						val16 += val18;
					}
					else
					{
						num13++;
					}
					if (val17.x > 0f || val17.z > 0f)
					{
						Prefab<DungeonGridLink> prefab21 = null;
						Vector3 val31 = Vector3.get_zero();
						int num17 = int.MinValue;
						Vector3 position4 = Vector3.get_zero();
						Quaternion rotation3 = Quaternion.get_identity();
						PathLinkSegment prevSegment3 = item5.upwards.prevSegment;
						Vector3 val32 = prevSegment3.position + prevSegment3.rotation * Vector3.Scale(prevSegment3.scale, prevSegment3.upSocket.get_localPosition());
						Quaternion val33 = prevSegment3.rotation * prevSegment3.upSocket.get_localRotation();
						array8 = array4;
						foreach (Prefab<DungeonGridLink> prefab22 in array8)
						{
							float num18 = SeedRandom.Value(ref seed);
							DungeonGridLink component2 = prefab22.Component;
							if (prevSegment3.upType != component2.DownType)
							{
								continue;
							}
							switch (component2.DownType)
							{
							case DungeonGridLinkType.Elevator:
								if (flag2 || val17.x != 0f || val17.z != 0f)
								{
									continue;
								}
								break;
							case DungeonGridLinkType.Transition:
								if (val17.x != 0f || val17.z != 0f)
								{
									continue;
								}
								break;
							}
							int num19 = ((!flag2) ? component2.Priority : 0);
							if (num17 > num19)
							{
								continue;
							}
							Quaternion val34 = val33 * Quaternion.Inverse(component2.DownSocket.get_localRotation());
							Quaternion val35 = val34 * component2.UpSocket.get_localRotation();
							PathLinkSegment prevSegment4 = item5.downwards.prevSegment;
							Quaternion val36 = prevSegment4.rotation * prevSegment4.downSocket.get_localRotation();
							if (component2.Rotation > 0)
							{
								if (Quaternion.Angle(val36, val35) > (float)component2.Rotation)
								{
									continue;
								}
								Quaternion val37 = val36 * Quaternion.Inverse(val35);
								val34 *= val37;
								val35 *= val37;
							}
							Vector3 val38 = val32 - val34 * component2.DownSocket.get_localPosition();
							Vector3 val39 = val34 * (component2.UpSocket.get_localPosition() - component2.DownSocket.get_localPosition());
							Vector3 val40 = val16 - val31;
							Vector3 val41 = val16 - val39;
							float magnitude5 = ((Vector3)(ref val40)).get_magnitude();
							float magnitude6 = ((Vector3)(ref val41)).get_magnitude();
							Vector3 val42 = Vector3.Scale(val40, val17);
							Vector3 val43 = Vector3.Scale(val41, val17);
							float magnitude7 = ((Vector3)(ref val42)).get_magnitude();
							float magnitude8 = ((Vector3)(ref val43)).get_magnitude();
							if (val31 != Vector3.get_zero())
							{
								if (magnitude7 < magnitude8 || (magnitude7 == magnitude8 && magnitude5 < magnitude6) || (magnitude7 == magnitude8 && magnitude5 == magnitude6 && num18 < 0.5f))
								{
									continue;
								}
							}
							else if (magnitude7 <= magnitude8)
							{
								continue;
							}
							if (Mathf.Abs(val43.x) - Mathf.Abs(val42.x) > 0.01f || (Mathf.Abs(val43.x) > 0.01f && val40.x * val41.x < 0f) || Mathf.Abs(val43.y) - Mathf.Abs(val42.y) > 0.01f || (Mathf.Abs(val43.y) > 0.01f && val40.y * val41.y < 0f) || Mathf.Abs(val43.z) - Mathf.Abs(val42.z) > 0.01f || (Mathf.Abs(val43.z) > 0.01f && val40.z * val41.z < 0f) || (flag2 && val17.x == 0f && val17.z == 0f && component2.UpType == DungeonGridLinkType.Default && ((Mathf.Abs(val41.x) > 0.01f && Mathf.Abs(val41.x) < LinkRadius * 2f - 0.1f) || (Mathf.Abs(val41.z) > 0.01f && Mathf.Abs(val41.z) < LinkRadius * 2f - 0.1f))))
							{
								continue;
							}
							num17 = num19;
							if (val17.x == 0f && val17.z == 0f)
							{
								if (!flag2 && Mathf.Abs(val41.y) < LinkTransition - 0.1f)
								{
									continue;
								}
							}
							else if ((!flag2 && magnitude8 > 0.01f && (Mathf.Abs(val41.x) < LinkRadius * 2f - 0.1f || Mathf.Abs(val41.z) < LinkRadius * 2f - 0.1f)) || (!flag3 && magnitude8 > 0.01f && (Mathf.Abs(val41.x) < LinkRadius * 1f - 0.1f || Mathf.Abs(val41.z) < LinkRadius * 1f - 0.1f)))
							{
								continue;
							}
							if (!flag2 || !(magnitude8 < 0.01f) || !(magnitude6 < 0.01f) || !(Quaternion.Angle(val36, val35) > 10f))
							{
								prefab21 = prefab22;
								val31 = val39;
								num17 = num19;
								position4 = val38;
								rotation3 = val34;
							}
						}
						if (val31 != Vector3.get_zero())
						{
							PathLinkSegment pathLinkSegment2 = new PathLinkSegment();
							pathLinkSegment2.position = position4;
							pathLinkSegment2.rotation = rotation3;
							pathLinkSegment2.scale = Vector3.get_one();
							pathLinkSegment2.prefab = prefab21;
							pathLinkSegment2.link = prefab21.Component;
							item5.upwards.segments.Add(pathLinkSegment2);
							val16 -= val31;
						}
						else
						{
							num12++;
						}
					}
					else
					{
						num12++;
					}
				}
			}
		}
		foreach (PathLink item6 in list3)
		{
			foreach (PathLinkSegment segment2 in item6.downwards.segments)
			{
				World.AddPrefab("Dungeon", segment2.prefab, segment2.position, segment2.rotation, segment2.scale);
			}
			foreach (PathLinkSegment segment3 in item6.upwards.segments)
			{
				World.AddPrefab("Dungeon", segment3.prefab, segment3.position, segment3.rotation, segment3.scale);
			}
		}
		if (Object.op_Implicit((Object)(object)TerrainMeta.Path))
		{
			TerrainMeta.Path.DungeonGridRoot = HierarchyUtil.GetRoot("Dungeon");
		}
	}
}
