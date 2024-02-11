#version 330 core
layout (location = 0) in vec2 vPos;
layout (location = 1) in vec4 vCol;
layout (location = 2) in vec2 vTex;
        
uniform mat4 u_Projection;

out vec4 oCol;
out vec2 oTex;

void main()
{
    gl_Position = u_Projection * vec4(vPos.x, vPos.y, 1.0, 1.0);
    oCol = vCol;
    oTex = vTex;
}