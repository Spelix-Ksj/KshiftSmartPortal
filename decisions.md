## 블록제작 3단계: 가공 탭 활성화 (ScmBlockCuttingPlanMan) — 2026-09-17
- Task Type: feature
- Team: Explorer(가공 템플릿·DB 설정 확인) ∥ Implementer(템플릿 배포 + ViewDefs 활성화) → Critic(소규모) + Tester.
- 전제: 범용 페이지 BlockSchedule.aspx?view=ScmBlockCuttingPlanMan 이 DB(TEMP_MASTER/DETAIL) 주도로 동작. DB 실측: TEMP002, TAG2=SCM_BLOCK_CUTTING_WS, EXCEL_NAME `.\ExcelFile\2. 가공 SCHEDULE.xlsx`, ST_ROW=11(0-based→엑셀 12행), ST_COL=7, TAG3 null(시트명은 상수 SCHEDULE).
- Key Decisions:
  - 탭 순서 권장: 가공 → 조립 → PE → 도장 → 선적 → 셀가이드 → 메인입력(SCM) → 소조(다행 레코드라 마지막).
  - 탭당 작업 = 템플릿 xlsx 복사·csproj Content + ViewDefs Enabled=true + 검증. 코드 로직 변경 없음이 목표.
- Status: COMPLETE (가공 탭)

### Phase 1: 조사 + 구현 — 완료
- 탐색: 템플릿 `2. 가공 SCHEDULE.xlsx`(112KB) 시트 SCHEDULE(visible, 활성) + 빈 Sheet1(visible, 참조 없음). A1:BY5354, 틀고정 V12, autoFilter A11:BY11, 제목 5행 "◈ 금강중공업 절단 공정", 헤더 7~10행, 12행부터 데이터(ST_ROW=11 정합). 샘플 실데이터 0, hidden 시트 0, 조건부서식 0, 수식 AD/AE 2열(12~3068행). 외부 링크 파트 4개(UNC \Ks05, N:\데이타\..., OneDrive URL — 셀 수식 참조 0, definedName만), Print_Area=#REF!, EMF 이미지 2·메모 2.
- DB: TEMP002 ST_ROW=11, DETAIL 19행 전부 MASTER/USE_YN=Y/COLUMN_CELL 있음(G~AL), TAG2 NUMBER 9·DATE 5·빈 5, PROP2=Y 3(가공중기/절단 착수/실행 착수), KEY 호선·가공블록, PROP_NAME 전부 실재. 데이터 1010/ING 58건.
- ALIS Cutting MakeScheduleSheet는 Master와 동일(시그니처만 다름), sheetName "SCHEDULE", ORDER BY PROJECT_NO, ITM_COD.
- 구현: 템플릿 복사(MD5 동일) + csproj Content 88행 + ViewDefs 가공 Enabled=true. 코드 로직 변경 없음. 빌드 오류 0.

