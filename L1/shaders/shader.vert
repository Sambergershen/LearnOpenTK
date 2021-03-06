#version 330 core
in vec3 aPosition;
in vec3 aColor;
out vec4 color;
uniform mat4 modelview;

void main()
{
    gl_Position  = vec4(aPosition, 1.0);
    color = vec4(aColor, 1.0);
}
