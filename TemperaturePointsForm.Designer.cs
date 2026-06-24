namespace LaserWeldingCalculator
{
    partial class TemperaturePointsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage500;
        private System.Windows.Forms.TabPage tabPage600;
        private System.Windows.Forms.DataGridView dgv500;
        private System.Windows.Forms.DataGridView dgv600;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.NumericUpDown num500Min;
        private System.Windows.Forms.NumericUpDown num500Max;
        private System.Windows.Forms.NumericUpDown num600Min;
        private System.Windows.Forms.NumericUpDown num600Max;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numGridStep;
        private System.Windows.Forms.NumericUpDown numGridSize;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.GroupBox groupBoxRanges;
        private System.Windows.Forms.Label lblXRange;
        private System.Windows.Forms.Label lblYRange;
        private System.Windows.Forms.Button btnExport500;
        private System.Windows.Forms.Button btnExport600;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tabControl = new TabControl();
            tabPage500 = new TabPage();
            dgv500 = new DataGridView();
            btnExport500 = new Button();
            tabPage600 = new TabPage();
            dgv600 = new DataGridView();
            btnExport600 = new Button();
            groupBoxSettings = new GroupBox();
            numGridSize = new NumericUpDown();
            label6 = new Label();
            numGridStep = new NumericUpDown();
            label5 = new Label();
            num600Max = new NumericUpDown();
            label600 = new Label();
            num600Min = new NumericUpDown();
            label3 = new Label();
            num500Max = new NumericUpDown();
            label2 = new Label();
            num500Min = new NumericUpDown();
            label500 = new Label();
            btnCalculate = new Button();
            groupBoxRanges = new GroupBox();
            btnClose = new Button();
            lblStatus = new Label();
            lblYRange = new Label();
            lblXRange = new Label();
            tabControl.SuspendLayout();
            tabPage500.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv500).BeginInit();
            tabPage600.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv600).BeginInit();
            groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numGridSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numGridStep).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num600Max).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num600Min).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num500Max).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num500Min).BeginInit();
            groupBoxRanges.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.Controls.Add(tabPage500);
            tabControl.Controls.Add(tabPage600);
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1200, 209);
            tabControl.TabIndex = 0;
            // 
            // tabPage500
            // 
            tabPage500.Controls.Add(dgv500);
            tabPage500.Controls.Add(btnExport500);
            tabPage500.Location = new Point(4, 29);
            tabPage500.Name = "tabPage500";
            tabPage500.Padding = new Padding(3);
            tabPage500.Size = new Size(1192, 176);
            tabPage500.TabIndex = 0;
            tabPage500.Text = "Точки ~500°C";
            tabPage500.UseVisualStyleBackColor = true;
            // 
            // dgv500
            // 
            dgv500.AllowUserToAddRows = false;
            dgv500.AllowUserToDeleteRows = false;
            dgv500.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv500.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dgv500.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv500.Dock = DockStyle.Fill;
            dgv500.Location = new Point(3, 3);
            dgv500.Name = "dgv500";
            dgv500.ReadOnly = true;
            dgv500.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dgv500.RowTemplate.Height = 28;
            dgv500.Size = new Size(1186, 141);
            dgv500.TabIndex = 0;
            // 
            // btnExport500
            // 
            btnExport500.Dock = DockStyle.Bottom;
            btnExport500.Location = new Point(3, 144);
            btnExport500.Name = "btnExport500";
            btnExport500.Size = new Size(1186, 29);
            btnExport500.TabIndex = 1;
            btnExport500.Text = "Экспорт в Excel";
            btnExport500.UseVisualStyleBackColor = true;
            btnExport500.Click += btnExport500_Click;
            // 
            // tabPage600
            // 
            tabPage600.Controls.Add(dgv600);
            tabPage600.Controls.Add(btnExport600);
            tabPage600.Location = new Point(4, 29);
            tabPage600.Name = "tabPage600";
            tabPage600.Padding = new Padding(3);
            tabPage600.Size = new Size(1192, 176);
            tabPage600.TabIndex = 1;
            tabPage600.Text = "Точки ~600°C";
            tabPage600.UseVisualStyleBackColor = true;
            // 
            // dgv600
            // 
            dgv600.AllowUserToAddRows = false;
            dgv600.AllowUserToDeleteRows = false;
            dgv600.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv600.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dgv600.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv600.Dock = DockStyle.Fill;
            dgv600.Location = new Point(3, 3);
            dgv600.Name = "dgv600";
            dgv600.ReadOnly = true;
            dgv600.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dgv600.RowTemplate.Height = 28;
            dgv600.Size = new Size(1186, 141);
            dgv600.TabIndex = 0;
            // 
            // btnExport600
            // 
            btnExport600.Dock = DockStyle.Bottom;
            btnExport600.Location = new Point(3, 144);
            btnExport600.Name = "btnExport600";
            btnExport600.Size = new Size(1186, 29);
            btnExport600.TabIndex = 1;
            btnExport600.Text = "Экспорт в Excel";
            btnExport600.UseVisualStyleBackColor = true;
            btnExport600.Click += btnExport600_Click;
            // 
            // groupBoxSettings
            // 
            groupBoxSettings.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            groupBoxSettings.Controls.Add(numGridSize);
            groupBoxSettings.Controls.Add(label6);
            groupBoxSettings.Controls.Add(numGridStep);
            groupBoxSettings.Controls.Add(label5);
            groupBoxSettings.Controls.Add(num600Max);
            groupBoxSettings.Controls.Add(label600);
            groupBoxSettings.Controls.Add(num600Min);
            groupBoxSettings.Controls.Add(label3);
            groupBoxSettings.Controls.Add(num500Max);
            groupBoxSettings.Controls.Add(label2);
            groupBoxSettings.Controls.Add(num500Min);
            groupBoxSettings.Controls.Add(label500);
            groupBoxSettings.Location = new Point(7, 215);
            groupBoxSettings.Name = "groupBoxSettings";
            groupBoxSettings.Size = new Size(356, 213);
            groupBoxSettings.TabIndex = 1;
            groupBoxSettings.TabStop = false;
            groupBoxSettings.Text = "Настройки расчета";
            // 
            // numGridSize
            // 
            numGridSize.DecimalPlaces = 2;
            numGridSize.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numGridSize.Location = new Point(190, 158);
            numGridSize.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numGridSize.Minimum = new decimal(new int[] { 1, 0, 0, 196608 });
            numGridSize.Name = "numGridSize";
            numGridSize.Size = new Size(71, 27);
            numGridSize.TabIndex = 12;
            numGridSize.Value = new decimal(new int[] { 10, 0, 0, 65536 });
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(18, 160);
            label6.Name = "label6";
            label6.Size = new Size(129, 20);
            label6.TabIndex = 11;
            label6.Text = "Размер сетки, см:";
            // 
            // numGridStep
            // 
            numGridStep.DecimalPlaces = 3;
            numGridStep.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
            numGridStep.Location = new Point(190, 128);
            numGridStep.Minimum = new decimal(new int[] { 1, 0, 0, 327680 });
            numGridStep.Name = "numGridStep";
            numGridStep.Size = new Size(71, 27);
            numGridStep.TabIndex = 10;
            numGridStep.Value = new decimal(new int[] { 10, 0, 0, 196608 });
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(18, 130);
            label5.Name = "label5";
            label5.Size = new Size(106, 20);
            label5.TabIndex = 9;
            label5.Text = "Шаг сетки, см:";
            // 
            // num600Max
            // 
            num600Max.Location = new Point(267, 75);
            num600Max.Maximum = new decimal(new int[] { 620, 0, 0, 0 });
            num600Max.Minimum = new decimal(new int[] { 600, 0, 0, 0 });
            num600Max.Name = "num600Max";
            num600Max.Size = new Size(71, 27);
            num600Max.TabIndex = 8;
            num600Max.Value = new decimal(new int[] { 602, 0, 0, 0 });
            // 
            // label600
            // 
            label600.AutoSize = true;
            label600.Location = new Point(18, 77);
            label600.Name = "label600";
            label600.Size = new Size(121, 20);
            label600.TabIndex = 7;
            label600.Text = "Диапазон 600°C";
            // 
            // num600Min
            // 
            num600Min.Location = new Point(190, 75);
            num600Min.Maximum = new decimal(new int[] { 600, 0, 0, 0 });
            num600Min.Minimum = new decimal(new int[] { 580, 0, 0, 0 });
            num600Min.Name = "num600Min";
            num600Min.Size = new Size(71, 27);
            num600Min.TabIndex = 6;
            num600Min.Value = new decimal(new int[] { 598, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(190, 19);
            label3.Name = "label3";
            label3.Size = new Size(38, 20);
            label3.TabIndex = 5;
            label3.Text = "мин";
            // 
            // num500Max
            // 
            num500Max.Location = new Point(267, 42);
            num500Max.Maximum = new decimal(new int[] { 520, 0, 0, 0 });
            num500Max.Minimum = new decimal(new int[] { 500, 0, 0, 0 });
            num500Max.Name = "num500Max";
            num500Max.Size = new Size(71, 27);
            num500Max.TabIndex = 4;
            num500Max.Value = new decimal(new int[] { 502, 0, 0, 0 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(267, 19);
            label2.Name = "label2";
            label2.Size = new Size(42, 20);
            label2.TabIndex = 3;
            label2.Text = "макс";
            // 
            // num500Min
            // 
            num500Min.Location = new Point(190, 42);
            num500Min.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            num500Min.Minimum = new decimal(new int[] { 480, 0, 0, 0 });
            num500Min.Name = "num500Min";
            num500Min.Size = new Size(71, 27);
            num500Min.TabIndex = 2;
            num500Min.Value = new decimal(new int[] { 498, 0, 0, 0 });
            // 
            // label500
            // 
            label500.AutoSize = true;
            label500.Location = new Point(18, 44);
            label500.Name = "label500";
            label500.Size = new Size(121, 20);
            label500.TabIndex = 1;
            label500.Text = "Диапазон 500°C";
            // 
            // btnCalculate
            // 
            btnCalculate.Anchor = AnchorStyles.Bottom;
            btnCalculate.Location = new Point(6, 169);
            btnCalculate.Name = "btnCalculate";
            btnCalculate.Size = new Size(122, 31);
            btnCalculate.TabIndex = 0;
            btnCalculate.Text = "Пересчитать";
            btnCalculate.UseVisualStyleBackColor = true;
            btnCalculate.Click += btnCalculate_Click;
            // 
            // groupBoxRanges
            // 
            groupBoxRanges.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxRanges.Controls.Add(btnClose);
            groupBoxRanges.Controls.Add(lblStatus);
            groupBoxRanges.Controls.Add(lblYRange);
            groupBoxRanges.Controls.Add(lblXRange);
            groupBoxRanges.Controls.Add(btnCalculate);
            groupBoxRanges.Location = new Point(369, 222);
            groupBoxRanges.Name = "groupBoxRanges";
            groupBoxRanges.Size = new Size(824, 206);
            groupBoxRanges.TabIndex = 2;
            groupBoxRanges.TabStop = false;
            groupBoxRanges.Text = "Область температур 500–600°C";
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom;
            btnClose.BackColor = Color.LightSalmon;
            btnClose.FlatAppearance.BorderColor = SystemColors.Control;
            btnClose.FlatAppearance.BorderSize = 2;
            btnClose.FlatStyle = FlatStyle.Popup;
            btnClose.ForeColor = SystemColors.ControlText;
            btnClose.Location = new Point(134, 169);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(122, 31);
            btnClose.TabIndex = 3;
            btnClose.Text = "Закрыть";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(6, 87);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(116, 20);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Найдено точек:";
            // 
            // lblYRange
            // 
            lblYRange.AutoSize = true;
            lblYRange.Location = new Point(6, 53);
            lblYRange.Name = "lblYRange";
            lblYRange.Size = new Size(147, 20);
            lblYRange.TabIndex = 1;
            lblYRange.Text = "При x=0: y от ... до ...";
            // 
            // lblXRange
            // 
            lblXRange.AutoSize = true;
            lblXRange.Location = new Point(6, 23);
            lblXRange.Name = "lblXRange";
            lblXRange.Size = new Size(147, 20);
            lblXRange.TabIndex = 0;
            lblXRange.Text = "При y=0: x от ... до ...";
            // 
            // TemperaturePointsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 440);
            Controls.Add(groupBoxRanges);
            Controls.Add(groupBoxSettings);
            Controls.Add(tabControl);
            Name = "TemperaturePointsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Точки материала, нагретые до температур 500°C и 600°C";
            Load += TemperaturePointsForm_Load;
            tabControl.ResumeLayout(false);
            tabPage500.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgv500).EndInit();
            tabPage600.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgv600).EndInit();
            groupBoxSettings.ResumeLayout(false);
            groupBoxSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numGridSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)numGridStep).EndInit();
            ((System.ComponentModel.ISupportInitialize)num600Max).EndInit();
            ((System.ComponentModel.ISupportInitialize)num600Min).EndInit();
            ((System.ComponentModel.ISupportInitialize)num500Max).EndInit();
            ((System.ComponentModel.ISupportInitialize)num500Min).EndInit();
            groupBoxRanges.ResumeLayout(false);
            groupBoxRanges.PerformLayout();
            ResumeLayout(false);
        }
        private Label label600;
        private Label label500;
    }
}