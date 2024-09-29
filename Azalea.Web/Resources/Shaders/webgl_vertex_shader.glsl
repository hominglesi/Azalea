#version 300 es
precision highp float;

in vec2 vPos;
in vec4 vCol;
in vec2 vTex;

uniform mat4 u_Projection;

out vec4 oCol;
out vec2 oTex;

void main(){
	gl_Position = u_Projection * vec4(vPos.x, vPos.y, 1.0, 1.0);
    oCol = vCol;
    oTex = vTex;
}