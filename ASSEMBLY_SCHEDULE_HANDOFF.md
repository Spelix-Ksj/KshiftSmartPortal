# 조립 탭 작업 기록 — 2026-09-20

상태: 로컬 구현·DB/브라우저 검증 완료, 미커밋.
기존 decisions.md는 잘못된 UTF-8 바이트가 있어 원문을 보존하고 이 파일에 후속 기록을 작성했다.

## 구현 및 DB 설정

- Claude 기록의 다음 탭 순서에 따라 `ScmBlockAssPlanMan` 조립 탭 활성화.
- ViewDefs Enabled=true, csproj Content에 `App_Data/ExcelTemplates/2. 조립 SCHEDULE.xlsx` 등록.
- 기존 정렬 `NVL(PROP01,'999'), PROJECT_NO, ITM_COD` 사용. 공용 조회·주입 로직 변경 없음.
- DB 실측: 회사 1010 / 케이스 ING, TEMP005, TAG2=SCM_BLOCK_ASS_WS, ST_ROW=11(엑셀 12행), ST_COL=2. DETAIL 36개 전부 MASTER, C~BO열. 388건.
- 원본: `D:/Dev/ALIS/trunk/bin/Debug/ExcelFile/2. 조립 SCHEDULE.xlsx`. SCHEDULE 시트 1개, A1:ABZ441. ALIS 원본은 수정하지 않았다.
- 낡은 주석 2곳 정리: ViewDefs 활성화 설명, ASPX의 RenderResult 실행 시점.

## 조립 템플릿 배포 처리

가공과 달리 실제 셀 외부 수식 993개가 존재한다. 외부 링크 파트만 제거하면 깨진 수식이 남으므로 `Tools/Prepare-AssemblyTemplate.py`로 먼저 외부 수식·계산 캐시를 비운다. 외부 파일에 의존하던 집계 셀은 빈 셀로 배포되며 웹에서 외부 파일을 갱신하지 않는다. 내부 수식과 서식은 유지한다.

- 삭제된 수식을 가리키는 calcChain 파트 및 관계/ContentType도 제거.
- 사용하지 않는 sharedStrings가 있으면 제외(이번 원본은 255개 모두 사용되어 그대로 유지).
- 이후 기존 `Tools/Sanitize-ExcelTemplate.ps1` 적용: 외부 링크 3개(파트 6개), 메모, 작성자, 저장 경로 등을 정리.
- 정리 스크립트에 임시 작업 폴더의 절대 경로 검증 추가.

재생성 순서(PowerShell 7):

```powershell
python KshiftSmartPortalWeb/Tools/Prepare-AssemblyTemplate.py 'D:/Dev/ALIS/trunk/bin/Debug/ExcelFile/2. 조립 SCHEDULE.xlsx' 'KshiftSmartPortalWeb/App_Data/ExcelTemplates/2. 조립 SCHEDULE.xlsx'
& KshiftSmartPortalWeb/Tools/Sanitize-ExcelTemplate.ps1 -Path 'KshiftSmartPortalWeb/App_Data/ExcelTemplates/2. 조립 SCHEDULE.xlsx'
```

원본 대비 검사 통과: 내부 수식 4,945개 본문/속성 동일, styles.xml·병합·이미지 동일. 외부 수식 993개 제거, externalLinks/comments/calcChain 없음, XML 전체 파싱 통과.

## 검증 결과

### DB 및 워크북

`Tools/Inspect-AssemblySchedule.ps1`은 Windows PowerShell 5.1에서 기존 Web.config 연결로 읽기 전용 조회한다. 비어 있지 않은 매핑 셀 11,947개를 DB와 날짜/숫자/문자 타입별로 대조하여 일치했고, 첫 행·마지막 행도 일치했다. 메모리 엑셀 저장/재로드 통과, 외부 워크북 참조 없음.

독립 PowerShell의 Workbook 구현은 설치된 `DevExpress.Docs.v25.2.dll`을 사용한다. 이 호스트에서는 웹 프로젝트 라이선스가 적용되지 않아 Evaluation Warning 시트가 추가된다. 검사용 호스트의 제한이며 실제 웹 다운로드는 SCHEDULE 1개 시트로 확인했다. 프로젝트 배포 DLL은 추가하지 않았다.

```powershell
powershell.exe -NoProfile -File KshiftSmartPortalWeb/Tools/Inspect-AssemblySchedule.ps1
```

### 브라우저

`Tools/Verify-AssemblyBrowser.py`는 Python Playwright와 설치된 Chrome을 사용한다. 계정은 `KSHIFT_TEST_USER` / `KSHIFT_TEST_PASSWORD` 환경변수로 전달하며 파일에 저장하지 않는다. `--expected-rows` 기본값은 388이다.

```powershell
python KshiftSmartPortalWeb/Tools/Verify-AssemblyBrowser.py --url http://localhost:52413 --expected-rows 388
```

통과한 시나리오:

- 조립 탭 활성 및 조회 388건, 그리드 페이징 콜백.
- 실제 시트 렌더링과 화면 확인, 그리드↔시트 왕복.
- 그리드 및 스프레드시트 엑셀 다운로드.
- 0건 재조회 시 시트 숨김·안내, 내보내기 F12의 이전 데이터 제거.
- 시트 모드 Company 변경 시 정상 초기화.
- 예상된 0건 안내 외 alert 및 JavaScript 오류 없음.

실제 다운로드: 약 1.72MB, SCHEDULE 1개 시트, 외부 링크·메모 없음. 따뜻한 로컬 실행에서 시트 표시 약 2.4초(고정 SLA 아님).

최종 MSBuild 성공(오류 0), `git diff --check` 통과. 기존 MSB3247(System.Text.Json 8.0.0.5↔8.0.0.6) 경고는 남아 있다.

## 다음 작업

PE → 도장 → 선적 → 셀가이드 → 메인입력(SCM) → 소조 순서. 각 템플릿은 외부 수식 여부를 먼저 검사해야 한다.
