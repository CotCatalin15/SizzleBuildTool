cmake_minimum_required(VERSION 3.12)
add_library(Renderer SHARED test.h MiniRenderer\test.h test.h MiniRenderer\test.h )
set_target_properties(Renderer PROPERTIES LINKER_LANGUAGE CXX)
