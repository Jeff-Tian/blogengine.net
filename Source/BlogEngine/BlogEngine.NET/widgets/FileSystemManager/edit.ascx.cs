// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The edit.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgets.FileSystemManager
{
    using System;

    using App_Code.Controls;
    using System.IO;
    using System.Web.UI.WebControls;

    /// <summary>
    /// The edit.
    /// </summary>
    public partial class Edit : WidgetEditBase
    {
        #region Public Methods

        /// <summary>
        /// Saves this the basic widget settings such as the Title.
        /// </summary>
        public override void Save()
        {
            var settings = this.GetSettings();
            //settings["content"] = this.txtText.Text;
            this.SaveSettings(settings);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            var settings = this.GetSettings();
            //this.txtText.Text = settings["content"];
            InitInterface();
            base.OnInit(e);
        }

        private void InitInterface()
        {
            this.txtCurrentPath.Text = Server.MapPath("/").ToString();
            UpdateExplorer(this.txtCurrentPath.Text.Trim());
        }

        private void UpdateExplorer(string path)
        {
            UpdateExplorer(new DirectoryInfo(path));
        }

        private void UpdateExplorer(DirectoryInfo di)
        {
            if (di != null && di.Exists)
            {
                this.gvSubDirectories.DataSource = di.GetDirectories();
                this.gvSubDirectories.DataBind();
                this.gvFiles.DataSource = di.GetFiles();
                this.gvFiles.DataBind();
            }
        }

        private void GotoDirectory(DirectoryInfo di)
        {
            if (di != null && di.Exists)
            {
                this.txtCurrentPath.Text = di.FullName;
                UpdateExplorer(di);
            }
        }

        private void DownloadFile(string fullPath)
        {
            FileInfo fi = new FileInfo(fullPath);
            this.txtLog.Text += string.Format("\r\nTrying to download file {0}...\r\n", fullPath);
            if (fi.Exists)
            {
                try
                {
                    Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}", Path.GetFileName(fullPath)));
                    Response.TransmitFile(fullPath);
                    Response.End();
                }
                catch (Exception ex)
                {
                    this.txtLog.Text += string.Format("\r\n{0}\r\n{1}", ex.Message, ex.StackTrace);
                }
            }
            else
            {
                this.txtLog.Text += string.Format("\r\nFile {0} does not exist.\r\n", fullPath);
            }
        }

        #endregion

        protected void btnGo_Click(object sender, EventArgs e)
        {
            GotoDirectory(new DirectoryInfo(this.txtCurrentPath.Text.Trim()));
        }

        protected void btnUp_Click(object sender, EventArgs e)
        {
            string currentPath = this.txtCurrentPath.Text.Trim();
            DirectoryInfo di = new DirectoryInfo(currentPath);
            if (di.Exists)
            {
                try
                {
                    di = di.Parent;
                    this.txtCurrentPath.Text = di.FullName.ToString();
                    UpdateExplorer(di.FullName);
                }
                catch (Exception ex)
                {
                    this.txtLog.Text += ex.Message;
                    this.txtLog.Text += "\r\n";
                    this.txtLog.Text += ex.StackTrace;
                }
            }
        }

        protected void lnkName_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton link = (LinkButton)sender;
                GotoDirectory(new DirectoryInfo(Path.Combine(this.txtCurrentPath.Text.Trim(), link.Text)));
            }
            catch (Exception ex)
            {
                this.txtLog.Text += ex.Message;
                this.txtLog.Text += "\r\n";
                this.txtLog.Text += ex.StackTrace;
            }
        }

        protected void lnkFileName_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton linkButton = sender as LinkButton;
                string fullName = Path.Combine(this.txtCurrentPath.Text.Trim(), linkButton.Text);
                DownloadFile(fullName);
            }
            catch (Exception ex)
            {
                this.txtLog.Text += ex.Message;
                this.txtLog.Text += "\r\n";
                this.txtLog.Text += ex.StackTrace;
            }
        }

        protected void btnCreateNewDirectory_Click(object sender, EventArgs e)
        {
            string directoryName = this.txtNewDirectoryName.Text.Trim();
            try
            {
                Directory.CreateDirectory(Path.Combine(this.txtCurrentPath.Text.Trim(), directoryName));
            }
            catch (Exception ex)
            {
                this.txtLog.Text += string.Format("\r\n{0}\r\n{1}\r\n", ex.Message, ex.StackTrace);
            }
            finally
            {
                UpdateExplorer(this.txtCurrentPath.Text.Trim());
            }
        }

        protected void gvSubDirectories_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridView gv = sender as GridView;
            try
            {
                LinkButton link = gv.Rows[e.RowIndex].FindControl("lnkName") as LinkButton;
                if (link != null)
                {
                    Directory.Delete(Path.Combine(this.txtCurrentPath.Text.Trim(), link.Text.Trim()), true);
                }
            }
            catch (Exception ex)
            {
                this.txtLog.Text += string.Format("\r\n{0}\r\n{1}\r\n", ex.Message, ex.StackTrace);
            }
            finally
            {
                UpdateExplorer(this.txtCurrentPath.Text.Trim());
            }
        }

        protected void gvFiles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            GridView gv = sender as GridView;
            try
            {
                LinkButton link = gv.Rows[e.RowIndex].FindControl("lnkFileName") as LinkButton;
                if (link != null)
                {
                    File.Delete(Path.Combine(this.txtCurrentPath.Text.Trim(), link.Text.Trim()));
                }
            }
            catch (Exception ex)
            {
                this.txtLog.Text += string.Format("\r\n{0}\r\n{1}\r\n", ex.Message, ex.StackTrace);
            }
            finally
            {
                UpdateExplorer(this.txtCurrentPath.Text.Trim());
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (this.fileUpload.HasFile)
            {
                string fileName = fileUpload.FileName;
                try
                {
                    fileUpload.SaveAs(Path.Combine(this.txtCurrentPath.Text.Trim(), fileName));
                }
                catch (Exception ex)
                {
                    this.txtLog.Text += string.Format("\r\n{0}\r\n{1}\r\n", ex.Message, ex.StackTrace);
                }
                finally
                {
                    UpdateExplorer(this.txtCurrentPath.Text.Trim());
                }
            }
        }
    }
}