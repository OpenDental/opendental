#pragma once

class Direct2DWrapper{
	public:
		Direct2DWrapper();
		~Direct2DWrapper();
		HRESULT Initialize();
		HRESULT BeginDraw(UINT32 width,UINT32 height,HDC hdc);
		void CreateGradientBrush(UINT32 brushNum, UINT32 color1, UINT32 color2, FLOAT x1, FLOAT y1, FLOAT x2, FLOAT y2);
		HRESULT EndDraw(void);
		void ReleaseDeviceResources(void);
		//from here down, everything is alphabetical---------------------------------------------
		void AddArc(float x,float y,float width,float height,float rotation,bool isLargeArc,bool isCW);
		void AddBezier(float x1,float y1,float x2,float y2,float x,float y);
		void AddLine(float x,float y);
		void AddQuadraticBezier(float x1,float y1,float x,float y);
		void BeginFigure(float x,float y,bool isFilled);
		void BeginPath();
		void Clear(UINT32 color);
		void DrawEllipse(UINT32 color,FLOAT cx,FLOAT cy,FLOAT rx,FLOAT ry,FLOAT strokeWidth);
		void DrawLine(UINT32 color,FLOAT x1,FLOAT y1,FLOAT x2,FLOAT y2,FLOAT strokeWidth);
		void DrawRectangle(UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height,FLOAT strokeWidth);
		void EndFigure(bool isClosed);
		void EndPath(bool isFilled,bool isOutline,int colorFill,int colorOutline,float strokeWidth);
		void FillEllipse(UINT32 color,FLOAT cx,FLOAT cy,FLOAT rx,FLOAT ry);
		void FillRectangle(UINT32 color,FLOAT x,FLOAT y,FLOAT width,FLOAT height);
		void FillRectangleGradient(int gradientNum, FLOAT x, FLOAT y, FLOAT width, FLOAT height);
		void FillRoundedRectangle(UINT32 color, FLOAT x, FLOAT y, FLOAT width, FLOAT height,FLOAT radiusX);
		void RestoreDrawingState(int idx);
		void Rotate(float angle);
		void SaveDrawingState(int idx);
		void Scale(float scale);
		void Translate(float x,float y);

	private:
		Microsoft::WRL::ComPtr<ID2D1Factory1> m_D2D1Factory1;
		Microsoft::WRL::ComPtr<ID2D1BitmapRenderTarget> m_D2D1BitmapRenderTarget;
		Microsoft::WRL::ComPtr<ID2D1DCRenderTarget> m_D2D1DCRenderTarget;
		UINT32 m_width;
		UINT32 m_height;
		HDC m_hdc;
		//device independent:
		Microsoft::WRL::ComPtr<ID2D1DrawingStateBlock> m_D2D1DrawingStateBlock;
		Microsoft::WRL::ComPtr<ID2D1DrawingStateBlock> m_ID2D1DrawingStateBlock1;
		Microsoft::WRL::ComPtr<ID2D1PathGeometry> m_ID2D1PathGeometry;
		Microsoft::WRL::ComPtr<ID2D1GeometrySink> m_ID2D1GeometrySink;
		//device dependent:
		Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_brush;//gets repeatedly used for any solid brush
		Microsoft::WRL::ComPtr<ID2D1LinearGradientBrush> m_ID2D1LinearGradientBrush0;

};