# K-SHIFT Text2SQL ChatBot - Web Application Handoff Document

## 1. Overview

WPF 데스크톱 챗봇을 ASP.NET Web Forms 웹 애플리케이션으로 포팅하기 위한 인수인계 문서입니다.

### 현재 구현 (WPF Desktop)
- .NET Framework 4.7.2 WPF Class Library
- MVVM 패턴 (INotifyPropertyChanged, RelayCommand)
- Newtonsoft.Json + System.Net.Http

### 대상 Web Application
- **Repository**: https://github.com/Spelix-Ksj/KshiftSmartPortal
- **기술 스택**: ASP.NET Web Forms (.aspx), C# code-behind
- **CSS**: Bootstrap 5.1.3, Font Awesome 6.0
- **JS**: jQuery (Bootstrap 번들 포함)
- **Layout**: Site.Master (마스터 페이지)
- **기존 패턴**: `Views/ToDoList.aspx`, `Views/Home.aspx` 등 Content Page 구조

---

## 2. Architecture

```
Web Browser (JavaScript + HTML)
    |
    | fetch() / jQuery.ajax()
    | HTTPS
    |
    v
Gateway API (ASP.NET Core 8, https://hq.spelix.co.kr)
    |-- POST /api/auth/login    (Anonymous, JWT 발급)
    |-- POST /api/chat/ask      (Bearer JWT, SQL 생성)
    |-- POST /api/chat/execute   (Bearer JWT, SQL 실행)
    |-- GET  /api/chat/health    (Bearer JWT, GPU 상태)
    |-- GET  /api/health         (Anonymous, Gateway 상태)
    |
    v
GPU FastAPI Server (192.168.10.40:8002, Internal)
    |-- text2sql_pipeline (vLLM + Oracle DB)
```

---

## 3. API Specification

### 3.1 Login
```
POST https://hq.spelix.co.kr/api/auth/login
Content-Type: application/json

Request:  { "userId": "admin", "password": "admin" }
Response: { "token": "eyJ...", "expiresAt": "2026-03-07T18:00:00Z" }
Error:    401 Unauthorized (ID/PW 불일치)
```

### 3.2 Ask (SQL Generation)
```
POST https://hq.spelix.co.kr/api/chat/ask
Content-Type: application/json
Authorization: Bearer {token}

Request:  { "question": "전체 호선 목록을 보여줘", "modelKey": null }
Response: { "sql": "SELECT ...", "reasoning": "추론 과정...", "error": null }
```

### 3.3 Execute (SQL Execution)
```
POST https://hq.spelix.co.kr/api/chat/execute
Content-Type: application/json
Authorization: Bearer {token}

Request:  { "sql": "SELECT ...", "question": "원래 질문", "reasoning": "추론", "modelKey": null }
Response: {
    "columns": ["호선코드", "호선명", ...],
    "rows": [["S001", "테스트호선", ...], ...],
    "rowCount": 42,
    "error": null,
    "report": "분석 보고서 텍스트..."
}
```

### 3.4 Health Check
```
GET https://hq.spelix.co.kr/api/chat/health
Authorization: Bearer {token}

Response 200: { "status": "ok", "gpu": "connected" }
Response 503: { "status": "degraded", "gpu": "disconnected" }
```

### 중요: JSON Property Naming
- Gateway는 ASP.NET Core 8.0 (System.Text.Json) 사용
- **응답**: camelCase (`expiresAt`, `rowCount`, `modelKey`)
- **요청**: case-insensitive 역직렬화 (camelCase 권장)

---

## 4. UI Design Specification

