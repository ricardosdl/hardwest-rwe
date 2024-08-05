using UnityEngine;

public class CFGFXPhysicsExplosion : MonoBehaviour
{
	public float radius = 5f;

	public float power = 10f;

	public float fadeStart = 3f;

	public float fadeTime = 2f;

	public GameObject[] debrisList;

	private float timer;

	private float alpha;

	private Color color;

	private void Start()
	{
		Vector3 position = base.transform.position;
		GameObject[] array = debrisList;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject && (bool)gameObject.GetComponent<Rigidbody>())
			{
				gameObject.GetComponent<Rigidbody>().AddExplosionForce(power, position, radius, 3f);
			}
		}
		alpha = 0.1f / fadeTime;
	}

	private void FixedUpdate()
	{
		timer += Time.smoothDeltaTime;
		if (!(timer >= fadeStart))
		{
			return;
		}
		GameObject[] array = debrisList;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				color = gameObject.GetComponent<Renderer>().material.GetColor("_Color");
				color.a -= alpha;
				gameObject.GetComponent<Renderer>().material.SetColor("_Color", color);
			}
		}
		if (color.a <= 0f)
		{
			GameObject[] array2 = debrisList;
			foreach (GameObject obj in array2)
			{
				Object.Destroy(obj);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
