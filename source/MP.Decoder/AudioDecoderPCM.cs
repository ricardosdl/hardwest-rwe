using System;
using System.Diagnostics;
using UnityEngine;

namespace MP.Decoder;

public class AudioDecoderPCM : AudioDecoder
{
	public const uint FOURCC_MS = 1u;

	public const uint FORMAT_UNCOMPRESSED = 1u;

	public const uint FORMAT_ALAW = 6u;

	public const uint FORMAT_ULAW = 7u;

	private AudioStreamInfo streamInfo;

	private Demux demux;

	private AudioClip audioClip;

	private float _totalDecodeTime;

	private Stopwatch watch;

	private static float[] ALawExpandLookupTable = new float[256]
	{
		-0.167969f,
		-0.160156f,
		-0.183594f,
		-0.175781f,
		-0.136719f,
		-0.128906f,
		-0.152344f,
		-0.144531f,
		-0.230469f,
		-0.222656f,
		-0.246094f,
		-0.238281f,
		-0.199219f,
		-0.191406f,
		-0.214844f,
		-0.207031f,
		-0.083984f,
		-0.080078f,
		-0.091797f,
		-0.087891f,
		-0.068359f,
		-0.064453f,
		-0.076172f,
		-0.072266f,
		-0.115234f,
		-0.111328f,
		-0.123047f,
		-0.119141f,
		-0.099609f,
		-0.095703f,
		-0.107422f,
		-0.103516f,
		-43f / 64f,
		-41f / 64f,
		-47f / 64f,
		-45f / 64f,
		-35f / 64f,
		-33f / 64f,
		-39f / 64f,
		-37f / 64f,
		-59f / 64f,
		-57f / 64f,
		-63f / 64f,
		-61f / 64f,
		-51f / 64f,
		-49f / 64f,
		-55f / 64f,
		-53f / 64f,
		-0.335938f,
		-0.320312f,
		-0.367188f,
		-0.351562f,
		-0.273438f,
		-0.257812f,
		-0.304688f,
		-0.289062f,
		-0.460938f,
		-0.445312f,
		-0.492188f,
		-0.476562f,
		-0.398438f,
		-0.382812f,
		-0.429688f,
		-0.414062f,
		-0.010498f,
		-0.01001f,
		-0.011475f,
		-0.010986f,
		-0.008545f,
		-0.008057f,
		-0.009521f,
		-0.009033f,
		-0.014404f,
		-0.013916f,
		-0.015381f,
		-0.014893f,
		-0.012451f,
		-0.011963f,
		-0.013428f,
		-0.012939f,
		-0.002686f,
		-0.002197f,
		-0.003662f,
		-0.003174f,
		-0.000732f,
		-0.000244f,
		-0.001709f,
		-0.001221f,
		-0.006592f,
		-0.006104f,
		-0.007568f,
		-0.00708f,
		-0.004639f,
		-0.00415f,
		-0.005615f,
		-0.005127f,
		-0.041992f,
		-0.040039f,
		-0.045898f,
		-0.043945f,
		-0.03418f,
		-0.032227f,
		-0.038086f,
		-0.036133f,
		-0.057617f,
		-0.055664f,
		-0.061523f,
		-0.05957f,
		-0.049805f,
		-0.047852f,
		-0.053711f,
		-0.051758f,
		-0.020996f,
		-0.02002f,
		-0.022949f,
		-0.021973f,
		-0.01709f,
		-0.016113f,
		-0.019043f,
		-0.018066f,
		-0.028809f,
		-0.027832f,
		-0.030762f,
		-0.029785f,
		-0.024902f,
		-0.023926f,
		-0.026855f,
		-0.025879f,
		0.167969f,
		0.160156f,
		0.183594f,
		0.175781f,
		0.136719f,
		0.128906f,
		0.152344f,
		0.144531f,
		0.230469f,
		0.222656f,
		0.246094f,
		0.238281f,
		0.199219f,
		0.191406f,
		0.214844f,
		0.207031f,
		0.083984f,
		0.080078f,
		0.091797f,
		0.087891f,
		0.068359f,
		0.064453f,
		0.076172f,
		0.072266f,
		0.115234f,
		0.111328f,
		0.123047f,
		0.119141f,
		0.099609f,
		0.095703f,
		0.107422f,
		0.103516f,
		43f / 64f,
		41f / 64f,
		47f / 64f,
		45f / 64f,
		35f / 64f,
		33f / 64f,
		39f / 64f,
		37f / 64f,
		59f / 64f,
		57f / 64f,
		63f / 64f,
		61f / 64f,
		51f / 64f,
		49f / 64f,
		55f / 64f,
		53f / 64f,
		0.335938f,
		0.320312f,
		0.367188f,
		0.351562f,
		0.273438f,
		0.257812f,
		0.304688f,
		0.289062f,
		0.460938f,
		0.445312f,
		0.492188f,
		0.476562f,
		0.398438f,
		0.382812f,
		0.429688f,
		0.414062f,
		0.010498f,
		0.01001f,
		0.011475f,
		0.010986f,
		0.008545f,
		0.008057f,
		0.009521f,
		0.009033f,
		0.014404f,
		0.013916f,
		0.015381f,
		0.014893f,
		0.012451f,
		0.011963f,
		0.013428f,
		0.012939f,
		0.002686f,
		0.002197f,
		0.003662f,
		0.003174f,
		0.000732f,
		0.000244f,
		0.001709f,
		0.001221f,
		0.006592f,
		0.006104f,
		0.007568f,
		0.00708f,
		0.004639f,
		0.00415f,
		0.005615f,
		0.005127f,
		0.041992f,
		0.040039f,
		0.045898f,
		0.043945f,
		0.03418f,
		0.032227f,
		0.038086f,
		0.036133f,
		0.057617f,
		0.055664f,
		0.061523f,
		0.05957f,
		0.049805f,
		0.047852f,
		0.053711f,
		0.051758f,
		0.020996f,
		0.02002f,
		0.022949f,
		0.021973f,
		0.01709f,
		0.016113f,
		0.019043f,
		0.018066f,
		0.028809f,
		0.027832f,
		0.030762f,
		0.029785f,
		0.024902f,
		0.023926f,
		0.026855f,
		0.025879f
	};

