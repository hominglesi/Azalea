import { dotnet } from './_framework/dotnet.js'

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

setModuleImports('JSImports', {
    WebGL: {
        AttachShader,
        BindBuffer,
        BindTexture,
        BlendFuncSeparate,
        BufferData,
        ClearColor,
        Clear,
        CompileShader,
        CreateBuffer,
        CreateProgram,
        CreateShader,
        CreateTexture,
        DeleteBuffer,
        DeleteProgram,
        DeleteShader,
        Disable,
        DrawArrays,
        DrawElements,
        Enable,
        EnableVertexAttribArray,
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
        RequestAnimationFrame
    }
});

const config = getConfig();
exports = await getAssemblyExports(config.mainAssemblyName);

InvokeWindowResized();
window.addEventListener("resize", InvokeWindowResized);

//dotnet.run() starts the actual program main
await dotnet.run();


