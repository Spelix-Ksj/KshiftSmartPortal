# SCM Block Contract Web Application

## 프로젝트 개요
DevExpress 24.2.10을 사용한 ASP.NET Web Forms 프로젝트

## 개발 환경
- Visual Studio 2022
- .NET Framework 4.8
- DevExpress ASP.NET 24.2.10

## 프로젝트 구조
```
ScmBlockContractWeb/
├── Default.aspx              # 메인 UI 페이지
├── Default.aspx.cs           # Code-behind
├── Web.config                # 설정 파일
├── Models/
│   └── ScmContractDetail.cs  # 데이터 모델
└── ScmBlockContractWeb.csproj # 프로젝트 파일
```

## 설치 방법
1. Visual Studio 2022에서 ScmBlockContractWeb.csproj 파일 열기
2. NuGet 패키지 복원
3. Web.config에서 DB 연결 문자열 확인
4. 프로젝트 빌드 및 실행

## 주요 기능
- 조회 조건 설정 (Company 구분, Company, 케이스, 날짜 범위)
- SCM_CONTRACT_DETAIL 테이블 조회
- DevExpress GridView로 결과 표시
