# SCM Block Contract Web - 설치 및 사용 가이드

## 📋 목차
1. [시스템 요구사항](#시스템-요구사항)
2. [설치 방법](#설치-방법)
3. [프로젝트 구성](#프로젝트-구성)
4. [데이터베이스 설정](#데이터베이스-설정)
5. [실행 방법](#실행-방법)
6. [기능 설명](#기능-설명)
7. [문제 해결](#문제-해결)

---

## 시스템 요구사항

### 필수 소프트웨어
- **Visual Studio 2022** (Community, Professional, Enterprise 중 하나)
- **.NET Framework 4.8** SDK
- **IIS Express** (Visual Studio와 함께 설치됨)
- **Oracle Database** 접근 권한

### 필수 NuGet 패키지
- DevExpress.Web (v25.1.10)
- DevExpress.Web.Mvc (v25.1.10)

---

## 설치 방법

### 1. 프로젝트 열기
```bash
# 1. Visual Studio 2022 실행
# 2. 파일 > 열기 > 프로젝트/솔루션
# 3. ScmBlockContractWeb.sln 파일 선택
```

### 2. NuGet 패키지 복원
Visual Studio에서 자동으로 NuGet 패키지를 복원합니다. 수동으로 복원하려면:

```bash
# Package Manager Console에서 실행
Update-Package -reinstall
```

또는 Solution Explorer에서:
- 솔루션 우클릭 > "Restore NuGet Packages"

### 3. DevExpress 라이선스 등록
DevExpress 컴포넌트를 사용하려면 유효한 라이선스가 필요합니다.
- 평가판: https://www.devexpress.com/products/net/controls/asp/
- 라이선스가 없는 경우, 평가판 모드로 실행됩니다.


1. Web.config 수정:
```xml
<connectionStrings>
    <add name="OracleConnection"
		    connectionString="Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=HQ.SPELIX.CO.KR)(PORT=7744))(CONNECT_DATA=(SID=SPELIXDB)));User Id=amisys;Password=amisys1101;"
		    providerName="Oracle.ManagedDataAccess.Client" />
  </connectionStrings>
```

2. Default.aspx.cs 수정:
```csharp
using Oracle.ManagedDataAccess.Types;  // OracleDbType 사용
```

---

## 프로젝트 구성

```
ScmBlockContractWeb/
│
├── ScmBlockContractWeb.sln        # Visual Studio 솔루션 파일
├── ScmBlockContractWeb.csproj     # 프로젝트 파일
├── Web.config                      # 설정 파일 (DB 연결 문자열 포함)
├── README.md                       # 프로젝트 개요
├── INSTALLATION_GUIDE.md           # 이 파일
│
├── Default.aspx                    # 메인 UI 페이지
├── Default.aspx.cs                 # Code-behind (비즈니스 로직)
├── Default.aspx.designer.cs        # Designer 생성 파일
│
└── Models/
    └── ScmContractDetail.cs        # 데이터 모델 클래스
```

---

## 데이터베이스 설정

### 1. 연결 문자열 확인
`Web.config` 파일에서 Oracle 연결 문자열을 확인하세요:

```xml
<connectionStrings>
    <add name="OracleConnection"
		    connectionString="Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=HQ.SPELIX.CO.KR)(PORT=7744))(CONNECT_DATA=(SID=SPELIXDB)));User Id=amisys;Password=amisys1101;"
		    providerName="Oracle.ManagedDataAccess.Client" />
  </connectionStrings>
```

### 2. 데이터베이스 테이블 확인
필요한 테이블: `SCM_CONTRACT_DETAIL`

테이블 구조 예시 (최소 필수 컬럼):
```sql
CREATE TABLE SCM_CONTRACT_DETAIL (
    COMPANY_NO VARCHAR2(50),
    CASE_NO VARCHAR2(50),
    CONTRACT_ID VARCHAR2(100) PRIMARY KEY,
    CONTRACT_NAME VARCHAR2(200),
    CONTRACT_NO VARCHAR2(100),
    CNTR_DT DATE,
    MP_DT DATE,
    PROJECT_NO VARCHAR2(100),
    PRODUCT_TYPE VARCHAR2(100),
    CNTR_EA NUMBER(10,0),
    CNTR_WGT NUMBER(15,2),
    OWNER VARCHAR2(100),
    RMK VARCHAR2(500),
    -- 기타 컬럼들...
);
```

### 3. 샘플 데이터 삽입 (선택사항)
```sql
INSERT INTO SCM_CONTRACT_DETAIL (
    COMPANY_NO, CASE_NO, CONTRACT_ID, CONTRACT_NAME, CNTR_DT
) VALUES (
    '1002', 'MASTER', 'CNTR001', '테스트 계약', SYSDATE
);
COMMIT;
```

---

## 실행 방법

### Visual Studio에서 실행

1. **F5 키 또는 "시작" 버튼 클릭**
   - IIS Express가 자동으로 시작됩니다
   - 기본 브라우저가 열립니다

2. **실행 URL**
   ```
   http://localhost:52413/Default.aspx
   ```
   (포트 번호는 자동 할당되므로 다를 수 있습니다)

### IIS에 배포

1. **게시(Publish) 설정**
   - 프로젝트 우클릭 > "게시"
   - 대상: "폴더" 선택
   - 경로: 원하는 배포 폴더 지정

2. **IIS 설정**
   - IIS 관리자 실행
   - 사이트 추가 또는 기존 사이트에 애플리케이션 추가
   - 실제 경로: 게시한 폴더 지정
   - 애플리케이션 풀: .NET Framework v4.0 이상

3. **web.config 확인**
   - `<compilation debug="false">` 로 변경 (프로덕션 환경)

---

## 기능 설명

### 1. 조회 조건 설정
- **Company 구분**: 본사/지사 선택
- **Company**: 회사 선택 (기본값: 1002 - SPELIX)
- **케이스**: 케이스 선택 (기본값: MASTER)
- **전체 조회**: 체크 시 날짜 조건 무시
- **계약일 (시작/종료)**: 기간 검색

### 2. 조회 버튼
- 설정된 조건으로 데이터베이스 조회
- 결과를 그리드에 표시
- 총 건수, 수량 합계, 중량 합계 표시

### 3. 초기화 버튼
- 모든 조회 조건을 기본값으로 재설정
- 그리드 데이터 초기화

### 4. 엑셀 내보내기
- 현재 그리드의 데이터를 Excel 파일로 다운로드
- 파일명: `SCM_Contract_YYYYMMDD_HHmmss.xlsx`

### 5. 그리드 기능
- **필터링**: 각 컬럼 헤더에서 필터 아이콘 클릭
- **정렬**: 컬럼 헤더 클릭으로 오름차순/내림차순
- **페이징**: 페이지당 10/20/50/100건 또는 전체 표시
- **포커스 행**: 클릭한 행 하이라이트

---

## 문제 해결

### 1. DevExpress 컨트롤이 표시되지 않음
**증상**: 페이지가 비어있거나 오류 발생

**해결방법**:
```bash
# Package Manager Console에서 실행
Install-Package DevExpress.Web -Version 24.2.10
Update-Package DevExpress.Web -reinstall
```

Web.config에 다음이 포함되어 있는지 확인:
```xml
<pages>
  <controls>
    <add tagPrefix="dx" namespace="DevExpress.Web" 
         assembly="DevExpress.Web.v25.1, Version=25.1.6.0, ..." />
  </controls>
</pages>
```

### 2. Oracle 연결 오류
**증상**: "ORA-12154: TNS:could not resolve the connect identifier specified"

**해결방법**:
1. 네트워크 연결 확인
2. Oracle 서버 및 포트 확인
3. 방화벽 설정 확인
4. 연결 문자열 구문 확인

### 3. 빌드 오류
**증상**: "Could not load file or assembly 'DevExpress.Web.v25.1'"

**해결방법**:
1. NuGet 패키지 복원
2. 프로젝트 클린 및 리빌드
   ```
   빌드 > 솔루션 정리
   빌드 > 솔루션 다시 빌드
   ```

### 4. 데이터가 조회되지 않음
**체크리스트**:
- [ ] 데이터베이스 연결 성공 여부
- [ ] SCM_CONTRACT_DETAIL 테이블 존재 여부
- [ ] 테이블에 데이터 존재 여부
- [ ] WHERE 조건 확인 (CASE_NO, COMPANY_NO)
- [ ] 사용자 권한 확인 (SELECT 권한)

**디버깅**:
```csharp
// Default.aspx.cs의 LoadData() 메서드에 중단점 설정
// 쿼리 문자열 확인
// OracleDataReader에서 읽어온 데이터 확인
```

### 5. 엑셀 내보내기 오류
**증상**: "Unable to export to Excel"

**해결방법**:
- DevExpress.Printing.v25.1.Core 패키지 설치 확인
- 브라우저 팝업 차단 해제
- 파일 다운로드 권한 확인

---

## 추가 참고자료

### DevExpress 문서
- ASP.NET GridView: https://docs.devexpress.com/AspNet/3718/components/grid-view
- ASP.NET Controls: https://docs.devexpress.com/AspNet/4329/aspnet-webforms-controls

### Oracle 연결
- Oracle.ManagedDataAccess: https://www.oracle.com/database/technologies/appdev/dotnet/odp.html
- Devart dotConnect: https://www.devart.com/dotconnect/oracle/

### 기술 지원
- DevExpress Support: https://supportcenter.devexpress.com/
- Oracle Forums: https://community.oracle.com/

---

## 라이선스 정보

### DevExpress
- 상용 라이선스 필요 (또는 평가판 사용)
- 라이선스: https://www.devexpress.com/buy/net/

### 프로젝트 코드
- 내부 사용 프로젝트
- 수정 및 배포 가능

---

## 버전 히스토리

### v1.0.0 (2025-01-15)
- 초기 프로젝트 생성
- 기본 조회 기능 구현
- DevExpress 24.2.10 적용
- Oracle 데이터베이스 연동

---

## 문의
기술 지원이 필요하면 개발팀에 문의하세요.
