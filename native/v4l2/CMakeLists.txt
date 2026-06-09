cmake_minimum_required(VERSION 3.16)
project(vgc_v4l2 C)

set(CMAKE_C_STANDARD 11)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

# Build as shared library for Unity P/Invoke
add_library(vgc_v4l2 SHARED vgc_v4l2.c)

# Strip debug symbols in Release for smaller plugin size
target_compile_options(vgc_v4l2 PRIVATE
    $<$<CONFIG:Release>:-O2 -fvisibility=hidden>
    $<$<CONFIG:Debug>:-g -O0>
)

# Unity expects the plugin at:
# Assets/Plugins/Linux/x86_64/libvgc_v4l2.so
set(UNITY_PLUGIN_OUTPUT_DIR
    "${CMAKE_SOURCE_DIR}/../../../Assets/Plugins/Linux/x86_64"
    CACHE PATH "Unity plugin output directory")

install(TARGETS vgc_v4l2
    LIBRARY DESTINATION "${UNITY_PLUGIN_OUTPUT_DIR}")

# Convenience: 'make install' copies directly into Unity project
add_custom_command(TARGET vgc_v4l2 POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E make_directory "${UNITY_PLUGIN_OUTPUT_DIR}"
    COMMAND ${CMAKE_COMMAND} -E copy
        $<TARGET_FILE:vgc_v4l2>
        "${UNITY_PLUGIN_OUTPUT_DIR}/libvgc_v4l2.so"
    COMMENT "Copying libvgc_v4l2.so to Unity plugin directory"
)
