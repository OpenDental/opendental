//Copyright: Jordan Sparks, 2020

#include "pch.h"
#include "D2dWrapperHwnd.h"

using namespace D2D1;
using namespace Microsoft::WRL;

extern "C" __declspec(dllexport) D2DWrapperHwnd* WrapperHwnd_Create() {
	return new D2DWrapperHwnd();
}

extern "C" __declspec(dllexport) int WrapperHwnd_Initialize(D2DWrapperHwnd* pD2DWrapperHwnd) {
	return pD2DWrapperHwnd->Initialize();
}

extern "C" __declspec(dllexport) bool WrapperHwnd_DeviceResourcesNeedRecreate(D2DWrapperHwnd* pD2DWrapperHwnd) {
	return pD2DWrapperHwnd->DeviceResourcesNeedRecreate();
}

extern "C" __declspec(dllexport) int WrapperHwnd_CreateRenderTarget(D2DWrapperHwnd* pD2DWrapperHwnd,UINT32 width,UINT32 height,HWND hwnd) {
	return pD2DWrapperHwnd->CreateRenderTarget(width,height,hwnd);
}

extern "C" __declspec(dllexport) void WrapperHwnd_CreateGradientBrush(D2DWrapperHwnd* pD2DWrapperHwnd,UINT32 brushNum,UINT32 color1,UINT32 color2,FLOAT x1,FLOAT y1,FLOAT x2,FLOAT y2){
	pD2DWrapperHwnd->CreateGradientBrush(brushNum,color1,color2,x1,y1,x2,y2);
}

extern "C" __declspec(dllexport) void WrapperHwnd_CreateBitmap(D2DWrapperHwnd* pD2DWrapperHwnd,UINT32 bitmapNum,byte bytesBitmap[],UINT32 width,UINT32 height){
	pD2DWrapperHwnd->CreateBitmap(bitmapNum,bytesBitmap,width,height);
}

extern "C" __declspec(dllexport) void WrapperHwnd_CreateBitmapBlank(D2DWrapperHwnd* pD2DWrapperHwnd,UINT32 bitmapNum,UINT32 width,UINT32 height){
	pD2DWrapperHwnd->CreateBitmapBlank(bitmapNum,width,height);
}

extern "C" __declspec(dllexport) void WrapperHwnd_SetTargetToBitmap(D2DWrapperHwnd* pD2DWrapperHwnd,UINT32 bitmapNum){
	pD2DWrapperHwnd->SetTargetToBitmap(bitmapNum);
}

extern "C" __declspec(dllexport) void WrapperHwnd_SetTargetToOriginal(D2DWrapperHwnd* pD2DWrapperHwnd){
	pD2DWrapperHwnd->SetTargetToOriginal();
}

extern "C" __declspec(dllexport) void WrapperHwnd_BeginDraw(D2DWrapperHwnd* pD2DWrapperHwnd) {
	pD2DWrapperHwnd->BeginDraw();
}

extern "C" __declspec(dllexport) int WrapperHwnd_EndDraw(D2DWrapperHwnd* pD2DWrapperHwnd){
	return pD2DWrapperHwnd->EndDraw();
}

extern "C" __declspec(dllexport) void WrapperHwnd_Delete(D2DWrapperHwnd* pD2DWrapperHwnd) {
	delete pD2DWrapperHwnd;//calls destructor
}

//From here down, everything is alphabetical-----------------------------------------------------------------------------
extern "C" __declspec(dllexport) void WrapperHwnd_AddArc(D2DWrapperHwnd* pD2DWrapperHwnd,float x,float y,float width,float height,float rotation,bool isLargeArc,bool isCW){
	pD2DWrapperHwnd->AddArc(x,y,width,height,rotation,isLargeArc,isCW);
}

extern "C" __declspec(dllexport) void WrapperHwnd_AddBezier(D2DWrapperHwnd* pD2DWrapperHwnd,float x1,float y1,float x2,float y2,float x,float y){
	pD2DWrapperHwnd->AddBezier(x1,y1,x2,y2,x,y);
}

extern "C" __declspec(dllexport) void WrapperHwnd_AddLine(D2DWrapperHwnd* pD2DWrapperHwnd,float x,float y){
	pD2DWrapperHwnd->AddLine(x,y);
}

extern "C" __declspec(dllexport) void WrapperHwnd_AddQuadraticBezier(D2DWrapperHwnd* pD2DWrapperHwnd,float x1,float y1,float x,float y){
	pD2DWrapperHwnd->AddQuadraticBezier(x1,y1,x,y);
}

extern "C" __declspec(dllexport) void WrapperHwnd_BeginFigure(D2DWrapperHwnd* pD2DWrapperHwnd,float x,float y,bool isFilled){
	pD2DWrapperHwnd->BeginFigure(x,y,isFilled);
}

extern "C" __declspec(dllexport) void WrapperHwnd_BeginPath(D2DWrapperHwnd* pD2DWrapperHwnd){
	pD2DWrapperHwnd->BeginPath();
}

extern "C" __declspec(dllexport) void WrapperHwnd_Clear(D2DWrapperHwnd* pD2DWrapperHwnd,UINT32 color){
	pD2DWrapperHwnd->Clear(color);
}

