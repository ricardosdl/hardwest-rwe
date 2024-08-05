using UnityEngine;

[ExecuteInEditMode]
public class VertHandler : MonoBehaviour
{
	private Mesh mesh;

	private Vector3[] verts;

	private Vector3 vertPos;

	private GameObject[] handles;

	private void OnEnable()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		verts = mesh.vertices;
		Vector3[] array = verts;
		foreach (Vector3 position in array)
		{
			vertPos = base.transform.TransformPoint(position);
			GameObject gameObject = new GameObject("handle");
			gameObject.transform.position = vertPos;
			gameObject.transform.parent = base.transform;
			gameObject.tag = "handle";
			gameObject.AddComponent<GizmoSphere>();
		}
	}

	private void OnDisable()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("handle");
		GameObject[] array2 = array;
		foreach (GameObject obj in array2)
		{
			Object.DestroyImmediate(obj);
		}
	}

	private void Update()
	{
		handles = GameObject.FindGameObjectsWithTag("handle");
		for (int i = 0; i < verts.Length; i++)
		{
			ref Vector3 reference = ref verts[i];
			reference = handles[i].transform.localPosition;
		}
		mesh.vertices = verts;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}
}
