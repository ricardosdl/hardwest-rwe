using UnityEngine;

public class CFGRicochetObject : CFGGameObject
{
	[SerializeField]
	private float m_Angle = 360f;

	[SerializeField]
	private float m_ColliderScale = 1f;

	public float Angle => m_Angle;

	public float ColliderScale => m_ColliderScale;

	public CFGCell Cell => CFGCellMap.GetCell(base.transform.position);

	protected override void Start()
	{
		base.Start();
		if (CFGOptions.Gameplay.InteractiveObjectsGlow >= 1)
		{
			base.gameObject.AddComponent<CFGRicochetObjVis>();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		UpdateCell();
	}

	public void UpdateCell()
	{
	}
}
