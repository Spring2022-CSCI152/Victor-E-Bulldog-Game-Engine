// See https://aka.ms/new-console-template for more information
using System;
using SDL2;

namespace Bulldog
{
    class Program
    {
        static void Main(string[] args)
        {
            Application game = new Test("test",720,1280,SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);
            game.Run();
        }
    }
    
}