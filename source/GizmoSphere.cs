using UnityEngine;

[ExecuteInEditMode]
public class GizmoSphere : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position, 0.1f);
	}
}