	private static float[] uLawExpandLookupTable = new float[256]
	{
		-0.980347f,
		-0.949097f,
		-0.917847f,
		-0.886597f,
		-0.855347f,
		-0.824097f,
		-0.792847f,
		-0.761597f,
		-0.730347f,
		-0.699097f,
		-0.667847f,
		-0.636597f,
		-0.605347f,
		-0.574097f,
		-0.542847f,
		-0.511597f,
		-0.488159f,
		-0.472534f,
		-0.456909f,
		-0.441284f,
		-0.425659f,
		-0.410034f,
		-0.394409f,
		-0.378784f,
		-0.363159f,
		-0.347534f,
		-203f / (225f * (float)Math.E),
		-0.316284f,
		-0.300659f,
		-0.285034f,
		-0.269409f,
		-0.253784f,
		-0.242065f,
		-0.234253f,
		-0.22644f,
		-0.218628f,
		-0.210815f,
		-0.203003f,
		-0.19519f,
		-0.187378f,
		-0.179565f,
		-0.171753f,
		-0.16394f,
		-0.156128f,
		-0.148315f,
		-0.140503f,
		-0.13269f,
		-0.124878f,
		-0.119019f,
		-0.115112f,
		-0.111206f,
		-0.1073f,
		-0.103394f,
		-0.099487f,
		-0.095581f,
		-0.091675f,
		-0.087769f,
		-0.083862f,
		-0.079956f,
		-0.07605f,
		-0.072144f,
		-0.068237f,
		-0.064331f,
		-0.060425f,
		-0.057495f,
		-0.055542f,
		-0.053589f,
		-0.051636f,
		-0.049683f,
		-0.047729f,
		-0.045776f,
		-0.043823f,
		-0.04187f,
		-0.039917f,
		-0.037964f,
		-0.036011f,
		-0.034058f,
		-0.032104f,
		-0.030151f,
		-0.028198f,
		-0.026733f,
		-0.025757f,
		-0.02478f,
		-0.023804f,
		-0.022827f,
		-0.021851f,
		-0.020874f,
		-0.019897f,
		-0.018921f,
		-0.017944f,
		-0.016968f,
		-0.015991f,
		-0.015015f,
		-0.014038f,
		-0.013062f,
		-0.012085f,
		-0.011353f,
		-0.010864f,
		-0.010376f,
		-0.009888f,
		-0.009399f,
		-0.008911f,
		-0.008423f,
		-0.007935f,
		-0.007446f,
		-0.006958f,
		-0.00647f,
		-0.005981f,
		-0.005493f,
		-0.005005f,
		-0.004517f,
		-0.004028f,
		-0.003662f,
		-0.003418f,
		-0.003174f,
		-0.00293f,
		-0.002686f,
		-0.002441f,
		-0.002197f,
		-0.001953f,
		-0.001709f,
		-0.001465f,
		-0.001221f,
		-0.000977f,
		-0.000732f,
		-0.000488f,
		-0.000244f,
		0f,
		0.980347f,
		0.949097f,
		0.917847f,
		0.886597f,
		0.855347f,
		0.824097f,
		0.792847f,
		0.761597f,
		0.730347f,
		0.699097f,
		0.667847f,
		0.636597f,
		0.605347f,
		0.574097f,
		0.542847f,
		0.511597f,
		0.488159f,
		0.472534f,
		0.456909f,
		0.441284f,
		0.425659f,
		0.410034f,
		0.394409f,
		0.378784f,
		0.363159f,
		0.347534f,
		203f / (225f * (float)Math.E),
		0.316284f,
		0.300659f,
		0.285034f,
		0.269409f,
		0.253784f,
		0.242065f,
		0.234253f,
		0.22644f,
		0.218628f,
		0.210815f,
		0.203003f,
		0.19519f,
		0.187378f,
		0.179565f,
		0.171753f,
		0.16394f,
		0.156128f,
		0.148315f,
		0.140503f,
		0.13269f,
		0.124878f,
		0.119019f,
		0.115112f,
		0.111206f,
		0.1073f,
		0.103394f,
		0.099487f,
		0.095581f,
		0.091675f,
		0.087769f,
		0.083862f,
		0.079956f,
		0.07605f,
		0.072144f,
		0.068237f,
		0.064331f,
		0.060425f,
		0.057495f,
		0.055542f,
		0.053589f,
		0.051636f,
		0.049683f,
		0.047729f,
		0.045776f,
		0.043823f,
		0.04187f,
		0.039917f,
		0.037964f,
		0.036011f,
		0.034058f,
		0.032104f,
		0.030151f,
		0.028198f,
		0.026733f,
		0.025757f,
		0.02478f,
		0.023804f,
		0.022827f,
		0.021851f,
		0.020874f,
		0.019897f,
		0.018921f,
		0.017944f,
		0.016968f,
		0.015991f,
		0.015015f,
		0.014038f,
		0.013062f,
		0.012085f,
		0.011353f,
		0.010864f,
		0.010376f,
		0.009888f,
		0.009399f,
		0.008911f,
		0.008423f,
		0.007935f,
		0.007446f,
		0.006958f,
		0.00647f,
		0.005981f,
		0.005493f,
		0.005005f,
		0.004517f,
		0.004028f,
		0.003662f,
		0.003418f,
		0.003174f,
		0.00293f,
		0.002686f,
		0.002441f,
		0.002197f,
		0.001953f,
		0.001709f,
		0.001465f,
		0.001221f,
		0.000977f,
		0.000732f,
		0.000488f,
		0.000244f,
		0f
	};

