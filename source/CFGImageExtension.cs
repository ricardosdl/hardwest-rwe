using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CFGImageExtension : Image
{
	public delegate void OnAnimate(string text, int data);

	public List<Sprite> m_SpriteList = new List<Sprite>();

	[SerializeField]
	private int m_IconNumber;

	public int m_Data;

	public string m_DataString = string.Empty;

	public OnAnimate m_OnAnimateCallback;

	public int IconNumber
	{
		get
		{
			return m_IconNumber;
		}
		set
		{
			m_IconNumber = value;
			UpdateIconNumber();
		}
	}

	public void SetRandomImage()
	{
		IconNumber = Random.Range(0, m_SpriteList.Count);
	}

	protected override void Start()
	{
		base.Start();
		UpdateIconNumber();
	}

	protected void Update()
	{
	}

	public void UpdateIconNumber()
	{
		if (m_IconNumber < m_SpriteList.Count && m_IconNumber >= 0)
		{
			base.sprite = m_SpriteList[m_IconNumber];
		}
	}

	public void OnAnimationCallback()
	{
		if (m_OnAnimateCallback != null)
		{
			m_OnAnimateCallback(m_DataString, m_Data);
		}
	}
}
