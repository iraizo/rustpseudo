using System.IO;
using Network;
using SilentOrbit.ProtocolBuffers;
using UnityEngine;

public static class NetworkWriteEx
{
	public static void WriteObject<T>(this NetWrite write, T obj)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		if (typeof(T) == typeof(Vector3))
		{
			Vector3 val = GenericsUtil.Cast<T, Vector3>(obj);
			write.Vector3(ref val);
			return;
		}
		if (typeof(T) == typeof(Ray))
		{
			Ray val2 = GenericsUtil.Cast<T, Ray>(obj);
			write.Ray(ref val2);
			return;
		}
		if (typeof(T) == typeof(float))
		{
			write.Float(GenericsUtil.Cast<T, float>(obj));
			return;
		}
		if (typeof(T) == typeof(short))
		{
			write.Int16(GenericsUtil.Cast<T, short>(obj));
			return;
		}
		if (typeof(T) == typeof(ushort))
		{
			write.UInt16(GenericsUtil.Cast<T, ushort>(obj));
			return;
		}
		if (typeof(T) == typeof(int))
		{
			write.Int32(GenericsUtil.Cast<T, int>(obj));
			return;
		}
		if (typeof(T) == typeof(uint))
		{
			write.UInt32(GenericsUtil.Cast<T, uint>(obj));
			return;
		}
		if (typeof(T) == typeof(byte[]))
		{
			write.Bytes(GenericsUtil.Cast<T, byte[]>(obj));
			return;
		}
		if (typeof(T) == typeof(long))
		{
			write.Int64(GenericsUtil.Cast<T, long>(obj));
			return;
		}
		if (typeof(T) == typeof(ulong))
		{
			write.UInt64(GenericsUtil.Cast<T, ulong>(obj));
			return;
		}
		if (typeof(T) == typeof(string))
		{
			write.String(GenericsUtil.Cast<T, string>(obj));
			return;
		}
		if (typeof(T) == typeof(sbyte))
		{
			write.Int8(GenericsUtil.Cast<T, sbyte>(obj));
			return;
		}
		if (typeof(T) == typeof(byte))
		{
			write.UInt8(GenericsUtil.Cast<T, byte>(obj));
			return;
		}
		if (typeof(T) == typeof(bool))
		{
			write.Bool(GenericsUtil.Cast<T, bool>(obj));
			return;
		}
		if (typeof(T) == typeof(Color))
		{
			Color val3 = GenericsUtil.Cast<T, Color>(obj);
			write.Color(ref val3);
			return;
		}
		object obj2 = obj;
		IProto val4;
		if ((val4 = (IProto)((obj2 is IProto) ? obj2 : null)) != null)
		{
			val4.WriteToStream((Stream)(object)write);
			return;
		}
		Debug.LogError((object)string.Concat("NetworkData.Write - no handler to write ", obj, " -> ", obj.GetType()));
	}
}
