using System;
using System.Drawing;
using System.Windows.Forms;
using DecisionTreeC45.BLL.Services.Eclat;
using DecisionTreeC45.BLL.Models.ModelApriori; // ECLAT uses same output models
using System.Data;

namespace AlgorithmsApp
{
    public partial class FormEclat : Form
    {
        private string _filePath = "";
        private clsEclatEngine? _engine;
        private DataTable? _dataTable;

        private NumericUpDown numMinSupport;
        private NumericUpDown numMinConfidence;
        private RichTextBox rtbResults;

        public FormEclat()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "ECLAT Association Rules Dashboard";
            this.Size = new Size(800, 650);
            this.StartPosition = FormStartPosition.CenterParent;

            Button btnLoad = new Button { Text = "Load Dataset (CSV)", Location = new Point(20, 20), Size = new Size(150, 30) };
            btnLoad.Click += BtnLoad_Click;
            this.Controls.Add(btnLoad);

            Label lblSupport = new Label { Text = "Min Support (0-1):", Location = new Point(190, 25), AutoSize = true };
            this.Controls.Add(lblSupport);

            numMinSupport = new NumericUpDown { Location = new Point(300, 22), Size = new Size(70, 30), DecimalPlaces = 2, Increment = 0.05M, Minimum = 0.01M, Maximum = 1.0M, Value = 0.20M };
            this.Controls.Add(numMinSupport);

            Label lblConfidence = new Label { Text = "Min Confidence (0-1):", Location = new Point(390, 25), AutoSize = true };
            this.Controls.Add(lblConfidence);

            numMinConfidence = new NumericUpDown { Location = new Point(520, 22), Size = new Size(70, 30), DecimalPlaces = 2, Increment = 0.05M, Minimum = 0.01M, Maximum = 1.0M, Value = 0.50M };
            this.Controls.Add(numMinConfidence);

            Button btnAnalyze = new Button { Text = "Analyze", Location = new Point(620, 20), Size = new Size(100, 30) };
            btnAnalyze.Click += BtnAnalyze_Click;
            this.Controls.Add(btnAnalyze);

            Label lblResults = new Label { Text = "Frequent Itemsets & Association Rules (Vertical Format / Intersections):", Location = new Point(20, 60), AutoSize = true };
            this.Controls.Add(lblResults);

            rtbResults = new RichTextBox { Location = new Point(20, 80), Size = new Size(740, 500), ReadOnly = true, Font = new Font("Consolas", 10) };
            this.Controls.Add(rtbResults);
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
                        _engine = new clsEclatEngine();
                        _dataTable = DecisionTreeC45.DAL.clsCsvReader.Read(_filePath);
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

            try
            {
                _engine.MinSupport = (double)numMinSupport.Value;
                _engine.MinConfidence = (double)numMinConfidence.Value;

                rtbResults.Text = "Running ECLAT Algorithm using Vertical Data Format...\n";
                Application.DoEvents(); 

                clsAprioriResult result = (clsAprioriResult)_engine.Analyze(_dataTable, "");

                DisplayResults(result);

                MessageBox.Show("Analysis completed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during analysis: " + ex.Message);
            }
        }

        private void DisplayResults(clsAprioriResult result)
        {
            rtbResults.Clear();
            if (result == null)
            {
                rtbResults.Text = "No Result Generated.";
                return;
            }

            rtbResults.AppendText($"Dataset Rows (Transactions): {result.Dataset?.Rows.Count ?? 0}\n");
            rtbResults.AppendText($"Min Support: {numMinSupport.Value}\n");
            rtbResults.AppendText($"Min Confidence: {numMinConfidence.Value}\n\n");

            rtbResults.AppendText("=========================================\n");
            rtbResults.AppendText($"  FREQUENT ITEMSETS (Total: {result.FrequentItemsets.Count})\n");
            rtbResults.AppendText("=========================================\n");
            foreach (var itemset in result.FrequentItemsets)
            {
                rtbResults.AppendText($"{itemset} (Count: {itemset.SupportCount})\n");
            }
            
            rtbResults.AppendText("\n=========================================\n");
            rtbResults.AppendText($"  ASSOCIATION RULES (Total: {result.AssociationRules.Count})\n");
            rtbResults.AppendText("=========================================\n");

            foreach (var rule in result.AssociationRules)
            {
                rtbResults.AppendText($"{rule}\n");
            }
        }
    }
}
