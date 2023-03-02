const std = @import("std");
const sdl = @cImport(@cInclude("SDL.h"));

const SCREEN_WIDTH = 800;
const SCREEN_HEIGHT = 600;

const Editor = struct {
    allocator: std.mem.Allocator,
    text: std.ArrayList(u8),

    fn init(allocator: std.mem.Allocator): Editor {
        return .{
            .allocator: allocator,
            .text: std.ArrayList(u8).init(allocator),
        };
    }

    fn backspace(self: Editor) void {
        self.text.pop();
    }

    fn append(self: Editor, slice: []const u8) !void {
        
    }
}

const textColor = sdl.SDL_Color{ 255, 255, 255, 0 };

fn drawEditor(ren: *sdl.SDL_Renderer, editor: Editor) {
    SDL_Surface* textSurface = TTF_RenderText_Solid(font, score_text.c_str(), textColor);
    SDL_Texture* text = SDL_CreateTextureFromSurface(renderer, textSurface);
    int text_width = textSurface->w;
    int text_height = textSurface->h;
    SDL_FreeSurface(textSurface);
    SDL_Rect renderQuad = { 20, win_height - 30, text_width, text_height };
    SDL_RenderCopy(renderer, text, NULL, &renderQuad);
    SDL_DestroyTexture(text);
}

pub fn main() !void {
    var win: ?*sdl.SDL_Window = null;
    var ren: ?*sdl.SDL_Renderer = null;

    _ = sdl.SDL_Init(sdl.SDL_INIT_EVERYTHING);
    _ = sdl.SDL_CreateWindowAndRenderer(SCREEN_WIDTH, SCREEN_HEIGHT, 0, &win, &ren);
    _ = sdl.SDL_SetWindowTitle(win, "Notepad--");

    var e: sdl.SDL_Event = .{ .type = 0 };

    var arena = std.heap.ArenaAllocator.init(std.heap.page_allocator);
    defer arena.deinit();

    const allocator = arena.allocator();
    var editor = Editor.init(allocator);

    sdl.SDL_StartTextInput();
    defer sdl.SDL_StopTextInput();

    outer: while (true) {
        var start = sdl.SDL_GetPerformanceCounter();

        // Handle all queued events
        while (sdl.SDL_PollEvent(&e) != 0) {
            if (e.type == sdl.SDL_QUIT) {
                break :outer;
            }

            if( e.type == SDL_KEYDOWN ) {
                if (e.key.keysym.sym == sdl.SDLK_BACKSPACE) {
                    editor.backspace();
                } else if (e.key.keysym.sym == sdl.SDLK_v && sdl.SDL_GetModState() & sdl.KMOD_CTRL != 0) {
                    editor.append(sdl.SDL_GetClipboardText());
                }
            }
        }

        drawEditor(ren, editor);

        // Done drawing
        _ = sdl.SDL_RenderPresent(ren);

        // Cap to 60 FPS
        var end = sdl.SDL_GetPerformanceCounter();
        var elapsedMS = (end - start) / (sdl.SDL_GetPerformanceFrequency() * 1000);
        sdl.SDL_Delay(@floatToInt(u32, std.math.floor(16.666 - @intToFloat(f32, elapsedMS))));
    }

    sdl.SDL_DestroyRenderer(ren);
    sdl.SDL_DestroyWindow(win);
    sdl.SDL_Quit();
}