	public override float totalDecodeTime => _totalDecodeTime;

	public override int Position
	{
		get
		{
			return demux.AudioPosition;
		}
		set
		{
			demux.AudioPosition = value;
		}
	}

	public AudioDecoderPCM(AudioStreamInfo streamInfo)
	{
		this.streamInfo = streamInfo;
		if (streamInfo == null)
		{
			throw new ArgumentException("Can't initialize stream decoder without proper AudioStreamInfo");
		}
		if (streamInfo.audioFormat != 1 && streamInfo.audioFormat != 6 && streamInfo.audioFormat != 7)
		{
			throw new ArgumentException("Unsupported PCM format=0x" + streamInfo.audioFormat.ToString("X"));
		}
		int num = streamInfo.sampleSize / streamInfo.channels;
		if (num > 2)
		{
			throw new ArgumentException("Only 8bit and 16bit_le audio is supported. " + num * 8 + "bits given");
		}
	}

	public override void Init(out AudioClip audioClip, Demux demux, LoadOptions loadOptions = null)
	{
		if (loadOptions == null)
		{
			loadOptions = LoadOptions.Default;
		}
		if (demux == null)
		{
			throw new ArgumentException("Missing Demux to get audio samples for decoding");
		}
		this.demux = demux;
		_totalDecodeTime = 0f;
		watch = new Stopwatch();
		this.audioClip = AudioClip.Create("_movie_audio_", streamInfo.sampleCount, streamInfo.channels, streamInfo.sampleRate, !loadOptions.preloadAudio, OnAudioRead, OnAudioSeek);
		audioClip = this.audioClip;
	}

