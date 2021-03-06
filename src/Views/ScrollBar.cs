using System;
using TurboVision.Objects;

namespace TurboVision.Views
{
	public class ScrollBar : View
	{
		private static char[] CScrollBar = ("\x04\x04\x05").ToCharArray();

		public int Value;
		public int Min;
		public int Max;
		public int PgStep;
		public int ArStep;
		private string chars;

		private string ldVerticalScroll = "▲▼▒■▓";
		private string ldHorizontalScroll = "◄▒■▓"; 

		public ScrollBar( Rect Bounds):base( Bounds)
		{
			Value = 0;
			Min = 0;
			Max = 0;
			PgStep = 1;
			ArStep = 1;
			if( Size.X == 1)
			{
				GrowMode = GrowModes.gfGrowLoX | GrowModes.gfGrowHiX | GrowModes.gfGrowHiY;
				Chars = ldVerticalScroll;
			}
			else
			{
				GrowMode = GrowModes.gfGrowLoY | GrowModes.gfGrowHiX | GrowModes.gfGrowHiY;
				Chars = ldHorizontalScroll;
			}
		}

		public long GetSize()
		{
			long S;
			if( Size.X == 1)
				S = Size.Y;
			else
				S = Size.X;
			if( S < 3)
				return 3;
			else
				return S;
		}

		public long GetPos()
		{
			long R;
			R = Max - Min;
			if ( R == 0)
				return 1;
			else
				return (( Value - Min) * ( GetSize() - 3) + ( R >> 1)) / R + 1;
		}

		public long ScrollStep( int Part)
		{
			int Step;
			if( (Part & 2) == 0)
				Step = ArStep;
			else
				Step = PgStep;
			if( (Part & 1) == 0)
				return -Step;
			else
				return Step;
		}

		public string Chars
		{
			get
			{
				return chars;
			}
			set
			{
				chars = value;
			}
		}

        public override char[] GetPalette()
		{
			return CScrollBar;
		}

		public void DrawPos( long Pos)
		{
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			long S = GetSize() - 1;
			B.FillChar( (char)chars[0], (char)GetColor(2), 1, 0);
			if( Max == Min)
				B.FillChar( (char)chars[4], (char)GetColor(1), (int)(S - 1), 1);
			else
			{
				B.FillChar( (char)chars[2], (char)GetColor(1), (int)(S - 1), 1);
				B.FillChar( (char)chars[3], (char)GetColor(3), 1, (int)(Pos));
			}
			B.FillChar( (char)chars[1], (char)GetColor(2), 1, (int)S);
			WriteBuf( 0, 0, (int)Size.X, (int)Size.Y, B);
		}

		public override void Draw()
		{
			DrawPos( GetPos());
		}

		public void SetParams( int AValue, int AMin, int AMax, int APgStep, int AArStep)
		{
			if( AMax < AMin)
				AMax = AMin;
			if( AValue < AMin)
				AValue = AMin;
			if( AValue > AMax)
				AValue = AMax;
			int SValue = Value;
			if( (SValue != AValue) || ( Min != AMin) || ( Max != AMax))
			{
				Value = AValue;
				Min = AMin;
				Max = AMax;
				DrawView();
				if( SValue != AValue)
					ScrollDraw();
			}
			PgStep = APgStep;
			ArStep = AArStep;
		}

		public void SetRange( int AMin, int AMax)
		{
			SetParams( Value, AMin, AMax, PgStep, ArStep);
		}

		public void SetStep( int APgStep, int AArStep)
		{
			SetParams( Value, Min, Max, APgStep, AArStep);
		}

		public void ScrollDraw()
		{
			Message( Owner, Event.Broadcast, cmScrollBarChanged, this);
		}

		public void SetValue( int AValue)
		{
			SetParams( AValue, Min, Max, PgStep, ArStep);
		}

		internal void Clicked()
		{
			Message( Owner, Event.Broadcast, cmScrollBarClicked, this);
		}

