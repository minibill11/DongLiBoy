using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JianHeMES.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据。若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<Assemble> Assemble { get; set; }
        public DbSet<Waterproofabnormal> Waterproofabnormal { get; set; }
        public DbSet<ViewCheckabnormal> ViewCheckabnormal { get; set; }
        public DbSet<PQCCheckabnormal> PQCCheckabnormal { get; set; }
        public DbSet<BarCodes> BarCodes { get; set; }
        public DbSet<OrderMgm> OrderMgm { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<OrderInformation> OrderInformation { get; set; }
        public DbSet<CalibrationRecord> CalibrationRecord { get; set; }
        public DbSet<ModelCollections> ModelCollections { get; set; }
        public DbSet<AdapterCard_Power_Collection> AdapterCard_Power_Collection { get; set; }
        public DbSet<AssembleLineId> AssembleLineId { get; set; }
        public DbSet<Burn_in> Burn_in { get; set; }
        public DbSet<Burn_in_OQCCheckAbnormal> Burn_in_OQCCheckAbnormal { get; set; }
        public DbSet<Appearance> Appearance { get; set; }
        public DbSet<Appearance_OQCCheckAbnormal> Appearance_OQCCheckAbnormal { get; set; }
        public DbSet<Packaging> Packaging{ get; set; }
        public DbSet<Packaging_OQCCheckAbnormal> Packaging_OQCCheckAbnormal { get; set; }
        public DbSet<IQCReport> IQCReports { get; set; }

        public DbSet<SMT_ProductionPlan> SMT_ProductionPlan { get; set; }

        public DbSet<SMT_ProcutionLineInfo> SMT_ProcutionLineInfo { get; set; }

        public DbSet<SMT_ProductionData> SMT_ProductionData { get; set; }

        public DbSet<SMT_OrderInfo> SMT_OrderInfo { get; set; }
        
        public object BarCode { get; internal set; }
        public Task TNews { get; internal set; }

        //创建数据库操作上下文，EF需要这个文件来创建和访问数据库
        //完成后需要重新编译项目（快捷键Ctrl+Shift+B），否则下面添加控制器时会出错。

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        //public System.Data.Entity.DbSet<JianHeMES.Models.Packaging> Packagings { get; set; }

        //public System.Data.Entity.DbSet<JianHeMES.Models.Appearance> Appearances { get; set; }

        //public System.Data.Entity.DbSet<JianHeMES.Models.Burn_in> Burn_in { get; set; }
    }
}