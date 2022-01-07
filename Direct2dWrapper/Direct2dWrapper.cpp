//Copyright: Jordan Sparks, 2020-2021

#include "pch.h"
#include "Direct2dWrapper.h"

using namespace D2D1;
using namespace Microsoft::WRL;
//using Microsoft::WRL::ComPtr;

extern "C" __declspec(dllexport) Direct2DWrapper* Wrapper_Create() { 
	return new Direct2DWrapper(); 
}

extern "C" __declspec(dllexport) int Wrapper_Initialize(Direct2DWrapper* pDirect2DWrapper) { 
	return pDirect2DWrapper->Initialize(); 
}

extern "C" __declspec(dllexport) int Wrapper_BeginDraw(Direct2DWrapper* pDirect2DWrapper,UINT32 width,UINT32 height,HDC hdc) { 
	return pDirect2DWrapper->BeginDraw(width,height,hdc); 
}

extern "C" __declspec(dllexport) void Wrapper_CreateGradientBrush(Direct2DWrapper * pDirect2DWrapper, UINT32 brushNum, UINT32 color1, UINT32 color2, FLOAT x1, FLOAT y1, FLOAT x2, FLOAT y2) {
	pDirect2DWrapper->CreateGradientBrush(brushNum, color1, color2, x1, y1, x2, y2);
}

extern "C" __declspec(dllexport) int Wrapper_EndDraw(Direct2DWrapper* pDirect2DWrapper){ 
	return pDirect2DWrapper->EndDraw(); 
}

extern "C" __declspec(dllexport) void Wrapper_ReleaseDeviceResources(Direct2DWrapper* pDirect2DWrapper){ 
	return pDirect2DWrapper->ReleaseDeviceResources(); 
}

extern "C" __declspec(dllexport) void Wrapper_Delete(Direct2DWrapper* pDirect2DWrapper) { 
	delete pDirect2DWrapper; //calls destructor
}

//From here down, everything is alphabetical-----------------------------------------------------------------------------
extern "C" __declspec(dllexport) void Wrapper_AddArc(Direct2DWrapper* pDirect2DWrapper,float x,float y,float width,float height,float rotation,bool isLargeArc,bool isCW){
	pDirect2DWrapper->AddArc(x,y,width,height,rotation,isLargeArc,isCW);
}

extern "C" __declspec(dllexport) void Wrapper_AddBezier(Direct2DWrapper* pDirect2DWrapper,float x1,float y1,float x2,float y2,float x,float y){
	pDirect2DWrapper->AddBezier(x1,y1,x2,y2,x,y);
}

extern "C" __declspec(dllexport) void Wrapper_AddLine(Direct2DWrapper* pDirect2DWrapper,float x,float y){
	pDirect2DWrapper->AddLine(x,y);
}

extern "C" __declspec(dllexport) void Wrapper_AddQuadraticBezier(Direct2DWrapper* pDirect2DWrapper,float x1,float y1,float x,float y){
	pDirect2DWrapper->AddQuadraticBezier(x1,y1,x,y);
}

extern "C" __declspec(dllexport) void Wrapper_BeginFigure(Direct2DWrapper* pDirect2DWrapper,float x,float y,bool isFilled){
	pDirect2DWrapper->BeginFigure(x,y,isFilled);
}

extern "C" __declspec(dllexport) void Wrapper_BeginPath(Direct2DWrapper* pDirect2DWrapper){
	pDirect2DWrapper->BeginPath();
}

extern "C" __declspec(dllexport) void Wrapper_Clear(Direct2DWrapper* pDirect2DWrapper,UINT32 color){ 
	pDirect2DWrapper->Clear(color); 
}

extern "C" __declspec(dllexport) void Wrapper_DrawEllipse(Direct2DWrapper* pDirect2DWrapper,UINT32 color,FLOAT cx,FLOAT cy,FLOAT rx,FLOAT ry,FLOAT strokeWidth){ 
	return pDirect2DWrapper->DrawEllipse(color,cx,cy,rx,ry,strokeWidth); 
}

extern "C" __declspec(dllexport) void Wrapper_DrawLine(Direct2DWrapper* pDirect2DWrapper,UINT32 color,FLOAT x1,FLOAT y1,FLOAT x2,FLOAT y2,FLOAT strokeWidth){ 
	return pDirect2DWrapper->DrawLine(color,x1,y1,x2,y2,strokeWidth); 
}

