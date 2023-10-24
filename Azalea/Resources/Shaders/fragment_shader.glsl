#version 330 core

out vec4 OutputColor;

uniform vec4 u_Color;

void main()
{
    OutputColor = u_Color;
}