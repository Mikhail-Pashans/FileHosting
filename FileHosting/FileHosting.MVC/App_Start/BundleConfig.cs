using System.Web.Optimization;

namespace FileHosting.MVC
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            /*-----------------------------jQuery---------------------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jQuery/jquery-{version}.js",
                "~/Scripts/jQuery/jquery-{version}.min.js"));

            /*-----------------------------jQueryUI-------------------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jQueryUI/jquery-ui-{version}.js"));

            /*-----------------------------jQueryValidataion----------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jQueryUnobtrusive/jquery.unobtrusive*",
                "~/Scripts/jQueryValidate/jquery.validate*"));

            /*-----------------------------Modernizr------------------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/Modernizr/modernizr-*"));

            /*-----------------------------FineUploader---------------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/fineuploader").Include(
                "~/Scripts/FineUploader/all.fineuploader-5.0.2.min.js"));

            bundles.Add(new StyleBundle("~/Content/FineUploader/css").Include(
                "~/Content/FineUploader/fineuploader-5.0.2.min.css"));

            /*-----------------------------Bootstrap------------------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/Bootstrap/bootstrap.min.js",
                "~/Scripts/Bootstrap/bootstrap-select.min.js"));

            bundles.Add(new StyleBundle("~/Content/Bootstrap/css").Include(
                "~/Content/Bootstrap/bootstrap.min.css",
                "~/Content/Bootstrap/bootstrap-select.min.css"));

            /*-----------------------------Jumbotron------------------------------------*/
            bundles.Add(new StyleBundle("~/Content/Jumbotron/css").Include(
                "~/Content/Jumbotron/jumbotron.css"));

            /*-----------------------------CKEditor-------------------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/ckeditor-js").Include(
                "~/CKEditor/ckeditor.js"));

            bundles.Add(new ScriptBundle("~/bundles/ckeditor-adapters").Include(
                "~/CKEditor/adapters/jquery.js"));

            bundles.Add(new StyleBundle("~/CKEditor/css").Include(
                "~/CKEditor/contents.css"));

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

            /*-----------------------------CustomScripts--------------------------------*/
            bundles.Add(new ScriptBundle("~/bundles/custom").IncludeDirectory(
                "~/Scripts/CustomScripts", "*.js"));
        }
    }
}