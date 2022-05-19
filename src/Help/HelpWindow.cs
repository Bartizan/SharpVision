using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Help
{

	public class HelpFile : Objects.Object
	{
	}

	public class HelpWindow : Window
	{

		private static char[] CHelpWindow = ("\x80\x81\x82\x83\x84\x85\x86\x87").ToCharArray();

		public HelpWindow( HelpFile HFile, uint Context)
			:base( new Rect(0, 0, 50, 18), "Help", wnNoNumber)
		{
		}

		public override char[] GetPalette()
		{
			return CHelpWindow;
		}
	}
}