extern "C" __declspec(dllexport) void WrapperHwnd_DrawAtlas(D2DWrapperHwnd* pD2DWrapperHwnd,int bitmapNum,int xSource,int ySource,int sizeSource,int x,int y,int size){
	pD2DWrapperHwnd->DrawAtlas(bitmapNum,xSource,ySource,sizeSource,x,y,size);
}

extern "C" __declspec(dllexport) void WrapperHwnd_DrawBitmap(D2DWrapperHwnd* pD2DWrapperHwnd,int bitmapNum,int x,int y,int width,int height){
	pD2DWrapperHwnd->DrawBitmap(bitmapNum,x,y,width,height);
}

extern "C" __declspec(dllexport) void WrapperHwnd_DrawBitmapImmediate(D2DWrapperHwnd* pD2DWrapperHwnd,byte bytesBitmap[],int widthBitmap,int heightBitmap,int x,int y,int width,int height){
	pD2DWrapperHwnd->DrawBitmapImmediate(bytesBitmap,widthBitmap,heightBitmap,x,y,width,height);
}

extern "C" __declspec(dllexport) void WrapperHwnd_DrawEllipse(D2DWrapperHwnd* pD2DWrapperHwnd,UINT32 color,FLOAT cx,FLOAT cy,FLOAT rx,FLOAT ry,FLOAT strokeWidth){
	pD2DWrapperHwnd->DrawEllipse(color,cx,cy,rx,ry,strokeWidth);
}

extern "C" __declspec(dllexport) void WrapperHwnd_DrawLine(D2DWrapperHwnd* pD2DWrapperHwnd,UINT32 color,FLOAT x1,FLOAT y1,FLOAT x2,FLOAT y2,FLOAT strokeWidth){
	pD2DWrapperHwnd->DrawLine(color,x1,y1,x2,y2,strokeWidth);
}

extern "C" __declspec(dllexport) void WrapperHwnd_DrawRectangle(D2DWrapperHwnd* pD2DWrapperHwnd,UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height,FLOAT strokeWidth){
	pD2DWrapperHwnd->DrawRectangle(color,x,y,width,height,strokeWidth);
}

extern "C" __declspec(dllexport) void WrapperHwnd_DrawRoundedRectangle(D2DWrapperHwnd* pD2DWrapperHwnd,UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height,FLOAT radiusX,FLOAT strokeWidth){
	pD2DWrapperHwnd->DrawRoundedRectangle(color,x,y,width,height,radiusX,strokeWidth);
}

extern "C" __declspec(dllexport) void WrapperHwnd_DrawText(D2DWrapperHwnd* pD2DWrapperHwnd,FLOAT x,FLOAT y,FLOAT width,FLOAT height,UINT32 color,FLOAT fontSize,LPCWSTR text){
	pD2DWrapperHwnd->DrawTextMy(x,y,width,height,color,fontSize,text);
}

extern "C" __declspec(dllexport) void WrapperHwnd_EndFigure(D2DWrapperHwnd* pD2DWrapperHwnd,bool isClosed){
	pD2DWrapperHwnd->EndFigure(isClosed);
}

extern "C" __declspec(dllexport) void WrapperHwnd_EndPath(D2DWrapperHwnd* pD2DWrapperHwnd,bool isFilled,bool isOutline,int colorFill,int colorOutline,float strokeWidth){
	pD2DWrapperHwnd->EndPath(isFilled,isOutline,colorFill,colorOutline,strokeWidth);
}

extern "C" __declspec(dllexport) void WrapperHwnd_FillEllipse(D2DWrapperHwnd* pD2DWrapperHwnd,UINT32 color,FLOAT cx,FLOAT cy,FLOAT rx,FLOAT ry){
	pD2DWrapperHwnd->FillEllipse(color,cx,cy,rx,ry);
}

extern "C" __declspec(dllexport) void WrapperHwnd_FillRectangle(D2DWrapperHwnd* pD2DWrapperHwnd,UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height){
	pD2DWrapperHwnd->FillRectangle(color,x,y,width,height);
}

extern "C" __declspec(dllexport) void WrapperHwnd_FillRectangleGradient(D2DWrapperHwnd* pD2DWrapperHwnd,int gradientNum,FLOAT x,FLOAT y,FLOAT width,FLOAT height){
	pD2DWrapperHwnd->FillRectangleGradient(gradientNum,x,y,width,height);
}

extern "C" __declspec(dllexport) void WrapperHwnd_FillRoundedRectangle(D2DWrapperHwnd* pD2DWrapperHwnd,UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height,FLOAT radiusX){
	pD2DWrapperHwnd->FillRoundedRectangle(color,x,y,width,height,radiusX);
}

extern "C" __declspec(dllexport) void WrapperHwnd_RestoreDrawingState(D2DWrapperHwnd* pD2DWrapperHwnd,int levelNum){
	pD2DWrapperHwnd->RestoreDrawingState(levelNum);
}

extern "C" __declspec(dllexport) void WrapperHwnd_Rotate(D2DWrapperHwnd* pD2DWrapperHwnd,float angle){
	pD2DWrapperHwnd->Rotate(angle);
}

