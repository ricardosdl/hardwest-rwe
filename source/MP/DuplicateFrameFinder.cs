using UnityEngine;

namespace MP;

public class DuplicateFrameFinder
{
	public class Options
	{
		public float maxImageDiff = 5f;

		public int maxPixelDiff = 50;

		public int maxLookbackFrames = 100;

		public int toneCompareDistrust = 2;

		public int pixelCacheSize = 101;

		public bool otherStreamsAvailable;

		public static Options Default => new Options();
	}

	public struct Stats
	{
		public long totalFramesCompared;

		public long framesPartiallyCompared;

		public long framesFullyCompared;

		public long pixelCacheQueries;

		public long pixelCacheHits;

		public int duplicateCount;

		public float framesPartiallyComparedPercent => (float)framesPartiallyCompared / (float)totalFramesCompared * 100f;

		public float framesFullyComparedPercent => (float)framesFullyCompared / (float)totalFramesCompared * 100f;

		public float pixelCacheHitPercent => (float)pixelCacheHits / (float)pixelCacheQueries * 100f;
	}

	public Stats stats;

	private int[] duplicateOf;

	private Color32[] frameTones;

	private Color32[][] pixelCache;

	private int currentFrame;

	private VideoDecoder videoDecoder;

	private Texture2D framebuffer;

	private int frameOffset;

	private int frameCount;

	private Options options;

	public int framesProcessed => currentFrame;

	public int[] duplicates => duplicateOf;

	public DuplicateFrameFinder(VideoDecoder videoDecoder, Texture2D framebuffer, int frameOffset, int frameCount, Options options = null)
	{
		if (options == null)
		{
			options = Options.Default;
		}
		this.options = options;
		this.videoDecoder = videoDecoder;
		this.framebuffer = framebuffer;
		this.frameOffset = frameOffset;
		this.frameCount = frameCount;
		Reset();
	}

	public void Reset()
	{
		Reset(frameOffset, frameCount, options);
	}

	public void Reset(int frameOffset, int frameCount, Options options = null)
	{
		this.frameOffset = frameOffset;
		this.frameCount = frameCount;
		frameTones = new Color32[frameCount];
		duplicateOf = new int[frameCount];
		for (int i = 0; i < frameCount; i++)
		{
			duplicateOf[i] = -1;
		}
		if (options.pixelCacheSize > 0)
		{
			pixelCache = new Color32[options.pixelCacheSize][];
		}
		else
		{
			pixelCache = null;
		}
		currentFrame = 0;
		stats = default(Stats);
	}

	public bool Progress()
	{
		if (currentFrame >= frameCount)
		{
			return false;
		}
		videoDecoder.Decode(currentFrame + frameOffset);
		Color32[] array = ((pixelCache == null) ? framebuffer.GetPixels32() : (pixelCache[currentFrame % pixelCache.Length] = framebuffer.GetPixels32()));
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		uint num4 = 0u;
		Color32[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Color32 color = array2[i];
			num += color.r;
			num2 += color.g;
			num3 += color.b;
			num4 += color.a;
		}
		ref Color32 reference = ref frameTones[currentFrame];
		reference = new Color32((byte)(num / (uint)array.Length), (byte)(num2 / (uint)array.Length), (byte)(num3 / (uint)array.Length), (byte)(num4 / (uint)array.Length));
		int num5 = currentFrame - 1;
		int num6 = 0;
		while (num6 < options.maxLookbackFrames && num5 >= 0)
		{
			if (duplicateOf[num5] < 0)
			{
				stats.totalFramesCompared++;
				if (Mathf.Abs(SqrPixelDiff(frameTones[currentFrame], frameTones[num5])) <= options.toneCompareDistrust * options.toneCompareDistrust)
				{
					stats.pixelCacheQueries++;
					Color32[] b;
					if (pixelCache != null && currentFrame - num5 < pixelCache.Length)
					{
						stats.pixelCacheHits++;
						b = pixelCache[num5 % pixelCache.Length];
					}
					else
					{
						videoDecoder.Decode(num5 + frameOffset);
						b = framebuffer.GetPixels32();
					}
					stats.framesPartiallyCompared++;
					ImageDiff(out var sqrImageDiff, out var maxPixelDiff, array, b, fasterPixelCompare: true, 53);
					if (sqrImageDiff <= options.maxImageDiff && maxPixelDiff <= options.maxPixelDiff)
					{
						stats.framesFullyCompared++;
						ImageDiff(out sqrImageDiff, out maxPixelDiff, array, b);
						if (sqrImageDiff <= options.maxImageDiff && maxPixelDiff <= options.maxPixelDiff)
						{
							duplicateOf[currentFrame] = num5;
							stats.duplicateCount++;
							break;
						}
					}
				}
				if (!options.otherStreamsAvailable)
				{
					num6++;
				}
			}
			if (options.otherStreamsAvailable)
			{
				num6++;
			}
			num5--;
		}
		currentFrame++;
		return true;
	}

	private static int SqrPixelDiff(Color32 c1, Color32 c2)
	{
		int num = c1.r - c2.r;
		int num2 = c1.g - c2.g;
		int num3 = c1.b - c2.b;
		int num4 = c1.a - c2.a;
		return num * num + num2 * num2 + num3 * num3 + num4 * num4;
	}

	private static void ImageDiff(out float sqrImageDiff, out int maxPixelDiff, Color32[] a, Color32[] b, bool fasterPixelCompare = false, int considerEveryNthPixel = 1)
	{
		long num = 0L;
		maxPixelDiff = 0;
		if (considerEveryNthPixel < 1)
		{
			considerEveryNthPixel = 1;
		}
		int num2 = a.Length;
		if (fasterPixelCompare)
		{
			for (int i = 0; i < num2; i += considerEveryNthPixel)
			{
				int num3 = a[i].g - b[i].g;
				if (num3 > maxPixelDiff)
				{
					maxPixelDiff = num3;
				}
				num += num3 * num3;
			}
			num *= 4;
		}
		else
		{
			for (int j = 0; j < num2; j += considerEveryNthPixel)
			{
				int num4 = SqrPixelDiff(a[j], b[j]);
				num += num4;
				if (num4 > maxPixelDiff)
				{
					maxPixelDiff = num4;
				}
			}
			maxPixelDiff = Mathf.RoundToInt(Mathf.Sqrt(maxPixelDiff));
		}
		sqrImageDiff = (float)((double)num / (double)a.Length);
	}
}
