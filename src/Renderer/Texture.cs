using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.InteropServices;

namespace Bulldog.Renderer
{
    public class Texture : IDisposable
    {
        private uint _handle;
        private GL _gl;

        public unsafe Texture(GL gl, string path)
        {
            //Loading an image using image.sharp.
            var img = (Image<Rgba32>) Image.Load(path);

            // OpenGL has image origin in the bottom-left corner.
            fixed (void* data = &MemoryMarshal.GetReference(img.GetPixelRowSpan(0)))
            {
                //Loading the actual image.
                Load(gl, data, (uint) img.Width, (uint) img.Height);
            }

            //Deleting the img from image.sharp.
            img.Dispose();
        }

        public unsafe Texture(GL gl, Span<byte> data, uint width, uint height)
        {
            //We want the ability to create a texture using data generated from code as well.
            fixed (void* d = &data[0])
            {
                Load(gl, d, width, height);
            }
        }

        private unsafe void Load(GL gl, void* data, uint width, uint height)
        {
            //Saving the gl instance.
            _gl = gl;

            //Generating the opengl handle;
            _handle = _gl.GenTexture();
            Bind();

            //Setting the data of a texture.
            _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            //Setting some texture parameters so the texture behaves as expected.
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Linear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);

            //Generating mipmaps.
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }

        public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
        {
            //When we bind a texture we can choose which texture.slot we can bind it to.
            _gl.ActiveTexture(textureSlot);
            _gl.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public void Dispose()
        {
            //In order to dispose we need to delete the opengl handle for the texture.
            _gl.DeleteTexture(Handle);
        }
    }
}

