﻿using SkiaSharp;
using Accord.Imaging;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics.Distributions.DensityKernels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelSnap
{
    public static class PixelArtConverter
    {
        private static SKBitmap LoadImage(string path)
        {
            using var stream = File.OpenRead(path);
            return SKBitmap.Decode(stream);
        }

        public static void SaveImage(SKBitmap bitmap, string path)
        {
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            File.WriteAllBytes(path, data.ToArray());
        }

        public static SKBitmap ConvertToPixelArt(string path, int pixelSize)
        {
            SKBitmap original = LoadImage(path);
            int width = original.Width / pixelSize;
            int height = original.Height / pixelSize;
            SKBitmap small = original.Resize(new SKImageInfo(width, height), SKFilterQuality.None);
            SKBitmap pixelated = small.Resize(new SKImageInfo(original.Width, original.Height), SKFilterQuality.None);
            return pixelated;
        }
       public static SKBitmap ReduceColors(string path, int clusterCount)
        {
            SKBitmap bitmap = LoadImage(path);
            int width = bitmap.Width;
            int height = bitmap.Height;
            double[][] pixelData = new double[width * height][];

            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = y * width + x;
                    SKColor color = bitmap.GetPixel(x, y);
                    pixelData[idx] = new double[] { color.Red, color.Green, color.Blue };
                }
            });

            KMeans kmeans = new KMeans(clusterCount);
            KMeansClusterCollection clusters = kmeans.Learn(pixelData);
            int[] labels = clusters.Decide(pixelData);

            SKBitmap clusteredBitmap = new SKBitmap(width, height);
            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = y * width + x;
                    int clusterIndex = labels[idx];
                    double[] centroid = clusters.Centroids[clusterIndex];
                    SKColor newColor = new SKColor((byte)centroid[0], (byte)centroid[1], (byte)centroid[2]);
                    clusteredBitmap.SetPixel(x, y, newColor);
                }
            });

            return clusteredBitmap;
        }


    }
}
