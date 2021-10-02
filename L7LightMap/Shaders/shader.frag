#version 330 core
out vec4 FragColor;

struct Material{
    sampler2D diffuse;
    sampler2D specular;
    sampler2D emission;
    float shininess;
};

struct Light{
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Light light;
uniform Material material;
uniform vec3 viewPos; //The position of the view and/or of the player.
uniform float time;

in vec3 Normal; //The normal of the fragment is calculated in the vertex shader.
in vec3 FragPos; //The fragment position.
in vec2 TexCoords;

void main()
{
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));

    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = light.diffuse * (diff*vec3(texture(material.diffuse, TexCoords)));

    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = light.specular*(spec*vec3(texture(material.specular, TexCoords)));

    vec2 myTexCoords = TexCoords;
    myTexCoords.x = myTexCoords.x + 0.045f;
    vec3 emissionMap = vec3(texture(material.emission, myTexCoords + vec2(0.0, time*0.75)));
    vec3 emission = emissionMap * (sin(time)*0.5f+0.5f)*2.0;
    vec3 emissionMask = step(vec3(1.0f), vec3(1.0f)-specular);
    emission = emission * emissionMask;

    vec3 result = ambient + diffuse + specular + emission;

    FragColor = vec4(result, 1.0);
}