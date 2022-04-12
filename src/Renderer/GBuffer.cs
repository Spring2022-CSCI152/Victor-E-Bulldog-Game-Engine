using Silk.NET.OpenGL;
// ReSharper disable InconsistentNaming

namespace Bulldog.Renderer;


public unsafe class gBuffer
{
     private uint _handel;
     private GL _gl;
     public uint rboDepth;
     public uint gAlebedo, gNormals, gPbr, gPostion;
     private GLEnum[] attachments;

     public void init(uint height, uint width, GL gl)
     {
          _gl = gl;
          // generate offscreen gBuffer
          _gl.GenFramebuffers( 1, (uint*) _handel);
          _gl.BindFramebuffer( FramebufferTarget.Framebuffer , _handel);
          
          // bind normal to a 2d texture in the gBuffer
          _gl.GenTextures(1, (uint*) gNormals);
          _gl.BindTexture(TextureTarget.Texture2D, gNormals );
          _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba, width, height, 0, GLEnum.Rgb16f, GLEnum.Float , null);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMinFilter,(uint) GLEnum.Nearest);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMagFilter,(uint) GLEnum.Nearest);
          _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, gNormals,0);
          //  bind postion to a 2d texture in the gBuffer
          _gl.GenTextures(1, (uint*) gPostion);
          _gl.BindTexture(TextureTarget.Texture2D, gPostion );
          _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba, width, height, 0, GLEnum.Rgb16f, GLEnum.Float , null);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMinFilter,(uint) GLEnum.Nearest);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMagFilter,(uint) GLEnum.Nearest);
          _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, gPostion,0);
          //  bind PBR maps to a 2d texture in the gBuffer
          _gl.GenTextures(1, (uint*) gPbr);
          _gl.BindTexture(TextureTarget.Texture2D, gPbr );
          _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.RG8, width, height, 0, GLEnum.UnsignedByte, GLEnum.Float , null);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMinFilter,(uint) GLEnum.Nearest);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMagFilter,(uint) GLEnum.Nearest);
          _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, TextureTarget.Texture2D, gPbr,0);
          // bind albedo to a 2d texture in the gBuffer
          _gl.GenTextures(1, (uint*) gAlebedo);
          _gl.BindTexture(TextureTarget.Texture2D, gAlebedo );
          _gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba, width, height, 0, GLEnum.UnsignedByte, GLEnum.Float , null);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMinFilter,(uint) GLEnum.Nearest);
          _gl.TexParameterI(TextureTarget.Texture2D,GLEnum.TextureMagFilter,(uint) GLEnum.Nearest);
          _gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment3, TextureTarget.Texture2D, gAlebedo,0);

          // ReSharper disable once HeapView.ObjectAllocation.Evident
          attachments = new []
          {
               (GLEnum) FramebufferAttachment.ColorAttachment0, (GLEnum) FramebufferAttachment.ColorAttachment1, (GLEnum) FramebufferAttachment.ColorAttachment2, (GLEnum) FramebufferAttachment.ColorAttachment3
          };

          _gl.DrawBuffers(4, attachments);

          _gl.GenRenderbuffers(1,(uint*) rboDepth);
          _gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboDepth);
          _gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, GLEnum.DepthComponent, width, height);
          _gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rboDepth);

     }

}