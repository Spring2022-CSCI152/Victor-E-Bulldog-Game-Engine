using SDL2;
using Silk.NET.Core;
using Silk.NET.OpenGL;

namespace Bulldog
{
    static class DisplayManger
    {
        public static IntPtr _mWindow = IntPtr.Zero;
        public static IntPtr _mContext = IntPtr.Zero;


        public static void CreateWindow(string title, int height, int width, SDL.SDL_WindowFlags type)
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                throw new InvalidOperationException("SDL could not be initialize: Error{0}");
            }
                        
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 2);
            SDL.SDL_GL_SetAttribute(
                SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK,
                SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
            SDL.SDL_GL_SetSwapInterval(0);
            
            _mWindow = SDL.SDL_CreateWindow(
                title,
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                width, height,
                type
            );
            if (_mWindow == null)
            {
                throw new InvalidOperationException("Unable to create Window");
            }
            _mContext =  SDL.SDL_GL_CreateContext(_mWindow);
            GL.ClearColor(1,1,1,1);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public static void DestroyWindow()
        {
            SDL.SDL_DestroyWindow(_mWindow);
            SDL.SDL_GL_DeleteContext(_mWindow);
            SDL.SDL_Quit();
        }
    }
}