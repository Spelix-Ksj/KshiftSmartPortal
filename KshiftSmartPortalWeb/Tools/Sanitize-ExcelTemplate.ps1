<#
.SYNOPSIS
    xlsx 템플릿(App_Data\ExcelTemplates) 배포 사본에서 다운로드 유출 위험 파트를 zip 수준에서 제거한다. Windows PowerShell 5.1 호환.
.DESCRIPTION
    런타임 ScheduleSheetInjector.Sanitize()가 DevExpress API로 지울 수 없는 외부 링크 파트(캐시 데이터·내부 호스트/드라이브 경로·OneDrive URL)를
    포함해 메모·작성자·저장 경로를 제거한다. 시트 데이터·수식·병합·필터·서식은 손대지 않는다.
      ① xl/externalLinks/ 폴더 삭제
      ② xl/_rels/workbook.xml.rels 의 externalLink Relationship 제거          (⑥ 공통: 삭제된 파트를 가리키는 관계 정리)
      ③ xl/workbook.xml: <externalReferences>, 외부참조([n])·#REF! definedName, 저장 경로(x15ac:absPath) 제거
      ④ [Content_Types].xml 의 externalLink Override 제거                      (⑥ 공통: 삭제된 파트의 Override 정리)
      ⑤ docProps/core.xml: dc:creator, cp:lastModifiedBy 값 비움(요소 유지)
      ⑥ 메모: comments*.xml·threadedComments·persons·메모 도형 vmlDrawing 삭제, 시트 <legacyDrawing/> 제거, 관계·Override 정리,
         그림의 카메라 도구 외부 링크(a14:cameraTool [n]…) 제거 → 정적 그림
      ⑦ 변경이 있을 때만 재압축(원본 덮어쓰기). -Backup 이면 .bak 보관. 변경 0이면 파일을 건드리지 않는다(멱등).
.PARAMETER Path
    대상 xlsx 경로
.PARAMETER Backup
    덮어쓰기 전 <파일>.bak 보관
.EXAMPLE
    .\Sanitize-ExcelTemplate.ps1 -Path '..\App_Data\ExcelTemplates\2. 가공 SCHEDULE.xlsx' -Backup
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$Path,
    [switch]$Backup
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression.FileSystem
$utf8 = New-Object System.Text.UTF8Encoding($false)   # BOM 없이 기록(원본 파트와 동일)

function Read-Text([string]$p) { [System.IO.File]::ReadAllText($p, $utf8) }
function Write-Text([string]$p, [string]$s) { [System.IO.File]::WriteAllText($p, $s, $utf8) }

# 파일에서 정규식 일치 항목을 제거하고 제거 건수를 반환. 파일이 없으면 0
function Remove-Match([string]$file, [string]$pattern) {
    if (-not (Test-Path -LiteralPath $file)) { return 0 }
    $s = Read-Text $file
    $n = [regex]::Matches($s, $pattern).Count
    if ($n -gt 0) { Write-Text $file ([regex]::Replace($s, $pattern, '')) }
    return $n
}

