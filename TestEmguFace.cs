using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Dnn;

public class TestEmguFace
{
    public void TestCompilation()
    {
        try
        {
            var names = Enum.GetNames(typeof(FaceRecognizerSF.DisType));
            System.IO.File.WriteAllLines("enum_values.txt", names);
        }
        catch (Exception ex)
        {
            System.IO.File.WriteAllText("enum_values.txt", "Error: " + ex.ToString());
        }
    }
}
