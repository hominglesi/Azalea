// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import { dotnet } from './_framework/dotnet.js'

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

setModuleImports('JSImports', {
    draw: (color1, color2) => draw(color1, color2)
    //, add new functions here
});
const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

//main loop call


//dotnet.run() starts the actual program main
await dotnet.run();




function draw(color1, color2) {
    const canvas = document.getElementById("Canvas");
    if (canvas.getContext) {
        const ctx = canvas.getContext("2d");
        ctx.fillStyle = color1;
        ctx.fillRect(10, 10, 50, 50);

        ctx.fillStyle = color2;
        ctx.fillRect(30, 30, 50, 50);
    }
}