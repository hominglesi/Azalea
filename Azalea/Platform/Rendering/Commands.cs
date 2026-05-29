using static Azalea.Platform.Rendering.PlatformRenderer;

namespace Azalea.Platform.Rendering;

internal record FramebufferTexture2D(Framebuffer framebuffer, Texture2D texture, int target, int attachment, int textarget, int level) : RenderCommand;
internal record TexImage2DCommand(Texture2D texture, int target, int level, int internalFormat, int width, int height, int border, int format, int type, byte[]? pixels) : RenderCommand;
internal record TexParameteri(Texture2D texture, int target, int parameter, int value) : RenderCommand;
