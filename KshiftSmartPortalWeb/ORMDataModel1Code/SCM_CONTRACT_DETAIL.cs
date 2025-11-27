using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
namespace KShiftSmartPortal.Database
{

    public partial class SCM_CONTRACT_DETAIL
    {
        public SCM_CONTRACT_DETAIL(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
