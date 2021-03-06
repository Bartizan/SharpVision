using System;
using System.Runtime.InteropServices;

using TurboVision.Objects;

namespace TurboVision.Views
{
    /// <summary>
    /// TView is the fundamental object for Turbo Vision. Every visible object in
    /// Turbo Vision must be a descendent of this object.
    /// TView defines a section, or rectangle, of the screen.
    /// It must be able to draw itself at any time (when the Draw method is called)
    /// and handle any events directed to it through the HandleEvent.
    /// TView objects are rarely instantiated in Turbo Vision programs.
    /// The TView object type exists to provide basic fields and methods for its
    /// descendants.
    /// </summary>
    [Serializable]
	public class View : Objects.Object
	{
		protected const byte ErrorAttr = 0xCF;

		private static bool commandsetChanged = false;

		private static System.Collections.BitArray curCommandSet = 
			new System.Collections.BitArray( 256, true);

		public const uint hcNoContext = 0;
		public const uint hcDragging = 1;

		public const uint hcUndo = 0xFF10;
		public const uint hcCut = 0xFF11;
		public const uint hcCopy = 0xFF12;
		public const uint hcPaste = 0xFF13;
		public const uint hcClear = 0xFF14;

		public const int cmValid = 0;
		public const int cmQuit = 1;
		public const int cmError = 2;
		public const int cmMenu = 3;
		public const int cmClose = 4;
		public const int cmZoom = 5;
		public const int cmResize = 6;
		public const int cmNext = 7;
		public const int cmPrev = 8;
		public const int cmHelp = 9;
		public const int cmOk = 10;
		public const int cmCancel = 11;
		public const int cmYes     = 12;
		public const int cmNo      = 13;
		public const int cmDefault = 14;

		public const int cmCut = 20;
		public const int cmCopy = 21;
		public const int cmPaste = 22;
		public const int cmUndo = 26;
		public const int cmClear = 24;
		public const int cmTile    = 25;
		public const int cmCascade = 26;

		public const int cmReceivedFocus     = 50;
		public const int cmReleasedFocus     = 51;
		public const int cmCommandSetChanged = 52;
		public const int cmScrollBarChanged  = 53;
		public const int cmScrollBarClicked  = 54;
		public const int cmSelectWindowNum     = 55;
		public const int cmListItemSelected  = 56;

		public const int cmRecordHistory  = 60;
		public const int cmGrabDefault    = 61;
		public const int cmReleaseDefault = 62;

		public string ldFrameChars = "   └ │┌├ ┘─┴┐┤┬┼   ╚ ║╔║ ╝══╗║═ '";
		public string ldMenuFrameChars = " ┌─┐  └─┘  │ │  ├─┤ ";
		public const char ldVerticalBar = '│';
		public const char ldLeftScroll = '';
		public const char ldRightScroll = '';
		public const char ldBlockFull = '█';
		public const char ldBlockBottom = '▄';
		public const char ldBlockTop = '▀';
		public const char ldSubMenuArrow = '►';
		public const char ldDblHorizontalBar = '═';
		public const char ldHorizontalBar = '─';
		public const char ldModified = '☼';
		public const char ldDesktopBackground = '░';
		public string ldHistoryDropDown = "▐~↓~▌";
		public string ldCloseWindow = "[~■~]";
		public string ldCloseClicked = "[~☼~]";
		public string ldMaximize = "[~↑~]";
		public string ldBottomRight = "~─┘~";

		public const byte sbLeftArrow  = 0;
		public const byte sbRightArrow = 1;
		public const byte sbPageLeft   = 2;
		public const byte sbPageRight  = 3;
		public const byte sbUpArrow    = 4;
		public const byte sbDownArrow  = 5;
		public const byte sbPageUp     = 6;
		public const byte sbPageDown   = 7;
		public const byte sbIndicator  = 8;

		public View Next;

		public const byte wnNoNumber = 0;

		public static bool ShowMarkers = false;

		public const string SpecialChars = "по☻←  ";

		public static View TheTopView;
		public static DrawBuffer StaticVar1 = new DrawBuffer( 255);

		[Flags]
		public enum StateFlags
		{
			sfVisible = 0x0001,
			sfCursorVis = 0x0002,
			sfCursorIns = 0x0004,
			sfShadow = 0x0008,
			sfActive = 0x0010,
			sfSelected = 0x0020,
			sfDragging = 0x0080,
			sfFocused = 0x0040,
			sfDisabled = 0x0100,
			sfModal = 0x0200,
			sfExposed = 0x0800,
		}

		[Flags]
		public enum DragModes
		{
			dmDragMove = 0x01,
			dmDragGrow = 0x02,
			dmLimitLoX = 0x10,
			dmLimitLoY = 0x20,
			dmLimitHiX = 0x40,
			dmLimitHiY = 0x80,
			dmLimitAll = 0xF0,
		}

		[Flags]
		public enum GrowModes
		{
			gfGrowLoX = 0x01,
			gfGrowLoY = 0x02,
			gfGrowHiX = 0x04,
			gfGrowHiY = 0x08,
			gfGrowRel = 0x10,
			gfGrowAll = 0x0F,
		}

		public enum HelpContexts
		{
			hcNoContext = 0,
			hcDragging = 1,
		}

		[Flags]
		public enum EventMasks
		{
			evMouseDown = 0x0001,
			evMouseAuto = 0x0008,
			evKeyDown = 0x0010,
			evCommand = 0x0100,
			evBroadcast = 0x0200,
			evAll = evCommand | evBroadcast | evKeyDown | evMouseDown,
		}

