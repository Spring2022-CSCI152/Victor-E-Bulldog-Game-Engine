using SDL2;

namespace Bulldog
{
    public class GameTime
    {
        public static ulong currentTime = SDL.SDL_GetPerformanceCounter();
        public static ulong previousTime = 0;
        public static double deltaTime = 0;

        public static void Tick()
        {
            previousTime = currentTime;
            currentTime = SDL.SDL_GetPerformanceCounter();

            deltaTime = (double) ((currentTime - previousTime) * 1000 / (double) SDL.SDL_GetPerformanceFrequency());

        }
    }
}