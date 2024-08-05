using UnityEngine;

public class CFGCurveGo : MonoBehaviour
{
	public bool look;

	public CFGCurve c;

	public float position;

	private float look_pos;

	public float speed = 1f;

	private void UpdatePosition()
	{
		if (!c)
		{
			return;
		}
		position += Time.fixedDeltaTime * speed;
		if (position < 0f)
		{
			position = 0f;
		}
		if (position >= 1f)
		{
			position = 0f;
		}
		Vector3 vector = c.GetPosition(position);
		if (base.transform.position != vector)
		{
			base.transform.position = vector;
		}
		if (look)
		{
			look_pos = position + Time.fixedDeltaTime * speed;
			if (look_pos >= 1f)
			{
				look_pos = 0f + (look_pos - 1f);
			}
			base.transform.LookAt(c.GetPosition(look_pos));
		}
	}

	private void Update()
	{
		UpdatePosition();
	}
}