		[Flags]
		public enum OptionFlags
		{
			ofSelectable  = 0x0001,
			ofTopSelect   = 0x0002,
			ofFirstClick  = 0x0004,
			ofFramed      = 0x0008,
			ofPreProcess  = 0x0010,
			ofPostProcess = 0x0020,
			ofBuffered    = 0x0040,
			ofTileable    = 0x0080,
			ofCenterX     = 0x0100,
			ofCenterY     = 0x0200,
			ofCentered    = 0x0300,
			ofValidate    = 0x0400,
			ofVersion     = 0x3000,
			ofVersion10   = 0x0000,
			ofVersion20   = 0x1000,
		}
		[Flags]
		public enum WindowFlags
		{
			wfMove  = 0x01,
			wfGrow  = 0x02,
			wfClose = 0x04,
			wfZoom  = 0x08,
            wfAll   = 0x0F,
		}
		[Flags]
		public enum FrameModes
		{
			fmCloseClicked = 0x0001,
			fmZoomClicked  = 0x0002,
		}

        /// <summary>
        /// Cursor is the location of the hardware cursor within the view.
        /// The cursor is visible only if the view is focused and the cursor turned on.
        /// The shape of the cursor is either underline or block (determined by
        /// sfCursorIns).
        /// </summary>
        public Point Cursor;

        /// <summary>
        /// DragMode determines how the view should behave when mouse-dragged.
        /// </summary>
        public DragModes DragMode;

        /// <summary>
        /// Options determine various behaviors of the view.
        /// </summary>
        public OptionFlags Options = 0;

        /// <summary>
        /// EventMask is a bit mask that determines which event classes will be
        /// recognized by the view.
        /// The default EventMask enables evMouseDown, evKeyDown, and evCommand.
        /// Assigning 0xFFFF to EventMask causes the view to react to all event classes.
        /// A value of 0 causes the view to not react to any events.
        /// </summary>
		public EventMasks EventMask = 0;

        /// <summary>
        /// GrowMode determines how the view will grow when its owner view is
        /// resized.
        /// GrowMode is assigned one or more of the gfXXXX masks.
        /// </summary>
        public GrowModes GrowMode = 0;

        /// <summary>
        /// HelpCtx is the help context of the view.
        /// When the view is focused, this field will represent the help context of the
        /// application unless the context number is hcNoContext.
        /// If the context number is hcNoContext, there is no help context.
        /// </summary>
        public uint HelpCtx;

        /// <summary>
        /// Origin is the (X, Y) coordinates, relative to the owner's Origin, of
        /// the top-left corner of the view.
        /// </summary>
        public Point Origin;

        /// <summary>
        /// Owner points to the TGroup object that owns this view.
        /// </summary>
        public Group Owner;

        /// <summary>
        /// Size is the size of the view.
        /// </summary>
        public Point Size;

        /// <summary>
        /// Many TView methods test and/or alter the State field by calling
        /// SetState.
        /// GetState(AState) returns True if the view's State is AState.
        /// </summary>
        public StateFlags State = 0;

		private StatVar2 StaticVar2;

		static View()
		{
			ShadowSize.X = 2;
			ShadowSize.Y = 1;
		}

        /// <summary>
        /// TView.Init creates a TView object with the given Bounds rectangle.
        /// Syntax:
        ///     constructor Init(var Bounds: TRect);
        /// TObject.Init will zero all fields in TView descendants.
        /// </summary>
        /// <param name="Bounds"></param>
        public View(Rect Bounds):base()
        {
			Owner = null;
			State = StateFlags.sfVisible;
			SetBounds( Bounds);
			DragMode = DragModes.dmLimitLoY;
			HelpCtx = hcNoContext;
			EventMask = EventMasks.evMouseDown | EventMasks.evKeyDown | EventMasks.evCommand;
		}

        public static Point ShadowSize;
		public byte ShadowAttr = 0x08;

		private static bool commandEnabled( int Command)
		{
			return (( Command > 255) || curCommandSet.Get( (int)Command));
		}

		public void SetBounds( Rect Bounds)
		{
			Origin = Bounds.A;
			Size = Bounds.B;
			Size -= Origin;
		}

		public virtual void ChangeBounds( Rect R)
		{
			SetBounds( R);
			DrawView();
		}

		public void SetCursor( int X, int Y)
		{
			Cursor.X = X;
			Cursor.Y = Y;
			DrawCursor();
		}

		public virtual bool Valid(  int Command)
		{
			return true;
		}

		public Rect GetExtent()
		{
			return new Rect( 0, 0, Size.X, Size.Y);
		}

		public Rect GetBounds()
		{
			Rect R = new Rect();
			R.A = Origin;
			R.B.X = Origin.X + Size.X;
			R.B.Y = Origin.Y + Size.Y;
			return R;
		}

		public bool GetState( StateFlags State)
		{
			return (State & this.State) == State;
		}

		public virtual uint DataSize()
		{
			return 0;
		}

		public virtual int Execute()
		{
			return cmCancel;
		}

		public virtual uint GetHelpCtx()
		{
			if( ((StateFlags)State & StateFlags.sfDragging) !=0 )
				return (uint)HelpContexts.hcDragging;
			else
				return HelpCtx;
		}

		public bool CommandSetChanged
		{
			get
			{
				return commandsetChanged;
			}
			set
			{
				commandsetChanged = value;
			}
		}

		public System.Collections.BitArray CurCommandSet
		{
			get
			{
				return curCommandSet;
			}
			set
			{
				curCommandSet = value;
			}
		}

		public bool CommandEnabled( int Command)
		{
			return commandEnabled( Command);
		}

		public void DisableCommands( params int[] Commands)
		{
			foreach( int b in Commands)
			{
				if( CurCommandSet.Get( (int)b))
				{
					CurCommandSet.Set( (int)b, false);
					CommandSetChanged = true;
					break;
				}
			}
		}

		public void DisableCommand( int Command)
		{
			DisableCommands( new int[1]{Command});
		}

