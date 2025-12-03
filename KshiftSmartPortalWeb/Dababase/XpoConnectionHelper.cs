using System;
using System.Configuration;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;

namespace KShiftSmartPortal.Database
{
    /// <summary>
    /// XPO 통합 연결 헬퍼 클래스
    /// 여러 ORM Data Model의 Entity를 통합 관리합니다.
    /// </summary>
    public static class XpoConnectionHelper
    {
        // ========================================
        // 모든 ORM Model의 Persistent Types 통합
        // ========================================
        private static readonly Type[] persistentTypes = new Type[]
        {
            // ORMDataModel1 - 계약 관련
            typeof(SCM_CONTRACT_MASTER),
            typeof(SCM_CONTRACT_DETAIL),
            
            // OrmToDoList - 작업지시 관련
            typeof(SCM_CASE_MASTER),
            typeof(SCM_WORK_ORDER_DETAIL),
            typeof(SCM_WORK_ORDER_MASTER),
            typeof(STD_PERSONNEL_INFO)
        };

        /// <summary>
        /// 등록된 모든 Persistent Type 반환
        /// </summary>
        public static Type[] GetPersistentTypes()
        {
            Type[] copy = new Type[persistentTypes.Length];
            Array.Copy(persistentTypes, copy, persistentTypes.Length);
            return copy;
        }

        /// <summary>
        /// Oracle 연결 문자열 (Web.config의 oracleCon 사용)
        /// </summary>
        public static string ConnectionString
        {
            // get { return ConfigurationManager.ConnectionStrings["oracleCon"]?.ConnectionString; }
            get { return System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; }
        }

        /// <summary>
        /// XPO 기본 연결 설정
        /// </summary>
        /// <param name="autoCreateOption">테이블 자동 생성 옵션</param>
        /// <param name="threadSafe">멀티스레드 지원 여부</param>
        public static void Connect(AutoCreateOption autoCreateOption = AutoCreateOption.None, bool threadSafe = true)
        {
            if (threadSafe)
            {
                // ThreadSafe 모드: 웹 애플리케이션 권장
                var provider = XpoDefault.GetConnectionProvider(ConnectionString, autoCreateOption);
                var dictionary = new ReflectionDictionary();
                dictionary.GetDataStoreSchema(persistentTypes);
                XpoDefault.DataLayer = new ThreadSafeDataLayer(dictionary, provider);
            }
            else
            {
                // 단일 스레드 모드
                XpoDefault.DataLayer = XpoDefault.GetDataLayer(ConnectionString, autoCreateOption);
            }
            XpoDefault.Session = null;
        }

        /// <summary>
        /// DataLayer 인스턴스 생성 (개별 세션용)
        /// </summary>
        public static IDataLayer GetDataLayer(AutoCreateOption autoCreateOption = AutoCreateOption.None)
        {
            var dictionary = new ReflectionDictionary();
            dictionary.GetDataStoreSchema(persistentTypes);
            var provider = XpoDefault.GetConnectionProvider(ConnectionString, autoCreateOption);
            return new ThreadSafeDataLayer(dictionary, provider);
        }


        public static IDataStore GetConnectionProvider(AutoCreateOption autoCreateOption)
        {
            return XpoDefault.GetConnectionProvider(ConnectionString, autoCreateOption);
        }
        public static IDataStore GetConnectionProvider(AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect)
        {
            return XpoDefault.GetConnectionProvider(ConnectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
        }

        /// <summary>
        /// 새 UnitOfWork 생성
        /// </summary>
        public static UnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(XpoDefault.DataLayer);
        }

        /// <summary>
        /// 새 Session 생성
        /// </summary>
        public static Session CreateSession()
        {
            return new Session(XpoDefault.DataLayer);
        }
    }
}