// pch.h: This is a precompiled header file.
// Files listed below are compiled only once, improving build performance for future builds.
// This also affects IntelliSense performance, including code completion and many code browsing features.
// However, files listed here are ALL re-compiled if any one of them is updated between builds.
// Do not add files here that you will be updating frequently as this negates the performance advantage.

#ifndef PCH_H
#define PCH_H

// add headers that you want to pre-compile here
#include "framework.h"
#include <wrl/client.h>
//#include <d2d1.h>//1.1 is backward compatible
#include <d2d1_1.h>
#include <d2d1_1helper.h>
#include <dwrite_1.h>
#include <d3d11.h>
#include <d3d11_1.h>
#include <dxgi1_2.h>
#include <dcommon.h>
#include <d2d1effects.h>//added dxguid.lib
//also added d2d1.lib, dwrite.lib, D3D11.lib to project props linker

#endif //PCH_H
