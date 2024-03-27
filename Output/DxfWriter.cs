using ImageToDxf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageToDxf.Output
{
    internal class DxfWriter
    {
        private StreamWriter _stream;

        public DxfWriter(Stream stream)
        {
            _stream = new StreamWriter(stream);
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }



        public void BeginHeaderSection()
        {
            _stream.Write("  0\nSECTION\n  2\nHEADER\n");
        }
        public void BeginBlocksSection()
        {
            _stream.Write("  0\nSECTION\n  2\nBLOCKS\n");
        }

        public void BeginEntitiesSection()
        {
            _stream.Write("  0\nSECTION\n  2\nENTITIES\n");
        }
        public void EndSection()
        {
            _stream.Write("  0\nENDSEC\n");
        }
        public void EOF()
        {
            _stream.Write("  0\nEOF\n");
            _stream.Flush();
        }

        public void WriteHeader(string header)
        {
            _stream.Write("999\n");
            _stream.WriteLine(header);
        }

        //public void BlockSquare(string name, Vector2 from, float sideLen) => BlockRect(from, sideLen, sideLen);
        //public void BlockRect(string name, Vector2 from, float sideLenX, float sideLenY)
        //{
        //    BlockArea(name, "  1", 2,
        //        new Vector2(from.X, from.Y),
        //        new Vector2(from.X + sideLenX, from.Y),
        //        new Vector2(from.X + sideLenX, from.Y + sideLenY),
        //        new Vector2(from.X, from.Y + sideLenY),
        //        new Vector2(from.X, from.Y)
        //        );
        //}

        public void BeginBlock(string name, string Alayer, Vector2 basePoint)
        {
            float THICK = 0.1345f;

            _stream.WriteLine(" 0");
            _stream.WriteLine("BLOCK");
            _stream.WriteLine(" 8");
            _stream.WriteLine(Alayer);
            _stream.WriteLine("  2"); //Block name
            _stream.WriteLine("*" + name);
            _stream.WriteLine(" 70"); //Block flags
            _stream.WriteLine("1"); //1 = anonymous block
            _stream.WriteLine(" 10"); //Base X
            WriteFloat(basePoint.X);
            _stream.WriteLine(" 20"); //Base Y
            WriteFloat(basePoint.Y);
            _stream.WriteLine(" 30"); //Base Z
            WriteFloat(0); //Z is ignored
            _stream.WriteLine("  3");
            _stream.WriteLine("*" + name);
            _stream.WriteLine("  1");
            _stream.WriteLine();
            //_stream.WriteLine(" 40");
            //WriteFloat(THICK);
            //_stream.WriteLine(" 41");
            //WriteFloat(THICK);

            //foreach (Vector2 vertice in vertices)
            //{
            //    WriteNeoVertex(vertice, HEIGHT, Alayer);
            //}


        }

        public void EndBlock(string ALayer)
        {
            _stream.WriteLine(" 0");
            _stream.WriteLine("ENDBLK");
            _stream.WriteLine("5"); //Handle
            _stream.WriteLine("3A7");
            _stream.WriteLine("8");
            _stream.WriteLine(ALayer);

        }

        public void WriteSolid(float x1, float x2, float y1, float y2, string Alayer)
        {
            _stream.WriteLine("  0");
            _stream.WriteLine("SOLID");
            _stream.WriteLine("  5"); //Handle next
            _stream.WriteLine("3A3");
            _stream.WriteLine("  8"); //Layer next
            _stream.WriteLine(Alayer);
            _stream.WriteLine("  6");
            _stream.WriteLine("CONTINUOUS");
            _stream.WriteLine(" 62"); //COLOR
            _stream.WriteLine("   242");

            _stream.WriteLine(" 10");
            WriteFloat(x1);
            _stream.WriteLine(" 20");
            WriteFloat(y1);
            _stream.WriteLine(" 30");
            WriteFloat(0);

            _stream.WriteLine(" 11");
            WriteFloat(x1);
            _stream.WriteLine(" 21");
            WriteFloat(y2);
            _stream.WriteLine(" 31");
            WriteFloat(0);

            _stream.WriteLine(" 12");
            WriteFloat(x2);
            _stream.WriteLine(" 22");
            WriteFloat(y1);
            _stream.WriteLine(" 32");
            WriteFloat(0);

            _stream.WriteLine(" 13");
            WriteFloat(x2);
            _stream.WriteLine(" 23");
            WriteFloat(y2);
            _stream.WriteLine(" 33");
            WriteFloat(0);


            _stream.WriteLine("210");
            WriteFloat(0);
            _stream.WriteLine("220");
            WriteFloat(0);
            _stream.WriteLine("230");
            WriteFloat(-1);
        }

        public void WriteBlockEntity(string layer, string blockName, float scale)
        {
            _stream.WriteLine("  0");
            _stream.WriteLine("INSERT");
            _stream.WriteLine("  5");
            _stream.WriteLine("344");
            _stream.WriteLine(" 8");
            _stream.WriteLine(layer);
            _stream.WriteLine("  6");
            _stream.WriteLine("CONTINUOUS");
            _stream.WriteLine(" 62");
            _stream.WriteLine("   242");
            _stream.WriteLine("  2");
            _stream.WriteLine("*" + blockName);
            _stream.WriteLine(" 10");
            _stream.WriteLine("0.0");
            _stream.WriteLine(" 20");
            _stream.WriteLine("0.0");
            _stream.WriteLine(" 30");
            _stream.WriteLine("0.0");

            _stream.WriteLine(" 41");
            WriteFloat(scale);
            _stream.WriteLine(" 42");
            WriteFloat(scale);
            _stream.WriteLine(" 43");
            WriteFloat(scale);
        }
        public void WriteMultipleHatchEntities(string layer, List<List<Vector2>> polygons)
        {


            foreach (List<Vector2> polygon in polygons)
            {
                _stream.WriteLine("    " + "  0");
                _stream.WriteLine("    " + "HATCH");
                _stream.WriteLine("    " + " 8");
                _stream.WriteLine("    " + layer);
                _stream.WriteLine("    " + " 91");
                _stream.WriteLine("    " + 1);
                _stream.WriteLine("    " + " 92");
                _stream.WriteLine("    " + "2");
                _stream.WriteLine("    " + " 93");
                _stream.Write("    ");
                _stream.WriteLine(polygon.Count);
                foreach (Vector2 corner in polygon)
                {
                    _stream.WriteLine("        " + " 10");
                    _stream.Write("        ");
                    WriteFloat(corner.X);
                    _stream.WriteLine("        " + " 20");
                    _stream.Write("        ");
                    WriteFloat(corner.Y);
                }
            }

        }

        public void WriteHatchEntity(string layer, List<Polygon> polygons)
        {
            const string indent = "    ";
            const string indent2 = indent + indent;
            const string indent3 = indent2 + indent;
            _stream.WriteLine(indent + "  0");
            _stream.WriteLine(indent + "HATCH");
            _stream.WriteLine(indent + " 8");
            _stream.WriteLine(indent + layer);
            _stream.WriteLine(indent + " 70");
            _stream.WriteLine(indent + "1");
            _stream.WriteLine(indent + " 91");
            _stream.WriteLine(indent + polygons.Count);

            foreach (Polygon polygon in polygons)
            {
                _stream.WriteLine(indent2 + " 92");
                _stream.WriteLine(indent2 + "2");
                _stream.WriteLine(indent2 + " 93");
                _stream.WriteLine(indent2 + polygon.Points.Count);
                foreach (Vector2 corner in polygon.Points)
                {
                    _stream.WriteLine(indent3 + " 10");
                    _stream.Write(indent3);
                    WriteFloat(corner.X);
                    _stream.WriteLine(indent3 + " 20");
                    _stream.Write(indent3);
                    WriteFloat(corner.Y);
                }
            }
            _stream.WriteLine(indent + " 75");
            _stream.WriteLine(indent + "0");
        }

        public void WriteHatchEntitySplines(string layer, List<List<Vector2>> polygons)
        {
            const string indent = "    ";
            const string indent2 = indent + indent;
            const string indent3 = indent2 + indent;
            _stream.WriteLine(indent + "  0");
            _stream.WriteLine(indent + "HATCH");
            _stream.WriteLine(indent + " 8");
            _stream.WriteLine(indent + layer);
            _stream.WriteLine(indent + " 70");
            _stream.WriteLine(indent + "1");
            _stream.WriteLine(indent + " 91");
            _stream.WriteLine(indent + polygons.Count);

            foreach (List<Vector2> polygon in polygons)
            {
                _stream.WriteLine(indent2 + " 92");
                _stream.WriteLine(indent2 + "0");
                _stream.WriteLine(indent2 + " 93");
                _stream.WriteLine(indent2 + polygon.Count);
                _stream.WriteLine(indent2 + " 72");
                _stream.WriteLine(indent2 + "4");
                _stream.WriteLine(" 94"); //Degree of the spline curve
                _stream.WriteLine("3");
                //_stream.WriteLine(" 73"); //Rational
                //_stream.WriteLine("1");
                //_stream.WriteLine(" 74"); //Periodic
                //_stream.WriteLine("0");
                _stream.WriteLine(indent2 + " 95"); //Number of knots
                _stream.WriteLine(indent2 + (polygon.Count));
                _stream.WriteLine(indent2 + " 96"); //Number of control points
                _stream.WriteLine(indent2 + polygon.Count);
                for (int i = 0; i < polygon.Count; i++)
                {
                    _stream.WriteLine(indent3 + " 40");
                    _stream.WriteLine(indent3 + "1");
                }
                foreach (Vector2 corner in polygon)
                {
                    _stream.WriteLine(indent3 + " 10");
                    _stream.Write(indent3);
                    WriteFloat(corner.X);
                    _stream.WriteLine(indent3 + " 20");
                    _stream.Write(indent3);
                    WriteFloat(corner.Y);
                }
            }
            _stream.WriteLine(indent + " 75");
            _stream.WriteLine(indent + "0");

        }


        private void WriteFloat(float value)
        {
            if (value >= 0)
                _stream.Write("  ");
            else
                _stream.Write(' ');

            _stream.Write(value.ToString().Replace(',', '.'));
            if (value == 0)
            {
                _stream.Write(".0");
            }
            _stream.Write('\n');
        }
    }
}
