-- ToDoList 마스터 데이터 (SCM_CASE_MASTER, STD_PERSONNEL_INFO)
-- ToDoList_TestData.sql 실행 전에 먼저 실행해야 함
-- 실행일: 2025-12-08

-- ========================================
-- SCM_CASE_MASTER (케이스 마스터 1건)
-- ========================================
INSERT INTO SCM_CASE_MASTER (
    CASE_NO, COMPANY_NO, CASE_TYPE, CASE_NAME, CASE_STATUS,
    APPST_DT, USE_YN, VIEW_ORDER, RMK,
    IN_USER, IN_DATE, IN_PID
) VALUES (
    'CASE_IMG', '100', 'TODO', 'ToDoList 테스트 케이스', 'ACTIVE',
    TO_DATE('2025-12-01', 'YYYY-MM-DD'), 'Y', 1, 'ToDoList 기능 테스트용 케이스',
    'Spelix', SYSDATE, 'ToDoList_MasterData'
);

-- ========================================
-- STD_PERSONNEL_INFO (직원 정보 12건)
-- ========================================

-- E000001 박상훈
INSERT INTO STD_PERSONNEL_INFO (
    COMPANY_NO, EMP_NO, EMP_NAME, USER_ID, USER_NAME,
    ORG_NO, JOB, JOB_STATUS, RANK, JOB_POS,
    EMAIL_ADDR, TEL_NO, ENTRY_YEAR, RMK,
    IN_USER, IN_DATE, IN_PID
) VALUES (
    '100', 'E000001', '박상훈', 'psh001', '박상훈',
    'DEV', '개발', '재직', '부장', '개발팀장',
    'psh001@kshift.co.kr', '010-1234-0001', '2015', '개발팀 총괄',
    'Spelix', SYSDATE, 'ToDoList_MasterData'
);

-- E000002 김성주
INSERT INTO STD_PERSONNEL_INFO (
    COMPANY_NO, EMP_NO, EMP_NAME, USER_ID, USER_NAME,
    ORG_NO, JOB, JOB_STATUS, RANK, JOB_POS,
    EMAIL_ADDR, TEL_NO, ENTRY_YEAR, RMK,
    IN_USER, IN_DATE, IN_PID
) VALUES (
    '100', 'E000002', '김성주', 'ksj002', '김성주',
    'DEV', '개발', '재직', '차장', '선임개발자',
    'ksj002@kshift.co.kr', '010-1234-0002', '2017', '백엔드 개발 담당',
    'Spelix', SYSDATE, 'ToDoList_MasterData'
);

-- E000003 박성진
INSERT INTO STD_PERSONNEL_INFO (
    COMPANY_NO, EMP_NO, EMP_NAME, USER_ID, USER_NAME,
    ORG_NO, JOB, JOB_STATUS, RANK, JOB_POS,
    EMAIL_ADDR, TEL_NO, ENTRY_YEAR, RMK,
    IN_USER, IN_DATE, IN_PID
) VALUES (
    '100', 'E000003', '박성진', 'psj003', '박성진',
    'DOC', '기술문서', '재직', '과장', '기술문서팀원',
    'psj003@kshift.co.kr', '010-1234-0003', '2018', '기술 문서 작성 담당',
    'Spelix', SYSDATE, 'ToDoList_MasterData'
);

-- E000004 윤다솜
INSERT INTO STD_PERSONNEL_INFO (
    COMPANY_NO, EMP_NO, EMP_NAME, USER_ID, USER_NAME,
    ORG_NO, JOB, JOB_STATUS, RANK, JOB_POS,
    EMAIL_ADDR, TEL_NO, ENTRY_YEAR, RMK,
    IN_USER, IN_DATE, IN_PID
) VALUES (
    '100', 'E000004', '윤다솜', 'yds004', '윤다솜',
    'DESIGN', '디자인', '재직', '대리', 'UI/UX디자이너',
    'yds004@kshift.co.kr', '010-1234-0004', '2020', 'UI/UX 디자인 담당',
    'Spelix', SYSDATE, 'ToDoList_MasterData'
);

-- E000005 이창근
INSERT INTO STD_PERSONNEL_INFO (
    COMPANY_NO, EMP_NO, EMP_NAME, USER_ID, USER_NAME,
    ORG_NO, JOB, JOB_STATUS, RANK, JOB_POS,
    EMAIL_ADDR, TEL_NO, ENTRY_YEAR, RMK,
    IN_USER, IN_DATE, IN_PID
) VALUES (
    '100', 'E000005', '이창근', 'lcg005', '이창근',
    'DATA', '데이터분석', '재직', '과장', '데이터분석가',
    'lcg005@kshift.co.kr', '010-1234-0005', '2019', '데이터 분석 및 리포팅 담당',
    'Spelix', SYSDATE, 'ToDoList_MasterData'
);

-- E000006 선우진성
INSERT INTO STD_PERSONNEL_INFO (
    COMPANY_NO, EMP_NO, EMP_NAME, USER_ID, USER_NAME,
    ORG_NO, JOB, JOB_STATUS, RANK, JOB_POS,
    EMAIL_ADDR, TEL_NO, ENTRY_YEAR, RMK,
    IN_USER, IN_DATE, IN_PID
) VALUES (
    '100', 'E000006', '선우진성', 'swjs006', '선우진성',
    'DEV', '개발', '재직', '차장', '시스템연동개발자',
    'swjs006@kshift.co.kr', '010-1234-0006', '2016', '외부 시스템 연동 개발 담당',
    'Spelix', SYSDATE, 'ToDoList_MasterData'
);

