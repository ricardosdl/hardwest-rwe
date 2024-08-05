using UnityEngine;

public static class EDirectionExtention
{
	public static ECornerDirection GetCornerDirection(this EDirection dir, bool prev)
	{
		return dir switch
		{
			EDirection.SOUTH => prev ? ECornerDirection.SE : ECornerDirection.SW, 
			EDirection.WEST => (!prev) ? ECornerDirection.NW : ECornerDirection.SW, 
			EDirection.EAST => (!prev) ? ECornerDirection.SE : ECornerDirection.NE, 
			_ => prev ? ECornerDirection.NW : ECornerDirection.NE, 
		};
	}

	public static EDirection Opposite(this EDirection dir)
	{
		return dir switch
		{
			EDirection.SOUTH => EDirection.NORTH, 
			EDirection.WEST => EDirection.EAST, 
			EDirection.NORTH => EDirection.SOUTH, 
			_ => EDirection.WEST, 
		};
	}

	public static Vector3 GetForward(this EDirection dir)
	{
		return dir switch
		{
			EDirection.NORTH => new Vector3(-1f, 0f, 0f), 
			EDirection.EAST => new Vector3(0f, 0f, 1f), 
			EDirection.WEST => new Vector3(0f, 0f, -1f), 
			EDirection.SOUTH => new Vector3(1f, 0f, 0f), 
			_ => Vector3.zero, 
		};
	}

	public static bool IsEW(this EDirection dir)
	{
		return dir switch
		{
			EDirection.WEST => true, 
			EDirection.EAST => true, 
			_ => false, 
		};
	}

	public static EDirectionDistance GetDistance(this EDirection from, EDirection to)
	{
		return (to - from) switch
		{
			0 => EDirectionDistance.SAME, 
			1 => EDirectionDistance.CLOCKWISE, 
			2 => EDirectionDistance.OPPOSITE, 
			-1 => EDirectionDistance.COUNTERCLOCKWISE, 
			-3 => EDirectionDistance.CLOCKWISE, 
			3 => EDirectionDistance.COUNTERCLOCKWISE, 
			-2 => EDirectionDistance.OPPOSITE, 
			_ => EDirectionDistance.SAME, 
		};
	}

	public static EDirection GetDir_Clockwise(this EDirection dir)
	{
		return dir switch
		{
			EDirection.EAST => EDirection.SOUTH, 
			EDirection.SOUTH => EDirection.WEST, 
			EDirection.WEST => EDirection.NORTH, 
			_ => EDirection.EAST, 
		};
	}

	public static EDirection GetDir_CounterClockwise(this EDirection dir)
	{
		return dir switch
		{
			EDirection.EAST => EDirection.NORTH, 
			EDirection.SOUTH => EDirection.EAST, 
			EDirection.WEST => EDirection.SOUTH, 
			_ => EDirection.WEST, 
		};
	}

	public static EDirection GetDirection(this EDirection dir, EDirectionDistance distance)
	{
		int num = 0;
		switch (distance)
		{
		case EDirectionDistance.COUNTERCLOCKWISE:
			num = -1;
			break;
		case EDirectionDistance.CLOCKWISE:
			num = 1;
			break;
		case EDirectionDistance.OPPOSITE:
			num = 2;
			break;
		case EDirectionDistance.SAME:
			num = 0;
			break;
		}
		return (int)(dir + num) switch
		{
			0 => EDirection.NORTH, 
			1 => EDirection.EAST, 
			2 => EDirection.SOUTH, 
			3 => EDirection.WEST, 
			4 => EDirection.NORTH, 
			5 => EDirection.EAST, 
			-1 => EDirection.WEST, 
			_ => EDirection.NORTH, 
		};
	}

	public static EDirection GetClosestDirection(Vector3 dir)
	{
		EDirection result = EDirection.NORTH;
		float num = -1f;
		for (int i = 0; i < 4; i++)
		{
			EDirection eDirection = (EDirection)i;
			float num2 = Vector3.Dot(eDirection.GetForward(), dir);
			if (num2 > num)
			{
				result = eDirection;
				num = num2;
			}
		}
		return result;
	}
}
