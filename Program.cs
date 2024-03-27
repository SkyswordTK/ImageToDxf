global using SkiaSharp;
global using System.Drawing;  //Point
global using System.Numerics; //Vector2
using ImageToDxf;
using ImageToDxf.Data;
using ImageToDxf.Output;

// See https://aka.ms/new-console-template for more information

try
{

    if (args.Length != 2)
    {
        Console.WriteLine("The program requires exactly 2 parameters:");
        Console.WriteLine("\t1.The path to an image (in a common format like bmp, png or jpg) to convert.");
        Console.WriteLine("\t2.The path to an output file (will be overwritten if it exists).");
        Console.WriteLine("Terminating program.");
        Console.ReadLine();
        return;
    }


    string imagePath = args[0]; //  "C:\\Program Files (x86)\\KEYENCE\\KeyenceLaserMarker\\img3.png";
    string dxfPath = args[1]; // "C:\\Program Files (x86)\\KEYENCE\\KeyenceLaserMarker\\DXFConverter\\3912\\dxftemporary.dxf";

    if (!File.Exists(imagePath))
    {
        Console.WriteLine($"Failed to find input image path '{imagePath}'!");
        Console.WriteLine("Terminating program.");
        return;
    }

    string? outputDirectory = Path.GetDirectoryName(dxfPath);
    if (outputDirectory is null || !Directory.Exists(outputDirectory))
    {
        Console.WriteLine($"Failed to find output directory '{outputDirectory}'!");
        Console.WriteLine("Terminating program.");
        return;
    }

    HatchCollection hatchCollection = BitmapProcessor.LoadAndPrepareImageFile(imagePath);
    BitmapProcessor.ProcessHatchCollection(hatchCollection);


    File.Delete(dxfPath);
    using FileStream dxfStream = new FileStream(dxfPath, FileMode.CreateNew, FileAccess.Write);
    DxfWriter dxfWriter = new(dxfStream);


    dxfWriter.BeginHeaderSection();
    dxfWriter.EndSection();


    dxfWriter.BeginBlocksSection();
    dxfWriter.EndSection();


    dxfWriter.BeginEntitiesSection();

    foreach (Hatch hatch in hatchCollection)
    {
        dxfWriter.WriteHatchEntity("0", hatch.Polygons);
    }

    dxfWriter.EndSection();
    dxfWriter.EOF();
} catch (Exception ex)
{
    Console.WriteLine("An error occured during program execution, printing error and terminating. Full Exception:");
    Console.WriteLine(ex.ToString());
    return;
}
