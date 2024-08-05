using UnityEngine;

public class TankWeaponController : MonoBehaviour
{
	public TankProjectile ProjectilePrefab;

	public Transform Nozzle;

	private void Update()
	{
		if (!GetComponent<Animation>().isPlaying && Input.GetKeyDown(KeyCode.Space))
		{
			GetComponent<Animation>().Play();
			Object.Instantiate(ProjectilePrefab, Nozzle.position, Nozzle.rotation);
		}
	}
}
