using System;
using UnityEngine;

public class CFGLightning : MonoBehaviour
{
	[Header("GENERAL SETTINGS")]
	public Transform[] m_Targets;

	public int m_Resolution = 50;

	public Material m_Material;

	public Color m_ColorStart = Color.white;

	public Color m_ColorEnd = Color.blue;

	[Space(5f)]
	public float m_WidthStart = 0.25f;

	public float m_WidthEnd = 0.12f;

	[SerializeField]
	public AnimationCurve m_WidthOverLife = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	[Header("BEAM SETTINGS")]
	public Vector2 m_AmplitudeRange = new Vector2(0.7f, 1.4f);

	public float m_Frequency = 2f;

	public float m_Shifting = 2f;

	public float m_NoiseSize = 25f;

	[SerializeField]
	public AnimationCurve m_BeamFade = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	public Vector2 m_FadeInDurationRange = new Vector2(0f, 0.3f);

	public AnimationCurve m_YOffsetCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	private float m_FadeInDuration;

	private float m_FadeInTimer;

	private bool m_isFadingIn = true;

	[Tooltip("Min/Max value in seconds")]
	[Space(10f)]
	public Vector2 m_ZapDurationRange = new Vector2(0.1f, 0.4f);

	[Tooltip("Min/Max value in seconds")]
	public Vector2 m_ZapDelayRange = new Vector2(0.1f, 0.6f);

	public bool m_EmissionLoop;

	[Header("LOW NOISE SETTINGS")]
	public bool m_LowNoise = true;

	public Vector2 m_LowNoiseAmplitudeRange = new Vector2(0.7f, 2f);

	public float m_LowNoiseFrequency = 3f;

	public float m_LowNoiseShifting = 1.5f;

	public float m_LowNoiseSize = 8f;

	[SerializeField]
	public AnimationCurve m_LowNoiseFade = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private float m_LowNoiseAmplitude;

	[Header("LIGHT OPTIONS")]
	public bool m_EmitLight;

	public Color m_LightColor = Color.white;

	public Vector2 m_LightIntensity = new Vector2(1f, 5f);

	public Vector2 m_LightRadius = new Vector2(5f, 15f);

	[Header("PARTICLE OPTIONS")]
	public bool m_SpawnParticles;

	public ParticleSystem m_ParticlePrefab;

	private ParticleSystem m_CurrentParticle;

	[Range(0f, 100f)]
	public int m_ChanceToSpawn = 80;

	public bool m_StickToTarget;

	public bool m_LookAtEmitter = true;

	private bool m_ParticleSpawned;

	[Header("OTHER OPTIONS")]
	public bool m_StaticTarget = true;

	public bool m_AutoDestroy;

	private Transform m_Target;

	private LineRenderer m_Lightning;

	private Vector3 m_EndPoint = Vector3.zero;

	private CFGLight m_Light;

	private Perlin perlin = new Perlin();

	private float m_Amplitude;

	private float m_ZapTimer;

	private float m_DelayTimer;

	private float m_ZapDuration;

	private float m_ZapDelay;

	private bool m_Delay;

	private float m_Distance;

	private float m_Lifetime;

	private Vector2 m_Seed = new Vector2(0f, 0f);

	private Vector2 m_LowNoiseSeed = new Vector2(0f, 0f);

	private bool m_ReachedDestination;

	private bool m_Emit = true;

	private CFGCharacterAnimator.OnEventDelegate m_OnDestroyDelegate;

	private void Start()
	{
		m_Lightning = base.gameObject.AddComponent<LineRenderer>();
		m_Lightning.hideFlags = HideFlags.HideInInspector;
		m_Lightning.useWorldSpace = false;
		m_ReachedDestination = false;
		GetTarget();
		SetBeam(m_Resolution);
		SetAmplitude();
		SetSeed();
		SetZapDuration();
		SetZapDelay();
		SetFade();
		if (m_EmitLight)
		{
			GameObject gameObject = new GameObject("SparkLight");
			m_Light = gameObject.AddComponent<CFGLight>();
			m_Light.transform.parent = null;
			m_Light.SetPosition(m_Target.transform.position);
			m_Light.SetColor(m_LightColor);
		}
		m_LowNoiseFade.postWrapMode = WrapMode.Once;
		m_WidthOverLife.postWrapMode = WrapMode.Once;
	}

