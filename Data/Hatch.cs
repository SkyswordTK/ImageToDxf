using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ImageToDxf.Data
{
    public class Hatch
    {
        public readonly PixelWeights HatchPixelWeights;
        public readonly List<PixelCornerPolygon> PixelCornerPolygons;
        private float _CurScale = 1f;
        public readonly List<Polygon> Polygons = new();

        public Hatch(PixelWeights hatchPixelWeights)
        {
            this.HatchPixelWeights = hatchPixelWeights;
            this.PixelCornerPolygons = PixelCornerPolygon.ProcessHatchWeights(HatchPixelWeights);
        }

        public void UpdatePolygons()
        {
            Polygons.Clear();
            foreach(PixelCornerPolygon pixelCornerPolygon in PixelCornerPolygons)
            {
                Polygon polygon = Polygon.FromPixelCornerPolygon(pixelCornerPolygon);
                if (_CurScale != 1)
                    polygon.Rescale(_CurScale);
                Polygons.Add(polygon);
            }
        }

        public void SetPolygonScale(float scale)
        {
            _CurScale = scale;
            foreach (Polygon polygon in Polygons)
            {
                polygon.SetScale(scale);
            }
        }
        public void RescalePolygons(float scale)
        {
            _CurScale *= scale;
            foreach(Polygon polygon in Polygons)
            {
                polygon.Rescale(scale);
            }
        }

        public void AddPolygonPointsPerEdge(int count)
        {
            foreach (Polygon polygon in Polygons)
            {
                polygon.AddPointsPerEdge(count);
            }
        }

        internal void SmoothenPolygons(float factor)
        {
            foreach (Polygon polygon in Polygons)
            {
                polygon.Smoothen(factor);
            }
        }

        internal void SmoothenSmallPolygons(int maxCornerCount, float smoothenFactor)
        {
            foreach (Polygon polygon in Polygons)
            {
                if (polygon.Points.Count <= maxCornerCount)
                {
                    polygon.Smoothen(smoothenFactor);
                }
            }
        }

        internal void AddPolygonPointsPerEdgeSmallPolygons(int maxCornerCount, int pointCount)
        {
            foreach (Polygon polygon in Polygons)
            {
                if (polygon.Points.Count <= maxCornerCount)
                {
                    polygon.AddPointsPerEdge(pointCount);
                }
            }
        }
    }
}