extern "C" __declspec(dllexport) void WrapperHwnd_SaveDrawingState(D2DWrapperHwnd* pD2DWrapperHwnd,int levelNum){
	pD2DWrapperHwnd->SaveDrawingState(levelNum);
}

extern "C" __declspec(dllexport) void WrapperHwnd_Scale(D2DWrapperHwnd* pD2DWrapperHwnd,float scale){
	pD2DWrapperHwnd->Scale(scale);
}

extern "C" __declspec(dllexport) void WrapperHwnd_Translate(D2DWrapperHwnd* pD2DWrapperHwnd,float x,float y){
	pD2DWrapperHwnd->Translate(x,y);
}

//static definitions and initialization. 
//These are shared between controls/windows and do last the life of the program.
ComPtr<ID2D1Factory1> D2DWrapperHwnd::m_ID2D1Factory1(NULL);
ComPtr<IDWriteFactory1> D2DWrapperHwnd::m_IDWriteFactory1(NULL);

D2DWrapperHwnd::D2DWrapperHwnd():
	m_ID3D11DeviceContext(NULL),
	m_ID3D11Device(NULL),
	m_ID3D11Device1(NULL),
	m_ID3D11DeviceContext1(NULL),
	m_IDXGIDevice1(NULL),
	m_ID2D1Device(NULL),
	m_ID2D1DeviceContext(NULL),
	m_IDXGISwapChain1(NULL),
	m_ID2D1Bitmap1_Render(NULL),
	m_ID2D1ImageTargetOriginal(NULL),
	//Device independent
	m_ID2D1DrawingStateBlock0(NULL),
	m_ID2D1DrawingStateBlock1(NULL),
	m_ID2D1PathGeometry(NULL),
	m_ID2D1GeometrySink(NULL)
	//The docs say to create device independent resources that last life of the program.
	//For now, the practical interpretation of that is "life of the control"
	//Upon resize of the control, the resources may be set to different values, but will not be released until control is disposed.
	//Device dependent resources, especially the gradient brush need to be recreated if EndDraw returns D2DERR_RECREATE_TARGET
	//Bitmaps:
	//We are creating various bitmaps from m_ID2D1DeviceContext, not from our RT, m_ID2D1Bitmap1_Render.
	//MS docs say on the ID2D1Bitmap page to recreate bitmaps whenever the RT needs to be recreated.
	//But on the ID2D1BitmapRenderTarget page to recreate bitmaps when the device becomes unavailable. Might happen when display resolution changes.
	//m_ID2D1DeviceContext does not get touched when resizing, and I interpret the above to mean we do not need to recreate bitmaps in that case.
	//So, on resize, we will not recreate bitmaps other than m_ID2D1Bitmap1_Render.
	//Testing has shown that this works fine.
{
}

D2DWrapperHwnd::~D2DWrapperHwnd(){
	//Does not release Factories, which can be reused by other wrappers
	//Not sure if we need to add the rest. ComPtr objects are automatically released on dispose.
	//Device Dependent Resources
	m_ID2D1DeviceContext.Reset();
	m_ID2D1LinearGradientBrush0.Reset();
	//Independent
	m_ID2D1DrawingStateBlock0.Reset();
	m_ID2D1DrawingStateBlock1.Reset();
	m_ID2D1PathGeometry.Reset();
	m_ID2D1GeometrySink.Reset();
}

HRESULT D2DWrapperHwnd::Initialize() {
	HRESULT hr=S_OK;
	if(m_ID2D1Factory1.Get()){
		return hr;
	}
	//Direct3D device and context===============================================================
	hr=D3D11CreateDevice(//11.0
		nullptr,//*pAdapter, pass null to use default adapter
		D3D_DRIVER_TYPE_WARP,//DriverType.  Might later give option to use D3D_DRIVER_TYPE_HARDWARE, but some users will always have trouble with that.
		0,//Software, handle to DLL that implements software rasterizer
		//D3D11_CREATE_DEVICE_DEBUG | 
		D3D11_CREATE_DEVICE_BGRA_SUPPORT,//Flags, BGRA required for Direct2D
		NULL,//*pFeatureLevels, pointer to array of feature levels to attempt to create/request.  If NULL, uses 6 reasonable levels from 11 through 9.
		0,//FeatureLevels, number of elements in pFeatureLevels
		D3D11_SDK_VERSION,
		&m_ID3D11Device,//**ppDevice gets returned
		NULL,//*pFeatureLevel gets returned.  Supply NULL if we don't need to determine which feature level is supported.
		&m_ID3D11DeviceContext//**ppImmediateContext gets returned
	);
	if(FAILED(hr)) return hr;
	///Get the 1.1 device and context====================================================================================
	hr=m_ID3D11Device->QueryInterface(__uuidof(ID3D11Device1),(void **)&m_ID3D11Device1);
	if(FAILED(hr)) return hr;
	hr=m_ID3D11DeviceContext->QueryInterface(__uuidof(ID3D11DeviceContext1),(void **)&m_ID3D11DeviceContext1);
	if(FAILED(hr)) return hr;
	//hr=m_ID3D11Device1->QueryInterface(__uuidof(IDXGIDevice1),(void **)&m_IDXGIDevice1);
	hr=m_ID3D11Device1.As(&m_IDXGIDevice1);
	if(FAILED(hr)) return hr;
	//Factories=================================================================================
	D2D1_FACTORY_OPTIONS factory_options={};
	//#ifdef DEBUG
	//	factory_options.debugLevel=D2D1_DEBUG_LEVEL_INFORMATION;
	//#endif
	hr=D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED,factory_options,m_ID2D1Factory1.ReleaseAndGetAddressOf());
	if(FAILED(hr)) return hr;
	hr=DWriteCreateFactory(DWRITE_FACTORY_TYPE_SHARED,__uuidof(m_IDWriteFactory1),reinterpret_cast<IUnknown **>(m_IDWriteFactory1.ReleaseAndGetAddressOf()));
	if(FAILED(hr)) return hr;
	//2D Device and Context=====================================================================
	hr=m_ID2D1Factory1->CreateDevice(m_IDXGIDevice1.Get(),&m_ID2D1Device);
	if(FAILED(hr)) return hr;
	hr=m_ID2D1Device->CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS_NONE,&m_ID2D1DeviceContext);
	if(FAILED(hr)) return hr;
	//Drawing State Blocks======================================================================
	hr=m_ID2D1Factory1->CreateDrawingStateBlock(&m_ID2D1DrawingStateBlock0);
	if(FAILED(hr)) return hr;
	hr=m_ID2D1Factory1->CreateDrawingStateBlock(&m_ID2D1DrawingStateBlock1);
	if(FAILED(hr)) return hr;
	
	return hr;
}

