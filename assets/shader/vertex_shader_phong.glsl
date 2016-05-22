varying vec3 N; // Normal vector
varying vec3 p; // Surface point
varying vec3 color; // Surface color
uniform vec3 camera_position; // Set in Java application
varying vec2 texture_coordinate; // Texture coordinate

/**
 * Vertex shader: Phong lighting model, Phong shading.
 */
void main(void)
{
    p  = vec3(gl_Vertex);
    N = gl_Normal;
    color = gl_Color.xyz;
    texture_coordinate = vec2(gl_MultiTexCoord0);
    gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
}