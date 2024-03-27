using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ImageToDxf.Data
{
    public class Polygon
    {
        public List<Vector2> Points = new();
        private float _CurScale = 1.0f;

        public Polygon()
        {
        }

        public void SetScale(float scale)
        {
            float rescaleFactor = scale / _CurScale;
            _CurScale = scale;
            RescalePoints(rescaleFactor);
        }
        public void Rescale(float scale)
        {
            _CurScale *= scale;
            RescalePoints(scale);
        }
        private void RescalePoints(float rescaleFactor)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = Points[i] * rescaleFactor;
            }
        }

        public static Polygon FromPixelCornerPolygon(PixelCornerPolygon pixelCornerPolygon)
        {
            Polygon result = new();

            Point prevPoint = pixelCornerPolygon.CornerCoordinates[pixelCornerPolygon.CornerCoordinates.Count - 1];
            foreach (Point point in pixelCornerPolygon.CornerCoordinates)
            {
                float targetX = point.X;
                float targetY = point.Y;
                if (point.X % 2 == 0)
                { //vertical border center point
                    targetY += (prevPoint.Y < point.Y) ? -1f : 1f;
                }
                else
                { //horizontal border center point
                    targetX += (prevPoint.X < point.X) ? -1f : 1f;
                }
                result.Points.Add(new Vector2(targetX / 2f, -1f * targetY / 2f));
                prevPoint = point;
            }

            return result;
        }

        public void AddPointsPerEdge(int count)
        {
            List<Vector2> result = new();
            
            Vector2 prev = Points[Points.Count - 1];
            int originalPointCount = Points.Count;
            for (int i = 0; i < originalPointCount; i++)
            {
                Vector2 cur = Points[i];
                float xDiff = cur.X - prev.X;
                float yDiff = cur.Y - prev.Y;
                float xAdd = xDiff / count;
                float yAdd = yDiff / count;
                for(int c = 0; c < count; c++)
                {
                    result.Add(new Vector2(prev.X + (xAdd * c), prev.Y + (yAdd * c)));
                }
                result.Add(cur);
                prev = cur;
            }

            Points = result;
        }

        public void Smoothen2(float factor = 0.33f)
        {

            if (Points.Count <= 32)
            {
                factor = 0.25f;
                AddPointsPerEdge(1);
                //polygonList = InsertMiddlePixels(polygonList);
            }

            Vector2 prevPoint = Points[Points.Count - 1];
            Points.Add(Points[0]);

            for (int i = 0; i < Points.Count - 1; i++)
            {
                Vector2 curPoint = Points[i];
                Vector2 nextPoint = Points[i + 1];

                float xDiff1 = prevPoint.X - curPoint.X;
                float xDiff2 = nextPoint.X - curPoint.X;
                float xDiff = xDiff1 + xDiff2;
                float xAdd = xDiff * factor;

                float yDiff1 = prevPoint.Y - curPoint.Y;
                float yDiff2 = nextPoint.Y - curPoint.Y;
                float yDiff = yDiff1 + yDiff2;
                float yAdd = yDiff * factor;

                Vector2 smoothedPoint = new Vector2(curPoint.X + xAdd, curPoint.Y + yAdd);
                Points[i] = smoothedPoint;

                prevPoint = curPoint;
            }

            Points.RemoveAt(Points.Count - 1);
        }

        public void Smoothen(float factor = 0.33f)
        {
            Vector2 prevPoint = Points[Points.Count - 1];
            Points.Add(Points[0]);

            for (int i = 0; i < Points.Count - 1; i++)
            {
                Vector2 curPoint = Points[i];
                Vector2 nextPoint = Points[i + 1];

                float xDiff1 = prevPoint.X - curPoint.X;
                float xDiff2 = nextPoint.X - curPoint.X;
                float xDiff = xDiff1 + xDiff2;
                float xAdd = xDiff * factor;

                float yDiff1 = prevPoint.Y - curPoint.Y;
                float yDiff2 = nextPoint.Y - curPoint.Y;
                float yDiff = yDiff1 + yDiff2;
                float yAdd = yDiff * factor;

                Vector2 smoothedPoint = new Vector2(curPoint.X + xAdd, curPoint.Y + yAdd);
                Points[i] = smoothedPoint;

                prevPoint = curPoint;
            }

            Points.RemoveAt(Points.Count - 1);
        }
    }
}
