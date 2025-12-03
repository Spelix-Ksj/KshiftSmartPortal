using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace KShiftSmartPortal.Database
{

    public partial class SCM_CASE_MASTER
    {
        public SCM_CASE_MASTER(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