-- E000007 김태영
INSERT INTO STD_PERSONNEL_INFO (
    COMPANY_NO, EMP_NO, EMP_NAME, USER_ID, USER_NAME,
    ORG_NO, JOB, JOB_STATUS, RANK, JOB_POS,
    EMAIL_ADDR, TEL_NO, ENTRY_YEAR, RMK,
    IN_USER, IN_DATE, IN_PID
) VALUES (
    '100', 'E000007', '김태영', 'kty007', '김태영',
    'SEC', '보안', '재직', '과장', '보안담당자',
    'kty007@kshift.co.kr', '010-1234-0007', '2018', '시스템 보안 담당',
    'Spelix', SYSDATE, 'ToDoList_MasterData'
);

-- E000008 윤찬후
INSERT INTO STD_PERSONNEL_INFO (
    COMPANY_NO, EMP_NO, EMP_NAME, USER_ID, USER_NAME,
    ORG_NO, JOB, JOB_STATUS, RANK, JOB_POS,
    EMAIL_ADDR, TEL_NO, ENTRY_YEAR, RMK,
    IN_USER, IN_DATE, IN_PID
) VALUES (
    '100', 'E000008', '윤찬후', 'ych008', '윤찬후',
    'DBA', 'DBA', '재직', '차장', 'DBA',
    'ych008@kshift.co.kr', '010-1234-0008', '2017', '데이터베이스 관리 및 튜닝 담당',
    'Spelix', SYSDATE, 'ToDoList_MasterData'
);

-- E000009 김종수
INSERT INTO STD_PERSONNEL_INFO (
    COMPANY_NO, EMP_NO, EMP_NAME, USER_ID, USER_NAME,
    ORG_NO, JOB, JOB_STATUS, RANK, JOB_POS,
    EMAIL_ADDR, TEL_NO, ENTRY_YEAR, RMK,
    IN_USER, IN_DATE, IN_PID
) VALUES (
    '100', 'E000009', '김종수', 'kjs009', '김종수',
    'QA', 'QA', '재직', '대리', 'QA엔지니어',
    'kjs009@kshift.co.kr', '010-1234-0009', '2021', '품질 보증 및 테스트 담당',
    'Spelix', SYSDATE, 'ToDoList_MasterData'
);

-- E000010 박상은
INSERT INTO STD_PERSONNEL_INFO (
    COMPANY_NO, EMP_NO, EMP_NAME, USER_ID, USER_NAME,
    ORG_NO, JOB, JOB_STATUS, RANK, JOB_POS,
    EMAIL_ADDR, TEL_NO, ENTRY_YEAR, RMK,
    IN_USER, IN_DATE, IN_PID
) VALUES (
    '100', 'E000010', '박상은', 'pse010', '박상은',
    'OPS', '운영', '재직', '과장', '시스템운영자',
    'pse010@kshift.co.kr', '010-1234-0010', '2019', '시스템 배포 및 운영 담당',
    'Spelix', SYSDATE, 'ToDoList_MasterData'
);

-- E000011 권수현
INSERT INTO STD_PERSONNEL_INFO (
    COMPANY_NO, EMP_NO, EMP_NAME, USER_ID, USER_NAME,
    ORG_NO, JOB, JOB_STATUS, RANK, JOB_POS,
    EMAIL_ADDR, TEL_NO, ENTRY_YEAR, RMK,
    IN_USER, IN_DATE, IN_PID
) VALUES (
    '100', 'E000011', '권수현', 'ksh011', '권수현',
    'CS', '고객지원', '재직', '대리', '고객지원담당',
    'ksh011@kshift.co.kr', '010-1234-0011', '2022', '고객 문의 대응 및 지원 담당',
    'Spelix', SYSDATE, 'ToDoList_MasterData'
);

-- E000012 이순영
INSERT INTO STD_PERSONNEL_INFO (
    COMPANY_NO, EMP_NO, EMP_NAME, USER_ID, USER_NAME,
    ORG_NO, JOB, JOB_STATUS, RANK, JOB_POS,
    EMAIL_ADDR, TEL_NO, ENTRY_YEAR, RMK,
    IN_USER, IN_DATE, IN_PID
) VALUES (
    '100', 'E000012', '이순영', 'lsy012', '이순영',
    'PM', 'PM', '재직', '차장', 'PM',
    'lsy012@kshift.co.kr', '010-1234-0012', '2016', '프로젝트 관리 담당',
    'Spelix', SYSDATE, 'ToDoList_MasterData'
);

COMMIT;

-- ========================================
-- 데이터 삭제 쿼리 (테스트 후 정리용)
-- ========================================
-- DELETE FROM STD_PERSONNEL_INFO WHERE COMPANY_NO = '100' AND EMP_NO LIKE 'E00%' AND IN_PID = 'ToDoList_MasterData';
-- DELETE FROM SCM_CASE_MASTER WHERE CASE_NO = 'CASE_IMG' AND COMPANY_NO = '100';
-- COMMIT;
