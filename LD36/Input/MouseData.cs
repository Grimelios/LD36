using Microsoft.Xna.Framework;

namespace LD36.Input
{
	internal enum ClickStates
	{
		Held,
		Released,
		PressedThisFrame,
		ReleasedThisFrame
	}

	internal class MouseData
	{
		public MouseData(Vector2 screenPosition, Vector2 worldPosition, ClickStates leftClickState, ClickStates rightClickState)
		{
			ScreenPosition = screenPosition;
			WorldPosition = worldPosition;
			LeftClickState = leftClickState;
			RightClickState = rightClickState;
		}

		public Vector2 ScreenPosition { get; }
		public Vector2 WorldPosition { get; }

		public ClickStates LeftClickState { get; }
		public ClickStates RightClickState { get; }
	}
}
