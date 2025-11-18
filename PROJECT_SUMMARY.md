# 🎉 SCM Block Contract Web 프로젝트 완성!

## 📦 프로젝트 개요

DevExpress 24.2.10을 사용한 ASP.NET Web Forms 기반의 SCM 계약 관리 웹 애플리케이션이 생성되었습니다.

---

## 📂 프로젝트 구조

```
ScmBlockContractWeb/
│
├── 📄 ScmBlockContractWeb.sln          # Visual Studio 솔루션 파일
├── 📄 ScmBlockContractWeb.csproj       # 프로젝트 파일
├── 📄 Web.config                        # 애플리케이션 설정 (DB 연결)
├── 📄 packages.config                   # NuGet 패키지 목록
│
├── 📄 Default.aspx                      # 메인 UI 페이지
├── 📄 Default.aspx.cs                   # Code-behind (비즈니스 로직)
├── 📄 Default.aspx.designer.cs          # Designer 자동 생성 파일
│
├── 📁 Models/
│   └── 📄 ScmContractDetail.cs         # 데이터 모델 클래스
│
├── 📁 Properties/
│   └── 📄 AssemblyInfo.cs              # 어셈블리 정보
│
├── 📄 README.md                         # 프로젝트 개요
├── 📄 QUICK_START.md                    # 빠른 시작 가이드 ⭐
└── 📄 INSTALLATION_GUIDE.md             # 상세 설치 가이드 ⭐
```

---

## 🚀 빠른 시작

### 1. Visual Studio 2022에서 열기
```
파일 > 열기 > 프로젝트/솔루션
ScmBlockContractWeb.sln 선택
```

### 2. NuGet 패키지 복원
Visual Studio가 자동으로 복원합니다. 또는:
```bash
# Package Manager Console
Update-Package -reinstall
```

### 3. 실행
```
F5 키 또는 "시작" 버튼 클릭
```

**상세 가이드**: `QUICK_START.md` 참조

---

## 🔧 주요 기능

### ✅ 구현된 기능
1. **조회 조건 설정**
   - Company 구분 (본사/지사)
   - Company 선택
   - 케이스 선택
   - 전체 조회 / 기간 조회
   - 계약일 범위 검색

2. **데이터 조회**
   - Oracle 데이터베이스 연동
   - SCM_CONTRACT_DETAIL 테이블 조회
   - 파라미터화된 쿼리 (SQL Injection 방지)
   - 동적 WHERE 조건 생성

3. **데이터 표시**
   - DevExpress GridView
   - 페이징 (10/20/50/100/전체)
   - 컬럼 필터링
   - 정렬 (오름차순/내림차순)
   - 총계 표시 (건수, 수량, 중량)
   - 컬럼별 색상 구분

4. **추가 기능**
   - 조회 조건 초기화
   - 엑셀 내보내기
   - 반응형 디자인

---

## 💾 데이터베이스 설정

### Oracle 연결 문자열 (Web.config)
```xml
<connectionStrings>
    <add name="OracleConnection" 
         connectionString="server=HQ.SPELIX.CO.KR:7744; 
                          user id=amisys; 
                          password=amisys1101; 
                          sid=SPELIXDB; 
                          Direct=true; 
                          Trim Fixed Char=false; 
                          Unicode=true;" 
         providerName="Devart.Data.Oracle" />
</connectionStrings>
```

### 조회 쿼리
```sql
SELECT 
    COMPANY_NO, CASE_NO, CONTRACT_ID, CONTRACT_NAME, 
    CONTRACT_NO, CNTR_DT, MP_DT, PROJECT_NO, 
    PRODUCT_TYPE, CNTR_EA, CNTR_WGT, OWNER, RMK
    -- 기타 컬럼들...
FROM 
    SCM_CONTRACT_DETAIL
WHERE 
    CASE_NO = :CASE_NO
    AND COMPANY_NO = :COMPANY_NO
    -- 선택적 날짜 조건
ORDER BY 
    CONTRACT_ID
```

---

## 📦 필수 패키지

### NuGet 패키지
```xml
<packages>
  <package id="DevExpress.Web" version="24.2.10" />
  <package id="DevExpress.Web.Mvc" version="24.2.10" />
  <package id="DevExpress.Data" version="24.2.10" />
  <package id="Devart.Data.Oracle" version="10.0.56" />
</packages>
```

### 대체 패키지 (무료)
Oracle 연결을 위해 Devart 대신 무료 패키지 사용 가능:
```bash
Install-Package Oracle.ManagedDataAccess.Core
```

---

## 🎨 UI 특징

### DevExpress Office365 테마
- 모던하고 깔끔한 인터페이스
- 반응형 레이아웃
- 직관적인 아이콘

### 색상 구분
- **파란색 헤더**: 기본 정보 (기업, 케이스, 계약ID)
- **주황색 헤더**: 계약 일반 정보
- **녹색 헤더**: 일정 정보
- **보라색 헤더**: 제품 정보

### 그리드 기능
- ✅ 자동 필터 행
- ✅ 컬럼 정렬
- ✅ 페이징
- ✅ 포커스 행 하이라이트
- ✅ 교차 행 색상
- ✅ 총계 표시

---

## 🔒 보안 기능

1. **SQL Injection 방지**
   - 파라미터화된 쿼리 사용
   - OracleParameter로 값 바인딩