extern "C" __declspec(dllexport) void Wrapper_DrawRectangle(Direct2DWrapper* pDirect2DWrapper,UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height,FLOAT strokeWidth){ 
	return pDirect2DWrapper->DrawRectangle(color,x,y,width,height,strokeWidth); 
}

extern "C" __declspec(dllexport) void Wrapper_EndFigure(Direct2DWrapper* pDirect2DWrapper,bool isClosed){
	return pDirect2DWrapper->EndFigure(isClosed);
}

extern "C" __declspec(dllexport) void Wrapper_EndPath(Direct2DWrapper* pDirect2DWrapper,bool isFilled,bool isOutline,int colorFill,int colorOutline,float strokeWidth){
	return pDirect2DWrapper->EndPath(isFilled,isOutline,colorFill,colorOutline,strokeWidth);
}

extern "C" __declspec(dllexport) void Wrapper_FillEllipse(Direct2DWrapper* pDirect2DWrapper,UINT32 color,FLOAT cx,FLOAT cy,FLOAT rx,FLOAT ry){ 
	return pDirect2DWrapper->FillEllipse(color,cx,cy,rx,ry); 
}

extern "C" __declspec(dllexport) void Wrapper_FillRectangle(Direct2DWrapper* pDirect2DWrapper,UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height){ 
	return pDirect2DWrapper->FillRectangle(color,x,y,width,height); 
}

extern "C" __declspec(dllexport) void Wrapper_FillRectangleGradient(Direct2DWrapper * pDirect2DWrapper, int gradientNum, FLOAT x, FLOAT y, FLOAT width, FLOAT height) {
	pDirect2DWrapper->FillRectangleGradient(gradientNum, x, y, width, height);
}

extern "C" __declspec(dllexport) void Wrapper_FillRoundedRectangle(Direct2DWrapper* pDirect2DWrapper,UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height,FLOAT radiusX){
	return pDirect2DWrapper->FillRoundedRectangle(color,x,y,width,height,radiusX);
}

extern "C" __declspec(dllexport) void Wrapper_RestoreDrawingState(Direct2DWrapper* pDirect2DWrapper,int levelNum){
	pDirect2DWrapper->RestoreDrawingState(levelNum);
}

extern "C" __declspec(dllexport) void Wrapper_Rotate(Direct2DWrapper* pDirect2DWrapper,float angle){
	pDirect2DWrapper->Rotate(angle);
}

extern "C" __declspec(dllexport) void Wrapper_SaveDrawingState(Direct2DWrapper* pDirect2DWrapper,int levelNum){
	pDirect2DWrapper->SaveDrawingState(levelNum);
}

extern "C" __declspec(dllexport) void Wrapper_Scale(Direct2DWrapper* pDirect2DWrapper,float scale){ 
	pDirect2DWrapper->Scale(scale); 
}

extern "C" __declspec(dllexport) void Wrapper_Translate(Direct2DWrapper* pDirect2DWrapper,float x,float y){ 
	pDirect2DWrapper->Translate(x,y); 
}





Direct2DWrapper::Direct2DWrapper() :
	m_width(0),
	m_height(0),
	m_hdc(0)
{
}

Direct2DWrapper::~Direct2DWrapper(){
	
}

HRESULT Direct2DWrapper::Initialize() {
	HRESULT hr=S_OK;
	if(m_D2D1Factory1){
		return hr;
	}
	D2D1_FACTORY_OPTIONS factory_options={};
	//#ifdef DEBUG
	//	factory_options.debugLevel=D2D1_DEBUG_LEVEL_INFORMATION;
	//#endif
	hr=D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED,factory_options,m_D2D1Factory1.ReleaseAndGetAddressOf());
	return hr;
}

///<summary>brushNum is 0 index item in a list of brushes.</summary>
void Direct2DWrapper::CreateGradientBrush(UINT32 brushNum, UINT32 color1, UINT32 color2, FLOAT x1, FLOAT y1, FLOAT x2, FLOAT y2) {
	ComPtr<ID2D1GradientStopCollection> pGradientStops;
	D2D1_GRADIENT_STOP gradientStops[2];
	gradientStops[0].color = ColorF(color1);
	gradientStops[0].position = 0.0f;
	gradientStops[1].color = ColorF(color2);
	gradientStops[1].position = 1.0f;
	m_D2D1BitmapRenderTarget->CreateGradientStopCollection(
		gradientStops,
		2,
		D2D1_GAMMA_2_2,
		D2D1_EXTEND_MODE_CLAMP,
		pGradientStops.GetAddressOf()
	);
	m_D2D1BitmapRenderTarget->CreateLinearGradientBrush(
		D2D1::LinearGradientBrushProperties(D2D1::Point2F(x1, y1), D2D1::Point2F(x2, y2)),
		pGradientStops.Get(),
		m_ID2D1LinearGradientBrush0.ReleaseAndGetAddressOf()
	);
	pGradientStops.Reset();
}

