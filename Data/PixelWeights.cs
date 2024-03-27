using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToDxf.Data
{
    public class PixelWeights
    {
        private readonly int[,] Weights;
        public int Width => Weights.GetLength(0);
        public int Height => Weights.GetLength(1);
        public int this[int x, int y] => Weights[x, y];

        public int[,] GetWeightsClone() => (int[,])Weights.Clone();

        public PixelWeights(int width, int height)
        {
            Weights = new int[width, height];
        }

        public static int DefaultColorToWeight(SKColor color)
        {
            
            if (color.Alpha - color.Red < 128 ||
                color.Alpha - color.Green < 128 ||
                color.Alpha - color.Blue < 128)
            {
                return 1;
            }
            return 0;

        }

        public static PixelWeights FromBitmap(SKBitmap bitmap) => FromBitmap(bitmap, DefaultColorToWeight);
        public static PixelWeights FromBitmap(SKBitmap bitmap, Func<SKColor, int> colorToWeight)
        {
            PixelWeights result = new(bitmap.Width, bitmap.Height);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    result.Weights[x, y] = colorToWeight(bitmap.GetPixel(x, y));
                }
            }

            return result;
        }

        public static HatchCollection GenerateHatchData(PixelWeights pixelWeights)
        {
            HatchCollection result = new();

            while (TryExtractNextHatch(pixelWeights, out PixelWeights hatchPixelWeights))
            {
                result.Hatches.Add(new Hatch(hatchPixelWeights));
            }

            return result;
        }

        private static bool TryExtractNextHatch(PixelWeights pixelWeights, out PixelWeights hatchPixelWeights)
        {
            int[,] bitmapPixelWeights = pixelWeights.Weights;
            int width = pixelWeights.Width;
            int height = pixelWeights.Height;

            hatchPixelWeights = new PixelWeights(width, height);
            int[,] hatchData = hatchPixelWeights.Weights;
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    hatchData[i, j] = -1;

            int roughChangeCount = 0; //Note that this is not exact as it will count pixels multiple times due to the column and row preprocessing

            //Column preprocessing: For each column, find the topmost and botmost pixels to be set for the current hatch
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (bitmapPixelWeights[i, j] > 0)
                    {
                        hatchData[i, j] = 1;
                        bitmapPixelWeights[i, j] = -1; //also update pixelsToSet to prevent detecting this pixel in future iterations
                        roughChangeCount++;
                        goto FullBreak;
                        break;
                    }
                    else
                    {
                        hatchData[i, j] = 0;
                    }
                }
                for (int j = height - 1; j >= 0; j--)
                {
                    if (bitmapPixelWeights[i, j] > 0)
                    {
                        hatchData[i, j] = 1;
                        bitmapPixelWeights[i, j] = -1; //also update pixelsToSet to prevent detecting this pixel in future iterations
                        roughChangeCount++;
                        goto FullBreak;
                        break;
                    }
                    else
                    {
                        hatchData[i, j] = 0;
                    }
                }
            }

            //Row preprocessing: For each row, find the leftmost and rightmost pixels to be set for the current hatch
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (bitmapPixelWeights[i, j] > 0)
                    {
                        hatchData[i, j] = 1;
                        bitmapPixelWeights[i, j] = -1; //also update pixelsToSet to prevent detecting this pixel in future iterations
                        roughChangeCount++;
                        goto FullBreak;
                        break;
                    }
                    else
                    {
                        hatchData[i, j] = 0;
                    }
                }
                for (int i = width - 1; i >= 0; i--)
                {
                    if (bitmapPixelWeights[i, j] > 0)
                    {
                        hatchData[i, j] = 1;
                        bitmapPixelWeights[i, j] = -1; //also update pixelsToSet to prevent detecting this pixel in future iterations
                        roughChangeCount++;
                        goto FullBreak;
                        break;
                    }
                    else
                    {
                        hatchData[i, j] = 0;
                    }
                }
            }

        FullBreak:;
            bool hatchFound = roughChangeCount > 0;

            //spread the 0 and 1 values for this hatch, in the end all non-set pixel clusters that touch the border should contain 0, every set pixel cluster that touches a 0 cluster should be set to 1, all other values should be -1
            while (roughChangeCount > 0)
            {
                roughChangeCount = 0;
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        if (bitmapPixelWeights[i, j] > 0)
                        {
                            //If a neighbouring pixel is already set, mark the current pixel as set
                            if ((i > 0 && hatchData[i - 1, j] == 1) ||
                                (i < (width - 1) && hatchData[i + 1, j] == 1) ||
                                (j > 0 && hatchData[i, j - 1] == 1) ||
                                (j < (height - 1) && hatchData[i, j + 1] == 1))
                            {
                                hatchData[i, j] = 1;
                                bitmapPixelWeights[i, j] = -1; //also update pixelsToSet to prevent detecting this pixel in future iterations
                                roughChangeCount++;
                            }
                        }
                        //else if (pixelsToSet[i, j] <= 0)
                        //{
                        //    //If a neighbouring pixel is already clear, mark the current pixel as clear
                        //    if ((i > 0 && hatchData[i - 1, j] == 0) ||
                        //        (i < (pixelsToSet.GetLength(0) - 1) && hatchData[i + 1, j] == 0) ||
                        //        (j > 0 && hatchData[i, j - 1] == 0) ||
                        //        (j < (pixelsToSet.GetLength(1) - 1) && hatchData[i, j + 1] == 0))
                        //    {
                        //        hatchData[i, j] = 0;
                        //        roughChangeCount++;
                        //    }
                        //}
                    }
                }
            }

            return hatchFound;
        }

    }
}
