#version 330 core
in vec4 oCol;
in vec2 oTex;

uniform sampler2D u_Texture;

out vec4 FragColor;

float median(float r, float g, float b) {
    return max(min(r, g), min(max(r, g), b));
}

float screenPxRange() {
    vec2 unitRange = vec2(2.0) / vec2(textureSize(u_Texture, 0));
    vec2 screenTexSize = vec2(1.0) / fwidth(oTex);
    return max(0.5 * dot(unitRange, screenTexSize), 1.0);
}

void main()
{
    vec3 msd = texture(u_Texture, oTex).rgb;
    float sd = median(msd.r, msd.g, msd.b);
    float screenPxDistance = screenPxRange()*(sd - 0.5);
    float opacity = clamp(screenPxDistance + 0.5, 0.0, 1.0);
    FragColor = vec4(oCol.x, oCol.y, oCol.z, oCol.w * opacity);
}