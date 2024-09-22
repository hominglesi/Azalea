const canvas = document.getElementById("Canvas");
const gl = canvas.getContext("webgl");

function clearColor(r,g,b,a) {
    gl.clearColor(r, g, b, a);
}
function clearScreen() {
    gl.clear(gl.COLOR_BUFFER_BIT);
}
function createBuffer() {
    return gl.createBuffer();
}
function bindBuffer(target,buffer) {
    gl.bindBuffer(target, buffer);
}
function bufferData(target, size, usage) {
    gl.bufferData(target,size,usage);
}
function createShader(type) {
    return gl.createShader(type);
}
function shaderSource(shader,source) {
    return gl.ShaderSource(shader, source);
}
function getShaderSource(shader) {
    return gl.getshaderSource(shader);
}
function compileShader(shader) {
    gl.compileShader(shader);
}
function createProgram() {
    return gl.createProgram();
}
function attachShader(program,shader) {
    gl.attachShader(program, shader);
}
function linkProgram(program) {
    gl.linkProgram(program);
}
function vertexAttribPointer(index, size, type, normalized, stride, offset) {
    gl.vertexAttribPointer(index, size, type, normalized, stride, offset);
}
function enableVertexAttribArray(index) {
    gl.enableVertexAttribArray(index);
}
function enable(cap) {
    gl.enable(cap);

}
function blendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha) {
    gl.blendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha);
}
function getUniformLocation(program, name) {
    return gl.getUniformLocation(program, name);
}
//uniforme i ostala odeca
function uniform1i(location, v0) {
    gl.uniform1i(location, v0);
}
function uniform1iv(location, value) {
    gl.uniform1iv(location, value);
}
function uniform4f(location, v0, v1, v2, v3) {
    gl.uniform4f(location, v0, v1, v2, v3);
}
function uniformMatrix4fv(location, transpose, value) {
    gl.uniformMatrix4fv(location, transpose, value);
}
function drawElements(mode, count, type, offset) {
    return gl.drawElements(mode, count, type, offset);
}
function scissor(x, y, width, height) {
    gl.scissor(x, y, width, height);
}
function disable(capability) {
    gl.disable(capability);
}
function viewport(x, y, width, height) {
    gl.viewport(x, y, width, height);
}
function bindTexture(target, texture) {
    gl.bindTexture(target, texture);
}
function createTexture() {
    return gl.createTexture();
}