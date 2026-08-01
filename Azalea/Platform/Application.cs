using Azalea.Design.Containers;
using Azalea.Design.Schemes;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using Azalea.Native.OpenGL;
using Azalea.Platform.Rendering;
using Azalea.Platform.Scheduling;
using Azalea.Utils;
using System;
using System.Collections.Generic;

namespace Azalea.Platform;
public sealed class Application
{
	public readonly Windowing.PlatformWindow Window;
	public readonly PlatformRenderer Renderer;
	public readonly PlatformScheduler Scheduler;

	public Application()
	{
		Window = Windowing.PlatformWindow.Create(new(800, 600));
		Renderer = PlatformRenderer.AttachRenderer(Window);
		Scheduler = PlatformScheduler.AttachScheduler(Window);

		Window.Closed.OnValueChanged += _ => OnClosed?.Invoke();

		var commandGroup = ObjectPool<RenderCommandGroup>.Borrow();

		commandGroup.Enable(GL.BLEND);
		commandGroup.Enable(GL.CULL_FACE);
		commandGroup.Disable(GL.DEPTH_TEST);
		commandGroup.BlendFunction(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);

		Renderer.Thread.SubmitCommandGroup(commandGroup);

		var coordinator = Renderer.Coordinator;
		var quadBatch = coordinator.DefaultQuadBatch;

		Renderer.PrintErrors();

		bool mimicMainWindow = true;
		var debugContainer = new ScrollableContainer()
		{
			BackgroundColor = new Color(24, 19, 38)
		};

		if (mimicMainWindow)
		{
			//Renderer.FramebufferSize = (Vector2Int)AzaleaGame.RENDERED_GAME.DrawSize;
		}
		else
		{
			var schemeFormatter = new SchemeFormatter();
			schemeFormatter.AddScheme("Post", createPost);
			schemeFormatter.AddScheme("Section", createSection);
			schemeFormatter.AddScheme("Title", createTitle);
			schemeFormatter.AddScheme("SubTitle", createSubTitle);
			schemeFormatter.AddScheme("Paragraph", createParagraph);
			schemeFormatter.AddScheme("LineBreak", createLineBreak);
			schemeFormatter.AddScheme("UnorderedList", createUnorderedList);
			schemeFormatter.AddScheme("ListItem", createListItem);
			schemeFormatter.AddScheme("Image", createImage);

			debugContainer.ScrollBar.Head.Color = new Color(255, 255, 255);
			debugContainer.Add(schemeFormatter.Format(Assets.GetText("Xml/SchemeTestTemp.xml")!));
		}

		Scheduler.InjectProtocol((win, rend) =>
		{
			var clientSize = win.ClientSize;

			if (clientSize == Vector2Int.Zero)
				return;

			var renderQueue = coordinator.BeginCommandQueue();
			renderQueue.PrepareRendering(clientSize);
			renderQueue.Clear(Palette.Flowers.Azalea);

			if (mimicMainWindow)
			{
				try
				{
					AzaleaGame.RENDERED_GAME.Draw(null, Renderer.Coordinator);

				}
				catch (Exception)
				{
					ObjectPool<RenderCommandGroup>.Return(renderQueue);
					renderQueue = ObjectPool<RenderCommandGroup>.Borrow();
					renderQueue.PrepareRendering(clientSize);
					renderQueue.Clear(Palette.Flowers.Azalea);
				}
			}
			else
			{
				debugContainer.Size = Window.ClientSize.Value;
				debugContainer.UpdateSubTree();
				debugContainer.Draw(null, Renderer.Coordinator);
			}

			quadBatch.Draw(renderQueue);

			renderQueue.PrintErrors();

			renderQueue.SwapBuffers();

			Renderer.StageQueue(coordinator.EndCommandQueue());
		});

		GameObject createPost(List<object> content)
		{
			var container = new FlexContainer()
			{
				Anchor = Anchor.TopCenter,
				Origin = Anchor.TopCenter,
				Width = 1000,
				AutoSizeAxes = Axes.Y,
				Direction = FlexDirection.Vertical,
				Wrapping = FlexWrapping.NoWrapping
			};

			container.Add(new Box()
			{
				Size = new(1, 50),
				Alpha = 0.01f
			});

			foreach (var item in content)
			{
				if (item is GameObject obj)
					container.Add(obj);
			}

			container.Add(new Box()
			{
				Size = new(1, 50),
				Alpha = 0.01f
			});

			return container;
		}

		GameObject createTitle(List<object> content)
		{
			if (content.Count != 1 && content[0] is not string)
				throw new Exception("This tag only accepts text");

			string text = (string)content[0];

			return new SpriteText()
			{
				Margin = new(top: 30, left: 40),
				Text = text,
				Font = FontUsage.Default.With(size: 42)
			};
		}

		GameObject createSubTitle(List<object> content)
		{
			if (content.Count != 1 && content[0] is not string)
				throw new Exception("This tag only accepts text");

			string text = (string)content[0];

			return new SpriteText()
			{
				Text = text,
				Margin = new(top: 30, left: 40),
				Font = FontUsage.Default.With(size: 32)
			};
		}

		GameObject createLineBreak(List<object> content)
		{
			return new Box
			{
				Size = new(900, 1),
				Color = new Color(70, 66, 81),
				Margin = new(top: 40, left: 40),
			};
		}

		GameObject createUnorderedList(List<object> content)
		{
			var container = new FlexContainer()
			{
				Direction = FlexDirection.Vertical,
				Margin = new(top: 30, left: 40),
				Width = 900,
				AutoSizeAxes = Axes.Y,
			};

			foreach (var item in content)
			{
				if (item is not GameObject obj)
					continue;

				container.Add(new Composition()
				{
					RelativeSizeAxes = Axes.X,
					AutoSizeAxes = Axes.Y,
					Children = new GameObject[]
					{
					new Box()
					{
						Size = new(5),
						Margin = new(10)
					},
					obj
					}
				});

				obj.Width = 875;
				obj.Margin = new(left: 25);
			}

			return container;
		}

		GameObject createImage(List<object> content)
		{
			if (content.Count != 1 && content[0] is not string)
				throw new Exception("This tag only accepts text");

			string text = (string)content[0];

			var texture = Assets.GetTexture(text);
			var aspectRatio = texture.Height / (float)texture.Width;
			return new Sprite()
			{
				Size = new(900, 900 * aspectRatio),
				Texture = texture,
				Margin = new(top: 20, left: 40),
			};
		}

		GameObject createListItem(List<object> content)
		{
			if (content.Count != 1 && content[0] is not string)
				throw new Exception("This tag only accepts text");

			string text = (string)content[0];

			return new TextContainer(t => t.Font = FontUsage.Default.With(size: 24))
			{
				AutoSizeAxes = Axes.Y,
				Text = text
			};
		}

		GameObject createParagraph(List<object> content)
		{
			if (content.Count != 1 && content[0] is not string)
				throw new Exception("This tag only accepts text");

			string text = (string)content[0];

			return new TextContainer(t => t.Font = FontUsage.Default.With(size: 24))
			{
				Text = text,
				Margin = new(top: 30, left: 40),
				Width = 900,
				AutoSizeAxes = Axes.Y
			};
		}

		GameObject createSection(List<object> content)
		{
			var container = new FlexContainer()
			{
				Direction = FlexDirection.Vertical,
				Margin = new(top: 20),
				AutoSizeAxes = Axes.Y,
				RelativeSizeAxes = Axes.X,
				BorderThickness = new(1),
				BorderColor = new Color(70, 66, 81),
			};

			foreach (var item in content)
			{
				if (item is GameObject obj)
					container.Add(obj);
			}

			container.Add(new Box()
			{
				Size = new(1, 30),
				Alpha = 0.01f
			});

			return container;
		}
	}

	public Action? OnClosed;
	public void Close() => Window.Close();
}
