using Azalea.Graphics.OpenGL.Enums;
using Azalea.Web.Rendering;
using System;
namespace Azalea.Web;
public class Program
{
	static void Main(string[] args)
	{
		try
		{
			WebGL.ClearColor(0.5f, 0.5f, 1f, 1f);
			WebGL.Clear(GLBufferBit.Color);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}
}
