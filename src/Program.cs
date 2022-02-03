// See https://aka.ms/new-console-template for more information
using System;
using SDL2;

namespace Bulldog
{
    class Program
    {
        static void Main(string[] args)
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            var window = IntPtr.Zero;
            window = SDL.SDL_CreateWindow(
                "Test", 
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED, 
                1020, 860, 
                SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE
            );
            //SDL.SDL_Delay(5000);
            SDL.SDL_Event e;
            bool quit = false;
            while (!quit)
            {
                while (SDL.SDL_PollEvent(out e) != 0)
                {
                    switch (e.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            quit = true;
                            break;
                    }
                }
            }
            SDL.SDL_DestroyWindow(window);
            SDL.SDL_Quit();
        }
    }
    
}