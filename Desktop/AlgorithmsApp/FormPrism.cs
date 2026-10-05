using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using DecisionTreeC45.BLL.Services.Prism;
using DecisionTreeC45.BLL.Models.ModelPrism;

namespace AlgorithmsApp
{
    public partial class FormPrism : Form
    {
        private string _filePath = "";
        private clsPrismEngine? _engine;
        private DataTable? _dataTable;

        private ComboBox cbTargetColumn;
        private RichTextBox rtbRules;

        public FormPrism()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "PRISM Algorithm Dashboard";
            this.Size = new Size(800, 600);
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            Button btnLoad = new Button { Text = "Load Dataset (CSV)", Location = new Point(20, 20), Size = new Size(150, 30) };
            btnLoad.Click += BtnLoad_Click;
            this.Controls.Add(btnLoad);

            Button btnExport = new Button { Text = "Export PDF", Location = new Point(740, 20), Size = new Size(100, 30) };
            btnExport.Click += BtnExport_Click;
            this.Controls.Add(btnExport);

            Label lblTarget = new Label { Text = "Target Column:", Location = new Point(190, 25), AutoSize = true };
            this.Controls.Add(lblTarget);

            cbTargetColumn = new ComboBox { Location = new Point(290, 22), Size = new Size(150, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cbTargetColumn);

            Button btnAnalyze = new Button { Text = "Analyze", Location = new Point(460, 20), Size = new Size(100, 30) };
            btnAnalyze.Click += BtnAnalyze_Click;
            this.Controls.Add(btnAnalyze);

            rtbRules = new RichTextBox { Location = new Point(20, 70), Size = new Size(740, 470), ReadOnly = true };
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
                        _engine = new clsPrismEngine(_filePath);
                        _dataTable = _engine.ReadData();
                        
                        cbTargetColumn.DataSource = _engine.GetColumnNames(_dataTable);
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
                
                rtbRules.Text = "Analyzing with PRISM Algorithm...\n\n";
                
                clsRuleSet ruleSet = (clsRuleSet)_engine.Analyze(_dataTable, targetCol);
                
                DisplayRules(ruleSet);
                MessageBox.Show("Analysis completed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during analysis: " + ex.Message);
            }
        }

        private void DisplayRules(clsRuleSet ruleSet)
        {
            rtbRules.Clear();
            if (ruleSet == null || ruleSet.Rules == null || ruleSet.Rules.Count == 0)
            {
                rtbRules.Text = "No Rules Generated.";
                return;
            }

            int ruleNumber = 1;
            foreach (var rule in ruleSet.Rules)
            {
                string conditions = string.Join(" AND ", System.Linq.Enumerable.Select(rule.Conditions, c => $"{c.AttributeName} {c.Operator} {c.Value}"));
                rtbRules.AppendText($"Rule {ruleNumber}:\r\nIF {conditions} THEN {rule.TargetAttribute} = {rule.PredictedClass}\r\n------------------------------------------------------------\r\n\r\n");
                ruleNumber++;
            }
        }
        
        private void BtnExport_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rtbRules.Text) || rtbRules.Text == "No Rules Generated.")
            {
                MessageBox.Show("No results to export. Please analyze the dataset first.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "PDF Files (*.pdf)|*.pdf", FileName = "PRISM_Results.pdf" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var document = new PdfSharpCore.Pdf.PdfDocument();
                        document.Info.Title = "PRISM Algorithm Results";
                        
                        var page = document.AddPage();
                        var gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
                        var font = new PdfSharpCore.Drawing.XFont("Arial", 11, PdfSharpCore.Drawing.XFontStyle.Regular);
                        
                        int yPos = 40;
                        gfx.DrawString("PRISM Association/Classification Rules", new PdfSharpCore.Drawing.XFont("Arial", 16, PdfSharpCore.Drawing.XFontStyle.Bold), PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XRect(0, 20, page.Width, page.Height), PdfSharpCore.Drawing.XStringFormats.TopCenter);
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