		public void EnableCommands( int[] Commands)
		{
			foreach( int b in Commands)
			{
				if( ! CurCommandSet.Get( (int)b))
				{
					CurCommandSet.Set( (int)b, true);
					CommandSetChanged = true;
					break;
				}
			}
		}

		public void SetCommands( int[] Commands)
		{
			CurCommandSet.SetAll( false);
			foreach( int b in Commands)
			{
				CurCommandSet.Set( (int)b, true);
			}
			CommandSetChanged = true;
		}

		public int[] GetCommands()
		{
			System.Collections.ArrayList s = new System.Collections.ArrayList();
			for( int i = 0; i < CurCommandSet.Length; i++)
				if( CurCommandSet.Get(i))
					s.Add(i);
			return (int[])s.ToArray( typeof(int));
		}

		public View NextView()
		{
			if( this == Owner.Last)
				return null;
			else
				return Next;
		}

		public View Prev
		{
			get
			{
				View Result = this;
				while( (Result != null) && ( Result.Next != this))
					Result = Result.Next;
				return Result;
			}
		}

		public View PrevView
		{
			get
			{
				if( this == Owner.First)
					return null;
				else
					return Prev;
			}
		}

		public Point MakeGlobal( Point P)
		{
			View cur = this;
			Point Dest;
			Dest.X = P.X;
			Dest.Y = P.Y;
			Dest += Origin;
			while( cur.Owner != null)
			{
				cur = cur.Owner;
				Dest += Origin;
			}
			return Dest;
		}

		public Point MakeLocal( Point P)
		{
			View cur = this;
			Point Dest;
			Dest = P;
			Dest -= Origin;
			while( cur.Owner != null)
			{
				cur = cur.Owner;
				Dest -= cur.Origin;
			}
			return Dest;
		}

		public View TopView
		{
			get
			{
				View P;
				if( TheTopView == null)
				{
					P = this;
					while( (P != null) &&( (P.State & StateFlags.sfModal) == 0))
					{
						P = P.Owner;
					}
					return P;
				}
				else
					return TheTopView;
			}
		}

		public void do_WriteViewRec1( long x1, long x2, View p, int shadowCounter)
		{
			Group G;
			char d;
			long BufPos, SrcPos, l, dx;

			do
			{
				p = p.Next;
				if( p == null)
					break;
				if( p == StaticVar2.Target)
				{
					G = p.Owner;
					if( G.Buffer != null)
					{
						BufPos = G.Size.X * StaticVar2.Y + x1;
						SrcPos = x1 - StaticVar2.Offset;
						l = x2 - x1;
						//if( shadowCounter == 0)
						{
							for( int i = 0; i < l; i++)
							{
								G.Buffer[BufPos*2 + i*2] = (char)(StaticVar1[ (int)SrcPos + i].AsciiChar);
								G.Buffer[BufPos*2 + i*2 + 1] = (char)(StaticVar1[ (int)SrcPos + i].Attribute);
							}
						}
                        if (shadowCounter != 0)
						{
							while( l > 0)
							{
								d = (char)(StaticVar1[(int)SrcPos].Attribute & ShadowAttr);
								G.Buffer[BufPos*2 + 1] = d;
								BufPos ++;
								SrcPos ++;
								l--;
							}
						}

						if ( G.Buffer == ScreenManager.ScreenBuffer)
							ScreenManager.ConsoleOutput( (int)x1, (int)x2, (int)StaticVar2.Y);
					}
					if( G.LockFlag == 0)
						do_WriteViewRec2( x1, x2, G, shadowCounter);
					break;
				}
				if( ((p.State & StateFlags.sfVisible) != 0) && (StaticVar2.Y >= p.Origin.Y))
				{
					if( StaticVar2.Y < (p.Origin.Y + p.Size.Y))
					{
						if( x1 < p.Origin.X)
						{
							if( x2 <= p.Origin.X)
								continue;
							do_WriteViewRec1( x1, p.Origin.X, p, shadowCounter);
							x1 = p.Origin.X;
						}
						dx = p.Origin.X + p.Size.X;
						if( x2 < dx)
							return;
						if( x1 < dx)
							x1 = dx;
						dx += ShadowSize.X;
						if( ((p.State & StateFlags.sfShadow) != 0) && ( StaticVar2.Y >= p.Origin.Y + ShadowSize.Y))
							if ( x1 > dx)
								continue;
							else
							{
								shadowCounter ++;
								if( x2 <= dx)
									continue;
								else
								{
									do_WriteViewRec1( x1, dx, p, shadowCounter);
									x1 = dx;
									shadowCounter --;
									continue;
								}
							}
						else
							continue;
					}
					if( ((p.State & StateFlags.sfShadow) != 0) && ( StaticVar2.Y < (p.Origin.Y + p.Size.Y + ShadowSize.Y)))
					{
						dx = p.Origin.X + ShadowSize.X;
						if( x1 < dx)
						{
							if( x2 < dx)
								continue;
							do_WriteViewRec1( x1, dx, p, shadowCounter);
							x1 = dx;
						}
						dx += p.Size.X;
						if( x1 > dx)
							continue;
						shadowCounter ++;
						if( x2 < dx)
							continue;
						else
						{
							do_WriteViewRec1( x1, dx, p, shadowCounter);
							x1 = dx;
							shadowCounter --;
						}
					}
				}
			}while( true);
		}