		internal int GetPartCode( Point Mouse, Rect Extent, int P, int S)
		{
			int Mark, Part;
			Part = -1;
			if( Extent.Contains( Mouse))
			{
				if( Size.X == 1)
					Mark = Mouse.Y;
				else
					Mark = Mouse.X;
				if( Mark == P)
					Part = sbIndicator;
				else
				{
					if( Mark < 1)
						Part = sbLeftArrow;
					else
						if( Mark < P)
						Part = sbPageLeft;
					else
						if( Mark < S)
						Part = sbPageRight;
					else
						Part = sbRightArrow;
					if( Size.X == 1)
						Part += 4;
				}
			}
			return Part;
		}

		public override void HandleEvent(ref Event Event)
		{

			bool Tracking;
			int P, S, ClickPart;
			Point Mouse;
			Rect Extent;

			base.HandleEvent (ref Event);
			int I = 0;
			switch( Event.What)
			{
				case Event.MouseDown :
					Clicked();
					Mouse = MakeLocal( Event.Where);
					Extent = GetExtent();
					Extent.Grow( 1, 1);
					P = (int)GetPos();
					S = (int)GetSize() - 1;
					ClickPart = GetPartCode( Mouse, Extent, P, S);
					if( ClickPart == sbIndicator)
					{
						do
						{
							Mouse = MakeLocal( Event.Where);
							if( GetPartCode( Mouse, Extent, P, S) == ClickPart)
								SetValue( (int)(Value + ScrollStep( ClickPart)));
						}while( MouseEvent( ref Event, Event.MouseAuto));
					}
					else
					{
						do
						{
							Mouse = MakeLocal( Event.Where);
							Tracking = Extent.Contains( Mouse);
							if( Tracking)
							{
								if( Size.X == 1)
									I = Mouse.Y;
								else
									I = Mouse.X;
								if( I < 0)
									I = 1;
								if( I > S)
									I = S - 1;
							}
							else
								I = (int)GetPos();
							if( I != P)
							{
								DrawPos( I);
								P = I;
							}
						}while( MouseEvent( ref Event, Event.MouseMove));
						if( Tracking && ( S > 2))
						{
							S -=2;
							SetValue( (( P - 1 ) * ( Max - Min) + ( S >> 1)) / S + Min );
						}
					}
					ClearEvent( ref Event);
					break;
				case Event.KeyDown :
					if( (State & StateFlags.sfVisible) != 0)
					{
						ClickPart = sbIndicator;
						if( Size.Y == 1)
							switch( Drivers.CtrlToArrow( Event.KeyCode))
							{
								case KeyboardKeys.Left :
									ClickPart = sbLeftArrow;
									break;
								case KeyboardKeys.Right :
									ClickPart = sbRightArrow;
									break;
								case KeyboardKeys.CtrlLeft :
									ClickPart = sbPageLeft;
									break;
								case KeyboardKeys.CtrlRight :
									ClickPart = sbPageRight;
									break;
								case KeyboardKeys.Home :
									I = Min;
									break;
								case KeyboardKeys.End :
									I = Max;
									break;
								default :
									return;
							}
						else
							switch( Drivers.CtrlToArrow( Event.KeyCode))
							{
								case KeyboardKeys.Up :
									ClickPart = sbUpArrow;
									break;
								case KeyboardKeys.Down :
									ClickPart = sbDownArrow;
									break;
								case KeyboardKeys.PageUp :
									ClickPart = sbPageUp;
									break;
								case KeyboardKeys.PageDown :
									ClickPart = sbPageDown;
									break;
								case KeyboardKeys.CtrlPageUp :
									I = Min;
									break;
                                case KeyboardKeys.CtrlPageDown:
									I = Max;
									break;
								default :
									return;
							}
						Clicked();
						if( ClickPart != sbIndicator)
							I = (int)(Value + ScrollStep( ClickPart));
						SetValue( I);
						ClearEvent( ref Event);
					}
					break;
			}
		}
	}
}
