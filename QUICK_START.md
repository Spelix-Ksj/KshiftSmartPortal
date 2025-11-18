# 🚀 빠른 시작 가이드

## 5분 안에 실행하기

### 1단계: 프로젝트 열기 (30초)
```bash
1. Visual Studio 2022 실행
2. 파일 > 열기 > 프로젝트/솔루션
3. ScmBlockContractWeb.sln 선택
```

### 2단계: NuGet 패키지 복원 (2분)
```bash
# Visual Studio에서 자동으로 복원됩니다.
# 또는 Package Manager Console에서:
Update-Package -reinstall
```

**중요**: DevExpress 24.2.10 라이선스 필요
- 평가판: https://www.devexpress.com/products/try/
- 라이선스 없으면 평가판 배너가 표시됩니다

### 3단계: 데이터베이스 연결 확인 (1분)
`Web.config` 파일 확인:
```xml
<connectionStrings>
    <add name="OracleConnection" 
         connectionString="server=HQ.SPELIX.CO.KR:7744; user id=amisys; password=amisys1101; sid=SPELIXDB; ..." 
         providerName="Devart.Data.Oracle" />
</connectionStrings>
```

**Oracle 접속 불가 시**: 
- 샘플 데이터로 테스트하려면 아래 "오프라인 모드" 참조

### 4단계: 실행 (30초)
```bash
F5 키 누르기 또는 "시작" 버튼 클릭
```

브라우저가 자동으로 열립니다:
```
http://localhost:52413/Default.aspx
```

### 5단계: 데이터 조회 (30초)
1. 조회 조건 확인 (기본값: CASE_NO='MASTER', COMPANY_NO='1002')
2. **조회** 버튼 클릭
3. 결과 확인

---

## 오프라인 모드 (Oracle 없이 테스트)

Oracle DB가 없을 때 샘플 데이터로 테스트하는 방법:

### Default.aspx.cs 수정
`LoadData()` 메서드를 다음과 같이 수정:

```csharp
private void LoadData()
{
    // 샘플 데이터 생성
    List<ScmContractDetail> contractList = new List<ScmContractDetail>
    {
        new ScmContractDetail
        {
            COMPANY_NO = "1002",
            CASE_NO = "MASTER",
            CONTRACT_ID = "CNTR001",
            CONTRACT_NAME = "샘플 계약 1",
            CONTRACT_NO = "C2025-001",
            CNTR_DT = DateTime.Now.AddMonths(-3),
            MP_DT = DateTime.Now.AddMonths(3),
            PROJECT_NO = "PRJ2025-001",
            PRODUCT_TYPE = "Steel Block",
            CNTR_EA = 100,
            CNTR_WGT = 1500.50m,
            OWNER = "현대중공업",
            RMK = "테스트 데이터"
        },
        new ScmContractDetail
        {
            COMPANY_NO = "1002",
            CASE_NO = "MASTER",
            CONTRACT_ID = "CNTR002",
            CONTRACT_NAME = "샘플 계약 2",
            CONTRACT_NO = "C2025-002",
            CNTR_DT = DateTime.Now.AddMonths(-2),
            MP_DT = DateTime.Now.AddMonths(4),
            PROJECT_NO = "PRJ2025-002",
            PRODUCT_TYPE = "Engine Parts",
            CNTR_EA = 50,
            CNTR_WGT = 800.25m,
            OWNER = "삼성중공업",
            RMK = "샘플 데이터 2"
        }
    };

    // 그리드에 데이터 바인딩
    gridContracts.DataSource = contractList;
    gridContracts.DataBind();

    lblRecordCount.Text = $"총 {contractList.Count}건의 데이터가 조회되었습니다.";
}
```

---

## 자주 발생하는 문제

### ❌ 문제: DevExpress 컨트롤이 보이지 않음
**해결**: NuGet 패키지 재설치
```bash
Install-Package DevExpress.Web -Version 24.2.10
```

### ❌ 문제: Oracle 연결 실패
**해결 옵션 1**: 연결 문자열 확인
```xml
connectionString="server=서버주소:포트; user id=사용자명; password=암호; sid=SID명; ..."
```

**해결 옵션 2**: Oracle.ManagedDataAccess 사용 (무료)
```bash
Install-Package Oracle.ManagedDataAccess
```

그리고 코드 수정:
```csharp
using Oracle.ManagedDataAccess.Client;  // Devart 대신
```

### ❌ 문제: 빌드 오류
**해결**:
```bash
빌드 > 솔루션 정리
빌드 > 솔루션 다시 빌드
```

---

## 다음 단계

1. ✅ 프로젝트 실행 완료
2. 📝 `INSTALLATION_GUIDE.md` 읽기 (상세 가이드)
3. 🔧 실제 환경에 맞게 커스터마이징
4. 🚀 IIS에 배포 (프로덕션 환경)

---

## 핵심 파일 위치

```
ScmBlockContractWeb/
│
├── 📄 Default.aspx              ← UI (여기서 디자인 수정)
├── 📄 Default.aspx.cs           ← 비즈니스 로직 (여기서 코드 수정)
├── 📄 Web.config                ← 설정 (DB 연결 문자열 수정)
└── 📁 Models/
    └── ScmContractDetail.cs     ← 데이터 모델 (필드 추가/수정)
```

---

## 지원

- 📖 상세 가이드: `INSTALLATION_GUIDE.md`
- 🌐 DevExpress 문서: https://docs.devexpress.com/AspNet
- 💬 문의: 개발팀

---

**Happy Coding! 🎉**