		public void do_WriteViewRec2( long x1, long x2, View p, int shadowCounter)
		{
			StatVar2 SavedStatics;
			long dx;
			Group G;

			G = p.Owner;

			if( ((p.State & StateFlags.sfVisible) != 0) && ( G != null))
			{
				SavedStatics = StaticVar2;
				StaticVar2.Y += p.Origin.Y;
				dx = p.Origin.X;
				x1 += dx;
				x2 += dx;
				StaticVar2.Offset += dx;
				StaticVar2.Target = p;
				if( (StaticVar2.Y >= G.Clip.A.Y) && (StaticVar2.Y < G.Clip.B.Y))
				{
					if( x1 < G.Clip.A.X)
						x1 = G.Clip.A.X;
					if( x2 > G.Clip.B.X)
						x2 = G.Clip.B.X;
					if( x1 < x2)
						do_WriteViewRec1( x1, x2, G.Last, shadowCounter);
				}
				StaticVar2 = SavedStatics;
			}
		}

		public void WriteView( long x1, long x2, long y, DrawBuffer Buf)
		{
			if( (y >= 0) && ( y < Size.Y))
			{
				if( x1 < 0)
					x1 = 0;
				if( x2 > Size.X)
					x2 = Size.X;
				if ( x1 < x2)
				{
					StaticVar2.Offset = x1;
					StaticVar2.Y = y;
					StaticVar1.drawBuffer = Buf.drawBuffer;
					do_WriteViewRec2( x1, x2, this, 0);
				}
			}
		}

		private bool do_ExposedRec1( long x1, long x2, View p)
		{
			Group G;
			long dx, dy;

			while( true)
			{
				p = p.Next;
				G = p.Owner;
				if( p == StaticVar2.Target)
				{
					return do_ExposedRec2( x1, x2, G);
				}
				dx = p.Origin.X;
				dy = p.Origin.Y;
				if( ((p.State & StateFlags.sfVisible) != 0) & ( StaticVar2.Y >= dy))
				{
					if( StaticVar2.Y < (dy + p.Size.Y))
					{
						if( x1 < dx)
						{
							if( x2 <= dx)
								continue;
							if( x2 > ( dx + p.Size.X))
							{
								return true;
							}
							x1 = dx + p.Size.X;
						}
						else
							x2 = dx;
					}
					else
					{
						if( x1 < ( dx + p.Size.X))
							x1 = dx + p.Size.X;
						if( x1 > x2)
						{
							return false;
						}
					}
				}
			}
		}

		private bool do_ExposedRec2( long x1, long x2, View p)
		{

			Group G;
			StatVar2 SavedStat;
			bool Result;

			if( (p.State & StateFlags.sfVisible) == 0)
				return false;
			else
			{
				G = p.Owner;
				if( (G == null) || (G.Buffer != null))
					Result = true;
				else
				{
					SavedStat = StaticVar2;
					StaticVar2.Y += p.Origin.Y;
					x1 += p.Origin.X;
					x2 += p.Origin.X;
					StaticVar2.Target = p;
					if( (StaticVar2.Y < G.Clip.A.Y) || ( StaticVar2.Y >= G.Clip.B.Y))
						Result = false;
					else
					{
						if( x1 < G.Clip.A.X)
							x1 = G.Clip.A.X;
						if( x2 > G.Clip.B.X)
							x2 = G.Clip.B.X;
						if( x1 >= x2)
							Result = false;
						else
							Result = do_ExposedRec1( x1, x2, this);
					}
					StaticVar2 = SavedStat;
				}
			}
			return Result;
		}

		public bool Exposed()
		{
			bool OK;
			int y;
			bool Result;

			if( ((State & StateFlags.sfExposed) != 0) && 
				( Size.X > 0) &&
				( Size.Y > 0))
			{
				OK = false;
				y = 0;
				while( (y < Size.Y) && !OK )
				{
					StaticVar2.Y = y;
					OK = do_ExposedRec2( 0, Size.X, this);
					y++;
				}
				Result = OK;
			}
			else
				Result = false;
			return Result;
		}

		public void WriteLine( int X, int Y, int W, int H, DrawBuffer B)
		{
			if( H == 0)
				return;
			for( int i = 0; i < H; i++)
				WriteView( X, X+W, Y+i, B);
		}

		public void WriteBuf( int x, int y, int w, int h, DrawBuffer Buf)
		{
            WriteBuf(x, y, w, h, Buf, 0);
        }

        public void WriteBuf(int X, int Y, int W, int H, DrawBuffer Buf, int StartPos)
        {
            DrawBuffer V = new DrawBuffer(Size.X * Size.Y);
            if (H > 0)
            {
                for (int i = 0; i < H; i++)
                {
                    for (int j = W * i; j < W * (i + 1); j++)
                        V[j - W * i] = Buf[j + StartPos];
                    WriteView(X, X + W, Y + i, V);
                }
            }
        }

        public void WriteBuf(int X, int Y, int W, int H, char[] Buf)
        {
            WriteBuf(X, Y, W, H, Buf, 0);
        }

        public void WriteBuf(int X, int Y, int W, int H, char[] Buf, int StartPos)
        {
            DrawBuffer V = new DrawBuffer(Size.X * Size.Y);
            if (H > 0)
            {
                for (int i = 0; i < H; i++)
                {
                    for (int j = W * i; j < W * (i + 1); j++)
                    {
                        V.drawBuffer[j - W * i].AsciiChar = Buf[j * 2 + StartPos];
                        V.drawBuffer[j - W * i].Attribute = Buf[j * 2 + 1 + StartPos];
                    }
                    WriteView(X, X + W, Y + i, V);
                }
            }
        }

        public void _WriteBuf(int X, int Y, int W, int H, char[] Buf)
        {
			DrawBuffer V = new DrawBuffer( Size.X * Size.Y);
			if( H > 0)
			{
				for( int i = 0; i < H; i++)
				{
					for( int j = W*i; j < W*( i + 1); j++)
					{
						V.drawBuffer[j - W * i].Attribute = Buf[j*2];
						V.drawBuffer[j - W * i].AsciiChar = Buf[j*2 + 1];
					}
					WriteView( X, X + W, Y + i, V);
				}
			}
		}

