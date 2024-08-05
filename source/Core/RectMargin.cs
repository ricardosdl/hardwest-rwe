using UnityEngine;

namespace Core;

public struct RectMargin
{
	public int top;

	public int bottom;

	public int left;

	public int right;

	public int vertical => top + bottom;

	public int horizontal => left + right;

	public RectMargin(RectOffset offset)
	{
		top = offset.top;
		bottom = offset.bottom;
		left = offset.left;
		right = offset.right;
	}

	public Rect Add(Rect rect)
	{
		rect.x -= left;
		rect.y -= top;
		rect.width += horizontal;
		rect.height += vertical;
		return rect;
	}

	public Rect Sub(Rect rect)
	{
		rect.x += left;
		rect.width = Mathf.Max(0f, rect.width - (float)horizontal);
		rect.y += top;
		rect.height = Mathf.Max(0f, rect.height - (float)vertical);
		return rect;
	}

	public override string ToString()
	{
		return $"RectMargin (l:{left} r:{right} t:{top} b:{bottom})";
	}
}