///<summary>Only tests the render target, but that test determines whether all the other device resources need to be recreated as well.</summary>
BOOL D2DWrapperHwnd::DeviceResourcesNeedRecreate(){
	//todo: test to see if the device becomes unavailable.
	//This would require running Initialize again, 
	//"Drawing with Direct2D" page hints that this may be what D2DERR_RECREATE_TARGET is asking us to do.
	if(m_ID2D1Bitmap1_Render.Get()){
		return false;
	}
	return true;
}

///<summary>Only gets called if DeviceResourcesNeedRecreate is true.</summary>
HRESULT D2DWrapperHwnd::CreateRenderTarget(UINT32 width,UINT32 height,HWND hwnd){
	m_IDXGISwapChain1.Reset();
	m_ID2D1Bitmap1_Render.Reset();
	HRESULT hr=S_OK;
	///DXGI Adapter=================================================================================================
	ComPtr<IDXGIAdapter> pIDXGIAdapter;
	hr=m_IDXGIDevice1->GetAdapter(&pIDXGIAdapter);
	if(FAILED(hr)) return hr;
	hr=m_IDXGIDevice1->SetMaximumFrameLatency(1);// Ensure that DXGI doesn't queue more than one frame at a time.
	if(FAILED(hr)) return hr;
	//DXGI Factory2====================================================================================================
	//Warning.  This might behave differently in Windows 7. 
	//Especially, the D3D_DRIVER_TYPE_WARP that is used does not support 11.1
	//https://docs.microsoft.com/en-us/windows/win32/direct3darticles/platform-update-for-windows-7?redirectedfrom=MSDN
	//But WARP is supported in Windows 8 and Server 2012
	//https://docs.microsoft.com/en-us/windows/win32/direct3darticles/directx-warp
	ComPtr<IDXGIFactory2> pIDXGIFactory2;
	hr=pIDXGIAdapter->GetParent(IID_PPV_ARGS(&pIDXGIFactory2));
	if(FAILED(hr)) return hr;
	pIDXGIAdapter.Reset();
	//SwapChain=========================================================================================================
	DXGI_SWAP_CHAIN_DESC1 swapChainDesc ={0};
	swapChainDesc.Width = 0;//auto
	swapChainDesc.Height = 0;//auto
	swapChainDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
	swapChainDesc.Stereo = false;
	swapChainDesc.SampleDesc.Count = 8;//maybe _very_ slightly better than 4. Edges of crowns will need different strategy.
	swapChainDesc.SampleDesc.Quality = D3D11_STANDARD_MULTISAMPLE_PATTERN;//defined by hardware vendors?
	swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
	swapChainDesc.BufferCount = 1;//Windowed uses only 1 extra buffer as render target, with the front buffer simply being the screen.
	swapChainDesc.Scaling = DXGI_SCALING_STRETCH;
	swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;//DXGI_SWAP_EFFECT enum. This is the only 1 of 4 that can be used with multisampling.
	swapChainDesc.Flags = 0;
	hr=pIDXGIFactory2->CreateSwapChainForHwnd(//dxgi 1.2
		m_ID3D11Device.Get(),//*pDevice
		hwnd,//
		&swapChainDesc,
		nullptr, //*pFullscreenDesc null for windowed
		nullptr, //*pRestrictToOutput or null
		&m_IDXGISwapChain1);
	if(FAILED(hr)) return hr;
	pIDXGIFactory2.Reset();
	//DXGI Surface=====================================================================================================
	//Get the back buffer as an IDXGISurface (Direct2D doesn't accept an ID3D11Texture2D directly as a render target)
	ComPtr<IDXGISurface> pIDXGISurface;
	hr=m_IDXGISwapChain1->GetBuffer(0,IID_PPV_ARGS(&pIDXGISurface));
	if(FAILED(hr)) return hr;
	if(!pIDXGISurface) return E_FAIL;
	///Direct2D Bitmap as render target===============================================================================
		//todo: What does this do on a 4K monitor
		///UINT dpi=GetDpiForWindow(hWnd);
	D2D1_BITMAP_PROPERTIES1 bitmapProperties=BitmapProperties1(
		D2D1_BITMAP_OPTIONS_TARGET | D2D1_BITMAP_OPTIONS_CANNOT_DRAW,
		PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM,D2D1_ALPHA_MODE_IGNORE),
		96.0f,96.0f);
	hr=m_ID2D1DeviceContext->CreateBitmapFromDxgiSurface(pIDXGISurface.Get(),&bitmapProperties,&m_ID2D1Bitmap1_Render);
	if(FAILED(hr)) return hr;
	pIDXGISurface.Reset();
	m_ID2D1DeviceContext->SetTarget(m_ID2D1Bitmap1_Render.Get());
	/*
	D2D1_SIZE_U size = D2D1::SizeU(width,height);
	hr=m_ID2D1Factory1->CreateHwndRenderTarget(
		D2D1::RenderTargetProperties(),
		D2D1::HwndRenderTargetProperties(hwnd,size),
		m_ID2D1HwndRenderTarget.ReleaseAndGetAddressOf());
	if(FAILED(hr)) return hr;
	m_ID2D1HwndRenderTarget->SetAntialiasMode(D2D1_ANTIALIAS_MODE_PER_PRIMITIVE);//already default
	//D2D1_DRAWING_STATE_DESCRIPTION drawingState=DrawingStateDescription();
	m_ID2D1HwndRenderTarget->SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE_CLEARTYPE);//only works for screen, obviously. Already default, and it is working.
	*/
	return hr;
}

