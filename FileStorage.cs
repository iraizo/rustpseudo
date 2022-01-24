using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Sqlite;
using Ionic.Crc;
using ProtoBuf;
using UnityEngine.Assertions;

public class FileStorage : IDisposable
{
	private class CacheData
	{
		public byte[] data;

		public uint entityID;

		public uint numID;
	}

	public enum Type
	{
		png,
		jpg,
		ogg
	}

	private class FileDatabase : Database
	{
		public IEnumerable<AssociatedFile> QueryAll(uint entityID)
		{
			IntPtr intPtr = ((Database)this).Prepare("SELECT filetype, crc, part, data FROM data WHERE entid = ?");
			Database.Bind<uint>(intPtr, 1, entityID);
			return ((Database)this).ExecuteAndReadQueryResults<AssociatedFile>(intPtr, (Func<IntPtr, AssociatedFile>)ReadAssociatedFileRow, true);
		}

		private static AssociatedFile ReadAssociatedFileRow(IntPtr stmHandle)
		{
			AssociatedFile obj = Pool.Get<AssociatedFile>();
			obj.type = Database.GetColumnValue<int>(stmHandle, 0);
			obj.crc = (uint)Database.GetColumnValue<int>(stmHandle, 1);
			obj.numID = (uint)Database.GetColumnValue<int>(stmHandle, 2);
			obj.data = Database.GetColumnValue<byte[]>(stmHandle, 3);
			return obj;
		}

		public FileDatabase()
			: this()
		{
		}
	}

	private FileDatabase db;

	private CRC32 crc = new CRC32();

	private MruDictionary<uint, CacheData> _cache = new MruDictionary<uint, CacheData>(1000, (Action<uint, CacheData>)null);

	public static FileStorage server = new FileStorage("sv.files." + 220, server: true);

	protected FileStorage(string name, bool server)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		if (server)
		{
			string text = Server.rootFolder + "/" + name + ".db";
			db = new FileDatabase();
			((Database)db).Open(text, true);
			if (!((Database)db).TableExists("data"))
			{
				((Database)db).Execute("CREATE TABLE data ( crc INTEGER PRIMARY KEY, data BLOB, updated INTEGER, entid INTEGER, filetype INTEGER, part INTEGER )");
				((Database)db).Execute("CREATE INDEX IF NOT EXISTS entindex ON data ( entid )");
			}
		}
	}

	~FileStorage()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (db != null)
		{
			((Database)db).Close();
			db = null;
		}
	}

	private uint GetCRC(byte[] data, Type type)
	{
		TimeWarning val = TimeWarning.New("FileStorage.GetCRC", 0);
		try
		{
			crc.Reset();
			crc.SlurpBlock(data, 0, data.Length);
			crc.UpdateCRC((byte)type);
			return (uint)crc.get_Crc32Result();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public uint Store(byte[] data, Type type, uint entityID, uint numID = 0u)
	{
		TimeWarning val = TimeWarning.New("FileStorage.Store", 0);
		try
		{
			uint cRC = GetCRC(data, type);
			if (db != null)
			{
				((Database)db).Execute<int, byte[], int, int, int>("INSERT OR REPLACE INTO data ( crc, data, entid, filetype, part ) VALUES ( ?, ?, ?, ?, ? )", (int)cRC, data, (int)entityID, (int)type, (int)numID);
			}
			_cache.Remove(cRC);
			_cache.Add(cRC, new CacheData
			{
				data = data,
				entityID = entityID,
				numID = numID
			});
			return cRC;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public byte[] Get(uint crc, Type type, uint entityID, uint numID = 0u)
	{
		TimeWarning val = TimeWarning.New("FileStorage.Get", 0);
		try
		{
			CacheData cacheData = default(CacheData);
			if (_cache.TryGetValue(crc, ref cacheData))
			{
				Assert.IsTrue(cacheData.data != null, "FileStorage cache contains a null texture");
				return cacheData.data;
			}
			if (db == null)
			{
				return null;
			}
			byte[] array = ((Database)db).QueryBlob<int, int, int, int>("SELECT data FROM data WHERE crc = ? AND filetype = ? AND entid = ? AND part = ? LIMIT 1", (int)crc, (int)type, (int)entityID, (int)numID);
			if (array == null)
			{
				return null;
			}
			_cache.Remove(crc);
			_cache.Add(crc, new CacheData
			{
				data = array,
				entityID = entityID,
				numID = 0u
			});
			return array;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void Remove(uint crc, Type type, uint entityID)
	{
		TimeWarning val = TimeWarning.New("FileStorage.Remove", 0);
		try
		{
			if (db != null)
			{
				((Database)db).Execute<int, int, int>("DELETE FROM data WHERE crc = ? AND filetype = ? AND entid = ?", (int)crc, (int)type, (int)entityID);
			}
			_cache.Remove(crc);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void RemoveExact(uint crc, Type type, uint entityID, uint numid)
	{
		TimeWarning val = TimeWarning.New("FileStorage.RemoveExact", 0);
		try
		{
			if (db != null)
			{
				((Database)db).Execute<int, int, int, int>("DELETE FROM data WHERE crc = ? AND filetype = ? AND entid = ? AND part = ?", (int)crc, (int)type, (int)entityID, (int)numid);
			}
			_cache.Remove(crc);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void RemoveEntityNum(uint entityid, uint numid)
	{
		TimeWarning val = TimeWarning.New("FileStorage.RemoveEntityNum", 0);
		try
		{
			if (db != null)
			{
				((Database)db).Execute<int, int>("DELETE FROM data WHERE entid = ? AND part = ?", (int)entityid, (int)numid);
			}
			uint[] array = Enumerable.ToArray<uint>(Enumerable.Select<KeyValuePair<uint, CacheData>, uint>(Enumerable.Where<KeyValuePair<uint, CacheData>>((IEnumerable<KeyValuePair<uint, CacheData>>)_cache, (Func<KeyValuePair<uint, CacheData>, bool>)((KeyValuePair<uint, CacheData> x) => x.Value.entityID == entityid && x.Value.numID == numid)), (Func<KeyValuePair<uint, CacheData>, uint>)((KeyValuePair<uint, CacheData> x) => x.Key)));
			foreach (uint num in array)
			{
				_cache.Remove(num);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	internal void RemoveAllByEntity(uint entityid)
	{
		TimeWarning val = TimeWarning.New("FileStorage.RemoveAllByEntity", 0);
		try
		{
			if (db != null)
			{
				((Database)db).Execute<int>("DELETE FROM data WHERE entid = ?", (int)entityid);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void ReassignEntityId(uint oldId, uint newId)
	{
		TimeWarning val = TimeWarning.New("FileStorage.ReassignEntityId", 0);
		try
		{
			if (db != null)
			{
				((Database)db).Execute<int, int>("UPDATE data SET entid = ? WHERE entid = ?", (int)newId, (int)oldId);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public IEnumerable<AssociatedFile> QueryAllByEntity(uint entityID)
	{
		return db.QueryAll(entityID);
	}
}