2. **안전한 데이터 읽기**
   - Null 체크 헬퍼 메서드
   - try-catch 예외 처리

3. **입력 검증**
   - 날짜 형식 검증
   - ComboBox 값 제한

---

## 📚 제공된 문서

| 문서 | 설명 | 대상 |
|------|------|------|
| **QUICK_START.md** | 5분 빠른 시작 가이드 | 모든 사용자 ⭐ |
| **INSTALLATION_GUIDE.md** | 상세 설치 및 설정 가이드 | 개발자/관리자 |
| **README.md** | 프로젝트 개요 | 모든 사용자 |

---

## ⚙️ 커스터마이징 가이드

### 1. 조회 조건 추가
`Default.aspx`에 컨트롤 추가:
```xml
<dx:ASPxComboBox ID="cmbNewField" runat="server" />
```

`Default.aspx.cs`의 `BuildQuery()`에 WHERE 조건 추가:
```csharp
if (cmbNewField.Value != null)
{
    query += " AND NEW_FIELD = :NEW_FIELD";
}
```

### 2. 그리드 컬럼 추가
`Default.aspx`의 GridView Columns에 추가:
```xml
<dx:GridViewDataTextColumn FieldName="NEW_FIELD" 
                           Caption="새 필드" 
                           Width="150px" />
```

`ScmContractDetail.cs` 모델에 속성 추가:
```csharp
public string NEW_FIELD { get; set; }
```

### 3. DB 연결 변경
`Web.config`에서 connectionString 수정

### 4. 테마 변경
`Web.config`의 devExpress 섹션:
```xml
<themes theme="Material" />
<!-- 또는 "Office2019Colorful", "HighContrast" 등 -->
```

---

## 🐛 문제 해결

### DevExpress 평가판 배너
**증상**: "DevExpress Trial Version" 배너 표시

**해결**:
1. DevExpress 라이선스 구매: https://www.devexpress.com/buy/net/
2. 또는 평가판 계속 사용 (기능 제한 없음)

### Oracle 연결 실패
**증상**: "ORA-12154" 또는 연결 타임아웃

**해결**:
1. 네트워크 연결 확인
2. 방화벽 설정 확인 (포트 7744)
3. 연결 문자열 검증
4. 오프라인 모드로 테스트 (`QUICK_START.md` 참조)

### 빌드 오류
**해결**:
```bash
1. 빌드 > 솔루션 정리
2. 빌드 > 솔루션 다시 빌드
3. NuGet 패키지 복원 확인
```

---

## 🚀 다음 단계

### 개발 단계
1. ✅ 프로젝트 실행 확인
2. 📝 실제 데이터로 테스트
3. 🎨 UI 커스터마이징
4. 🔧 추가 기능 구현
   - 데이터 수정 기능
   - 삭제 기능
   - 상세 보기 팝업
   - 권한 관리

### 배포 단계
1. 📦 IIS 설정
2. 🔒 SSL 인증서 설치
3. 🗄️ 프로덕션 DB 연결
4. 📊 로깅 및 모니터링
5. 👥 사용자 교육

---

## 📞 지원 및 문의

### 문서
- 빠른 시작: `QUICK_START.md`
- 상세 가이드: `INSTALLATION_GUIDE.md`

### 온라인 리소스
- DevExpress 문서: https://docs.devexpress.com/AspNet
- DevExpress 지원: https://supportcenter.devexpress.com
- Oracle 문서: https://docs.oracle.com

### 커뮤니티
- DevExpress 포럼: https://supportcenter.devexpress.com/ticket/list
- Stack Overflow: #devexpress, #asp.net

---

## 📝 라이선스

### 상용 라이선스
- **DevExpress**: 연간 구독 또는 영구 라이선스
- **Devart.Data.Oracle**: 연간 구독 또는 영구 라이선스

### 무료 대안
- **Oracle.ManagedDataAccess**: 무료 (Oracle 공식)
- **ASP.NET Web Forms**: 무료 (.NET Framework 포함)

---

## ✅ 체크리스트

프로젝트 시작 전 확인사항:

- [ ] Visual Studio 2022 설치
- [ ] .NET Framework 4.8 SDK 설치
- [ ] DevExpress 라이선스 (또는 평가판)
- [ ] Oracle 데이터베이스 접근 권한
- [ ] 방화벽 포트 7744 개방 (Oracle)
- [ ] NuGet 패키지 복원 가능

프로젝트 실행 확인:

- [ ] 솔루션 파일 열림
- [ ] 빌드 성공
- [ ] 웹 페이지 표시
- [ ] 조회 버튼 동작
- [ ] 데이터 표시 확인

---

## 🎯 프로젝트 목표 달성!

✅ **DevExpress 24.2.10** 적용
✅ **ASP.NET Web Forms** 구조
✅ **Oracle 데이터베이스** 연동
✅ **조회 조건** 및 **조회 버튼** 구현
✅ **GridView** 데이터 표시
✅ **엑셀 내보내기** 기능
✅ **Visual Studio 2022** 프로젝트

---

**프로젝트가 성공적으로 생성되었습니다! 🎉**

다음 문서를 참조하여 시작하세요:
1. 📖 **QUICK_START.md** - 5분 빠른 시작
2. 📚 **INSTALLATION_GUIDE.md** - 상세 설정 가이드

Happy Coding! 💻✨
