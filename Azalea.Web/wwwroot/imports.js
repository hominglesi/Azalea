const canvas = document.getElementById("Canvas");
const gl = canvas.getContext("webgl");

// WebGL
const AttachShader = (program, shader) => gl.attachShader(program, shader);
const BindBuffer = (target, buffer) => gl.bindBuffer(target, buffer);
const BindTexture = (target, texture) => gl.bindTexture(target, texture);
const BlendFuncSeparate = (srcRGB, dstRGB, srcAlpha, dstAlpha) => gl.blendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha);
const BufferData = (target, size, usage) => gl.bufferData(target, size, usage);
const ClearColor = (red, green, blue, alpha) => gl.clearColor(red, green, blue, alpha);
const Clear = (mask) => gl.clear(mask);
const CompileShader = (shader) => gl.compileShader(shader);
const CreateBuffer = () => gl.createBuffer();
const CreateProgram = () => gl.createProgram();
const CreateShader = (type) => gl.createShader(type);
const CreateTexture = () => gl.createTexture();
const Disable = (capability) => gl.disable(capability);
const DrawElements = (mode, count, type, offset) => gl.drawElements(mode, count, type, offset);
const Enable = (cap) => gl.enable(cap);
const EnableVertexAttribArray = (index) => gl.enableVertexAttribArray(index);
const GetShaderSource = (shader) => gl.getShaderSource(shader);
const GetUniformLocation = (program, name) => gl.getUniformLocation(program, name);
const LinkProgram = (program) => gl.linkProgram(program);
const Scissor = (x, y, width, height) => gl.scissor(x, y, width, height);
const ShaderSource = (shader, source) => gl.shaderSource(shader, source);
const Uniform1i = (location, v0) => gl.uniform1i(location, v0);
const Uniform1iv = (location, value) => gl.uniform1iv(location, value);
const Uniform4f = (location, v0, v1, v2, v3) => gl.uniform4f(location, v0, v1, v2, v3);
const UniformMatrix4fv = (location, transpose, value) => gl.uniformMatrix4fv(location, transpose, value);
const VertexAttribPointer = (index, size, type, normalized, stride, offset) => gl.vertexAttribPointer(index, size, type, normalized, stride, offset);
const Viewport = (x, y, width, height) => gl.viewport(x, y, width, height);

// WebEvents
const RequestAnimationFrame = () => window.requestAnimationFrame(InvokeAnimationFrameRequested);