		public void WriteStr( int x, int Y, string Str, byte Color)
		{
			int l;
			byte myColor;
			DrawBuffer b = new DrawBuffer( Size.X * Size.Y);

			l = Str.Length;
			if( l == 0)
				return;
			myColor = MapColor(Color);
			int p = 0;
			foreach( char c in Str)
			{
				b.drawBuffer[p].Attribute = (char)myColor;
				b.drawBuffer[p].AsciiChar = (char)c;
				p++;
			}
			WriteView(x, x + l, Y, b);
		}

		public DrawBuffer CreateDrawBuffer()
		{
			return new DrawBuffer( Size.X * Size.Y);
		}

		public virtual void Draw()
		{
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			B.FillChar( (char)' ', (char)GetColor(1), (int)Size.X);
			WriteLine( 0, 0, (int)Size.X, (int)Size.Y, B);
		}

		public virtual char[] GetPalette()
		{
			return new char[0];
		}

		public byte MapColor( byte color)
		{
			byte Result;
			View cur;
			char[] p;

			if( color == 0)
				Result = ErrorAttr;
			else
			{
				cur = this;
				do
				{
					p = cur.GetPalette();
					if( p.Length != 0)
						if( p.Length != 0)
						{
							if( color > p.Length)
							{
								Result = ErrorAttr;
								return Result;
							}
							color = (byte)p[color - 1];
							if( color == 0)
							{
								Result = ErrorAttr;
								return Result;
							}
						}
					cur = cur.Owner;
				}while( cur != null);
				Result = color;
			}
			return Result;
		}

		public uint GetColor( uint Color)
		{
			uint h;
			h = Color >> 8;
			if( h != 0)
				h = MapColor( (byte)h);
			return ( h << 8) + MapColor( (byte)(Color & 0xFF));
		}

		public virtual void ResetCursor()
		{
			StateFlags sfV_CV_F = StateFlags.sfVisible | StateFlags.sfCursorVis | StateFlags.sfFocused;
			View p, p2;
			Group G;
			Point cur;
			byte res;
			bool Check0;
			if( (State & sfV_CV_F) == sfV_CV_F)
			{
				p = this;
				cur = Cursor;
				while( true)
				{
					if( ( cur.X < 0) || ( cur.X >= p.Size.X) ||
						( cur.Y < 0) || ( cur.Y >= p.Size.Y))
						break;
					cur += p.Origin;
					p2 = p;
					G = p.Owner;
					if( G == null)
					{
						Drivers.SysTVSetCurPos( cur.X, cur.Y);
						if( (State & StateFlags.sfCursorIns) != 0)
							Drivers.SetCursorType( CursorType.Block);
						else
							Drivers.SetCursorType( CursorType.Underline);
						return;
					}
					if( (State & StateFlags.sfVisible) == 0)
						break;
					p = G.Last;
					res = 0;
					while( res == 0)
					{
						p = p.Next;
						if( p == p2)
						{
							p = p.Owner;
							res = 1;
						}
						else
							if( ((p.State & StateFlags.sfVisible) != 0) &&
							( cur.X > p.Origin.X) &&
							( cur.X < p.Size.X + p.Origin.X) &&
							( cur.Y > p.Origin.Y) &&
							( cur.Y < p.Size.Y + p.Origin.Y))
							res = 2;
					}
					Check0 = res == 2;
					if( Check0)
						break;
				}
			}
			Drivers.SetCursorType( CursorType.Hidden);
		}

		public void DrawCursor()
		{
			if( (State & StateFlags.sfFocused) != 0)
				ResetCursor();
		}

		public void DrawView()
		{
			if( Exposed())
			{
				Draw();
				DrawCursor();
			}
		}

		public Rect GetClipRect()
		{
			Rect Clip = GetBounds();
			if( Owner != null)
				Clip.Intersect( Owner.Clip);
			Clip.Move( -Origin.X, -Origin.Y);
			return Clip;
		}

		public void DrawUnderRect( int AX, int AY, int BX, int BY, View LastView)
		{
			Rect R = new Rect( AX, AY, BX, BY);
			Owner.Clip.Intersect( R);
			Owner.DrawSubViews( NextView(), LastView);
			Owner.Clip = Owner.GetExtent();
		}

		public virtual void SizeLimits( out Point Min, out Point Max)
		{
			Min.X = 0;
			Min.Y = 0;
			if( Owner != null)
				Max = Owner.Size;
			else
			{
				Max.X = int.MaxValue;
				Max.Y = int.MaxValue;
			}
		}

		public void DrawUnderView( bool DoShadow, View View)
		{
			Rect R = GetBounds();
			R.B.X += ShadowSize.X;
			R.B.Y += ShadowSize.Y;
			DrawUnderRect( (int)R.A.X, (int)R.A.Y, (int)R.B.X, (int)R.B.Y, View);
		}

		public void DrawShow( View LastView)
		{
			DrawView();
			if( (State & StateFlags.sfShadow) != 0)
				DrawUnderView( true, LastView);
		}

		public void DrawHide( View LastView)
		{
			DrawCursor();
			DrawUnderView(( State & StateFlags.sfShadow) != 0, LastView);
		}

		public virtual void Awaken()
		{
		}

		public bool MouseInView( Point Mouse)
		{
			return GetExtent().Contains( MakeLocal( Mouse));
		}

        internal int _Range(int val, int min, int max)
        {
            if (val < min)
                return min;
            else
                if (val > max)
                return max;
            else
                return val;
        }

