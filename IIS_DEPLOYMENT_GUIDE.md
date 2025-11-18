# 🌐 IIS 웹서버 배포 가이드

## 📋 목차
1. [사전 준비사항](#사전-준비사항)
2. [IIS 설치 및 설정](#iis-설치-및-설정)
3. [애플리케이션 배포](#애플리케이션-배포)
4. [SSL 인증서 설정](#ssl-인증서-설정)
5. [보안 설정](#보안-설정)
6. [성능 최적화](#성능-최적화)
7. [문제 해결](#문제-해결)

---

## 사전 준비사항

### 서버 사양 권장
- **OS**: Windows Server 2016/2019/2022 또는 Windows 10/11 Pro
- **CPU**: 4 Core 이상
- **RAM**: 8GB 이상
- **디스크**: 50GB 이상 여유 공간
- **.NET Framework**: 4.8 설치 필요

### 필요한 정보 준비
- [ ] 서버 IP 주소 또는 도메인 이름
- [ ] SSL 인증서 (HTTPS 사용 시)
- [ ] Oracle DB 접속 정보
- [ ] 관리자 권한 계정

---

## IIS 설치 및 설정

### 1단계: IIS 기능 설치

#### Windows Server의 경우:

**PowerShell (관리자 권한):**
```powershell
# IIS 및 관련 기능 설치
Install-WindowsFeature -Name Web-Server -IncludeManagementTools
Install-WindowsFeature -Name Web-Asp-Net45
Install-WindowsFeature -Name Web-Windows-Auth
Install-WindowsFeature -Name Web-Filtering
Install-WindowsFeature -Name Web-Mgmt-Console

# 설치 확인
Get-WindowsFeature | Where-Object {$_.Installed -eq $True -and $_.Name -like "Web-*"}
```

#### Windows 10/11 Pro의 경우:

1. **제어판 > 프로그램 > Windows 기능 켜기/끄기**
2. 다음 항목 체크:
   - ✅ 인터넷 정보 서비스
   - ✅ 웹 관리 도구 > IIS 관리 콘솔
   - ✅ World Wide Web 서비스
     - ✅ 응용 프로그램 개발 기능 > ASP.NET 4.8
     - ✅ 일반 HTTP 기능 (모두)
     - ✅ 보안 > Windows 인증
   - ✅ .NET Framework 4.8 고급 서비스

3. **확인** 클릭 → 재부팅 필요 시 재부팅

### 2단계: .NET Framework 4.8 확인

**확인 방법:**
```powershell
# PowerShell에서 실행
Get-ChildItem 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\' | 
    Get-ItemPropertyValue -Name Release | 
    ForEach-Object { [version]"$($_)" }
```

출력이 **4.8 이상**이면 OK

**없으면 설치:**
- https://dotnet.microsoft.com/download/dotnet-framework/net48

### 3단계: IIS 기본 설정

**IIS 관리자 열기:**
```
시작 > "IIS" 검색 > 인터넷 정보 서비스(IIS) 관리자
```

**응용 프로그램 풀 설정:**
1. 왼쪽 트리에서 **응용 프로그램 풀** 선택
2. **DefaultAppPool** 우클릭 > **고급 설정**
3. 다음 설정 확인/변경:
   ```
   .NET CLR 버전: v4.0
   관리되는 파이프라인 모드: 통합
   32비트 응용 프로그램 사용: False
   유휴 시간 제한(분): 20 (기본값)
   ```

---

## 애플리케이션 배포

### 1단계: 프로젝트 게시 (Visual Studio)

1. **솔루션 탐색기**에서 프로젝트 우클릭
2. **게시** 선택
3. **대상**: 폴더 선택
4. **위치**: `C:\Publish\ScmBlockContractWeb` (예시)
5. **구성**:
   ```
   구성: Release
   대상 프레임워크: .NET Framework 4.8
   배포 모드: 프레임워크 종속
   대상 런타임: win-x64
   ```
6. **게시** 클릭

### 2단계: 게시된 파일 서버로 복사

**서버의 배포 경로 예시:**
```
D:\WebApps\ScmBlockContractWeb\
```

**복사할 내용:**
- 게시 폴더의 모든 파일과 폴더
- `bin/` 폴더 포함
- `Web.config` 포함

### 3단계: Web.config 프로덕션 설정

**D:\WebApps\ScmBlockContractWeb\Web.config 수정:**

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <!-- 기존 configSections는 그대로 -->
  
  <connectionStrings>
    <add name="OracleConnection" 
         connectionString="Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=HQ.SPELIX.CO.KR)(PORT=7744))(CONNECT_DATA=(SID=SPELIXDB)));User Id=amisys;Password=amisys1101;" 
         providerName="Oracle.ManagedDataAccess.Client" />
  </connectionStrings>

  <appSettings>
    <add key="vs:EnableBrowserLink" value="false" />
  </appSettings>

  <system.web>
    <!-- ⭐ 중요: 프로덕션에서는 debug를 false로 -->
    <compilation debug="false" targetFramework="4.8">
      <!-- assemblies는 그대로 -->
    </compilation>
    
    <!-- ⭐ 보안: 상세 오류 숨기기 -->
    <customErrors mode="RemoteOnly" defaultRedirect="~/Error.aspx">
      <error statusCode="404" redirect="~/NotFound.aspx" />
      <error statusCode="500" redirect="~/Error.aspx" />
    </customErrors>
    
    <!-- ⭐ 세션 타임아웃 설정 (분) -->
    <sessionState timeout="30" />
    
    <httpRuntime targetFramework="4.8" 
                 maxRequestLength="4096" 
                 requestValidationMode="4.0" 
                 executionTimeout="110" 
                 enableVersionHeader="false" />
    
    <!-- 나머지 설정은 그대로 -->
  </system.web>

  <system.webServer>
    <!-- ⭐ HTTP 헤더 보안 -->
    <httpProtocol>
      <customHeaders>
        <add name="X-Frame-Options" value="SAMEORIGIN" />
        <add name="X-Content-Type-Options" value="nosniff" />
        <add name="X-XSS-Protection" value="1; mode=block" />
        <remove name="X-Powered-By" />
      </customHeaders>
    </httpProtocol>
    
    <!-- 나머지 설정은 그대로 -->
  </system.webServer>
  
  <!-- devExpress 설정은 그대로 -->
</configuration>
```

### 4단계: IIS에 웹사이트 생성

**IIS 관리자에서:**

1. **사이트** 우클릭 > **웹 사이트 추가**

2. **기본 정보 입력:**
   ```
   사이트 이름: ScmBlockContractWeb
   응용 프로그램 풀: DefaultAppPool (또는 새로 만든 풀)
   실제 경로: D:\WebApps\ScmBlockContractWeb
   ```

3. **바인딩 설정:**
   
   **HTTP (임시):**
   ```
   유형: http
   IP 주소: 모두 할당되지 않음
   포트: 80
   호스트 이름: scm.spelix.co.kr (선택사항)
   ```
   
   **HTTPS (권장):**
   ```
   유형: https
   IP 주소: 모두 할당되지 않음
   포트: 443
   호스트 이름: scm.spelix.co.kr
   SSL 인증서: (다음 섹션에서 설정)
   ```

4. **확인** 클릭

### 5단계: 응용 프로그램 풀 권한 설정

**파일 시스템 권한 부여:**

1. `D:\WebApps\ScmBlockContractWeb` 폴더 우클릭
2. **속성 > 보안 탭**
3. **편집 > 추가** 클릭
4. 다음 계정 추가:
   ```
   IIS AppPool\DefaultAppPool
   또는
   IIS AppPool\ScmBlockContractWebPool
   ```
5. 권한 설정:
   - ✅ 읽기 및 실행
   - ✅ 폴더 내용 보기
   - ✅ 읽기
6. **확인**

---

## SSL 인증서 설정

### 옵션 1: 정식 SSL 인증서 (권장)

#### 1-1. SSL 인증서 구매
- **상용 CA**: Comodo, DigiCert, GlobalSign 등
- **무료**: Let's Encrypt (갱신 필요)

#### 1-2. CSR 생성 (IIS에서)

1. **IIS 관리자 > 서버 인증서**
2. **오른쪽: 인증서 요청 만들기**
3. **정보 입력:**
   ```
   일반 이름(CN): scm.spelix.co.kr
   조직(O): SPELIX
   조직 구성 단위(OU): IT Department
   구/군/시(L): Seoul
   시/도(S): Seoul
   국가(C): KR
   ```
4. **암호화 서비스 공급자:**
   ```
   Microsoft RSA SChannel Cryptographic Provider
   비트 길이: 2048
   ```
5. **파일 이름**: `C:\SSL\scm_spelix_co_kr.csr`
6. **마침**

#### 1-3. CSR을 CA에 제출하여 인증서 발급

CA에서 다음 파일을 받게 됩니다:
- `scm_spelix_co_kr.crt` (인증서)
- `intermediate.crt` (중간 인증서)
- `root.crt` (루트 인증서)

#### 1-4. 인증서 설치

1. **IIS 관리자 > 서버 인증서**
2. **인증서 요청 완료**
3. 발급받은 `.crt` 파일 선택
4. **친숙한 이름**: `scm.spelix.co.kr SSL`
5. **확인**

#### 1-5. HTTPS 바인딩 추가

1. **사이트 > ScmBlockContractWeb** 선택
2. **오른쪽: 바인딩 > 추가**
3. 설정:
   ```
   유형: https
   IP 주소: 모두 할당되지 않음
   포트: 443
   호스트 이름: scm.spelix.co.kr
   SSL 인증서: scm.spelix.co.kr SSL (방금 설치한 인증서)
   ☑ 서버 이름 표시 필요
   ```
4. **확인**

### 옵션 2: Let's Encrypt (무료)

**win-acme 도구 사용:**

1. **다운로드**: https://github.com/win-acme/win-acme/releases

2. **압축 해제 후 관리자 권한으로 실행:**
   ```cmd
   wacs.exe
   ```

3. **메뉴 선택:**
   ```
   N - Create certificate (default settings)
   4 - Manual input
   1 - Single binding of an IIS site
   ```

4. **도메인 입력:**
   ```
   scm.spelix.co.kr
   ```

5. **자동 갱신 설정** (90일마다)

### 옵션 3: 자체 서명 인증서 (테스트용만)

**⚠️ 주의: 브라우저에서 보안 경고 표시됨**

**PowerShell (관리자 권한):**
```powershell
# 자체 서명 인증서 생성
$cert = New-SelfSignedCertificate `
    -DnsName "scm.spelix.co.kr" `
    -CertStoreLocation "cert:\LocalMachine\My" `
    -NotAfter (Get-Date).AddYears(5)

# IIS에 바인딩
New-WebBinding -Name "ScmBlockContractWeb" `
    -IP "*" -Port 443 -Protocol https

$binding = Get-WebBinding -Name "ScmBlockContractWeb" `
    -Protocol "https"
$binding.AddSslCertificate($cert.GetCertHashString(), "my")
```

### HTTP에서 HTTPS로 자동 리디렉션

**Web.config에 추가:**
```xml
<system.webServer>
  <!-- 기존 설정 위에 추가 -->
  <rewrite>
    <rules>
      <rule name="HTTP to HTTPS redirect" stopProcessing="true">
        <match url="(.*)" />
        <conditions>
          <add input="{HTTPS}" pattern="off" ignoreCase="true" />
        </conditions>
        <action type="Redirect" url="https://{HTTP_HOST}/{R:1}" 
                redirectType="Permanent" />
      </rule>
    </rules>
  </rewrite>
  
  <!-- 기존 설정들... -->
</system.webServer>
```

**URL Rewrite 모듈 설치 필요:**
- https://www.iis.net/downloads/microsoft/url-rewrite

---

## 보안 설정

### 1. Windows 방화벽 설정

**PowerShell (관리자 권한):**
```powershell
# HTTP 포트 80 허용
New-NetFirewallRule -DisplayName "HTTP (TCP-In)" `
    -Direction Inbound -Protocol TCP -LocalPort 80 -Action Allow

# HTTPS 포트 443 허용
New-NetFirewallRule -DisplayName "HTTPS (TCP-In)" `
    -Direction Inbound -Protocol TCP -LocalPort 443 -Action Allow
```

**또는 GUI로:**
1. **제어판 > Windows Defender 방화벽**
2. **고급 설정**
3. **인바운드 규칙 > 새 규칙**
4. 포트 80, 443 허용

### 2. IIS 요청 필터링

**IIS 관리자:**
1. **사이트 > ScmBlockContractWeb** 선택
2. **요청 필터링** 더블클릭
3. **파일 이름 확장명 탭** - 위험한 확장자 차단:
   ```
   .exe, .bat, .cmd, .com
   ```
4. **HTTP 동사 탭** - 허용할 HTTP 메서드만:
   ```
   GET, POST
   ```

### 3. 디렉터리 검색 비활성화

**IIS 관리자:**
1. **사이트 > ScmBlockContractWeb** 선택
2. **디렉터리 검색**
3. **사용 안 함**

### 4. 상세 오류 숨기기

**이미 Web.config에 설정됨:**
```xml
<customErrors mode="RemoteOnly" />
```

---

## 성능 최적화

### 1. 정적 콘텐츠 압축

**IIS 관리자 > 서버 수준 > 압축:**
```
✅ 정적 콘텐츠 압축 사용
✅ 동적 콘텐츠 압축 사용
```

### 2. 출력 캐싱

**Web.config에 추가:**
```xml
<system.webServer>
  <caching enabled="true" enableKernelCache="true">
    <profiles>
      <add extension=".aspx" policy="CacheUntilChange" 
           kernelCachePolicy="CacheUntilChange" />
      <add extension=".js" policy="CacheUntilChange" 
           kernelCachePolicy="CacheUntilChange" />
      <add extension=".css" policy="CacheUntilChange" 
           kernelCachePolicy="CacheUntilChange" />
    </profiles>
  </caching>
</system.webServer>
```

### 3. 응용 프로그램 풀 최적화

**응용 프로그램 풀 > DefaultAppPool > 고급 설정:**
```
재생 > 일정 시간(분): 1740 (29시간)
재생 > 요청 제한: 0
프로세스 모델 > 유휴 시간 제한(분): 20
프로세스 모델 > 최대 작업자 프로세스: 1
```

---

## 문제 해결

### 1. 사이트에 접속이 안 됨

**확인 사항:**
```powershell
# IIS 서비스 상태 확인
Get-Service W3SVC

# 서비스 시작
Start-Service W3SVC

# 방화벽 규칙 확인
Get-NetFirewallRule | Where-Object {$_.DisplayName -like "*HTTP*"}

# 포트 리스닝 확인
netstat -ano | findstr :80
netstat -ano | findstr :443
```

### 2. 500 오류 발생

**상세 오류 보기 (임시):**
```xml
<system.web>
  <customErrors mode="Off" />
</system.web>
```

**IIS 로그 확인:**
```
C:\inetpub\logs\LogFiles\W3SVC1\
```

**이벤트 뷰어 확인:**
```
시작 > 이벤트 뷰어 > Windows 로그 > 응용 프로그램
```

### 3. Oracle 연결 오류

**연결 테스트:**
```powershell
# Oracle 포트 연결 테스트
Test-NetConnection -ComputerName HQ.SPELIX.CO.KR -Port 7744
```

**Oracle.ManagedDataAccess 확인:**
- `bin/` 폴더에 `Oracle.ManagedDataAccess.dll` 있는지 확인

### 4. DevExpress 리소스 로딩 오류

**Web.config 확인:**
```xml
<system.webServer>
  <handlers>
    <add type="DevExpress.Web.ASPxHttpHandlerModule, DevExpress.Web.v24.2, ..." 
         verb="GET,POST" path="DX.ashx" name="ASPxHttpHandlerModule" 
         preCondition="integratedMode" />
  </handlers>
</system.webServer>
```

---

## 배포 체크리스트

### 배포 전:
- [ ] Visual Studio에서 Release 모드로 게시
- [ ] Web.config - debug="false" 확인
- [ ] Web.config - customErrors="RemoteOnly" 확인
- [ ] 데이터베이스 연결 문자열 확인
- [ ] 모든 DLL 파일 포함 확인

### 서버 설정:
- [ ] IIS 설치 완료
- [ ] .NET Framework 4.8 설치 확인
- [ ] 응용 프로그램 풀 생성
- [ ] 웹사이트 생성
- [ ] 파일 시스템 권한 설정
- [ ] 방화벽 포트 개방 (80, 443)

### 보안:
- [ ] SSL 인증서 설치 (HTTPS)
- [ ] HTTP to HTTPS 리디렉션 설정
- [ ] 상세 오류 숨김 (customErrors)
- [ ] 디렉터리 검색 비활성화
- [ ] 요청 필터링 설정

### 테스트:
- [ ] HTTP 접속 테스트
- [ ] HTTPS 접속 테스트
- [ ] 로그인 기능 테스트
- [ ] 데이터 조회 테스트
- [ ] 엑셀 다운로드 테스트
- [ ] 다양한 브라우저 테스트 (Chrome, Edge, Firefox)

---

## 모니터링

### 1. IIS 로그 분석

**기본 위치:**
```
C:\inetpub\logs\LogFiles\W3SVC1\
```

**로그 분석 도구:**
- Log Parser Studio
- Splunk
- ELK Stack

### 2. 성능 모니터

**Windows 성능 모니터:**
```
perfmon.exe
```

**모니터링할 카운터:**
- ASP.NET Applications > Requests/Sec
- .NET CLR Memory > % Time in GC
- Processor > % Processor Time
- Memory > Available MBytes

### 3. 상태 확인 URL

**Application_Start에 헬스체크 추가 (선택사항):**
```
/HealthCheck.aspx
```

---

## 백업 및 복구

### 백업 대상:
1. **애플리케이션 파일**: `D:\WebApps\ScmBlockContractWeb\`
2. **IIS 설정**:
   ```powershell
   # IIS 설정 백업
   %windir%\system32\inetsrv\appcmd.exe add backup "ScmBackup"
   ```
3. **SSL 인증서**:
   ```powershell
   # 인증서 내보내기
   certutil -store my
   ```

### 복구:
```powershell
# IIS 설정 복원
%windir%\system32\inetsrv\appcmd.exe restore backup "ScmBackup"
```

---

## 추가 리소스

- **IIS 공식 문서**: https://docs.microsoft.com/iis
- **SSL Labs 테스트**: https://www.ssllabs.com/ssltest/
- **DevExpress 배포 가이드**: https://docs.devexpress.com/AspNet/402535

---

**배포 성공을 기원합니다! 🚀**
