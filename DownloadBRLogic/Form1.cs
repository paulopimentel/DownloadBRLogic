using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace DownloadBRLogic
{
    public partial class Form1 : Form
    {
        string path = @"c:\MusicasBRLogic\";
        object token = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
                MessageBox.Show("Diretório: " + path + " criado com sucesso.", "Atenção!", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
            }
        }

        private void btnExecutar_Click(object sender, EventArgs e)
        {
            try
            {
                WebClient client = new WebClient();
                token = webBrowser1.Document.InvokeScript("eval", new object[] { "RFM_AUTH_PARAM" });

                var structureFather = webBrowser1.Document.GetElementById("radio-file-manager");
                var structureFiles = structureFather.Children[4].Children[0].Children;
                HtmlElementCollection structureDirectory = null;
                string subPath = string.Empty;
                int qtdMusic = structureFiles.Count;
                int musicDownload = 0;
                DateTime startDownload = DateTime.Now;
                DateTime endDownload = DateTime.Now;

                foreach (HtmlElement item in structureFiles)
                {
                    if (item.Id.Contains("folder"))
                    {
                        qtdMusic--;
                        continue;
                    }

                    if (structureDirectory == null)
                    {
                        structureDirectory = structureFather.Children[3].Children[1].GetElementsByTagName("a");
                        string directory = structureDirectory[structureDirectory.Count - 1].InnerText;
                        subPath = System.IO.Path.Combine(path, directory);

                        if (!System.IO.Directory.Exists(path + @"\" + directory))
                        {
                            System.IO.Directory.CreateDirectory(subPath);
                            MessageBox.Show("Sub Diretório: " + subPath + " criado com sucesso.", "Atenção!", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
                            startDownload = DateTime.Now;
                        }
                    }

                    string identify = item.GetAttribute("data-id");
                    string dirMusicFull = subPath + @"\" + item.GetElementsByTagName("a")[0].InnerText + ".mp3";
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)");

                    if (System.IO.File.Exists(dirMusicFull))
                    {
                        continue;
                    }

                    musicDownload++;

                    this.lblInicio.Visible = true;
                    this.lblInicio.Text = "Iniciado às " + startDownload.ToString("HH:mm tt");
                    this.lblInicio.Refresh();

                    this.lblProgresso.Visible = true;
                    this.lblProgresso.Text = "Baixando " + musicDownload.ToString() + " de " + qtdMusic.ToString() + " músicas";
                    this.lblProgresso.Refresh();

                    client.DownloadFile(@"https://servidor23.brlogic.com/index.php/163212/radio-file-manager/download?" + token + "&file=" + identify, dirMusicFull);
                }

                if (musicDownload > 0)
                {
                    endDownload = DateTime.Now;
                    TimeSpan duracao = endDownload - startDownload;
                    StreamWriter sw = File.AppendText(subPath + @"\Log.txt");
                    sw.WriteLine
                        (
                            "Download iniciado às " + startDownload + " e finalizado às " + endDownload +
                            " | Duração do download: " + duracao.ToString(@"hh\:mm\:ss") +
                            " | " + musicDownload + " músicas baixadas."
                        );
                    sw.Close();
                    MessageBox.Show("Download finalizado com sucesso.\n\nVocê baixou " + musicDownload + " músicas em " + duracao.ToString(@"hh\:mm\:ss") + ".", "Atenção!", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Não foi baixado nenhuma música, verifique se você carregou a página toda até o rodapé!", "Atenção!", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Exclamation);
                }

                this.lblProgresso.Text = "";
                this.lblProgresso.Visible = false;
                this.lblInicio.Text = "";
                this.lblInicio.Visible = false;
            }

            catch (Exception ex)
            {
                MessageBox.Show("Provávelmente você não está na página de repositório de música, favor acesse e tente novamente!", "ERRO!", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
            }

        }
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
