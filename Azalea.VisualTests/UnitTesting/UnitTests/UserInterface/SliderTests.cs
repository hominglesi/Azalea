using Azalea.Design.UserInterface;
using Azalea.Design.UserInterface.Basic;
using Azalea.Graphics;
using Azalea.Utils;
using System.Numerics;

#pragma warning disable CS8602, CS8618

namespace Azalea.VisualTests.UnitTesting.UserInterface.UserInput;
public class SliderTests : UnitTestSuite
{
	public class ValueTest : UnitTest
	{
		private readonly static Vector2 __size = new Vector2(200, 30);

		private Slider? _slider;

		private float _headPosition =>
			_slider.Direction == SliderDirection.Horizontal ? _slider.Head.Position.X : _slider.Head.Position.Y;

		public ValueTest()
		{
			var sizeVertical = new Vector2(__size.Y, __size.X);
			var sizeVerticalShrinked = new Vector2(sizeVertical.X, sizeVertical.Y - 50);
			AddOperation("Set Value to 1", () => _slider.Value = 1);
			AddResult("Check if Value is 1", () => _slider.Value == 1);
			addHeadPositionCheck(__size.X);
			AddOperation("Set HeadInsideBody to true", () => _slider.HeadInsideBody = true);
			addHeadPositionCheck(__size.X - 12.5f);
			AddOperation("Set Direction to Vertical", () =>
			{
				_slider.Size = sizeVertical;
				_slider.Direction = SliderDirection.Vertical;
			});
			addHeadPositionCheck(sizeVertical.Y - 12.5f);
			AddOperation("Shrink heigth", () => _slider.Size = sizeVerticalShrinked);
			addHeadPositionCheck(sizeVerticalShrinked.Y - 12.5f);
		}

		private void addHeadPositionCheck(float value)
		{
			AddResult("Check if Head Position is correct", () =>
			{
				TestContainer.ForceUpdate();
				return Precision.AlmostEquals(_headPosition, value);
			});
		}

		public override void Setup(UnitTestContainer scene)
		{
			base.Setup(scene);

			_slider = new BasicSlider()
			{
				Origin = Anchor.Center,
				Anchor = Anchor.Center,
				Size = __size,
				Value = 1,
				HeadInsideBody = false
			};

			TestContainer.Add(_slider);
		}

		public override void TearDown(UnitTestContainer scene)
		{
			base.TearDown(scene);

			if (_slider is not null)
				scene.Remove(_slider);
		}
	}
}

#pragma warning restore CS8602, CS8618
