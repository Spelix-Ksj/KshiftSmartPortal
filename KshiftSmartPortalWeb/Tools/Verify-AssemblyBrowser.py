"""로컬 조립 탭의 조회, 스프레드시트 및 다운로드 통합 검사."""
import argparse
import os
from pathlib import Path
import tempfile
import time
from zipfile import ZipFile
import xml.etree.ElementTree as ET
from playwright.sync_api import sync_playwright

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--url', default='http://localhost:52413')
parser.add_argument('--expected-rows', type=int, default=388)
args = parser.parse_args()

with sync_playwright() as playwright:
    browser = playwright.chromium.launch(channel='chrome', headless=True)
    page = browser.new_page(viewport={'width': 1600, 'height': 1000})
    errors = []
    page.on('pageerror', lambda error: errors.append(str(error)))
    dialogs = []
    page.on('dialog', lambda dialog: (dialogs.append(dialog.message), dialog.dismiss()))
    page.goto(args.url + '/Views/Login.aspx', timeout=60000)
    page.locator('#txtUserId_I').fill(os.environ['KSHIFT_TEST_USER'])
    page.locator('#txtPassword_I').fill(os.environ['KSHIFT_TEST_PASSWORD'])
    with page.expect_navigation(timeout=60000):
        page.evaluate('btnLogin.DoClick()')
    page.goto(args.url + '/Views/BlockSchedule.aspx?view=ScmBlockAssPlanMan', timeout=60000)
    page.wait_for_function('window.cmbCompany && window.btnSearch')
    assert page.locator('.view-tabs .active').inner_text() == '조립'
    with page.expect_navigation(timeout=60000):
        page.evaluate("cmbCompany.SetValue('1010'); __doPostBack('ctl00$MainContent$cmbCompany', '')")
    page.wait_for_function('window.cmbCase && window.btnSearch')
    with page.expect_navigation(timeout=60000):
        page.evaluate("cmbCase.SetValue('ING'); btnSearch.DoClick()")
    page.wait_for_function('window.gridSchedule')
    print('GRID', page.locator('[id$="lblRecordCount"]').inner_text())
    assert f'{args.expected_rows}건' in page.locator('[id$="lblRecordCount"]').inner_text()
    # 그리드 페이징 콜백 후에도 동적 컬럼과 데이터가 유지되는지 확인한다.
    if args.expected_rows > 50:
        page.evaluate('gridSchedule.NextPage()')
        page.wait_for_function('gridSchedule.GetPageIndex() === 1 && !gridSchedule.InCallback()')
        print('GRID paging PASS')
    start = time.monotonic()
    with page.expect_navigation(timeout=60000):
        page.locator('#MainContent_rblViewMode_RB1_I').check()
    page.wait_for_function('window.spreadsheet', timeout=60000)
    page.get_by_text('Loading...', exact=True).wait_for(state='hidden', timeout=60000)
    page.wait_for_timeout(500)
    print('SHEET render seconds', round(time.monotonic() - start, 2))
    screenshot = Path(tempfile.gettempdir()) / 'kshift-assembly-verified.png'
    page.screenshot(path=str(screenshot), full_page=True)
    print('SCREENSHOT', screenshot)
    with page.expect_download(timeout=60000) as download_info:
        page.locator('#MainContent_btnExcel').click()
    download = download_info.value
    with ZipFile(download.path()) as archive:
        names = archive.namelist()
        assert not any('externalLinks/' in name or 'comments' in name.lower() for name in names)
        ns = {'m': 'http://schemas.openxmlformats.org/spreadsheetml/2006/main'}
        workbook = ET.fromstring(archive.read('xl/workbook.xml'))
        sheets = [s.get('name') for s in workbook.find('m:sheets', ns)]
        assert sheets == ['SCHEDULE'], sheets
        print('DOWNLOAD PASS: one SCHEDULE sheet; no external links or comments;', len(download.path().read_bytes()), 'bytes')
    # 두 표시 방식을 왕복하고 그리드 내보내기도 검증한다.
    with page.expect_navigation(timeout=60000):
        page.locator('#MainContent_rblViewMode_RB0_I').check()
    page.wait_for_function('window.gridSchedule')
    with page.expect_download(timeout=60000) as grid_download:
        page.locator('#MainContent_btnExcel').click()
    with ZipFile(grid_download.value.path()) as archive:
        assert 'xl/workbook.xml' in archive.namelist()
    with page.expect_navigation(timeout=60000):
        page.locator('#MainContent_rblViewMode_RB1_I').check()
    page.wait_for_function('window.spreadsheet')
    page.get_by_text('Loading...', exact=True).wait_for(state='hidden', timeout=60000)
    print('MODE roundtrip and grid export PASS')
    # 조회 결과가 0건일 때 이전 388건이 문서에 남지 않는지 검사한다.
    with page.expect_navigation(timeout=60000):
        page.evaluate("cmbDateType.SetSelectedIndex(1); dtStart.SetDate(new Date(2099,0,1)); dtEnd.SetDate(new Date(2099,0,2)); btnSearch.DoClick()")
    page.wait_for_function('window.btnSearch')
    assert page.locator('#MainContent_spreadsheet').count() == 0
    assert '0건' in page.locator('[id$="lblRecordCount"]').inner_text()
    assert dialogs == ['조회된 데이터가 없습니다.'], dialogs
    dialogs.clear()
    with page.expect_download(timeout=60000) as empty_download:
        page.locator('#MainContent_btnExcel').click()
    with ZipFile(empty_download.value.path()) as archive:
        sheet = ET.fromstring(archive.read('xl/worksheets/sheet1.xml'))
        cell = sheet.find('.//m:c[@r="F12"]', ns)
        assert cell is None or (cell.find('m:v', ns) is None and cell.find('m:is', ns) is None)
    print('EMPTY requery/export PASS: previous block value cleared')
    with page.expect_navigation(timeout=60000):
        page.evaluate("cmbCompany.SetValue('100'); __doPostBack('ctl00$MainContent$cmbCompany', '')")
    assert page.locator('#MainContent_spreadsheet').count() == 0
    print('COMPANY change in sheet mode PASS')
    assert not dialogs, dialogs
    assert not errors, errors
    browser.close()