HRESULT Direct2DWrapper::BeginDraw(UINT32 width,UINT32 height,HDC hdc){
	HRESULT hr=S_OK;
	m_width=width;
	m_height=height;
	m_hdc=hdc;
	if(!m_D2D1DCRenderTarget){//render target does not exist
		D2D1_PIXEL_FORMAT pixelFormat=PixelFormat(
			DXGI_FORMAT_B8G8R8A8_UNORM,
			D2D1_ALPHA_MODE_PREMULTIPLIED
		);
		D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties=RenderTargetProperties();
		renderTargetProperties.pixelFormat=pixelFormat;
		renderTargetProperties.type=D2D1_RENDER_TARGET_TYPE_SOFTWARE;//this is required for sharing a DC with GDI+ graphics
		//otherwise, the drawing gets stuck on the GPU and won't clear out, making antialiased lines look pixelated.
		//This can be revisited if we need D2D without a shared GDI+ DC.
		hr=m_D2D1Factory1->CreateDCRenderTarget(
			&renderTargetProperties,
			m_D2D1DCRenderTarget.ReleaseAndGetAddressOf());
		if(FAILED(hr)) return hr;
		m_D2D1DCRenderTarget->SetAntialiasMode(D2D1_ANTIALIAS_MODE_PER_PRIMITIVE);//already default
		RECT rect=Rect<int>(0,0,width,height);
		hr=m_D2D1DCRenderTarget->BindDC(hdc, &rect);
		if(FAILED(hr)) return hr;
		hr=m_D2D1DCRenderTarget->CreateCompatibleRenderTarget(m_D2D1BitmapRenderTarget.ReleaseAndGetAddressOf());
		if(FAILED(hr)) return hr;
		//D2D1_DRAWING_STATE_DESCRIPTION drawingState=DrawingStateDescription();
		m_D2D1Factory1->CreateDrawingStateBlock(m_D2D1DrawingStateBlock.ReleaseAndGetAddressOf());
		m_D2D1Factory1->CreateDrawingStateBlock(m_ID2D1DrawingStateBlock1.ReleaseAndGetAddressOf());
	}
	m_D2D1BitmapRenderTarget->BeginDraw();
	return S_OK;
}

HRESULT Direct2DWrapper::EndDraw(){
	HRESULT hr=S_OK;
	hr=m_D2D1BitmapRenderTarget->EndDraw();
	ComPtr<ID2D1Bitmap> bitmap;
	m_D2D1BitmapRenderTarget->GetBitmap(bitmap.GetAddressOf());
	RECT rect=Rect<int>(0,0,m_width,m_height);
	m_D2D1DCRenderTarget->BindDC(m_hdc,&rect);
	m_D2D1DCRenderTarget->BeginDraw();
	m_D2D1DCRenderTarget->DrawBitmap(bitmap.Get());
	hr=m_D2D1DCRenderTarget->EndDraw();
	if(hr==D2DERR_RECREATE_TARGET){//for bitmaps, we're only drawing once, so we don't care about this
		m_D2D1DCRenderTarget.Reset();
		m_D2D1BitmapRenderTarget.Reset();
	}
	return hr;
}

void Direct2DWrapper::ReleaseDeviceResources(void){
	//Does not release Factory
	m_D2D1BitmapRenderTarget.Reset();
	m_D2D1DCRenderTarget.Reset();
}