///<summary>Only gets called if DeviceResourcesNeedRecreate is true. brushNum is 0 index item in a list of brushes.</summary>
void D2DWrapperHwnd::CreateGradientBrush(UINT32 brushNum,UINT32 color1,UINT32 color2,FLOAT x1,FLOAT y1,FLOAT x2,FLOAT y2){
	ComPtr<ID2D1GradientStopCollection> pGradientStops;
	D2D1_GRADIENT_STOP gradientStops[2];
	gradientStops[0].color=ColorF(color1);
	gradientStops[0].position=0.0f;
	gradientStops[1].color=ColorF(color2);
	gradientStops[1].position=1.0f;
	m_ID2D1DeviceContext->CreateGradientStopCollection(
		gradientStops,
		2,
		D2D1_GAMMA_2_2,
		D2D1_EXTEND_MODE_CLAMP,
		pGradientStops.GetAddressOf()
	);
	m_ID2D1DeviceContext->CreateLinearGradientBrush(
		D2D1::LinearGradientBrushProperties(D2D1::Point2F(x1,y1),D2D1::Point2F(x2,y2)),
		pGradientStops.Get(),
		m_ID2D1LinearGradientBrush0.ReleaseAndGetAddressOf()
	);
	pGradientStops.Reset();
}

///<summary>Create a D2D bitmap from an existing bitmap.  bitmapNum is 0 index item in a list of bitmaps.</summary>
void D2DWrapperHwnd::CreateBitmap(int bitmapNum,byte bytesBitmap[],int width,int height){
	D2D1_BITMAP_PROPERTIES1 d2D1_BITMAP_PROPERTIES1=D2D1::BitmapProperties1();
	d2D1_BITMAP_PROPERTIES1.pixelFormat=PixelFormat(
		DXGI_FORMAT_B8G8R8A8_UNORM,//this is backward from the format in C#
		D2D1_ALPHA_MODE_PREMULTIPLIED);//D2D1_ALPHA_MODE_STRAIGHT crashes
	//d2D1_BITMAP_PROPERTIES.dpiX and Y are already default 96 dpi
	//D2D1_SIZE_U size = D2D1::SizeU(width,height);
	m_ID2D1DeviceContext->CreateBitmap(SizeU(width,height),
		bytesBitmap,//srcData const void* pointer to memory location of image data
		width*4,//pitch=width*bytes/pixel
		&d2D1_BITMAP_PROPERTIES1,//bitmapProperties
		m_ID2D1Bitmap1_0.ReleaseAndGetAddressOf());//bitmap
}

///<summary>Create a bitmap on which to draw.  bitmapNum is 0 index item in a list of bitmaps.</summary>
void D2DWrapperHwnd::CreateBitmapBlank(int bitmapNum,int width,int height){
	D2D1_BITMAP_PROPERTIES1 d2D1_BITMAP_PROPERTIES1=D2D1::BitmapProperties1(
		D2D1_BITMAP_OPTIONS_TARGET,
		D2D1::PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM,//this is backward from the format in C#
			D2D1_ALPHA_MODE_PREMULTIPLIED),//D2D1_ALPHA_MODE_STRAIGHT crashes
		96,96);
	m_ID2D1DeviceContext->CreateBitmap(SizeU(width,height),
		nullptr,//srcData
		0,//pitch
		&d2D1_BITMAP_PROPERTIES1,//bitmapProperties
		m_ID2D1Bitmap1_0.ReleaseAndGetAddressOf());//bitmap
}