        public void Locate(Rect Bounds)
        {
			Point Min, Max;
			SizeLimits( out Min, out Max);
			Bounds.B.X = Bounds.A.X + _Range( Bounds.B.X - Bounds.A.X, Min.X, Max.X);
			Bounds.B.Y = Bounds.A.Y + _Range( Bounds.B.Y - Bounds.A.Y, Min.Y, Max.Y);
			Rect R = GetBounds();
			if( ! Bounds.Equals(R))
			{
				ChangeBounds(Bounds);
				if( (Owner != null) && ( (State & StateFlags.sfVisible) != 0))
				{
					if( (State & StateFlags.sfShadow) != 0)
					{
						R.Union( Bounds);
						R.B.X = R.B.X + ShadowSize.X;
						R.B.Y = R.B.Y + ShadowSize.Y;
					}
					DrawUnderRect( R.A.X, R.A.Y, R.B.X, R.B.Y, null);
				}
			}
		}

        public delegate void Grow(ref int i);
        public delegate int Range(int Val, int Min, int Max);

        /// <summary>
        /// CalcBounds; When a view's owner changes size, the owner repeatedly
        /// calls CalcBounds and ChangeBounds for all its subviews.
        /// CalcBounds must calculate the new bounds of the view given that its owner's
        /// size has changed by Delta, and return the new bounds in Bounds.
        /// <para>CalcBounds must:
        /// calculate the new bounds of the view given that its owner's size has
        /// changed by Delta, return the new bounds in Bounds.</para>
        /// </summary>
        /// <param name="Delta"></param>
        /// <returns></returns>
        public virtual Rect CalcBounds( ref Point Delta)
        {
            int S = 0, D = 0;
            Point Min, Max;

            Grow Grow = delegate(ref int I)
            {
                if ((GrowMode & GrowModes.gfGrowRel) == 0)
                    I += D;
                else
                    I = (I * S + (( S - D) >> 1)) / ( S - D);
            };

            Range Range = delegate(int vVal, int vMin, int vMax)
            {
                if (vVal < vMin)
                    return vMin;
                else if (vVal > vMax)
                    return vMax;
                else
                    return vVal;
            };

            Rect Bounds = GetBounds();
            S = Owner.Size.X;
            D = Delta.X;
            if ((GrowMode & GrowModes.gfGrowLoX) != 0)
                Grow(ref Bounds.A.X);
            if ((GrowMode & GrowModes.gfGrowHiX) != 0)
                Grow(ref Bounds.B.X);
            if ((Bounds.B.X - Bounds.A.X) > ScreenManager.MaxViewWidth)
                Bounds.B.X = Bounds.A.X + ScreenManager.MaxViewWidth;
            S = Owner.Size.Y;
            D = Delta.Y;
            if ((GrowMode & GrowModes.gfGrowLoY) != 0)
                Grow(ref Bounds.A.Y);
            if ((GrowMode & GrowModes.gfGrowHiY) != 0)
                Grow(ref Bounds.B.Y);
            SizeLimits(out Min, out Max);
            Bounds.B.X = Bounds.A.X + Range(Bounds.B.X - Bounds.A.X, Min.X, Max.X);
            Bounds.B.Y = Bounds.A.Y + Range(Bounds.B.Y - Bounds.A.Y, Min.Y, Max.Y);
            return Bounds;
        }
		public Event CreateEvent()
		{
			Event E = new Event();
			E.What = Event.Nothing;
			return E;
		}

		public virtual void PutEvent( Event Event)
		{
			if( Owner != null)
				Owner.PutEvent( Event);
		}

		public virtual void HandleEvent( ref Event Event)
		{
			if( Event.What == Event.MouseDown)
				if( (( State & (StateFlags.sfSelected | StateFlags.sfDisabled)) == 0) &&
					( (Options & OptionFlags.ofSelectable) != 0))
					if( (!Focus()) || ( (Options & OptionFlags.ofFirstClick) == 0))
						ClearEvent( ref Event);
		}

		public virtual void SetState( StateFlags AState, bool Enable )
		{
			int Command;

			if( Enable)
				State |= AState;
			else
				State &= ~AState;
			if( Owner != null)
				switch( AState)
				{
					case StateFlags.sfVisible :
						if( (Owner.State & StateFlags.sfExposed) != 0 )
							SetState( StateFlags.sfExposed, Enable);
						if( Enable)
							DrawShow(null);
						else
							DrawHide(null);
						if( (Options & OptionFlags.ofSelectable) !=0 )
							Owner.ResetCurrent();
						break;
					case StateFlags.sfCursorVis :
					case StateFlags.sfCursorIns :
						DrawCursor();
						break;
					case StateFlags.sfShadow :
						DrawUnderView( (State & StateFlags.sfShadow) != 0, null);
						break;
					case StateFlags.sfFocused :
						ResetCursor();
						if( Enable)
							Command = cmReceivedFocus;
						else
							Command = cmReleasedFocus;
						Message( Owner, Event.Broadcast, Command, this);
						break;
				}
		}

		public static object Message( View Receiver, int What, int Command, object InfoPtr)
		{
			object Result = null;
			Event Event = new Event();
			
			if( Receiver != null)
			{
				Event.What = What;
				Event.Command = Command;
				Event.InfoPtr = InfoPtr;
				Receiver.HandleEvent( ref Event);
				if( Event.What == Event.Nothing)
					Result = Event.InfoPtr;
			}
			return Result;
		}

		public void Show()
		{
				if( (State & StateFlags.sfVisible) == 0)
					SetState( StateFlags.sfVisible, true);
		}

		public void Hide()
		{
			if( (State & StateFlags.sfVisible) != 0)
				SetState(StateFlags.sfVisible, false);
		}

		public void HideCursor()
		{
			SetState( StateFlags.sfCursorVis, false);
		}

		public void ShowCursor()
		{
			SetState( StateFlags.sfCursorVis, true);
		}

