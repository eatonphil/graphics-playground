using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using SkiaSharp;

namespace SkiaGame.App
{
    class SkiaGameWindow : GameWindow
    {
        private int WindowWidth = 1280, WindowHeight = 720;

        private SkiaGameWindow() : base(GetDefaultDWS(), GetDefaultNWS())
        {
        }

        private static NativeWindowSettings GetDefaultNWS()
        {
            var retval = new NativeWindowSettings
            {
                WindowBorder = WindowBorder.Resizable,
                WindowState = WindowState.Normal,
                Title = "SkiaSharp Game Window"
            };

            if (Monitors.GetMonitors().Count > 0)
            {
                MonitorInfo info = Monitors.GetMonitors()[0];
                var area = info.ClientArea;
                retval.WindowState = WindowState.Maximized;
                retval.Location = area.Min;
                retval.Size = area.Size;
            }

            return retval;
        }

        private static GameWindowSettings GetDefaultDWS()
        {
            return new GameWindowSettings
            {
                RenderFrequency = 60.0,
                UpdateFrequency = 60.0
            };
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            SkiaInit();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            SkiaResize(e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {

            }
            SkiaRender((float)args.Time);
            SwapBuffers();
        }

        GRContext? skiaCtx = null;
        SKSurface? skSurface = null;

        private void SkiaInit()
        {
            skiaCtx = GRContext.CreateGl();
            SkiaResize(WindowWidth, WindowHeight);
        }

        private void SkiaCleanUp()
        {
            skiaCtx?.Dispose();
            skSurface?.Dispose();
        }

        private void SkiaResize(int w, int h)
        {
	    Console.WriteLine("W: {0}, H: {1}", w, h);
            GL.Viewport(0, 0, WindowWidth, WindowHeight);
            WindowWidth = w;
            WindowHeight = h;

            if (skiaCtx == null) SkiaInit();

            GRGlFramebufferInfo fbi = new GRGlFramebufferInfo(0, (uint)InternalFormat.Rgba8);
            var ctype = SKColorType.Rgba8888;
            var beTarget = new GRBackendRenderTarget(w, h, 0, 0, fbi);

            // Dispose Previous Surface
            skSurface?.Dispose();
            skSurface = SKSurface.Create(skiaCtx, beTarget, GRSurfaceOrigin.BottomLeft, ctype, null, null);
            if (skSurface == null)
            {
                Close();
            }
        }

        protected override void OnUnload()
        {
            SkiaCleanUp();
            base.OnUnload();
        }

        private void SkiaRender(float time)
        {
            var ctx = skSurface?.Canvas;

            ctx?.DrawColor(SKColors.White, SKBlendMode.Src);

            var paint = new SKPaint();
            paint.TextSize = 50;
            paint.Color = SKColors.Black;
            paint.Typeface = SKTypeface.FromFamilyName(
            "Arial",
            SKFontStyleWeight.Bold,
            SKFontStyleWidth.Normal,
            SKFontStyleSlant.Italic);
            ctx?.DrawText("Fancy text", 50, 50, paint);

            skiaCtx?.Flush();
        }

        static void Main(string[] args)
        {
            using (var prg = new SkiaGameWindow())
            {
                try
                {
                    prg.Run();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
