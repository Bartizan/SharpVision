using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Help
{
	public class HelpViewer : Scroller
	{

		private static char[] CHelpViewer = ("\x06\x07\x08").ToCharArray();

		public HelpViewer( Rect Bounds, ScrollBar AHScrollBar, ScrollBar AVScrollBar, HelpFile HelpFile, uint Context):base( Bounds, AHScrollBar, AVScrollBar)
		{
		}

        public override char[] GetPalette()
		{
			return CHelpViewer;
		}
	}
}
