using TurboVision.Objects;
using TurboVision.Dialogs;
using TurboVision.Views;

namespace TurboVision.Dialogs
{
    public class PasswordInput : InputLine
	{

        public char PasswordChar = '*';

        public PasswordInput(Rect R, int MaxLength, char PasswordChar)
            : base(R, MaxLength)
        {
            this.PasswordChar = PasswordChar;
        }

		public PasswordInput( Rect R, int MaxLength):base( R, MaxLength)
		{
		}
		
		public override void Draw()
		{
			byte Color;
			int L, R;
			DrawBuffer B = new DrawBuffer( Size.X * Size.Y);

			if( (State & StateFlags.sfFocused) == 0)
				Color = (byte)GetColor(1);
			else
				Color = (byte)GetColor(2);
			B.FillChar( (char)' ', (char)Color, (int)Size.X);
			B.FillStr( (new string( PasswordChar, Data.Length) + new string(' ', (int)Size.X)).Substring( FirstPos, (int)(Size.X - 2)), (char)Color, 1);
			if( CanScroll(1))
				B.FillChar( (char)ldRightScroll, (char)GetColor(4), 1, (int)Size.X - 1);
			if( (State & StateFlags.sfFocused) != 0)
			{
				if( CanScroll( -1))
					B.FillChar( (char)ldLeftScroll, (char)GetColor(4), 1);
				L = SelStart - FirstPos;
				R = SelEnd - FirstPos;
				if( L < 0)
					L = 0;
				if( R > (Size.X - 2))
					R = (int)Size.X - 2;
				if( L < R)
					B.FillChar( '\x00', (char)GetColor(3), R - L, L + 1);
			}
			WriteLine( 0, 0, (int)Size.X, (int)Size.Y, B);
			SetCursor( CurPos - FirstPos + 1, 0);
		}
	}	
}