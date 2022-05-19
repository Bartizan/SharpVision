using System;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Gadgets
{
	public class CalendarView : View
	{

		public int Year = DateTime.Now.Year;
		public int Month = DateTime.Now.Month;
		public uint Days = 1;

		public uint CurYear;
		public uint CurMonth;
		public uint CurDay;

		public string DaysInMonth = "\x1F\x1C\x1F\x1E\x1F\x1E\x1F\x1F\x1E\x1F\x1E\x1F";
		public string[] MonthStr = new string[12];

		public CalendarView( Rect Bounds):base( Bounds)
		{
			MonthStr[0] =  "January   ";
			MonthStr[1] =  "February  ";
			MonthStr[2] =  "March     ";
			MonthStr[3] =  "April     ";
			MonthStr[4] =  "May       ";
			MonthStr[5] =  "June      ";
			MonthStr[6] =  "July      ";
			MonthStr[7] =  "August    ";
			MonthStr[8] =  "Septemeber";
			MonthStr[9] =  "October   ";
			MonthStr[10] = "November  ";
			MonthStr[11] = "December  ";
			Options |= OptionFlags.ofSelectable;
			EventMask |= EventMasks.evMouseAuto;
			Year = DateTime.Now.Year;
			Month = DateTime.Now.Month;
			DrawView();
		}

		public override void Draw()
		{
			const int Width = 20;
			int DayOf, CurDays;
			string S;
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);
			byte Color, BoldColor;

			Color = (byte)GetColor(6);
			BoldColor = (byte)GetColor(7);
			DayOf = (int)(new DateTime( (int)Year, (int)Month, 1)).DayOfWeek;
			Days = (byte)(DaysInMonth[(int)(Month - 1)]);
			if (((Year % 4) == 0) && ( Month == 2))
				Days++;
			S = string.Format("{0:0000}", Year);
			B.FillChar( (char)' ', (char)Color, Width);
			B.FillCStr( MonthStr[Month - 1] + " " + S + " \x1E \x1F", Color, 0 );
			WriteLine( 0, 0, Width, 1, B);
			B.FillChar( (char)' ', (char)Color, Width);
			B.FillStr( "Su Mo Tu We Th Fr Sa", (char)Color, 0);
			WriteLine(0, 1, Width, 1, B);
			CurDays = 1 - DayOf;
			for( int i = 1; i <= 6; i++)
			{
				for( int j = 0; j <= 6; j++)
				{
					if( (CurDays < 1) || ( CurDays > Days))
						B.FillStr( "   ", (char)Color, j * 3);
					else
						if( (Year == CurYear) && ( Month == CurMonth) && ( CurDays == CurDay))
						B.FillStr( string.Format("{0:00}", CurDays), (char)BoldColor, j*3);
					else
						B.FillStr( string.Format("{0:00}", CurDays), (char)Color, j*3);
					CurDays++;
				}
				WriteLine( 0, i + 1, Width, 1, B);
			}
		}

		public override void HandleEvent(ref Event Event)
		{
			Point Point;

			base.HandleEvent (ref Event);
			if( (State & StateFlags.sfSelected) != 0)
			{
				if( (Event.What & ( Event.MouseDown | Event.MouseAuto)) != 0)
				{
					Point = MakeLocal( Event.Where);
					if( (Point.X == 16) && ( Point.Y == 0))
					{
						Month ++;
						if( Month > 12)
						{
							Year ++;
							Month = 1;
						}
						DrawView();
					}
					if( (Point.X == 18) && ( Point.Y == 0) )
					{
						Month --;
						if( Month < 1)
						{
							Year --;
							Month = 12;
						}
						DrawView();
					}
				}
				else
					if( Event.What == Event.KeyDown)
				{
					if( (((int)Event.KeyCode & 0xFF) == (byte)'+') || ( Event.KeyCode == KeyboardKeys.Down))
					{
						Month++;
						if( Month > 12)
						{
							Year ++;
							Month = 1;
						}
					}
					if( (((int)Event.KeyCode & 0xFF) == (byte)'-') || ( Event.KeyCode == KeyboardKeys.Up))
					{
						Month --;
						if( Month < 1)
						{
							Year --;
							Month = 12;
						}
					}
					DrawView();
				}
			}
		}
	}
}
