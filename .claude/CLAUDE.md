# K-SHIFT Portal 프로젝트

## 기술 스택
- ASP.NET Web Forms
- DevExpress Bootstrap v25.1.6.0
- Oracle Database (Oracle.ManagedDataAccess)
- Bootstrap 5.3

## 코드 컨벤션
- ASPX 주석: `<%-- 주석 --%>`
- 한글 주석 필수
- MVC 패턴 준수

## 주요 패턴
- BootstrapGridView + PopupEditForm 편집
- List<ViewModel> 기반 세션 캐싱
- 복합키는 KeyFieldName에 세미콜론 구분

## 디렉토리 구조
- /Views - ASPX 페이지
- /Controllers - 비즈니스 로직
- /Models - XPO 엔티티
- /ViewModels - 화면용 DTO