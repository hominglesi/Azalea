﻿using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Graphics.OpenGL.Textures;

internal class GLTexture : INativeTexture
{
    protected readonly GLRenderer Renderer;
    protected readonly GL _gl;
    IRenderer INativeTexture.Renderer => Renderer;

    public int Width { get; set; }
    public int Height { get; set; }

    public uint TextureId => _textureId;
    private uint _textureId;

    public GLTexture(GLRenderer renderer, GL gl, int width, int height)
    {
        Renderer = renderer;
        _gl = gl;
        Width = width;
        Height = height;
    }

    public unsafe void SetData(ITextureUpload upload)
    {
        _textureId = _gl.GenTexture();
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _textureId);

        _gl.TextureParameter(_textureId, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        _gl.TextureParameter(_textureId, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        _gl.TextureParameter(_textureId, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        _gl.TextureParameter(_textureId, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        fixed (Rgba32* ptr = upload.Data)
            _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)Width,
                (uint)Height, 0, Silk.NET.OpenGL.PixelFormat.Rgba, Silk.NET.OpenGL.PixelType.UnsignedByte, ptr);

        _gl.GenerateMipmap(TextureTarget.Texture2D);
    }
}