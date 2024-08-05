using System;
using UnityEngine;

[Serializable]
public class StrategicLocation
{
	public Texture2D m_TextureHover;

	public Texture2D m_TextureNormal;

	public Color m_TextureColorHover = new Color(0.847f, 0.478f, 0.011f, 1f);

	public Color m_TextureColorNormal = Color.black;

	public Color m_TextColorHover = new Color(0.749f, 0.533f, 0.078f, 1f);

	public Color m_TextColorNormal = new Color(0.819f, 0.643f, 0.407f, 1f);
}
