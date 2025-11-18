# 🔐 로그인 기능 가이드

## 📋 개요

DevExpress ASP.NET Web Forms 기반의 보안 강화 로그인 시스템입니다.

---

## ✨ 주요 기능

### 1. 로그인
- ✅ 회사 선택
- ✅ 아이디 / 암호 입력
- ✅ Enter 키로 로그인
- ✅ 세션 관리 (30분 타임아웃)

### 2. 보안
- ✅ **SQL Injection 방지** (파라미터화된 쿼리)
- ✅ **로그인 실패 제한** (5회 실패 시 계정 잠금)
- ✅ **입력값 검증** (정규식 사용)
- ✅ **세션 보안**

### 3. 사용자 경험
- ✅ 반응형 디자인
- ✅ 애니메이션 효과
- ✅ 명확한 오류 메시지
- ✅ 키보드 네비게이션 (Tab, Enter)

---

## 📂 파일 구조

```
ScmBlockContractWeb/
├── Login.aspx                  # 로그인 UI
├── Login.aspx.cs               # 로그인 로직
├── Login.aspx.designer.cs      # Designer
├── Logout.aspx                 # 로그아웃 페이지
├── Logout.aspx.cs              # 로그아웃 로직
└── Default.aspx                # 메인 페이지 (로그인 필수)
```

---

## 🗄️ 데이터베이스 테이블

### TCM_USER (사용자 테이블)

| 컬럼명 | 타입 | 설명 |
|--------|------|------|
| USER_ID | VARCHAR2(50) | 사용자 ID (PK) |
| USER_PW | VARCHAR2(100) | 비밀번호 |
| COMPANY_NO | VARCHAR2(128) | 회사번호 |
| DOMAIN_CATEGORY | VARCHAR2(50) | 도메인 카테고리 |
| IS_BLOCKED | CHAR(1) | 계정 잠금 여부 (Y/N) |
| BLOCKED_REASON | VARCHAR2(200) | 잠금 사유 |
| TAG05 | VARCHAR2(10) | 로그인 실패 횟수 |

### STD_COMPANY_MASTER (회사 테이블)

| 컬럼명 | 타입 | 설명 |
|--------|------|------|
| COMPANY_NO | VARCHAR2(128) | 회사번호 (PK) |
| COMPANY_NAME | VARCHAR2(200) | 회사명 |
| COMPANY_TYPE | VARCHAR2(10) | 회사 유형 |
| VIEW_ORDER | NUMBER | 정렬 순서 |
| USE_YN | CHAR(1) | 사용 여부 (Y/N) |

---

## 🔒 보안 기능 상세

### 1. SQL Injection 방지

**❌ 잘못된 방법 (취약):**
```csharp
string query = $"SELECT * FROM TCM_USER WHERE USER_ID = '{userId}' AND USER_PW = '{password}'";
```

**✅ 올바른 방법 (안전):**
```csharp
string query = "SELECT * FROM TCM_USER WHERE USER_ID = :USER_ID AND USER_PW = :USER_PW";
cmd.Parameters.Add("USER_ID", OracleDbType.Varchar2, 50).Value = userId;
cmd.Parameters.Add("USER_PW", OracleDbType.Varchar2, 100).Value = password;
```

### 2. 입력값 검증

```csharp
// 아이디: 영문, 숫자, _, - 만 허용 (3-50자)
private bool IsValidUserId(string userId)
{
    return Regex.IsMatch(userId, @"^[a-zA-Z0-9_-]{3,50}$");
}
```

### 3. 로그인 실패 제한

```csharp
private const int MAX_LOGIN_FAIL_COUNT = 5;

// 로그인 실패 시
int failCount = IncrementLoginFailCount(userId, companyNo);
if (failCount >= MAX_LOGIN_FAIL_COUNT)
{
    BlockUser(userId, companyNo, "비밀번호 5회 이상 실패");
}
```

### 4. 세션 보안

```csharp
// 로그인 성공 시
Session["UserID"] = userId;
Session["CompanyNo"] = companyNo;
Session["LoginTime"] = DateTime.Now;
Session.Timeout = 30; // 30분

// 모든 페이지에서 체크
if (Session["UserID"] == null)
{
    Response.Redirect("Login.aspx");
}
```

---

## 🎨 UI 디자인

### 디자인 특징
- **그라데이션 배경** (보라색 계열)
- **중앙 정렬 카드** 디자인
- **애니메이션** (슬라이드업, 쉐이크)
- **반응형** 레이아웃
- **DevExpress 테마** 적용

### 색상 팔레트
- Primary: `#667eea` → `#764ba2` (그라데이션)
- Background: White
- Error: `#c33`
- Text: `#333`

---

## 🔄 사용자 흐름

```
┌─────────────┐
│ Login.aspx  │ ← 시작점
└──────┬──────┘
       │
       ├─ 입력값 검증
       │   ├─ 회사 선택 필수
       │   ├─ 아이디 형식 검증
       │   └─ 암호 입력 필수
       │
       ├─ DB 인증
       │   ├─ 파라미터화된 쿼리
       │   ├─ 계정 잠금 확인
       │   └─ 비밀번호 검증
       │
       ├─ 로그인 실패
       │   ├─ 실패 횟수 증가
       │   ├─ 5회 실패 → 계정 잠금
       │   └─ 오류 메시지 표시
       │
       └─ 로그인 성공
           ├─ 세션 생성
           ├─ 실패 횟수 초기화
           └─ Default.aspx로 리디렉션
               │
               ├─ 세션 체크
               ├─ 사용자 정보 표시
               └─ 로그아웃 버튼
```

