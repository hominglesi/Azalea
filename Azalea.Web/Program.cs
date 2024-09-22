using Azalea.Web.Rendering;
using System;
namespace Azalea.Web;
public class Program
{
	static void Main(string[] args) {
		try
		{
			WebGL.MainLoop();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}
}
