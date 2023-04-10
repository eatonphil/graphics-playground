using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using SkiaSharp;

namespace Skia_OpenTK
{
    public class Window : GameWindow
    {
	GRGlInterface grgInterface;
	GRContext grContext;
	SKSurface surface;
	SKCanvas canvas;
        GRBackendRenderTarget renderTarget;
        SKPaint TestBrush;

	int Width, Height;

        public Window(string title, int width, int height) : base(new GameWindowSettings {
            IsMultiThreaded = false,
            RenderFrequency = 60.0
        },
        new NativeWindowSettings {
            Title = title,
            WindowBorder = WindowBorder.Resizable,
            Size = new Vector2i(width, height)
        })
        {
            VSync = VSyncMode.Off;
	    Width = width;
	    Height = height;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            //Context.MakeCurrent();
            grgInterface = GRGlInterface.Create();
	    grContext = GRContext.CreateGl(grgInterface);

            TestBrush = new SKPaint
	    {
		Color = SKColors.White,
		IsAntialias = true,
		Style = SKPaintStyle.Fill,
		TextAlign = SKTextAlign.Center,
		TextSize = 24
	    };

        }

        protected override void OnUnload()
        {
            TestBrush.Dispose();
            surface.Dispose();
            renderTarget.Dispose();
            grContext.Dispose();
            grgInterface.Dispose();
            base.OnUnload();
        }

	protected override void OnResize(ResizeEventArgs e) {
	    Width = e.Width;
	    Height = e.Height;
	    grContext.ResetContext(GRBackendState.None);
	}

        double time = 0;
        protected override void OnRenderFrame(FrameEventArgs args)
        {
	    renderTarget = new GRBackendRenderTarget(Width, Height, 0, 8, new GRGlFramebufferInfo(0, (uint)SizedInternalFormat.Rgba8));
            surface = SKSurface.Create(grContext, renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
            canvas = surface.Canvas;

            time += args.Time;
            canvas.Clear(SKColors.CornflowerBlue);

            TestBrush.Color = SKColors.White;
            canvas.DrawRoundRect(new SKRoundRect(new SKRect(0, 0, 256, 256), (float)Math.Max(Math.Sin(-time) * 128.0f, 0)), TestBrush);

            TestBrush.Color = SKColors.Black;
            canvas.DrawText("Hello, World!", 128, 300, TestBrush);

            canvas.Flush();
            SwapBuffers();
        }
    }
}
