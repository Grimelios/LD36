using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace LD36.Input
{
	internal class KeyboardData
	{
		public KeyboardData(List<Keys> keysDown, List<Keys> keysPressedThisFrame, List<Keys> keysReleasedThisFrame)
		{
			KeysDown = keysDown;
			KeysPressedThisFrame = keysPressedThisFrame;
			KeysReleasedThisFrame = keysReleasedThisFrame;
		}

		public List<Keys> KeysDown { get; }
		public List<Keys> KeysPressedThisFrame { get; }
		public List<Keys> KeysReleasedThisFrame { get; }
	}
}
