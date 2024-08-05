using System;
using System.Globalization;
using UnityEngine;

[Serializable]
public class CFGGUID
{
	[SerializeField]
	public int ValA;

	[SerializeField]
	public int ValB;

	[SerializeField]
	public int ValC;

	[SerializeField]
	public int ValD;

	public static int _GenerationIndex = 1;

	public CFGGUID()
	{
	}

	public CFGGUID(int a, int b, int c, int d)
	{
		ValA = a;
		ValB = b;
		ValC = c;
		ValD = d;
	}

	public CFGGUID(CFGGUID other)
	{
		SetFrom(other);
	}

	public void Clear()
	{
		ValA = 0;
		ValB = 0;
		ValC = 0;
		ValD = 0;
	}

	public bool IsClear()
	{
		if (ValA != 0)
		{
			return false;
		}
		if (ValB != 0)
		{
			return false;
		}
		if (ValC != 0)
		{
			return false;
		}
		if (ValD != 0)
		{
			return false;
		}
		return true;
	}

	public void GenerateNew()
	{
		byte[] array = Guid.NewGuid().ToByteArray();
		ValA = array[0] + (array[1] << 8) + (array[2] << 16) + (array[3] << 24);
		ValB = array[4] + (array[5] << 8) + (array[6] << 16) + (array[7] << 24);
		ValC = array[8] + (array[9] << 8) + (array[10] << 16) + (array[11] << 24);
		ValD = array[12] + (array[13] << 8) + (array[14] << 16) + (array[15] << 24);
	}

	public void GenerateFast(int ObjectType)
	{
		ValA = _GenerationIndex++;
		long num = DateTime.Now.ToBinary();
		ValB = (int)(num & 0xFFFFFFFFu);
		ValC = (int)(num >> 32);
		ValD = ObjectType;
	}

	public void SetFrom(CFGGUID Other)
	{
		ValA = Other.ValA;
		ValB = Other.ValB;
		ValC = Other.ValC;
		ValD = Other.ValD;
	}

	public void Set(int va, int vb, int vc, int vd)
	{
		ValA = va;
		ValB = vb;
		ValC = vc;
		ValD = vd;
	}

	public bool IsEqualTo(CFGGUID other)
	{
		if (other == null)
		{
			return false;
		}
		if (ValA != other.ValA)
		{
			return false;
		}
		if (ValB != other.ValB)
		{
			return false;
		}
		if (ValC != other.ValC)
		{
			return false;
		}
		if (ValD != other.ValD)
		{
			return false;
		}
		return true;
	}

	public override string ToString()
	{
		return $"{ValA:X},{ValB:X},{ValC:X},{ValD:X}";
	}

	public void FromString(string strSource)
	{
		string[] array = strSource.Split(',');
		Clear();
		if (array.Length < 3)
		{
			return;
		}
		try
		{
			ValA = int.Parse(array[0], NumberStyles.AllowHexSpecifier);
			ValB = int.Parse(array[1], NumberStyles.AllowHexSpecifier);
			ValC = int.Parse(array[2], NumberStyles.AllowHexSpecifier);
			ValD = int.Parse(array[3], NumberStyles.AllowHexSpecifier);
		}
		catch
		{
		}
	}
}
