using ImageToDxf.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToDxf
{
    internal class BitmapProcessor
    {
        public static SKBitmap LoadBitmapFromImageFile(string filePath)
        {
            using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using SKImage image = SKImage.FromEncodedData(fileStream);
            return SKBitmap.FromImage(image);
        }

        public static HatchCollection LoadAndPrepareImageFile(string filePath)
        {
            using SKBitmap bitmap = LoadBitmapFromImageFile(filePath);
            return PrepareBitmap(bitmap);
        }

        public static HatchCollection PrepareBitmap(SKBitmap bitmap)
        {
            PixelWeights pixelWeights = PixelWeights.FromBitmap(bitmap);
            return PixelWeights.GenerateHatchData(pixelWeights);
        }

        public static void ProcessHatchCollection(HatchCollection hatches)
        {
            hatches.UpdatePolygons();
            hatches.SmoothenSmallPolygons(32, 0.25f);
            hatches.AddPolygonPointsPerEdgeSmallPolygons(32, 1);
            hatches.SmoothenPolygons(0.25f);
            hatches.AddPolygonPointsPerEdge(1);
            hatches.SmoothenPolygons(0.25f);
            hatches.Rescale(0.05f);
        }


    }
}
