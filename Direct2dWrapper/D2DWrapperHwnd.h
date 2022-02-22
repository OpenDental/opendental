#pragma once

class D2DWrapperHwnd{
	public:
		D2DWrapperHwnd();
		~D2DWrapperHwnd();
		HRESULT Initialize();
		BOOL DeviceResourcesNeedRecreate();
		HRESULT CreateRenderTarget(UINT32 width,UINT32 height,HWND hwnd);
		void CreateGradientBrush(UINT32 brushNum,UINT32 color1,UINT32 color2,FLOAT x1,FLOAT y1,FLOAT x2,FLOAT y2);
		void CreateBitmap(int bitmapNum,byte bytesBitmap[],int width,int height);
		void CreateBitmapBlank(int bitmapNum,int width,int height);
		void SetTargetToBitmap(int bitmapNum);
		void SetTargetToOriginal();
		void BeginDraw();
		HRESULT EndDraw(void);
		//from here down, everything is alphabetical---------------------------------------------
		void AddArc(float x,float y,float width,float height,float rotation,bool isLargeArc,bool isCW);
		void AddBezier(float x1,float y1,float x2,float y2,float x,float y);
		void AddLine(float x,float y);
		void AddQuadraticBezier(float x1,float y1,float x,float y);
		void BeginFigure(float x,float y,bool isFilled);
		void BeginPath();
		void Clear(UINT32 color);
		void DrawAtlas(int bitmapNum,int xAtlas,int yAtlas,int sizeAtlas,int x,int y,int size);
		void DrawBitmap(int bitmapNum,int x,int y,int width,int height);
		void DrawBitmapImmediate(byte bytesBitmap[],int widthBitmap,int heightBitmap,int x,int y,int width,int height);
		void DrawEllipse(UINT32 color,FLOAT cx,FLOAT cy,FLOAT rx,FLOAT ry,FLOAT strokeWidth);
		void DrawLine(UINT32 color,FLOAT x1,FLOAT y1,FLOAT x2,FLOAT y2,FLOAT strokeWidth);
		void DrawRectangle(UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height,FLOAT strokeWidth);
		void DrawRoundedRectangle(UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height,FLOAT radiusX,FLOAT strokeWidth);
		void DrawTextMy(FLOAT x,FLOAT y,FLOAT width,FLOAT height,UINT32 color,FLOAT fontSize,LPCWSTR text);
		void EndFigure(bool isClosed);
		void EndPath(bool isFilled,bool isOutline,int colorFill,int colorOutline,float strokeWidth);
		void FillEllipse(UINT32 color,FLOAT cx,FLOAT cy,FLOAT rx,FLOAT ry);
		void FillRectangle(UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height);
		void FillRectangleGradient(int gradientNum,FLOAT x,FLOAT y,FLOAT width,FLOAT height);
		void FillRoundedRectangle(UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height,FLOAT radiusX);
		void RestoreDrawingState(int idx);
		void Rotate(float angle);
		void SaveDrawingState(int idx);
		void Scale(float scale);
		void Translate(float x,float y);

	private:
		static Microsoft::WRL::ComPtr<ID2D1Factory1> m_ID2D1Factory1;
		static Microsoft::WRL::ComPtr<IDWriteFactory1> m_IDWriteFactory1;
		Microsoft::WRL::ComPtr<ID3D11DeviceContext> m_ID3D11DeviceContext;
		Microsoft::WRL::ComPtr<ID3D11Device> m_ID3D11Device;
		Microsoft::WRL::ComPtr<ID3D11Device1> m_ID3D11Device1;
		Microsoft::WRL::ComPtr<ID3D11DeviceContext1> m_ID3D11DeviceContext1;
		Microsoft::WRL::ComPtr<IDXGIDevice1> m_IDXGIDevice1;
		Microsoft::WRL::ComPtr<ID2D1Device> m_ID2D1Device;
		Microsoft::WRL::ComPtr<ID2D1DeviceContext> m_ID2D1DeviceContext;
		Microsoft::WRL::ComPtr<IDXGISwapChain1> m_IDXGISwapChain1;
		Microsoft::WRL::ComPtr<ID2D1Bitmap1> m_ID2D1Bitmap1_Render;
		Microsoft::WRL::ComPtr<ID2D1Image> m_ID2D1ImageTargetOriginal;
		//device independent:
		Microsoft::WRL::ComPtr<ID2D1DrawingStateBlock> m_ID2D1DrawingStateBlock0;
		Microsoft::WRL::ComPtr<ID2D1DrawingStateBlock> m_ID2D1DrawingStateBlock1;
		Microsoft::WRL::ComPtr<ID2D1PathGeometry> m_ID2D1PathGeometry;
		Microsoft::WRL::ComPtr<ID2D1GeometrySink> m_ID2D1GeometrySink;
		//device dependent:
		Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_brush;//gets repeatedly used for any solid brush
		Microsoft::WRL::ComPtr<IDWriteTextFormat> m_IDWriteTextFormat;
		Microsoft::WRL::ComPtr<ID2D1LinearGradientBrush> m_ID2D1LinearGradientBrush0;
		Microsoft::WRL::ComPtr<ID2D1Bitmap1> m_ID2D1Bitmap1_0;
};