//from here down, everything is alphabetical---------------------------------------------------------------------------------------------------
void Direct2DWrapper::AddArc(float x,float y,float width,float height,float rotation,bool isLargeArc,bool isCW){
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

void Direct2DWrapper::AddBezier(float x1,float y1,float x2,float y2,float x,float y){
	D2D1_BEZIER_SEGMENT bezier;
	bezier.point1.x = x1;//control 1
	bezier.point1.y = y1;
	bezier.point2.x = x2;//control 2
	bezier.point2.y = y2;
	bezier.point3.x = x;//end point
	bezier.point3.y = y;
	m_ID2D1GeometrySink->AddBezier(&bezier);
}

void Direct2DWrapper::AddLine(float x,float y){
	m_ID2D1GeometrySink->AddLine(Point2F(x,y));
}

void Direct2DWrapper::AddQuadraticBezier(float x1,float y1,float x,float y){
	D2D1_QUADRATIC_BEZIER_SEGMENT bezier;
	bezier.point1.x = x1;//control 1
	bezier.point1.y = y1;
	bezier.point2.x = x;//endpoint
	bezier.point2.y = y;
	m_ID2D1GeometrySink->AddQuadraticBezier(&bezier);
}

void Direct2DWrapper::BeginFigure(float x,float y,bool isFilled) {
	if(isFilled){
		m_ID2D1GeometrySink->BeginFigure({x,y}, D2D1_FIGURE_BEGIN_FILLED);
	}
	else{
		m_ID2D1GeometrySink->BeginFigure({x,y}, D2D1_FIGURE_BEGIN_HOLLOW);
	}
}

void Direct2DWrapper::BeginPath(){
	m_D2D1Factory1->CreatePathGeometry(m_ID2D1PathGeometry.ReleaseAndGetAddressOf());
	m_ID2D1PathGeometry->Open(m_ID2D1GeometrySink.ReleaseAndGetAddressOf());
	m_ID2D1GeometrySink->SetFillMode(D2D1_FILL_MODE_WINDING);
}

void Direct2DWrapper::Clear(UINT32 color){
	m_D2D1BitmapRenderTarget->Clear(ColorF(color));
}

void Direct2DWrapper::DrawEllipse(UINT32 color,FLOAT cx,FLOAT cy,FLOAT rx,FLOAT ry,FLOAT strokeWidth){
	m_D2D1BitmapRenderTarget->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	m_D2D1BitmapRenderTarget->DrawEllipse(Ellipse(Point2F(cx,cy),rx,ry),m_brush.Get(),strokeWidth);
}

void Direct2DWrapper::DrawLine(UINT32 color,FLOAT x1,FLOAT y1,FLOAT x2,FLOAT y2,FLOAT strokeWidth){
	m_D2D1BitmapRenderTarget->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	m_D2D1BitmapRenderTarget->DrawLine(Point2F(x1,y1),Point2F(x2,y2),m_brush.Get(),strokeWidth);
}

void Direct2DWrapper::DrawRectangle(UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height,FLOAT strokeWidth){
	m_D2D1BitmapRenderTarget->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	m_D2D1BitmapRenderTarget->DrawRectangle(RectF(x,y,x+width,y+height),m_brush.Get(),strokeWidth);
}

void Direct2DWrapper::EndFigure(bool isClosed) {
	if(isClosed){
		m_ID2D1GeometrySink->EndFigure(D2D1_FIGURE_END_CLOSED);
	}
	else{
		m_ID2D1GeometrySink->EndFigure(D2D1_FIGURE_END_OPEN);
	}
}

void Direct2DWrapper::EndPath(bool isFilled,bool isOutline,int colorFill,int colorOutline,float strokeWidth){
	m_ID2D1GeometrySink->Close();
	m_ID2D1GeometrySink.Reset();
	if(isFilled){
		m_D2D1BitmapRenderTarget->CreateSolidColorBrush(ColorF(colorFill),m_brush.ReleaseAndGetAddressOf());
		m_D2D1BitmapRenderTarget->FillGeometry(m_ID2D1PathGeometry.Get(),m_brush.Get());
	}
	if(isOutline){
		m_D2D1BitmapRenderTarget->CreateSolidColorBrush(ColorF(colorOutline),m_brush.ReleaseAndGetAddressOf());
		//this was an attempt to support rounded ends and connections.  I couldn't get it to work quickly, and it's low priority, so revisit later:
		//ComPtr<ID2D1StrokeStyle>  pID2D1StrokeStyle;
		//float dashes[] ={1.0f,1.0f};
		//m_ID2D1Factory1->CreateStrokeStyle(StrokeStyleProperties(D2D1_CAP_STYLE_ROUND, D2D1_CAP_STYLE_ROUND, D2D1_CAP_STYLE_FLAT, D2D1_LINE_JOIN_ROUND,10.0f,D2D1_DASH_STYLE_SOLID,0.0f),dashes,ARRAYSIZE(dashes),pID2D1StrokeStyle.GetAddressOf());//,10.0f,D2D1_DASH_STYLE_SOLID,0.0f,
		m_D2D1BitmapRenderTarget->DrawGeometry(m_ID2D1PathGeometry.Get(),m_brush.Get(),strokeWidth);//,pID2D1StrokeStyle.Get());
		//pID2D1StrokeStyle.Reset();
	}
}

void Direct2DWrapper::FillEllipse(UINT32 color,FLOAT cx,FLOAT cy,FLOAT rx,FLOAT ry){
	m_D2D1BitmapRenderTarget->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	m_D2D1BitmapRenderTarget->FillEllipse(Ellipse(Point2F(cx,cy),rx,ry),m_brush.Get());
}

void Direct2DWrapper::FillRectangle(UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height){
	m_D2D1BitmapRenderTarget->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	m_D2D1BitmapRenderTarget->FillRectangle(RectF(x,y,x+width,y+height),m_brush.Get());
}

void Direct2DWrapper::FillRectangleGradient(int gradientNum, FLOAT x, FLOAT y, FLOAT width, FLOAT height) {
	if (gradientNum == 0) {
		m_D2D1BitmapRenderTarget->FillRectangle(RectF(x, y, x + width, y + height), m_ID2D1LinearGradientBrush0.Get());
	}
	//others are not yet supported
}

void Direct2DWrapper::FillRoundedRectangle(UINT32 color, FLOAT x, FLOAT y, FLOAT width, FLOAT height,FLOAT radiusX) {
	m_D2D1BitmapRenderTarget->CreateSolidColorBrush(ColorF(color),m_brush.ReleaseAndGetAddressOf());
	D2D1_ROUNDED_RECT roundedRect;
	roundedRect.rect.top=y;
	roundedRect.rect.left=x;
	roundedRect.rect.right=x+width;
	roundedRect.rect.bottom=y+height;
	roundedRect.radiusX=radiusX;
	roundedRect.radiusY=radiusX;
	m_D2D1BitmapRenderTarget->FillRoundedRectangle(roundedRect,m_brush.Get());
}

void Direct2DWrapper::RestoreDrawingState(int levelNum){
	if(levelNum==0){
		m_D2D1BitmapRenderTarget->RestoreDrawingState(m_D2D1DrawingStateBlock.Get());
	}
	if(levelNum==1){
		m_D2D1BitmapRenderTarget->RestoreDrawingState(m_ID2D1DrawingStateBlock1.Get());
	}
}

void Direct2DWrapper::Rotate(float angle){
	D2D1_MATRIX_3X2_F matrixOld;
	m_D2D1BitmapRenderTarget->GetTransform(&matrixOld);
	D2D1_MATRIX_3X2_F matrixRotate=D2D1::Matrix3x2F::Rotation(angle);
	m_D2D1BitmapRenderTarget->SetTransform(matrixRotate * matrixOld);
}

void Direct2DWrapper::SaveDrawingState(int levelNum){
	if(levelNum==0){
		m_D2D1BitmapRenderTarget->SaveDrawingState(m_D2D1DrawingStateBlock.Get());
	}
	if(levelNum==1){
		m_D2D1BitmapRenderTarget->SaveDrawingState(m_ID2D1DrawingStateBlock1.Get());
	}
}

void Direct2DWrapper::Scale(float scale){
	D2D1_MATRIX_3X2_F matrixOld;
	m_D2D1BitmapRenderTarget->GetTransform(&matrixOld);
	D2D1_MATRIX_3X2_F matrixScale=D2D1::Matrix3x2F::Scale(D2D1::Size(scale,scale),D2D1::Point2F(0,0));
	m_D2D1BitmapRenderTarget->SetTransform(matrixScale * matrixOld); 
}

void Direct2DWrapper::Translate(float x,float y){
	D2D1_MATRIX_3X2_F matrixOld;
	m_D2D1BitmapRenderTarget->GetTransform(&matrixOld);
	D2D1_MATRIX_3X2_F matrixTranslation =D2D1::Matrix3x2F::Translation(x,y);
	m_D2D1BitmapRenderTarget->SetTransform(matrixTranslation * matrixOld);
}


