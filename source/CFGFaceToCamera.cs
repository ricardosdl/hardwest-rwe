using UnityEngine;

public class CFGFaceToCamera : MonoBehaviour
{
	private void Start()
	{
		RotateToCam();
	}

	private void Update()
	{
		RotateToCam();
	}

	private void RotateToCam()
	{
		if ((bool)Camera.main)
		{
			Vector3 worldPosition = new Vector3(Camera.main.transform.position.x, base.transform.position.y, Camera.main.transform.position.z);
			base.transform.LookAt(worldPosition);
		}
	}
}