        /// <summary>
        ///  BlockCursor sets sfCursorIns to change the cursor to a solid block.
        ///  The cursor will only be visible if sfCursorVis is also set (and the view is
        ///  visible).
        /// </summary>
		public void BlockCursor()
		{
			SetState( StateFlags.sfCursorIns, true);
		}

		public void NormalCursor()
		{
			SetState( StateFlags.sfCursorIns, false);
		}

		public void Select()
		{
			if( (Options & OptionFlags.ofSelectable) != 0)
				if( (Options & OptionFlags.ofTopSelect) != 0)
					MakeFirst();
			else
				if( Owner != null)
					Owner.SetCurrent( this, SelectMode.NormalSelect);
		}

		public void MoveTo( int X, int Y)
		{
			Locate( new Rect( X, Y, Size.X + X, Size.Y + Y));
		}

		public void GrowTo( int X, int Y)
		{
			Locate( new Rect( Origin.X, Origin.Y, Origin.X + X, Origin.Y + Y));
		}

		public void ClearEvent( ref Event Event)
		{
			Event.What = Event.Nothing;
			Event.InfoPtr = this;
		}

		public virtual void EndModal( int Command)
		{
			if( TopView != null)
				TopView.EndModal( Command);
		}

		protected virtual Event GetEvent()
		{
			if( Owner != null)
				return Owner.GetEvent();
			else
				return new Event();
		}

		public Event KeyEvent()
		{
			Event Result = new Event();
			do
				Result = GetEvent();
			while( Result.What != Event.KeyDown);
			return Result;
		}

		public bool EventAvail()
		{
			Event Event = GetEvent();
			if( Event.What != Event.Nothing)
				PutEvent( Event);
			return Event.What != Event.Nothing;
		}

		public bool Focus()
		{
			bool Result = true;
			if( (State & ( StateFlags.sfSelected | StateFlags.sfModal)) == 0)
			{
				if( Owner != null)
				{
					Result = Owner.Focus();
					if( Result)
						if( (Owner.Current == null) ||
							( (Owner.Current.Options & OptionFlags.ofValidate) == 0) ||
							( Owner.Current.Valid( cmReleasedFocus)))
							Select();
						else
							Result = false;
				}
			}
			return Result;
		}

		public override void Done()
		{
			Hide();
			if( Owner != null)
				Owner.Delete( this);
		}

		public virtual void SetData( params object[] Rec)
		{
		}

		public virtual object[] GetData()
		{
			return new object[0];
		}

		internal void MoveView( View V, View Target)
		{
			Owner.RemoveView( V);
			Owner.InsertView( V, Target);
		}

		public void PutInFrontOf( View Target)
		{
			View P, LastView;

			if( (Owner != null) && ( Target != this) && ( Target != NextView()) &&
				( ( Target == null ) || ( Target.Owner == Owner)))
				if( (State & StateFlags.sfVisible) == 0)
					MoveView( this, Target);
				else
				{
					LastView = NextView();
					if( LastView != null)
					{
						P = Target;
						while( (P != null) && ( P != LastView))
							P = P.NextView();
						if( P == null)
							LastView = Target;
					}
					State = State & ~( StateFlags.sfVisible);
					if( LastView == Target)
						DrawHide( LastView);
					MoveView( this, Target);
					State = State | StateFlags.sfVisible;
					if( LastView != Target)
						DrawShow( LastView);
					if( (Options & OptionFlags.ofSelectable) != 0)
					{
						Owner.ResetCurrent();
						Owner.ResetCursor();
					}
				}
		}

		public void MakeFirst()
		{
			PutInFrontOf( Owner.First);
		}

		public bool MouseEvent( ref Event Event, uint Mask)
		{
			do
			{
				Event = GetEvent();
			}while( ((Event.What & ( Mask | Event.MouseUp)) == 0));
			return Event.What != Event.MouseUp;
		}

		internal void Update( DragModes Mode, ref Point P, int X, int Y)
		{
			if( (Mode & DragModes.dmDragMove) != 0)
			{
				P.X = X;
				P.Y = Y;
			}
		}

		internal void Change( DragModes Mode, ref Point P, ref Point S, int DX, int DY)
		{
			if( ((Mode & DragModes.dmDragMove) != 0) && ( ( Drivers.GetShiftState() & 0x03) == 0))
			{
				P.X += DX;
				P.Y += DY;
			}
			else
				if( ((Mode & DragModes.dmDragGrow) != 0) && ( ( Drivers.GetShiftState() & 0x03) != 0))
			{
				S.X += DX;
				S.Y += DY;
			}
		}

		internal void MoveGrow( Point P, Point S, Point MinSize, Point MaxSize, Rect Limits, DragModes Mode)
		{
			S.X = Math.Min(Math.Max(S.X, MinSize.X), MaxSize.X);
			S.Y = Math.Min(Math.Max(S.Y, MinSize.Y), MaxSize.Y);

			P.X = Math.Min(Math.Max(P.X, Limits.A.X - S.X + 1), Limits.B.X - 1);
			P.Y = Math.Min(Math.Max(P.Y, Limits.A.Y - S.Y + 1), Limits.B.Y - 1);

			if( (Mode & DragModes.dmLimitLoX) != 0)
				P.X = Math.Max( P.X, Limits.A.X);
			if( (Mode & DragModes.dmLimitLoY) != 0)
				P.Y = Math.Max( P.Y, Limits.A.Y);
			if( (Mode & DragModes.dmLimitHiX) != 0)
				P.X = Math.Min( P.X, Limits.B.X - S.X);
			if( (Mode & DragModes.dmLimitHiY) != 0)
				P.Y = Math.Min( P.Y, Limits.B.Y - S.Y);
			Rect R = new Rect(P.X, P.Y, P.X + S.X, P.Y + S.Y);
			Locate( R);
		}

