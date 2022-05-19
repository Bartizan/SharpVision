using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Editors
{
	public class Memo : Editor
	{

		private static char[] CMemo = ("\x1a\x1b").ToCharArray();

		public Memo( Rect Bounds, ScrollBar AHScrollBar, ScrollBar AVScrollBar, Indicator AIndicator, int ABufSize):base( Bounds, AHScrollBar, AVScrollBar, AIndicator, ABufSize)
		{
		}

        public override char[] GetPalette()
		{
			return CMemo;
		}
	}
}
