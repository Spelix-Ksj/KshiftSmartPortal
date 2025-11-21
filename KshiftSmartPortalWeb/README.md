# K-SHIFT Portal Management System

## 프로젝트 개요
DevExpress 25.1.6을 사용한 ASP.NET Web Forms 기반 공급망 관리(SCM) 블록 계약 관리 시스템

## 주요 기능
### 1. 사용자 인증
- 회사별 로그인 관리
- 세션 기반 인증
- 비밀번호 실패 횟수 제한 (5회)
- 계정 자동 잠금 기능

### 2. 메인 바로가기 화면 (Home.aspx)
- 사용자별 맞춤형 바로가기 타일 (최대 8개)
- 실시간 검색 기능
- 편집 모드를 통한 바로가기 관리
- 타일 색상 및 아이콘 커스터마이징
- 잠금 기능으로 중요 바로가기 보호

### 3. 계약 관리
- SCM_CONTRACT_DETAIL 테이블 조회
- DevExpress GridView 기반 데이터 표시
- 엑셀 내보내기 기능
- 다양한 검색 조건 지원

## 기술 스택
- **Framework**: .NET Framework 4.8
- **UI Library**: DevExpress ASP.NET 25.1.6
- **Database**: Oracle Database (Oracle.ManagedDataAccess 23.5.1)
- **Frontend**: Bootstrap 5.1.3, Font Awesome 6.0.0
- **Development Tool**: Visual Studio 2022

## 프로젝트 구조
```
ScmBlockContractWeb/
├── Views/
│   ├── Login.aspx                 # 로그인 페이지
│   ├── Home.aspx                  # 메인 바로가기 화면
│   ├── Default.aspx               # 계약 관리 화면
│   └── Logout.aspx                # 로그아웃 처리
├── Controllers/
│   ├── LoginController.cs         # 로그인 비즈니스 로직
│   ├── ShortcutController.cs      # 바로가기 관리 로직
│   └── ContractController.cs      # 계약 데이터 관리
├── Models/
│   ├── ScmContractDetail.cs       # 계약 데이터 모델
│   └── UserShortcut.cs            # 바로가기 데이터 모델
├── Web.config                     # 애플리케이션 설정
└── Database_Scripts.sql           # Oracle 테이블 생성 스크립트
```

## 데이터베이스 테이블
### TCM_USER_SHORTCUT
사용자별 바로가기 설정을 저장하는 테이블
- USER_ID: 사용자 ID
- COMPANY_NO: 회사 번호
- SHORTCUT_ORDER: 바로가기 순서 (1-8)
- MENU_NAME: 메뉴 이름
- MENU_URL: 메뉴 URL
- MENU_ICON: Font Awesome 아이콘 클래스
- MENU_COLOR: 타일 배경 색상
- IS_LOCKED: 잠금 여부

## 설치 및 실행 방법

### 1. 데이터베이스 설정
```sql
-- Database_Scripts.sql 실행
-- TCM_USER_SHORTCUT 테이블 생성
-- 기본 바로가기 데이터 삽입
```

### 2. 프로젝트 설정
1. Visual Studio 2022에서 솔루션 파일 열기
2. NuGet 패키지 복원
3. Web.config에서 Oracle 연결 문자열 확인
4. 빌드 및 실행

### 3. 기본 로그인 정보
- 회사: 1002 (SPELIX)
- 사용자: user01
- 비밀번호: (데이터베이스에서 설정)

## 주요 페이지 설명

### Home.aspx (메인 바로가기)
- 로그인 성공 후 첫 화면
- 사용자별 맞춤형 바로가기 타일 표시
- 편집 모드로 바로가기 추가/수정/삭제 가능
- 검색 기능으로 원하는 메뉴 빠르게 찾기

### Login.aspx
- 회사 선택 및 사용자 인증
- 테마 선택 기능
- 비밀번호 실패 시 자동 계정 잠금

### Default.aspx
- 계약 데이터 조회 및 관리
- DevExpress GridView로 데이터 표시
- 엑셀 내보내기 기능

## 보안 기능
- SQL Injection 방지 (Parameterized Query 사용)
- 세션 기반 인증 관리
- 비밀번호 실패 횟수 제한
- 계정 자동 잠금 기능
- 입력값 유효성 검증

## 개발 참고사항
- MVC 패턴 사용 (Model-View-Controller)
- Controller를 통한 비즈니스 로직 분리
- Oracle Parameterized Query로 안전한 데이터베이스 접근
- DevExpress 서버 사이드 기능 우선 사용
- Bootstrap Grid System으로 반응형 레이아웃 구현

## 버전 정보
- Version: 1.0
- Last Updated: 2025
- Copyright © SPELIX 2025