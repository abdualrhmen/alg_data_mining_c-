using System;
using System.Drawing;
using System.Windows.Forms;
using DecisionTreeC45.BLL.Services.NaiveBayes;
using DecisionTreeC45.BLL.Models.ModelNaiveBayes;
using System.Data;
using System.Linq;

namespace AlgorithmsApp
{
    public partial class FormNaiveBayes : Form
    {
        private string _filePath = "";
        private clsNaiveBayesEngine? _engine;
        private DataTable? _dataTable;

        private ComboBox cbTargetColumn;
        private RichTextBox rtbRules; // Using this to show Probabilities

        public FormNaiveBayes()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Naive Bayes Dashboard";
            this.Size = new Size(800, 650);
            this.StartPosition = FormStartPosition.CenterParent;

            Button btnLoad = new Button { Text = "Load Dataset (CSV)", Location = new Point(20, 20), Size = new Size(150, 30) };
            btnLoad.Click += BtnLoad_Click;
            this.Controls.Add(btnLoad);

            Label lblTarget = new Label { Text = "Target Column:", Location = new Point(190, 25), AutoSize = true };
            this.Controls.Add(lblTarget);

            cbTargetColumn = new ComboBox { Location = new Point(290, 22), Size = new Size(150, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cbTargetColumn);

            Button btnAnalyze = new Button { Text = "Analyze", Location = new Point(460, 20), Size = new Size(100, 30) };
            btnAnalyze.Click += BtnAnalyze_Click;
            this.Controls.Add(btnAnalyze);

            Button btnExport = new Button { Text = "Export PDF", Location = new Point(700, 20), Size = new Size(80, 30) };
            btnExport.Click += BtnExport_Click;
            this.Controls.Add(btnExport);

            Label lblRules = new Label { Text = "Probabilities Model:", Location = new Point(20, 60), AutoSize = true };
            this.Controls.Add(lblRules);

            rtbRules = new RichTextBox { Location = new Point(20, 80), Size = new Size(740, 500), ReadOnly = true, Font = new Font("Consolas", 10) };
            this.Controls.Add(rtbRules);
        }

        private void BtnLoad_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "CSV Files (*.csv)|*.csv" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _filePath = ofd.FileName;
                    try
                    {
                        _engine = new clsNaiveBayesEngine();
                        _dataTable = DecisionTreeC45.DAL.clsCsvReader.Read(_filePath);
                        cbTargetColumn.DataSource = _dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
                        MessageBox.Show($"Dataset loaded successfully. Rows: {_dataTable.Rows.Count}, Columns: {_dataTable.Columns.Count}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading dataset: " + ex.Message);
                    }
                }
            }
        }

        private void BtnAnalyze_Click(object? sender, EventArgs e)
        {
            if (_engine == null || _dataTable == null)
            {
                MessageBox.Show("Please load a dataset first.");
                return;
            }

            if (cbTargetColumn.SelectedItem == null)
            {
                MessageBox.Show("Please select a target column.");
                return;
            }

            try
            {
                string targetCol = cbTargetColumn.SelectedItem.ToString() ?? "";
                clsNaiveBayesResult result = (clsNaiveBayesResult)_engine.Analyze(_dataTable, targetCol);

                DisplayModel(result);

                MessageBox.Show("Analysis completed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during analysis: " + ex.Message);
            }
        }

        private void DisplayModel(clsNaiveBayesResult result)
        {
            rtbRules.Clear();
            if (result == null || result.PriorProbabilities.Count == 0)
            {
                rtbRules.Text = "No Model Generated.";
                return;
            }

            rtbRules.AppendText("=========================================\n");
            rtbRules.AppendText("       PRIOR PROBABILITIES P(C)\n");
            rtbRules.AppendText("=========================================\n");
            foreach (var kvp in result.PriorProbabilities)
            {
                rtbRules.AppendText($"P({kvp.Key}) = {kvp.Value:0.0000}\n");
            }
            
            rtbRules.AppendText("\n=========================================\n");
            rtbRules.AppendText("     CONDITIONAL PROBABILITIES P(X|C)\n");
            rtbRules.AppendText("=========================================\n");

            foreach (var feature in result.ConditionalProbabilities)
            {
                rtbRules.AppendText($"\n--- Feature: {feature.Key} ---\n");
                foreach (var val in feature.Value)
                {
                    foreach (var cls in val.Value)
                    {
                        rtbRules.AppendText($"P({feature.Key}={val.Key} | {cls.Key}) = {cls.Value:0.0000}\n");
                    }
                }
            }
        }
        
        private void BtnExport_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rtbRules.Text) || rtbRules.Text == "No Model Generated.")
            {
                MessageBox.Show("No results to export. Please analyze the dataset first.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "PDF Files (*.pdf)|*.pdf", FileName = "NaiveBayes_Results.pdf" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var document = new PdfSharpCore.Pdf.PdfDocument();
                        document.Info.Title = "Naive Bayes Algorithm Results";
                        
                        var page = document.AddPage();
                        var gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
                        var font = new PdfSharpCore.Drawing.XFont("Consolas", 10, PdfSharpCore.Drawing.XFontStyle.Regular);
                        
                        int yPos = 40;
                        gfx.DrawString("Naive Bayes Probabilities Model", new PdfSharpCore.Drawing.XFont("Arial", 16, PdfSharpCore.Drawing.XFontStyle.Bold), PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XRect(0, 20, page.Width, page.Height), PdfSharpCore.Drawing.XStringFormats.TopCenter);
                        yPos += 40;

                        string[] lines = rtbRules.Text.Split('\n');
                        foreach (var line in lines)
                        {
                            if (yPos > page.Height - 50)
                            {
                                page = document.AddPage();
                                gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
                                yPos = 40;
                            }
                            gfx.DrawString(line.Replace("\r", ""), font, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XPoint(40, yPos));
                            yPos += 15;
                        }

                        document.Save(sfd.FileName);
                        MessageBox.Show("PDF exported successfully!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error exporting PDF: " + ex.Message);
                    }
                }
            }
        }
    }
}
