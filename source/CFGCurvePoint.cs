using UnityEngine;

public class CFGCurvePoint : MonoBehaviour
{
	public void Main()
	{
	}

	public void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "Aperture_CurvePoint.tiff");
	}
}
