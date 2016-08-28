using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using LD36.Entities;
using LD36.Entities.Environment;
using LD36.Generation;
using LD36.Input;
using LD36.Interfaces;
using LD36.Json;
using LD36.Messaging;
using LD36.Messaging.Input;
using LD36.Physics;
using LD36.Service;
using LD36.Timing;
using LD36.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LD36
{
	using EntityLayers = Dictionary<Type, List<Entity>>;

	internal class LDGame : Game, IMessageReceiver
	{
		private const int PyramidSize = 75;
		private const int PyramidScaleMultiplier = 3;
        private const float Gravity = 25;

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private Camera camera;
		private InputGenerator inputGenerator;
		private TimerCollection timerCollection;
		private World world;
		private PhysicsDebug physicsDebug;
		private UserInterface userInterface;

		private EntityLayers entityLayers;
		private List<Type> updateOrder;
		private List<Type> renderOrder;

		private PyramidGenerator pyramidGenerator;
		private Texture2D mapTexture;
		private Texture2D lightTexture;

		public LDGame()
		{
			graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = Constants.ScreenWidth,
				PreferredBackBufferHeight = Constants.ScreenHeight
			};

			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			ContentLoader.Initialize(Content);

			DIKernel.Bind(new Camera(GraphicsDevice.Viewport));
			DIKernel.Bind(GraphicsDevice);
			DIKernel.Bind<MessageSystem>();
			DIKernel.Bind<PhysicsFactory>();
			DIKernel.Bind<TimerCollection>();
			DIKernel.Bind<UserInterface>();
			DIKernel.Bind(new World(new Vector2(0, Gravity)));

			camera = DIKernel.Get<Camera>();
			inputGenerator = DIKernel.Get<InputGenerator>();
			world = DIKernel.Get<World>();
			timerCollection = DIKernel.Get<TimerCollection>();
			userInterface = DIKernel.Get<UserInterface>();

			EntityMaster master = JsonUtilities.Deserialize<EntityMaster>("Entities.json");
			updateOrder = master.UpdateOrder;
			renderOrder = master.RenderOrder;
			entityLayers = new EntityLayers();
			master.MasterList.ForEach(type => entityLayers.Add(type, new List<Entity>()));

			EntityUtilities.Initialize(entityLayers);

			pyramidGenerator = new PyramidGenerator(PyramidSize, PyramidScaleMultiplier);
			pyramidGenerator.Generate();
			mapTexture = pyramidGenerator.Texture;

			PlayerCharacter player = new PlayerCharacter(new Vector2(775, 7070));
			entityLayers[typeof(PlayerCharacter)].Add(player);
			camera.Target = player;

			// Creating these objects registers them as message listeners.
			DIKernel.Get<ArtifactService>();
			DIKernel.Get<UserInterfaceService>();

			MessageSystem messageSystem = DIKernel.Get<MessageSystem>();
			messageSystem.Subscribe(MessageTypes.Keyboard, this);
			messageSystem.Subscribe(MessageTypes.Mouse, this);
			messageSystem.Send(new StartMessage());

			lightTexture = ContentLoader.LoadTexture("LightGradient");

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		protected override void UnloadContent()
		{
		}

		public void Receive(GameMessage message)
		{
			switch (message.Type)
			{
				case MessageTypes.Keyboard:
					HandleKeyboardMessage((KeyboardMessage)message);
					break;

				case MessageTypes.Mouse:
					HandleMouseMessage((MouseMessage)message);
					break;
			}
		}

		private void HandleKeyboardMessage(KeyboardMessage message)
		{
			if (message.Data.KeysPressedThisFrame.Contains(Keys.G))
			{
			}
		}

		private void HandleMouseMessage(MouseMessage message)
		{
			MouseData data = message.Data;
			//camera.Position = data.ScreenPosition * 10;
		}

		protected override void Update(GameTime gameTime)
		{
			float dt = (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
			
			timerCollection.Update(dt);
			inputGenerator.GenerateInputEvents();
			userInterface.Update(dt);

			// This assumes that the frame rate will behave and pretty much stay at a constant 60FPS. See
			// http://www.box2d.org/forum/viewtopic.php?t=4785 for more information).
			world.Step(dt);

			foreach (Type type in updateOrder)
			{
				entityLayers[type].ForEach(entity => entity.Update(dt));
			}

			camera.Update(dt);
		}

		protected override void Draw(GameTime gameTime)
		{
			// This color matches the exterior background color inside the pyramid.
			GraphicsDevice.Clear(new Color(10, 4, 0));

			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.Transform);

			foreach (Type type in renderOrder)
			{
				entityLayers[type].ForEach(entity => entity.Render(spriteBatch));
			}
			
			spriteBatch.End();

			spriteBatch.Begin();
			spriteBatch.Draw(lightTexture, Vector2.Zero, Color.White);
			userInterface.Render(spriteBatch);
			spriteBatch.End();
		}
	}
}
