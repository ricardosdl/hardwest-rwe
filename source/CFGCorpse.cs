using System.Collections.Generic;
using UnityEngine;

public class CFGCorpse : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_Skulls;

	[SerializeField]
	private GameObject[] m_Ribcages;

	[SerializeField]
	private GameObject[] m_Meats;

	[SerializeField]
	private ParticleSystem m_Bugs;

	[SerializeField]
	private Projector m_Projector;

	private List<GameObject> m_SpawnedObjects;

	private CFGCamera m_Cam;

	private CFGCell m_CurrentCell;

	private void Start()
	{
		SpawnActions(rollSpawnChances: true, m_Skulls);
		SpawnActions(rollSpawnChances: true, m_Ribcages);
		SpawnActions(rollSpawnChances: false, m_Meats);
		if (Random.Range(0, 100) > 60)
		{
			Object.DestroyImmediate(m_Bugs.gameObject);
		}
		if ((bool)m_Projector)
		{
			m_Projector.transform.position = base.transform.position + new Vector3(0f, 0.1657f, 0f);
			m_Projector.transform.LookAt(base.transform);
		}
		m_SpawnedObjects = new List<GameObject>();
		GetGameObjects(m_Skulls);
		GetGameObjects(m_Ribcages);
		GetGameObjects(m_Meats);
		if (m_Bugs != null)
		{
			m_SpawnedObjects.Add(m_Bugs.gameObject);
		}
		if (m_Projector != null)
		{
			m_SpawnedObjects.Add(m_Projector.gameObject);
		}
		m_Cam = Camera.main.GetComponent<CFGCamera>();
		m_CurrentCell = CFGCellMap.GetCharacterCell(base.transform.position);
	}

	private void GetGameObjects(GameObject[] getFromThisArray)
	{
		for (int i = 0; i < getFromThisArray.Length; i++)
		{
			if (getFromThisArray[i] != null)
			{
				m_SpawnedObjects.Add(getFromThisArray[i]);
			}
		}
	}

	private void Update()
	{
		if ((bool)m_Cam && (bool)m_CurrentCell)
		{
			if ((int)m_Cam.CurrentFloorLevel < m_CurrentCell.Floor)
			{
				SetActiveSpawnedObjects(value: false);
			}
			else
			{
				SetActiveSpawnedObjects(value: true);
			}
		}
	}

	private void SetActiveSpawnedObjects(bool value)
	{
		for (int i = 0; i < m_SpawnedObjects.Count; i++)
		{
			if ((bool)m_SpawnedObjects[i])
			{
				m_SpawnedObjects[i].SetActive(value);
			}
		}
	}

	private void SpawnActions(bool rollSpawnChances, GameObject[] objects)
	{
		int seed = Random.seed;
		Random.seed = Random.Range(0, 99999);
		int num = Roll(objects.Length);
		bool flag = !rollSpawnChances || SpawnCheck();
		for (int i = 0; i < objects.Length; i++)
		{
			if (!flag)
			{
				Object.DestroyImmediate(objects[i]);
			}
			else if (i != num)
			{
				Object.DestroyImmediate(objects[i]);
			}
		}
		Random.seed = seed;
	}

	private bool SpawnCheck()
	{
		int num = Random.Range(0, 100);
		return num <= 60;
	}

	private int Roll(int max)
	{
		return Random.Range(0, max);
	}
}
