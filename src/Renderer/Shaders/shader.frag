//Specifying the version like in our vertex shader.
#version 330 core
//The input variables, again prefixed with an f as they are the input variables of our fragment shader.
//These have to share name for now even though there is a way around this later on.
in vec2 fUv;

//A uniform of the type sampler2D will have the storage value of our texture.
uniform sampler2D uTexture0;

//The output of our fragment shader, this just has to be a vec3 or a vec4, containing the color information about
//each fragment or pixel of our geometry.
out vec4 FragColor;

void main()
{
    //Here we sample the texture based on the Uv coordinates of the fragment
    FragColor = texture(uTexture0, fUv);
}