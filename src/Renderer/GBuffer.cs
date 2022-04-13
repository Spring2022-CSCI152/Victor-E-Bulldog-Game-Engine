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
          _gl.BindFramebuffer( FramebufferTarget.Framebuffer , Handle);
          
          // bind normal to a 2d texture in the gBuffer
          gNormals = _gl.GenTexture();
          _gl.BindTexture(TextureTarget.Texture2D, Handle );
          _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba, width, height, 0, GLEnum.Rgba16f, GLEnum.Float , null);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMinFilter,(uint) GLEnum.Nearest);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMagFilter,(uint) GLEnum.Nearest);
          _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, gNormals,0);
          //  bind postion to a 2d texture in the gBuffer
          gPostion = _gl.GenTexture();
          _gl.BindTexture(TextureTarget.Texture2D, Handle );
          _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba, width, height, 0, GLEnum.Rgb16f, GLEnum.Float , null);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMinFilter,(uint) GLEnum.Nearest);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMagFilter,(uint) GLEnum.Nearest);
          _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, gPostion,0);
          // bind albedo to a 2d texture in the gBuffer
          gAlebedo = _gl.GenTexture();
          _gl.BindTexture(TextureTarget.Texture2D, Handle );
          _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba, width, height, 0, GLEnum.UnsignedByte, GLEnum.Float , null);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMinFilter,(uint) GLEnum.Nearest);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMagFilter,(uint) GLEnum.Nearest);
          _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, TextureTarget.Texture2D, gAlebedo,0);

          // ReSharper disable once HeapView.ObjectAllocation.Evident
          attachments = new []
          {
               GLEnum.ColorAttachment1, GLEnum.ColorAttachment1, GLEnum.ColorAttachment2
          };

          _gl.DrawBuffers(3, attachments);

          rboDepth = _gl.GenRenderbuffer();
          _gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
          _gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, GLEnum.DepthComponent, width, height);
          _gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rboDepth);

          var temp = _gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
          if (_gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
          {
               Console.WriteLine("framebuffer is not complete");
          }

          _gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
          
     }

}
// ReSharper disable once InvalidXmlDocComment
/**
public void Configure() {
     GLEnum[] drawBuffers = new GLEnum[Textures.Count];
     
     for(int texId = 0; texId < Textures.Count; texId++) {
     
          Renderer.Gl.BindTexture(TextureTarget.Texture2D, Textures[texId].GetHandle());
          Renderer.Gl.FramebufferTexture(GLEnum.Framebuffer, GLEnum.ColorAttachment0 + texId, Textures[texId].GetHandle(), 0)
          ;
          drawBuffers[texId] = GLEnum.ColorAttachment0 + texId;
     }
     Renderer.Gl.DrawBuffers((uint)Textures.Count, drawBuffers);
            
     if(Renderer.Gl.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete) {
          System.Console.WriteLine("Framebuffer configured incorrectly");
          System.Environment.Exit(-1);
     }
}*/