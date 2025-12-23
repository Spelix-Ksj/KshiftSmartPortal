<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Site.Master" 
    CodeBehind="ToDoList.aspx.cs" Inherits="KShiftSmartPortalWeb.ToDoList" %>
<%@ Register Assembly="DevExpress.Web.Bootstrap.v25.1, Version=25.1.6.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    To-Do List - K-SHIFT Portal
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

        /* 컴보박스/DateEdit 높이 */
        .search-item .dxbs-combobox,
        .search-item .dxbs-date-edit,
        .search-item .dxbs-textbox {
            height: 32px !important;
            min-height: 32px !important;
        }
        .search-item .dxbs-combobox .form-control,
        .search-item .dxbs-date-edit .form-control {
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
        #gridToDoList {
            font-size: 12px !important;
        }

        /* 그리드 헤더 가운데 정렬 */
        #gridToDoList th,
        #gridToDoList th a,
        #gridToDoList th span,
        #gridToDoList thead th,
        #gridToDoList thead th a,
        #gridToDoList thead th span,
        .dxbs-gridview th,
        .dxbs-gridview th a,
        .dxbs-gridview th span {
            text-align: center !important;
            vertical-align: middle !important;
            font-size: 12px !important;
            padding: 6px 4px !important;
            color: #fff !important;
        }
        #gridToDoList th a:hover,
        .dxbs-gridview th a:hover {
            color: #fff !important;
            text-decoration: none !important;
        }

        /* 그리드 셀 스타일 */
        #gridToDoList td {
            font-size: 12px !important;
            padding: 4px !important;
            white-space: nowrap;
        }

        /* 헤더 색상 클래스 - CssClasses HeaderCell용 */
        .header-basic {
            background-color: #2196f3 !important;
            color: #fff !important;
        }
        .header-editable {
            background-color: #4caf50 !important;
            color: #fff !important;
        }
        .header-schedule {
            background-color: #ff9800 !important;
            color: #fff !important;
        }
        .header-status {
            background-color: #607d8b !important;
            color: #fff !important;
        }

        /* 수정 가능 컬럼 셀 배경색 */
        .cell-editable {
            background-color: #e8f5e9 !important;
        }

        /* 상태별 셀 색상 */
        .status-complete { background-color: #c8e6c9 !important; color: #2e7d32 !important; font-weight: bold; }
        .status-inprogress { background-color: #fff9c4 !important; color: #f9a825 !important; font-weight: bold; }
        .status-pending { background-color: #ffccbc !important; color: #e64a19 !important; font-weight: bold; }

        /* 필터 행 */
        .dxbs-gridview .dxgvFilterRow td {
            padding: 2px !important;
        }
        .dxbs-gridview .dxgvFilterRow input {
            height: 20px !important;
            font-size: 12px !important;
        }

        /* ===== 모바일 Card 스타일 ===== */
        .mobile-cards {
            display: none;
        }
        .todo-card {
            background: #fff;
            border-radius: 8px;
            padding: 12px;
            margin-bottom: 10px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            border-left: 4px solid #2196f3;
        }
        .todo-card.selected {
            border-left-color: #ff9800;
            background: #fff8e1;
        }
        .todo-card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 8px;
            padding-bottom: 8px;
            border-bottom: 1px solid #eee;
        }
        .todo-card-title {
            font-weight: 600;
            font-size: 14px;
            color: #333;
        }
        .todo-card-badge {
            padding: 3px 8px;
            border-radius: 12px;
            font-size: 11px;
            font-weight: 600;
        }
        .badge-complete { background: #c8e6c9; color: #2e7d32; }
        .badge-inprogress { background: #fff9c4; color: #f9a825; }
        .badge-pending { background: #ffccbc; color: #e64a19; }
        .todo-card-body {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 6px;
            font-size: 12px;
        }
        .todo-card-item {
            display: flex;
            flex-direction: column;
        }
        .todo-card-item.full-width {
            grid-column: 1 / -1;
        }
        .todo-card-label {
            color: #666;
            font-size: 11px;
        }
        .todo-card-value {
            color: #333;
            font-weight: 500;
        }
        .todo-card-value.editable {
            color: #4caf50;
        }
        .todo-card-footer {
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
            .search-item .dxbs-combobox,
            .search-item .dxbs-date-edit {
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

            /* 데스크탑 그리드 숨기고 모바일 카드 표시 */
            /* 그리드를 화면 밖으로 이동하되 렌더링은 유지 (팝업이 정상 동작하도록) */
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

            /* Bootstrap 모달 팝업 강제 표시 */
            body.modal-open {
                overflow: auto !important;
                padding-right: 0 !important;
            }
            .modal {
                display: block !important;
                position: fixed !important;
                left: 0 !important;
                top: 0 !important;
                z-index: 1050 !important;
            }
            .modal.show .modal-dialog {
                transform: none !important;
            }
            .modal-backdrop {
                z-index: 1040 !important;
            }

            /* 상태 바 모바일 */
            .status-bar {
                flex-direction: column;
                gap: 8px;
                text-align: center;
            }
            .status-bar > div:last-child {
                font-size: 11px;
            }
            .status-bar > div:last-child span {
                margin: 0 5px !important;
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

        // 모바일 카드에서 수정 버튼 클릭 시
        function openMobileEdit(companyNo, caseNo, projectNo, orderNo) {
            // CustomCallback을 통해 서버에서 해당 행 편집 처리
            var grid = gridToDoList;
            if (grid) {
                var param = 'EDIT:' + companyNo + '|' + caseNo + '|' + projectNo + '|' + orderNo;
                grid.PerformCallback(param);
            }
        }

        // 모바일 이전 페이지
        function mobilePrevPage() {
            var grid = gridToDoList;
            if (grid && grid.GetPageIndex() > 0) {
                grid.PrevPage();
            }
        }

        // 모바일 다음 페이지
        function mobileNextPage() {
            var grid = gridToDoList;
            if (grid && grid.GetPageIndex() < grid.GetPageCount() - 1) {
                grid.NextPage();
            }
        }

        // 그리드 콜백 완료 후 처리
        function onGridEndCallback(s, e) {
            if (s.cpMessage) {
                alert(s.cpMessage);
                s.cpMessage = null;

                // 모바일 뷰에서 데이터 수정 완료 시 조회 버튼 클릭으로 새로고침
                if (isMobileView() && s.cpNeedRefresh) {
                    s.cpNeedRefresh = null;
                    // DevExpress 버튼의 DoClick 메서드로 전체 페이지 갱신
                    if (typeof btnSearch !== 'undefined' && btnSearch) {
                        btnSearch.DoClick();
                    }
                }
            }
        }

        // 등록 폼 토글
        function toggleAddForm(show) {
            var form = document.getElementById('addFormContainer');
            if (show) {
                form.classList.add('show');
                // 기본값 설정
                if (typeof dtAddWorkSt !== 'undefined') {
                    dtAddWorkSt.SetValue(new Date());
                }
                if (typeof dtAddWorkFi !== 'undefined') {
                    dtAddWorkFi.SetValue(new Date());
                }
                // 폼으로 스크롤
                form.scrollIntoView({ behavior: 'smooth', block: 'start' });
            } else {
                form.classList.remove('show');
                clearAddForm();
            }
        }

        // 등록 폼 초기화
        function clearAddForm() {
            if (typeof cmbAddCase !== 'undefined') cmbAddCase.SetValue(null);
            if (typeof cmbAddProject !== 'undefined') cmbAddProject.SetValue(null);
            if (typeof txtAddOrderName !== 'undefined') txtAddOrderName.SetValue('');
            if (typeof txtAddWorkList !== 'undefined') txtAddWorkList.SetValue('');
            if (typeof dtAddWorkSt !== 'undefined') dtAddWorkSt.SetValue(new Date());
            if (typeof dtAddWorkFi !== 'undefined') dtAddWorkFi.SetValue(new Date());
            if (typeof spnAddPlanMhr !== 'undefined') spnAddPlanMhr.SetValue(0);
            if (typeof spnAddRealMhr !== 'undefined') spnAddRealMhr.SetValue(0);
            if (typeof spnAddPlanMp !== 'undefined') spnAddPlanMp.SetValue(0);
            if (typeof spnAddRealMp !== 'undefined') spnAddRealMp.SetValue(0);
            if (typeof dtAddCompDate !== 'undefined') dtAddCompDate.SetValue(null);
            if (typeof txtAddRmk !== 'undefined') txtAddRmk.SetValue('');
        }

        // 등록 유효성 검사
        function validateAddForm() {
            var caseNo = cmbAddCase.GetValue();
            var projectNo = cmbAddProject.GetValue();
            var orderName = txtAddOrderName.GetValue();

            if (!caseNo) {
                alert('케이스를 선택하세요.');
                cmbAddCase.Focus();
                return false;
            }
            if (!projectNo) {
                alert('프로젝트를 선택하세요.');
                cmbAddProject.Focus();
                return false;
            }
            if (!orderName || orderName.trim() === '') {
                alert('작업 내용을 입력하세요.');
                txtAddOrderName.Focus();
                return false;
            }
            return true;
        }

        // 등록 확인
        function confirmAdd() {
            if (!validateAddForm()) return;
            if (confirm('입력한 내용으로 작업 실적을 등록하시겠습니까?')) {
                // 서버로 포스트백
                return true;
            }
            return false;
        }
    </script>
</asp:Content>

<asp:Content ID="PageTitleContent" ContentPlaceHolderID="PageTitleContent" runat="server">
    To-Do List (작업지시 관리)
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- 조회 조건 패널 -->
    <div class="search-panel">
        <div class="search-panel-header">
            <i class="fas fa-search"></i> 조회 조건
        </div>
        
        <div class="search-row">
            <div class="search-item">
                <label>Company 유형:</label>
                <dx:BootstrapComboBox ID="cmbCompanyType" runat="server" 
                    Width="150px" 
                    OnSelectedIndexChanged="cmbCompanyType_SelectedIndexChanged" 
                    AutoPostBack="true" />
            </div>
            <div class="search-item">
                <label>Company:</label>
                <dx:BootstrapComboBox ID="cmbCompany" runat="server" 
                    Width="250px" 
                    AutoPostBack="false" />
            </div>
            <div class="search-item">
                <label>기준일:</label>
                <dx:BootstrapDateEdit  ID="dtBaseDate" runat="server" 
                    Width="150px" 
                    DisplayFormatString="yyyy-MM-dd" 
                    EditFormat="Date" 
                    AllowNull="false" />
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

            <dx:BootstrapButton ID="btnReset" runat="server"
                Text="<i class='fas fa-sync-alt'></i> 초기화"
                EncodeHtml="false"
                OnClick="btnReset_Click">
                <SettingsBootstrap RenderOption="Secondary" />
            </dx:BootstrapButton>

            <dx:BootstrapButton ID="btnDelete" runat="server"
                Text="<i class='fas fa-times'></i> 삭제"
                EncodeHtml="false"
                OnClick="btnDelete_Click">
                <SettingsBootstrap RenderOption="Danger" />
                <ClientSideEvents Click="function(s, e) {
                    e.processOnServer = confirm('선택한 데이터를 삭제하시겠습니까?');
                }" />
            </dx:BootstrapButton>

            <dx:BootstrapButton ID="btnExcel" runat="server"
                Text="<i class='fas fa-file-excel'></i> 엑셀 다운로드"
                EncodeHtml="false"
                OnClick="btnExcel_Click">
                <SettingsBootstrap RenderOption="Success" />
            </dx:BootstrapButton>

            <dx:BootstrapButton ID="btnAdd" runat="server"
                Text="<i class='fas fa-plus'></i> 실적 등록"
                EncodeHtml="false"
                AutoPostBack="false">
                <SettingsBootstrap RenderOption="Info" />
                <ClientSideEvents Click="function(s, e) { toggleAddForm(true); }" />
            </dx:BootstrapButton>
        </div>
    </div>

    <!-- 실적 등록 폼 (모바일 최적화) -->
    <div id="addFormContainer" class="add-form-container">
        <div class="add-form-header">
            <span class="add-form-title"><i class="fas fa-plus-circle"></i> 작업 실적 등록</span>
            <button type="button" class="add-form-close" onclick="toggleAddForm(false)">
                <i class="fas fa-times"></i>
            </button>
        </div>

        <div class="add-form-row">
            <div class="add-form-item">
                <label>케이스 <span class="required">*</span></label>
                <dx:BootstrapComboBox ID="cmbAddCase" runat="server"
                    ClientInstanceName="cmbAddCase"
                    Width="100%"
                    NullText="케이스 선택" />
            </div>
            <div class="add-form-item">
                <label>프로젝트 <span class="required">*</span></label>
                <dx:BootstrapComboBox ID="cmbAddProject" runat="server"
                    ClientInstanceName="cmbAddProject"
                    Width="100%"
                    NullText="프로젝트 선택" />
            </div>
        </div>

        <div class="add-form-row full">
            <div class="add-form-item">
                <label>작업 내용 <span class="required">*</span></label>
                <dx:BootstrapTextBox ID="txtAddOrderName" runat="server"
                    ClientInstanceName="txtAddOrderName"
                    Width="100%"
                    NullText="오늘 수행한 작업 내용을 입력하세요" />
            </div>
        </div>

        <div class="add-form-row full">
            <div class="add-form-item">
                <label>작업 목록 (상세)</label>
                <dx:BootstrapMemo ID="txtAddWorkList" runat="server"
                    ClientInstanceName="txtAddWorkList"
                    Width="100%"
                    Rows="3"
                    NullText="상세 작업 내용이나 특이사항" />
            </div>
        </div>

        <div class="add-form-row">
            <div class="add-form-item">
                <label>작업 시작일</label>
                <dx:BootstrapDateEdit ID="dtAddWorkSt" runat="server"
                    ClientInstanceName="dtAddWorkSt"
                    Width="100%"
                    DisplayFormatString="yyyy-MM-dd"
                    EditFormat="Date" />
            </div>
            <div class="add-form-item">
                <label>작업 종료일</label>
                <dx:BootstrapDateEdit ID="dtAddWorkFi" runat="server"
                    ClientInstanceName="dtAddWorkFi"
                    Width="100%"
                    DisplayFormatString="yyyy-MM-dd"
                    EditFormat="Date" />
            </div>
        </div>

        <div class="add-form-row">
            <div class="add-form-item">
                <label>계획 M/H</label>
                <dx:BootstrapSpinEdit ID="spnAddPlanMhr" runat="server"
                    ClientInstanceName="spnAddPlanMhr"
                    Width="100%"
                    DisplayFormatString="N1"
                    MinValue="0" MaxValue="99999"
                    Number="0" />
            </div>
            <div class="add-form-item">
                <label>실적 M/H</label>
                <dx:BootstrapSpinEdit ID="spnAddRealMhr" runat="server"
                    ClientInstanceName="spnAddRealMhr"
                    Width="100%"
                    DisplayFormatString="N1"
                    MinValue="0" MaxValue="99999"
                    Number="0" />
            </div>
        </div>

        <div class="add-form-row">
            <div class="add-form-item">
                <label>계획 인원</label>
                <dx:BootstrapSpinEdit ID="spnAddPlanMp" runat="server"
                    ClientInstanceName="spnAddPlanMp"
                    Width="100%"
                    DisplayFormatString="N1"
                    MinValue="0" MaxValue="9999"
                    Number="0" />
            </div>
            <div class="add-form-item">
                <label>실적 인원</label>
                <dx:BootstrapSpinEdit ID="spnAddRealMp" runat="server"
                    ClientInstanceName="spnAddRealMp"
                    Width="100%"
                    DisplayFormatString="N1"
                    MinValue="0" MaxValue="9999"
                    Number="0" />
            </div>
        </div>

        <div class="add-form-row">
            <div class="add-form-item">
                <label>완료일 (완료 시 입력)</label>
                <dx:BootstrapDateEdit ID="dtAddCompDate" runat="server"
                    ClientInstanceName="dtAddCompDate"
                    Width="100%"
                    DisplayFormatString="yyyy-MM-dd"
                    EditFormat="Date"
                    AllowNull="true" />
            </div>
            <div class="add-form-item">
                <label>비고</label>
                <dx:BootstrapTextBox ID="txtAddRmk" runat="server"
                    ClientInstanceName="txtAddRmk"
                    Width="100%"
                    NullText="비고사항" />
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
            <i class="fas fa-tasks"></i> 작업 목록
        </div>

        <%-- 데스크탑용 그리드 --%>
        <div class="desktop-grid">
        <dx:BootstrapGridView ID="gridToDoList" runat="server"
            ClientInstanceName="gridToDoList"
            Width="100%"
            KeyFieldName="COMPANY_NO;CASE_NO;PROJECT_NO;ORDER_NO"
            AutoGenerateColumns="False"
            EnableCallBacks="true"
            OnPageIndexChanged="gridToDoList_PageIndexChanged"
            OnCustomCallback="gridToDoList_CustomCallback"
            OnRowUpdating="gridToDoList_RowUpdating"
            OnHtmlRowPrepared="gridToDoList_HtmlRowPrepared">

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

            <ClientSideEvents EndCallback="onGridEndCallback" />
            
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
                PopupEditFormCaption="작업지시 수정" />            

            <%-- EditForm 템플릿 - 수정 가능한 필드만 표시 --%>
            <Templates>
                <EditForm>
                    <div style="padding: 20px; min-width: 350px;">
                        <table class="table" style="margin-bottom: 15px;">
                            <tr>
                                <td style="width: 120px; font-weight: bold; vertical-align: middle;">완료일</td>
                                <td>
                                    <dx:BootstrapDateEdit ID="edtCompDate" runat="server"
                                        Width="100%"
                                        DisplayFormatString="yyyy-MM-dd"
                                        EditFormat="Date"
                                        AllowNull="true"
                                        Value='<%# Bind("COMP_DATE") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">계획 M/H</td>
                                <td>
                                    <dx:BootstrapSpinEdit ID="edtPlanMhr" runat="server"
                                        Width="100%"
                                        DisplayFormatString="N1"
                                        MinValue="0" MaxValue="99999"
                                        AllowNull="true"
                                        Value='<%# Bind("PLAN_MHR") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">실적 M/H</td>
                                <td>
                                    <dx:BootstrapSpinEdit ID="edtRealMhr" runat="server"
                                        Width="100%"
                                        DisplayFormatString="N1"
                                        MinValue="0" MaxValue="99999"
                                        AllowNull="true"
                                        Value='<%# Bind("REAL_MHR") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">계획 인원</td>
                                <td>
                                    <dx:BootstrapSpinEdit ID="edtPlanMp" runat="server"
                                        Width="100%"
                                        DisplayFormatString="N1"
                                        MinValue="0" MaxValue="9999"
                                        AllowNull="true"
                                        Value='<%# Bind("PLAN_MP") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="font-weight: bold; vertical-align: middle;">실적 인원</td>
                                <td>
                                    <dx:BootstrapSpinEdit ID="edtRealMp" runat="server"
                                        Width="100%"
                                        DisplayFormatString="N1"
                                        MinValue="0" MaxValue="9999"
                                        AllowNull="true"
                                        Value='<%# Bind("REAL_MP") %>' />
                                </td>
                            </tr>
                        </table>
                        <div style="text-align: right; padding-top: 10px; border-top: 1px solid #ddd;">
                            <dx:BootstrapButton ID="btnUpdate" runat="server" Text="저장" AutoPostBack="false">
                                <SettingsBootstrap RenderOption="Primary" />
                                <ClientSideEvents Click="function(s, e) { gridToDoList.UpdateEdit(); }" />
                            </dx:BootstrapButton>
                            <dx:BootstrapButton ID="btnCancel" runat="server" Text="취소" AutoPostBack="false" CssClasses-Control="ml-2">
                                <SettingsBootstrap RenderOption="Secondary" />
                                <ClientSideEvents Click="function(s, e) { gridToDoList.CancelEdit(); }" />
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

                

                <%-- 기본 정보 컬럼 (ReadOnly) - 파란색 #2196f3 --%>
                <dx:BootstrapGridViewTextColumn FieldName="PROJECT_NO" Caption="프로젝트 번호" Width="120px" ReadOnly="true" VisibleIndex="1">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="PROP01" Caption="호선번호" Width="100px" ReadOnly="true" VisibleIndex="2">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="ORDER_NAME" Caption="지시명칭" Width="200px" ReadOnly="true" VisibleIndex="3">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>
                
                <%-- 수정 가능 컬럼 - 녹색 #4caf50 --%>
                <dx:BootstrapGridViewDateColumn FieldName="COMP_DATE" Caption="완료일" Width="120px" VisibleIndex="4">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" AllowNull="true" />
                    <CssClasses HeaderCell="header-editable text-center" />
                </dx:BootstrapGridViewDateColumn>

                <dx:BootstrapGridViewSpinEditColumn FieldName="PLAN_MHR" Caption="계획 M/H" Width="100px" VisibleIndex="5">
                    <PropertiesSpinEdit DisplayFormatString="N1" MinValue="0" MaxValue="99999" AllowNull="true" />
                    <CssClasses HeaderCell="header-editable text-center" />
                </dx:BootstrapGridViewSpinEditColumn>

                <dx:BootstrapGridViewSpinEditColumn FieldName="REAL_MHR" Caption="실적 M/H" Width="100px" VisibleIndex="6">
                    <PropertiesSpinEdit DisplayFormatString="N1" MinValue="0" MaxValue="99999" AllowNull="true" />
                    <CssClasses HeaderCell="header-editable text-center" />
                </dx:BootstrapGridViewSpinEditColumn>

                <dx:BootstrapGridViewSpinEditColumn FieldName="PLAN_MP" Caption="계획 인원" Width="100px" VisibleIndex="7">
                    <PropertiesSpinEdit DisplayFormatString="N1" MinValue="0" MaxValue="9999" AllowNull="true" />
                    <CssClasses HeaderCell="header-editable text-center" />
                </dx:BootstrapGridViewSpinEditColumn>

                <dx:BootstrapGridViewSpinEditColumn FieldName="REAL_MP" Caption="실적 인원" Width="100px" VisibleIndex="8">
                    <PropertiesSpinEdit DisplayFormatString="N1" MinValue="0" MaxValue="9999" AllowNull="true" />
                    <CssClasses HeaderCell="header-editable text-center" />
                </dx:BootstrapGridViewSpinEditColumn>

                 <%-- 기본 정보 컬럼 (ReadOnly) - 파란색 #2196f3 --%>
                <dx:BootstrapGridViewTextColumn FieldName="ORDER_NO" Caption="지시번호" Width="120px" ReadOnly="true" VisibleIndex="9">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>                
                <dx:BootstrapGridViewTextColumn FieldName="PROP02" Caption="블록번호" Width="100px" ReadOnly="true" VisibleIndex="10">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="WORK_LIST" Caption="작업목록" Width="150px" ReadOnly="true" VisibleIndex="11">
                    <CssClasses HeaderCell="header-basic text-center" />
                </dx:BootstrapGridViewTextColumn>

                <%-- 일정 정보 컬럼 (ReadOnly) - 주황색 #ff9800 --%>
                <dx:BootstrapGridViewDateColumn FieldName="WORK_ST" Caption="작업시작일" Width="110px" ReadOnly="true" VisibleIndex="12">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <CssClasses HeaderCell="header-schedule text-center" />
                </dx:BootstrapGridViewDateColumn>
                <dx:BootstrapGridViewDateColumn FieldName="WORK_FI" Caption="작업종료일" Width="110px" ReadOnly="true" VisibleIndex="13">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <CssClasses HeaderCell="header-schedule text-center" />
                </dx:BootstrapGridViewDateColumn>
                <dx:BootstrapGridViewDateColumn FieldName="DUE_DATE" Caption="마감일" Width="110px" ReadOnly="true" VisibleIndex="14">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <CssClasses HeaderCell="header-schedule text-center" />
                </dx:BootstrapGridViewDateColumn>
                <dx:BootstrapGridViewDateColumn FieldName="QM_DATE" Caption="품질관리일" Width="110px" ReadOnly="true" VisibleIndex="15">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <CssClasses HeaderCell="header-schedule text-center" />
                </dx:BootstrapGridViewDateColumn>
                <dx:BootstrapGridViewDateColumn FieldName="QM_COMP_DATE" Caption="QM완료일" Width="110px" ReadOnly="true" VisibleIndex="16">
                    <PropertiesDateEdit DisplayFormatString="yyyy-MM-dd" />
                    <CssClasses HeaderCell="header-schedule text-center" />
                </dx:BootstrapGridViewDateColumn>

                <%-- 상태 컬럼 (ReadOnly) --%>
                <dx:BootstrapGridViewTextColumn FieldName="STATUS" Caption="상태" Width="80px" ReadOnly="true" VisibleIndex="17">
                    <CssClasses HeaderCell="header-status text-center" />
                </dx:BootstrapGridViewTextColumn>
                <dx:BootstrapGridViewTextColumn FieldName="MHR_RATE" Caption="달성률(%)" Width="90px" ReadOnly="true" VisibleIndex="18">
                    <CssClasses HeaderCell="header-status text-center" />
                </dx:BootstrapGridViewTextColumn>

                <%-- 숨김 Key 컬럼 --%>
                <dx:BootstrapGridViewTextColumn FieldName="COMPANY_NO" Visible="false" />
                <dx:BootstrapGridViewTextColumn FieldName="CASE_NO" Visible="false" />
            </Columns>
        </dx:BootstrapGridView>
        </div>
        <%-- // 데스크탑용 그리드 끝 --%>

        <%-- 엑셀 Exporter --%>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridToDoList" />

        <%-- 모바일용 카드 뷰 --%>
        <div class="mobile-cards">
            <asp:Repeater ID="rptMobileCards" runat="server">
                <ItemTemplate>
                    <div class="todo-card" data-keys='<%# Eval("COMPANY_NO") + ";" + Eval("CASE_NO") + ";" + Eval("PROJECT_NO") + ";" + Eval("ORDER_NO") %>'>
                        <div class="todo-card-header">
                            <span class="todo-card-title"><%# Eval("ORDER_NAME") %></span>
                            <span class='todo-card-badge <%# GetStatusBadgeClass(Eval("STATUS") == null ? "" : Eval("STATUS").ToString()) %>'>
                                <%# Eval("STATUS") %>
                            </span>
                        </div>
                        <div class="todo-card-body">
                            <div class="todo-card-item">
                                <span class="todo-card-label">프로젝트</span>
                                <span class="todo-card-value"><%# Eval("PROJECT_NO") %></span>
                            </div>
                            <div class="todo-card-item">
                                <span class="todo-card-label">호선번호</span>
                                <span class="todo-card-value"><%# Eval("PROP01") %></span>
                            </div>
                            <div class="todo-card-item">
                                <span class="todo-card-label">완료일</span>
                                <span class="todo-card-value editable"><%# Eval("COMP_DATE", "{0:yyyy-MM-dd}") %></span>
                            </div>
                            <div class="todo-card-item">
                                <span class="todo-card-label">마감일</span>
                                <span class="todo-card-value"><%# Eval("DUE_DATE", "{0:yyyy-MM-dd}") %></span>
                            </div>
                            <div class="todo-card-item">
                                <span class="todo-card-label">계획 M/H</span>
                                <span class="todo-card-value editable"><%# Eval("PLAN_MHR", "{0:N1}") %></span>
                            </div>
                            <div class="todo-card-item">
                                <span class="todo-card-label">실적 M/H</span>
                                <span class="todo-card-value editable"><%# Eval("REAL_MHR", "{0:N1}") %></span>
                            </div>
                            <div class="todo-card-item">
                                <span class="todo-card-label">달성률</span>
                                <span class="todo-card-value"><%# Eval("MHR_RATE") %>%</span>
                            </div>
                            <div class="todo-card-item">
                                <span class="todo-card-label">작업기간</span>
                                <span class="todo-card-value"><%# Eval("WORK_ST", "{0:MM-dd}") %> ~ <%# Eval("WORK_FI", "{0:MM-dd}") %></span>
                            </div>
                        </div>
                        <div class="todo-card-footer">
                            <button type="button" class="btn-card-edit"
                                onclick="openMobileEdit('<%# Eval("COMPANY_NO") %>', '<%# Eval("CASE_NO") %>', '<%# Eval("PROJECT_NO") %>', '<%# Eval("ORDER_NO") %>')">
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
            <div>
                <span style="margin-right:15px; color:#4caf50;">■ 수정가능</span>
                <span style="margin-right:15px; color:#2196f3;">■ 기본정보</span>
                <span style="margin-right:15px; color:#ff9800;">■ 일정정보</span>
            </div>
        </div>
    </div>
</asp:Content>