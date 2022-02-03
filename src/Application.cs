using SDL2;

namespace Bulldog
{
    public abstract class Application
    {
        public Application( string initialWindowTitle, int initialWindowHeight, int initialWindowWidth, SDL.SDL_WindowFlags flags)
        {
            Running = true;
            Minimized = false;
            InitialWindowHeight = initialWindowHeight;
            InitialWindowWidth = initialWindowWidth;
            InitialWindowTitle = initialWindowTitle;
            Flags = flags;
        }
        protected bool Running { get; set; }
        protected bool Minimized { get; set; }
        protected int InitialWindowHeight { get; set; }
        protected int InitialWindowWidth { get; set; }
        protected string InitialWindowTitle { get; set; }
        protected SDL.SDL_WindowFlags Flags { get; set; }

        protected SDL.SDL_Event e;

        public void Run()
        {
            Initialize();
            DisplayManger.CreateWindow(InitialWindowTitle, InitialWindowHeight, InitialWindowWidth, Flags);
            LoadContent();

            while (Running)
            {
                Update();
                SDL.SDL_PollEvent(out e);
                Render();
            }
            DisplayManger.DestroyWindow();
        }
        protected abstract void Initialize();
        protected abstract void LoadContent();

        protected abstract void Update();
        protected abstract void Render();
    }
}