void D2DWrapperHwnd::SetTargetToBitmap(int bitmapNum){
	//The intent of this is to draw to a small bitmap temporarily.  Don't use it too much, or UI will flicker.
	//ID2D1BitmapRenderTarget would be the way to do it if we needed something more intensive.
	m_ID2D1DeviceContext->GetTarget(&m_ID2D1ImageTargetOriginal);//preserve the existing target
	m_ID2D1DeviceContext->SetTarget(m_ID2D1Bitmap1_0.Get());
}

void D2DWrapperHwnd::SetTargetToOriginal(){
	m_ID2D1DeviceContext->SetTarget(m_ID2D1ImageTargetOriginal.Get());
}

void D2DWrapperHwnd::BeginDraw(){
	m_ID2D1DeviceContext->BeginDraw();
}

HRESULT D2DWrapperHwnd::EndDraw(){
	HRESULT hr=S_OK;
	hr=m_ID2D1DeviceContext->EndDraw();	
	if(hr==D2DERR_RECREATE_TARGET){
		hr=S_OK;
		m_ID2D1Bitmap1_Render.Reset();
		return hr;
	}
	if(FAILED(hr)) return hr;
	DXGI_PRESENT_PARAMETERS parameters = { 0 };
	parameters.DirtyRectsCount = 0;
	parameters.pDirtyRects = nullptr;
	parameters.pScrollRect = nullptr;
	parameters.pScrollOffset = nullptr;
	hr=m_IDXGISwapChain1->Present1(1, 0, &parameters);
	return hr;
}

//from here down, everything is alphabetical---------------------------------------------------------------------------------------------------
void D2DWrapperHwnd::AddArc(float x,float y,float width,float height,float rotation,bool isLargeArc,bool isCW){
	D2D1_ARC_SEGMENT arc;
	arc.point.x = x;//end point
	arc.point.y = y;
	arc.size.width = width;//radius
	arc.size.height = height;
	arc.rotationAngle = rotation;
	if(isLargeArc){
		arc.arcSize=D2D1_ARC_SIZE_LARGE;
	}
	else{
		arc.arcSize=D2D1_ARC_SIZE_SMALL;
	}
	if(isCW){
		arc.sweepDirection=D2D1_SWEEP_DIRECTION_CLOCKWISE;
	}
	else{
		arc.sweepDirection=D2D1_SWEEP_DIRECTION_COUNTER_CLOCKWISE;
	}
	m_ID2D1GeometrySink->AddArc(&arc);
}

void D2DWrapperHwnd::AddBezier(float x1,float y1,float x2,float y2,float x,float y){
	D2D1_BEZIER_SEGMENT bezier;
	bezier.point1.x = x1;//control 1
	bezier.point1.y = y1;
	bezier.point2.x = x2;//control 2
	bezier.point2.y = y2;
	bezier.point3.x = x;//end point
	bezier.point3.y = y;
	m_ID2D1GeometrySink->AddBezier(&bezier);
}

void D2DWrapperHwnd::AddLine(float x,float y){
	m_ID2D1GeometrySink->AddLine(Point2F(x,y));
}

void D2DWrapperHwnd::AddQuadraticBezier(float x1,float y1,float x,float y){
	D2D1_QUADRATIC_BEZIER_SEGMENT bezier;
	bezier.point1.x = x1;//control 1
	bezier.point1.y = y1;
	bezier.point2.x = x;//endpoint
	bezier.point2.y = y;
	m_ID2D1GeometrySink->AddQuadraticBezier(&bezier);
}

void D2DWrapperHwnd::BeginFigure(float x,float y,bool isFilled) {
	if(isFilled){
		m_ID2D1GeometrySink->BeginFigure({x,y},D2D1_FIGURE_BEGIN_FILLED);
	}
	else{
		m_ID2D1GeometrySink->BeginFigure({x,y},D2D1_FIGURE_BEGIN_HOLLOW);
	}
}

void D2DWrapperHwnd::BeginPath(){
	m_ID2D1Factory1->CreatePathGeometry(m_ID2D1PathGeometry.ReleaseAndGetAddressOf());
	m_ID2D1PathGeometry->Open(m_ID2D1GeometrySink.ReleaseAndGetAddressOf());
	m_ID2D1GeometrySink->SetFillMode(D2D1_FILL_MODE_WINDING);
}

void D2DWrapperHwnd::Clear(UINT32 color){
	m_ID2D1DeviceContext->Clear(ColorF(color));
	//m_ID2D1HwndRenderTarget->Clear(ColorF(color));
}

void D2DWrapperHwnd::DrawEllipse(UINT32 color,FLOAT cx,FLOAT cy,FLOAT rx,FLOAT ry,FLOAT strokeWidth){
	m_ID2D1DeviceContext->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	m_ID2D1DeviceContext->DrawEllipse(Ellipse(Point2F(cx,cy),rx,ry),m_brush.Get(),strokeWidth);
}

