using Azalea.Graphics.Textures;
using Azalea.Text;
using SharpFNT;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Azalea.IO.Stores;

public class GlyphStore : IResourceStore<TextureUpload>, IGlyphStore
{
    protected readonly string? AssetName;

    protected readonly IResourceStore<TextureUpload>? TextureLoader;

    public string? FontName { get; }

    public float? Baseline => Font?.Common.Base;

    protected readonly ResourceStore<byte[]> Store;

    protected BitmapFont? Font;

    public GlyphStore(ResourceStore<byte[]> store, string? assetName = null, IResourceStore<TextureUpload>? textureLoader = null)
    {
        Store = new ResourceStore<byte[]>(store);

        Store.AddExtention("bin");

        AssetName = assetName;
        TextureLoader = textureLoader;

        FontName = assetName?.Split('/').Last();

        Font = LoadFont();
    }

    public BitmapFont? LoadFont()
    {
        try
        {
            BitmapFont font;

            using var s = Store.GetStream($@"{AssetName}");

            font = BitmapFont.FromStream(s, FormatHint.Binary, false);

            return font;
        }
        catch
        {
            return null;
        }
    }

    public bool HasGlyph(char c) => Font?.Characters.ContainsKey(c) == true;

    public virtual TextureUpload? GetPageImage(int page)
    {
        if (TextureLoader != null)
            return TextureLoader.Get(GetFilenameForPage(page));

        using var stream = Store.GetStream(GetFilenameForPage(page));
        Debug.Assert(stream != null);
        return new TextureUpload(stream);
    }

    protected string GetFilenameForPage(int page)
    {
        Debug.Assert(Font != null);
        return $@"{AssetName}_{page.ToString().PadLeft((Font.Pages.Count - 1).ToString().Length, '0')}.png";
    }
    public CharacterGlyph? Get(char character)
    {
        if (Font == null) return null;

        Debug.Assert(Baseline != null);

        var bmCharacter = Font.GetCharacter(character);

        return new CharacterGlyph(character, bmCharacter.XOffset, bmCharacter.YOffset, bmCharacter.XAdvance, Baseline.Value, this);
    }

    public int GetKerning(char left, char right) => Font?.GetKerningAmount(left, right) ?? 0;

    public TextureUpload? Get(string name)
    {
        if (Font == null) return null;

        if (name.Length > 1 && !name.StartsWith($@"{FontName}/", StringComparison.Ordinal))
            return null;

        return Font.Characters.TryGetValue(name.Last(), out Character? c) ? LoadCharacter(c) : null;
    }

    protected int LoadedGlyphCount;

    protected virtual TextureUpload LoadCharacter(Character character)
    {
        var page = GetPageImage(character.Page);
        Debug.Assert(page != null);
        LoadedGlyphCount++;

        var image = new Image<Rgba32>(Configuration.Default, character.Width, character.Height);
        var source = page.Data;

        int readableHeight = Math.Min(character.Height, page.Height - character.Y);
        int readableWidth = Math.Min(character.Width, page.Width - character.X);

        for (int y = 0; y < character.Height; y++)
        {
            var pixelRowMemory = image.DangerousGetPixelRowMemory(y);
            int readOffset = (character.Y + y) * page.Width + character.X;

            for (int x = 0; x < character.Width; x++)
                pixelRowMemory.Span[x] = x < readableWidth && y < readableHeight ? source[readOffset + x] : new Rgba32(255, 255, 255, 0);
        }

        return new TextureUpload(image);
    }



    public Stream GetStream(string name) => throw new NotImplementedException();
    public IEnumerable<string> GetAvalibleResources() => Font?.Characters.Keys.Select(k => $"{FontName}/{(char)k}") ?? Enumerable.Empty<string>();

}
