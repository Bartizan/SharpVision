using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Views
{
	public class Frame : View
	{

		private static char[] CFrame = ("\x01\x01\x02\x02\x03").ToCharArray();
		public FrameModes FrameMode;

		public Frame( Rect Bounds):base( Bounds)
		{
			GrowMode |= ( GrowModes.gfGrowHiX | GrowModes.gfGrowHiY);
			EventMask |= EventMasks.evBroadcast;
		}

		public override char[] GetPalette()
		{
			return CFrame;
		}

		public override void Draw()
		{

			uint CFrame, CTitle;
			int F, I, L, Width;
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			string Title;
			Point Min, Max;

			if( (State & StateFlags.sfDragging) != 0)
			{
				CFrame = 0x0505;
				CTitle = 0x0005;
				F = 0;
			}
			else
				if( (State & StateFlags.sfActive) == 0)
			{
				CFrame = 0x0101;
				CTitle = 0x0002;
				F = 0;
			}
			else
			{
				CFrame = 0x0503;
				CTitle = 0x0004;
				F = 9;
			}
			CFrame = GetColor( CFrame);
			CTitle = GetColor( CTitle);
			Width = (int)Size.X;
			L = Width - 10;
			if( ((Owner as Window).Flags & (WindowFlags.wfClose | WindowFlags.wfZoom)) !=0 )
				L -= 6;
			FrameLine( B, 0, F, (byte)CFrame);
			if( ((Owner as Window).Number != wnNoNumber) && ((Owner as Window).Number) < 10)
			{
				L -= 4;
				if( ((Owner as Window).Flags & WindowFlags.wfZoom) != 0 )
					I = 7;
				else
					I = 3;
				B.drawBuffer[ Width - I].AsciiChar = (char)((Owner as Window).Number + 0x30);
			}
			if( Owner != null)
				Title = (Owner as Window).GetTitle(L);
			else
				Title = "";
			if ( Title != "")
			{
				L = Title.Length;
				if( L > (Width - 10))
					L = Width - 10;
				if( L < 0)
					L = 0;
				I = ( Width - L) >> 1;
				B.FillChar( (char)' ', (char)CTitle, 1, I - 1);
				B.FillBuf( Title, (byte)CTitle, (uint)L, I);
				B.FillChar( (char)' ', (char)CTitle, 1, I + L);
			}
			if( (State & StateFlags.sfActive) != 0)
			{
				if( ((Owner as Window).Flags & WindowFlags.wfClose) != 0)
					if( (FrameMode & FrameModes.fmCloseClicked) == 0)
						B.FillCStr( ldCloseWindow, CFrame, 2);
					else
						B.FillCStr( ldCloseClicked, CFrame, 2);
				if( ((Owner as Window).Flags & WindowFlags.wfZoom) != 0 )
				{
					B.FillCStr( ldMaximize, CFrame, Width - 5);
					Owner.SizeLimits( out Min, out Max);
					if( (FrameMode & FrameModes.fmZoomClicked) != 0)
						B.drawBuffer[Width -4].AsciiChar = (char)15;
					else
						if( (Owner.Size.X == Max.X) && ( Owner.Size.Y == Max.Y))
						B.drawBuffer[Width - 4].AsciiChar = (char)18;
				}
			}
			WriteLine( 0, 0, (int)Size.X, 1, B);
			for( I = 1; I<= Size.Y - 2; I++)
			{
				FrameLine( B, I, F + 3, (byte)CFrame);
				WriteLine( 0, I, (int)Size.X, 1, B);
			}
			FrameLine( B, (int)Size.Y - 1, F + 6, (byte)CFrame);
			if( (State & StateFlags.sfActive) != 0 )
				if( ((Owner as Window).Flags & WindowFlags.wfGrow) != 0)
					B.FillCStr( ldBottomRight, CFrame, Width - 2);
			WriteLine( 0, (int)Size.Y - 1, (int)Size.X, 1, B);
		}

		public void FrameLine( DrawBuffer B, int y, int n, byte Color)
		{
			string InitFrame = "\x06\x0A\x0C\x05\x00\x05\x03\x0A\x09\x16\x1A\x1C\x15\x00\x15\x13\x1A\x19";
			
			byte[] FrameMask = new byte[ScreenManager.MaxViewWidth];

			short i;
			FrameMask[0]=(byte)InitFrame[n];
			for (i=1; i+1<Size.X; i++) FrameMask[i]=(byte)InitFrame[n+1];
			FrameMask[Size.X-1]=(byte)InitFrame[n+2];

			View p;
			p=Owner.Last;
			while (true) 
			{
				p=p.Next;
				if (p==this) break;
				if (((p.Options & OptionFlags.ofFramed) != 0) && ((p.State & StateFlags.sfVisible) !=0)) 
				{
					byte mask1, mask2;
					if (y+1<p.Origin.Y) continue;
					else if (y+1==p.Origin.Y) { mask1=0x0A; mask2=0x06;}
					else if (y==p.Origin.Y+p.Size.Y) { mask1=0x0A; mask2=0x03;}
					else if (y<p.Origin.Y+p.Size.Y) { mask1=0; mask2=0x05;}
					else continue;
					ushort xMin=(ushort)(p.Origin.X);
					ushort xMax=(ushort)(p.Origin.X+p.Size.X);
					if (xMin<1) xMin=1;
					if (xMax>Size.X-1) xMax=(ushort)(Size.X-1);
					if (xMax>xMin) 
					{
						if (mask1==0) 
						{
							FrameMask[xMin-1] |= mask2;
							FrameMask[xMax]   |= mask2;
						} 
						else 
						{
							FrameMask[xMin-1] |= mask2;
							FrameMask[xMax]   |= (byte)(mask2 ^ mask1);
							for (i=(short)xMin; i< xMax; i++) 
							{
								FrameMask[i] |= mask1;
							}
						}
					}
				}
			} // while
			//  unsigned char* src=frameMask;
			i=(short)Size.X;
			short i1=0;
			for ( i = (short)Size.X; i > 0; i--) 
			{
				B.drawBuffer[i1].Attribute = (char)Color;
				B.drawBuffer[i1].AsciiChar = (char) ldFrameChars[FrameMask[i1]];
				i1++;
			} /* endwhile */
		}

		internal void DragWindow( ref Event Event, DragModes Mode)
		{
			Rect Limits;
			Point Min, Max;
			Limits = Owner.Owner.GetExtent();
			Owner.SizeLimits( out Min, out Max);
			Owner.DragView( Event, Owner.DragMode | Mode, Limits, Min, Max);
			ClearEvent( ref Event);
		}

		public override void HandleEvent(ref Event Event)
		{
			Point Mouse;

			base.HandleEvent (ref Event);
			if( Event.What == Event.MouseDown)
			{
				Mouse = MakeLocal( Event.Where);
				if( Mouse.Y == 0)
				{
					if( ((((Owner as Window).Flags) & ( WindowFlags.wfClose)) != 0) &&
						( (State & StateFlags.sfActive) != 0) &&
						(Mouse.X >= 2) &&
						(Mouse.X <= 4))
					{
						do
						{
							Mouse = MakeLocal( Event.Where);
							if( (Mouse.X >= 2) && ( Mouse.X <= 4) && ( Mouse.Y == 0))
								FrameMode = FrameModes.fmCloseClicked;
							else
								FrameMode = 0;
							DrawView();
						}while( MouseEvent( ref Event, Event.MouseMove | Event.MouseAuto));
						FrameMode = 0;
						if( (Mouse.X >= 2) && ( Mouse.X <= 4) && ( Mouse.Y == 0))
						{
							Event.What = Event.evCommand;
							Event.Command = cmClose;
							Event.InfoPtr = Owner;
							PutEvent( Event);
						}
						ClearEvent( ref Event);
						DrawView();
					}
					else
						if( (((Owner as Window).Flags & WindowFlags.wfZoom) != 0) &&
						( (State & StateFlags.sfActive) !=0) &&
						( Event.Double || ( Mouse.X >= Size.X - 5) &&
						( Mouse.X <= Size.X - 3)))
					{
						if( !Event.Double)
							do
							{
								Mouse = MakeLocal( Event.Where);
								if( (Mouse.X >= (Size.X - 5)) &&
									( Mouse.X <= (Size.X - 3)) &&
									( Mouse.Y == 0))
									FrameMode = FrameModes.fmZoomClicked;
								else
									FrameMode = 0;
								DrawView();
							}while( MouseEvent( ref Event, Event.MouseMove | Event.MouseAuto ));
						FrameMode = 0;
						if( ((Mouse.X >= (Size.X - 5)) && ( Mouse.X <= (Size.X - 3)) && ( Mouse.Y == 0)) || Event.Double)
						{
							Event.What = Event.evCommand;
							Event.Command = cmZoom;
							Event.InfoPtr = Owner;
							PutEvent( Event);
						}
						ClearEvent( ref Event);
						DrawView();
					}
					else
						if( ((Owner as Window).Flags & WindowFlags.wfMove) !=0)
						DragWindow( ref Event, DragModes.dmDragMove);
				}
				else
					if( (( State & StateFlags.sfActive) != 0) && ( Mouse.X >= (Size.X - 2)) && ( Mouse.Y >= ( Size.Y - 1)))
					if( ((Owner as Window).Flags & WindowFlags.wfGrow) != 0)
						DragWindow( ref Event, DragModes.dmDragGrow);
			}
		}

		public override void SetState(StateFlags AState, bool Enable)
		{
			base.SetState (AState, Enable);
			if( (AState & (StateFlags.sfActive | StateFlags.sfDragging)) != 0)
				DrawView();
		}

	}
}
