using System.IO;
using UnityEngine;

namespace MP;

public class Movie
{
	public Stream sourceStream;

	public Demux demux;

	public VideoDecoder videoDecoder;

	public AudioDecoder audioDecoder;

	public Rect[] frameUV;
}
