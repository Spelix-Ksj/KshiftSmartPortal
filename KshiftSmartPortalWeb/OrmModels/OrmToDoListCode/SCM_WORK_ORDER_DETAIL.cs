using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace KShiftSmartPortal.Database
{

    public partial class SCM_WORK_ORDER_DETAIL
    {
        public SCM_WORK_ORDER_DETAIL(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
