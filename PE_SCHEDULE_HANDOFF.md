# PE 탭 작업 기록 — 2026-09-20

상태: 로컬 구현 완료, MSBuild 성공, 마크업 컴파일 스모크(302) 확인. 브라우저 로그인·조회 검증은 미실시(별도 담당). 미커밋.

## 구현

- `ScmBlockPEPlanMan` PE 탭 활성화. `Views/BlockSchedule.aspx.cs` 28행 ViewDefs `Enabled` `false → true`만 변경, 다른 행·다른 코드 변경 없음.
- csproj Content에 `App_Data\ExcelTemplates\2. PE SCHEDULE.xlsx` 등록(89행 조립 바로 아래).
- 공용 조회·주입 로직(`Utils/ScheduleSheetInjector.cs`), 정렬(`OrderByOverrides`) 변경 없음 — PE는 ORDER BY 예외가 없는 범용 케이스.

## DB 설정(회사 1010, 실측)

- TEMP004, TAG2=`SCM_BLOCK_PE_WS`, EXCEL_NAME=`.\ExcelFile\2. PE SCHEDULE.xlsx`
- ST_ROW=4, ST_COL=1
- DETAIL 20행 전부 OBJ_NAME=MASTER, USE_YN=Y, COLUMN_CELL 지정됨
- 데이터: CASE_NO='ING' 268건

## PE 템플릿 처리

원본(`D:\Dev\ALIS\trunk\bin\Debug\ExcelFile\2. PE SCHEDULE.xlsx`, 1,588,833 bytes) 조사 결과 **외부 수식 0건, xl/externalLinks 0개, definedNames 0건, comments/vmlDrawing 없음** — 조립 탭과 달리 외부 참조를 비우는 `Tools/Prepare-AssemblyTemplate.py` 전처리가 불필요하여 실행하지 않았다. `Tools/Sanitize-ExcelTemplate.ps1`만으로 충분했다.

Sanitize 결과(1회차, `-Backup` 없이 실행 — App_Data에 `.bak` 잔재를 남기지 않기 위함):

- externalLinks 파트 삭제: 0
- workbook.xml absPath: 1건 제거 (`D:\spelix\ALIS\trunk\bin\Debug\ExcelFile\`)
- core.xml creator/lastModifiedBy 비움: 2건 (creator=`도장팀_구상수`, lastModifiedBy=`sanghoon park` → 둘 다 빈 문자열)
- 메모/Relationship/Content_Types 제거: 0
- 총 3건 제거, 재압축 완료

2회차 재실행(멱등성 확인): 전 항목 0건, "변경 없음 — 파일 유지". 1회차 이후 MD5(`3667826eb506b4a3f21564ddd33edfe2`)와 2회차 이후 MD5가 동일함을 확인, `.bak` 파일 없음.

정리본(zip) 직접 확인(읽기 전용):

- `xl/externalLinks/` 없음
- `docProps/core.xml`의 `dc:creator`, `cp:lastModifiedBy` 빈 값
- `workbook.xml`에 `absPath` 없음
- 시트 `SCHEDULE` 1개(`xl/worksheets/sheet1.xml` 1개)
- 내부 수식(`<f>`) 개수: 원본 2,564개 = 정리본 2,564개 — 동일, 손상 없음

## 변경 파일 · 라인

| 파일 | 변경 내용 |
|---|---|
| `KshiftSmartPortalWeb/Views/BlockSchedule.aspx.cs` 28행 | ViewDefs PE `Enabled` `false → true` |
| `KshiftSmartPortalWeb/KshiftSmartPortalWeb.csproj` | 조립 Content(89행) 아래에 `<Content Include="App_Data\ExcelTemplates\2. PE SCHEDULE.xlsx" />` 1줄 추가 |
| `KshiftSmartPortalWeb/App_Data/ExcelTemplates/2. PE SCHEDULE.xlsx` | 신규 — ALIS 원본 복사 후 Sanitize 적용 (원본 MD5 `1CF152D4FB30FDE0BB5B9F2FE461A1A2` 일치 확인 후 처리, 처리 후 크기 1,588,833 → 1,327,724 bytes) |

## 빌드

```
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "KshiftSmartPortalWeb.csproj" /t:Build /p:Configuration=Debug /verbosity:minimal
```

CS 오류 0. 경고 1건(기존 MSB3247, System.Text.Json 8.0.0.5↔8.0.0.6 바인딩 충돌) — 조립 탭 작업 시점부터 존재하던 기존 경고로 무시 대상. `KShiftSmartPortal.dll` 출력 정상.

## 스모크 확인

```
curl -s -o /dev/null -w "%{http_code}" "http://localhost:52413/Views/BlockSchedule.aspx?view=ScmBlockPEPlanMan"
```

결과: `302`(로그인 리다이렉트 — 마크업 컴파일 성공 증거). IIS Express 프로세스는 건드리지 않았다.

## 남은 검증(별도 담당)

- 브라우저 로그인 후 PE 탭 실제 조회(예상 268건), 그리드 ↔ 시트 왕복, 엑셀 다운로드
- 0건 재조회 시 시트 숨김/안내 정상 동작
- alert·JS 콘솔 오류 없음 확인

## 다음 탭 순서

도장 → 선적 → 셀가이드 → 메인입력(SCM) → 소조. 각 템플릿은 외부 수식 여부를 먼저 검사해야 한다(PE는 0건이라 예외적으로 단순했음, 조립은 993건이라 `Prepare-AssemblyTemplate.py` 전처리가 필요했음 — 사전 조사 없이 바로 Sanitize만 돌리지 말 것).
