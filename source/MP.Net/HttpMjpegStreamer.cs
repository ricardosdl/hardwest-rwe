using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace MP.Net;

public class HttpMjpegStreamer : Streamer
{
	private const int INITIAL_BYTE_BUFFER_SIZE = 131072;

	private const int MAX_BYTE_BUFFER_SIZE = 1048576;

	private object locker = new object();

	private string _status;

	private int receivedFrameCount;

	private long _bytesReceived;

	private Thread thread;

	private float timeout;

	private byte[][] frameRingBuffer;

	private volatile bool shouldStop;

	private volatile bool connected;

	public string Status
	{
		get
		{
			lock (locker)
			{
				return _status;
			}
		}
		private set
		{
			lock (locker)
			{
				_status = value;
			}
		}
	}

	public long BytesReceived
	{
		get
		{
			lock (locker)
			{
				return _bytesReceived;
			}
		}
		private set
		{
			lock (locker)
			{
				_bytesReceived = value;
			}
		}
	}

	public override bool IsConnected => thread != null && thread.IsAlive && connected;

	public override int VideoPosition
	{
		get
		{
			return receivedFrameCount;
		}
		set
		{
			throw new NotSupportedException("Can't seek a live stream");
		}
	}

	public override int AudioPosition
	{
		get
		{
			throw new NotSupportedException("There's no audio stream in HTTP MJPEG stream");
		}
		set
		{
			throw new NotSupportedException("Can't seek a live stream");
		}
	}

	public override void Connect(string url, LoadOptions loadOptions = null)
	{
		if (loadOptions != null)
		{
			base.videoStreamInfo = ((loadOptions.videoStreamInfo == null) ? new VideoStreamInfo() : loadOptions.videoStreamInfo);
			timeout = loadOptions.connectTimeout;
		}
		else
		{
			base.videoStreamInfo = new VideoStreamInfo();
			timeout = 10f;
		}
		base.videoStreamInfo.codecFourCC = 1196444237u;
		base.videoStreamInfo.frameCount = 0;
		base.videoStreamInfo.framerate = 0f;
		frameRingBuffer = new byte[1][];
		receivedFrameCount = 0;
		shouldStop = false;
		thread = new Thread(ThreadRun);
		thread.Start(url);
	}

	public override void Shutdown(bool force = false)
	{
		shouldStop = true;
		if (force && thread != null)
		{
			thread.Interrupt();
			thread = null;
		}
	}

	public override int ReadVideoFrame(out byte[] targetBuf)
	{
		lock (locker)
		{
			targetBuf = ((receivedFrameCount <= 0) ? null : frameRingBuffer[receivedFrameCount % frameRingBuffer.Length]);
		}
		return (targetBuf != null) ? targetBuf.Length : 0;
	}

	public override int ReadAudioSamples(out byte[] targetBuf, int sampleCount)
	{
		throw new NotSupportedException("There's no audio stream in HTTP MJPEG stream");
	}

	private void FrameReceived(byte[] bytes)
	{
		Status = "Received frame " + receivedFrameCount;
		lock (locker)
		{
			frameRingBuffer[receivedFrameCount % frameRingBuffer.Length] = bytes;
			receivedFrameCount++;
		}
	}

	private void ThreadRun(object url)
	{
		Stream stream = null;
		try
		{
			connected = false;
			Status = "Connecting to " + url;
			WebRequest webRequest = WebRequest.Create((string)url);
			webRequest.Timeout = (int)(timeout * 1000f);
			webRequest.ContentType = "image/png,image/*;q=0.8,*/*;q=0.5";
			BytesReceived = 0L;
			stream = webRequest.GetResponse().GetResponseStream();
			BinaryReader binaryReader = new BinaryReader(new BufferedStream(stream), new ASCIIEncoding());
			List<byte> list = new List<byte>(131072);
			Status = "Connected. Waiting for the first frame...";
			connected = true;
			int num = 0;
			bool flag = false;
			while (!shouldStop)
			{
				byte b = binaryReader.ReadByte();
				BytesReceived++;
				if (flag)
				{
					if (list.Count > 1048576)
					{
						list.Clear();
						flag = false;
					}
					else
					{
						list.Add(b);
					}
				}
				switch (num)
				{
				case 0:
					if (b == byte.MaxValue)
					{
						num = 1;
					}
					break;
				case 1:
					switch (b)
					{
					case 216:
						list.Clear();
						list.Add(byte.MaxValue);
						list.Add(216);
						flag = true;
						break;
					case 217:
						FrameReceived(list.ToArray());
						flag = false;
						Thread.Sleep(1);
						break;
					}
					num = 0;
					break;
				}
			}
			Status = "Closing the connection";
		}
		catch (Exception ex)
		{
			Status = ex.ToString();
		}
		finally
		{
			connected = false;
			stream?.Close();
		}
	}

	private static string ReadLine(BinaryReader reader)
	{
		StringBuilder stringBuilder = new StringBuilder(100);
		char c;
		while ((c = reader.ReadChar()) != '\n')
		{
			if (c != '\r' && c != '\n')
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}
}
