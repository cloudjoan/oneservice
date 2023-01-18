using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using OneService.Models;
using OneService.Utils;

namespace OneService.Controllers
{
	public class AjaxController : Controller
	{
        private readonly IWebHostEnvironment _HostEnvironment;

        TSTIONEContext oneDB = new TSTIONEContext();

        public AjaxController(IWebHostEnvironment hostingEnvironment)
        {
            _HostEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
		{
			return View();
		}

        //[HttpPost]
        //public ActionResult AjaxFileUpload(IFormFile upload, bool needResize, int? width, int? height)
        //{
        //    try
        //    {
        //        CkFileBean bean = new CkFileBean();
        //        if (upload != null)
        //        {
        //            string webRootPath = _HostEnvironment.WebRootPath + "/upload";

        //            string fileId = Guid.NewGuid().ToString();
        //            string fileOrgName = upload.FileName;
        //            string fileName = fileId + Path.GetExtension(upload.FileName);
        //            string path = Path.Combine(webRootPath, fileName);
        //            bean.fileId = fileId;
        //            bean.fileName = fileName;
        //            bean.fileOrgName = fileOrgName;
        //            bean.url = "https://www.etatung.com/upload/" + fileName;
        //            bean.uploaded = 1;
        //            using (Stream fileStream = new FileStream(path, FileMode.Create))
        //            {
        //                upload.CopyToAsync(fileStream);
        //            }
        //            //縮圖
        //            //if (needResize)
        //            //{
        //            //    Image img = resizeImageFromFile(path, (int)width, (int)height, true, true);
        //            //    img.Save(path);
        //            //    GetPicThumbnail(path, path, 80);
        //            //}

        //        }
        //        ViewBag.Message = "File Uploaded Successfully!!";
        //        return Json(bean);
        //    }
        //    catch
        //    {
        //        ViewBag.Message = "File upload failed!!";
        //        return View();
        //    }
        //}



        [HttpPost]
        public async Task<ActionResult> AjaxFileUpload()
        {
            TbOneDocument docBean = new TbOneDocument();
            string webRootPath = _HostEnvironment.WebRootPath + "/files";

            try
            {
                foreach (var file in Request.Form.Files)
                {

                    System.Diagnostics.Debug.WriteLine(file.FileName);

                    string fileId = Guid.NewGuid().ToString();
                    string fileOrgName = file.FileName;
                    string fileName = fileId + Path.GetExtension(file.FileName);
                    string path = Path.Combine(webRootPath, fileName);

                    docBean.Id = new Guid(fileId);
                    docBean.FileName = fileName;
                    docBean.FileOrgName = fileOrgName;
                    docBean.FileExt = Path.GetExtension(file.FileName);
                    docBean.InsertTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
                    oneDB.TbOneDocuments.Add(docBean);
                    oneDB.SaveChanges();

                    using (Stream fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            catch
            {

            }

            return Json(docBean);
        }


        public ActionResult GetFileData(string fileId)
        {
            if (!string.IsNullOrEmpty(fileId))
            {
                var fileBean = oneDB.TbOneDocuments.FirstOrDefault(x => x.Id == new Guid(fileId));
                return Json(fileBean);
            }
            return Json(null);
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
