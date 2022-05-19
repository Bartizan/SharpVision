using System;
using TurboVision.Dialogs;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.History
{
	public class HistoryWindow : Window
	{

		private static char[] CHistoryWindow = ("\x13\x13\x15\x18\x19\x13\x14").ToCharArray();

        ListViewer Viewer = null;

        public HistoryWindow( Rect Bounds, int HistoryId):base( Bounds, "", wnNoNumber)
		{
            Flags = WindowFlags.wfClose;
            InitViewer(HistoryId);
        }

		public override char[] GetPalette()
		{
			return CHistoryWindow;
		}

        public string GetSelection()
        {
            return Viewer.GetText(Viewer.Focused, 255);
        }

        public void InitViewer(int HistoryId)
        {
            Rect R = GetExtent();
            R.Grow(-1, -1);
            Viewer = new HistoryViewer(
                R, StandardScrollBar(sbHorizontal + sbHandleKeyboard),
                StandardScrollBar(sbVertical + sbHandleKeyboard),
                HistoryId);
            Insert(Viewer);
        }
    }
}
