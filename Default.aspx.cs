using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebFormsAsyncResultTest
{
    public partial class _Default : System.Web.UI.Page, ICallbackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (! IsPostBack && ! string.IsNullOrEmpty(Request.Params["GetResult"]))
            {
                ReturnResultsFile();
            }
        }

        protected void StartButton_Click(object sender, EventArgs e)
        {
            Session["result"] = BeginWork(5);
            WorkerProgressPanel.Visible = true;
        }

        public string GetCallbackResult()
        {
            var res = (IAsyncResult)Session["result"];
            if (res == null)
                return "UNKNOWN";

            if (!res.IsCompleted)
                return "WORKING";

            // async process is finished so clear the session
            Session["result"] = null;

            // check for error
            if (!string.IsNullOrEmpty((string)Session["ResultException"]))
            {
                string errorMsg = (string)Session["ResultException"];
                Session["ResultException"] = null;
                return errorMsg;
            }
            return "SUCCESS";
        }

        public void RaiseCallbackEvent(string args)
        {
        }

        protected delegate void BeginWorkDelegate(int seconds);
        protected IAsyncResult BeginWork(int seconds)
        {
            return new BeginWorkDelegate(DoWork).BeginInvoke(seconds, WorkCallback, null);
        }

        protected void WorkCallback(IAsyncResult iresult)
        {
            ((iresult as AsyncResult).AsyncDelegate as BeginWorkDelegate).EndInvoke(iresult);
        }

        protected void DoWork(int seconds)
        {
            try
            {
                System.Threading.Thread.Sleep(seconds * 1000);
                Session["ResultBytes"] = System.IO.File.ReadAllBytes(Server.MapPath("~/file.xls"));
                Session["ResultAttachmentName"] = "file.xls";
            }
            catch (Exception e)
            {
                Session["ResultException"] = e.ToString();
            }
        }

        protected void ReturnResultsFile()
        {
            byte[] responseBuffer = (byte[])Session["ResultBytes"];
            Session["ResultBytes"] = null;

            string responseAttachmentName = (string) Session["ResultAttachmentName"];

            if (responseBuffer == null || responseAttachmentName == null)
                throw new Exception("Result file not found");

            // Return the workbook to the stream
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", responseAttachmentName));
            Response.AddHeader("Content-Length", responseBuffer.Length.ToString());
            Response.BinaryWrite(responseBuffer);
            Response.End();
        }


    }
}