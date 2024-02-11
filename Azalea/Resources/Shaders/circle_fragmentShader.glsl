#version 330 core
in vec4 oCol;
in vec2 oTex;

uniform sampler2D u_Texture;

out vec4 FragColor;

void main()
{
    vec2 adjustedUV = (oTex - 0.5f) * 2;
    float alpha = 1 - length(adjustedUV);
    alpha = smoothstep(0.0, 0.1, alpha);

    FragColor = texture(u_Texture, oTex) * vec4(oCol.x, oCol.y, oCol.z, oCol.w);
    FragColor.a *= alpha;
}