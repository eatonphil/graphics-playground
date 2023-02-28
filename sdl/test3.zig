const std = @import("std");
const sdl = @cImport(@cInclude("SDL.h"));

pub fn main() !void {
    var win: ?*sdl.SDL_Window = null;
    var ren: ?*sdl.SDL_Renderer = null;

    _ = sdl.SDL_Init(sdl.SDL_INIT_EVERYTHING);
    _ = sdl.SDL_CreateWindowAndRenderer(640, 640, 0, &win, &ren);
    _ = sdl.SDL_SetWindowTitle(win, "Hello world!");

    var e: sdl.SDL_Event = .{ .type = 0 };

    var quit = false;
    while (!quit) {
        var start = sdl.SDL_GetPerformanceCounter();

        // Handle all queued events
        while (sdl.SDL_PollEvent(&e) != 0) {
            if (e.type == sdl.SDL_QUIT) {
                quit = true;
            }
        }

        // Draw background
        _ = sdl.SDL_SetRenderDrawColor(ren, 255, 255, 255, 255);
        _ = sdl.SDL_RenderClear(ren);

        // Draw border
        var border: sdl.SDL_Rect = .{
            .x = 245,
            .y = 145,
            .w = 210,
            .h = 210,
        };
        _ = sdl.SDL_SetRenderDrawColor(ren, 255, 0, 0, 255);
        _ = sdl.SDL_RenderFillRect(ren, &border);

        // Draw stuff
        var rect: sdl.SDL_Rect = .{
            .x = 250,
            .y = 150,
            .w = 200,
            .h = 200,
        };
        _ = sdl.SDL_SetRenderDrawColor(ren, 0, 0, 0, 255);
        _ = sdl.SDL_RenderFillRect(ren, &rect);

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
