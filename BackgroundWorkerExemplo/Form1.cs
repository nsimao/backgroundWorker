using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackgroundWorkerExemplo
{
    public partial class Form1 : Form
    {
        BackgroundWorker bgWorker = null;
        delegate void SetTextCallback(Label lbl, string texto);

        public Form1()
        {
            InitializeComponent();
        }

        private void ParaBackGroundWorker()
        {
            if (bgWorker.IsBusy)
                bgWorker.CancelAsync();

            lblStatus.Text = "Execução:";

            progressBar1.Value = 0;

            listBox1.Items.Clear();

            btnIniciar.Enabled = true;
            btnCancelar.Enabled = false;
        }


        #region ThreadSafe
        private void SetLabel(Label lbl, string texto)
        {
            if (lbl.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetLabelThreadSafe);
                this.Invoke(d, new object[] { lbl, texto });
            }
            else
            {
                lbl.Text = texto;
            }
        }

        private void AddToListBox(string texto)
        {
            Invoke(new MethodInvoker(
                           delegate { listBox1.Items.Add(texto); }
                           ));
        }


        private void StepItProgressBar()
        {
            Invoke(new MethodInvoker(
                           delegate { progressBar1.PerformStep(); }
                           ));
        }

        private void SetLabelThreadSafe(Label lbl, string texto)
        {
            lbl.Text = texto;
        }
        #endregion

        #region Handlers
        private void Form1_Load(object sender, EventArgs e)
        {
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += BgWorker_DoWork;
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (1 == 1)
            {
                string texto = "Execução em: " + DateTime.Now.ToString();

                SetLabel(lblStatus, texto);
                AddToListBox(texto);
                StepItProgressBar();

                Thread.Sleep(1000);

                if (bgWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy)
            {
                btnIniciar.Enabled = false;
                btnCancelar.Enabled = true;
                bgWorker.RunWorkerAsync();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            ParaBackGroundWorker();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ParaBackGroundWorker();
        }
        #endregion
    }
}
