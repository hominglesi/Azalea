import { dotnet } from './_framework/dotnet.js'


const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

setModuleImports('JSImports', {
    draw: (color1, color2) => draw(color1, color2),
    clearColor: (r, g, b, a) => clearColor(r, g, b, a),
    clearScreen: () => clearScreen(),
    createBuffer: () => createBuffer(),
    bindBuffer: (target, buffer) => bindBuffer(target, buffer),
    bufferData: (target, size, usage) => bufferData(target, size, usage),
    createShader: (type) => createShader(type),
    shaderSource: (shader, source) => shaderSource(shader, source),
    getShaderSource: (shader) => getShaderSource(shader),
    compileShader: (shader) => compileShader(shader),
    createProgram: () => createProgram(),
    attachShader: (program, shader) => attachShader(program, shader),
    linkProgram: (program) => linkProgram(program),
    vertexAttribPointer: (index, size, type, normalized, stride, offset) => vertexAttribPointer(index, size, type, normalized, stride, offset),
    enableVertexAttribArray: (index) => enableVertexAttribArray(index),
    enable: (cap) => enable(cap),
    blendFuncSeparate: (srcRGB, dstRGB, srcAlpha, dstAlpha) => blendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha),
    getUniformLocation: (program, name) => getUniformLocation(program, name),
    uniform1i: (location, v0) => uniform1i(location, v0),
    uniform1iv: (location, value) => uniform1iv(location, value),
    uniform4f: (location, v0, v1, v2, v3) => uniform4f(location, v0, v1, v2, v3),
    uniformMatrix4fv: (location, transpose, value) => uniformMatrix4fv(location, transpose, value),
    drawElements: (mode, count, type, offset) => drawElements(mode, count, type, offset),
    scissor: (x, y, width, height) => scissor(x, y, width, height),
    disable: (capability) => disable(capability),
    viewport: (x, y, width, height) => viewport(x, y, width, height),
    bindTexture: (target, texture) => bindTexture(target, texture),
    createTexture: () => createTexture()
    // add all function names from imports
});


const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

//dotnet.run() starts the actual program main
await dotnet.run();




