const std = @import("std");
const sdl = @cImport(@cInclude("SDL.h"));
const sdl_ttf = @cImport(@cInclude("SDL_ttf.h"));

const SCREEN_WIDTH = 800;
const SCREEN_HEIGHT = 600;

const Editor = struct {
    allocator: std.mem.Allocator,
    text: std.ArrayList(u8),

    fn init(allocator: std.mem.Allocator) Editor {
        return .{
            .allocator = allocator,
            .text = std.ArrayList(u8).init(allocator),
        };
    }

    fn deinit(self: Editor) void {
        self.text.deinit();
    }

    fn backspace(self: Editor) void {
        self.text.pop();
    }

    fn append(self: Editor, text: []const u8) !void {
        try self.text.appendSlice(text);
    }

    fn slice(self: Editor) []const u8 {
        return self.text.items;
    }
};

const EditorRenderer = struct {
    editor: *Editor,
    font: *sdl_ttf.TTF_OpenFont,

    fn init(editor: *Editor) EditorRenderer {
        var font = sdl_ttf.TTF_OpenFont("font.ttf", 72);
        if (!font) {
            std.debug.print("Error loading font: {s}", .{sdl_ttf.TTF_GetError()});
            std.os.exit(1);
        }

        return .{
            .font = font,
            .editor = editor,
        };
    }

    fn deinit(self: EditorRenderer) void {
        sdl_ttf.TTF_CloseFont(self.font);
    }

    fn render(self: EditorRenderer, renderer: *sdl.Renderer) void {
        self.renderText(renderer);
    }

    var textColor = sdl.SDL_Color{ 0, 0, 0 };
    fn renderText(self: EditorRenderer, renderer: *sdl.Renderer) void {
        var textSurface = sdl_ttf.TTF_RenderText_Solid(
            self.font,
            self.editor.slice(),
            textColor,
        );
        defer sdl.SDL_FreeSurface(textSurface);

        var texture = sdl.SDL_CreateTextureFromSurface(renderer, textSurface);
        defer sdl.DestroyTexture(texture);

        var dest = sdl.SDL_Rect{
            .x = 320 - (textSurface.w / 2.0),
            .y = 240,
            .w = textSurface.w,
            .h = textSurface.h,
        };
        sdl.SDL_RenderCopy(renderer, texture, 0, &dest);
    }
};

pub fn main() !void {
    defer sdl_ttf.TTF_Quit();
    defer sdl.SDL_Quit();
    var window: ?*sdl.SDL_Window = null;
    var renderer: ?*sdl.SDL_Renderer = null;

    _ = sdl.SDL_Init(sdl.SDL_INIT_EVERYTHING);
    _ = sdl.SDL_CreateWindowAndRenderer(SCREEN_WIDTH, SCREEN_HEIGHT, 0, &window, &renderer);
    defer sdl.SDL_DestroyRenderer(renderer);
    defer sdl.SDL_DestroyWindow(window);

    _ = sdl.SDL_SetWindowTitle(window, "Notepad--");

    var e: sdl.SDL_Event = .{ .type = 0 };

    var arena = std.heap.ArenaAllocator.init(std.heap.page_allocator);
    defer arena.deinit();

    const allocator = arena.allocator();

    var editor = Editor.init(allocator);
    defer editor.deinit();

    var er = EditorRenderer.init(editor);
    defer er.deinit();

    sdl.SDL_StartTextInput();
    defer sdl.SDL_StopTextInput();

    outer: while (true) {
        var start = sdl.SDL_GetPerformanceCounter();

        // Handle all queued events
        while (sdl.SDL_PollEvent(&e) != 0) {
            if (e.type == sdl.SDL_QUIT) {
                break :outer;
            }

            if (e.type == sdl.SDL_KEYDOWN) {
                if (e.key.keysym.sym == sdl.SDLK_BACKSPACE) {
                    editor.backspace();
                } else if (e.key.keysym.sym == sdl.SDLK_v and sdl.SDL_GetModState() & sdl.KMOD_CTRL != 0) {
                    editor.append(sdl.SDL_GetClipboardText());
                }
            }
        }

        er.render(renderer);

        // Done drawing
        _ = sdl.SDL_RenderPresent(renderer);

        // Cap to 60 FPS
        var end = sdl.SDL_GetPerformanceCounter();
        var elapsedMS = (end - start) / (sdl.SDL_GetPerformanceFrequency() * 1000);
        sdl.SDL_Delay(@floatToInt(u32, std.math.floor(16.666 - @intToFloat(f32, elapsedMS))));
    }
}
