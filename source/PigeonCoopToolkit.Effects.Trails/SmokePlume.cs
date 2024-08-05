using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails;

[AddComponentMenu("Pigeon Coop Toolkit/Effects/Smoke Plume")]
public class SmokePlume : TrailRenderer_Base
{
	public bool m_AffectedByWind;

	public float m_WindScale = 0.1f;

	public float TimeBetweenPoints = 0.1f;

	public Vector3 ConstantForce = Vector3.up * 0.5f;

	public float RandomForceScale = 0.05f;

	public int MaxNumberOfPoints = 50;

	private float _timeSincePoint;

	private CFGWind m_Wind;

	private Vector3 m_ConstantForceMod = Vector3.up * 0.5f;

	protected override void Start()
	{
		base.Start();
		_timeSincePoint = 0f;
		m_Wind = Object.FindObjectOfType<CFGWind>();
		SetConstantForce();
	}

	protected void SetConstantForce()
	{
		if ((bool)m_Wind)
		{
			m_ConstantForceMod = new Vector3(m_Wind.m_WindForce.x * m_WindScale + ConstantForce.x, ConstantForce.y, m_Wind.m_WindForce.y * m_WindScale + ConstantForce.z);
		}
	}

	protected override void OnStartEmit()
	{
		_timeSincePoint = 0f;
	}

	protected override void Reset()
	{
		base.Reset();
		TrailData.SizeOverLife = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 0.2f), new Keyframe(1f, 0.2f));
		TrailData.Lifetime = 6f;
		ConstantForce = Vector3.up * 0.5f;
		TimeBetweenPoints = 0.1f;
		RandomForceScale = 0.05f;
		MaxNumberOfPoints = 50;
		SetConstantForce();
	}

	protected override void Update()
	{
		if (_emit)
		{
			_timeSincePoint += ((!_noDecay) ? Time.deltaTime : 0f);
			if (_timeSincePoint >= TimeBetweenPoints)
			{
				AddPoint(new SmokeTrailPoint(), _t.position);
				_timeSincePoint = 0f;
			}
			SetConstantForce();
		}
		base.Update();
	}

	protected override void InitialiseNewPoint(PCTrailPoint newPoint)
	{
		((SmokeTrailPoint)newPoint).RandomVec = Random.onUnitSphere * RandomForceScale;
	}

	protected override void UpdateTrail(PCTrail trail, float deltaTime)
	{
		if (_noDecay)
		{
			return;
		}
		foreach (PCTrailPoint point in trail.Points)
		{
			point.Position += m_ConstantForceMod * deltaTime;
		}
	}

	protected override int GetMaxNumberOfPoints()
	{
		return MaxNumberOfPoints;
	}
}
