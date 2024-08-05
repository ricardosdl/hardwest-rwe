using System.Collections.Generic;
using UnityEngine;

public class CFGBullet : CFGGameObject
{
	private class SWinToBreak
	{
		public CFGWindowObject Target;

		public int NextNode = -1;

		public float fDist2NextNode = -1f;
	}

	public delegate void OnBulletHit(CFGBullet bullet);

	private int m_Damage;

	private CFGCharacter m_Attacker;

	private CFGIAttackable m_Target;

	private Vector3 m_TargetPoint = CFGMath.INIFITY3;

	private Vector3 m_StartPoint;

	private bool m_Hit;

	private List<Vector3> m_Path = new List<Vector3>();

	private int m_PathTarget = -1;

	private int m_RicochetObjectCnt;

	private List<SWinToBreak> m_WindowsToBreak;

	[SerializeField]
	private float m_Speed = 1f;

	[SerializeField]
	private float m_TimeToLive = 2f;

	private float m_StartTime;

	private bool m_FreeFlight;

	private int m_ChanceToHit;

	public Transform[] m_BulletTrail;

	public ParticleSystem[] m_FxOnHitPrefab;

	public ParticleSystem[] m_FxOnDiePrefab;

	public Transform m_FxGroundBlood;

	public CFGSoundDef m_MissSoundDef;

	public CFGSoundDef m_RicochetHitSoundDef;

	public CFGSoundDef m_BulletDeath;

	private Vector3 m_UpMod = Vector3.zero;

	public OnBulletHit m_OnBulletHitCallback;

	public void InitBullet(CFGCharacter Attacker, CFGIAttackable Target, bool bHit, int Damage, int CTH, List<CFGRicochetObject> RObjects = null)
	{
		if (Attacker == null)
		{
			Debug.LogWarning("InitBullet: Attacker is null");
		}
		m_Target = Target;
		m_Attacker = Attacker;
		m_Damage = Damage;
		m_ChanceToHit = CTH;
		bool flag = false;
		if (RObjects != null && RObjects.Count > 0)
		{
			flag = true;
			m_RicochetObjectCnt = RObjects.Count;
		}
		if (m_Target != null)
		{
			m_TargetPoint = m_Target.Position;
		}
		m_Hit = bHit;
		m_UpMod = Vector3.up * 1.3840001f;
		if (m_Hit)
		{
			m_TargetPoint += m_UpMod;
		}
		else
		{
			Vector3 Point = ((!(m_Attacker != null)) ? base.transform.position : m_Attacker.Position);
			if (RObjects != null && RObjects.Count > 0 && RObjects[RObjects.Count - 1] != null)
			{
				GetRicochetPoint(RObjects[RObjects.Count - 1], ref Point);
			}
			m_TargetPoint = GenerateMissPoint(m_TargetPoint, (Point - m_TargetPoint).normalized);
		}
		if (m_Path != null)
		{
			m_PathTarget = 0;
			m_Path.Clear();
			if (flag)
			{
				InitPath_Ricochet(RObjects);
			}
			m_Path.Add(m_TargetPoint);
			base.transform.LookAt(m_Path[0]);
			GenerateWindowsToBreak();
			if (!(Attacker == null))
			{
				DetectCollisions(Attacker.transform.position, base.transform.position);
			}
		}
	}

	private void GenerateWindowsToBreak()
	{
		if (m_Path == null || m_Path.Count == 0)
		{
			return;
		}
		CheckForWindows(m_Attacker.Position + 1.73f * Vector3.up, m_Path[0], 0);
		if (m_Path.Count != 1)
		{
			for (int i = 0; i < m_Path.Count - 1; i++)
			{
				CheckForWindows(m_Path[i], m_Path[i + 1], i + 1);
			}
		}
	}

	private void BreakWindows(int Node)
	{
		if (Node < 0 || Node >= m_Path.Count || m_WindowsToBreak == null || m_WindowsToBreak.Count == 0)
		{
			return;
		}
		Vector3 position = base.transform.position;
		float num = Vector3.Distance(position, m_Path[Node]);
		Vector3 breakingSource = m_Attacker.Position;
		if (Node > 0)
		{
			breakingSource = m_Path[Node - 1];
		}
		int num2 = 0;
		while (num2 < m_WindowsToBreak.Count)
		{
			SWinToBreak sWinToBreak = m_WindowsToBreak[num2];
			if (sWinToBreak != null && sWinToBreak.Target != null && sWinToBreak.NextNode == Node && num <= sWinToBreak.fDist2NextNode)
			{
				sWinToBreak.Target.BreakWindow(breakingSource);
				m_WindowsToBreak.RemoveAt(num2);
			}
			else
			{
				num2++;
			}
		}
	}

