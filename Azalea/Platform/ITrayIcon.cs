using Azalea.Inputs;
using System;

namespace Azalea.Platform;
public interface ITrayIcon
{
	Action<MouseButton>? OnClick { get; set; }
	Action<MouseButton>? OnDoubleClick { get; set; }

	void Destroy();

	public string Description => $"""
		<TraitHeader>
			<TraitImage>Textures/Scheme/img.jpg</TraitName>
			<TraitName>Yordle</TraitName>
		</TraitHeader>
		""";

	public string FinalOutput => $"""
		<Post>
			<Title>PATCH HIGHLIGHTS</Title>
			<Section>
				<Image>Textures/Scheme/img.jpg</Image>
			</Section>

			${Description}

			<Title>ROTATING SHOP AND CHAPTER  PASS</Title>
			<Section>
				<SubTitle>ROTATING SHOP</SubTitle>
				<Paragraph>The Rotating Shop arrives with patch 14.11. To learn more about it, click here!</Paragraph>
				<Image>Textures/Scheme/img2.jpg</Image>
				<LineBreak />
				<SubTitle>ROTATION UPDATE</SubTitle>
				<Paragraph>Prestige Chibi Dragonmancer Yasuo and Chibi Divine Sword Irelia are swinging by this patch, but you'll also get to meet the new Chibi Battle Academia Ezreal on his home turf: the Ultimate Showdown Arena. </Paragraph>
				<LineBreak />
				<SubTitle>CHIBI EZREAL</SubTitle>
				<Paragraph>Rotating Shop may be stealing the show, but we've got a new Chibi available for Direct Purchase for 1900RP. Chibi Ezreal will also come with his boom Trueshot Barrage!</Paragraph>
				<Image>Textures/Scheme/img3.jpg</Image>
				<LineBreak />
				<SubTitle>INKBORN FABLES CHAPTER 2 PASS</SubTitle>
				<Paragraph>The second Chapter of your journey through Inkborn Fables begins with patch 14.11 on May 30th, at 11 AM PT. The Pass+ is available for purchase for 1295 RP.</Paragraph>
				<Image>Textures/Scheme/img4.jpg</Image>
			</Section>
		<Post>
		""";
}
