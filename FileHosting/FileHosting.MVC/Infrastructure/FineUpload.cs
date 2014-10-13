using System.IO;
using System.Web.Mvc;

namespace FileHosting.MVC.Infrastructure
{
    [ModelBinder(typeof(ModelBinder))]
    public class FineUpload
    {
        public string FileName { get; set; }
        public string FileSection { get; set; }
        public Stream InputStream { get; set; }

        public void SaveAs(string destination, bool overwrite = false, bool autoCreateDirectory = true)
        {
            if (autoCreateDirectory)
            {
                var directory = new FileInfo(destination).Directory;
                if (directory != null) directory.Create();
            }

            using (var file = new FileStream(destination, overwrite ? FileMode.Create : FileMode.CreateNew))
                InputStream.CopyTo(file);
        }

        public class ModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.RequestContext.HttpContext.Request;
                var formUpload = request.Files.Count > 0;

                //find file section
                string formFileSection = request["fileSection"];

                // find file name
                string xFileName = request.Headers["X-File-Name"];
                string qqFile = request["qqfile"];
                string qqFileName = request["qqfilename"];
                string formFileName = formUpload ? request.Files[0].FileName : null;

                FineUpload upload = new FineUpload
                {
                    FileSection = formFileSection,
                    FileName = xFileName ?? qqFile ?? qqFileName ?? formFileName,
                    InputStream = formUpload ? request.Files[0].InputStream : request.InputStream
                };

                return upload;
            }
        }
    }
}