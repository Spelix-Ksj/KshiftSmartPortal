<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Site.Master" AutoEventWireup="true" CodeBehind="ChatBot.aspx.cs" Inherits="KShiftSmartPortalWeb.ChatBot" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    K-SHIFT Text2SQL
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        <%-- 챗봇 전체 레이아웃 --%>
        .chatbot-wrapper {
            height: calc(100vh - 140px);
            display: flex;
            flex-direction: column;
        }

        <%-- 채팅 패널 --%>
        #chatbot-chat-panel {
            display: flex;
            flex-direction: column;
            height: 100%;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 12px rgba(0,0,0,0.08);
        }

        <%-- 헤더바 --%>
        .chat-header {
            background: #0F4C81;
            color: white;
            padding: 12px 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-shrink: 0;
        }

        .chat-header .chat-title {
            font-size: 17px;
            font-weight: 600;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .chat-header .header-right {
            display: flex;
            align-items: center;
            gap: 14px;
        }

        .gpu-status {
            display: flex;
            align-items: center;
            gap: 6px;
            font-size: 12px;
        }

        .gpu-dot {
            width: 10px;
            height: 10px;
            border-radius: 50%;
            background: #999;
        }

        .gpu-dot.online {
            background: #4CAF50;
            box-shadow: 0 0 6px #4CAF50;
        }

        .gpu-dot.offline {
            background: #F44336;
            box-shadow: 0 0 6px #F44336;
        }

        .btn-chat-logout {
            background: rgba(255,255,255,0.15);
            color: white;
            border: 1px solid rgba(255,255,255,0.3);
            border-radius: 6px;
            padding: 6px 14px;
            font-size: 13px;
            cursor: pointer;
            transition: background 0.2s;
        }

        .btn-chat-logout:hover {
            background: rgba(255,255,255,0.25);
        }

        <%-- 예제 질문바 --%>
        .example-bar {
            background: #F0F4F8;
            padding: 10px 16px;
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
            flex-shrink: 0;
            border-bottom: 1px solid #E0E4E8;
        }

        .example-bar .example-btn {
            background: white;
            border: 1px solid #D0D5DD;
            border-radius: 16px;
            padding: 6px 14px;
            font-size: 12px;
            color: #333;
            cursor: pointer;
            transition: all 0.2s;
            white-space: nowrap;
        }

        .example-bar .example-btn:hover {
            background: #0F4C81;
            color: white;
            border-color: #0F4C81;
        }

        <%-- 메시지 영역 --%>
        #chatbot-messages {
            flex-grow: 1;
            overflow-y: auto;
            padding: 20px;
            background: #F5F5F5;
        }

        <%-- 메시지 버블 공통 --%>
        .msg-bubble {
            max-width: 85%;
            margin-bottom: 16px;
            display: flex;
            flex-direction: column;
        }

        .msg-bubble.user {
            align-self: flex-end;
            margin-left: auto;
        }

        .msg-bubble.assistant,
        .msg-bubble.system,
        .msg-bubble.error {
            align-self: flex-start;
            margin-right: auto;
        }

        .msg-content {
            padding: 12px 16px;
            border-radius: 12px;
            font-size: 14px;
            line-height: 1.6;
            word-break: break-word;
            white-space: pre-wrap;
        }

        .msg-bubble.user .msg-content {
            background: #0F4C81;
            color: white;
            border-bottom-right-radius: 4px;
        }

        .msg-bubble.assistant .msg-content {
            background: #F0F0F0;
            color: #333;
            border-bottom-left-radius: 4px;
        }

        .msg-bubble.system .msg-content {
            background: #E8F5E9;
            color: #333;
            border-bottom-left-radius: 4px;
        }

        .msg-bubble.error .msg-content {
            background: #FFEBEE;
            color: #333;
            border-bottom-left-radius: 4px;
        }

        .msg-timestamp {
            font-size: 11px;
            color: #999;
            margin-top: 4px;
        }

        .msg-bubble.user .msg-timestamp {
            text-align: right;
        }

        <%-- SQL 코드블록 --%>
        .sql-block {
            margin-top: 10px;
            border-radius: 8px;
            overflow: hidden;
        }

        .sql-block-header {
            background: #2D2D2D;
            color: #CCC;
            padding: 6px 12px;
            font-size: 12px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .sql-textarea {
            width: 100%;
            background: #1E1E1E;
            color: #D4D4D4;
            border: none;
            padding: 12px;
            font-family: Consolas, 'Courier New', monospace;
            font-size: 13px;
            line-height: 1.5;
            resize: none;
            outline: none;
            box-sizing: border-box;
            min-height: 60px;
        }

        .sql-actions {
            display: flex;
            gap: 8px;
            padding: 8px 12px;
            background: #2D2D2D;
        }

        .btn-sql-exec {
            background: #4CAF50;
            color: white;
            border: none;
            border-radius: 4px;
            padding: 5px 14px;
            font-size: 12px;
            cursor: pointer;
            transition: background 0.2s;
        }

        .btn-sql-exec:hover {
            background: #43A047;
        }

        .btn-sql-copy {
            background: #757575;
            color: white;
            border: none;
            border-radius: 4px;
            padding: 5px 14px;
            font-size: 12px;
            cursor: pointer;
            transition: background 0.2s;
        }

        .btn-sql-copy:hover {
            background: #616161;
        }

        <%-- 결과 테이블 --%>
        .result-table-wrapper {
            margin-top: 10px;
            max-height: 300px;
            overflow: auto;
            border: 1px solid #D0D5DD;
            border-radius: 6px;
            position: relative;
        }

        .result-table-header {
            background: #E3F2FD;
            padding: 8px 14px;
            font-size: 13px;
            font-weight: 600;
            color: #1565C0;
            border-bottom: 1px solid #D0D5DD;
        }

        .result-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 13px;
            table-layout: auto;
        }

        .result-table th {
            background: #F8F9FA;
            padding: 10px 14px;
            text-align: left;
            border-bottom: 2px solid #D0D5DD;
            font-weight: 600;
            color: #1B2A4A;
            white-space: nowrap;
            position: sticky;
            top: 0;
            z-index: 1;
        }

        .result-table td {
            padding: 8px 14px;
            border-bottom: 1px solid #EAEDF0;
            color: #444;
            white-space: nowrap;
        }

        .result-table tbody tr:nth-child(even) {
            background: #F8FAFB;
        }

        .result-table tbody tr:hover td {
            background: #E8F0FE;
        }

        <%-- 보고서 영역 --%>
        .report-block {
            margin-top: 10px;
            background: #F0F7F1;
            padding: 14px 16px;
            border-radius: 6px;
            border-left: 4px solid #4CAF50;
            font-size: 13px;
            line-height: 1.8;
            color: #333;
            white-space: pre-wrap;
        }

        <%-- 로딩 인디케이터 --%>
        .loading-dots {
            display: inline-flex;
            gap: 4px;
            padding: 8px 0;
        }

        .loading-dots span {
            width: 8px;
            height: 8px;
            border-radius: 50%;
            background: #999;
            animation: dotPulse 1.4s infinite ease-in-out both;
        }

        .loading-dots span:nth-child(1) { animation-delay: -0.32s; }
        .loading-dots span:nth-child(2) { animation-delay: -0.16s; }

        @keyframes dotPulse {
            0%, 80%, 100% { transform: scale(0); }
            40% { transform: scale(1.0); }
        }

        <%-- 입력바 --%>
        .chat-input-bar {
            display: flex;
            gap: 10px;
            padding: 12px 16px;
            background: white;
            border-top: 1px solid #E0E0E0;
            flex-shrink: 0;
            align-items: flex-end;
        }

        .chat-input-bar textarea {
            flex-grow: 1;
            border: 1px solid #D0D5DD;
            border-radius: 12px;
            padding: 10px 14px;
            font-size: 14px;
            resize: none;
            outline: none;
            max-height: 120px;
            min-height: 40px;
            line-height: 1.4;
            font-family: inherit;
        }

        .chat-input-bar textarea:focus {
            border-color: #0F4C81;
        }

        .btn-send {
            background: #0F4C81;
            color: white;
            border: none;
            border-radius: 50%;
            width: 42px;
            height: 42px;
            font-size: 16px;
            cursor: pointer;
            transition: background 0.2s;
            flex-shrink: 0;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .btn-send:hover {
            background: #0D3F6E;
        }

        .btn-send:disabled {
            opacity: 0.5;
            cursor: not-allowed;
        }

        <%-- 반응형 --%>
        @media (max-width: 768px) {
            .chatbot-wrapper {
                height: calc(100vh - 120px);
            }

            .example-bar {
                padding: 8px 10px;
                gap: 6px;
            }

            .example-bar .example-btn {
                font-size: 11px;
                padding: 5px 10px;
            }

            .msg-bubble {
                max-width: 92%;
            }

            .chat-input-bar {
                padding: 8px 10px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PageTitleContent" runat="server">
    Text2SQL ChatBot
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    <div class="chatbot-wrapper">
        <%-- 채팅 패널 --%>
        <div id="chatbot-chat-panel">
            <%-- 헤더바 --%>
            <div class="chat-header">
                <div class="chat-title">
                    <i class="fas fa-robot"></i>
                    K-SHIFT Text2SQL
                </div>
                <div class="header-right">
                    <div class="gpu-status">
                        <span class="gpu-dot" id="gpu-dot"></span>
                        <span id="gpu-label">GPU 확인 중...</span>
                    </div>
                    <button type="button" class="btn-chat-logout" onclick="chatbotLogout()">
                        <i class="fas fa-sign-out-alt"></i> 로그아웃
                    </button>
                </div>
            </div>

            <%-- 예제 질문바 --%>
            <div class="example-bar">
                <button type="button" class="example-btn" onclick="sendQuestion(this.textContent.trim())">전체 호선 목록을 보여줘</button>
                <button type="button" class="example-btn" onclick="sendQuestion(this.textContent.trim())">호선별 블록 수와 중량을 알려줘</button>
                <button type="button" class="example-btn" onclick="sendQuestion(this.textContent.trim())">올해 근무일수는 몇 일이야?</button>
                <button type="button" class="example-btn" onclick="sendQuestion(this.textContent.trim())">공종별 활동 건수를 보여줘</button>
                <button type="button" class="example-btn" onclick="sendQuestion(this.textContent.trim())">컨테이너선 목록을 보여줘</button>
            </div>

            <%-- 메시지 영역 --%>
            <div id="chatbot-messages"></div>

            <%-- 입력바 --%>
            <div class="chat-input-bar">
                <textarea id="chatbot-input" rows="1" placeholder="질문을 입력하세요..." onkeydown="handleInputKeydown(event)"></textarea>
                <button type="button" class="btn-send" id="btn-send" onclick="sendQuestion()">
                    <i class="fas fa-paper-plane"></i>
                </button>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript">
        // ===== 전역 상태 =====
        // 서버 측 프록시를 통한 API 호출 헬퍼
        async function callApi(endpoint, method, body, token) {
            var response = await fetch('ChatBot.aspx/ProxyApi', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    endpoint: endpoint,
                    method: method || 'GET',
                    body: body ? JSON.stringify(body) : '',
                    token: token || ''
                })
            });
            if (!response.ok) {
                throw new Error('서버 오류: ' + response.status);
            }
            var wrapper = await response.json();
            // ASP.NET WebMethod는 결과를 {d: "..."} 형태로 감싸서 반환
            return JSON.parse(wrapper.d);
        }

        let authToken = null;
        let tokenExpiry = null;
        let healthInterval = null;
        let isBusy = false;

        // ===== 유틸리티 함수 =====

        // 고유 메시지 ID 생성
        function generateMsgId() {
            return 'msg-' + Date.now() + '-' + Math.random().toString(36).substr(2, 9);
        }

        // 현재 시각 포맷 (HH:mm)
        function getTimeString() {
            var now = new Date();
            return now.getHours().toString().padStart(2, '0') + ':' + now.getMinutes().toString().padStart(2, '0');
        }

        // 스크롤 하단 이동
        function scrollToBottom() {
            var container = document.getElementById('chatbot-messages');
            if (container) {
                container.scrollTop = container.scrollHeight;
            }
        }

        // 토큰 만료 체크 (5분 버퍼)
        function isTokenExpired() {
            if (!tokenExpiry) return true;
            return Date.now() >= (tokenExpiry - 5 * 60 * 1000);
        }

        // textarea 높이 자동 조절
        function autoResizeTextarea(el) {
            el.style.height = 'auto';
            el.style.height = el.scrollHeight + 'px';
        }

        // XSS 방지용 텍스트 이스케이프
        function escapeHtml(text) {
            var div = document.createElement('div');
            div.textContent = text;
            return div.innerHTML;
        }

        // ===== 챗봇 API 자동 로그인 (페이지 로드 시 호출) =====
        async function chatbotAutoLogin() {
            try {
                var data = await callApi('/api/auth/login', 'POST', { userId: 'admin', password: 'admin' }, '');

                authToken = data.token;
                if (!authToken) throw new Error('토큰을 받지 못했습니다.');

                if (data.expiresAt) {
                    tokenExpiry = new Date(data.expiresAt).getTime();
                } else {
                    var expiresIn = data.expiresIn || 3600;
                    tokenExpiry = Date.now() + (expiresIn * 1000);
                }

                // GPU 상태 체크 시작
                checkHealth();
                healthInterval = setInterval(checkHealth, 30000);

                // 환영 메시지
                addMessage('system', '질문을 입력해주세요. 자연어로 데이터를 조회할 수 있습니다.', {});
            } catch (err) {
                addMessage('error', 'API 서버에 연결할 수 없습니다: ' + err.message, {});
            }
        }

        // ===== 로그아웃 =====
        function chatbotLogout() {
            authToken = null;
            tokenExpiry = null;
            isBusy = false;

            if (healthInterval) {
                clearInterval(healthInterval);
                healthInterval = null;
            }

            document.getElementById('chatbot-messages').innerHTML = '';

            // 챗봇 API 재연결
            chatbotAutoLogin();
        }

        // ===== GPU 상태 체크 =====
        function checkHealth() {
            callApi('/api/chat/health', 'GET', null, authToken)
            .then(function (data) {
                var dot = document.getElementById('gpu-dot');
                var label = document.getElementById('gpu-label');
                var isHealthy = data.status === 'ok' || data.gpu === 'connected';
                if (isHealthy) {
                    dot.className = 'gpu-dot online';
                    label.textContent = 'GPU 정상';
                } else {
                    dot.className = 'gpu-dot offline';
                    label.textContent = 'GPU 비정상';
                }
            })
            .catch(function () {
                var dot = document.getElementById('gpu-dot');
                var label = document.getElementById('gpu-label');
                dot.className = 'gpu-dot offline';
                label.textContent = 'GPU 연결 실패';
            });
        }

        // ===== 메시지 버블 생성 =====
        function addMessage(role, content, extras) {
            var msgId = generateMsgId();
            var container = document.getElementById('chatbot-messages');
            extras = extras || {};

            var bubble = document.createElement('div');
            bubble.className = 'msg-bubble ' + role;
            bubble.id = msgId;

            // 메시지 본문
            var contentDiv = document.createElement('div');
            contentDiv.className = 'msg-content';
            contentDiv.textContent = content || '';
            bubble.appendChild(contentDiv);

            // SQL 코드블록
            if (extras.sql) {
                var sqlBlock = createSqlBlock(msgId, extras.sql);
                bubble.appendChild(sqlBlock);
            }

            // 결과 테이블
            if (extras.columns && extras.rows) {
                var tableContainer = createResultTable(extras.columns, extras.rows);
                while (tableContainer.firstChild) {
                    bubble.appendChild(tableContainer.firstChild);
                }
            }

            // 보고서
            if (extras.report) {
                var reportDiv = document.createElement('div');
                reportDiv.className = 'report-block';
                reportDiv.textContent = extras.report;
                bubble.appendChild(reportDiv);
            }

            // 타임스탬프
            var timeDiv = document.createElement('div');
            timeDiv.className = 'msg-timestamp';
            timeDiv.textContent = getTimeString();
            bubble.appendChild(timeDiv);

            container.appendChild(bubble);
            scrollToBottom();

            return msgId;
        }

        // 기존 버블 업데이트
        function updateMessage(msgId, updates) {
            var bubble = document.getElementById(msgId);
            if (!bubble) return;

            // 본문 업데이트
            if (updates.content !== undefined) {
                var contentDiv = bubble.querySelector('.msg-content');
                if (contentDiv) {
                    contentDiv.textContent = updates.content;
                }
            }

            // 로딩 제거
            if (updates.removeLoading) {
                var loading = bubble.querySelector('.loading-dots');
                if (loading) loading.remove();
            }

            // SQL 추가
            if (updates.sql) {
                // 기존 SQL 블록 제거
                var oldSql = bubble.querySelector('.sql-block');
                if (oldSql) oldSql.remove();

                var sqlBlock = createSqlBlock(msgId, updates.sql);
                var timestamp = bubble.querySelector('.msg-timestamp');
                bubble.insertBefore(sqlBlock, timestamp);
            }

            // 결과 테이블 추가
            if (updates.columns && updates.rows) {
                // 기존 테이블 헤더와 래퍼 모두 제거
                var oldHeader = bubble.querySelector('.result-table-header');
                if (oldHeader) oldHeader.remove();
                var oldTable = bubble.querySelector('.result-table-wrapper');
                if (oldTable) oldTable.remove();

                var tableContainer = createResultTable(updates.columns, updates.rows);
                var timestamp2 = bubble.querySelector('.msg-timestamp');
                // container의 자식들(header + wrapper)을 bubble에 삽입
                while (tableContainer.firstChild) {
                    bubble.insertBefore(tableContainer.firstChild, timestamp2);
                }
            }

            // 보고서 추가
            if (updates.report) {
                var oldReport = bubble.querySelector('.report-block');
                if (oldReport) oldReport.remove();

                var reportDiv = document.createElement('div');
                reportDiv.className = 'report-block';
                reportDiv.textContent = updates.report;
                var timestamp3 = bubble.querySelector('.msg-timestamp');
                bubble.insertBefore(reportDiv, timestamp3);
            }

            // 에러로 변경
            if (updates.role) {
                bubble.className = 'msg-bubble ' + updates.role;
            }

            scrollToBottom();
        }

        // 로딩 메시지 추가
        function addLoadingMessage() {
            var msgId = generateMsgId();
            var container = document.getElementById('chatbot-messages');

            var bubble = document.createElement('div');
            bubble.className = 'msg-bubble assistant';
            bubble.id = msgId;

            var contentDiv = document.createElement('div');
            contentDiv.className = 'msg-content';

            var dots = document.createElement('div');
            dots.className = 'loading-dots';
            for (var i = 0; i < 3; i++) { dots.appendChild(document.createElement('span')); }
            contentDiv.appendChild(dots);

            bubble.appendChild(contentDiv);
            container.appendChild(bubble);
            scrollToBottom();

            return msgId;
        }

        // ===== SQL 코드블록 생성 =====
        function createSqlBlock(msgId, sql) {
            var block = document.createElement('div');
            block.className = 'sql-block';

            var header = document.createElement('div');
            header.className = 'sql-block-header';
            header.textContent = 'SQL';
            block.appendChild(header);

            var textarea = document.createElement('textarea');
            textarea.className = 'sql-textarea';
            textarea.id = 'sql-' + msgId;
            textarea.value = sql;
            textarea.spellcheck = false;
            textarea.addEventListener('input', function () {
                autoResizeTextarea(this);
            });
            block.appendChild(textarea);

            // 초기 높이 조절 (렌더링 후)
            setTimeout(function () {
                autoResizeTextarea(textarea);
            }, 0);

            var actions = document.createElement('div');
            actions.className = 'sql-actions';

            var execBtn = document.createElement('button');
            execBtn.type = 'button';
            execBtn.className = 'btn-sql-exec';
            execBtn.textContent = '실행';
            execBtn.onclick = function () { executeSql(msgId); };
            actions.appendChild(execBtn);

            var copyBtn = document.createElement('button');
            copyBtn.type = 'button';
            copyBtn.className = 'btn-sql-copy';
            copyBtn.textContent = '복사';
            copyBtn.onclick = function () { copySql(msgId); };
            actions.appendChild(copyBtn);

            block.appendChild(actions);
            return block;
        }

        // ===== 결과 테이블 생성 =====
        function createResultTable(columns, rows) {
            var container = document.createElement('div');

            // 건수 헤더 (스크롤 영역 바깥)
            var headerDiv = document.createElement('div');
            headerDiv.className = 'result-table-header';
            headerDiv.textContent = '조회 결과: ' + rows.length + '건';
            container.appendChild(headerDiv);

            // 스크롤 가능한 테이블 영역
            var wrapper = document.createElement('div');
            wrapper.className = 'result-table-wrapper';

            var table = document.createElement('table');
            table.className = 'result-table';

            var thead = document.createElement('thead');
            var headerRow = document.createElement('tr');
            columns.forEach(function (col) {
                var th = document.createElement('th');
                th.textContent = col;
                headerRow.appendChild(th);
            });
            thead.appendChild(headerRow);
            table.appendChild(thead);

            var tbody = document.createElement('tbody');
            rows.forEach(function (row) {
                var tr = document.createElement('tr');
                columns.forEach(function (col, idx) {
                    var td = document.createElement('td');
                    td.textContent = (row[idx] !== null && row[idx] !== undefined) ? String(row[idx]) : '';
                    tr.appendChild(td);
                });
                tbody.appendChild(tr);
            });
            table.appendChild(tbody);

            wrapper.appendChild(table);
            container.appendChild(wrapper);
            return container;
        }

        // ===== 질문 전송 (async/await) =====
        async function sendQuestion(question) {
            // 입력값 가져오기
            if (!question) {
                var inputEl = document.getElementById('chatbot-input');
                question = inputEl.value.trim();
                if (!question) return;
                inputEl.value = '';
                inputEl.style.height = 'auto';
            }

            // 토큰 만료 체크
            if (isTokenExpired()) {
                addMessage('error', '토큰이 만료되었습니다. 다시 로그인해주세요.', {});
                chatbotLogout();
                return;
            }

            if (isBusy) return;
            isBusy = true;
            document.getElementById('btn-send').disabled = true;

            // 사용자 메시지 버블
            addMessage('user', question, {});

            // 로딩 버블
            var loadingId = addLoadingMessage();

            try {
                // Ask API 호출 (서버 프록시 경유)
                var data = await callApi('/api/chat/ask', 'POST', { question: question }, authToken);

                // Ask 응답 처리
                var content = data.content || data.reasoning || data.message || '';
                var sql = data.sql || data.query || '';
                var reasoning = data.reasoning || '';

                // 로딩 버블을 assistant 응답으로 업데이트
                updateMessage(loadingId, {
                    content: reasoning || content || '응답을 받았습니다.',
                    removeLoading: true,
                    sql: sql || undefined
                });

                // 메시지 DOM에 question, reasoning 저장 (재실행 시 사용)
                var bubble = document.getElementById(loadingId);
                if (bubble) {
                    bubble.setAttribute('data-question', question);
                    bubble.setAttribute('data-reasoning', reasoning);
                }

                // SQL이 있으면 자동 실행 (Execute API)
                if (sql) {
                    await executeAskSql(loadingId, sql, question, reasoning);
                }
            } catch (err) {
                updateMessage(loadingId, {
                    content: err.message || '질문 처리 중 오류가 발생했습니다.',
                    removeLoading: true,
                    role: 'error'
                });
            } finally {
                isBusy = false;
                document.getElementById('btn-send').disabled = false;
            }
        }

        // Ask 후 자동 Execute (async/await)
        async function executeAskSql(msgId, sql, question, reasoning) {
            try {
                // Execute API 호출 (서버 프록시 경유)
                var result = await callApi('/api/chat/execute', 'POST', { sql: sql, question: question, reasoning: reasoning, modelKey: null }, authToken);
                var columns = result.columns || [];
                var rows = result.rows || result.data || [];
                var report = result.report || result.summary || '';

                if (columns.length > 0 && rows.length > 0) {
                    updateMessage(msgId, {
                        columns: columns,
                        rows: rows,
                        report: report || undefined
                    });
                } else if (columns.length > 0 && rows.length === 0) {
                    updateMessage(msgId, { report: '조회 결과가 없습니다. 조건을 변경하여 다시 시도해보세요.' });
                } else if (report) {
                    updateMessage(msgId, { report: report });
                } else {
                    updateMessage(msgId, { report: '조회 결과가 없습니다. 조건을 변경하여 다시 시도해보세요.' });
                }
            } catch (err) {
                addMessage('error', 'SQL 실행 오류: ' + (err.message || '알 수 없는 오류'), {});
            }
        }

        // ===== SQL 편집 후 재실행 =====
        function executeSql(msgId) {
            var textarea = document.getElementById('sql-' + msgId);
            if (!textarea) return;

            var sql = textarea.value.trim();
            if (!sql) return;

            if (isTokenExpired()) {
                addMessage('error', '토큰이 만료되었습니다. 다시 로그인해주세요.', {});
                chatbotLogout();
                return;
            }

            // 원래 메시지에서 question, reasoning 가져오기
            var origBubble = document.getElementById(msgId);
            var origQuestion = origBubble ? origBubble.getAttribute('data-question') || '' : '';
            var origReasoning = origBubble ? origBubble.getAttribute('data-reasoning') || '' : '';

            // 재실행 결과를 새 메시지로 추가
            var newMsgId = addMessage('system', 'SQL을 재실행합니다...', {});

            // Execute API 호출 (서버 프록시 경유)
            callApi('/api/chat/execute', 'POST', { sql: sql, question: origQuestion, reasoning: origReasoning, modelKey: null }, authToken)
            .then(function (result) {
                var columns = result.columns || [];
                var rows = result.rows || result.data || [];
                var report = result.report || result.summary || '';

                updateMessage(newMsgId, {
                    content: 'SQL 재실행 완료',
                    sql: sql
                });

                if (columns.length > 0 && rows.length > 0) {
                    updateMessage(newMsgId, {
                        columns: columns,
                        rows: rows,
                        report: report || undefined
                    });
                } else if (columns.length > 0 && rows.length === 0) {
                    updateMessage(newMsgId, { report: '조회 결과가 없습니다. 조건을 변경하여 다시 시도해보세요.' });
                } else if (report) {
                    updateMessage(newMsgId, { report: report });
                } else {
                    updateMessage(newMsgId, { report: '조회 결과가 없습니다. 조건을 변경하여 다시 시도해보세요.' });
                }
            })
            .catch(function (err) {
                updateMessage(newMsgId, {
                    content: 'SQL 실행 오류: ' + (err.message || '알 수 없는 오류'),
                    role: 'error'
                });
            });
        }

        // ===== SQL 복사 =====
        function copySql(msgId) {
            var textarea = document.getElementById('sql-' + msgId);
            if (!textarea) return;

            var sql = textarea.value;
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(sql).then(function () {
                    addMessage('system', 'SQL이 클립보드에 복사되었습니다.', {});
                }).catch(function () {
                    // 폴백: textarea 선택 방식
                    textarea.select();
                    document.execCommand('copy');
                    addMessage('system', 'SQL이 클립보드에 복사되었습니다.', {});
                });
            } else {
                textarea.select();
                document.execCommand('copy');
                addMessage('system', 'SQL이 클립보드에 복사되었습니다.', {});
            }
        }

        // ===== 입력 키 처리 =====
        function handleInputKeydown(e) {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                sendQuestion();
            }
            // Shift+Enter는 기본 동작 (줄바꿈)
        }

        // 페이지 로드 시 초기화
        document.addEventListener('DOMContentLoaded', function () {
            // 챗봇 API 자동 로그인
            chatbotAutoLogin();

            // 입력 textarea 자동 높이 조절
            var input = document.getElementById('chatbot-input');
            if (input) {
                input.addEventListener('input', function () {
                    autoResizeTextarea(this);
                });
            }
        });
    </script>
</asp:Content>
