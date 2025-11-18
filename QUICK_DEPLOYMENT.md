# 🚀 배포 빠른 가이드 (5단계)

웹서버에 배포하는 가장 빠른 방법입니다.

---

## 📋 사전 준비 (10분)

### Windows Server에서:

1. **관리자 권한 PowerShell 열기**
2. **IIS 설치 (한 번만):**
   ```powershell
   Install-WindowsFeature -Name Web-Server -IncludeManagementTools
   Install-WindowsFeature -Name Web-Asp-Net45
   ```

3. **.NET Framework 4.8 설치:**
   - https://dotnet.microsoft.com/download/dotnet-framework/net48

---

## 🎯 5단계 배포

### 1단계: Visual Studio에서 게시 (2분)

1. 솔루션 탐색기에서 프로젝트 우클릭
2. **게시** 클릭
3. **대상**: 폴더
4. **위치**: `C:\Publish\ScmBlockContractWeb`
5. **구성**: Release
6. **게시** 클릭

### 2단계: Web.config 수정 (1분)

**C:\Publish\ScmBlockContractWeb\Web.config 열기:**

```xml
<system.web>
  <!-- ⭐ 이것만 수정 -->
  <compilation debug="false" targetFramework="4.8">
  
  <!-- ⭐ 이것 추가 -->
  <customErrors mode="RemoteOnly" />
</system.web>
```

### 3단계: 파일을 서버로 복사 (3분)

**서버 경로 (예시):**
```
D:\WebApps\ScmBlockContractWeb\
```

**복사 방법:**
- USB 드라이브
- 네트워크 공유
- 원격 데스크톱

### 4단계: 자동 배포 스크립트 실행 (1분)

**관리자 권한 PowerShell에서:**

```powershell
# 스크립트가 있는 폴더로 이동
cd C:\Publish

# 실행 정책 변경 (한 번만)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# 배포 스크립트 실행
.\Deploy-ScmWeb.ps1
```

**스크립트가 자동으로 수행:**
- ✅ IIS 설치 확인
- ✅ 배포 폴더 생성
- ✅ 파일 복사
- ✅ 응용 프로그램 풀 생성
- ✅ 웹사이트 생성
- ✅ 권한 설정
- ✅ 방화벽 포트 개방

### 5단계: 접속 테스트 (1분)

**브라우저에서:**
```
http://서버IP주소
또는
http://scm.spelix.co.kr
```

---

## ✅ 완료!

배포가 성공하면 로그인 화면이 나타납니다.

---

## 🔒 HTTPS 설정 (선택사항, +10분)

### 옵션 1: Let's Encrypt (무료)

1. **win-acme 다운로드:**
   - https://github.com/win-acme/win-acme/releases

2. **관리자 권한 cmd에서 실행:**
   ```cmd
   wacs.exe
   ```

3. **메뉴 선택:**
   ```
   N > 4 > 1
   ```

4. **도메인 입력:**
   ```
   scm.spelix.co.kr
   ```

### 옵션 2: 정식 SSL 인증서

1. **IIS 관리자 > 서버 인증서**
2. **인증서 요청 만들기**
3. **CSR 생성 후 CA에 제출**
4. **발급받은 인증서 설치**

### HTTPS 리디렉션

**Web.config에 추가:**
```xml
<system.webServer>
  <rewrite>
    <rules>
      <rule name="HTTP to HTTPS" stopProcessing="true">
        <match url="(.*)" />
        <conditions>
          <add input="{HTTPS}" pattern="off" />
        </conditions>
        <action type="Redirect" url="https://{HTTP_HOST}/{R:1}" />
      </rule>
    </rules>
  </rewrite>
</system.webServer>
```

**URL Rewrite 모듈 설치:**
- https://www.iis.net/downloads/microsoft/url-rewrite

---

## 🐛 문제 해결

### "사이트에 연결할 수 없음"

```powershell
# 방화벽 확인
Get-NetFirewallRule | Where-Object {$_.DisplayName -like "*HTTP*"}

# IIS 서비스 확인
Get-Service W3SVC
Start-Service W3SVC

# 포트 확인
netstat -ano | findstr :80
```

### "500 오류"

**Web.config 임시 수정 (오류 확인용):**
```xml
<customErrors mode="Off" />
```

**이벤트 뷰어 확인:**
```
시작 > 이벤트 뷰어 > Windows 로그 > 응용 프로그램
```

### "Oracle 연결 오류"

```powershell
# Oracle 서버 연결 테스트
Test-NetConnection -ComputerName HQ.SPELIX.CO.KR -Port 7744
```

**bin 폴더에 Oracle.ManagedDataAccess.dll 있는지 확인**

---

## 📞 더 자세한 정보

**전체 가이드 참조:**
- `IIS_DEPLOYMENT_GUIDE.md` - 완전한 배포 가이드
- `Deploy-ScmWeb.ps1` - 자동 배포 스크립트

---

## 🎯 요약

```
1. Visual Studio에서 게시
2. Web.config 수정 (debug=false)
3. 파일을 서버로 복사
4. Deploy-ScmWeb.ps1 실행
5. 브라우저에서 테스트
```

**총 소요 시간: 약 10분** ⏱️

성공적인 배포를 기원합니다! 🚀
