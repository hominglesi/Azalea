window.nkCanvasGLContext.UniformMatrix4fv = function (uid, d) {
    var gc = nkJSObject.GetObject(uid);
    var uluid = Blazor.platform.readInt32Field(d, 0);
    var st = Blazor.platform.readInt32Field(d, 4);
    var arr = Blazor.platform.readInt32Field(d, 8);

    var arrPtr = Blazor.platform.getArrayEntryPtr(arr, 0, 4);
    var arrLen = Blazor.platform.getArrayLength(arr);
    var dt = new Float32Array(Module.HEAPU8.buffer, arrPtr, arrLen * st / 4);

    var ul = nkJSObject.GetObject(uluid);
    gc.uniformMatrix4fv(ul, false, dt);
}