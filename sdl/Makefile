CFLAGS= -Wall -Wextra -pedantic \
    `sdl2-config --cflags` \
    `sdl_ttf --cflags`
LIBS=`sdl2-config --libs`

test1: test.cc
	clang++ -o test1 test.cc -I ./SDL/include -L ./SDL/build/build/.libs -l SDL2
	install_name_tool -change /usr/local/lib/libSDL2-2.0.0.dylib @executable_path/SDL/build/build/.libs/libSDL2-2.0.0.dylib test1

test2: test2.cc
	clang++ -o test2 test2.cc -I ./SDL/include -L ./SDL/build/build/.libs -l SDL2
	install_name_tool -change /usr/local/lib/libSDL2-2.0.0.dylib @executable_path/SDL/build/build/.libs/libSDL2-2.0.0.dylib test2

test3: test3.zig
	zig build-exe -I ./SDL/include -L ./SDL/build/build/.libs -l SDL2 test3.zig -headerpad_max_install_names
	install_name_tool -change /usr/local/lib/libSDL2-2.0.0.dylib @executable_path/SDL/build/build/.libs/libSDL2-2.0.0.dylib test3
