using System;
using System.Runtime.InteropServices;
using TurboVision.Objects;

namespace TurboVision.Views
{
	public enum WindowPalettes
	{
		wpBlueWindow = 0,
		wpCyanWindow = 1,
		wpGrayWindow = 2,
	}

    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Window : Group
	{

		public static char[] CBlueWindow =
			("\x08\x09\x0A\x0B\x0C\x0D\x0E\x0F" +
            "\x40\x41\x42\x43\x44\x45\x46\x47\x48\x49\x4a\x4b\x4c" + // Dialog Colors
            "\x4d\x4e\x4f\x50\x51\x52\x53\x54\x55\x56\x57\x58" +
            "\x59\x5a\x5b\x5c\x5d\x5e\x5f").ToCharArray();
        public static char[] CCyanWindow =
            ("\x10\x11\x12\x13\x14\x15\x16\x17" +
            "\x60\x61\x62\x63\x64\x65\x66\x67\x68\x69\x6a\x6b\x6c" + // Dialog Colors
            "\x6d\x6e\x6f\x70\x71\x72\x73\x74\x75\x76\x77\x78" +
            "\x79\x7a\x7b\x7c\x7d\x7e\x7f").ToCharArray();
        public static char[] CGrayWindow =
            ("\x18\x19\x1a\x1b\x1c\x1d\x1e\x1f" +
            "\x20\x21\x22\x23\x24\x25\x26\x27\x28\x29\x2a\x2b\x2c\x2d\x2e" + //DialogColors
            "\x2f\x30\x31\x32\x33\x34\x35\x36\x37\x38\x39\x3a\x3b\x3c\x3d" +
            "\x3e\x3f").ToCharArray();

        public const int sbHorizontal = 0x0000;
        public const int sbVertical = 0x0001;
        public const int sbHandleKeyboard = 0x0002;

        private static Point minWinSize;

        private WindowFlags flags;
		private int number;
		public WindowPalettes Palette;
		public Frame Frame;
		private string title = "";

		public Rect ZoomRect;

		static Window()
		{
			minWinSize.X = 16;
			minWinSize.Y = 6;
		}

        public Window():base(
            new Rect( 10, 10, 50, 30))
        {
            State |= StateFlags.sfShadow;
            Options |= (OptionFlags.ofSelectable | OptionFlags.ofTopSelect);
            GrowMode = GrowModes.gfGrowAll | GrowModes.gfGrowRel;
            Flags = WindowFlags.wfMove | WindowFlags.wfZoom | WindowFlags.wfClose | WindowFlags.wfGrow;
            Title = "";
            Number = wnNoNumber;
            Palette = WindowPalettes.wpBlueWindow;
            InitFrame();
            if (Frame != null)
                Insert(Frame);
            ZoomRect = GetBounds();
        }

		public Window( Rect Bounds, string ATitle):base( Bounds)
		{
			State |= StateFlags.sfShadow;
			Options |= ( OptionFlags.ofSelectable | OptionFlags.ofTopSelect);
			GrowMode = GrowModes.gfGrowAll | GrowModes.gfGrowRel;
			Flags = WindowFlags.wfMove | WindowFlags.wfZoom | WindowFlags.wfClose | WindowFlags.wfGrow;
			Title = ATitle;
			Number = wnNoNumber;
			Palette = WindowPalettes.wpBlueWindow;
			InitFrame();
			if( Frame != null)
				Insert( Frame);
			ZoomRect = GetBounds();
		}

		public Window( Rect Bounds, string ATitle, int ANumber):base( Bounds)
		{
			State |= StateFlags.sfShadow;
			Options |= ( OptionFlags.ofSelectable | OptionFlags.ofTopSelect);
			GrowMode = GrowModes.gfGrowAll | GrowModes.gfGrowRel;
			Flags = WindowFlags.wfMove | WindowFlags.wfZoom | WindowFlags.wfClose | WindowFlags.wfGrow;
			Title = ATitle;
			Number = ANumber;
			Palette = WindowPalettes.wpBlueWindow;
			InitFrame();
			if( Frame != null)
				Insert( Frame);
			ZoomRect = GetBounds();
		}

		public virtual void InitFrame()
		{
			Rect R = GetExtent();
			Frame = new Frame( R);
		}

		public virtual string GetTitle( int MaxSize)
		{
			if( title != "")
				return title;
			else
				return "";
		}

		public WindowFlags Flags
		{
			get
			{
				return flags;
			}
			set
			{
				flags = value;
			}
		}
		public int Number
		{
			get
			{
				return number;
			}
			set
			{
				number = value;
			}
		}

		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				if( value == null)
					title = "";
				else
					title = value;
			}
		}

		public Point MinWinSize
		{
			get
			{
				return minWinSize;
			}
		}

		public override char[] GetPalette()
		{
             char[][] P = new char[3][] { CBlueWindow, CCyanWindow, CGrayWindow };
             return P[(int)Palette];
        }

        public override void SizeLimits(out Point Min, out Point Max)
		{
			base.SizeLimits (out Min, out Max);
			Min.X = MinWinSize.X;
			Min.Y = MinWinSize.Y;
		}

		public virtual void Zoom()
		{
			Point Min, Max;
			SizeLimits( out Min, out Max);
			if( (Size.X != Max.X) || ( Size.Y != Max.Y))
			{
				ZoomRect = GetBounds();
				Rect R = new Rect(0, 0, Max.X, Max.Y);
				Locate( R);
			}
			else
				Locate( ZoomRect);
		}

		public override void HandleEvent(ref Event Event)
		{
			Point Min, Max;
			Rect Limits;

			base.HandleEvent (ref Event);
			if( Event.What == Event.evCommand)
				switch( Event.Command)
				{
					case cmResize :
						if( (Flags & ( WindowFlags.wfMove | WindowFlags.wfGrow)) != 0)
						{
							Limits = Owner.GetExtent();
							SizeLimits( out Min, out Max);
							DragView( Event, (DragModes)(DragMode | (DragModes)( Flags & ( WindowFlags.wfMove | WindowFlags.wfGrow))),
								Limits, Min, Max);
							ClearEvent( ref Event);
						}
						break;
					case cmClose :
						if( ((Flags & WindowFlags.wfClose) != 0) &&
							((Event.InfoPtr == null) || ( Event.InfoPtr == this)))
						{
							ClearEvent( ref Event);
							if( (State & StateFlags.sfModal) == 0)
								Close();
							else
							{
								Event.What = Event.evCommand;
								Event.Command = cmCancel;
								Event.InfoPtr = null;
								PutEvent( Event);
								ClearEvent( ref Event);
							}
						}
						break;
					case cmZoom :
						if( ((Flags & WindowFlags.wfZoom) != 0) &&
							(( Event.InfoPtr == null) || ( Event.InfoPtr == this)))
						{
							Zoom();
							ClearEvent( ref Event);
						}
						break;
				}
			else
				if( Event.What == Event.KeyDown)
                switch (Event.KeyCode)
                {
					case KeyboardKeys.Tab :
					{
						FocusNext( false);
						ClearEvent( ref Event);
					}
						break;
					case KeyboardKeys.ShiftTab :
					{
						FocusNext( true);
						ClearEvent( ref Event);
					}
						break;
				}
			else
				if( (Event.What == Event.Broadcast) &&
				( Event.Command == cmSelectWindowNum) &&
				( Event.InfoInt == Number) && 
				( (Options & OptionFlags.ofSelectable) != 0))
			{
				Select();
				ClearEvent( ref Event);
			}
        }

		public virtual void Close()
		{
			if( Valid( cmClose))
				Free();
		}

		public override void SetState( StateFlags AState, bool Enable)
		{
			base.SetState( AState, Enable);
			if( AState == StateFlags.sfSelected)
				SetState( StateFlags.sfActive, Enable);
			if( (AState == StateFlags.sfSelected) || ( (AState == StateFlags.sfExposed) && ( (State & StateFlags.sfSelected) != 0)))
			{
			}
		}

        public ScrollBar StandardScrollBar(int AOptions)
        {
            Rect R = GetExtent();
            if ((AOptions & sbVertical) == 0)
                R = new Rect( R.A.X + 2, R.B.Y - 1, R.B.X - 2, R.B.Y);
            else
                R = new Rect( R.B.X - 1, R.A.Y + 1, R.B.X, R.B.Y - 1);
            ScrollBar S = new ScrollBar( R);
            Insert( S);
            if( (AOptions & sbHandleKeyboard) != 0)
                S.Options |= OptionFlags.ofPostProcess;
            return S;
        }
    }
}
