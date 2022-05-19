using System;
using System.Xml.Serialization;
using TurboVision.Objects;
using TurboVision.Views;

namespace TurboVision.Dialogs
{
	public enum DialogPalettes
	{
		dpBlueDialog = 0,
		dpCyanDialog = 1,
		dpGrayDialog = 2,
	}

    [Serializable]
	public class Dialog : Window
	{

		private static char[] CGrayDialog = 
			("\x20\x21\x22\x23\x24\x25\x26\x27\x28\x29\x2a\x2b\x2c\x2d\x2e"+
			"\x2f\x30\x31\x32\x33\x34\x35\x36\x37\x38\x39\x3a\x3b\x3c\x3d"+
			"\x3e\x3f").ToCharArray();
		private static char[] CBlueDialog =
			("\x40\x41\x42\x43\x44\x45\x46\x47\x48\x49\x4a\x4b\x4c"+
			"\x4d\x4e\x4f\x50\x51\x52\x53\x54\x55\x56\x57\x58" +
			"\x59\x5a\x5b\x5c\x5d\x5e\x5f").ToCharArray();
		private static char[] CCyanDialog =
			("\x60\x61\x62\x63\x64\x65\x66\x67\x68\x69\x6a\x6b\x6c" +
			"\x6d\x6e\x6f\x70\x71\x72\x73\x74\x75\x76\x77\x78" +
			"\x79\x7a\x7b\x7c\x7d\x7e\x7f").ToCharArray();

        public Dialog():base( new Rect(0, 0, 50, 20), "")
        {
            Initialize();
        }

        public Dialog( Rect Bounds, string ATitle):base( Bounds, ATitle, 0)
		{
            Initialize();
		}

        protected void Initialize()
        {
            Options |= OptionFlags.ofVersion20;
            GrowMode = 0;
            Flags = WindowFlags.wfMove | WindowFlags.wfClose;
            Palette = (WindowPalettes)DialogPalettes.dpGrayDialog;
        }

		public override char[] GetPalette()
		{
			char[][] P = new char[3][]{ CBlueDialog, CCyanDialog, CGrayDialog};
			return P[(int)Palette];
		}

		public override void SizeLimits(out Point Min, out Point Max)
		{
			base.SizeLimits (out Min, out Max);
			if( Owner != null)
			{
				Max.X = Owner.Size.X;
				Max.Y = Owner.Size.Y;
			}
			else
			{
				Max.X = int.MaxValue;
				Max.Y = int.MaxValue;
			}
		}

		public override bool Valid(int Command)
		{
			if( Command == cmCancel)
				return true;
			else
				return base.Valid( Command);
		}

		public override void HandleEvent( ref Event Event)
		{
			base.HandleEvent( ref Event);
			switch( Event.What)
			{
				case Event.KeyDown :
                    switch (Event.KeyCode)
                    {
					case KeyboardKeys.Esc :
						Event.What = Event.evCommand;
						Event.Command = cmCancel;
						Event.InfoPtr = null;
						PutEvent( Event);
						ClearEvent( ref Event);
						break;
					case KeyboardKeys.Enter :
						Event.What = Event.Broadcast;
						Event.Command = cmDefault;
						Event.InfoPtr = null;
						PutEvent( Event);
						ClearEvent( ref Event);
						break;
				}
					break;
				case Event.evCommand :
				switch( Event.Command)
				{
					case cmOk :
					case cmCancel :
					case cmYes :
					case cmNo :
						if( (State & StateFlags.sfModal) != 0)
						{
							EndModal( Event.Command);
							ClearEvent( ref Event);
						}
						break;
				}
					break;
			}
		}

	}
}