		public void DragView( Event Event, DragModes Mode, Rect Limits, Point MinSize, Point MaxSize)
		{

			Point P, S;
			Rect SaveBounds;

			SetState( StateFlags.sfDragging, true);
			if( Event.What == Event.MouseDown)
			{
				if( (Mode & DragModes.dmDragMove) != 0)
				{
					P.X = Origin.X - Event.Where.X;
					P.Y = Origin.Y - Event.Where.Y;
					do
					{
						Event.Where.X += P.X;
						Event.Where.Y += P.Y;
						MoveGrow( Event.Where, Size, MinSize, MaxSize, Limits, Mode);
					}while( MouseEvent( ref Event, Event.MouseMove));
				}
				else
				{
					P.X = Size.X - Event.Where.X;
					P.Y = Size.Y - Event.Where.Y;
					do
					{
						Event.Where.X += P.X;
						Event.Where.Y += P.Y;
						MoveGrow ( Origin, Event.Where, MinSize, MaxSize, Limits, Mode);
					}while( MouseEvent( ref Event, Event.MouseMove));
				}
			}
			else
			{
				SaveBounds = GetBounds();
				do
				{
					P = Origin;
					S = Size;
					Event = KeyEvent();
                    switch (Event.KeyCode & (KeyboardKeys)0xFF00)
                    {
						case KeyboardKeys.Left :
							Change( Mode, ref P, ref S, -1, 0);
							break;
						case KeyboardKeys.Right :
							Change( Mode, ref P, ref S, 1, 0);
							break;
						case KeyboardKeys.Up :
							Change( Mode, ref P, ref S, 0, -1);
							break;
						case KeyboardKeys.Down :
							Change( Mode, ref P, ref S, 0, 1);
							break;
						case KeyboardKeys.CtrlLeft :
							Change( Mode, ref P, ref S, -8, 0);
							break;
						case KeyboardKeys.CtrlRight :
							Change( Mode, ref P, ref S, 8, 0);
							break;
						case KeyboardKeys.Home :
							Update( Mode, ref P, Limits.A.X, P.Y);
							break;
						case KeyboardKeys.End :
							Update( Mode, ref P, Limits.B.X - S.X, P.Y);
							break;
						case KeyboardKeys.PageUp :
							Update( Mode, ref P, P.X, Limits.A.Y);
							break;
						case KeyboardKeys.PageDown :
							Update( Mode, ref P, P.X, Limits.B.Y - S.Y);
							break;
					}
					MoveGrow( P, S, MinSize, MaxSize, Limits, Mode);
                } while (!((Event.KeyCode == KeyboardKeys.Enter) || (Event.KeyCode == KeyboardKeys.Esc)));
                if (Event.KeyCode == KeyboardKeys.Esc)
                    Locate( SaveBounds);
			}
			SetState( StateFlags.sfDragging, false);
		}
	}

	public struct AnsiCharInfo
	{
		public char Attribute;
		public char AsciiChar;

		public AnsiCharInfo( char cc, char ca)
		{
			Attribute = ca;
			AsciiChar = cc;
		}
	}


	public class DrawBuffer
	{
		public AnsiCharInfo[] drawBuffer = null;

		public DrawBuffer( int Size)
		{
			drawBuffer = new AnsiCharInfo[ Size];
		}

		public AnsiCharInfo this[int index]
		{
			get
			{
				return drawBuffer[index];
			}
			set
			{
				drawBuffer[index] = value;
			}
		}

		public void SetCharInfo( int index, char cc, char ca)
		{
			drawBuffer[index] = new AnsiCharInfo( cc, ca);
		}

		public void FillChar( char c, char a, int count, int startPos)
		{
			for( int i = 0; i < count; i++)
			{
				if( a != 0)
					drawBuffer[i + startPos].Attribute = a;
				if( c != 0)
					drawBuffer[i + startPos].AsciiChar = c;
			}
		}

		public void FillChar( char c, char a, int count)
		{
			FillChar( c, a, count, 0);
		}

		public void FillCStr( string s, uint color, int startPos)
		{
			uint B;
			uint J;
			J = 0;
			for( int i = 0; i < s.Length; i++)
			{
				if( s[i] != '~')
				{
					if( (color & 0xFF) !=0 )
						drawBuffer[J + startPos].Attribute = (char)(color & 0xFF);
                    drawBuffer[J + startPos].AsciiChar = (char)s[i];
                    J++;
				}
				else
				{
					B = (byte)(color >> 8);
					color = (color & 0xFF) << 8;
					color |= B;
				}
			}
		}

		public void FillStr( string s, char color, int startPos)
		{
			for( int i = 0; i < s.Length; i++)
			{
				if( (i + startPos) >= drawBuffer.Length)
					break;
				if( color !=0)
					drawBuffer[i + startPos].Attribute = color;
				drawBuffer[i + startPos].AsciiChar = (char)s[i];
			}
		}

		public void FillBuf( string Source, byte Attr, uint Count)
		{
			FillBuf( Source, Attr, Count, 0);
		}

		public void FillBuf( string Source, byte Attr, uint Count, int startPos)
		{
			for( int i = 0; i < Count; i++)
			{
				if( Attr != 0)
					drawBuffer[i + startPos].Attribute = (char)Attr;
				drawBuffer[i + startPos].AsciiChar = (char)Source[i];
			}
		}

        public void FillBuf(CHAR_INFO[] Source, uint Count, int startPos)
        {
            for (int i = 0; i < Count; i++)
            {
                drawBuffer[i + startPos].Attribute = (char)Source[i + startPos].Attributes;
                drawBuffer[i + startPos].AsciiChar = Source[i + startPos].WideChar;
            }
        }
    }

    [Serializable]
	public struct StatVar2
	{
		public View Target;
		public long Offset;
		public long Y;
	}
}
