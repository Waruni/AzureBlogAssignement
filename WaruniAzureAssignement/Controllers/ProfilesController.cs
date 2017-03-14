using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using WaruniAzureAssignement.Models;

namespace WaruniAzureAssignement.Controllers
{
    public class ProfilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Profiles
        public ActionResult Index()
        {
            return View(db.Profiles.ToList());
        }

        // GET: Profiles/Details/5
        public ActionResult Details(string id=null)
        {
            Profile profile = null;
            if (id == null)
            {
                var email = User.Identity.Name;
                profile = db.Profiles.Where(a => a.Email == email).FirstOrDefault();
                return View(profile);
            }
            profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // GET: Profiles/Create
        public ActionResult Create()
        {
            ViewBag.loggedInUser = User.Identity.Name;
            return View();
        }

        // POST: Profiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProfileId,FirstName,LastName,Address,Email,Telephone,ImageUrl")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                var imageUri = "";
                if (!string.IsNullOrEmpty(profile.ImageUrl))
                {
                    var profileImageName = profile.Email.Split('@')[0];
                    imageUri = UploadPrlofileImage(profile.ImageUrl, profileImageName);
                }
                if (!string.IsNullOrEmpty(imageUri))
                {
                    profile.ImageUrl = imageUri;
                    db.Profiles.Add(profile);
                    db.SaveChanges();
                }
                var id = profile.ProfileId;
                return RedirectToAction("Details", new { id = id });
            }

            return View(profile);
        }
        private string UploadPrlofileImage(string path,string profileImageName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("profilecontainer");
            var imageuri = "";

            container.CreateIfNotExists();
            {
                container.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    }
                    );
            }
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(profileImageName);
            using (var fileStream = System.IO.File.OpenRead(@"C:\PofilesTemp\" + path + ""))
            {
                blockBlob.UploadFromStream(fileStream);
                imageuri = blockBlob.Uri.AbsoluteUri;
            }
            return imageuri;
            
        }

        // GET: Profiles/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProfileId,FirstName,LastName,Address,Email,Telephone,ImageUrl")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = profile.ProfileId });
            }
            return View(profile);
        }

        // GET: Profiles/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Profile profile = db.Profiles.Find(id);
            db.Profiles.Remove(profile);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
