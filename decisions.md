## Chatbot 페이지 연동 — 2026-03-07
- Task Type: feature
- Team: Explorer(조사) → Implementer(구현) → Critic(리뷰)
- Key Decisions:
  - Site.Master 사용 (포털 세션 인증 자동 적용)
  - API: https://hq.spelix.co.kr/api/ (별도 JWT 인증)
  - 전체 UI는 HTML/CSS/JS로 구현 (서버 코드 최소화)
  - 챗봇 API 로그인은 페이지 내부에서 별도 처리
- Status: COMPLETE

### Phase 1: 조사 — 완료
- Site.Master ContentPlaceHolder: TitleContent, HeadContent, PageTitleContent, MainContent, ScriptContent
- 인증: Session["UserID"] 체크 (Site.Master.cs Page_Load)
- API 사양: CHATBOT_WEB_HANDOFF.md에 상세 기술

### Phase 2: 구현 — 완료
- ChatBot.aspx: 챗봇 전체 UI (로그인패널 + 채팅패널) + CSS + JavaScript
- ChatBot.aspx.cs: 최소 code-behind (세션 체크는 Site.Master 위임)
- ChatBot.aspx.designer.cs: 디자이너 파일
- Site.Master: 헤더에 ChatBot 네비게이션 버튼 추가

### Phase 3: 1차 리뷰 — NEEDS_IMPROVEMENT
- CRITICAL 4건: expiresAt 파싱, Health 인증헤더, Execute 본문, isBusy 경쟁조건
- WARNING 5건: innerHTML, GPU 응답 파싱, 0건 메시지, password trim, textContent.trim

### Phase 4: 수정 — 완료
- CRITICAL 4건 + WARNING 5건 모두 수정
- async/await로 비동기 함수 리팩터링

### Phase 5: 재리뷰 — PASS
- CRITICAL 이슈 없음, 배포 가능

---

## DevExpress 버전 업데이트 후 태그 충돌 오류 수정 — 2026-03-04

- Task Type: bugfix
- Team: Researcher (Explorer), Implementer (general-purpose), Reviewer (Critic)
- Key Decisions: (진행 중 업데이트)
- Status: COMPLETE

### 오류 내용
- 파서 오류: 서버 태그 'dx:ASPxComboBox'이(가) 모호합니다
- 원인: 동일한 TagPrefix 'dx'로 여러 DevExpress 어셈블리 등록 충돌
- 기존 버전: v25.1.6.0 → 업데이트 버전: v25.2.5.0

### Phase 1: 연구 — 완료
- **충돌 원인**: Web.config 전역 등록(v25.1) + ASPX 페이지 수준 등록(v25.2) → 동일 TagPrefix "dx" 이중 등록
- **영향 파일**:
  - Web.config: 전역 controls 등록 v25.1 (44-45행), 어셈블리 참조 v25.1 (28-33행, 49-52행, 67-70행)
  - Login.aspx: Register v25.2 (2행)
  - ContractManagerXpo.aspx: Register v25.2 (2-3행)
  - ToDoList.aspx: Register v25.2 (3행)
  - Default.aspx: Register v25.1 (2행) — 미업데이트
  - packages.config: v25.1.6 (csproj는 v25.2.5)
- **수정 방향 결정**:
  1. Web.config 전체를 v25.2.5.0으로 통일
  2. ASPX 파일의 중복 Register 지시문 제거 (전역 등록으로 충분)
  3. packages.config를 v25.2.5로 업데이트 또는 제거

### Phase 2-3: 구현 — 완료
**구현팀 A (Web.config + packages.config):**
- Web.config: 16곳 v25.1 → v25.2 변경 (configSections, assemblies, controls, httpHandlers, httpModules, system.webServer)
- packages.config: 4개 DevExpress 패키지 25.1.6 → 25.2.5 변경

**구현팀 B (ASPX Register 지시문 제거):**
- Login.aspx: Register 1행 삭제
- ContractManagerXpo.aspx: Register 1행 삭제
- ToDoList.aspx: Register 1행 삭제
- Default.aspx: Register 1행 삭제

### Phase 4: 리뷰 — PASS
- CRITICAL 이슈: 없음
- WARNING 2건:
  1. PackageReference + packages.config 이중 관리 혼용 (배포 전 확인 필요)
  2. Web.config directoryBrowse enabled="true" 보안 설정 (운영 환경 주의)
- SUGGESTION 3건: 저작권 오타, packages.config Content 분류, .bak 파일 정리
- **결론: 핵심 오류(태그 충돌) 완전 해소, 배포 가능**
