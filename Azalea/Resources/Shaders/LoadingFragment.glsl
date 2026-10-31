#version 330 core
in vec4 oCol;
in vec2 oTex;

uniform sampler2D u_Texture;
uniform vec2 u_Offset;
uniform vec2 u_Resolution;
uniform vec2 u_ScreenResolution;
uniform float u_Time;

uniform vec4 u_BackgroundColor;
uniform vec4 u_RingColor;
uniform vec4 u_CircleColor;

out vec4 FragColor;

#define PI 3.14159265

float anim_speed   = 1.5;
float aa_level     = 1.375;
float r_small_rate = 1.0 / 7.0;

float sdCircle(in vec2 p, in float r)
{
    return length(p) - r;
}

void main()
{
    vec2 regionSize = vec2(50, 50);
    vec2 resCenter = u_Resolution / 2;

    vec2 pixelCoordinate = vec2(gl_FragCoord.x - u_Offset.x, (u_Resolution.y - gl_FragCoord.y) + (u_ScreenResolution.y - (u_Offset.y + u_Resolution.y)));
    vec2 regionPosition = (u_Resolution - regionSize) / 2;

    if(pixelCoordinate.x < regionPosition.x || pixelCoordinate.y < regionPosition.y 
    || pixelCoordinate.x > regionSize.x + regionPosition.x || pixelCoordinate.y > regionSize.y + regionPosition.y){
        FragColor = u_BackgroundColor;
        return;
    }

    vec2 uv = ((pixelCoordinate.xy - regionPosition) / regionSize) * 2.0 - 1.0;

    vec4 col = u_BackgroundColor;

    float aa_width = aa_level / min(regionSize.x, regionSize.y);

    float time = u_Time * anim_speed;

    float r_big = 1.0 / (r_small_rate + 1.0);
    float r_small = r_big * r_small_rate;

    float t_ring = abs(sdCircle(uv, r_big)) - r_small;
    t_ring = 1.0 - smoothstep(-aa_width, aa_width, t_ring);

    col = mix(col, u_RingColor, t_ring);

    vec2  a = vec2(4.0 / 5.0, 5.0 / 4.0);
          a = (pow(vec2(mod(time, 3.0) / 3.0), a) * 3.0) * PI * 2.0;
    vec2  p_cir_1 = vec2(sin(a.x), cos(a.x)) * r_big;
    vec2  p_cir_2 = vec2(sin(a.y), cos(a.y)) * r_big;

    float t_cir =     sdCircle(p_cir_1 - uv, r_small);
          t_cir = min(sdCircle(p_cir_2 - uv, r_small), t_cir);
          t_cir = 1.0 - smoothstep(-aa_width, aa_width, t_cir);

    vec2  uv_dir = normalize(uv);
    float is_right_dir = step(0.0, uv_dir.x) * 2.0 - 1.0;

    float a_uv  = acos(uv_dir.y * is_right_dir);
          a_uv += step(is_right_dir, 0.0) * PI;
    vec2  a_floor = floor(a / (PI * 2.0));

    vec2 b_uv = a_uv + a_floor * PI * 2.0;
         b_uv = step(vec2(0.0), min(b_uv - a.y, a.x - b_uv));
    t_cir = min(1.0, max(b_uv.x, b_uv.y) + t_cir) * t_ring;
    col = mix(col, u_CircleColor, t_cir);

    FragColor = col;
}
