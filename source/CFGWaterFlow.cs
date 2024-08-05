using UnityEngine;

public class CFGWaterFlow : MonoBehaviour
{
	public float Cycle = 0.15f;

	public float FlowSpeed = 0.5f;

	private float HalfCycle;

	private float FlowMapOffset0;

	private float FlowMapOffset1;

	private void Start()
	{
		HalfCycle = Cycle * 0.5f;
		FlowMapOffset0 = 0f;
		FlowMapOffset1 = HalfCycle;
		GetComponent<Renderer>().sharedMaterial.SetFloat("_HalfCycle", HalfCycle);
		GetComponent<Renderer>().sharedMaterial.SetFloat("_FlowOffset0", FlowMapOffset0);
		GetComponent<Renderer>().sharedMaterial.SetFloat("_FlowOffset1", FlowMapOffset1);
	}

	private void Update()
	{
		FlowMapOffset0 += FlowSpeed * Time.deltaTime;
		FlowMapOffset1 += FlowSpeed * Time.deltaTime;
		if (FlowMapOffset0 >= Cycle)
		{
			FlowMapOffset0 = 0f;
		}
		if (FlowMapOffset1 >= Cycle)
		{
			FlowMapOffset1 = 0f;
		}
		GetComponent<Renderer>().sharedMaterial.SetFloat("_FlowOffset0", FlowMapOffset0);
		GetComponent<Renderer>().sharedMaterial.SetFloat("_FlowOffset1", FlowMapOffset1);
	}
}