### Phase 2: 리뷰 + 검증 + 유출 방지 — 완료
- 리뷰(team-critic): NEEDS_IMPROVEMENT. **[CRITICAL] 가공 템플릿에 외부 링크 4개 + 캐시 데이터(거래처 단가·품목), rels에 내부 호스트(\Ks05), N:\데이타\거래처명 경로, 작성자 OneDrive 절대경로(고객사명 포함), docProps 작성자 실명, 메모(chj)** → SaveCopy 다운로드로 그대로 유출. 마스터 템플릿에는 없던 문제. [MEDIUM] 5,354행 서식으로 첫 렌더 지연 가능. [LOW] 메모 잔존.
- API 확인(리플렉션): `IWorkbook.DocumentProperties`(Author/LastModifiedBy/Company/Manager 등 설정 가능), `Worksheet.ClearComments()`, `DefinedNameCollection.Remove/Clear` 있음. **`ExternalWorkbookCollection`은 Add만 있고 제거 불가** → 외부 링크 파트는 배포 시 zip 수준 제거 스크립트 필요.
- 결정: (1) `ScheduleSheetInjector`에 `Sanitize(IWorkbook)` 추가 — 외부 참조/`#REF!` 정의 이름 제거, 문서 속성 개인정보 비움, 전 시트 ClearComments — 모든 탭 템플릿에 공통 적용. (2) 프로젝트에 `Tools\Sanitize-ExcelTemplate.ps1`(zip 수준: xl/externalLinks·rels·[Content_Types]·workbook.xml externalReferences·외부 definedNames·docProps 정리) 추가하고 가공 사본에 적용. 템플릿 배포 절차에 포함. (3) 다운로드 파일 zip 검사로 검증.
- 수정: `ScheduleSheetInjector.Sanitize(IWorkbook)`(외부참조·#REF! 정의 이름 제거, 문서 속성 개인정보 비움+Custom.Clear, 전 시트 Comments/ThreadedComments.Clear) — Inject 시작부 호출로 모든 템플릿 공통. 신규 `Tools\Sanitize-ExcelTemplate.ps1`(zip 수준: externalLinks·rels·Content_Types·externalReferences·외부 definedName·absPath·core.xml 작성자·comments/vmlDrawing/legacyDrawing·cameraTool 제거, 멱등) — 가공 35건·마스터 3건 정리 적용(-Backup 없음, 원본은 ALIS SVN). aspx 주석 갱신. 빌드 오류 0.
- 재검증(테스터): 가공 스프레드시트 정상(58건, 12행부터, 수식·EMF 그림 표시), 다운로드 zip에 externalLinks/comments/vml 없음·작성자 빈값·유출 문자열 0건. 메인 탭도 정상(663건, 조건부서식 보존, 시트 1개). 첫 렌더 가공 4.4초·메인 3.5초.
- 잔여(경미): `BlockSchedule.aspx.cs` L21 XML 주석 "1단계는 메인(공유)만 활성" 문구 낡음. MSB3247 경고(System.Text.Json 8.0.0.5↔8.0.0.6, Web.config bindingRedirect) 기존 문제.
- **탭 추가 절차(확정)**: ① ALIS 템플릿 xlsx → App_Data\ExcelTemplates 복사 ② `powershell -File Tools\Sanitize-ExcelTemplate.ps1 -Path <xlsx>` 실행 ③ csproj Content 등록 ④ ViewDefs Enabled=true ⑤ 빌드·브라우저 검증(그리드/시트/다운로드 zip).

## 블록제작 2단계: 라디오[그리드|스프레드시트] + 엑셀 양식 그대로 표시 — 2026-09-17
- Task Type: feature
- Team: Explorer×2(ALIS 주입 로직·템플릿·DB 설정 / ASPxSpreadsheet 통합 요건) → Plan → Implementer → Critic → Tester. ALIS 세션 협업.
- 전제(1단계 결과): Views/BlockSchedule.aspx(.cs) + Controllers/BlockScheduleController.cs 존재, 세션 _Meta(TempNo/TableName/SheetName/ExcelName/StRow/StCol) + _Cols(COLUMN_CELL 포함) + _Data 보존됨.
- Key Decisions:
  - 표시 = DevExpress ASPxSpreadsheet 읽기전용(로컬 설치본 DLL, csproj HintPath 방식). 시트명은 상수 "SCHEDULE"(TAG3 미사용). 주입은 OBJ_NAME='MASTER' 행만, ST_ROW부터 COLUMN_CELL 열에 PROP_NAME 값(DATE/NUMBER/텍스트).
  - 템플릿 xlsx는 웹서버 `~/App_Data/ExcelTemplates/{파일명}`에 배포(ALIS bin\Debug\ExcelFile\2. 마스터 SCHEDULE.xlsx 복사), EXCEL_NAME의 파일명만 취해 결합.
- Status: COMPLETE (2단계)

### Phase 1: 조사 — 완료
- ALIS 주입(탐색 A, MakeScheduleSheet 647-737 + SpreadSheetUtil + DB 실측): DB TEMP008(1010/ScmBlockMasterPlanMan) ST_ROW=7(0-based→엑셀 8행), ST_COL=5(코드 미사용), EXCEL_NAME `.\ExcelFile\2. 마스터 SCHEDULE.xlsx`, TAG3=SCHEDULE. DETAIL 48행 전부 OBJ_NAME=MASTER/USE_YN=Y/COLUMN_CELL 있음(F~CW), TAG2 DATE 19·NUMBER 4·NULL 25. 주입 규칙: 열=COLUMN_CELL 26진 변환(ALIS 문자열 오버로드는 버그, 워크시트 오버로드 사용), DATE→값만(서식은 템플릿: mm/dd, mm/dd aaa, m"/"d), NUMBER→double, 그 외→문자열 SetText(자동형변환 금지), 후처리는 데이터 범위 테두리(Thin Black)만. 매 조회 템플릿 디스크 재로드 = 초기화. 남는 행 방치(수식 IF 가드가 공백). 데이터 663건 = 템플릿 수식 행 8~670과 우연히 일치, 초과 시 서식 밖(ALIS 미처리).
- 템플릿: 시트 3개 중 소조1과/소조2과는 hidden(2010 데이터), SCHEDULE 활성. autoFilter A7:CW600, 틀고정 L8, 수식 15열 9,945셀, 조건부서식 8블록($G$4=TODAY() 의존, #REF! 2블록 원본 결함), 숨김열 A~E·CX~DD, 병합은 헤더 4~6행만.
- DB 접속 메모: Oracle.ManagedDataAccess는 Windows PowerShell 5.1 + `Pooling=false;Connection Timeout=30`로만 접속됨(pwsh 7은 타임아웃).

### Phase 2: 설계 — 완료 (Plan 에이전트)
- 라디오 `rblViewMode`(BootstrapRadioButtonList, AutoPostBack, 값은 컨트롤 ViewState) → Page_Load `RenderResult()`가 `gridSchedule.Visible`/`spreadsheet.Visible` 토글(Panel 없음).
- 문서 ID = `BlockSchedule_{Session.SessionID}_{ViewName}` (**DocumentManager는 앱 전역 캐시** → 세션ID 필수). 같은 ID로 `Open`하면 바이트 무시·재활성화만 → 재조회/ClearResult 시 `DocumentManager.CloseDocument` 필수(`ASPxSpreadsheet.Close()`는 obsolete). 그리드↔시트 왕복은 문서 유지. `Session_End`에서 접두 문서 정리. WorkDirectory 불필요.
- 주입 = 신규 `Utils/ScheduleSheetInjector.cs`(DevExpress.Spreadsheet만 의존): hidden 시트 2개 메모리 제거(수식 참조 0건) → SCHEDULE 활성 → OBJ_NAME=MASTER·COLUMN_CELL 26진 → DATE/NUMBER/문자열(`cell.Value = string`, SetValueFromText 금지) → 데이터 범위 Thin Black 테두리 → `wb.Calculate()`. 서식 미지정(템플릿 서식). 남는 행 방치, 초과 시 로그만.
- 엑셀 버튼: 시트 모드면 `spreadsheet.SaveCopy(DocumentFormat.Xlsx)` 바이트 응답(hidden 시트 제거된 상태), 그리드 모드는 기존.
- 파일: Web.config tagPrefix 1줄(+bindingRedirect 3개, 서버 GAC 충돌 방지 기존 패턴), csproj Content(템플릿 xlsx)+Compile(Injector), App_Data\ExcelTemplates\2. 마스터 SCHEDULE.xlsx 복사, 컨트롤러 SELECT에 OBJ_NAME + StRow DBNull 기본 2(ALIS ?? 2), aspx/designer/aspx.cs, Global.asax.cs Session_End.
- 알려진 제한: 같은 세션 다중 탭은 DocId 공유(한 탭 재조회가 다른 탭 문서를 닫음).

### Phase 3: 구현 — 완료 (빌드 오류 0·경고 0, aspnet_compiler 통과, ColumnLetterToIndex 실행검사 통과)
- 신규: Utils/ScheduleSheetInjector.cs(98줄), App_Data/ExcelTemplates/2. 마스터 SCHEDULE.xlsx(원본 md5 동일)
- 수정: Web.config(tagPrefix + bindingRedirect 3), csproj(Content+Compile), BlockScheduleController(OBJ_NAME, ST_ROW 기본 2), BlockSchedule.aspx(371줄: flex 헤더, 라디오, ASPxSpreadsheet), designer(+2), aspx.cs(415줄: IsSheetMode/DocId/RenderResult/ShowSpreadsheet/ResolveTemplatePath/CloseSheetDocument/엑셀 분기), Global.asax.cs(Session_End 문서 정리)
- 계획 외 판단: `spreadsheet.Visible = sheet && GridData != null` (조회 전 시트 모드에서 빈 문서 자동 생성·캐시 누적 방지)

### Phase 4: 리뷰 + 런타임 검증 — 완료
- C# 리뷰(ecc:csharp-reviewer): NEEDS_IMPROVEMENT. HIGH 2(① Page_Load가 이벤트 핸들러보다 먼저 RenderResult → 콤보 변경 시 ClearResult가 문서를 닫아도 spreadsheet.Visible=true로 남아 닫힌 문서 렌더 → 근본 수정: RenderResult를 Page_LoadComplete로 이동, ② Inject 예외 시 손상 문서가 캐시에 남음 → 실패 시 CloseDocument 후 rethrow), MEDIUM 2(콜백 중 ShowMessage 무효 → ShowMessageCallback 분기, DocumentId 신뢰성 → !cached만으로 게이팅), LOW 2(Response.End 관례, Content-Disposition 따옴표).
- 컨벤션 리뷰(team-critic): NEEDS_IMPROVEMENT. CRITICAL 1(위 ①과 동일), MEDIUM 1(콜백 ShowMessage), LOW 3(SqlLogger 오용 new Exception("rows"), 조회 전 시트 모드 안내 없음, 모바일 높이).
- 런타임(테스터, CDP headless Chrome): 시나리오 10개 중 8 PASS. 스프레드시트 = ALIS 레이아웃 재현(1행 열번호, 2행 제목, G4=TODAY, 5~6행 헤더, 8행부터 663건, mm/dd, 테두리, 수식 15열 계산값, 조건부서식 색상), 읽기전용 확인, 그리드↔시트 왕복 0.7초, 판접 재조회 317건 잔존 데이터 없음, 다운로드 xlsx는 SCHEDULE 시트 1개(hidden 제거)·663행·수식 캐시값 포함, view 변조 폴백, 모바일 400px OK.
  - FAIL ①: 시트 모드 Company 변경 → "Loading…" 고정 + DXS.ashx 404 + alert "Your session has expired"(리뷰 HIGH① 실증). FAIL ②: 같은 세션 다중 탭 — 탭B 조회가 탭A 문서를 닫아 스크롤마다 alert 반복(설계상 알려진 제한이나 증상이 심함 → 탭별 GUID로 DocId 분리 결정).
  - 성능: 첫 스프레드시트 렌더 11.5~15.1초(663건) — 주입 중 자동 재계산 의심 → BeginUpdate/EndUpdate 적용 후 재측정.
  - 참고: 재조회로 건수 감소 시 325~670행 테두리/채움 잔존 → 템플릿 자체 서식(8~670행 수식·조건부서식 `$G$4>빈셀`=TRUE)으로 추정, 확인 예정. 템플릿 없는 회사(100/1011) 조회 시 "템플릿 없음" 메시지는 1단계 설계대로 정상. autoFilter A7:CW600·G4 yyyy-mm-dd는 템플릿 원본 그대로.

### Phase 5: 수정 + 재검증 — 완료
- 수정: RenderResult → Page_LoadComplete(이벤트 처리 후 1회), ClearResult에 spreadsheet.Visible=false, ShowSpreadsheet는 항상 Open 후 미캐시 시만 Inject(실패 시 CloseDocument+rethrow), DocId에 ViewState GUID(탭별 문서 분리), Inject를 BeginUpdate/EndUpdate로 감싸고 Calculate, Notify(IsCallback→ShowMessageCallback)+그리드 EndCallback cpMessage alert, Content-Disposition 따옴표, SqlLogger.LogWarning 추가. 빌드 오류 0.
- 재검증(테스터): 6/6 PASS — 그리드 페이징·정렬 콜백 정상, 첫 스프레드시트 렌더 **15.1초 → 4.0초**, 시트 모드 Company 변경 시 오류 없이 비워지고 재조회 663건 정상, 판접 재조회 317건 잔존 값 없음, 다중 탭 각각 다른 문서 id로 콜백 200, 다운로드 SCHEDULE 1개 시트.
- 확인: 325~670행 테두리/채움은 템플릿 고유 서식(8~670행 동일 스타일 + 조건부서식 `$G$4>빈셀`=TRUE) — ALIS도 동일, 코드 변경 없음.
- 미커밋(사용자 지시 대기). 후속: 탭을 닫아도 문서는 Session_End까지 캐시(세션당 탭 수만큼), 데이터 663건 초과 시 템플릿 서식 밖(ALIS 동일), 나머지 8탭 활성화 시 각 템플릿 xlsx를 App_Data/ExcelTemplates에 추가 배포 필요.
- ALIS 세션 확인(ScmBlockMasterPlanMan.xaml.cs:647-656): MakeScheduleSheet는 ST_ROW 아래를 지우지 않고 **덮어쓰기만** 함(WPF는 매번 템플릿을 새로 열어 은폐됨) → 웹은 조회마다 템플릿을 새로 로드하거나 주입 전 데이터 영역 클리어. `dataSource.Count > 0` 가드로 0건이면 시트 미변경 → 웹은 0건을 명시 처리. ST_COL은 죽은 변수. 타입 변환 실패는 빈 catch → 웹은 로깅.
- 템플릿 `2. 마스터 SCHEDULE.xlsx`: 시트 3개(소조1과(9월), 소조2과(9월), SCHEDULE). SCHEDULE = A1:DD670, 108열, r1 열번호 행, r2 제목, r4 작성일, r5 헤더(개행 포함), r6~r670 빈 서식 행(샘플 데이터 없음) → ST_ROW(0-based)=5 추정, DB 확인 필요. **소조1과/소조2과 시트에 2010년 실데이터(163/157행, 업체명 포함) 잔존 → 웹 표시 시 반드시 제거/숨김.**
- ASPxSpreadsheet 통합(탐색 B): `DevExpress.Web.ASPxSpreadsheet.v25.2.dll` 등 필요 DLL 17개가 `DevExpress.Web.Office` 패키지(Web.Bootstrap/Web.Mvc 전이 의존)로 **이미 bin에 존재** → csproj 변경 불필요. **Web.config `<pages><controls>`에 tagPrefix 1줄만 추가**(namespace DevExpress.Web.ASPxSpreadsheet). 읽기전용 = `ReadOnly="True" RibbonMode="None" ShowFormulaBar="False"` + `<SettingsView Mode="Reading"/>`. API: `Open(documentId, DocumentFormat.Xlsx, Func<byte[]>)`, `Document`(IWorkbook), `Worksheets["SCHEDULE"]`, `Close()`, `SaveCopy(stream, format)`. 데모 `C:SERSPUBLICDOCUMENTSDEVEXPRESS DEMOS 25.2COMPONENTSASP.NETCSASPXSPREADSHEETDEMOS`. ̜�͗�: ̄�̓�̠� BINDINGREDIRECT 3ʰ�, CLEARRESULT̗� CLOSE ͕�̚�, OBJ_NAME̝� ̻�͊�ˡ�˟� SELECT̗� ˯�͏�͕�(̶�ʰ� ͕�̚�), APP_DATA ͏�ˍ� ̋�ˬ� ̗�̝�(CSPROJ FOLDER ̄�̖�˧�).
- ALIS ̗�ˬ�̞��͗�ˍ�˪� ˧�͕� ʰ�̄�: ˳�˥� ̜�̧�. 개선돼도 COLUMN_CELL은 하위호환 유지 → 웹은 COLUMN_CELL 사용.

## ALIS 블록제작 메인 탭 → 웹 조회 전용 화면 (1단계 그리드) — 2026-09-16
- Task Type: feature
- Team: Explorer×3(ALIS 화면/포털 패턴/9탭 매핑) → Plan(설계) → Implementer → Critic → Tester. ALIS 세션(excel-upload-highgrade-roadmap)과 크로스세션 협업.
- 계획 파일: C:\Users\mellass\.claude\plans\composed-squishing-fountain.md (승인됨)
- Key Decisions:
  - "금강중공업 블록제작" 전용 화면은 ALIS에 없음. "블록제작" = EBMS 워크스페이스(NodeId 2262), 탭 9개 = ScmBlock*PlanMan 9종(구조 동일).
  - 첫 화면 = 메인(공유) 탭(ScmBlockMasterPlanMan / SCM_BLOCK_MASTER_SCH), 조회 전용(조회조건+조회+엑셀). 웹은 Viewer만.
  - 표시 = 그리드 먼저(1단계), 라디오[그리드|스프레드시트] + ASPxSpreadsheet 읽기전용은 2단계.
  - 범용 페이지 1개 Views/BlockSchedule.aspx?view=... (허용목록 9뷰, 1단계 Master만 활성).
  - 그리드 컬럼은 SCM_EXCEL_TEMP_DETAIL 주도 런타임 생성(Page_Init 매 요청 재생성, EnableViewState=false). ALIS와 동일하게 USE_YN/OBJ_NAME 무필터.
  - 동적 SQL 조각 2곳(테이블명 TAG2, 기준일정 컬럼) 화이트리스트 검증 필수.
  - 데이터/컬럼정의 DataTable 세션 캐시. TEMP_NO/COLUMN_CELL/ST_ROW/EXCEL_NAME/TAG3 보존(2단계 셀 주입 키).
  - GetCaseList를 BaseController로 승격.
  - 가정: 1단계 "엑셀" = 그리드 내보내기(사용자 미확인).
  - ALIS 세션 이견 반영: 종료일은 ALIS와 동일하게 raw 바인드(00:00, 건수 일치). 2단계 시트명은 TAG3가 아닌 상수 "SCHEDULE"(블록 계열 ALIS가 TAG3 미사용). 2단계 주입은 OBJ_NAME='MASTER' 필터.
- Status: COMPLETE (1단계)

### Phase 1: 조사 — 완료 (상세는 계획 파일)

### Phase 2: 구현 — 완료 (빌드 오류 0·경고 0, aspnet_compiler 통과, 로직 자기검사 15/15)
- 신규: Views/BlockSchedule.aspx(340) / .aspx.cs(325) / .designer.cs(77), Controllers/BlockScheduleController.cs(260)
- 수정: BaseController(+GetCaseList), ResourceManagerController(-GetCaseList), csproj, Site.Master, ContractManagerController·ContractManagerXpoController(`new` 한정자만)
- 계획 외 판단: TAG2 Trim/Upper 후 검증, 기준일정 화이트리스트+SafeIdent 이중검사, 컬럼 폭 고정(DATE 100/NUMBER 90/텍스트 120), 고정컬럼 중복 제거, Guarded 핸들러 통일

### Phase 3: 리뷰 + 런타임 검증 — 완료
- C# 리뷰(ecc:csharp-reviewer): PASS. MEDIUM 1(정규식 앵커 A..z), LOW 3(PROP_NAME null 안전, Contract `new` 은폐, href 인코딩)
- 컨벤션 리뷰(team-critic): NEEDS_IMPROVEMENT. WARNING 2(ViewState 비활성 상태 검증 필요 → 런타임 PASS로 해소, ContractManagerController.GetCaseList 죽은 코드 삭제). 참고: ContractManagerXpoController 한글 주석 인코딩 깨짐(기존 문제, 범위 밖)
- 런타임(IIS Express 64bit PID 1178464, sjkim/1): 시나리오 9개 전부 PASS. (주)금강(1010)/생산진행관리 조회 663건, 50컬럼 5밴드, 기준일정 판접+2025-01-01 → 315건, 페이징/정렬/필터/페이지크기 콜백 후 컬럼 유지, 엑셀 69KB 정상, view 변조 폴백, 기준일정 변조는 콤보가 버려 "전체" 폴백(안전)
- 발견: 32bit IIS Express는 Oracle OCI BadImageFormatException → 반드시 64bit 사용. 밴드 순서가 ALIS(고정 선언)와 달리 첫 등장 순 → 수정 대상. bootstrap.bundle 5.1.3 콤보 ArrowDown 콘솔 오류(Site.Master 기존 문제)

### Phase 4: 수정 + 재검증 — 완료
- 수정: SafeIdent `A[A-Z0-9_]+z`, PROP_NAME null-safe, ContractManagerController.GetCaseList 삭제(호출자 0), 탭 href `<%#: %>` 인코딩, 콤보 변경/LoadData 진입 시 ClearResult, 표준 밴드 5개 ALIS 순서(기본정보→키정보→텍스트정보→일정정보→수치정보) 선생성 + 빈 밴드 제거. 빌드 오류 0.
- 재검증(테스터, 새 탭): 663건/50컬럼 동일, 밴드 순서 ALIS 일치, 페이징/정렬/필터 유지, Company 변경 시 그리드·라벨 초기화, 탭 href 정상 → 전부 PASS
- 미커밋 상태(사용자 지시 대기). 신규 4파일 1,016줄, 수정 6파일 +89/−85.
- 후속 과제: 2단계 스프레드시트(라디오 전환), 나머지 8탭 활성화, 메인(공유)↔메인입력 캡션 DB 확인(TCM_MENU), 날짜 컬럼 시간부 존재 시 종료일 처리 양쪽 개선, Site.Master bootstrap 5.1.3 콤보 ArrowDown 콘솔 오류(기존), ContractManagerXpoController 주석 인코딩(기존)

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
