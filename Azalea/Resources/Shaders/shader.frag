#version 450

layout(binding = 1) uniform sampler2D texSamplers[1024];

layout(location = 0) in vec3 fragColor;
layout(location = 1) in flat uint fragTexIndex;
layout(location = 2) in vec2 fragTexCoord;

layout(location = 0) out vec4 outColor;

void main() {
    outColor = texture(texSamplers[fragTexIndex], fragTexCoord) * vec4(fragColor, 1.0);
}