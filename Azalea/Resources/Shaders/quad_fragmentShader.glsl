#version 330 core
in vec4 oCol;
in vec2 oTex;

uniform sampler2D u_Texture;

out vec4 FragColor;

void main()
{
    FragColor = texture(u_Texture, oTex) * vec4(oCol.x, oCol.y, oCol.z, oCol.w);
}