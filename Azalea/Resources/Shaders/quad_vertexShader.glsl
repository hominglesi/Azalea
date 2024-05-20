#version 330 core
layout (location = 0) in vec2 vPos;
layout (location = 1) in vec4 vCol;
layout (location = 2) in vec2 vTex;
        
layout (std140) uniform Matrices 
{
    mat4 projection;
    float time;
};

out vec4 oCol;
out vec2 oTex;
out float iTime;

void main()
{
    gl_Position = projection * vec4(vPos.x, vPos.y, 1.0, 1.0);
    oCol = vCol;
    oTex = vTex;
    iTime = time;
}