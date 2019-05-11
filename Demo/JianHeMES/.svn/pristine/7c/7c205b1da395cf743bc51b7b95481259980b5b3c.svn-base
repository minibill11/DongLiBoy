using System.Web;
using System.Web.Optimization;

namespace JianHeMES
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/jquery").Include(
                        "~/Scripts/jquery-1.11.3.js",
                        "~/Scripts/jquery-1.11.3.intellisense.js",
                        "~/Scripts/locale/easyui-lang-zh_CN.js",
                        "~/Scripts/jquery.form.js"
                        ));

            bundles.Add(new ScriptBundle("~/Scripts/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            //bundles.Add(new ScriptBundle("~/Scripts_home").Include(
            //            "~/Scripts/home.js"));

            ////ECharts
            //bundles.Add(new ScriptBundle("~/Scripts/echarts").Include(
            //            "~/Scripts/echarts.js"));

            //easyui
            bundles.Add(new ScriptBundle("~/Scripts/easyui").Include(
                       "~/Scripts/jquery.easyui.min.js",
                       "~/Scripts/jquery.easyui.mobile-1.4.5.js"));


            //// 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            //// 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/Scripts/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-paginator.js"));

            bundles.Add(new ScriptBundle("~/Scripts/vue").Include(
                       "~/Scripts/vue.js",
                       "~/Scripts/vue-dragging.es5.js"));

            bundles.Add(new ScriptBundle("~/Scripts/highcharts").Include(
                      "~/Scripts/highcharts.js",
                      "~/Scripts/highcharts-3d.js",
                      "~/Scripts/modules/exporting.js"));



            bundles.Add(new StyleBundle("~/Content/easyuicss").Include(
                       "~/Content/easyui.css",
                       "~/Content/icon.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-theme.css",
                      "~/Content/Site.css"));


            //启动css，js压缩
            //BundleTable.EnableOptimizations = false;
        }
    }
}
