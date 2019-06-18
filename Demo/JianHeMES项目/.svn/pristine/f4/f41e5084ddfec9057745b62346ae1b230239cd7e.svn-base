using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JianHeMES.Models
{
    public class AllModels
    {
        public IEnumerable<Assemble> Assemble { get; set; }
        public IEnumerable<AssembleLineId> AssembleLineId { get; set; }
        public IEnumerable<ModelCollections> ModelCollections { get; set; }
        public IEnumerable<Waterproofabnormal> Waterproofabnormal { get; set; }
        public IEnumerable<ViewCheckabnormal> ViewCheckabnormal { get; set; }
        public IEnumerable<PQCCheckabnormal> PQCCheckabnormal { get; set; }
        public IEnumerable<Appearance>Appearance { get; set; }
        public IEnumerable<Appearance_OQCCheckAbnormal> Appearance_OQCCheckAbnormal { get; set; }
        public IEnumerable<BarCodes> BarCodes { get; set; }
        public IEnumerable<Burn_in> Burn_in { get; set; }
        public IEnumerable<Burn_in_OQCCheckAbnormal> Burn_in_OQCCheckAbnormal { get; set; }
        public IEnumerable<CalibrationRecord> CalibrationRecord { get; set; }
        public IEnumerable<OrderInformation> OrderInformation { get; set; }
        public IEnumerable<OrderMgm> OrderMgm { get; set; }
        public IEnumerable<Packaging> Packaging { get; set; }
        public IEnumerable<Users> Users { get; set; }
        public IEnumerable<IQCReport> IQCReports { get; set; }

        public IEnumerable<SMT_ProductionPlan> SMT_ProductionPlan { get; set; }

        public IEnumerable<SMT_ProductionLineInfo> SMT_ProcutionLineInfo { get; set; }

        public IEnumerable<SMT_ProductionData> SMT_ProductionData { get; set; }

        public IEnumerable<SMT_OrderInfo> SMT_OrderInfo { get; set; }

        
        public AllModels()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            this.Assemble = db.Assemble.ToList();
            this.AssembleLineId = db.AssembleLineId.ToList();
            this.ModelCollections = db.ModelCollections.ToList();
            this.Waterproofabnormal = db.Waterproofabnormal.ToList();
            this.ViewCheckabnormal = db.ViewCheckabnormal.ToList();
            this.PQCCheckabnormal = db.PQCCheckabnormal.ToList();
            this.Appearance = db.Appearance.ToList();
            this.Appearance_OQCCheckAbnormal = db.Appearance_OQCCheckAbnormal.ToList();
            this.BarCodes = db.BarCodes.ToList();
            this.Burn_in = db.Burn_in.ToList();
            this.Burn_in_OQCCheckAbnormal = db.Burn_in_OQCCheckAbnormal.ToList();
            this.CalibrationRecord = db.CalibrationRecord.ToList();
            this.OrderInformation = db.OrderInformation.ToList();
            this.OrderMgm = db.OrderMgm.ToList();
            this.Packaging = db.Packaging.ToList();
            this.Users = db.Users.ToList();
            this.IQCReports = db.IQCReports.ToList();
            this.SMT_ProductionPlan = db.SMT_ProductionPlan.ToList();
            this.SMT_ProcutionLineInfo = db.SMT_ProductionLineInfo.ToList();
            this.SMT_ProductionData = db.SMT_ProductionData.ToList();
            this.SMT_OrderInfo = db.SMT_OrderInfo.ToList();

        }


    }
}