### 4.1 Login Screen
| 요소 | 스타일 |
|------|--------|
| 배경 | 네이비 그라데이션 (#1B2A4A -> #2C3E6B, 대각선) |
| 로고 영역 | 64x64 아이콘, 배경 #3A5A8C, CornerRadius 16 |
| 타이틀 | "K-SHIFT Chatbot", 22pt, White, Bold |
| 서브타이틀 | "Master Plan Management System", 12pt, rgba(255,255,255,0.6) |
| 입력 카드 | 흰색, border-radius: 20px 20px 0 0 (상단만 둥글게) |
| ID/PW 필드 | height: 40px, border: 2px solid gray, border-radius: 12px, bg: #FAFAFA |
| 필드 포커스 | border-color: RoyalBlue |
| 로그인 버튼 | bg: #1B2A4A, color: white, border-radius: 20px, height: 45px, 전체 너비 |
| 에러 텍스트 | color: #FF4444, 중앙 정렬, 버튼 아래 |
| Copyright | "Copyright 2025.12 SPELIX. All rights reserved.", color: #AAAAAA, 10pt |

### 4.2 Chat Screen Layout (top to bottom)
1. **Header Bar**: bg #0F4C81, "K-SHIFT Text2SQL" 타이틀, GPU 상태 표시등(녹/적), 로그아웃 버튼
2. **Sample Questions Bar**: bg #F0F4F8, 5개 예제 질문 버튼 (클릭 시 자동 전송)
3. **Message Area**: 스크롤 가능, bg #F5F5F5
4. **Input Bar**: 하단 고정, 입력창 + 전송 버튼

### 4.3 Message Bubble Design
| 역할 | 정렬 | 배경색 | 글자색 |
|------|------|--------|--------|
| User | 우측 | #0F4C81 | White |
| Assistant | 좌측 | #F0F0F0 | #333333 |
| System | 좌측 | #E8F5E9 | #333333 |
| Error | 좌측 | #FFEBEE | #333333 |

### 4.4 Assistant Message 구성
하나의 Assistant 메시지 버블에 포함되는 요소들:
1. **Content/Reasoning 텍스트** (추론 과정)
2. **SQL 코드블록** (있을 경우):
   - 배경 #1E1E1E, font: Consolas 12px, color: #D4D4D4
   - 편집 가능한 textarea
   - "실행" 버튼 (#4CAF50) + "복사" 버튼 (#757575)
3. **결과 테이블** (있을 경우):
   - "조회 결과: N건" 헤더
   - HTML table (max-height: 250px, overflow-y: auto)
   - 결과 0건일 때: "조회 결과가 없습니다. 조건을 변경하여 다시 시도해보세요."
4. **보고서** (있을 경우):
   - 배경 #E8F5E9, padding 8px, border-radius 4px
5. **타임스탬프**: HH:mm, color #999999

### 4.5 Sample Questions (5개)
```
1. "전체 호선 목록을 보여줘"
2. "호선별 블록 수와 중량을 알려줘"
3. "올해 근무일수는 몇 일이야?"
4. "공종별 활동 건수를 보여줘"
5. "컨테이너선 목록을 보여줘"
```

---

## 5. Business Logic (JavaScript 구현 필요)

### 5.1 State Management
```javascript
// 전역 상태
let authToken = null;      // JWT 토큰
let tokenExpiry = null;     // 만료 시각 (Date)
let isLoggedIn = false;
let isBusy = false;
let messages = [];          // { role, content, sql, editableSql, reasoning, report, resultTable, rowCount, isLoading, timestamp }
```

### 5.2 Login Flow
```
1. POST /api/auth/login { userId, password }
2. 성공 시: token 저장, expiresAt 저장, 로그인 패널 숨김, 채팅 패널 표시
3. 실패 시 (401): "로그인에 실패했습니다. ID와 비밀번호를 확인하세요." 에러 표시
4. 네트워크 오류: "서버에 연결할 수 없습니다." 에러 표시
```

### 5.3 Send Question Flow (핵심 로직)
```
1. User 메시지 버블 추가 (우측)
2. Assistant 로딩 버블 추가 ("생각 중..." + 로딩 애니메이션)
3. POST /api/chat/ask { question }
   - 에러 → 에러 버블로 변환, return
4. 로딩 버블 업데이트: reasoning + SQL 코드블록 표시
5. SQL이 있으면 자동으로 POST /api/chat/execute { sql, question, reasoning }
   - rowCount == 0 → "조회 결과가 없습니다..." 보고서 영역에 표시
   - 에러 → "실행 오류: ..." 보고서 영역에 표시
   - 성공 → 결과 테이블 + 보고서 표시
6. 스크롤을 맨 아래로 이동
```

### 5.4 SQL Edit & Re-execute
```
1. 사용자가 SQL textarea 편집
2. "실행" 버튼 클릭 → POST /api/chat/execute { editedSql, question, reasoning }
3. 결과 테이블/보고서 갱신
```

### 5.5 Token Management
```
- 모든 API 호출 전 토큰 만료 확인 (5분 버퍼)
- 만료 시: 에러 메시지 "토큰이 만료되었습니다. 다시 로그인하세요."
- fetch() headers: { "Authorization": "Bearer " + authToken, "Content-Type": "application/json" }
```

### 5.6 Health Check (30초 간격)
```
- setInterval로 GET /api/chat/health 호출
- 200: 상태 표시등 녹색, "연결됨"
- 503/에러: 상태 표시등 적색, "GPU 연결 끊김"
- 로그아웃 시 clearInterval
```

### 5.7 Logout Flow
```
1. Health timer 정지
2. Token/credentials 클리어
3. Messages 배열 비우기
4. 채팅 패널 숨기고 로그인 패널 표시
5. PasswordBox 초기화
```

---

## 6. Web Implementation Guide

### 6.1 File Structure (권장)
```
KshiftSmartPortalWeb/
├── Views/
│   └── ChatBot.aspx              # 챗봇 페이지 (ContentPage)
│   └── ChatBot.aspx.cs           # Code-behind (minimal)
│   └── ChatBot.aspx.designer.cs  # Designer auto-gen
```

### 6.2 ChatBot.aspx 구조
```html
<%@ Page Title="Text2SQL ChatBot" Language="C#" MasterPageFile="~/Views/Site.Master"
         AutoEventWireup="true" CodeBehind="ChatBot.aspx.cs"
         Inherits="KShiftSmartPortalWeb.Views.ChatBot" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    K-SHIFT Text2SQL
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- 전체 챗봇 UI를 HTML + CSS + JavaScript로 구현 -->
    <!-- 서버 측 로직 불필요 — 모든 API 통신은 JavaScript fetch()로 처리 -->
</asp:Content>
```

### 6.3 핵심 구현 포인트

**a) SSL 인증서**: `hq.spelix.co.kr`은 자체서명 인증서 사용.
- 브라우저에서는 사용자가 한 번 "예외 추가"하면 이후 정상 동작
- WPF에서는 `ServerCertificateValidationCallback` bypass 필요했음

**b) CORS**: Gateway에 CORS 설정이 필요할 수 있음
- 현재 Gateway는 CORS 미설정 (WPF/IIS 리버스 프록시에서는 불필요했음)
- 웹 직접 호출 시 `builder.Services.AddCors()` 필요
- 또는 같은 IIS에서 리버스 프록시(/api/*)를 이미 설정해뒀으므로, 같은 도메인에서 호출하면 CORS 불필요

**c) 같은 도메인 호출 (CORS 회피, 권장)**:
```javascript
// hq.spelix.co.kr에서 호스팅되므로 상대경로 사용
fetch('/api/auth/login', { method: 'POST', ... })
// → IIS가 /api/* → localhost:5100 리버스 프록시 처리
```

**d) Enter키 전송**: `keydown` 이벤트에서 Enter → send(), Shift+Enter → 줄바꿈

**e) 코드 복사**: `navigator.clipboard.writeText(sql)` (HTTPS 필수)

---

## 7. CSS Color Reference (Quick Copy)

```css
/* 브랜드 */
--color-navy-dark:   #1B2A4A;
--color-navy-light:  #2C3E6B;
--color-brand-blue:  #0F4C81;
--color-accent-blue: #3A5A8C;

/* 메시지 버블 */
--color-user-bg:      #0F4C81;
--color-user-fg:      #FFFFFF;
--color-assistant-bg:  #F0F0F0;
--color-assistant-fg:  #333333;
--color-system-bg:     #E8F5E9;
--color-error-bg:      #FFEBEE;

/* SQL 코드블록 */
--color-code-bg:      #1E1E1E;
--color-code-fg:      #D4D4D4;

/* 버튼 */
--color-btn-run:      #4CAF50;
--color-btn-copy:     #757575;
--color-btn-send:     #0F4C81;

/* 입력 필드 */
--color-input-border:     gray;
--color-input-focus:      RoyalBlue;
--color-input-bg:         #FAFAFA;

/* 기타 */
--color-sample-bar-bg:    #F0F4F8;
--color-timestamp:        #999999;
--color-error-text:       #FF4444;
```

---

## 8. Reference: WPF Source Files

모든 WPF 소스 파일은 다음 경로에 있습니다:
```
C:\Projects\ALIS\src\Presentation\ChatBot\Alis.ChatBot\
├── Models\
│   ├── ChatMessage.cs           # 메시지 데이터 모델
│   ├── ApiModels.cs             # API DTO (Request/Response)
│   └── GatewayApiClient.cs      # HTTP 클라이언트 (토큰 관리)
├── ViewModels\
│   ├── ChatBotViewModel.cs      # 비즈니스 로직 (Send/Execute/Login/Logout)
│   └── RelayCommand.cs          # ICommand 구현
├── Views\
│   ├── ChatBotWindow.xaml       # UI 레이아웃
│   └── ChatBotWindow.xaml.cs    # Code-behind
└── Converters\                  # (웹에서는 CSS/JS로 대체)
    ├── BoolToVisibilityConverter.cs
    ├── InverseBoolConverter.cs
    ├── MessageAlignmentConverter.cs
    ├── MessageBackgroundConverter.cs
    └── MessageForegroundConverter.cs
```

### Gateway API 소스:
```
C:\Projects\KshiftLlmGateway\KshiftLlmGateway.Api\
├── Endpoints\AuthEndpoints.cs   # POST /api/auth/login
├── Endpoints\ChatEndpoints.cs   # POST /api/chat/ask, /execute, GET /health
├── Models\ChatRequest.cs        # LoginRequest, AskRequest, ExecuteRequest
├── Models\ChatResponse.cs       # LoginResponse, GenerateResponse, ExecuteResponse
└── appsettings.json             # Auth.Users: { "admin": "admin" }
```

---

## 9. Testing Checklist

- [ ] 로그인 성공/실패 처리
- [ ] 질문 전송 → SQL 생성 → 자동 실행 → 결과 표시
- [ ] 결과 0건일 때 메시지 표시
- [ ] SQL 편집 후 재실행
- [ ] SQL 복사 기능
- [ ] 예제 질문 버튼 동작
- [ ] GPU 상태 표시 (30초 Health Check)
- [ ] 로그아웃 → 세션 초기화
- [ ] Enter키 전송 / Shift+Enter 줄바꿈
- [ ] 토큰 만료 시 에러 안내
- [ ] 스크롤 자동 하단 이동
- [ ] 로딩 상태 표시 ("생각 중...")
- [ ] 반응형 레이아웃 (모바일 대응)
