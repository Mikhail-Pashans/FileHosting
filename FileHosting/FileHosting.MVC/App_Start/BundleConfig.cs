using System.Web.Optimization;

namespace FileHosting.MVC
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            /*-----------------------------jQuery---------------------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-{version}.min.js"));

            /*-----------------------------jQueryUI-------------------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-{version}.js"));

            /*-----------------------------jQueryValidataion----------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            /*-----------------------------Modernizr------------------------------------*/            
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            /*-----------------------------FineUploader---------------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/fineuploader").Include(
                "~/Scripts/all.fineuploader-5.0.2.min.js"));

            bundles.Add(new StyleBundle("~/Content/FineUploader/css").Include(
                "~/Content/FineUploader/fineuploader-5.0.2.min.css"));

            /*-----------------------------Bootstrap------------------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                //"~/Scripts/bootstrap.js",
                "~/Scripts/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/Bootstrap/css").Include(
                "~/Content/Bootstrap/bootstrap.min.css"));

            /*-----------------------------Jumbotron------------------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/jumbotorn").Include(
               "~/Scripts/ie10-viewport-bug-workaround.js",
               "~/Scripts/ie-emulation-modes-warning.js"));

            bundles.Add(new StyleBundle("~/Content/Jumbotron/css").Include(
                "~/Content/Jumbotron/jumbotron.css"));

            /*-----------------------------PagedList------------------------------------*/
            bundles.Add(new StyleBundle("~/Content/PagedList/css").Include(
                "~/Content/PagedList/PagedList.css"));

            /*-----------------------------Content--------------------------------------*/
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                "~/Content/themes/base/jquery.ui.core.css",
                "~/Content/themes/base/jquery.ui.resizable.css",
                "~/Content/themes/base/jquery.ui.selectable.css",
                "~/Content/themes/base/jquery.ui.accordion.css",
                "~/Content/themes/base/jquery.ui.autocomplete.css",
                "~/Content/themes/base/jquery.ui.button.css",
                "~/Content/themes/base/jquery.ui.dialog.css",
                "~/Content/themes/base/jquery.ui.slider.css",
                "~/Content/themes/base/jquery.ui.tabs.css",
                "~/Content/themes/base/jquery.ui.datepicker.css",
                "~/Content/themes/base/jquery.ui.progressbar.css",
                "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}