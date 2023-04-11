using System.Text;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace Eddy {
    class Cursor {
	public int X, Y = 0;
    }

    class Editor {
	List<StringBuilder> lines = new List<StringBuilder>{new StringBuilder()};
	Cursor cursor = new Cursor();
	int textSize = 60;
	SKColor textColor = SKColors.Black;
	SKColor backgroundColor = SKColors.White;

	public void OnTextInput(String text) {
	    var line = lines[cursor.Y];
	    line.Insert(cursor.X, text);
	    cursor.X += text.Length;
	}

	public void OnKeyDown(KeyboardKeyEventArgs e) {
	    if (e.Key == Keys.Enter) {
		var oldLine = lines[cursor.Y].ToString();
		lines.Insert(cursor.Y + 1, new StringBuilder(oldLine.Substring(cursor.X, oldLine.Length - cursor.X)));
		lines[cursor.Y] = new StringBuilder(oldLine.Substring(0, cursor.X));
		cursor.Y++;
		cursor.X = 0;
	    } else if (e.Key == Keys.Backspace) {
		if (cursor.X > 0) {
		    cursor.X -= 1;
		    lines[cursor.Y].Remove(cursor.X, 1);
		} else if (cursor.Y > 0) {
		    cursor.Y -= 1;
		    cursor.X = lines[cursor.Y].Length;
		    lines[cursor.Y].Append(lines[cursor.Y+1].ToString());
		    lines.RemoveAt(cursor.Y+1);
		}
	    } else if (e.Key == Keys.Up) {
		if (cursor.Y > 0) {
		    cursor.Y--;
		}
	    } else if (e.Key == Keys.Down) {
		if (cursor.Y < lines.Count - 1) {
		    cursor.Y++;
		}
	    } else if (e.Key == Keys.Left) {
		if (cursor.X == 0) {
		    if (cursor.Y > 0) {
			cursor.X = lines[cursor.Y - 1].Length;
			cursor.Y--;
		    }
		} else {
		    cursor.X--;
		}
	    } else if (e.Key == Keys.Right) {
		if (cursor.X < lines[cursor.Y].Length) {
		    cursor.X++;
		} else if (cursor.Y < lines.Count - 1) {
		    cursor.Y++;
		    cursor.X = 0;
		}
	    }
	}
	
	public void Draw(SKCanvas canvas) {
            canvas.DrawColor(backgroundColor, SKBlendMode.Src);

            var textPaint = new SKPaint();
            textPaint.TextSize = textSize;
            textPaint.Color = textColor;
            textPaint.Typeface = SKTypeface.FromFile("static/Inter-Regular.ttf", 0);
	    textPaint.TextEncoding = SKTextEncoding.Utf8;
	    textPaint.IsAntialias = true;

	    var i = 0;
	    float cursorWidth = 0;
	    var lineBufferSize = 10;
	    foreach (var line in lines) {
		var s = line.ToString();
		if (i == cursor.Y) {
		    var shaper = new SkiaSharp.HarfBuzz.SKShaper(textPaint.Typeface);
		    cursorWidth = shaper.Shape(s.Substring(0, cursor.X), textPaint).Width;
		}

		i++;
		var lineBuffer = i > 1 ? lineBufferSize : 0;
		CanvasExtensions.DrawShapedText(canvas, s, 25, 25 + textSize * i + lineBuffer, textPaint);
	    }

	    if (DateTime.Now.Millisecond < 500) {
		var x = 25 + cursorWidth;
		var y = 25 + textSize * cursor.Y + cursor.Y * lineBufferSize;
		var cursorRect = SKRect.Create(
		    new SkiaSharp.SKPoint(x, y),
		    new SkiaSharp.SKSize(10, textSize + 10)
		);
		var cursorPaint = new SKPaint{
		    Style = SKPaintStyle.Fill,
		    Color = SKColors.Blue,
		};

		canvas.DrawRect(cursorRect, cursorPaint);
	    }
	}
    }
}
