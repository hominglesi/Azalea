import { dotnet } from './_framework/dotnet.js'

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const canvas = document.getElementById("Canvas");
const gl = canvas.getContext("webgl2", { antialias: false, alpha: false });
var lastWindowWidth = 0;
var lastWindowHeight = 0;

const audio = new AudioContext();
const masterVolumeGain = audio.createGain();
masterVolumeGain.connect(audio.destination);

setModuleImports('JSImports', {
    WebAudio: {
        BufferData: (buffer, data) => {
            var uInt8Array = data._unsafe_create_view();
            var int16Array = new Int16Array(uInt8Array.buffer, uInt8Array.byteOffset, uInt8Array.length / 2);

            var channelDataPointers = new Array(buffer.numberOfChannels);
            for (let i = 0; i < buffer.numberOfChannels; i++) {
                channelDataPointers[i] = buffer.getChannelData(i);
            }

            var c = 1 / 32768;

            for (let i = 0; i < int16Array.length / buffer.numberOfChannels; i++) {
                for (var j = 0; j < buffer.numberOfChannels; j++) {
                    channelDataPointers[j][i] = int16Array[(i * buffer.numberOfChannels) + j] * c;
                }
            }
        },
        Connect: (source, destination) => source.connect(destination),
        ConnectToContext: (source) => source.connect(masterVolumeGain),
        CreateBuffer: (channels, length, sampleRate) => audio.createBuffer(channels, length, sampleRate),
        CreateBufferSource: () => audio.createBufferSource(),
        CreateGain: () => audio.createGain(),
        SetBuffer: (source, buffer) => source.buffer = buffer,
        SetGain: (gainNode, gain) => gainNode.gain.setValueAtTime(gain, audio.currentTime),
        SetLoop: (source, loop) => source.loop = loop,
        SetMasterVolume: (volume) => masterVolumeGain.gain.setValueAtTime(volume, audio.currentTime),
        StartSource: (source) => source.start(0),
        StopSource: (source) => source.stop()
    },
    WebEvents: {
        CheckClientResized: () => {
            if (lastWindowWidth == window.innerWidth && lastWindowHeight == window.innerHeight)
                return;

            canvas.width = lastWindowWidth = window.innerWidth;
            canvas.height = lastWindowHeight = window.innerHeight;
            webEvents.InvokeClientResized(window.innerWidth, window.innerHeight);
        },
        RequestAnimationFrame: () => window.requestAnimationFrame(webEvents.InvokeAnimationFrameRequested)
    },
    WebFunctions: {
        GetCurrentPreciseTime: () => new Date(performance.now()),
        SetTitle: (title) => document.title = title
    },
    WebGL: {
        ActiveTexture: (texture) => gl.activeTexture(texture),
        AttachShader: (program, shader) => gl.attachShader(program, shader),
        BindBuffer: (target, buffer) => gl.bindBuffer(target, buffer),
        BindTexture: (target, texture) => gl.bindTexture(target, texture),
        BindVertexArray: (vertexArray) => gl.bindVertexArray(vertexArray),
        BlendFuncSeparate: (srcRGB, dstRGB, srcAlpha, dstAlpha) => gl.blendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha),
        BufferData: (target, size, usage) => gl.bufferData(target, size._unsafe_create_view(), usage),
        ClearColor: (red, green, blue, alpha) => gl.clearColor(red, green, blue, alpha),
        Clear: (mask) => gl.clear(mask),
        CompileShader: (shader) => gl.compileShader(shader),
        CreateBuffer: () => gl.createBuffer(),
        CreateProgram: () => gl.createProgram(),
        CreateShader: (type) => gl.createShader(type),
        CreateTexture: () => gl.createTexture(),
        CreateVertexArray: () => gl.createVertexArray(),
        DeleteBuffer: (buffer) => gl.DeleteBuffer(buffer),
        DeleteProgram: (program) => gl.deleteProgram(program),
        DeleteShader: (shader) => gl.deleteShader(shader),
        DeleteTexture: (texture) => gl.deleteTexture(texture),
        DeleteVertexArray: (vertexArray) => gl.deleteVertexArray(vertexArray),
        Disable: (capability) => gl.disable(capability),
        DrawArrays: (mode, first, count) => gl.drawArrays(mode, first, count),
        DrawElements: (mode, count, type, offset) => gl.drawElements(mode, count, type, offset),
        Enable: (cap) => gl.enable(cap),
        EnableVertexAttribArray: (index) => gl.enableVertexAttribArray(index),
        GenerateMipmap: (target) => gl.generateMipmap(target),
        GetAttribLocation: (program, name) => gl.getAttribLocation(program, name),
        GetBufferParameter: (target, pname) => gl.getBufferParameter(target, pname),
        GetProgramInfoLog: (program) => gl.getProgramInfoLog(program),
        GetProgramParameter: (program, pname) => gl.getProgramParameter(program, pname),
        GetShaderInfoLog: (shader) => gl.getShaderInfoLog(shader),
        GetShaderParameter: (shader, pname) => gl.getShaderParameter(shader, pname),
        GetShaderSource: (shader) => gl.getShaderSource(shader),
        GetUniformLocation: (program, name) => gl.getUniformLocation(program, name),
        LinkProgram: (program) => gl.linkProgram(program),
        Scissor: (x, y, width, height) => gl.scissor(x, y, width, height),
        ShaderSource: (shader, source) => gl.shaderSource(shader, source),
        TexImage2D: (target, level, internalformat, width, height, border, format, type, source) => gl.texImage2D(target, level, internalformat, width, height, border, format, type, source._unsafe_create_view()),
        TexParameteri: (target, pname, param) => gl.texParameteri(target, pname, param),
        Uniform1i: (location, v0) => gl.uniform1i(location, v0),
        Uniform1iv: (location, value) => gl.uniform1iv(location, value),
        Uniform4f: (location, v0, v1, v2, v3) => gl.uniform4f(location, v0, v1, v2, v3),
        UniformMatrix4fv: (location, transpose, value) => gl.uniformMatrix4fv(location, transpose, value),
        UseProgram: (program) => gl.useProgram(program),
        ValidateProgram: (program) => gl.validateProgram(program),
        VertexAttribPointer: (index, size, type, normalized, stride, offset) => gl.vertexAttribPointer(index, size, type, normalized, stride, offset),
        Viewport: (x, y, width, height) => gl.viewport(x, y, width, height)
    },
    WebLocalStorage: {
        Clear: () => localStorage.clear(),
        GetItem: (key) => localStorage.getItem(key),
        GetLength: () => localStorage.length,
        Key: (index) => localStorage.key(index),
        RemoveItem: (key) => localStorage.removeItem(key),
        SetItem: (key, value) => localStorage.setItem(key, value)
    }
});

var exports = await getAssemblyExports("Azalea.Web");
var webEvents = exports.Azalea.Web.Platform.WebEvents;
var webInput = exports.Azalea.Web.Platform.WebInput;

window.addEventListener("beforeunload", (e) => webEvents.InvokeWindowClosing());
canvas.addEventListener("wheel", (e) => webInput.ReportScroll(e.deltaY / 100));
canvas.addEventListener("mousemove", (e) => webInput.ReportMouseMove(e.pageX, e.pageY));
canvas.addEventListener("mousedown", (e) => webInput.ReportMouseDown(e.button));
canvas.addEventListener("mouseup", (e) => webInput.ReportMouseUp(e.button));
canvas.addEventListener("keyup", (e) => webInput.ReportKeyUp(e.key));
canvas.addEventListener("keydown", (e) =>
{
    if (e.repeat)
        webInput.ReportKeyRepeat(e.key);
    else
        webInput.ReportKeyDown(e.key);

    if (e.key.length == 1 && e.ctrlKey == false) {
        webInput.ReportCharInput(e.key.codePointAt(0));
    }
});

document.getElementById("LoadingText").style.display = "None";
document.getElementById("Canvas").style.display = "Block";

//dotnet.run() starts the actual program main
await dotnet.run();