	public override void Shutdown()
	{
		if (audioClip != null)
		{
			if (Application.isEditor)
			{
				UnityEngine.Object.DestroyImmediate(audioClip);
			}
			else
			{
				UnityEngine.Object.Destroy(audioClip);
			}
		}
	}

	public override void DecodeNext(float[] data, int sampleCount)
	{
		if (data == null || demux == null)
		{
			return;
		}
		watch.Reset();
		watch.Start();
		int channels = streamInfo.channels;
		try
		{
			byte[] targetBuf;
			int num = demux.ReadAudioSamples(out targetBuf, sampleCount);
			for (int i = 0; i < channels; i++)
			{
				if (streamInfo.audioFormat == 1)
				{
					int num2 = streamInfo.sampleSize / channels;
					if (num2 == 2)
					{
						for (int j = 0; j < num; j++)
						{
							int num3 = j * channels + i;
							int num4 = num3 * 2;
							short num5 = (short)((targetBuf[num4 + 1] << 8) | targetBuf[num4]);
							data[num3] = (float)num5 / 32768f;
						}
					}
					else
					{
						for (int k = 0; k < num; k++)
						{
							int num6 = k * channels + i;
							data[num6] = (float)(targetBuf[num6] - 128) / 128f;
						}
					}
				}
				else if (streamInfo.audioFormat == 6)
				{
					for (int l = 0; l < num; l++)
					{
						int num7 = l * channels + i;
						data[num7] = ALawExpandLookupTable[targetBuf[num7]];
					}
				}
				else if (streamInfo.audioFormat == 7)
				{
					for (int m = 0; m < num; m++)
					{
						int num8 = m * channels + i;
						data[num8] = uLawExpandLookupTable[targetBuf[num8]];
					}
				}
			}
		}
		catch (Exception ex)
		{
			if (!(ex is IndexOutOfRangeException) && !(ex is ObjectDisposedException))
			{
				throw;
			}
		}
		watch.Stop();
		_totalDecodeTime += (float)(0.0010000000474974513 * watch.Elapsed.TotalMilliseconds);
	}

	public void OnAudioRead(float[] data)
	{
		DecodeNext(data, data.Length / streamInfo.channels);
	}

	public void OnAudioSeek(int newPosition)
	{
		Position = newPosition;
	}
}
