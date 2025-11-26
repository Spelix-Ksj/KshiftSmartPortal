<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="KShiftSmartPortalWeb.Home" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>K-SHIFT Portal Management</title>
    
    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Font Awesome -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" rel="stylesheet" />
    
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #1e3c72 0%, #2a5298 50%, #1e3c72 100%);
            min-height: 100vh;
            padding: 0;
            overflow-x: hidden;
        }

        .main-container {
            max-width: 1400px;
            margin: 0 auto;
            padding: 20px;
        }

        .header-section {
            text-align: center;
            color: white;
            margin-bottom: 30px;
            padding: 50px 20px;
        }

        .header-section h1 {
            font-size: 48px;
            font-weight: 700;
            margin-bottom: 10px;
            text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
        }

        .version-badge {
            display: inline-block;
            background: rgba(255, 255, 255, 0.2);
            padding: 5px 15px;
            border-radius: 20px;
            font-size: 14px;
            margin-left: 10px;
        }

        .search-section {
            margin-bottom: 30px;
        }

        .search-box {
            position: relative;
        }

        .search-box input {
            width: 100%;
            padding: 15px 50px 15px 20px;
            border: none;
            border-radius: 10px;
            font-size: 16px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }

        .search-box .search-icon {
            position: absolute;
            right: 20px;
            top: 50%;
            transform: translateY(-50%);
            color: #999;
            font-size: 20px;
        }

        .shortcuts-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }

        .shortcut-tile {
            background: white;
            border-radius: 15px;
            padding: 30px 20px;
            text-align: center;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            position: relative;
            min-height: 180px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
        }

        .shortcut-tile:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 12px rgba(0, 0, 0, 0.2);
        }

        .shortcut-tile.locked::before {
            content: '\f023';
            font-family: 'Font Awesome 6 Free';
            font-weight: 900;
            position: absolute;
            top: 10px;
            left: 10px;
            font-size: 14px;
            color: #666;
        }

        .shortcut-icon {
            width: 80px;
            height: 80px;
            border-radius: 15px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 15px;
            font-size: 36px;
            color: white;
        }

        .shortcut-title {
            font-size: 16px;
            font-weight: 600;
            color: #333;
            margin-bottom: 0;
        }

        .add-shortcut-tile {
            background: rgba(255, 255, 255, 0.1);
            border: 2px dashed rgba(255, 255, 255, 0.3);
            color: white;
            display: none; /* 기본적으로 숨김 */
        }

        .edit-mode .add-shortcut-tile {
            display: flex; /* 편집모드에서만 표시 */
        }

        .add-shortcut-tile:hover {
            background: rgba(255, 255, 255, 0.2);
            border-color: rgba(255, 255, 255, 0.5);
        }

        .add-shortcut-tile .shortcut-icon {
            background: transparent;
            color: white;
            font-size: 48px;
        }

        .add-shortcut-tile .shortcut-title {
            color: white;
        }

        .edit-mode-toggle {
            text-align: right;
            margin-bottom: 20px;
        }

        .btn-edit-mode {
            background: rgba(255, 255, 255, 0.2);
            border: 1px solid rgba(255, 255, 255, 0.3);
            color: white;
            padding: 10px 20px;
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.3s;
        }

        .btn-edit-mode:hover {
            background: rgba(255, 255, 255, 0.3);
        }

        .btn-edit-mode.active {
            background: #e74c3c;
            border-color: #c0392b;
        }

        .tile-actions {
            display: none;
            position: absolute;
            top: 10px;
            right: 10px;
            gap: 5px;
        }

        .edit-mode .tile-actions {
            display: flex;
        }

        .tile-action-btn {
            width: 30px;
            height: 30px;
            border-radius: 50%;
            border: none;
            background: rgba(0, 0, 0, 0.5);
            color: white;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: all 0.3s;
        }

        .tile-action-btn:hover {
            background: rgba(0, 0, 0, 0.7);
        }

        .user-info-section {
            position: absolute;
            top: 20px;
            right: 20px;            
            color: white;
            display: flex;
            align-items: center;
            gap: 15px;
        }

        .user-name {
            font-weight: 600;
        }

        .btn-logout {
            background: rgba(255, 255, 255, 0.2);
            border: 1px solid rgba(255, 255, 255, 0.3);
            color: white;
            padding: 8px 16px;
            border-radius: 6px;
            text-decoration: none;
            transition: all 0.3s;
        }

        .btn-logout:hover {
            background: rgba(255, 255, 255, 0.3);
            color: white;
        }

        /* 반응형 디자인 */
        @media (max-width: 768px) {
            .shortcuts-grid {
                grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
                gap: 15px;
            }

            .header-section h1 {
                font-size: 32px;
            }

            .shortcut-tile {
                min-height: 150px;
                padding: 20px 15px;
            }

            .shortcut-icon {
                width: 60px;
                height: 60px;
                font-size: 28px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- 사용자 정보 영역 -->
        <div class="user-info-section">
            <span class="user-name">
                <i class="fas fa-user"></i>
                <asp:Label ID="lblUserName" runat="server"></asp:Label>
            </span>
            <a href="Logout.aspx" class="btn-logout">
                <i class="fas fa-sign-out-alt"></i> 로그아웃
            </a>
        </div>

        <div class="main-container">
            <!-- 헤더 영역 -->
            <div class="header-section">
                <h1>
                    K-SHIFT Web Portal
                    <span class="version-badge">ver 1.0</span>
                </h1>
            </div>

            <!-- 검색 영역 -->
            <div class="search-section">
                <div class="search-box">
                    <input type="text" id="txtSearch" placeholder="메뉴 이름 또는 키워드 입력" />
                    <i class="fas fa-search search-icon"></i>
                </div>
            </div>

            <!-- 편집 모드 토글 -->
            <div class="edit-mode-toggle">
                <asp:Button ID="btnEditMode" runat="server" Text="편집 모드" CssClass="btn-edit-mode" OnClick="btnEditMode_Click" />
            </div>

            <!-- 바로가기 그리드 -->
            <div id="shortcutsContainer" class="shortcuts-grid" runat="server">
                <asp:Repeater ID="rptShortcuts" runat="server">
                    <ItemTemplate>
                        <div class="shortcut-tile <%# Eval("IsLocked").ToString() == "Y" ? "locked" : "" %>" 
                             onclick='navigateToUrl("<%# Eval("MenuUrl") %>")'>
                            <div class="tile-actions">
                                <button type="button" class="tile-action-btn" onclick='event.stopPropagation(); editShortcut(<%# Eval("ShortcutOrder") %>);'>
                                    <i class="fas fa-edit"></i>
                                </button>
                                <button type="button" class="tile-action-btn" onclick='event.stopPropagation(); deleteShortcut(<%# Eval("ShortcutOrder") %>);'>
                                    <i class="fas fa-trash"></i>
                                </button>
                            </div>
                            <div class="shortcut-icon" style='background-color: <%# Eval("MenuColor") %>;'>
                                <i class='<%# Eval("MenuIcon") %>'></i>
                            </div>
                            <p class="shortcut-title"><%# Eval("MenuName") %></p>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <!-- 바로가기 추가 타일 -->
                <div class="shortcut-tile add-shortcut-tile" onclick="addNewShortcut()">
                    <div class="shortcut-icon">
                        <i class="fas fa-plus"></i>
                    </div>
                    <p class="shortcut-title">바로가기 추가</p>
                </div>
            </div>
        </div>

        <!-- Hidden Fields for PostBack -->
        <asp:HiddenField ID="hdnAction" runat="server" />
        <asp:HiddenField ID="hdnShortcutOrder" runat="server" />
        <asp:HiddenField ID="hdnEditMode" runat="server" Value="false" />
        <asp:HiddenField ID="hdnMenuName" runat="server" />
        <asp:HiddenField ID="hdnMenuUrl" runat="server" />
        <asp:HiddenField ID="hdnMenuIcon" runat="server" />
        <asp:HiddenField ID="hdnMenuColor" runat="server" />
    </form>

    <!-- Bootstrap JS -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
    
    <script type="text/javascript">
        // 편집 모드 토글
        function toggleEditMode() {
            var container = document.getElementById('<%= shortcutsContainer.ClientID %>');
            var editModeBtn = document.getElementById('<%= btnEditMode.ClientID %>');
            var hdnEditMode = document.getElementById('<%= hdnEditMode.ClientID %>');
            
            if (container.classList.contains('edit-mode')) {
                container.classList.remove('edit-mode');
                editModeBtn.classList.remove('active');
                editModeBtn.value = '편집 모드';
                hdnEditMode.value = 'false';
            } else {
                container.classList.add('edit-mode');
                editModeBtn.classList.add('active');
                editModeBtn.value = '완료';
                hdnEditMode.value = 'true';
            }
        }

        // URL 이동
        function navigateToUrl(url) {
            var hdnEditMode = document.getElementById('<%= hdnEditMode.ClientID %>');
            if (hdnEditMode.value === 'false' && url) {
                window.location.href = url;
            }
        }

        // 바로가기 추가
        function addNewShortcut() {
            var menuName = prompt('메뉴 이름을 입력하세요:');
            if (menuName) {
                var menuUrl = prompt('메뉴 URL을 입력하세요 (예: Default.aspx):', "Default.aspx");
                if (menuUrl) {
                    var menuIcon = prompt('아이콘 클래스를 입력하세요 (예: fa-file, 기본값: fa-star):', 'fa-star');
                    var menuColor = prompt('배경색을 입력하세요 (예: #3498db, 기본값: #3498db):', '#3498db');

                    // HiddenField에 값 설정
                    // ShortcutOrder는 서버에서 자동으로 계산됨 (DB의 MAX + 1)
                    document.getElementById('<%= hdnAction.ClientID %>').value = 'add';
                    document.getElementById('<%= hdnMenuName.ClientID %>').value = menuName;
                    document.getElementById('<%= hdnMenuUrl.ClientID %>').value = menuUrl;
                    document.getElementById('<%= hdnMenuIcon.ClientID %>').value = menuIcon || 'fa-star';
                    document.getElementById('<%= hdnMenuColor.ClientID %>').value = menuColor || '#3498db';

                    // 서버로 전송하여 저장
                    __doPostBack('<%= hdnAction.ClientID %>', '');
                }
            }
        }

        // 바로가기 편집
        function editShortcut(order) {
            var menuName = prompt('새 메뉴 이름을 입력하세요:');
            if (menuName) {
                var menuUrl = prompt('새 메뉴 URL을 입력하세요 (예: Default.aspx):', "Default.aspx");
                if (menuUrl) {
                    var menuIcon = prompt('아이콘 클래스를 입력하세요 (예: fa-file):', 'fa-star');
                    var menuColor = prompt('배경색을 입력하세요 (예: #3498db):', '#3498db');

                    // HiddenField에 값 설정
                    document.getElementById('<%= hdnAction.ClientID %>').value = 'edit';
                    document.getElementById('<%= hdnShortcutOrder.ClientID %>').value = order;
                    document.getElementById('<%= hdnMenuName.ClientID %>').value = menuName;
                    document.getElementById('<%= hdnMenuUrl.ClientID %>').value = menuUrl;
                    document.getElementById('<%= hdnMenuIcon.ClientID %>').value = menuIcon || 'fa-star';
                    document.getElementById('<%= hdnMenuColor.ClientID %>').value = menuColor || '#3498db';

                    // 서버로 전송하여 저장
                    __doPostBack('<%= hdnAction.ClientID %>', '');
                }
            }
        }

        // 바로가기 삭제
        function deleteShortcut(order) {
            if (confirm('이 바로가기를 삭제하시겠습니까?')) {
                document.getElementById('<%= hdnAction.ClientID %>').value = 'delete';
                document.getElementById('<%= hdnShortcutOrder.ClientID %>').value = order;
                alert('바로가기가 삭제되었습니다.');
                __doPostBack('<%= hdnAction.ClientID %>', '');
            }
        }

        // 검색 기능
        document.addEventListener('DOMContentLoaded', function() {
            var searchBox = document.getElementById('txtSearch');
            if (searchBox) {
                searchBox.addEventListener('keyup', function(e) {
                    var searchText = this.value.toLowerCase();
                    var tiles = document.querySelectorAll('.shortcut-tile:not(.add-shortcut-tile)');
                    
                    tiles.forEach(function(tile) {
                        var title = tile.querySelector('.shortcut-title').textContent.toLowerCase();
                        if (title.indexOf(searchText) > -1) {
                            tile.style.display = 'flex';
                        } else {
                            tile.style.display = 'none';
                        }
                    });
                });
            }
        });
    </script>
</body>
</html>
