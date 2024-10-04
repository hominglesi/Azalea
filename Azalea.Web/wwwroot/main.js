import { dotnet } from './_framework/dotnet.js'

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

setModuleImports('JSImports', {
    WebGL: {
        ActiveTexture,
        AttachShader,
        BindBuffer,
        BindTexture,
        BindVertexArray,
        BlendFuncSeparate,
        BufferData,
        ClearColor,
        Clear,
        CompileShader,
        CreateBuffer,
        CreateProgram,
        CreateShader,
        CreateTexture,
        CreateVertexArray,
        DeleteBuffer,
        DeleteProgram,
        DeleteShader,
        DeleteTexture,
        DeleteVertexArray,
        Disable,
        DrawArrays,
        DrawElements,
        Enable,
        EnableVertexAttribArray,
        GenerateMipmap,
        GetAttribLocation,
        GetBufferParameter,
        GetProgramInfoLog,
        GetProgramParameter,
        GetShaderInfoLog,
        GetShaderParameter,
        GetShaderSource,
        GetUniformLocation,
        LinkProgram,
        Scissor,
        ShaderSource,
        TexImage2D,
        TexParameteri,
        Uniform1i,
        Uniform1iv,
        Uniform4f,
        UniformMatrix4fv,
        UseProgram,
        ValidateProgram,
        VertexAttribPointer,
        Viewport
    },
    WebEvents: {
        RequestAnimationFrame,
        CheckClientSize,
        GetCurrentPreciseTime
    },
    WebAudio: {
        Connect,
        BufferAudioData,
        ConnectToContext,
        CreateAudioBuffer,
        CreateBufferSource,
        CreateGain,
        SetBuffer,
        SetGain,
        SetLoop,
        SetMasterVolume,
        StartSource,
        StopSource
    }
});

exports = await getAssemblyExports("Azalea.Web");

canvas.addEventListener("mousemove", (e) => exports.Azalea.Web.WebEvents.ReportMouseMove(e.pageX, e.pageY));
canvas.addEventListener("mousedown", (e) => exports.Azalea.Web.WebEvents.ReportMouseDown(e.button));
canvas.addEventListener("mouseup", (e) => exports.Azalea.Web.WebEvents.ReportMouseUp(e.button));
canvas.addEventListener("keydown", (e) => exports.Azalea.Web.WebEvents.ReportKeyDown(e.key));
canvas.addEventListener("keyup", (e) => exports.Azalea.Web.WebEvents.ReportKeyUp(e.key));

//dotnet.run() starts the actual program main
await dotnet.run();