# 파트 파일 + 그 _rels 삭제. 삭제 건수(0/1) 반환
function Remove-Part([string]$file) {
    if (-not (Test-Path -LiteralPath $file)) { return 0 }
    Remove-Item -LiteralPath $file -Force
    $rels = Join-Path (Split-Path $file) ('_rels\' + (Split-Path $file -Leaf) + '.rels')
    if (Test-Path -LiteralPath $rels) { Remove-Item -LiteralPath $rels -Force }
    return 1
}

$Path = (Resolve-Path -LiteralPath $Path).Path
$work = Join-Path ([System.IO.Path]::GetTempPath()) ('xlsx_sanitize_' + [guid]::NewGuid().ToString('N'))
# 재귀 정리 대상이 시스템 임시 디렉터리 아래의 전용 작업 폴더인지 먼저 확인한다.
$tempRoot = [System.IO.Path]::GetFullPath([System.IO.Path]::GetTempPath()).TrimEnd('\') + '\'
$work = [System.IO.Path]::GetFullPath($work)
if (-not $work.StartsWith($tempRoot, [StringComparison]::OrdinalIgnoreCase) -or
    (Split-Path $work -Leaf) -notmatch '^xlsx_sanitize_[0-9a-f]{32}$') {
    throw '템플릿 정리용 임시 경로가 허용 범위를 벗어났습니다.'
}
$total = 0
try {
    [System.IO.Compression.ZipFile]::ExtractToDirectory($Path, $work)
    $xl = Join-Path $work 'xl'

    # ① 외부 링크 파트 폴더 삭제 (externalLink*.xml + _rels: 캐시 데이터·내부 경로·OneDrive URL)
    $n = 0
    $ext = Join-Path $xl 'externalLinks'
    if (Test-Path -LiteralPath $ext) { $n = @(Get-ChildItem -LiteralPath $ext -Recurse -File).Count; Remove-Item -LiteralPath $ext -Recurse -Force }
    "① externalLinks 파트 삭제: $n"; $total += $n

    # ③ workbook.xml: externalReferences, 외부참조([n])·#REF! 정의 이름, 저장 경로(x15ac:absPath)
    $wbXml = Join-Path $xl 'workbook.xml'
    $n1 = Remove-Match $wbXml '(?s)<externalReferences>.*?</externalReferences>'
    $n2 = Remove-Match $wbXml '<definedName\b[^>]*>[^<]*(\[\d+\]|#REF!)[^<]*</definedName>'
    $n3 = Remove-Match $wbXml '(?s)<mc:AlternateContent\b(?:(?!</mc:AlternateContent>).)*?<x15ac:absPath\b.*?</mc:AlternateContent>'
    "③ workbook.xml: externalReferences $n1 / definedName(외부참조·#REF!) $n2 / absPath $n3"; $total += $n1 + $n2 + $n3

    # ⑤ core.xml: 작성자 실명 비움(요소 유지). 이미 빈 값은 건수 미포함(멱등)
    $n = 0
    $core = Join-Path $work 'docProps\core.xml'
    if (Test-Path -LiteralPath $core) {
        $s = Read-Text $core
        $pat = '<(dc:creator|cp:lastModifiedBy)>[^<]+</\1>'
        $n = [regex]::Matches($s, $pat).Count
        if ($n -gt 0) { Write-Text $core ([regex]::Replace($s, $pat, '<$1></$1>')) }
    }
    "⑤ core.xml creator/lastModifiedBy 비움: $n"; $total += $n

    # ⑥ 메모: comments 관계가 있는 시트만 대상. comments·threadedComment 파트와 그 시트의 vmlDrawing(메모 도형 컨테이너) 삭제, <legacyDrawing/> 제거
    #    vmlDrawing 에 메모(Note)·카메라 그림 폴백(Pict) 외 개체(양식 컨트롤 등)가 있으면 경고 출력
    $nc = 0; $nv = 0; $nl = 0
    foreach ($rels in @(Get-ChildItem -LiteralPath (Join-Path $xl 'worksheets\_rels') -Filter *.rels -ErrorAction SilentlyContinue)) {
        $s = Read-Text $rels.FullName
        if ($s -notmatch '/relationships/comments"') { continue }
        $sheet = Join-Path $xl ('worksheets\' + ($rels.Name -replace '\.rels$', ''))
        foreach ($m in [regex]::Matches($s, '<Relationship\b[^>]*\bType="[^"]*/(comments|threadedComment|vmlDrawing)"[^>]*/>')) {
            $target = [regex]::Match($m.Value, '\bTarget="([^"]+)"').Groups[1].Value
            $target = [System.IO.Path]::GetFullPath((Join-Path (Split-Path $sheet) $target))
            if ($m.Groups[1].Value -ne 'vmlDrawing') { $nc += Remove-Part $target; continue }
            if (Test-Path -LiteralPath $target) {
                $others = [regex]::Matches((Read-Text $target), 'ObjectType="([^"]+)"') | ForEach-Object { $_.Groups[1].Value } |
                    Where-Object { $_ -notin 'Note', 'Pict' } | Select-Object -Unique
                if ($others) { Write-Warning "$(Split-Path $target -Leaf) 에 메모 외 개체($($others -join ','))가 있어 함께 삭제됨 — 확인 필요" }
            }
            $nv += Remove-Part $target
        }
        $nl += Remove-Match $sheet '<legacyDrawing\b[^>]*/>'
    }
    foreach ($dir in 'threadedComments', 'persons') {   # 스레드 메모 본문·작성자 명단(workbook 수준)
        $p = Join-Path $xl $dir
        if (Test-Path -LiteralPath $p) { $nc += @(Get-ChildItem -LiteralPath $p -Recurse -File).Count; Remove-Item -LiteralPath $p -Recurse -Force }
    }
    # 그림의 카메라 도구가 외부 워크북 셀 범위([n]…)를 가리키면 링크만 제거(캐시 이미지는 유지 → 정적 그림)
    $ncam = 0
    foreach ($d in @(Get-ChildItem -LiteralPath (Join-Path $xl 'drawings') -Filter drawing*.xml -ErrorAction SilentlyContinue)) {
        $ncam += Remove-Match $d.FullName '<a:ext uri="\{84589F7E-364E-4C9E-8A38-B11213B215E9\}"><a14:cameraTool\b[^>]*cellRange="[^"]*\[\d+\][^"]*"[^>]*/></a:ext>'
        $null = Remove-Match $d.FullName '<a:extLst>\s*</a:extLst>'
    }
    "⑥ 메모 파트 삭제: comments/threaded $nc / vmlDrawing $nv / legacyDrawing $nl / cameraTool 외부링크 $ncam"; $total += $nc + $nv + $nl + $ncam

    # ②④⑥ 공통: 삭제된 파트를 가리키는 Relationship(TargetMode=External 제외)과 [Content_Types] Override 정리
    $relRemoved = @()
    foreach ($rels in Get-ChildItem -LiteralPath $work -Recurse -Filter *.rels -File) {
        $base = Split-Path (Split-Path $rels.FullName)   # _rels 의 상위 = 소스 파트 폴더
        $s = Read-Text $rels.FullName; $orig = $s
        foreach ($m in [regex]::Matches($s, '<Relationship\b[^>]*/>')) {
            if ($m.Value -match 'TargetMode="External"') { continue }
            $t = [regex]::Match($m.Value, '\bTarget="([^"]+)"').Groups[1].Value
            $full = if ($t.StartsWith('/')) { Join-Path $work $t.TrimStart('/') } else { Join-Path $base $t }
            if (Test-Path -LiteralPath $full) { continue }
            $s = $s.Replace($m.Value, '')
            $relRemoved += "$($rels.FullName.Substring($work.Length + 1)) -> $t"
        }
        if ($s -ne $orig) { Write-Text $rels.FullName $s }
    }
    "②⑥ Relationship 제거: $($relRemoved.Count)"; $relRemoved | ForEach-Object { "    $_" }
    $ctRemoved = @()
    $ct = Join-Path $work '[Content_Types].xml'
    $s = Read-Text $ct; $orig = $s
    foreach ($m in [regex]::Matches($s, '<Override\b[^>]*/>')) {
        $pn = [regex]::Match($m.Value, '\bPartName="/([^"]+)"').Groups[1].Value
        if (Test-Path -LiteralPath (Join-Path $work $pn)) { continue }
        $s = $s.Replace($m.Value, ''); $ctRemoved += "/$pn"
    }
    if ($s -ne $orig) { Write-Text $ct $s }
    "④⑥ [Content_Types] Override 제거: $($ctRemoved.Count)"; $ctRemoved | ForEach-Object { "    $_" }
    $total += $relRemoved.Count + $ctRemoved.Count

    # ⑦ 재압축: 변경이 있을 때만 (멱등). 엔트리명은 '/' 구분자로 직접 지정
    if ($total -eq 0) { "⑦ 변경 없음 — 파일 유지: $Path"; return }
    if ($Backup) { Copy-Item -LiteralPath $Path -Destination "$Path.bak" -Force; "    백업: $Path.bak" }
    $tmpZip = "$work.xlsx"
    $zip = [System.IO.Compression.ZipFile]::Open($tmpZip, 'Create')
    try {
        foreach ($f in Get-ChildItem -LiteralPath $work -Recurse -File) {
            $name = $f.FullName.Substring($work.Length + 1).Replace('\', '/')
            $null = [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $f.FullName, $name, [System.IO.Compression.CompressionLevel]::Optimal)
        }
    }
    finally { $zip.Dispose() }
    Move-Item -LiteralPath $tmpZip -Destination $Path -Force
    "⑦ 재압축 완료 (총 $total 건 제거): $Path"
}
finally {
    if (Test-Path -LiteralPath $work) { Remove-Item -LiteralPath $work -Recurse -Force }
}
