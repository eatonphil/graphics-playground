using System.Text;

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
		//retval.Size = new OpenTK.Mathematics.Vector2i(400, 100);
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

	StringBuilder text;
        protected override void OnLoad()
        {
            base.OnLoad();
	    skiaCtx = GRContext.CreateGl();

	    text = new StringBuilder("안영");
        }

        protected override void OnResize(ResizeEventArgs e)
        {
	    base.OnResize(e);
            WindowWidth = e.Width;
            WindowHeight = e.Height;
	    skiaCtx.ResetContext(GRBackendState.None);
        }

	protected override void OnTextInput(TextInputEventArgs args) {
	    text.Append(args.AsString);
	}

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GRGlFramebufferInfo fbi = new GRGlFramebufferInfo(0, (uint)InternalFormat.Rgba8);
            var ctype = SKColorType.Rgba8888;
            var beTarget = new GRBackendRenderTarget(ClientSize.X, ClientSize.Y, 0, 0, fbi);

            // Dispose Previous Surface
            skSurface?.Dispose();
            skSurface = SKSurface.Create(skiaCtx, beTarget, GRSurfaceOrigin.BottomLeft, ctype, null, null);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {

            }
            SkiaRender((float)args.Time);
            SwapBuffers();
        }

        GRContext? skiaCtx = null;
        SKSurface? skSurface = null;

        private void SkiaCleanUp()
        {
            skiaCtx?.Dispose();
            skSurface?.Dispose();
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
            //paint.Typeface = SKTypeface.FromFamilyName("Apple SD 산돌고딕 Neo");
	    //paint.Typeface = SKTypeface.FromFile("IBMPlexSansKR-Regular.ttf");
	    paint.Typeface = SKTypeface.FromFamilyName("Arial");
	    paint.IsAntialias = true;
	    foreach (var g in paint.GetGlyphs(text.ToString())) {
		if (g == 0) {
		    throw new System.Exception("text not renderable in font");		    
		}
	    }
            ctx?.DrawText(text.ToString(), 50, 50, paint);

            skiaCtx?.Flush();
        }

        static void Main(string[] args)
        {
            using (var prg = new SkiaGameWindow())
            {
                try
                {
                    prg.Run();
		    //prg.CenterWindow();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