---

## 💻 사용 방법

### 1. 로그인

1. 브라우저에서 `http://서버주소/Login.aspx` 접속
2. **회사** 선택 (드롭다운)
3. **아이디** 입력
4. **암호** 입력
5. **로그인** 버튼 클릭 또는 **Enter** 키

### 2. 로그아웃

- 메인 페이지 우측 상단 **로그아웃** 버튼 클릭

### 3. 세션 타임아웃

- 30분 동안 활동이 없으면 자동 로그아웃
- 다시 로그인 필요

---

## 🐛 문제 해결

### "아이디, 암호를 다시 입력하세요"

**원인:**
- 잘못된 아이디 또는 암호
- 존재하지 않는 사용자

**해결:**
1. 대소문자 확인
2. 회사 선택 확인
3. DB에 사용자 존재 여부 확인

### "계정이 잠겨 있습니다"

**원인:**
- 5회 이상 로그인 실패
- 관리자가 수동으로 잠금

**해결:**
```sql
-- 계정 잠금 해제 (관리자용)
UPDATE TCM_USER 
SET IS_BLOCKED = 'N', 
    BLOCKED_REASON = '', 
    TAG05 = '0' 
WHERE USER_ID = '사용자ID' 
  AND COMPANY_NO = '회사번호';
COMMIT;
```

### "DB연결을 확인해 주세요"

**원인:**
- Oracle 서버 연결 실패
- 잘못된 연결 문자열

**해결:**
1. Web.config 연결 문자열 확인
2. Oracle 서버 상태 확인
3. 방화벽 포트 확인 (7744)

---

## 🔧 설정

### Web.config

```xml
<connectionStrings>
    <add name="OracleConnection" 
         connectionString="Data Source=(...);User Id=amisys;Password=amisys1101;" 
         providerName="Oracle.ManagedDataAccess.Client" />
</connectionStrings>

<system.web>
    <sessionState timeout="30" mode="InProc" />
</system.web>
```

### 로그인 실패 제한 변경

**Login.aspx.cs:**
```csharp
// 5회에서 다른 값으로 변경
private const int MAX_LOGIN_FAIL_COUNT = 5; // 원하는 값으로 변경
```

### 세션 타임아웃 변경

**Default.aspx.cs:**
```csharp
Session.Timeout = 30; // 분 단위 (원하는 값으로 변경)
```

---

## 📊 테스트 데이터

### 테스트용 사용자 생성

```sql
-- 테스트 회사 생성
INSERT INTO STD_COMPANY_MASTER 
(COMPANY_NO, COMPANY_NAME, COMPANY_TYPE, VIEW_ORDER, USE_YN)
VALUES 
('1002', 'SPELIX', 'H', 1, 'Y');

-- 테스트 사용자 생성
INSERT INTO TCM_USER 
(USER_ID, USER_PW, COMPANY_NO, DOMAIN_CATEGORY, IS_BLOCKED, TAG05)
VALUES 
('test', 'test123', '1002', 'SCM', 'N', '0');

COMMIT;
```

### 테스트 시나리오

1. **정상 로그인**
   - 회사: 1002 - SPELIX
   - 아이디: test
   - 암호: test123
   - 결과: ✅ 성공

2. **잘못된 암호 (5회)**
   - 암호: wrong123
   - 결과: ❌ 계정 잠금

3. **잠긴 계정**
   - 결과: ❌ "계정이 잠겨 있습니다"

---

## 🛡️ 보안 체크리스트

배포 전 확인사항:

- [ ] SQL Injection 방지 (파라미터화된 쿼리)
- [ ] 입력값 검증 (정규식)
- [ ] 로그인 실패 제한 (5회)
- [ ] 세션 타임아웃 설정 (30분)
- [ ] HTTPS 사용 (SSL 인증서)
- [ ] 연결 문자열 암호화
- [ ] 상세 오류 숨김 (customErrors="RemoteOnly")
- [ ] 비밀번호 평문 저장 금지 (해싱 권장)

---

## 🚀 추가 개선 사항 (선택)

### 1. 비밀번호 해싱

현재는 평문 저장. 보안 강화를 위해 해싱 추천:

```csharp
using System.Security.Cryptography;
using System.Text;

public static string HashPassword(string password)
{
    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
```

### 2. CAPTCHA 추가

로봇 공격 방지:
- Google reCAPTCHA
- DevExpress Captcha

### 3. 2단계 인증

추가 보안:
- SMS 인증
- 이메일 인증
- OTP (One-Time Password)

### 4. 로그인 이력

감사 추적:
```sql
CREATE TABLE TCM_LOGIN_HISTORY (
    LOGIN_ID NUMBER PRIMARY KEY,
    USER_ID VARCHAR2(50),
    COMPANY_NO VARCHAR2(128),
    LOGIN_DT DATE,
    LOGIN_IP VARCHAR2(50),
    SUCCESS_YN CHAR(1)
);
```

---

## 📞 지원

문제가 발생하면:
1. 이벤트 뷰어 확인 (Windows 로그 > 응용 프로그램)
2. IIS 로그 확인 (`C:\inetpub\logs\LogFiles\`)
3. 개발팀 문의

---

**로그인 기능이 성공적으로 구현되었습니다! 🎉**
