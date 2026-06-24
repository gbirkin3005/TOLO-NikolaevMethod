namespace LaserWeldingCalculator
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            groupBoxResults = new GroupBox();
            txtResults = new TextBox();
            btnExportResults = new Button();
            groupBoxTables = new GroupBox();
            tabControlTables = new TabControl();
            tabPageXAxis = new TabPage();
            btnExportXAxis = new Button();
            dgvXAxis = new DataGridView();
            tabPageYAxis = new TabPage();
            btnExportYAxis = new Button();
            dgvYAxis = new DataGridView();
            groupBoxCalcSettings = new GroupBox();
            label13 = new Label();
            label14 = new Label();
            label12 = new Label();
            label11 = new Label();
            lblPointCount = new Label();
            numPointCount = new NumericUpDown();
            numStartX = new NumericUpDown();
            numEndX = new NumericUpDown();
            numStartY = new NumericUpDown();
            numEndY = new NumericUpDown();
            btnCalculate = new Button();
            groupBoxInput = new GroupBox();
            btnShowTemperaturePoints = new Button();
            btnShowGraphs = new Button();
            lblMaterial = new Label();
            cboMaterial = new ComboBox();
            chkManualInput = new CheckBox();
            label10 = new Label();
            txtHeatTransferCoeff = new TextBox();
            label9 = new Label();
            txtThermalExpansion = new TextBox();
            label8 = new Label();
            txtThermalDiffusivity = new TextBox();
            label7 = new Label();
            txtVolumetricHeatCapacity = new TextBox();
            label6 = new Label();
            txtThermalConductivity = new TextBox();
            label5 = new Label();
            txtElasticModulus = new TextBox();
            label4 = new Label();
            txtPower = new TextBox();
            label3 = new Label();
            txtSpeed = new TextBox();
            label2 = new Label();
            txtWidthB2 = new TextBox();
            label1 = new Label();
            txtWidthB1 = new TextBox();
            lblThickness = new Label();
            txtThickness = new TextBox();
            saveFileDialog = new SaveFileDialog();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBoxResults.SuspendLayout();
            groupBoxTables.SuspendLayout();
            tabControlTables.SuspendLayout();
            tabPageXAxis.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvXAxis).BeginInit();
            tabPageYAxis.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvYAxis).BeginInit();
            groupBoxCalcSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numPointCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStartX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numEndX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numStartY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numEndY).BeginInit();
            groupBoxInput.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            resources.ApplyResources(tabControl1, "tabControl1");
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            resources.ApplyResources(tabPage1, "tabPage1");
            tabPage1.Controls.Add(groupBoxResults);
            tabPage1.Controls.Add(groupBoxTables);
            tabPage1.Controls.Add(groupBoxCalcSettings);
            tabPage1.Controls.Add(btnCalculate);
            tabPage1.Controls.Add(groupBoxInput);
            tabPage1.Name = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBoxResults
            // 
            resources.ApplyResources(groupBoxResults, "groupBoxResults");
            groupBoxResults.Controls.Add(txtResults);
            groupBoxResults.Controls.Add(btnExportResults);
            groupBoxResults.Name = "groupBoxResults";
            groupBoxResults.TabStop = false;
            groupBoxResults.Enter += groupBoxResults_Enter;
            // 
            // txtResults
            // 
            resources.ApplyResources(txtResults, "txtResults");
            txtResults.Name = "txtResults";
            txtResults.ReadOnly = true;
            // 
            // btnExportResults
            // 
            resources.ApplyResources(btnExportResults, "btnExportResults");
            btnExportResults.Name = "btnExportResults";
            btnExportResults.UseVisualStyleBackColor = true;
            // 
            // groupBoxTables
            // 
            resources.ApplyResources(groupBoxTables, "groupBoxTables");
            groupBoxTables.Controls.Add(tabControlTables);
            groupBoxTables.Name = "groupBoxTables";
            groupBoxTables.TabStop = false;
            // 
            // tabControlTables
            // 
            resources.ApplyResources(tabControlTables, "tabControlTables");
            tabControlTables.Controls.Add(tabPageXAxis);
            tabControlTables.Controls.Add(tabPageYAxis);
            tabControlTables.Multiline = true;
            tabControlTables.Name = "tabControlTables";
            tabControlTables.SelectedIndex = 0;
            // 
            // tabPageXAxis
            // 
            resources.ApplyResources(tabPageXAxis, "tabPageXAxis");
            tabPageXAxis.Controls.Add(btnExportXAxis);
            tabPageXAxis.Controls.Add(dgvXAxis);
            tabPageXAxis.Name = "tabPageXAxis";
            tabPageXAxis.UseVisualStyleBackColor = true;
            // 
            // btnExportXAxis
            // 
            resources.ApplyResources(btnExportXAxis, "btnExportXAxis");
            btnExportXAxis.Name = "btnExportXAxis";
            btnExportXAxis.UseVisualStyleBackColor = true;
            // 
            // dgvXAxis
            // 
            resources.ApplyResources(dgvXAxis, "dgvXAxis");
            dgvXAxis.AllowUserToAddRows = false;
            dgvXAxis.AllowUserToDeleteRows = false;
            dgvXAxis.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvXAxis.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvXAxis.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvXAxis.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvXAxis.DefaultCellStyle = dataGridViewCellStyle2;
            dgvXAxis.Name = "dgvXAxis";
            dgvXAxis.ReadOnly = true;
            dgvXAxis.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dgvXAxis.RowTemplate.Height = 28;
            // 
            // tabPageYAxis
            // 
            resources.ApplyResources(tabPageYAxis, "tabPageYAxis");
            tabPageYAxis.Controls.Add(btnExportYAxis);
            tabPageYAxis.Controls.Add(dgvYAxis);
            tabPageYAxis.Name = "tabPageYAxis";
            tabPageYAxis.UseVisualStyleBackColor = true;
            // 
            // btnExportYAxis
            // 
            resources.ApplyResources(btnExportYAxis, "btnExportYAxis");
            btnExportYAxis.Name = "btnExportYAxis";
            btnExportYAxis.UseVisualStyleBackColor = true;
            // 
            // dgvYAxis
            // 
            resources.ApplyResources(dgvYAxis, "dgvYAxis");
            dgvYAxis.AllowUserToAddRows = false;
            dgvYAxis.AllowUserToDeleteRows = false;
            dgvYAxis.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvYAxis.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dgvYAxis.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgvYAxis.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            dgvYAxis.DefaultCellStyle = dataGridViewCellStyle4;
            dgvYAxis.Name = "dgvYAxis";
            dgvYAxis.ReadOnly = true;
            dgvYAxis.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dgvYAxis.RowTemplate.Height = 28;
            // 
            // groupBoxCalcSettings
            // 
            resources.ApplyResources(groupBoxCalcSettings, "groupBoxCalcSettings");
            groupBoxCalcSettings.Controls.Add(label13);
            groupBoxCalcSettings.Controls.Add(label14);
            groupBoxCalcSettings.Controls.Add(label12);
            groupBoxCalcSettings.Controls.Add(label11);
            groupBoxCalcSettings.Controls.Add(lblPointCount);
            groupBoxCalcSettings.Controls.Add(numPointCount);
            groupBoxCalcSettings.Controls.Add(numStartX);
            groupBoxCalcSettings.Controls.Add(numEndX);
            groupBoxCalcSettings.Controls.Add(numStartY);
            groupBoxCalcSettings.Controls.Add(numEndY);
            groupBoxCalcSettings.Name = "groupBoxCalcSettings";
            groupBoxCalcSettings.TabStop = false;
            // 
            // label13
            // 
            resources.ApplyResources(label13, "label13");
            label13.Name = "label13";
            // 
            // label14
            // 
            resources.ApplyResources(label14, "label14");
            label14.Name = "label14";
            // 
            // label12
            // 
            resources.ApplyResources(label12, "label12");
            label12.Name = "label12";
            // 
            // label11
            // 
            resources.ApplyResources(label11, "label11");
            label11.Name = "label11";
            // 
            // lblPointCount
            // 
            resources.ApplyResources(lblPointCount, "lblPointCount");
            lblPointCount.Name = "lblPointCount";
            // 
            // numPointCount
            // 
            resources.ApplyResources(numPointCount, "numPointCount");
            numPointCount.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numPointCount.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            numPointCount.Name = "numPointCount";
            numPointCount.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // numStartX
            // 
            resources.ApplyResources(numStartX, "numStartX");
            numStartX.DecimalPlaces = 2;
            numStartX.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numStartX.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            numStartX.Name = "numStartX";
            numStartX.Value = new decimal(new int[] { 25, 0, 0, int.MinValue });
            // 
            // numEndX
            // 
            resources.ApplyResources(numEndX, "numEndX");
            numEndX.DecimalPlaces = 2;
            numEndX.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numEndX.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            numEndX.Name = "numEndX";
            numEndX.Value = new decimal(new int[] { 25, 0, 0, 0 });
            // 
            // numStartY
            // 
            resources.ApplyResources(numStartY, "numStartY");
            numStartY.DecimalPlaces = 2;
            numStartY.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numStartY.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            numStartY.Name = "numStartY";
            numStartY.Value = new decimal(new int[] { 25, 0, 0, int.MinValue });
            //
            // numEndY
            // 
            resources.ApplyResources(numEndY, "numEndY");
            numEndY.DecimalPlaces = 2;
            numEndY.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numEndY.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            numEndY.Name = "numEndY";
            numEndY.Value = new decimal(new int[] { 25, 0, 0, 0 });
            // 
            // btnCalculate
            // 
            resources.ApplyResources(btnCalculate, "btnCalculate");
            btnCalculate.BackColor = Color.LightBlue;
            btnCalculate.Name = "btnCalculate";
            btnCalculate.UseVisualStyleBackColor = true;
            // 
            // groupBoxInput
            // 
            resources.ApplyResources(groupBoxInput, "groupBoxInput");
            groupBoxInput.Controls.Add(btnShowTemperaturePoints);
            groupBoxInput.Controls.Add(btnShowGraphs);
            groupBoxInput.Controls.Add(lblMaterial);
            groupBoxInput.Controls.Add(cboMaterial);
            groupBoxInput.Controls.Add(chkManualInput);
            groupBoxInput.Controls.Add(label10);
            groupBoxInput.Controls.Add(txtHeatTransferCoeff);
            groupBoxInput.Controls.Add(label9);
            groupBoxInput.Controls.Add(txtThermalExpansion);
            groupBoxInput.Controls.Add(label8);
            groupBoxInput.Controls.Add(txtThermalDiffusivity);
            groupBoxInput.Controls.Add(label7);
            groupBoxInput.Controls.Add(txtVolumetricHeatCapacity);
            groupBoxInput.Controls.Add(label6);
            groupBoxInput.Controls.Add(txtThermalConductivity);
            groupBoxInput.Controls.Add(label5);
            groupBoxInput.Controls.Add(txtElasticModulus);
            groupBoxInput.Controls.Add(label4);
            groupBoxInput.Controls.Add(txtPower);
            groupBoxInput.Controls.Add(label3);
            groupBoxInput.Controls.Add(txtSpeed);
            groupBoxInput.Controls.Add(label2);
            groupBoxInput.Controls.Add(txtWidthB2);
            groupBoxInput.Controls.Add(label1);
            groupBoxInput.Controls.Add(txtWidthB1);
            groupBoxInput.Controls.Add(lblThickness);
            groupBoxInput.Controls.Add(txtThickness);
            groupBoxInput.Name = "groupBoxInput";
            groupBoxInput.TabStop = false;
            // 
            // btnShowTemperaturePoints
            // 
            resources.ApplyResources(btnShowTemperaturePoints, "btnShowTemperaturePoints");
            btnShowTemperaturePoints.Name = "btnShowTemperaturePoints";
            btnShowTemperaturePoints.UseVisualStyleBackColor = true;
            btnShowTemperaturePoints.Click += btnShowTemperaturePoints_Click;
            // 
            // btnShowGraphs
            // 
            resources.ApplyResources(btnShowGraphs, "btnShowGraphs");
            btnShowGraphs.Name = "btnShowGraphs";
            btnShowGraphs.UseVisualStyleBackColor = true;
            btnShowGraphs.Click += btnShowGraphs_Click;
            // 
            // lblMaterial
            // 
            resources.ApplyResources(lblMaterial, "lblMaterial");
            lblMaterial.Name = "lblMaterial";
            // 
            // cboMaterial
            // 
            resources.ApplyResources(cboMaterial, "cboMaterial");
            cboMaterial.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMaterial.FormattingEnabled = true;
            cboMaterial.Name = "cboMaterial";
            // 
            // chkManualInput
            // 
            resources.ApplyResources(chkManualInput, "chkManualInput");
            chkManualInput.Name = "chkManualInput";
            chkManualInput.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            resources.ApplyResources(label10, "label10");
            label10.Name = "label10";
            // 
            // txtHeatTransferCoeff
            // 
            resources.ApplyResources(txtHeatTransferCoeff, "txtHeatTransferCoeff");
            txtHeatTransferCoeff.Name = "txtHeatTransferCoeff";
            // 
            // label9
            // 
            resources.ApplyResources(label9, "label9");
            label9.Name = "label9";
            // 
            // txtThermalExpansion
            // 
            resources.ApplyResources(txtThermalExpansion, "txtThermalExpansion");
            txtThermalExpansion.Name = "txtThermalExpansion";
            // 
            // label8
            // 
            resources.ApplyResources(label8, "label8");
            label8.Name = "label8";
            // 
            // txtThermalDiffusivity
            // 
            resources.ApplyResources(txtThermalDiffusivity, "txtThermalDiffusivity");
            txtThermalDiffusivity.Name = "txtThermalDiffusivity";
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.Name = "label7";
            // 
            // txtVolumetricHeatCapacity
            // 
            resources.ApplyResources(txtVolumetricHeatCapacity, "txtVolumetricHeatCapacity");
            txtVolumetricHeatCapacity.Name = "txtVolumetricHeatCapacity";
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            // 
            // txtThermalConductivity
            // 
            resources.ApplyResources(txtThermalConductivity, "txtThermalConductivity");
            txtThermalConductivity.Name = "txtThermalConductivity";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            // 
            // txtElasticModulus
            // 
            resources.ApplyResources(txtElasticModulus, "txtElasticModulus");
            txtElasticModulus.Name = "txtElasticModulus";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            // 
            // txtPower
            // 
            resources.ApplyResources(txtPower, "txtPower");
            txtPower.Name = "txtPower";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // txtSpeed
            // 
            resources.ApplyResources(txtSpeed, "txtSpeed");
            txtSpeed.Name = "txtSpeed";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // txtWidthB2
            // 
            resources.ApplyResources(txtWidthB2, "txtWidthB2");
            txtWidthB2.Name = "txtWidthB2";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // txtWidthB1
            // 
            resources.ApplyResources(txtWidthB1, "txtWidthB1");
            txtWidthB1.Name = "txtWidthB1";
            // 
            // lblThickness
            // 
            resources.ApplyResources(lblThickness, "lblThickness");
            lblThickness.Name = "lblThickness";
            // 
            // txtThickness
            // 
            resources.ApplyResources(txtThickness, "txtThickness");
            txtThickness.Name = "txtThickness";
            // 
            // saveFileDialog
            // 
            saveFileDialog.DefaultExt = "txt";
            resources.ApplyResources(saveFileDialog, "saveFileDialog");
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl1);
            Name = "MainForm";
            Load += MainForm_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            groupBoxResults.ResumeLayout(false);
            groupBoxResults.PerformLayout();
            groupBoxTables.ResumeLayout(false);
            tabControlTables.ResumeLayout(false);
            tabPageXAxis.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvXAxis).EndInit();
            tabPageYAxis.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvYAxis).EndInit();
            groupBoxCalcSettings.ResumeLayout(false);
            groupBoxCalcSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numPointCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStartX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numEndX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numStartY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numEndY).EndInit();
            groupBoxInput.ResumeLayout(false);
            groupBoxInput.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBoxInput;
        private System.Windows.Forms.CheckBox chkManualInput;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtHeatTransferCoeff;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtThermalExpansion;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtThermalDiffusivity;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtVolumetricHeatCapacity;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtThermalConductivity;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtElasticModulus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPower;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSpeed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWidthB2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtWidthB1;
        private System.Windows.Forms.Label lblThickness;
        private System.Windows.Forms.TextBox txtThickness;
        private System.Windows.Forms.GroupBox groupBoxResults;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.GroupBox groupBoxTables;
        private System.Windows.Forms.GroupBox groupBoxCalcSettings;
        private System.Windows.Forms.Label lblPointCount;
        private System.Windows.Forms.NumericUpDown numPointCount;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Button btnExportResults;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Button btnShowTemperaturePoints;
        private Label lblMaterial;
        private ComboBox cboMaterial;
        private System.Windows.Forms.NumericUpDown numStartX;
        private System.Windows.Forms.NumericUpDown numEndX;
        private System.Windows.Forms.NumericUpDown numStartY;
        private System.Windows.Forms.NumericUpDown numEndY;
        private Label label12;
        private Label label11;
        private Label label13;
        private Label label14;
        private TabControl tabControlTables;
        private TabPage tabPageXAxis;
        private DataGridView dgvXAxis;
        private TabPage tabPageYAxis;
        private DataGridView dgvYAxis;
        private Button btnExportYAxis;
        private Button btnExportXAxis;
        private Button btnShowGraphs;
    }
}