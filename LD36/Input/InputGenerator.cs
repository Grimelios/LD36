using System.Collections.Generic;
using System.Linq;
using LD36.Messaging;
using LD36.Messaging.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LD36.Input
{
	internal class InputGenerator
	{
		private Camera camera;
		private KeyboardState oldKS;
		private KeyboardState newKS;
		private MouseState oldMS;
		private MouseState newMS;
		private MessageSystem messageSystem;

		public InputGenerator(Camera camera, MessageSystem messageSystem)
		{
			this.camera = camera;
			this.messageSystem = messageSystem;
		}

		public void GenerateInputEvents()
		{
			GenerateKeyboardMessage();
			GenerateMouseMessage();
		}

		private void GenerateKeyboardMessage()
		{
			oldKS = newKS;
			newKS = Keyboard.GetState();

			List<Keys> oldKeysDown = oldKS.GetPressedKeys().ToList();
			List<Keys> newKeysDown = newKS.GetPressedKeys().ToList();
			List<Keys> keysPressedThisFrame = newKeysDown.Except(oldKeysDown).ToList();
			List<Keys> keysReleasedThisFrame = oldKeysDown.Except(newKeysDown).ToList();
			KeyboardData data = new KeyboardData(newKeysDown, keysPressedThisFrame, keysReleasedThisFrame);

			messageSystem.Send(new KeyboardMessage(data));
		}

		private void GenerateMouseMessage()
		{
			oldMS = newMS;
			newMS = Mouse.GetState();

			Vector2 screenPosition = new Vector2(newMS.X, newMS.Y);
			Vector2 worldPosition = Vector2.Transform(screenPosition, camera.InverseTransform);

			ClickStates leftClickState = GetClickState(oldMS.LeftButton, newMS.LeftButton);
			ClickStates rightClickState = GetClickState(oldMS.RightButton, newMS.RightButton);

			MouseData data = new MouseData(screenPosition, worldPosition, leftClickState, rightClickState);

			messageSystem.Send(new MouseMessage(data));
		}

		private ClickStates GetClickState(ButtonState oldState, ButtonState newState)
		{
			if (oldState == newState)
			{
				return newState == ButtonState.Pressed ? ClickStates.Held : ClickStates.Released;
			}

			return newState == ButtonState.Pressed ? ClickStates.PressedThisFrame : ClickStates.ReleasedThisFrame;
		}
	}
}
