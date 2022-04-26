using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CheckMailAddress.Models;

namespace CheckMailAddress.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Index(EmailCheck emailCheck)
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
          
            if (GetResponseCode(ResponseString) == 250)
            {
                msg = "Mai Address is exist! "+ ResponseString;
              //  Response.Write(msg);
            }

            else
            {
                msg = "Mai Address Does not Exist!"+ ResponseString;
               // Response.Write(msg);
            }

            ViewBag.msg = msg;
            /* QUITE CONNECTION */
            dataBuffer = BytesFromString("QUITE" + CRLF);
            netStream.Write(dataBuffer, 0, dataBuffer.Length);
            tClient.Close();

            return View();
        }

        private byte[] BytesFromString(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
        private int GetResponseCode(string ResponseString)
        {
            //250 2.1.5 OK tq9si25439722pac.237 - gsmtp
            string st = ResponseString.Substring(0, 3);
            int r= int.Parse(st);
            return r;
        }
    }
}