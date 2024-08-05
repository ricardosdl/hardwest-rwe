using System.Collections;

public interface IPackage
{
	IList Data { get; set; }

	IEnumerator GetEnumerator();

	void OnNew();
}
