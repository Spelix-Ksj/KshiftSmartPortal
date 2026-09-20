# 조립 탭의 읽기 전용 DB 매핑 및 실제 워크북 주입 검사. 접속 문자열과 업무 데이터는 출력하지 않는다.
param([string]$DevExpressDirectory = 'C:/Program Files/DevExpress 25.2/Components/Bin/Framework')
$ErrorActionPreference = 'Stop'
$project = Split-Path $PSScriptRoot -Parent
[void][System.Reflection.Assembly]::LoadFrom((Join-Path $project 'bin/Oracle.ManagedDataAccess.dll'))
[xml]$config = Get-Content -LiteralPath (Join-Path $project 'Web.config')
$entry = $config.configuration.connectionStrings.add | Where-Object { $_.name -eq 'OracleConnection' }
$builder = New-Object Oracle.ManagedDataAccess.Client.OracleConnectionStringBuilder($entry.connectionString)
$builder.Pooling = $false
$builder['Connection Timeout'] = 30
$connection = New-Object Oracle.ManagedDataAccess.Client.OracleConnection($builder.ConnectionString)
try {
    $connection.Open()
    foreach ($sql in @(
        "SELECT COMPANY_NO,TEMP_NO,ST_ROW,ST_COL,EXCEL_NAME,TAG2 FROM SCM_EXCEL_TEMP_MASTER WHERE TAG1='ScmBlockAssPlanMan'",
        "SELECT A.PROP_NAME,A.COLUMN_CELL,A.OBJ_NAME,A.TAG2 FROM SCM_EXCEL_TEMP_DETAIL A JOIN SCM_EXCEL_TEMP_MASTER B ON A.COMPANY_NO=B.COMPANY_NO AND A.TEMP_NO=B.TEMP_NO WHERE B.TAG1='ScmBlockAssPlanMan' AND B.COMPANY_NO='1010' ORDER BY A.VIEW_ORDER",
        "SELECT COUNT(*) ROW_COUNT FROM SCM_BLOCK_ASS_WS WHERE COMPANY_NO='1010' AND CASE_NO='ING'"
    )) {
        $command = $connection.CreateCommand()
        $command.CommandText = $sql
        $adapter = New-Object Oracle.ManagedDataAccess.Client.OracleDataAdapter($command)
        $table = New-Object System.Data.DataTable
        [void]$adapter.Fill($table)
        $table | Format-Table -AutoSize | Out-String -Width 200 | Write-Output
        $adapter.Dispose()
        $command.Dispose()
    }
    $tables = @()
    foreach ($sql in @(
        "SELECT * FROM SCM_BLOCK_ASS_WS WHERE COMPANY_NO='1010' AND CASE_NO='ING' ORDER BY NVL(PROP01,'999'),PROJECT_NO,ITM_COD",
        "SELECT A.* FROM SCM_EXCEL_TEMP_DETAIL A JOIN SCM_EXCEL_TEMP_MASTER B ON A.COMPANY_NO=B.COMPANY_NO AND A.TEMP_NO=B.TEMP_NO WHERE B.TAG1='ScmBlockAssPlanMan' AND B.COMPANY_NO='1010' ORDER BY A.VIEW_ORDER"
    )) {
        $command = $connection.CreateCommand()
        $command.CommandText = $sql
        $adapter = New-Object Oracle.ManagedDataAccess.Client.OracleDataAdapter($command)
        $table = New-Object System.Data.DataTable
        [void]$adapter.Fill($table)
        $tables += ,$table
        $adapter.Dispose()
        $command.Dispose()
    }
    # .NET Framework PowerShell에서 웹 프로젝트와 같은 DevExpress 엔진으로 검증한다.
    foreach ($name in @('DevExpress.Data.v25.2.dll','DevExpress.Office.v25.2.Core.dll','DevExpress.Spreadsheet.v25.2.Core.dll','KShiftSmartPortal.dll')) {
        [void][System.Reflection.Assembly]::LoadFrom((Join-Path $project ('bin/' + $name)))
    }
    # Workbook의 구체 구현은 별도 Document Processor 어셈블리에 있다(웹 배포 의존성 추가 없음).
    [void][System.Reflection.Assembly]::LoadFrom((Join-Path $DevExpressDirectory 'DevExpress.Docs.v25.2.dll'))
    $workbook = New-Object DevExpress.Spreadsheet.Workbook
    $stream = New-Object System.IO.MemoryStream
    try {
        $template = Join-Path $project 'App_Data/ExcelTemplates/2. 조립 SCHEDULE.xlsx'
        $workbook.LoadDocument($template) | Out-Null
        [KShiftSmartPortalWeb.Utils.ScheduleSheetInjector]::Inject($workbook, $tables[0], $tables[1], 11)
        $sheet = $workbook.Worksheets['SCHEDULE']
        $checked = 0
        for ($row = 0; $row -lt $tables[0].Rows.Count; $row++) {
            foreach ($definition in $tables[1].Rows) {
                if ($definition['OBJ_NAME'] -ne 'MASTER') { continue }
                $value = $tables[0].Rows[$row][[string]$definition['PROP_NAME']]
                if ($value -eq [DBNull]::Value) { continue }
                $column = [KShiftSmartPortalWeb.Utils.ScheduleSheetInjector]::ColumnLetterToIndex([string]$definition['COLUMN_CELL'])
                $actual = $sheet.Cells[(11 + $row), $column].Value
                switch ([string]$definition['TAG2']) {
                    'NUMBER' { $same = $actual.NumericValue -eq [Convert]::ToDouble($value) }
                    'DATE' { $same = $actual.DateTimeValue -eq [Convert]::ToDateTime($value) }
                    default { $same = $actual.TextValue -ceq [Convert]::ToString($value) }
                }
                if (-not $same) { throw "셀 주입 불일치: row=$row, column=$column" }
                $checked++
            }
        }
        Write-Output "Typed cell verification PASS: $checked cells"
        if ($sheet.Cells['F12'].Value.TextValue -ne [string]$tables[0].Rows[0]['ITM_COD']) {
            throw '첫 행 블록 코드 주입 불일치'
        }
        $lastRow = $tables[0].Rows.Count - 1
        if ($sheet.Cells[(11 + $lastRow), 5].Value.TextValue -ne [string]$tables[0].Rows[$lastRow]['ITM_COD']) {
            throw '마지막 행 블록 코드 주입 불일치'
        }
        $workbook.SaveDocument($stream, [DevExpress.Spreadsheet.DocumentFormat]::Xlsx)
        $stream.Position = 0
        $reload = New-Object DevExpress.Spreadsheet.Workbook
        try {
            $reload.LoadDocument($stream, [DevExpress.Spreadsheet.DocumentFormat]::Xlsx) | Out-Null
            # 독립 PowerShell 호스트는 웹 프로젝트의 컴파일된 라이선스를 사용하지 않는다.
            $unexpected = @($reload.Worksheets | Where-Object { $_.Name -notin @('SCHEDULE', 'Evaluation Warning') })
            if ($unexpected.Count -gt 0) { throw '예상하지 않은 추가 시트' }
            if ($reload.Worksheets.Contains('Evaluation Warning')) {
                Write-Warning '독립 검증 호스트에 평가판 안내 시트가 추가됨. 웹 다운로드는 브라우저 검사에서 별도 확인.'
            }
            if ($reload.ExternalWorkbooks.Count -ne 0) { throw '외부 워크북 참조 잔존' }
            Write-Output "Workbook roundtrip PASS: $($tables[0].Rows.Count) rows, $($stream.Length) bytes, no external workbooks"
        } finally { $reload.Dispose() }
    } finally { $stream.Dispose(); $workbook.Dispose() }
} finally { $connection.Dispose() }