void D2DWrapperHwnd::DrawAtlas(int bitmapNum,int xSource,int ySource,int sizeSource,int x,int y,int size){
	ComPtr<ID2D1Effect> pID2D1EffectAtlas;
	m_ID2D1DeviceContext->CreateEffect(CLSID_D2D1Atlas, &pID2D1EffectAtlas);
	if(bitmapNum==0){
		pID2D1EffectAtlas->SetInput(0, m_ID2D1Bitmap1_0.Get());
		D2D1_RECT_F rectSource = D2D1::RectF(xSource,ySource,xSource+sizeSource,ySource+sizeSource);
		pID2D1EffectAtlas->SetValue(D2D1_ATLAS_PROP_INPUT_RECT,rectSource);

		m_ID2D1DeviceContext->DrawImage(pID2D1EffectAtlas.Get(),Point2F(x,y));

		//m_ID2D1DeviceContext->DrawBitmap(m_ID2D1Bitmap_0.Get(),RectF(x,y,x+width,y+height));
	}
	//more later
}

void D2DWrapperHwnd::DrawBitmap(int bitmapNum,int x,int y,int width,int height){
	if(bitmapNum==0){
		m_ID2D1DeviceContext->DrawBitmap(m_ID2D1Bitmap1_0.Get(),RectF(x,y,x+width,y+height));
	}
	//more later
}

void D2DWrapperHwnd::DrawBitmapImmediate(byte bytesBitmap[],int widthBitmap,int heightBitmap,int x,int y,int width,int height){
	ComPtr<ID2D1Bitmap1> pID2D1Bitmap1;
	D2D1_BITMAP_PROPERTIES1 d2D1_BITMAP_PROPERTIES=D2D1::BitmapProperties1();
	d2D1_BITMAP_PROPERTIES.pixelFormat=PixelFormat(
		DXGI_FORMAT_B8G8R8A8_UNORM,
		D2D1_ALPHA_MODE_PREMULTIPLIED);
	m_ID2D1DeviceContext->CreateBitmap(SizeU(widthBitmap,heightBitmap),
		bytesBitmap,//srcData const void* pointer to memory location of image data
		widthBitmap*4,//pitch=width*bytes/pixel
		&d2D1_BITMAP_PROPERTIES,//bitmapProperties
		pID2D1Bitmap1.ReleaseAndGetAddressOf());//bitmap
	m_ID2D1DeviceContext->DrawBitmap(pID2D1Bitmap1.Get(),RectF(x,y,x+width,y+height));
	pID2D1Bitmap1.Reset();
}

void D2DWrapperHwnd::DrawLine(UINT32 color,FLOAT x1,FLOAT y1,FLOAT x2,FLOAT y2,FLOAT strokeWidth){
	m_ID2D1DeviceContext->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	m_ID2D1DeviceContext->DrawLine(Point2F(x1,y1),Point2F(x2,y2),m_brush.Get(),strokeWidth);
}

void D2DWrapperHwnd::DrawRectangle(UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height,FLOAT strokeWidth){
	m_ID2D1DeviceContext->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	m_ID2D1DeviceContext->DrawRectangle(RectF(x,y,x+width,y+height),m_brush.Get(),strokeWidth);
}

void D2DWrapperHwnd::DrawRoundedRectangle(UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height,FLOAT radiusX,FLOAT strokeWidth) {
	m_ID2D1DeviceContext->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	D2D1_ROUNDED_RECT roundedRect;
	roundedRect.rect.top=y;
	roundedRect.rect.left=x;
	roundedRect.rect.right=x+width;
	roundedRect.rect.bottom=y+height;
	roundedRect.radiusX=radiusX;
	roundedRect.radiusY=radiusX;
	m_ID2D1DeviceContext->DrawRoundedRectangle(roundedRect,m_brush.Get(),strokeWidth);
}

void D2DWrapperHwnd::DrawTextMy(FLOAT x,FLOAT y,FLOAT width,FLOAT height,UINT32 color,FLOAT fontSize,LPCWSTR text){//DrawText is reserved word
	m_IDWriteFactory1->CreateTextFormat(
		L"Microsoft Sans Serif",//*fontFamilyName
		NULL,//*fontCollection
		DWRITE_FONT_WEIGHT_REGULAR,
		DWRITE_FONT_STYLE_NORMAL,
		DWRITE_FONT_STRETCH_NORMAL,
		fontSize,//12.0f,//fontSize in DIP (1/96")
		L"en-us",//*localeName
		m_IDWriteTextFormat.ReleaseAndGetAddressOf()//**textFormat IDWriteTextFormat
	);
	m_ID2D1DeviceContext->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	m_ID2D1DeviceContext->DrawTextW(text,wcslen(text),m_IDWriteTextFormat.Get(),RectF(x,y,x+width,y+width),m_brush.Get());
	//pIDWriteTextFormat->Release();//can't do this here because it crashes, so I made it class level
}

void D2DWrapperHwnd::EndFigure(bool isClosed) {
	if(isClosed){
		m_ID2D1GeometrySink->EndFigure(D2D1_FIGURE_END_CLOSED);
	}
	else{
		m_ID2D1GeometrySink->EndFigure(D2D1_FIGURE_END_OPEN);
	}
}

