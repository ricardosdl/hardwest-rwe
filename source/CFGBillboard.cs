using UnityEngine;

public class CFGBillboard : MonoBehaviour
{
	public void LateUpdate()
	{
		Transform transform = Camera.main.transform;
		if (transform != null)
		{
			base.transform.LookAt(transform.position, transform.up);
		}
	}
}