	private void BreakAllWindows()
	{
		if (m_WindowsToBreak == null)
		{
			return;
		}
		foreach (SWinToBreak item in m_WindowsToBreak)
		{
			if (item == null || item.Target == null)
			{
				continue;
			}
			Vector3 breakingSource = Vector3.zero;
			if (item.NextNode == 0)
			{
				if ((bool)m_Attacker)
				{
					breakingSource = m_Attacker.Position;
				}
			}
			else if (item.NextNode > 0 && item.NextNode < m_Path.Count)
			{
				breakingSource = m_Path[item.NextNode - 1];
			}
			item.Target.BreakWindow(breakingSource);
		}
	}

	private void CheckForWindows(Vector3 Start, Vector3 End, int NextNode)
	{
		Vector3 normalized = (End - Start).normalized;
		float maxDistance = Vector3.Distance(Start, End);
		RaycastHit[] array = Physics.RaycastAll(End, -normalized, maxDistance);
		if (array == null || array.Length == 0)
		{
			return;
		}
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (raycastHit.collider == null)
			{
				continue;
			}
			CFGWindowObject component = raycastHit.collider.GetComponent<CFGWindowObject>();
			if (!(component == null))
			{
				if (m_WindowsToBreak == null)
				{
					m_WindowsToBreak = new List<SWinToBreak>();
				}
				if (m_WindowsToBreak == null)
				{
					break;
				}
				SWinToBreak sWinToBreak = new SWinToBreak();
				if (sWinToBreak != null)
				{
					sWinToBreak.Target = component;
					sWinToBreak.NextNode = NextNode;
					sWinToBreak.fDist2NextNode = raycastHit.distance;
					m_WindowsToBreak.Add(sWinToBreak);
				}
			}
		}
	}

	private Vector3 GenerateMissPoint(Vector3 Target, Vector3 ToTargetDir)
	{
		Vector3 vector = default(Vector3);
		Vector3 rhs = ToTargetDir;
		rhs.y = 0f;
		rhs.Normalize();
		Vector3 vector2 = Vector3.Cross(Vector3.up, rhs);
		float num = Random.Range(0.5f, 1.2f);
		if (Random.value > 0.5f)
		{
			num = 0f - num;
		}
		return Target + m_UpMod * Random.Range(0.3f, 1f) + vector2 * num;
	}

	private void InitPath_Ricochet(List<CFGRicochetObject> RObjects)
	{
		Vector3 vector = m_Attacker.Position;
		Vector3 Point = vector;
		Vector3 Point2 = vector;
		if (RObjects.Count > 1)
		{
			for (int i = 0; i < RObjects.Count - 1; i++)
			{
				if (GetRicochetPoint(RObjects[i], ref Point) && GetRicochetPoint(RObjects[i + 1], ref Point2))
				{
					Point = GetRicochetHitPoint(vector, Point, Point2, RObjects[i]);
					m_Path.Add(Point);
					vector = Point;
				}
			}
		}
		if (GetRicochetPoint(RObjects[RObjects.Count - 1], ref Point))
		{
			m_Path.Add(GetRicochetHitPoint(vector, Point, m_TargetPoint, RObjects[RObjects.Count - 1]));
		}
	}

	private Vector3 GetRicochetHitPoint(Vector3 Start, Vector3 Hit, Vector3 End, CFGRicochetObject RObject)
	{
		if (RObject == null)
		{
			return Hit;
		}
		Collider component = RObject.transform.GetComponent<Collider>();
		if (component == null)
		{
			return Hit;
		}
		Vector3 vector = Start - Hit;
		Vector3 vector2 = (End - Hit).normalized + vector.normalized;
		vector2.Normalize();
		Ray ray = new Ray(Hit + vector2 * 2f, -vector2);
		Vector3 result = Hit;
		if (component.Raycast(ray, out var hitInfo, 4f))
		{
			float num = 2f - hitInfo.distance;
			Vector3 vector3 = new Vector3(num * vector2.x, num * vector2.y, num * vector2.z);
			vector3.x *= RObject.ColliderScale;
			vector3.z *= RObject.ColliderScale;
			result = Hit + vector3;
		}
		return result;
	}

	private bool GetRicochetPoint(CFGRicochetObject RObject, ref Vector3 Point)
	{
		if (RObject == null)
		{
			return false;
		}
		float num = 0.5f;
		Collider component = RObject.transform.GetComponent<Collider>();
		if ((bool)component)
		{
			num = component.bounds.center.y + 0.2f - RObject.transform.position.y;
		}
		Point = RObject.transform.position;
		Point.y += num;
		return true;
	}

	protected override void Update()
	{
		base.Update();
		if (CFGTimer.IsPaused_Gameplay)
		{
			return;
		}
		if (m_Target == null || m_Path == null || m_Path.Count == 0 || m_PathTarget < 0 || (m_PathTarget >= m_Path.Count && !m_FreeFlight))
		{
			End();
			return;
		}
		if (Time.time - m_StartTime > m_TimeToLive)
		{
			CFGSoundDef.Play(m_BulletDeath, base.transform.position);
			if (m_Target != null && m_Target.NameId == "MissTarget")
			{
				Object.Destroy((m_Target as CFGShootableObject).gameObject);
			}
			Object.Destroy(base.gameObject);
			return;
		}
		float num = m_Speed * Time.deltaTime;
		Vector3 vector = base.transform.position;
		Vector3 vector2 = base.transform.forward;
		Vector3 vector3 = vector;
		if (m_FreeFlight)
		{
			vector3 = vector + vector2 * num;
			DetectCollisions(vector, vector3);
		}
		else
		{
			while (num > 0f)
			{
				int pathTarget = m_PathTarget;
				float num2 = num;
				float num3 = Vector3.Distance(vector, m_Path[m_PathTarget]);
				if (num3 < num2)
				{
					if (m_PathTarget < m_Path.Count - 1)
					{
						vector3 = m_Path[m_PathTarget];
						PlayRicochet(vector3, vector2);
						m_PathTarget++;
						vector2 = m_Path[m_PathTarget] - m_Path[m_PathTarget - 1];
						vector2.Normalize();
						num2 = num3;
					}
					else
					{
						vector3 += vector2 * num2;
						if (!m_Hit)
						{
							m_FreeFlight = true;
						}
						End();
					}
				}
				else
				{
					vector3 += vector2 * num2;
				}
				BreakWindows(pathTarget);
				vector = vector3;
				num -= num2;
			}
		}
		base.Transform.forward = vector2;
		base.Transform.position = vector3;
	}

	private void PlayRicochet(Vector3 Position, Vector3 Dir)
	{
		Transform transform = null;
		if (CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData != null)
		{
			transform = CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_AbilityData.m_RicochetBounceFX;
		}
		if (transform != null)
		{
			Quaternion rotation = default(Quaternion);
			rotation.SetLookRotation(-Dir, Vector3.up);
			Object.Instantiate(transform, Position, rotation);
		}
		CFGSoundDef.Play(m_RicochetHitSoundDef, Position);
	}

	private void DetectCollisions(Vector3 Start, Vector3 End)
	{
		Vector3 normalized = (End - Start).normalized;
		float maxDistance = Vector3.Distance(Start, End) + 0.02f;
		RaycastHit[] array = Physics.RaycastAll(Start - normalized * 0.01f, normalized, maxDistance);
		if (array == null || array.Length == 0)
		{
			return;
		}
		RaycastHit[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit raycastHit = array2[i];
			if (!(raycastHit.collider == null))
			{
				CFGWindowObject component = raycastHit.collider.GetComponent<CFGWindowObject>();
				if (!(component == null))
				{
					component.BreakWindow(Start);
				}
			}
		}
	}

	private void End()
	{
		BreakAllWindows();
		if (m_Hit)
		{
			MakeDamage();
			if (m_Attacker.Owner != null && m_Attacker.Owner.IsPlayer && m_Attacker.Abilities.ContainsKey(ETurnAction.Jinx))
			{
				m_Attacker.JinxedFlagNeedFlash = true;
			}
			Object.Destroy(base.gameObject);
		}
		else if (m_Target != null || m_TargetPoint.x >= -800f)
		{
			Vector3 targetPoint = m_TargetPoint;
			targetPoint.y += 1.2f;
			if (m_Target != null && m_Target.IsDodging)
			{
				CFGCharacter cFGCharacter = m_Target as CFGCharacter;
				if ((bool)cFGCharacter)
				{
					cFGCharacter.FlagNeedFlash = true;
					Vector3 position = cFGCharacter.transform.position;
					position.y += 1.9f;
					CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position, "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("ability_float_dodge") + "</color>", plus: false, 1, -1, cFGCharacter);
					CFGSoundDef.Play(CFGSingletonResourcePrefab<CFGGameplaySettings>.Instance.m_SoundDefs.m_Dodge, cFGCharacter.transform);
				}
			}
			else
			{
				CFGCharacter cFGCharacter2 = m_Target as CFGCharacter;
				if ((bool)cFGCharacter2)
				{
					Vector3 position2 = cFGCharacter2.transform.position;
					position2.y += 1.9f;
					CFGSingleton<CFGWindowMgr>.Instance.SpawnSplash(position2, "<color=white>" + CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText("tactical_floating_miss") + "</color>", plus: false, 1, -1, cFGCharacter2);
				}
			}
			m_FreeFlight = true;
			CFGSoundDef.Play(m_MissSoundDef, base.transform.position);
		}
		if (m_OnBulletHitCallback != null)
		{
			m_OnBulletHitCallback(this);
		}
	}

	protected override void Start()
	{
		base.Start();
		m_StartTime = Time.time;
		m_StartPoint = base.transform.position;
	}

	private void CrippleTarget()
	{
		CFGCharacter cFGCharacter = m_Target as CFGCharacter;
		if (cFGCharacter == null)
		{
			return;
		}
		string text = string.Empty;
		float num = Vector3.Distance(m_TargetPoint, m_Attacker.CurrentCell.WorldPosition);
		if (num < 5f)
		{
			text = "crippledaim";
		}
		else
		{
			switch (CFGCharacter.GetTargetCover(m_Attacker.CurrentCell, m_Target.CurrentCell))
			{
			case ECoverType.NONE:
				text = "crippledmovement";
				break;
			case ECoverType.HALF:
				text = "crippledsight";
				break;
			case ECoverType.FULL:
				text = "crippleddefense";
				break;
			}
		}
		if (!(text == string.Empty))
		{
			cFGCharacter.CharacterData.AddBuff(text, EBuffSource.FromShot);
		}
	}

	private void MakeDamage()
	{
		if (m_Target == null)
		{
			return;
		}
		bool flag = m_Target.TakeDamage(m_Damage, m_Attacker, bSilent: false, base.transform.forward);
		if (m_Attacker != null && m_Attacker.HaveAbility(ETurnAction.Crippler) && !flag)
		{
			CrippleTarget();
		}
		if (m_Attacker != null && (bool)m_Attacker.Owner && m_Attacker.Owner.IsPlayer)
		{
			if (m_RicochetObjectCnt > 2 && flag)
			{
				CFGSingleton<CFGAchievementManager>.Instance.UnlockAchievement(EAchievement.ACHIEVEMENT_27);
			}
			CFGAchievmentTracker.OnShotFired(m_Attacker, m_Target as CFGCharacter, m_ChanceToHit, flag);
		}
		CFGSingletonResourcePrefab<CFGDialogSystem>.Instance.PlayAlert("alert_charshot", CFGSingletonResourcePrefab<CFGTextManager>.Instance.GetLocalizedText(m_Target.NameId), m_Damage.ToString());
		CFGCharacter cFGCharacter = m_Target as CFGCharacter;
		if (cFGCharacter != null && cFGCharacter.IsVisibleByPlayer() && (cFGCharacter.BestDetectionType & EBestDetectionType.Visible) == EBestDetectionType.Visible)
		{
			Transform damagePivot = m_Target.GetDamagePivot();
			if (m_FxOnHitPrefab != null && m_FxOnHitPrefab.Length > 0 && damagePivot != null)
			{
				int num = Random.Range(0, m_FxOnHitPrefab.Length);
				if (m_FxOnHitPrefab[num] != null)
				{
					ParticleSystem particleSystem = null;
					particleSystem = Object.Instantiate(m_FxOnHitPrefab[num], damagePivot.position, Quaternion.identity) as ParticleSystem;
					particleSystem.transform.parent = damagePivot;
					particleSystem.transform.LookAt(base.transform.position - base.transform.forward);
				}
				else
				{
					Debug.LogError("CFGBullet have empty FxOnHitPrefab elements! Reduce the list size or assign an element.", base.gameObject);
				}
			}
			if (flag && m_FxOnDiePrefab != null && m_FxOnDiePrefab.Length > 0 && damagePivot != null)
			{
				int num2 = Random.Range(0, m_FxOnDiePrefab.Length);
				if (m_FxOnDiePrefab[num2] != null)
				{
					ParticleSystem particleSystem2 = null;
					particleSystem2 = Object.Instantiate(m_FxOnDiePrefab[num2], damagePivot.position, Quaternion.identity) as ParticleSystem;
					particleSystem2.transform.parent = damagePivot;
					particleSystem2.transform.LookAt(base.transform.position - base.transform.forward);
				}
				else
				{
					Debug.LogError("CFGBullet have empty FxOnDiePrefab elements! Reduce the list size or assign an element.", base.gameObject);
				}
			}
			if ((bool)m_FxGroundBlood)
			{
				Object.Instantiate(m_FxGroundBlood, m_Target.Position, m_FxGroundBlood.transform.rotation);
			}
		}
		for (int i = 0; i < m_BulletTrail.Length; i++)
		{
			if (m_BulletTrail[i] != null)
			{
				m_BulletTrail[i].parent = null;
				m_BulletTrail[i] = null;
			}
		}
	}
}