	private void LateUpdate()
	{
		if (m_Emit)
		{
			if ((bool)m_Light)
			{
				if (m_Delay)
				{
					m_Light.SetLight(0f, 0f);
				}
				else if (!m_isFadingIn)
				{
					if (!m_StaticTarget)
					{
						m_Light.SetPosition(m_Target.transform.position);
					}
					float lightIntensity = UnityEngine.Random.Range(m_LightIntensity.x, m_LightIntensity.y);
					float lightRange = UnityEngine.Random.Range(m_LightRadius.x, m_LightRadius.y);
					m_Light.SetLight(lightIntensity, lightRange);
				}
			}
			SetTimer();
			SetLifetime();
			if (m_Delay || !m_Emit || !m_Target)
			{
				return;
			}
			if (!m_StaticTarget)
			{
				SetBeamPosition();
			}
			SpawnParticles();
			int num = 0;
			float num2 = 0f;
			float value = m_FadeInTimer / m_FadeInDuration;
			value = Mathf.Clamp(value, 0f, 1f);
			float num3 = ((!(m_Frequency < 0f)) ? (0f - m_NoiseSize) : m_NoiseSize);
			float num4 = ((!(m_LowNoiseShifting < 0f)) ? (0f - m_LowNoiseSize) : m_LowNoiseSize);
			float num5 = m_WidthOverLife.Evaluate(m_Lifetime);
			m_Lightning.SetWidth(m_WidthStart * num5, m_WidthEnd * num5);
			m_Seed.x += 0.01f * m_Frequency;
			m_Seed.y -= 0.01f * m_Frequency;
			m_LowNoiseSeed.x += 0.01f * m_LowNoiseFrequency;
			m_LowNoiseSeed.y -= 0.01f * m_LowNoiseFrequency;
			Vector2 vector = new Vector2(Mathf.Abs((Time.time + m_Seed.x) * m_Shifting), Mathf.Abs((Time.time + m_Seed.y) * m_Shifting));
			Vector3 vector2 = new Vector3(Mathf.Abs((Time.time + m_LowNoiseSeed.x) * m_LowNoiseShifting), Mathf.Abs((Time.time + m_LowNoiseSeed.y) * m_LowNoiseShifting), Mathf.Abs((Time.time + m_LowNoiseSeed.x + m_LowNoiseSeed.y) * m_LowNoiseShifting));
			while (num < m_Resolution)
			{
				float num6 = num2 / (float)m_Resolution;
				Vector3 vector3 = new Vector3(0f, m_YOffsetCurve.Evaluate(value * num6), m_Distance * value * num6);
				Vector2 vector4 = new Vector2(perlin.Noise(num3 * vector3.z + vector.x), perlin.Noise(num3 * vector3.z + vector.y));
				vector4 *= m_BeamFade.Evaluate(num6) * m_Amplitude;
				vector3.x += vector4.x;
				vector3.y += vector4.y;
				if (m_LowNoise)
				{
					Vector3 vector5 = new Vector3(perlin.Noise(num4 * vector3.z + vector2.x), perlin.Noise(num4 * vector3.z + vector2.y), perlin.Noise(num4 * vector3.z + vector2.z));
					vector5.z *= 0.1f;
					vector5 *= m_LowNoiseFade.Evaluate(num6) * m_LowNoiseAmplitude;
					vector3 += vector5;
				}
				if (num == m_Resolution - 1)
				{
					m_EndPoint = vector3;
				}
				m_Lightning.SetPosition(num, vector3);
				num++;
				num2 += 1f;
			}
			StickParticles();
		}
		else if (m_AutoDestroy)
		{
			if (m_OnDestroyDelegate != null)
			{
				m_OnDestroyDelegate();
				m_OnDestroyDelegate = null;
			}
			if ((bool)m_Light)
			{
				UnityEngine.Object.Destroy(m_Light);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void GetTarget()
	{
		if (m_Targets.Length > 0)
		{
			m_Target = m_Targets[UnityEngine.Random.Range(0, m_Targets.Length)];
			SetBeamPosition();
		}
		else
		{
			m_Target = null;
		}
		if ((bool)m_Light && (bool)m_Target)
		{
			m_Light.SetPosition(m_Target.transform.position);
		}
	}

	private void SetBeamPosition()
	{
		if (m_Target != null)
		{
			base.transform.LookAt(m_Target.transform);
			m_Distance = Vector3.Distance(base.transform.position, m_Target.position);
		}
	}

	private void SetBeam(int resolution)
	{
		if ((bool)m_Lightning)
		{
			m_Lightning.material = m_Material;
			m_Lightning.SetColors(m_ColorStart, m_ColorEnd);
			m_Lightning.SetWidth(m_WidthStart, m_WidthEnd);
			m_Lightning.SetVertexCount(resolution);
		}
	}

	private void SetTimer()
	{
		if (m_Emit)
		{
			if (!m_Delay)
			{
				if (m_ZapTimer > m_ZapDuration)
				{
					SetBeam(0);
					m_ZapTimer = 0f;
					m_Delay = true;
					SetZapDuration();
					SetAmplitude();
					SetSeed();
					GetTarget();
					SetFade();
				}
				else
				{
					m_ZapTimer += Time.deltaTime;
					if (m_isFadingIn)
					{
						if (m_FadeInTimer >= m_FadeInDuration)
						{
							m_isFadingIn = false;
							m_FadeInTimer = m_FadeInDuration;
						}
						else
						{
							m_FadeInTimer += Time.deltaTime;
						}
					}
				}
			}
			if (m_Delay)
			{
				m_ReachedDestination = true;
				m_CurrentParticle = null;
				if (m_DelayTimer > m_ZapDelay)
				{
					m_Delay = false;
					m_DelayTimer = 0f;
					SetBeam(m_Resolution);
					SetZapDelay();
					m_ParticleSpawned = false;
					SetFade();
					m_isFadingIn = true;
					m_FadeInTimer = 0f;
				}
				else
				{
					m_DelayTimer += Time.deltaTime;
				}
			}
		}
		if (!m_EmissionLoop && m_ReachedDestination)
		{
			m_Emit = false;
			SetBeam(0);
		}
	}

	private void SetFade()
	{
		if (m_FadeInDurationRange.x == 0f && m_FadeInDurationRange.y == 0f)
		{
			m_isFadingIn = false;
			return;
		}
		m_FadeInDuration = UnityEngine.Random.Range(m_FadeInDurationRange.x, m_FadeInDurationRange.y);
		if (m_FadeInDuration > m_ZapDuration)
		{
			m_FadeInDuration = 0.5f * m_ZapDuration;
		}
		if (m_FadeInDuration == 0f)
		{
			m_isFadingIn = false;
		}
	}

	private void SetSeed()
	{
		m_Seed.x = UnityEngine.Random.Range(0f, 9999f);
		m_Seed.y = UnityEngine.Random.Range(0f, 9999f);
		m_LowNoiseSeed.x = UnityEngine.Random.Range(0f, 9999f);
		m_LowNoiseSeed.y = UnityEngine.Random.Range(0f, 9999f);
	}

	private void SetAmplitude()
	{
		m_Amplitude = UnityEngine.Random.Range(m_AmplitudeRange.x, m_AmplitudeRange.y);
		m_LowNoiseAmplitude = UnityEngine.Random.Range(m_LowNoiseAmplitudeRange.x, m_LowNoiseAmplitudeRange.y);
	}

	private void SetZapDuration()
	{
		m_ZapDuration = UnityEngine.Random.Range(m_ZapDurationRange.x, m_ZapDurationRange.y);
	}

	private void SetZapDelay()
	{
		m_ZapDelay = UnityEngine.Random.Range(m_ZapDelayRange.x, m_ZapDelayRange.y);
	}

	private void SetLifetime()
	{
		m_Lifetime = m_ZapTimer / m_ZapDuration;
	}

	private void SpawnParticles()
	{
		if (!m_SpawnParticles || m_ParticleSpawned || m_isFadingIn)
		{
			return;
		}
		int num = UnityEngine.Random.Range(0, 100);
		if (num <= m_ChanceToSpawn)
		{
			m_CurrentParticle = (ParticleSystem)UnityEngine.Object.Instantiate(m_ParticlePrefab, m_Target.transform.position, m_Target.transform.rotation);
			if (m_LookAtEmitter && (bool)m_CurrentParticle)
			{
				m_CurrentParticle.transform.LookAt(base.transform.position);
			}
		}
		m_ParticleSpawned = true;
	}

	private void StickParticles()
	{
		if (m_StickToTarget && m_CurrentParticle != null && !m_isFadingIn)
		{
			m_CurrentParticle.transform.position = base.transform.TransformPoint(m_EndPoint);
			if (m_LookAtEmitter)
			{
				m_CurrentParticle.transform.LookAt(base.transform.position);
			}
		}
	}

	public static void AddEmitter()
	{
		GameObject gameObject = new GameObject("CFGLightning_Emitter");
		gameObject.AddComponent<CFGLightning>();
	}

	public void SetTargets(Transform[] targets)
	{
		m_Targets = targets;
		GetTarget();
	}

	public void SetTargets(Transform target, CFGCharacterAnimator.OnEventDelegate on_end_callback = null)
	{
		m_Targets = new Transform[1];
		m_Targets[0] = target;
		GetTarget();
		m_OnDestroyDelegate = (CFGCharacterAnimator.OnEventDelegate)Delegate.Combine(m_OnDestroyDelegate, on_end_callback);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "editor_lightning_gizmo.png", allowScaling: true);
	}
}
