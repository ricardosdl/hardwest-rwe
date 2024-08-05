using UnityEngine;

public class CFGHighlighter : MonoBehaviour
{
	public enum BlurType
	{
		StandardGauss,
		SgxGauss
	}

	public float m_ColorBoost = 1f;

	public Shader m_ReplaceMentShaderAlly;

	public Shader m_ReplaceMentShaderEnemy;

	public Camera m_Camera;

	private RenderTexture m_RTEnemyAlly;

	private Camera m_AllyCamera;

	private Camera m_EnemyCamera;

	[Range(0f, 2f)]
	public int downsample = 1;

	[Range(0f, 10f)]
	public float blurSize = 3f;

	[Range(1f, 4f)]
	public int blurIterations = 2;

	public BlurType blurType;

	public Shader blurShader;

	private Material blurMaterial;

	private bool m_canWork;

	private void Start()
	{
		int width = Screen.width;
		int height = Screen.height;
		CFGRenderTargetUtility.CalcRenderTargetSize(ref width, ref height);
		m_RTEnemyAlly = new RenderTexture(width, height, 0, RenderTextureFormat.Default);
		m_AllyCamera = Object.Instantiate(m_Camera);
		m_AllyCamera.name = "AllyCamera";
		m_AllyCamera.transform.parent = Camera.main.transform;
		m_AllyCamera.transform.localEulerAngles = Vector3.zero;
		m_AllyCamera.transform.localPosition = Vector3.zero;
		m_AllyCamera.cullingMask = LayerMask.GetMask("Ally");
		m_AllyCamera.SetReplacementShader(m_ReplaceMentShaderAlly, string.Empty);
		m_AllyCamera.targetTexture = m_RTEnemyAlly;
		m_AllyCamera.depthTextureMode = DepthTextureMode.Depth;
		m_EnemyCamera = Object.Instantiate(m_Camera);
		m_EnemyCamera.name = "EnemyCamera";
		m_EnemyCamera.transform.parent = Camera.main.transform;
		m_EnemyCamera.transform.localEulerAngles = Vector3.zero;
		m_EnemyCamera.transform.localPosition = Vector3.zero;
		m_EnemyCamera.cullingMask = LayerMask.GetMask("Enemy");
		m_EnemyCamera.depth = 1f;
		m_EnemyCamera.clearFlags = CameraClearFlags.Nothing;
		m_EnemyCamera.SetReplacementShader(m_ReplaceMentShaderEnemy, string.Empty);
		m_EnemyCamera.targetTexture = m_RTEnemyAlly;
		m_EnemyCamera.depthTextureMode = DepthTextureMode.Depth;
		CheckResources();
	}

	private void CheckResources()
	{
		blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);
		if ((bool)blurMaterial)
		{
			blurMaterial.SetFloat("_Boost", m_ColorBoost);
		}
		if ((bool)blurMaterial && (bool)blurShader)
		{
			m_canWork = true;
		}
	}

	protected Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	{
		if (!s)
		{
			Debug.Log("Missing shader in " + ToString());
			base.enabled = false;
			return null;
		}
		if (s.isSupported && (bool)m2Create && m2Create.shader == s)
		{
			return m2Create;
		}
		if (!s.isSupported)
		{
			Debug.Log("The shader " + s.ToString() + " on effect " + ToString() + " is not supported on this platform!");
			return null;
		}
		m2Create = new Material(s);
		m2Create.hideFlags = HideFlags.DontSave;
		if ((bool)m2Create)
		{
			return m2Create;
		}
		return null;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!m_canWork)
		{
			Graphics.Blit(source, destination);
			Debug.Log($"Shader not assigned in gameobject {base.name}");
			return;
		}
		Graphics.Blit(source, destination);
		int width = source.width;
		int height = source.height;
		CFGRenderTargetUtility.CalcRenderTargetSize(ref width, ref height);
		int width2 = width >> downsample;
		int height2 = height >> downsample;
		float num = 1f / (1f * (float)(1 << downsample));
		blurMaterial.SetVector("_Parameter", new Vector4(blurSize * num, (0f - blurSize) * num, 0f, 0f));
		RenderTexture renderTexture = RenderTexture.GetTemporary(width2, height2, 0, source.format);
		renderTexture.filterMode = FilterMode.Bilinear;
		Graphics.Blit(m_RTEnemyAlly, renderTexture, blurMaterial, 0);
		blurMaterial.SetTexture("_AllyTex", m_RTEnemyAlly);
		int num2 = ((blurType != 0) ? 2 : 0);
		for (int i = 0; i < blurIterations; i++)
		{
			float num3 = (float)i * 1f;
			blurMaterial.SetVector("_Parameter", new Vector4(blurSize * num + num3, (0f - blurSize) * num - num3, 0f, 0f));
			RenderTexture temporary = RenderTexture.GetTemporary(width2, height2, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary, blurMaterial, 1 + num2);
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
			temporary = RenderTexture.GetTemporary(width2, height2, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			if (i != blurIterations - 1)
			{
				Graphics.Blit(renderTexture, temporary, blurMaterial, 2 + num2);
			}
			else
			{
				Graphics.Blit(renderTexture, destination, blurMaterial, 5);
			}
			RenderTexture.ReleaseTemporary(renderTexture);
			renderTexture = temporary;
		}
		RenderTexture.ReleaseTemporary(renderTexture);
	}
}
