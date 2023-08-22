using Microsoft.AspNetCore.Mvc;
using NPOI.XSSF.UserModel;
using System.Web;

namespace OneService.Controllers
{
	public class FileUploadController : Controller
	{
        private readonly IWebHostEnvironment _HostEnvironment;
        public IActionResult Index()
		{
			return View();
		}

		[RequestSizeLimit(100 * 1024 * 1024)]
		[HttpPost]
        public ActionResult UploadFile(IFormFile upload, bool needResize, int? width, int? height)
        {
            try
            {
                CkFileBean bean = new CkFileBean();
                if (upload != null)
                {
                    string webRootPath = _HostEnvironment.WebRootPath + "/upload";

                    string fileId = Guid.NewGuid().ToString();
                    string fileOrgName = upload.FileName;
                    string fileName = fileId + Path.GetExtension(upload.FileName);
                    string path = Path.Combine(webRootPath, fileName);
                    bean.fileId = fileId;
                    bean.fileName = fileName;
                    bean.fileOrgName = fileOrgName;
                    bean.url = "https://www.etatung.com/upload/" + fileName;
                    bean.uploaded = 1;
                    using (Stream fileStream = new FileStream(path, FileMode.Create))
                    {
                        upload.CopyToAsync(fileStream);
                    }
                    //縮圖
                    //if (needResize)
                    //{
                    //    Image img = resizeImageFromFile(path, (int)width, (int)height, true, true);
                    //    img.Save(path);
                    //    GetPicThumbnail(path, path, 80);
                    //}

                }
                ViewBag.Message = "File Uploaded Successfully!!";
                return Json(bean);
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }


		public struct CkFileBean
        {
            public int uploaded;
            public string fileName;
            public string fileOrgName;
            public string url;
            public string fileId;
        }
    }
}
