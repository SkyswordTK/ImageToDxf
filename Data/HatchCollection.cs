using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ImageToDxf.Data
{
    public class HatchCollection : IEnumerable<Hatch>
    {
        public readonly List<Hatch> Hatches = new();

        public void UpdatePolygons()
        {
            foreach (Hatch hatch in Hatches) { hatch.UpdatePolygons(); }
        }
        public void SetScale(float scale)
        {
            foreach (Hatch hatch in Hatches) { hatch.SetPolygonScale(scale); }
        }
        public void Rescale(float scale)
        {
            foreach (Hatch hatch in Hatches) { hatch.RescalePolygons(scale); }
        }

        public void AddPolygonPointsPerEdge(int count)
        {
            foreach (Hatch hatch in Hatches) { hatch.AddPolygonPointsPerEdge(count); }
        }

        public void SmoothenPolygons(float factor)
        {
            foreach (Hatch hatch in Hatches) { hatch.SmoothenPolygons(factor); }
        }

        public IEnumerator<Hatch> GetEnumerator()
        {
            return Hatches.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void SmoothenSmallPolygons(int maxCornerCount, float smoothenFactor)
        {
            //foreach (Hatch hatch in Hatches) { hatch.SmoothenSmallPolygons(maxCornerCount, smoothenFactor); }
            foreach (Hatch hatch in Hatches) { hatch.SmoothenSmallPolygons(maxCornerCount, smoothenFactor); }
        }

        internal void AddPolygonPointsPerEdgeSmallPolygons(int maxCornerCount, int pointCount)
        {
            foreach (Hatch hatch in Hatches) { hatch.AddPolygonPointsPerEdgeSmallPolygons(maxCornerCount, pointCount); }
        }
    }
}
