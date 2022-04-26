using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CheckMailAddress.Models;

namespace CheckMailAddress.Controllers
{
    public class EmailChecksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EmailChecks
        public async Task<ActionResult> Index()
        {
            return View(await db.EmailChecks.ToListAsync());
        }
        [HttpPost]
        public async Task<ActionResult> Index(EmailCheck emailCheck)
        {
           string email=emailCheck.Email;

            TcpClient tClient = new TcpClient("gmail-smtp-in.l.google.com", 25);
            string CRLF = "\r\n";
            byte[] dataBuffer;
            string ResponseString;
            NetworkStream netStream = tClient.GetStream();
            StreamReader reader = new StreamReader(netStream);
            ResponseString = reader.ReadLine();
            /* Perform HELO to SMTP Server and get Response */
            dataBuffer = BytesFromString("HELO KirtanHere" + CRLF);
            netStream.Write(dataBuffer, 0, dataBuffer.Length);
            ResponseString = reader.ReadLine();
            dataBuffer = BytesFromString("MAIL FROM:<ranahamid007@gmail.com>" + CRLF);
            netStream.Write(dataBuffer, 0, dataBuffer.Length);
            ResponseString = reader.ReadLine();
            /* Read Response of the RCPT TO Message to know from google if it exist or not */
            dataBuffer = BytesFromString("RCPT TO:<" + email.Trim() + ">" + CRLF);
            netStream.Write(dataBuffer, 0, dataBuffer.Length);
            ResponseString = reader.ReadLine();
            string msg = string.Empty;
            if (GetResponseCode(ResponseString) == 550)
            {
                msg = "Mai Address Does not Exist !<br/><br/>"+ "<B><font color='red'>Original Error from Smtp Server :</font></b>" + ResponseString;
                Response.Write(msg);
            }
            ViewBag.msg = msg;
            /* QUITE CONNECTION */
            dataBuffer = BytesFromString("QUITE" + CRLF);
            netStream.Write(dataBuffer, 0, dataBuffer.Length);
            tClient.Close();

            return View(await db.EmailChecks.ToListAsync());
         }

        private byte[] BytesFromString(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
        private int GetResponseCode(string ResponseString)
        {
            return int.Parse(ResponseString.Substring(0, 3));
        }

        // GET: EmailChecks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailCheck emailCheck = await db.EmailChecks.FindAsync(id);
            if (emailCheck == null)
            {
                return HttpNotFound();
            }
            return View(emailCheck);
        }

        // GET: EmailChecks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmailChecks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Email")] EmailCheck emailCheck)
        {
            if (ModelState.IsValid)
            {
                string email = emailCheck.Email;

                TcpClient tClient = new TcpClient("gmail-smtp-in.l.google.com", 25);
                string CRLF = "\r\n";
                byte[] dataBuffer;
                string ResponseString;
                NetworkStream netStream = tClient.GetStream();
                StreamReader reader = new StreamReader(netStream);
                ResponseString = reader.ReadLine();
                /* Perform HELO to SMTP Server and get Response */
                dataBuffer = BytesFromString("HELO KirtanHere" + CRLF);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                ResponseString = reader.ReadLine();
                dataBuffer = BytesFromString("MAIL FROM:<ranahamid007@gmail.com>" + CRLF);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                ResponseString = reader.ReadLine();
                /* Read Response of the RCPT TO Message to know from google if it exist or not */
                dataBuffer = BytesFromString("RCPT TO:<" + email.Trim() + ">" + CRLF);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                ResponseString = reader.ReadLine();
                string msg = string.Empty;
                if (GetResponseCode(ResponseString) == 550)
                {
                    msg = "Mai Address Does not Exist !<br/><br/>" + "<B><font color='red'>Original Error from Smtp Server :</font></b>" + ResponseString;
                    Response.Write(msg);
                }
                ViewBag.msg = msg;
                /* QUITE CONNECTION */
                dataBuffer = BytesFromString("QUITE" + CRLF);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                tClient.Close();


                db.EmailChecks.Add(emailCheck);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(emailCheck);
        }

        // GET: EmailChecks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailCheck emailCheck = await db.EmailChecks.FindAsync(id);
            if (emailCheck == null)
            {
                return HttpNotFound();
            }
            return View(emailCheck);
        }

        // POST: EmailChecks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Email")] EmailCheck emailCheck)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emailCheck).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(emailCheck);
        }

        // GET: EmailChecks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailCheck emailCheck = await db.EmailChecks.FindAsync(id);
            if (emailCheck == null)
            {
                return HttpNotFound();
            }
            return View(emailCheck);
        }

        // POST: EmailChecks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            EmailCheck emailCheck = await db.EmailChecks.FindAsync(id);
            db.EmailChecks.Remove(emailCheck);
            await db.SaveChangesAsync();
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
