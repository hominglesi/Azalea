const canvas = document.getElementById("Canvas");
const gl = canvas.getContext("webgl2", { antialias: false, alpha: false });
var lastWindowWidth = 0;
var lastWindowHeight = 0;

const audio = new AudioContext();
const masterVolumeGain = audio.createGain();
masterVolumeGain.connect(audio.destination);

// WebGL
const ActiveTexture = (texture) => gl.activeTexture(texture);
const AttachShader = (program, shader) => gl.attachShader(program, shader);
const BindBuffer = (target, buffer) => gl.bindBuffer(target, buffer);
const BindTexture = (target, texture) => gl.bindTexture(target, texture);
const BindVertexArray = (vertexArray) => gl.bindVertexArray(vertexArray);
const BlendFuncSeparate = (srcRGB, dstRGB, srcAlpha, dstAlpha) => gl.blendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha);
const BufferData = (target, size, usage) => gl.bufferData(target, size._unsafe_create_view(), usage);
const ClearColor = (red, green, blue, alpha) => gl.clearColor(red, green, blue, alpha);
const Clear = (mask) => gl.clear(mask);
const CompileShader = (shader) => gl.compileShader(shader);
const CreateBuffer = () => gl.createBuffer();
const CreateProgram = () => gl.createProgram();
const CreateShader = (type) => gl.createShader(type);
const CreateTexture = () => gl.createTexture();
const CreateVertexArray = () => gl.createVertexArray();
const DeleteBuffer = (buffer) => gl.DeleteBuffer(buffer);
const DeleteProgram = (program) => gl.deleteProgram(program);
const DeleteShader = (shader) => gl.deleteShader(shader);
const DeleteTexture = (texture) => gl.deleteTexture(texture);
const DeleteVertexArray = (vertexArray) => gl.deleteVertexArray(vertexArray);
const Disable = (capability) => gl.disable(capability);
const DrawArrays = (mode, first, count) => gl.drawArrays(mode, first, count);
const DrawElements = (mode, count, type, offset) => gl.drawElements(mode, count, type, offset);
const Enable = (cap) => gl.enable(cap);
const EnableVertexAttribArray = (index) => gl.enableVertexAttribArray(index);
const GenerateMipmap = (target) => gl.generateMipmap(target);
const GetAttribLocation = (program, name) => gl.getAttribLocation(program, name);
const GetBufferParameter = (target, pname) => gl.getBufferParameter(target, pname);
const GetProgramInfoLog = (program) => gl.getProgramInfoLog(program);
const GetProgramParameter = (program, pname) => gl.getProgramParameter(program, pname);
const GetShaderInfoLog = (shader) => gl.getShaderInfoLog(shader);
const GetShaderParameter = (shader, pname) => gl.getShaderParameter(shader, pname);
const GetShaderSource = (shader) => gl.getShaderSource(shader);
const GetUniformLocation = (program, name) => gl.getUniformLocation(program, name);
const LinkProgram = (program) => gl.linkProgram(program);
const Scissor = (x, y, width, height) => gl.scissor(x, y, width, height);
const ShaderSource = (shader, source) => gl.shaderSource(shader, source);
const TexImage2D = (target, level, internalformat, width, height, border, format, type, source) => gl.texImage2D(target, level, internalformat, width, height, border, format, type, source._unsafe_create_view());
const TexParameteri = (target, pname, param) => gl.texParameteri(target, pname, param);
const Uniform1i = (location, v0) => gl.uniform1i(location, v0);
const Uniform1iv = (location, value) => gl.uniform1iv(location, value);
const Uniform4f = (location, v0, v1, v2, v3) => gl.uniform4f(location, v0, v1, v2, v3);
const UniformMatrix4fv = (location, transpose, value) => gl.uniformMatrix4fv(location, transpose, value);
const UseProgram = (program) => gl.useProgram(program);
const ValidateProgram = (program) => gl.validateProgram(program);
const VertexAttribPointer = (index, size, type, normalized, stride, offset) => gl.vertexAttribPointer(index, size, type, normalized, stride, offset);
const Viewport = (x, y, width, height) => gl.viewport(x, y, width, height);

// WebEvents
const RequestAnimationFrame = () => window.requestAnimationFrame(InvokeAnimationFrameRequested);
const CheckClientSize = () => {
    if (lastWindowWidth == window.innerWidth && lastWindowHeight == window.innerHeight)
        return;

    canvas.width = lastWindowWidth = window.innerWidth;
    canvas.height = lastWindowHeight = window.innerHeight;
    UpdateClientSize(window.innerWidth, window.innerHeight);
}
const GetCurrentPreciseTime = () => new Date(performance.now());

// WebAudio
const BufferAudioData = (buffer, data) => {
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
}
const Connect = (source, destination) => source.connect(destination);
const ConnectToContext = (source) => source.connect(masterVolumeGain);
const CreateAudioBuffer = (channels, length, sampleRate) => audio.createBuffer(channels, length, sampleRate);
const CreateBufferSource = () => audio.createBufferSource();
const CreateGain = () => audio.createGain();
const SetBuffer = (source, buffer) => source.buffer = buffer;
const SetGain = (gainNode, gain) => gainNode.gain.setValueAtTime(gain, audio.currentTime);
const SetLoop = (source, loop) => source.loop = loop;
const SetMasterVolume = (volume) => masterVolumeGain.gain.setValueAtTime(volume, audio.currentTime);
const StartSource = (source) => source.start(0);
const StopSource = (source) => source.stop();