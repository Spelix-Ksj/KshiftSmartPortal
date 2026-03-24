<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Site.Master"
    CodeBehind="OrganizationManager.aspx.cs" Inherits="KShiftSmartPortalWeb.OrganizationManager" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    조직 관리 - K-SHIFT Portal
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* 전체 폰트 사이즈 */
        .search-panel, .grid-container { font-size: 13px; }

        /* 조회 조건 패널 스타일 */
        .search-panel {
            background: #fff;
            border-radius: 8px;
            padding: 15px;
            margin-bottom: 15px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .search-panel-header {
            font-size: 14px;
            font-weight: 600;
            color: #333;
            margin-bottom: 10px;
            padding-bottom: 8px;
            border-bottom: 2px solid #17a2b8;
        }
        .search-row {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
            align-items: flex-end;
        }
        .search-item {
            display: flex;
            flex-direction: column;
            gap: 3px;
            min-width: 150px;
        }
        .search-item label {
            font-size: 12px;
            font-weight: 600;
            color: #666;
        }
        .button-group {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin-top: 12px;
            padding-top: 12px;
            border-top: 1px solid #eee;
        }
        .grid-container {
            background: #fff;
            border-radius: 8px;
            padding: 12px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .grid-header {
            font-size: 14px;
            text-align: center;
            font-weight: 600;
            color: #333;
            margin-bottom: 12px;
            padding-bottom: 8px;
            border-bottom: 2px solid #17a2b8;
        }
        .status-bar {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 10px 15px;
            background: #e8f4f8;
            border-radius: 6px;
            margin-top: 12px;
            border-left: 4px solid #17a2b8;
        }
        .record-count {
            font-size: 13px;
            color: #333;
        }
        .record-count strong {
            color: #17a2b8;
            font-size: 14px;
        }

        /* 컴보박스 높이 */
        .search-item .dxbs-combobox {
            height: 32px !important;
            min-height: 32px !important;
        }
        .search-item .dxbs-combobox .form-control {
            height: 32px !important;
            padding: 2px 8px !important;
            font-size: 13px !important;
        }

        /* 버튼 크기 조정 */
        .button-group .btn {
            padding: 5px 14px !important;
            font-size: 13px !important;
            min-width: 100px;
        }

        /* BootstrapGridView 스타일 */
        #gridOrg {
            font-size: 12px !important;
        }

        /* 그리드 헤더 가운데 정렬 */
        #gridOrg th,
        #gridOrg th a,
        #gridOrg th span,
        #gridOrg thead th,
        #gridOrg thead th a,
        #gridOrg thead th span,
        .dxbs-gridview th,
        .dxbs-gridview th a,
        .dxbs-gridview th span {
            text-align: center !important;
            vertical-align: middle !important;
            font-size: 12px !important;
            padding: 6px 4px !important;
            color: #fff !important;
        }
        #gridOrg th a:hover,
        .dxbs-gridview th a:hover {
            color: #fff !important;
            text-decoration: none !important;
        }

        /* 그리드 셀 스타일 */
        #gridOrg td {
            font-size: 12px !important;
            padding: 4px !important;
            white-space: nowrap;
        }

        /* 헤더 색상 클래스 */
        .header-basic {
            background-color: #2196f3 !important;
            color: #fff !important;
        }
        .header-editable {
            background-color: #4caf50 !important;
            color: #fff !important;
        }

        /* 필터 행 */
        .dxbs-gridview .dxgvFilterRow td {
            padding: 2px !important;
        }
        .dxbs-gridview .dxgvFilterRow input {
            height: 20px !important;
            font-size: 12px !important;
        }

        /* ===== 등록 폼 스타일 ===== */
        .add-form-container {
            display: none;
            background: #fff;
            border-radius: 8px;
            padding: 15px;
            margin-bottom: 15px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.15);
            border-left: 4px solid #4caf50;
        }
        .add-form-container.show {
            display: block;
        }
        .add-form-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 2px solid #4caf50;
        }
        .add-form-title {
            font-size: 16px;
            font-weight: 600;
            color: #333;
        }
        .add-form-close {
            background: none;
            border: none;
            font-size: 20px;
            cursor: pointer;
            color: #666;
            padding: 0 5px;
        }
        .add-form-close:hover {
            color: #e53935;
        }
        .add-form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 12px;
            margin-bottom: 12px;
        }
        .add-form-row.full {
            grid-template-columns: 1fr;
        }
        .add-form-row.triple {
            grid-template-columns: 1fr 1fr 1fr;
        }
        .add-form-item {
            display: flex;
            flex-direction: column;
            gap: 4px;
        }
        .add-form-item label {
            font-size: 12px;
            font-weight: 600;
            color: #555;
        }
        .add-form-item label .required {
            color: #e53935;
            margin-left: 2px;
        }
        .add-form-actions {
            display: flex;
            gap: 10px;
            justify-content: center;
            padding-top: 15px;
            border-top: 1px solid #eee;
            margin-top: 15px;
        }
        .add-form-actions .btn {
            min-width: 120px;
            padding: 10px 20px !important;
            font-size: 14px !important;
        }
        .btn-dup-check {
            padding: 4px 10px !important;
            font-size: 12px !important;
            min-width: auto !important;
            margin-left: 5px;
        }
        .dup-row {
            display: flex;
            align-items: flex-end;
            gap: 5px;
        }
        .dup-row > div:first-child {
            flex: 1;
        }

        /* ===== 모바일 Card 스타일 ===== */
        .mobile-cards {
            display: none;
        }
        .org-card {
            background: #fff;
            border-radius: 8px;
            padding: 12px;
            margin-bottom: 10px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            border-left: 4px solid #2196f3;
        }
        .org-card.selected {
            border-left-color: #ff9800;
            background: #fff8e1;
        }
        .org-card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 8px;
            padding-bottom: 8px;
            border-bottom: 1px solid #eee;
        }
        .org-card-title {
            font-weight: 600;
            font-size: 14px;
            color: #333;
        }
        .org-card-badge {
            padding: 3px 8px;
            border-radius: 12px;
            font-size: 11px;
            font-weight: 600;
        }
        .badge-active { background: #c8e6c9; color: #2e7d32; }
        .badge-inactive { background: #ffccbc; color: #e64a19; }
        .org-card-body {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 6px;
            font-size: 12px;
        }
        .org-card-item {
            display: flex;
            flex-direction: column;
        }
        .org-card-item.full-width {
            grid-column: 1 / -1;
        }
        .org-card-label {
            color: #666;
            font-size: 11px;
        }
        .org-card-value {
            color: #333;
            font-weight: 500;
        }
        .org-card-footer {
            margin-top: 10px;
            padding-top: 8px;
            border-top: 1px solid #eee;
            text-align: right;
        }
        .btn-card-edit {
            padding: 6px 16px;
            font-size: 12px;
            background: #2196f3;
            color: #fff;
            border: none;
            border-radius: 4px;
            cursor: pointer;
        }
        .btn-card-edit:hover {
            background: #1976d2;
        }

        /* 모바일 페이징 */
        .mobile-pager {
            display: none;
            justify-content: center;
            align-items: center;
            gap: 10px;
            padding: 15px;
            background: #f5f5f5;
            border-radius: 6px;
            margin-top: 10px;
        }
        .mobile-pager button {
            padding: 8px 16px;
            border: 1px solid #ddd;
            background: #fff;
            border-radius: 4px;
            cursor: pointer;
        }
        .mobile-pager button:disabled {
            opacity: 0.5;
            cursor: not-allowed;
        }
        .mobile-pager .page-info {
            font-size: 13px;
            color: #666;
        }

        /* ===== 모바일 반응형 ===== */
        @media (max-width: 768px) {
            .search-panel {
                padding: 12px;
            }
            .search-row {
                flex-direction: column;
                gap: 10px;
            }
            .search-item {
                width: 100%;
                min-width: unset;
            }
            .search-item .dxbs-combobox {
                width: 100% !important;
            }
            .button-group {
                display: grid;
                grid-template-columns: 1fr 1fr;
                gap: 8px;
            }
            .button-group .btn {
                width: 100% !important;
                min-width: unset;
            }
            .add-form-row {
                grid-template-columns: 1fr;
            }
            .add-form-row.triple {
                grid-template-columns: 1fr;
            }

            /* 데스크탑 그리드 숨기고 모바일 카드 표시 */
            .desktop-grid {
                position: absolute;
                left: -9999px;
                width: 1px;
                height: 1px;
                overflow: hidden;
            }
            .mobile-cards {
                display: block;
            }
            .mobile-pager {
                display: flex;
            }

            /* 상태 바 모바일 */
            .status-bar {
                flex-direction: column;
                gap: 8px;
                text-align: center;
            }
        }

        /* 데스크탑에서는 그리드 표시 */
        @media (min-width: 769px) {
            .desktop-grid {
                display: block;
            }
            .mobile-cards {
                display: none;
            }
            .mobile-pager {
                display: none;
            }
        }
    </style>
    <script type="text/javascript">
        // 모바일 뷰 여부 확인
        function isMobileView() {
            return window.innerWidth <= 768;
        }

        // 그리드 콜백 완료 후 처리
        function onGridEndCallback(s, e) {
            if (s.cpMessage) {
                alert(s.cpMessage);
                s.cpMessage = null;

                if (isMobileView() && s.cpNeedRefresh) {
                    s.cpNeedRefresh = null;
                    if (typeof btnSearch !== 'undefined' && btnSearch) {
                        btnSearch.DoClick();
                    }
                }
            }

            if (s.cpDupResult) {
                if (s.cpDupResult === 'OK') {
                    window._dupChecked = true;
                    var lbl = document.getElementById('lblDupResult');
                    if (lbl) { lbl.innerText = '사용 가능'; lbl.style.color = 'green'; }
                } else {
                    window._dupChecked = false;
                    var lbl = document.getElementById('lblDupResult');
                    if (lbl) { lbl.innerText = '이미 사용 중'; lbl.style.color = 'red'; }
                }
                s.cpDupResult = null;
            }
        }

        // 등록 폼 토글
        function toggleAddForm(show) {
            var form = document.getElementById('addFormContainer');
            if (show) {
                form.classList.add('show');
                form.scrollIntoView({ behavior: 'smooth', block: 'start' });
            } else {
                form.classList.remove('show');
                clearAddForm();
            }
        }

        // 등록 폼 초기화
        function clearAddForm() {
            if (typeof txtAddOrgNo !== 'undefined') txtAddOrgNo.SetValue('');
            if (typeof txtAddOrgName !== 'undefined') txtAddOrgName.SetValue('');
            if (typeof txtAddParentObsNo !== 'undefined') txtAddParentObsNo.SetValue('');
            if (typeof txtAddObsType !== 'undefined') txtAddObsType.SetValue('');
            if (typeof spnAddObsLevel !== 'undefined') spnAddObsLevel.SetValue(1);
            if (typeof cmbAddUseYn !== 'undefined') cmbAddUseYn.SetSelectedIndex(0);
            if (typeof spnAddViewOrder !== 'undefined') spnAddViewOrder.SetValue(0);
            if (typeof txtAddRmk !== 'undefined') txtAddRmk.SetValue('');

            // 중복확인 상태 초기화
            window._dupChecked = false;
            var lbl = document.getElementById('lblDupResult');
            if (lbl) lbl.innerText = '';
        }

        // 중복 체크
        function checkDuplicate() {
            var orgNo = txtAddOrgNo.GetValue();
            if (!orgNo || orgNo.trim() === '') {
                alert('조직번호를 입력하세요.');
                txtAddOrgNo.Focus();
                return;
            }
            gridOrg.PerformCallback('DUPCHECK:' + orgNo.trim());
        }

        // 등록 유효성 검사
        function validateAddForm() {
            var orgNo = txtAddOrgNo.GetValue();
            var orgName = txtAddOrgName.GetValue();

            if (!orgNo || orgNo.trim() === '') {
                alert('조직번호를 입력하세요.');
                txtAddOrgNo.Focus();
                return false;
            }
            if (!orgName || orgName.trim() === '') {
                alert('조직명을 입력하세요.');
                txtAddOrgName.Focus();
                return false;
            }
            if (!window._dupChecked) {
                alert('조직번호 중복 확인을 먼저 수행하세요.');
                return false;
            }
            return true;
        }

        // 등록 확인
        function confirmAdd() {
            if (!validateAddForm()) return false;
            if (confirm('입력한 내용으로 조직을 등록하시겠습니까?')) {
                return true;
            }
            return false;
        }

        // ===== 선택 항목 추적 =====
        var _selectedOrgName = '';

        // 데스크탑 그리드 행 포커스 변경 시
        function onFocusedRowChanged(s, e) {
            document.getElementById('hdnSelOrgNo').value = '';

            var rowIndex = s.GetFocusedRowIndex();
            if (rowIndex >= 0) {
                s.GetRowValues(rowIndex, 'OrgName', function(value) {
                    _selectedOrgName = value || '';
                });
                s.GetRowValues(rowIndex, 'OrgNo', function(value) {
                    document.getElementById('hdnSelOrgNo').value = value || '';
                });
            } else {
                _selectedOrgName = '';
            }
        }

        // 모바일 카드 선택
        function selectCard(element) {
            var orgNo = element.getAttribute('data-org-no');
            var orgName = element.getAttribute('data-org-name');
            document.querySelectorAll('.org-card.selected').forEach(function(card) {
                card.classList.remove('selected');
            });
            element.classList.add('selected');
            _selectedOrgName = orgName;
            document.getElementById('hdnSelOrgNo').value = orgNo;
        }

        // 모바일 수정 버튼
        function openMobileEdit(orgNo) {
            var grid = gridOrg;
            if (grid) {
                grid.PerformCallback('EDIT:' + orgNo);
            }
        }

        // 삭제 확인
        function confirmDelete(s, e) {
            if (isMobileView()) {
                var hdnOrg = document.getElementById('hdnSelOrgNo');
                if (!hdnOrg || !hdnOrg.value) {
                    alert('삭제할 조직을 선택하세요.');
                    e.processOnServer = false;
                    return;
                }
            } else {
                var grid = gridOrg;
                if (!grid || grid.GetFocusedRowIndex() < 0) {
                    alert('삭제할 조직을 선택하세요.');
                    e.processOnServer = false;
                    return;
                }
            }
            var name = _selectedOrgName || '';
            e.processOnServer = confirm('[' + name + ']\n이 조직을 삭제하시겠습니까?');
        }

        // 모바일 페이징
        function mobilePrevPage() {
            var grid = gridOrg;
            if (grid && grid.GetPageIndex() > 0) {
                grid.PrevPage();
            }
        }
        function mobileNextPage() {
            var grid = gridOrg;
            if (grid && grid.GetPageIndex() < grid.GetPageCount() - 1) {
                grid.NextPage();
            }
        }
    </script>
</asp:Content>

<asp:Content ID="PageTitleContent" ContentPlaceHolderID="PageTitleContent" runat="server">
    조직 관리 (Organization Manager)
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <%-- 모바일 카드 선택 키 전달용 --%>
    <input type="hidden" id="hdnSelOrgNo" name="hdnSelOrgNo" />

    <!-- 조회 조건 패널 -->
    <div class="search-panel">
        <div class="search-panel-header">
            <i class="fas fa-search"></i> 조회 조건
        </div>

        <div class="search-row">
            <div class="search-item">
                <label>Company:</label>
                <dx:BootstrapComboBox ID="cmbCompany" runat="server"
                    ClientInstanceName="cmbCompany"
                    Width="250px"
                    AutoPostBack="false" />
            </div>
        </div>

        <!-- 버튼 그룹 -->
        <div class="button-group">
            <dx:BootstrapButton ID="btnSearch" runat="server"
                ClientInstanceName="btnSearch"
                Text="<i class='fas fa-search'></i> 조회"
                EncodeHtml="false"
                OnClick="btnSearch_Click">
                <SettingsBootstrap RenderOption="Primary" />
            </dx:BootstrapButton>

            <dx:BootstrapButton ID="btnAdd" runat="server"
                Text="<i class='fas fa-plus'></i> 추가"
                EncodeHtml="false"
                AutoPostBack="false">
                <SettingsBootstrap RenderOption="Info" />
                <ClientSideEvents Click="function(s, e) { toggleAddForm(true); }" />
            </dx:BootstrapButton>

            <dx:BootstrapButton ID="btnDelete" runat="server"
                Text="<i class='fas fa-times'></i> 삭제"
                EncodeHtml="false"
                OnClick="btnDelete_Click">
                <SettingsBootstrap RenderOption="Danger" />
                <ClientSideEvents Click="function(s, e) { confirmDelete(s, e); }" />
            </dx:BootstrapButton>

            <dx:BootstrapButton ID="btnExcel" runat="server"
                Text="<i class='fas fa-file-excel'></i> 엑셀 다운로드"
                EncodeHtml="false"
                OnClick="btnExcel_Click">
                <SettingsBootstrap RenderOption="Success" />
            </dx:BootstrapButton>
        </div>
    </div>

    <!-- 조직 등록 폼 -->
    <div id="addFormContainer" class="add-form-container">
        <div class="add-form-header">
            <span class="add-form-title"><i class="fas fa-plus-circle"></i> 조직 등록</span>
            <button type="button" class="add-form-close" onclick="toggleAddForm(false)">
                <i class="fas fa-times"></i>
            </button>
        </div>

        <div class="add-form-row">
            <div class="add-form-item">
                <label>조직번호 <span class="required">*</span></label>
                <div class="dup-row">
                    <div>
                        <dx:BootstrapTextBox ID="txtAddOrgNo" runat="server"
                            ClientInstanceName="txtAddOrgNo"
                            Width="100%"
                            NullText="조직번호 입력">
                            <ClientSideEvents ValueChanged="function(s, e) { window._dupChecked = false; var lbl = document.getElementById('lblDupResult'); if(lbl) { lbl.innerText = ''; } }" />
                        </dx:BootstrapTextBox>
                    </div>
                    <dx:BootstrapButton ID="btnDupCheck" runat="server"
                        Text="중복확인"
                        AutoPostBack="false"
                        CssClasses-Control="btn-dup-check">
                        <SettingsBootstrap RenderOption="Warning" />
                        <ClientSideEvents Click="function(s, e) { checkDuplicate(); }" />
                    </dx:BootstrapButton>
                </div>
                <span id="lblDupResult" style="font-size: 11px;"></span>
            </div>
            <div class="add-form-item">
                <label>조직명 <span class="required">*</span></label>
                <dx:BootstrapTextBox ID="txtAddOrgName" runat="server"
                    ClientInstanceName="txtAddOrgName"
                    Width="100%"
                    NullText="조직명 입력" />
            </div>
        </div>

        <div class="add-form-row">
            <div class="add-form-item">
                <label>상위조직번호</label>
                <dx:BootstrapTextBox ID="txtAddParentObsNo" runat="server"
                    ClientInstanceName="txtAddParentObsNo"
                    Width="100%"
                    NullText="상위조직번호 입력" />
            </div>
            <div class="add-form-item">
                <label>조직타입</label>
                <dx:BootstrapTextBox ID="txtAddObsType" runat="server"
                    ClientInstanceName="txtAddObsType"
                    Width="100%"
                    NullText="조직타입 입력" />
            </div>
        </div>

        <div class="add-form-row">
            <div class="add-form-item">
                <label>조직레벨</label>
                <dx:BootstrapSpinEdit ID="spnAddObsLevel" runat="server"
                    ClientInstanceName="spnAddObsLevel"
                    Width="100%"
                    DisplayFormatString="N0"
                    MinValue="1" MaxValue="99"
                    Number="1" />
            </div>
            <div class="add-form-item">
                <label>사용여부</label>
                <dx:BootstrapComboBox ID="cmbAddUseYn" runat="server"
                    ClientInstanceName="cmbAddUseYn"
                    Width="100%"
                    NullText="사용여부 선택" />
            </div>
        </div>

        <div class="add-form-row">
            <div class="add-form-item">
                <label>표시순서</label>
                <dx:BootstrapSpinEdit ID="spnAddViewOrder" runat="server"
                    ClientInstanceName="spnAddViewOrder"
                    Width="100%"
                    DisplayFormatString="N0"
                    MinValue="0" MaxValue="99999"
                    Number="0" />
            </div>
            <div class="add-form-item">
                <label>비고</label>
                <dx:BootstrapTextBox ID="txtAddRmk" runat="server"
                    ClientInstanceName="txtAddRmk"
                    Width="100%"
                    NullText="비고 입력" />
            </div>
        </div>

        <div class="add-form-actions">
            <dx:BootstrapButton ID="btnAddSave" runat="server"
                Text="<i class='fas fa-save'></i> 등록"
                EncodeHtml="false"
                OnClick="btnAddSave_Click">
                <SettingsBootstrap RenderOption="Primary" />
                <ClientSideEvents Click="function(s, e) { e.processOnServer = confirmAdd(); }" />
            </dx:BootstrapButton>
            <dx:BootstrapButton ID="btnAddCancel" runat="server"
                Text="<i class='fas fa-times'></i> 취소"
                EncodeHtml="false"
                AutoPostBack="false">
                <SettingsBootstrap RenderOption="Secondary" />
                <ClientSideEvents Click="function(s, e) { toggleAddForm(false); }" />
            </dx:BootstrapButton>
        </div>
    </div>

    <!-- 그리드 컨테이너 -->
    <div class="grid-container">
        <div class="grid-header">
            <i class="fas fa-sitemap"></i> 조직 목록
        </div>

        <%-- 데스크탑용 그리드 --%>
        <div class="desktop-grid">
        <dx:BootstrapGridView ID="gridOrg" runat="server"
            ClientInstanceName="gridOrg"
            Width="100%"
            KeyFieldName="OrgNo"
            AutoGenerateColumns="False"
            EnableCallBacks="true"
            OnPageIndexChanged="gridOrg_PageIndexChanged"
            OnCustomCallback="gridOrg_CustomCallback"
            OnRowUpdating="gridOrg_RowUpdating">

            <Settings
                ShowFilterRow="True"
                ShowFilterRowMenu="True"
                VerticalScrollBarMode="Visible"
                VerticalScrollableHeight="450"
                HorizontalScrollBarMode="Auto" />

            <SettingsBehavior
                AllowFocusedRow="True"
                AllowSelectByRowClick="True" />

            <SettingsDataSecurity
                AllowEdit="True"
                AllowInsert="False"
                AllowDelete="False" />

            <ClientSideEvents EndCallback="onGridEndCallback" FocusedRowChanged="onFocusedRowChanged" />

            <%-- PopupEditForm 방식 설정 --%>
            <SettingsEditing Mode="PopupEditForm" />
            <SettingsPopup>
                <EditForm
                    HorizontalAlign="WindowCenter"
                    VerticalAlign="WindowCenter"
                    Modal="True"
                    PopupAnimationType="Auto" />
            </SettingsPopup>
            <SettingsText
                CommandEdit="수정"
                CommandUpdate="저장"
                CommandCancel="취소"
                PopupEditFormCaption="조직 정보 수정" />

            <%-- EditForm 템플릿 --%>
            <Templates>
                <EditForm>
                    <div style="padding: 20px; min-width: 450px;">
                        <table class="table" style="margin-bottom: 15px;">
                            <tr>
                                <td style="width: 120px; font-weight: bold; vertical-align: middle;">조직명</td>
                                <td>
                                    <dx:BootstrapTextBox ID="edtOrgName" runat="server"
                                        Width="100%"
                                        Value='<%# Bind("OrgName") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">상위조직번호</td>
                                <td>
                                    <dx:BootstrapTextBox ID="edtParentObsNo" runat="server"
                                        Width="100%"
                                        Value='<%# Bind("ParentObsNo") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">조직타입</td>
                                <td>
                                    <dx:BootstrapTextBox ID="edtObsType" runat="server"
                                        Width="100%"
                                        Value='<%# Bind("ObsType") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">조직레벨</td>
                                <td>
                                    <dx:BootstrapSpinEdit ID="edtObsLevel" runat="server"
                                        Width="100%"
                                        DisplayFormatString="N0"
                                        MinValue="1" MaxValue="99"
                                        AllowNull="true"
                                        Value='<%# Bind("ObsLevel") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">사용여부</td>
                                <td>
                                    <dx:BootstrapTextBox ID="edtUseYn" runat="server"
                                        Width="100%"
                                        Value='<%# Bind("UseYn") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">표시순서</td>
                                <td>
                                    <dx:BootstrapSpinEdit ID="edtViewOrder" runat="server"
                                        Width="100%"
                                        DisplayFormatString="N0"
                                        MinValue="0" MaxValue="99999"
                                        AllowNull="true"
                                        Value='<%# Bind("ViewOrder") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">비고</td>
                                <td>
                                    <dx:BootstrapTextBox ID="edtRmk" runat="server"
                                        Width="100%"
                                        Value='<%# Bind("Rmk") %>' />
                                </td>
                            </tr>
                        </table>
                        <div style="text-align: right; padding-top: 10px; border-top: 1px solid #ddd;">
                            <dx:BootstrapButton ID="btnUpdate" runat="server" Text="저장" AutoPostBack="false">
                                <SettingsBootstrap RenderOption="Primary" />
                                <ClientSideEvents Click="function(s, e) { gridOrg.UpdateEdit(); }" />
                            </dx:BootstrapButton>
                            <dx:BootstrapButton ID="btnCancel" runat="server" Text="취소" AutoPostBack="false" CssClasses-Control="ml-2">
                                <SettingsBootstrap RenderOption="Secondary" />
                                <ClientSideEvents Click="function(s, e) { gridOrg.CancelEdit(); }" />
                            </dx:BootstrapButton>
                        </div>
                    </div>
                </EditForm>
            </Templates>

            <SettingsPager Mode="ShowPager" PageSize="50" Position="Bottom">
                <PageSizeItemSettings Visible="true" Items="50,100,200,500" />
            </SettingsPager>

            <Columns>
                <%-- 수정 버튼 컬럼 --%>
                <dx:BootstrapGridViewCommandColumn Width="60px" ShowEditButton="True" VisibleIndex="0" Caption=" ">
                    <CssClasses HeaderCell="text-center" />
                </dx:BootstrapGridViewCommandColumn>

                <%-- 기본 정보 컬럼 --%>
                <dx:BootstrapGridViewTextColumn FieldName="OrgNo" Caption="조직번호" Width="120px" ReadOnly="true" VisibleIndex="1">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="OrgName" Caption="조직명" Width="200px" ReadOnly="true" VisibleIndex="2">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="ParentObsNo" Caption="상위조직번호" Width="120px" ReadOnly="true" VisibleIndex="3">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="ObsType" Caption="조직타입" Width="100px" ReadOnly="true" VisibleIndex="4">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewSpinEditColumn FieldName="ObsLevel" Caption="조직레벨" Width="90px" ReadOnly="true" VisibleIndex="5">
                    <PropertiesSpinEdit DisplayFormatString="N0" />
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewSpinEditColumn>
                <dx:BootstrapGridViewTextColumn FieldName="UseYn" Caption="사용여부" Width="80px" ReadOnly="true" VisibleIndex="6">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewSpinEditColumn FieldName="ViewOrder" Caption="표시순서" Width="90px" ReadOnly="true" VisibleIndex="7">
                    <PropertiesSpinEdit DisplayFormatString="N0" />
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewSpinEditColumn>
            </Columns>
        </dx:BootstrapGridView>
        </div>
        <%-- // 데스크탑용 그리드 끝 --%>

        <%-- 엑셀 Exporter --%>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridOrg" />

        <%-- 모바일용 카드 뷰 --%>
        <div class="mobile-cards">
            <asp:Repeater ID="rptMobileCards" runat="server">
                <ItemTemplate>
                    <div class="org-card"
                        data-org-no='<%#: Eval("OrgNo") %>'
                        data-org-name='<%#: Eval("OrgName") %>'
                        onclick="selectCard(this)">
                        <div class="org-card-header">
                            <span class="org-card-title"><%#: Eval("OrgName") %></span>
                            <span class='org-card-badge <%# (Eval("UseYn") != null && Eval("UseYn").ToString() == "Y") ? "badge-active" : "badge-inactive" %>'>
                                <%# (Eval("UseYn") != null && Eval("UseYn").ToString() == "Y") ? "사용" : "미사용" %>
                            </span>
                        </div>
                        <div class="org-card-body">
                            <div class="org-card-item">
                                <span class="org-card-label">조직번호</span>
                                <span class="org-card-value"><%#: Eval("OrgNo") %></span>
                            </div>
                            <div class="org-card-item">
                                <span class="org-card-label">조직타입</span>
                                <span class="org-card-value"><%#: Eval("ObsType") %></span>
                            </div>
                            <div class="org-card-item">
                                <span class="org-card-label">상위조직번호</span>
                                <span class="org-card-value"><%#: Eval("ParentObsNo") %></span>
                            </div>
                            <div class="org-card-item">
                                <span class="org-card-label">조직레벨</span>
                                <span class="org-card-value"><%#: Eval("ObsLevel") %></span>
                            </div>
                            <div class="org-card-item">
                                <span class="org-card-label">표시순서</span>
                                <span class="org-card-value"><%#: Eval("ViewOrder") %></span>
                            </div>
                        </div>
                        <div class="org-card-footer">
                            <button type="button" class="btn-card-edit"
                                data-org-no='<%#: Eval("OrgNo") %>'
                                onclick="event.stopPropagation(); openMobileEdit(this.getAttribute('data-org-no'))">
                                <i class="fas fa-edit"></i> 수정
                            </button>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <%-- 데이터 없음 메시지 --%>
            <asp:Panel ID="pnlNoData" runat="server" Visible="false" CssClass="text-center py-4 text-muted">
                <i class="fas fa-inbox fa-3x mb-3 d-block"></i>
                조회된 데이터가 없습니다.
            </asp:Panel>
        </div>
        <%-- // 모바일용 카드 뷰 끝 --%>

        <%-- 모바일용 간소화 페이징 --%>
        <div class="mobile-pager">
            <button type="button" onclick="mobilePrevPage()" id="btnMobilePrev">
                <i class="fas fa-chevron-left"></i> 이전
            </button>
            <span class="page-info">
                <asp:Label ID="lblMobilePageInfo" runat="server" Text="1 / 1" />
            </span>
            <button type="button" onclick="mobileNextPage()" id="btnMobileNext">
                다음 <i class="fas fa-chevron-right"></i>
            </button>
        </div>

        <%-- 상태 바 --%>
        <div class="status-bar">
            <div class="record-count">
                <asp:Label ID="lblRecordCount" runat="server" Text="조회된 데이터가 없습니다." />
            </div>
        </div>
    </div>
</asp:Content>
