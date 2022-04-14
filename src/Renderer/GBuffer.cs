using Silk.NET.OpenGL;

// ReSharper disable InconsistentNaming

namespace Bulldog.Renderer;


public unsafe static class GBuffer
{
     public static uint Handle { get; private set; }

     private static GL _gl;
     public static uint rboDepth { get; private set; }
     public static uint gAlebedo { get; private set; }
     public static uint gNormals { get; private set; }
     public static uint gPbr { get; private set; }
     public static uint gPostion { get; private set; }
     private static GLEnum[] attachments;

     public static void init(uint height, uint width, GL gl)
     {
          _gl = gl;
          // generate offscreen gBuffer
          Handle = _gl.GenFramebuffer();
          _gl.BindFramebuffer( GLEnum.Framebuffer , Handle);
          
          gPostion = _gl.GenTexture();
          _gl.BindTexture(TextureTarget.Texture2D, gPostion );
          _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba16f, width, height, 0, GLEnum.Rgba, GLEnum.Float , null);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMinFilter,(uint) GLEnum.Nearest);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMagFilter,(uint) GLEnum.Nearest);
          _gl.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0, TextureTarget.Texture2D, gPostion,0);
          
          gNormals = _gl.GenTexture();
          _gl.BindTexture(TextureTarget.Texture2D, gNormals );
          _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba16f, width, height, 0, GLEnum.Rgba, GLEnum.Float , null);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMinFilter,(uint) GLEnum.Nearest);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMagFilter,(uint) GLEnum.Nearest);
          _gl.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment1, TextureTarget.Texture2D, gNormals,0);
          
          gAlebedo = _gl.GenTexture();
           _gl.BindTexture(TextureTarget.Texture2D, gAlebedo );
           _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba, width, height, 0, GLEnum.Rgba, GLEnum.Float , null);
           _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMinFilter,(uint) GLEnum.Nearest);
           _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMagFilter,(uint) GLEnum.Nearest);
           _gl.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment2, TextureTarget.Texture2D, gAlebedo,0);
          // ReSharper disable once HeapView.ObjectAllocation.Evident
          //gl.TexImage2D(/*target*/, 0, /* internal format */, image.Width, image.Height, 0, /* format */, /* type */, null);
          
          attachments = new []
          {
               GLEnum.ColorAttachment0, GLEnum.ColorAttachment1, GLEnum.ColorAttachment2
          };

          _gl.DrawBuffers(attachments);

          rboDepth = _gl.GenRenderbuffer();
          _gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
          _gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, GLEnum.DepthComponent, width, height);
          _gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rboDepth);

          var temp = _gl.CheckFramebufferStatus(GLEnum.Framebuffer);
          if (_gl.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete)
          {
               Console.WriteLine("framebuffer is not complete");
          }

          _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
          
     }

}
