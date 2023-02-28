test1: test.cc SDL
	clang++ -o test1 test.cc -I ./SDL/include -L ./SDL/build/build/.libs -l SDL2
	install_name_tool -change /usr/local/lib/libSDL2-2.0.0.dylib @executable_path/SDL/build/build/.libs/libSDL2-2.0.0.dylib test1

test2: test2.cc SDL
	clang++ -o test2 test2.cc -I ./SDL/include -L ./SDL/build/build/.libs -l SDL2
	install_name_tool -change /usr/local/lib/libSDL2-2.0.0.dylib @executable_path/SDL/build/build/.libs/libSDL2-2.0.0.dylib test2

test3: test3.zig SDL
	zig build-exe -I ./SDL/include -L ./SDL/build/build/.libs -l SDL2 test3.zig -headerpad_max_install_names
	install_name_tool -change /usr/local/lib/libSDL2-2.0.0.dylib @executable_path/SDL/build/build/.libs/libSDL2-2.0.0.dylib test3

SDL:
	git clone https://github.com/libsdl-org/SDL -b SDL2
	mkdir SDL/build
	cd SDL/build && ../configure && make -j8
