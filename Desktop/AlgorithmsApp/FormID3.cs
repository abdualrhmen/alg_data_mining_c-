using System;
using System.Drawing;
using System.Windows.Forms;
using DecisionTreeC45.BLL.Services.ID3;
using DecisionTreeC45.BLL.Models.ModelC45;
using System.Data;
using System.Linq;

namespace AlgorithmsApp
{
    public partial class FormID3 : Form
    {
        private string _filePath = "";
        private clsID3Engine? _engine;
        private DataTable? _dataTable;

        private ComboBox cbTargetColumn;
        private TreeView tvDecisionTree;
        private RichTextBox rtbRules;

        public FormID3()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "ID3 Decision Tree Dashboard";
            this.Size = new Size(900, 650);
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

            Label lblTree = new Label { Text = "Decision Tree:", Location = new Point(20, 60), AutoSize = true };
            this.Controls.Add(lblTree);

            tvDecisionTree = new TreeView { Location = new Point(20, 80), Size = new Size(400, 500) };
            this.Controls.Add(tvDecisionTree);

            Label lblRules = new Label { Text = "Generated Rules:", Location = new Point(440, 60), AutoSize = true };
            this.Controls.Add(lblRules);

            rtbRules = new RichTextBox { Location = new Point(440, 80), Size = new Size(420, 500), ReadOnly = true };
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
                        _engine = new clsID3Engine();
                        // ID3Engine doesnt have ReadData internally that takes a path directly in constructor (I removed it).
                        // I need to use the CsvReader.
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
                clsAnalysisResult result = (clsAnalysisResult)_engine.Analyze(_dataTable, targetCol);

                RenderTree(tvDecisionTree, result.RootNode);
                DisplayRules(result);

                MessageBox.Show("Analysis completed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during analysis: " + ex.Message);
            }
        }

        private void RenderTree(TreeView tv, clsDecisionTreeNode rootNode)
        {
            tv.Nodes.Clear();
            if (rootNode != null)
            {
                TreeNode winNode = new TreeNode(rootNode.AttributeName ?? rootNode.ClassLabel ?? "Root");
                AddChildNodes(winNode, rootNode);
                tv.Nodes.Add(winNode);
                tv.ExpandAll();
            }
        }

        private void AddChildNodes(TreeNode winNode, clsDecisionTreeNode logicNode)
        {
            if (logicNode.Children != null)
            {
                foreach (var child in logicNode.Children)
                {
                    string text = child.BranchValue != null ? $"[{child.BranchValue}] " : "";
                    text += child.AttributeName ?? child.ClassLabel ?? "Node";
                    TreeNode childWinNode = new TreeNode(text);
                    AddChildNodes(childWinNode, child);
                    winNode.Nodes.Add(childWinNode);
                }
            }
        }

        private void DisplayRules(clsAnalysisResult result)
        {
            rtbRules.Clear();
            if (result == null || result.Rules == null || result.Rules.Count == 0)
            {
                rtbRules.Text = "No Rules Generated.";
                return;
            }

            int ruleNumber = 1;
            foreach (string rule in result.Rules)
            {
                rtbRules.AppendText($"Rule {ruleNumber}\r\n{rule}\r\n------------------------------------------------------------\r\n\r\n");
                ruleNumber++;
            }
        }
    }
}
