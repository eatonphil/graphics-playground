using System.Text;

using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

using SkiaSharp;

namespace Eddy
{
    class EddyWindow : GameWindow
    {
        GRContext? skiaCtx = null;
        SKSurface? skSurface = null;
	int WindowWidth = 1280, WindowHeight = 720;
	Editor editor;

        EddyWindow(Editor editor) : base(GetDefaultDWS(), GetDefaultNWS())
        {
	    this.editor = editor;
        }

        static NativeWindowSettings GetDefaultNWS()
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

        static GameWindowSettings GetDefaultDWS()
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
	    skiaCtx = GRContext.CreateGl();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
	    base.OnResize(e);
            WindowWidth = e.Width;
            WindowHeight = e.Height;
	    skiaCtx.ResetContext(GRBackendState.None);
        }

	protected override void OnTextInput(TextInputEventArgs args) {
	    base.OnTextInput(args);
	    editor.OnTextInput(args.AsString);
	}

	protected override void OnKeyDown(KeyboardKeyEventArgs args) {
	    base.OnKeyDown(args);
	    editor.OnKeyDown(args);
	}

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GRGlFramebufferInfo fbi = new GRGlFramebufferInfo(0, (uint)InternalFormat.Rgba8);
            var ctype = SKColorType.Rgba8888;
            var beTarget = new GRBackendRenderTarget(ClientSize.X, ClientSize.Y, 0, 0, fbi);

            // Dispose Previous Surface
            skSurface?.Dispose();
            skSurface = SKSurface.Create(skiaCtx, beTarget, GRSurfaceOrigin.BottomLeft, ctype, null, null);

            SkiaRender((float)args.Time);
            SwapBuffers();
        }

        protected override void OnUnload()
        {
            skiaCtx?.Dispose();
            skSurface?.Dispose();
            base.OnUnload();
        }

        void SkiaRender(float time)
        {
	    var canvas = skSurface?.Canvas;
	    if (canvas != null) {
		editor.Draw(canvas);
		skiaCtx?.Flush();
	    }
        }

        static void Main(string[] args)
        {
	    var editor = new Editor();
            using (var prg = new EddyWindow(editor))
            {
                    prg.Run();
            }
        }
    }
}
