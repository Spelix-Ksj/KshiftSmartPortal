# IIS 배포 자동화 스크립트
# ScmBlockContractWeb 배포용

# ================================
# 설정 변수 (여기만 수정하세요)
# ================================

$siteName = "ScmBlockContractWeb"
$appPoolName = "ScmBlockContractWebPool"
$sitePath = "D:\WebApps\ScmBlockContractWeb"
$sourceFiles = "C:\Publish\ScmBlockContractWeb"  # Visual Studio 게시 폴더
$hostName = "scm.spelix.co.kr"  # 선택사항
$httpPort = 80
$httpsPort = 443

# ================================
# 스크립트 시작
# ================================

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  SCM Block Contract Web 배포 시작  " -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# 관리자 권한 확인
if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "❌ 이 스크립트는 관리자 권한으로 실행해야 합니다!" -ForegroundColor Red
    Write-Host "PowerShell을 관리자 권한으로 다시 실행하세요." -ForegroundColor Yellow
    pause
    exit
}

# 1. IIS 설치 확인
Write-Host "[1/8] IIS 설치 확인 중..." -ForegroundColor Yellow
Import-Module WebAdministration -ErrorAction SilentlyContinue

if (!(Get-Module WebAdministration)) {
    Write-Host "❌ IIS가 설치되지 않았습니다!" -ForegroundColor Red
    Write-Host "먼저 IIS를 설치해야 합니다." -ForegroundColor Yellow
    
    $install = Read-Host "IIS를 지금 설치하시겠습니까? (Y/N)"
    if ($install -eq "Y") {
        Write-Host "IIS 설치 중..." -ForegroundColor Green
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer -All
        Enable-WindowsOptionalFeature -Online -FeatureName IIS-ASPNET45 -All
        Write-Host "✓ IIS 설치 완료" -ForegroundColor Green
    } else {
        exit
    }
}
Write-Host "✓ IIS 설치 확인됨" -ForegroundColor Green

# 2. 배포 폴더 생성
Write-Host "[2/8] 배포 폴더 준비 중..." -ForegroundColor Yellow

if (!(Test-Path $sitePath)) {
    New-Item -ItemType Directory -Path $sitePath -Force | Out-Null
    Write-Host "✓ 폴더 생성: $sitePath" -ForegroundColor Green
} else {
    Write-Host "✓ 폴더 존재: $sitePath" -ForegroundColor Green
}

# 3. 파일 복사
Write-Host "[3/8] 애플리케이션 파일 복사 중..." -ForegroundColor Yellow

if (Test-Path $sourceFiles) {
    Copy-Item -Path "$sourceFiles\*" -Destination $sitePath -Recurse -Force
    Write-Host "✓ 파일 복사 완료" -ForegroundColor Green
} else {
    Write-Host "❌ 소스 폴더를 찾을 수 없습니다: $sourceFiles" -ForegroundColor Red
    Write-Host "Visual Studio에서 먼저 게시(Publish)하세요!" -ForegroundColor Yellow
    pause
    exit
}

# 4. 응용 프로그램 풀 생성
Write-Host "[4/8] 응용 프로그램 풀 설정 중..." -ForegroundColor Yellow

if (Test-Path "IIS:\AppPools\$appPoolName") {
    Write-Host "⚠ 기존 응용 프로그램 풀 제거: $appPoolName" -ForegroundColor Yellow
    Remove-WebAppPool -Name $appPoolName
}

New-WebAppPool -Name $appPoolName
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name "managedRuntimeVersion" -Value "v4.0"
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name "managedPipelineMode" -Value "Integrated"
Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name "processModel.idleTimeout" -Value "00:20:00"

Write-Host "✓ 응용 프로그램 풀 생성 완료: $appPoolName" -ForegroundColor Green

# 5. 기존 사이트 제거 (있으면)
Write-Host "[5/8] 기존 사이트 확인 중..." -ForegroundColor Yellow

if (Test-Path "IIS:\Sites\$siteName") {
    Write-Host "⚠ 기존 사이트 제거: $siteName" -ForegroundColor Yellow
    Remove-WebSite -Name $siteName
}

# 6. 새 웹사이트 생성
Write-Host "[6/8] 웹사이트 생성 중..." -ForegroundColor Yellow

if ($hostName) {
    New-WebSite -Name $siteName `
                -PhysicalPath $sitePath `
                -ApplicationPool $appPoolName `
                -Port $httpPort `
                -HostHeader $hostName
} else {
    New-WebSite -Name $siteName `
                -PhysicalPath $sitePath `
                -ApplicationPool $appPoolName `
                -Port $httpPort
}

Write-Host "✓ 웹사이트 생성 완료: $siteName" -ForegroundColor Green

# 7. 파일 시스템 권한 설정
Write-Host "[7/8] 파일 시스템 권한 설정 중..." -ForegroundColor Yellow

$acl = Get-Acl $sitePath
$appPoolIdentity = "IIS AppPool\$appPoolName"
$permission = $appPoolIdentity, "ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule $permission
$acl.AddAccessRule($accessRule)
Set-Acl $sitePath $acl

Write-Host "✓ 권한 설정 완료" -ForegroundColor Green

# 8. 방화벽 규칙 설정
Write-Host "[8/8] 방화벽 설정 중..." -ForegroundColor Yellow

# HTTP 포트 허용
$httpRule = Get-NetFirewallRule -DisplayName "HTTP (SCM Web)" -ErrorAction SilentlyContinue
if (!$httpRule) {
    New-NetFirewallRule -DisplayName "HTTP (SCM Web)" `
                        -Direction Inbound `
                        -Protocol TCP `
                        -LocalPort $httpPort `
                        -Action Allow | Out-Null
    Write-Host "✓ 방화벽 규칙 추가: HTTP ($httpPort)" -ForegroundColor Green
}

# HTTPS 포트 허용
$httpsRule = Get-NetFirewallRule -DisplayName "HTTPS (SCM Web)" -ErrorAction SilentlyContinue
if (!$httpsRule) {
    New-NetFirewallRule -DisplayName "HTTPS (SCM Web)" `
                        -Direction Inbound `
                        -Protocol TCP `
                        -LocalPort $httpsPort `
                        -Action Allow | Out-Null
    Write-Host "✓ 방화벽 규칙 추가: HTTPS ($httpsPort)" -ForegroundColor Green
}

# 완료
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "      배포 완료! 🎉                 " -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "접속 URL:" -ForegroundColor Yellow
if ($hostName) {
    Write-Host "  http://$hostName" -ForegroundColor Green
} else {
    $ipAddress = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.IPAddress -notlike "127.*"} | Select-Object -First 1).IPAddress
    Write-Host "  http://$ipAddress" -ForegroundColor Green
}
Write-Host ""
Write-Host "다음 단계:" -ForegroundColor Yellow
Write-Host "  1. 브라우저에서 위 URL로 접속 테스트" -ForegroundColor White
Write-Host "  2. SSL 인증서 설치 (HTTPS 사용 시)" -ForegroundColor White
Write-Host "  3. Web.config 설정 확인" -ForegroundColor White
Write-Host ""

# IIS 관리자 열기
$openIIS = Read-Host "IIS 관리자를 여시겠습니까? (Y/N)"
if ($openIIS -eq "Y") {
    Start-Process "inetmgr.exe"
}

pause
