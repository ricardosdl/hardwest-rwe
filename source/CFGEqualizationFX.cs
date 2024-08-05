using UnityEngine;

public class CFGEqualizationFX : MonoBehaviour
{
	private void Start()
	{
		Transform transform = null;
		if ((bool)base.transform.parent && (bool)base.transform.parent.parent && (bool)base.transform.parent.parent.parent)
		{
			transform = base.transform.parent.parent.parent;
		}
		if ((bool)transform)
		{
			base.transform.parent = transform;
			base.transform.localPosition = Vector3.zero;
			base.transform.localEulerAngles = Vector3.zero;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
