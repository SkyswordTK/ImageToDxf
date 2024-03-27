using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToDxf.Data
{
    public class PixelCornerPolygon
    {
        public readonly List<Point> CornerCoordinates;
        
        public PixelCornerPolygon(List<Point> cornerCoordinates)
        {
            CornerCoordinates = cornerCoordinates;
        }


        public static List<PixelCornerPolygon> ProcessHatchWeights(PixelWeights hatchWeights)
        {
            List<PixelCornerPolygon> result = new();
            int[,] pixelsToSet = hatchWeights.GetWeightsClone();

            //copy the array with a border of "0" ints "around" the original array
            //int[,] pixelsToSetExtended = new int[pixelsToSet.GetLength(0) + 2, pixelsToSet.GetLength(1) + 2];
            bool[,] pixelBorderArray = GeneratePixelBorderArray(pixelsToSet);

            //PrintPixelBorderArray(pixelBorderArray);
            //return result;
            while (TryFindNextBorder(pixelBorderArray, out Point borderStartPosition))
            {
                List<Point> polygonOutline = FindPolygonOutline(pixelBorderArray, borderStartPosition);
                result.Add(new PixelCornerPolygon(polygonOutline));
            }

            return result;
        }

        private enum StepDirection { Left, Right, Up, Down }

        private static void PrintPixelBorderArray(bool[,] pixelBorderArray)
        {
            for (int j = 0; j < pixelBorderArray.GetLength(1); j++)
            {
                for (int i = 0; i < pixelBorderArray.GetLength(0); i++)
                {
                    Console.Write(pixelBorderArray[i, j] ? 'X' : ' ');
                }
                Console.WriteLine();
            }

        }

        private static bool TryFindNextBorder(bool[,] pixelBorderArray, out Point position)
        {
            for (int j = 0; j < pixelBorderArray.GetLength(1); j++)
            {
                for (int i = 0; i < pixelBorderArray.GetLength(0); i++)
                {
                    if (pixelBorderArray[i, j])
                    {
                        position = new Point(i, j);
                        return true;
                    }
                }
            }

            position = Point.Empty;
            return false;
        }

        private static bool[,] GeneratePixelBorderArray(int[,] pixelsToSet)
        {
            int width = pixelsToSet.GetLength(0);
            int height = pixelsToSet.GetLength(1);
            bool[,] result = new bool[width * 2 + 1, height * 2 + 1];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (pixelsToSet[i, j] > 0)
                    { //Invert each border position in order to get only true if exactly 1 of the 2 neighbouring pixels is set
                        int i2 = i * 2;
                        int j2 = j * 2;
                        result[i2 + 0, j2 + 1] = !result[i2 + 0, j2 + 1];
                        result[i2 + 2, j2 + 1] = !result[i2 + 2, j2 + 1];
                        result[i2 + 1, j2 + 0] = !result[i2 + 1, j2 + 0];
                        result[i2 + 1, j2 + 2] = !result[i2 + 1, j2 + 2];
                    }
                }
            }

            return result;
        }


        private static List<Point> FindPolygonOutline(bool[,] pixelBorderArray, Point startPosition)
        {
            List<Point> outlinePositions = new();

            outlinePositions.Add(startPosition);

            Point curPosition = startPosition;
            StepDirection curDirection = StepDirection.Right;
            do
            {
                switch (curDirection)
                {
                    case StepDirection.Right:
                        if (AttemptStepRightDown(pixelBorderArray, ref curPosition, ref curDirection)) break;
                        if (AttemptStepRight(pixelBorderArray, ref curPosition, ref curDirection)) break;
                        if (AttemptStepRightUp(pixelBorderArray, ref curPosition, ref curDirection)) break;
                        throw new InvalidOperationException();
                    case StepDirection.Left:
                        if (AttemptStepLeftUp(pixelBorderArray, ref curPosition, ref curDirection)) break;
                        if (AttemptStepLeft(pixelBorderArray, ref curPosition, ref curDirection)) break;
                        if (AttemptStepLeftDown(pixelBorderArray, ref curPosition, ref curDirection)) break;
                        throw new InvalidOperationException();
                    case StepDirection.Up:
                        if (AttemptStepUpRight(pixelBorderArray, ref curPosition, ref curDirection)) break;
                        if (AttemptStepUp(pixelBorderArray, ref curPosition, ref curDirection)) break;
                        if (AttemptStepUpLeft(pixelBorderArray, ref curPosition, ref curDirection)) break;
                        throw new InvalidOperationException();
                    case StepDirection.Down:
                        if (AttemptStepDownLeft(pixelBorderArray, ref curPosition, ref curDirection)) break;
                        if (AttemptStepDown(pixelBorderArray, ref curPosition, ref curDirection)) break;
                        if (AttemptStepDownRight(pixelBorderArray, ref curPosition, ref curDirection)) break;
                        throw new InvalidOperationException();
                    default: throw new InvalidOperationException();
                }

                outlinePositions.Add(curPosition);
                pixelBorderArray[curPosition.X, curPosition.Y] = false;

                //Console.WriteLine($"{curPosition.X.ToString().PadLeft(3)}  {curPosition.Y.ToString().PadLeft(3)}");
            } while (!curPosition.Equals(startPosition));

            outlinePositions.RemoveAt(outlinePositions.Count - 1);
            return outlinePositions;
        }

        private static bool AttemptStepRightDown(bool[,] arr, ref Point curPosition, ref StepDirection newDirection)
        {
            if (!arr[curPosition.X + 1, curPosition.Y + 1])
                return false;

            curPosition.X += 1;
            curPosition.Y += 1;
            newDirection = StepDirection.Down;
            return true;
        }

        private static bool AttemptStepRight(bool[,] arr, ref Point curPosition, ref StepDirection newDirection)
        {
            if (!arr[curPosition.X + 2, curPosition.Y])
                return false;

            curPosition.X += 2;
            newDirection = StepDirection.Right;
            return true;
        }

        private static bool AttemptStepRightUp(bool[,] arr, ref Point curPosition, ref StepDirection newDirection)
        {
            if (!arr[curPosition.X + 1, curPosition.Y - 1])
                return false;

            curPosition.X += 1;
            curPosition.Y -= 1;
            newDirection = StepDirection.Up;
            return true;
        }


        private static bool AttemptStepLeftUp(bool[,] arr, ref Point curPosition, ref StepDirection newDirection)
        {
            if (!arr[curPosition.X - 1, curPosition.Y - 1])
                return false;

            curPosition.X -= 1;
            curPosition.Y -= 1;
            newDirection = StepDirection.Up;
            return true;
        }

        private static bool AttemptStepLeft(bool[,] arr, ref Point curPosition, ref StepDirection newDirection)
        {
            if (!arr[curPosition.X - 2, curPosition.Y])
                return false;

            curPosition.X -= 2;
            newDirection = StepDirection.Left;
            return true;
        }

        private static bool AttemptStepLeftDown(bool[,] arr, ref Point curPosition, ref StepDirection newDirection)
        {
            if (!arr[curPosition.X - 1, curPosition.Y + 1])
                return false;

            curPosition.X -= 1;
            curPosition.Y += 1;
            newDirection = StepDirection.Down;
            return true;
        }


        private static bool AttemptStepDownLeft(bool[,] arr, ref Point curPosition, ref StepDirection newDirection)
        {
            if (!arr[curPosition.X - 1, curPosition.Y + 1])
                return false;

            curPosition.X -= 1;
            curPosition.Y += 1;
            newDirection = StepDirection.Left;
            return true;
        }
        private static bool AttemptStepDown(bool[,] arr, ref Point curPosition, ref StepDirection newDirection)
        {
            if (!arr[curPosition.X, curPosition.Y + 2])
                return false;

            curPosition.Y += 2;
            newDirection = StepDirection.Down;
            return true;
        }
        private static bool AttemptStepDownRight(bool[,] arr, ref Point curPosition, ref StepDirection newDirection)
        {
            if (!arr[curPosition.X + 1, curPosition.Y + 1])
                return false;

            curPosition.X += 1;
            curPosition.Y += 1;
            newDirection = StepDirection.Right;
            return true;
        }

        private static bool AttemptStepUpRight(bool[,] arr, ref Point curPosition, ref StepDirection newDirection)
        {
            if (!arr[curPosition.X + 1, curPosition.Y - 1])
                return false;

            curPosition.X += 1;
            curPosition.Y -= 1;
            newDirection = StepDirection.Right;
            return true;
        }
        private static bool AttemptStepUp(bool[,] arr, ref Point curPosition, ref StepDirection newDirection)
        {
            if (!arr[curPosition.X, curPosition.Y - 2])
                return false;

            curPosition.Y -= 2;
            newDirection = StepDirection.Up;
            return true;
        }
        private static bool AttemptStepUpLeft(bool[,] arr, ref Point curPosition, ref StepDirection newDirection)
        {
            if (!arr[curPosition.X - 1, curPosition.Y - 1])
                return false;

            curPosition.X -= 1;
            curPosition.Y -= 1;
            newDirection = StepDirection.Left;
            return true;
        }

    }
}
