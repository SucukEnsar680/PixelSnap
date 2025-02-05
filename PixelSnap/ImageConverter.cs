using SkiaSharp;
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
            try
            {
                SKBitmap originalBitmap = LoadImage(path);

                // 🔹 Limit large image size to 1024x1024 for performance
                int maxSize = 1024;
                int newWidth = originalBitmap.Width;
                int newHeight = originalBitmap.Height;

                if (newWidth > maxSize || newHeight > maxSize)
                {
                    float scale = Math.Min((float)maxSize / newWidth, (float)maxSize / newHeight);
                    newWidth = (int)(newWidth * scale);
                    newHeight = (int)(newHeight * scale);
                    originalBitmap = originalBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.Medium);
                }

                int width = originalBitmap.Width;
                int height = originalBitmap.Height;
                SKBitmap clusteredBitmap = new SKBitmap(width, height);

                // 🔹 Extract pixel data manually
                SKColor[] pixels = new SKColor[width * height];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixels[y * width + x] = originalBitmap.GetPixel(x, y);
                    }
                }

                // Convert pixel data into a jagged array for KMeans
                double[][] pixelData = new double[pixels.Length][];
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixelData[i] = new double[] { pixels[i].Red, pixels[i].Green, pixels[i].Blue };
                }

                // 🔹 Perform K-Means clustering
                KMeans kmeans = new KMeans(clusterCount)
                {
                    MaxIterations = 10,
                    Tolerance = 0.05
                };
                KMeansClusterCollection clusters = kmeans.Learn(pixelData);
                int[] labels = clusters.Decide(pixelData);

                // 🔹 Function to ensure valid byte range (0-255)
                byte Clamp(double value) => (byte)Math.Max(0, Math.Min(255, value));

                // Assign clustered colors efficiently
                for (int i = 0; i < pixels.Length; i++)
                {
                    int clusterIndex = labels[i];
                    double[] centroid = clusters.Centroids[clusterIndex];
                    pixels[i] = new SKColor(Clamp(centroid[0]), Clamp(centroid[1]), Clamp(centroid[2]));
                }

                // 🔹 Set pixels to new bitmap efficiently
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        clusteredBitmap.SetPixel(x, y, pixels[y * width + x]);
                    }
                }

                return clusteredBitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null; // Handle error gracefully
            }
        }



    }
}