void D2DWrapperHwnd::EndPath(bool isFilled,bool isOutline,int colorFill,int colorOutline,float strokeWidth){
	m_ID2D1GeometrySink->Close();
	m_ID2D1GeometrySink.Reset();
	if(isFilled){
		m_ID2D1DeviceContext->CreateSolidColorBrush(ColorF(colorFill),m_brush.ReleaseAndGetAddressOf());
		m_ID2D1DeviceContext->FillGeometry(m_ID2D1PathGeometry.Get(),m_brush.Get());
	}
	if(isOutline){
		m_ID2D1DeviceContext->CreateSolidColorBrush(ColorF(colorOutline),m_brush.ReleaseAndGetAddressOf());
		//this was an attempt to support rounded ends and connections.  I couldn't get it to work quickly, and it's low priority, so revisit later:
		//ComPtr<ID2D1StrokeStyle>  pID2D1StrokeStyle;
		//float dashes[] ={1.0f,1.0f};
		//m_ID2D1Factory1->CreateStrokeStyle(StrokeStyleProperties(D2D1_CAP_STYLE_ROUND, D2D1_CAP_STYLE_ROUND, D2D1_CAP_STYLE_FLAT, D2D1_LINE_JOIN_ROUND,10.0f,D2D1_DASH_STYLE_SOLID,0.0f),dashes,ARRAYSIZE(dashes),pID2D1StrokeStyle.GetAddressOf());//,10.0f,D2D1_DASH_STYLE_SOLID,0.0f,
		m_ID2D1DeviceContext->DrawGeometry(m_ID2D1PathGeometry.Get(),m_brush.Get(),strokeWidth);//,pID2D1StrokeStyle.Get());
		//pID2D1StrokeStyle.Reset();
	}
}

void D2DWrapperHwnd::FillEllipse(UINT32 color,FLOAT cx,FLOAT cy,FLOAT rx,FLOAT ry){
	m_ID2D1DeviceContext->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	m_ID2D1DeviceContext->FillEllipse(Ellipse(Point2F(cx,cy),rx,ry),m_brush.Get());
}

void D2DWrapperHwnd::FillRectangle(UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height){
	m_ID2D1DeviceContext->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	m_ID2D1DeviceContext->FillRectangle(RectF(x,y,x+width,y+height),m_brush.Get());
}

void D2DWrapperHwnd::FillRectangleGradient(int gradientNum,FLOAT x,FLOAT y,FLOAT width,FLOAT height){
	if(gradientNum==0){
		m_ID2D1DeviceContext->FillRectangle(RectF(x,y,x+width,y+height),m_ID2D1LinearGradientBrush0.Get());
	}
	//others are not yet supported
}

void D2DWrapperHwnd::FillRoundedRectangle(UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height,FLOAT radiusX) {
	m_ID2D1DeviceContext->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	D2D1_ROUNDED_RECT roundedRect;
	roundedRect.rect.top=y;
	roundedRect.rect.left=x;
	roundedRect.rect.right=x+width;
	roundedRect.rect.bottom=y+height;
	roundedRect.radiusX=radiusX;
	roundedRect.radiusY=radiusX;
	m_ID2D1DeviceContext->FillRoundedRectangle(roundedRect,m_brush.Get());
}

void D2DWrapperHwnd::RestoreDrawingState(int levelNum){
	if(levelNum==0){
		m_ID2D1DeviceContext->RestoreDrawingState(m_ID2D1DrawingStateBlock0.Get());
	}
	if(levelNum==1){
		m_ID2D1DeviceContext->RestoreDrawingState(m_ID2D1DrawingStateBlock1.Get());
	}
}

void D2DWrapperHwnd::Rotate(float angle){
	D2D1_MATRIX_3X2_F matrixOld;
	m_ID2D1DeviceContext->GetTransform(&matrixOld);
	D2D1_MATRIX_3X2_F matrixRotate=D2D1::Matrix3x2F::Rotation(angle);
	m_ID2D1DeviceContext->SetTransform(matrixRotate * matrixOld);
}

void D2DWrapperHwnd::SaveDrawingState(int levelNum){
	if(levelNum==0){
		m_ID2D1DeviceContext->SaveDrawingState(m_ID2D1DrawingStateBlock0.Get());
	}
	if(levelNum==1){
		m_ID2D1DeviceContext->SaveDrawingState(m_ID2D1DrawingStateBlock1.Get());
	}
}

void D2DWrapperHwnd::Scale(float scale){
	D2D1_MATRIX_3X2_F matrixOld;
	m_ID2D1DeviceContext->GetTransform(&matrixOld);
	D2D1_MATRIX_3X2_F matrixScale=D2D1::Matrix3x2F::Scale(D2D1::Size(scale,scale),D2D1::Point2F(0,0));
	m_ID2D1DeviceContext->SetTransform(matrixScale * matrixOld);
}

void D2DWrapperHwnd::Translate(float x,float y){
	D2D1_MATRIX_3X2_F matrixOld;
	m_ID2D1DeviceContext->GetTransform(&matrixOld);
	D2D1_MATRIX_3X2_F matrixTranslation =D2D1::Matrix3x2F::Translation(x,y);
	m_ID2D1DeviceContext->SetTransform(matrixTranslation * matrixOld);
}