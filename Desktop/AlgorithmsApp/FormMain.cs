using System;
using System.Drawing;
using System.Windows.Forms;

namespace AlgorithmsApp
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Algorithms Platform";
            this.Size = new Size(400, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label lblTitle = new Label();
            lblTitle.Text = "Select Algorithm";
            lblTitle.Font = new Font("Arial", 16, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(100, 30);
            this.Controls.Add(lblTitle);

            Button btnC45 = new Button();
            btnC45.Text = "C4.5 Decision Tree";
            btnC45.Size = new Size(200, 50);
            btnC45.Location = new Point(90, 90);
            btnC45.Click += BtnC45_Click;
            this.Controls.Add(btnC45);

            Button btnPrism = new Button();
            btnPrism.Text = "PRISM Algorithm";
            btnPrism.Size = new Size(200, 50);
            btnPrism.Location = new Point(90, 160);
            btnPrism.Click += BtnPrism_Click;
            this.Controls.Add(btnPrism);

            Button btnID3 = new Button();
            btnID3.Text = "ID3 Decision Tree";
            btnID3.Size = new Size(200, 50);
            btnID3.Location = new Point(90, 230);
            btnID3.Click += BtnID3_Click;
            this.Controls.Add(btnID3);

            Button btnNaiveBayes = new Button();
            btnNaiveBayes.Text = "Naive Bayes Algorithm";
            btnNaiveBayes.Size = new Size(200, 50);
            btnNaiveBayes.Location = new Point(90, 300);
            btnNaiveBayes.Click += BtnNaiveBayes_Click;
            this.Controls.Add(btnNaiveBayes);

            Button btnApriori = new Button();
            btnApriori.Text = "Apriori Algorithm";
            btnApriori.Size = new Size(200, 50);
            btnApriori.Location = new Point(90, 370);
            btnApriori.Click += BtnApriori_Click;
            this.Controls.Add(btnApriori);

            Button btnFPGrowth = new Button();
            btnFPGrowth.Text = "FP-Growth Algorithm";
            btnFPGrowth.Size = new Size(200, 50);
            btnFPGrowth.Location = new Point(90, 440);
            btnFPGrowth.Click += BtnFPGrowth_Click;
            this.Controls.Add(btnFPGrowth);
            Button btnEclat = new Button();
            btnEclat.Text = "ECLAT Algorithm";
            btnEclat.Size = new Size(200, 50);
            btnEclat.Location = new Point(90, 510);
            btnEclat.Click += BtnEclat_Click;
            this.Controls.Add(btnEclat);
        }

        private void BtnEclat_Click(object? sender, EventArgs e)
        {
            FormEclat form = new FormEclat();
            form.ShowDialog();
        }

        private void BtnFPGrowth_Click(object? sender, EventArgs e)
        {
            FormFPGrowth form = new FormFPGrowth();
            form.ShowDialog();
        }

        private void BtnApriori_Click(object? sender, EventArgs e)
        {
            FormApriori form = new FormApriori();
            form.ShowDialog();
        }

        private void BtnNaiveBayes_Click(object? sender, EventArgs e)
        {
            FormNaiveBayes form = new FormNaiveBayes();
            form.ShowDialog();
        }

        private void BtnID3_Click(object? sender, EventArgs e)
        {
            FormID3 form = new FormID3();
            form.ShowDialog();
        }

        private void BtnC45_Click(object? sender, EventArgs e)
        {
            FormC45 form = new FormC45();
            form.ShowDialog();
        }

        private void BtnPrism_Click(object? sender, EventArgs e)
        {
            FormPrism form = new FormPrism();
            form.ShowDialog();
        }
    }
}
