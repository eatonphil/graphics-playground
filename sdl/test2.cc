#include <stdlib.h>
#include <SDL.h>


int main(void)
{
  SDL_Window      *win = NULL;
  SDL_Renderer    *ren = NULL;

  SDL_Init(SDL_INIT_EVERYTHING);
  SDL_CreateWindowAndRenderer(320, 640, 0, &win, &ren);

  SDL_SetRenderDrawColor(ren, 0, 0, 0, 255);
  SDL_RenderClear(ren);
  SDL_RenderPresent(ren);
  bool quit = false;

  //Event handler
  SDL_Event e;

  //While application is running
  while( !quit )
    {
      //Handle events on queue
      while( SDL_PollEvent( &e ) != 0 ) // poll for event
	{
	  //User requests quit
	  if( e.type == SDL_QUIT ) // unless player manually quits
	    {
	      quit = true;
	    }
	}
    }

  SDL_DestroyRenderer(ren);
  SDL_DestroyWindow(win);
  SDL_Quit();

  return (0);
}
