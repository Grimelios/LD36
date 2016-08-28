using LD36.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD36
{
	internal class Camera
	{
		private int viewportWidth;
		private int viewportHeight;

		public Camera(Viewport viewport)
		{
			viewportWidth = viewport.Width;
			viewportHeight = viewport.Height;
			Zoom = 1;
			Transform = Matrix.Identity;
			InverseTransform = Matrix.Identity;
		}

		public float Rotation { get; set; }
		public float Zoom { get; set; }

		public Vector2 Position { get; set; }
		public Matrix Transform { get; set; }
		public Matrix InverseTransform { get; set; }
		public Entity Target { get; set; }

		public void Update(float dt)
		{
			if (Target != null)
			{
				Vector2 targetPosition = Target.Position;
				Position = new Vector2((int)targetPosition.X, (int)targetPosition.Y);
			}

			Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
				  Matrix.CreateRotationZ(-Rotation) *
				  Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
				  Matrix.CreateTranslation(new Vector3(viewportWidth * 0.5f, viewportHeight * 0.5f, 0));
			InverseTransform = Matrix.Invert(Transform);
		}
	}
}
