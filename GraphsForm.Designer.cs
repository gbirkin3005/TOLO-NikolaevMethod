namespace LaserWeldingCalculator
{
    partial class GraphsForm
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
            tabControl = new TabControl();
            tabPage1 = new TabPage();
            formsPlot1 = new ScottPlot.WinForms.FormsPlot();
            tabPage2 = new TabPage();
            formsPlot2 = new ScottPlot.WinForms.FormsPlot();
            tabPage3 = new TabPage();
            formsPlot3 = new ScottPlot.WinForms.FormsPlot();
            tabPage4 = new TabPage();
            formsPlot4 = new ScottPlot.WinForms.FormsPlot();
            tabPage5 = new TabPage();
            formsPlot5 = new ScottPlot.WinForms.FormsPlot();
            btnSave = new Button();
            btnClose = new Button();
            tabControl.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage4.SuspendLayout();
            tabPage5.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.Controls.Add(tabPage1);
            tabControl.Controls.Add(tabPage2);
            tabControl.Controls.Add(tabPage3);
            tabControl.Controls.Add(tabPage4);
            tabControl.Controls.Add(tabPage5);
            tabControl.Location = new Point(12, 12);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1109, 500);
            tabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(formsPlot1);
            tabPage1.Location = new Point(4, 27);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1101, 469);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Температура по Y";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // formsPlot1
            // 
            formsPlot1.DisplayScale = 1.25F;
            formsPlot1.Dock = DockStyle.Fill;
            formsPlot1.Location = new Point(3, 3);
            formsPlot1.Margin = new Padding(4, 5, 4, 5);
            formsPlot1.Name = "formsPlot1";
            formsPlot1.Size = new Size(1095, 463);
            formsPlot1.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(formsPlot2);
            tabPage2.Location = new Point(4, 27);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1101, 469);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Температура по X";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // formsPlot2
            // 
            formsPlot2.DisplayScale = 1.25F;
            formsPlot2.Dock = DockStyle.Fill;
            formsPlot2.Location = new Point(3, 3);
            formsPlot2.Margin = new Padding(4, 5, 4, 5);
            formsPlot2.Name = "formsPlot2";
            formsPlot2.Size = new Size(1095, 463);
            formsPlot2.TabIndex = 1;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(formsPlot3);
            tabPage3.Location = new Point(4, 27);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(1101, 469);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Изотермы";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // formsPlot3
            // 
            formsPlot3.DisplayScale = 1.25F;
            formsPlot3.Dock = DockStyle.Fill;
            formsPlot3.Location = new Point(0, 0);
            formsPlot3.Margin = new Padding(4, 5, 4, 5);
            formsPlot3.Name = "formsPlot3";
            formsPlot3.Size = new Size(1101, 469);
            formsPlot3.TabIndex = 1;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(formsPlot4);
            tabPage4.Location = new Point(4, 27);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(1101, 469);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Стадия нагрева";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // formsPlot4
            // 
            formsPlot4.DisplayScale = 1.25F;
            formsPlot4.Dock = DockStyle.Fill;
            formsPlot4.Location = new Point(0, 0);
            formsPlot4.Margin = new Padding(4, 5, 4, 5);
            formsPlot4.Name = "formsPlot4";
            formsPlot4.Size = new Size(1101, 469);
            formsPlot4.TabIndex = 1;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(formsPlot5);
            tabPage5.Location = new Point(4, 27);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(1101, 469);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "После охлаждения";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // formsPlot5
            // 
            formsPlot5.DisplayScale = 1.25F;
            formsPlot5.Dock = DockStyle.Fill;
            formsPlot5.Location = new Point(0, 0);
            formsPlot5.Margin = new Padding(4, 5, 4, 5);
            formsPlot5.Name = "formsPlot5";
            formsPlot5.Size = new Size(1101, 469);
            formsPlot5.TabIndex = 1;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.Location = new Point(922, 518);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(103, 30);
            btnSave.TabIndex = 1;
            btnSave.Text = "Сохранить";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Location = new Point(1031, 518);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(90, 30);
            btnClose.TabIndex = 2;
            btnClose.Text = "Закрыть";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // GraphsForm
            // 
            AutoScaleDimensions = new SizeF(9F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1133, 560);
            Controls.Add(btnClose);
            Controls.Add(btnSave);
            Controls.Add(tabControl);
            Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Margin = new Padding(4, 5, 4, 5);
            Name = "GraphsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Графики результатов расчета";
            tabControl.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            tabPage5.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private ScottPlot.WinForms.FormsPlot formsPlot1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private ScottPlot.WinForms.FormsPlot formsPlot2;
        private ScottPlot.WinForms.FormsPlot formsPlot3;
        private ScottPlot.WinForms.FormsPlot formsPlot4;
        private ScottPlot.WinForms.FormsPlot formsPlot5;
    }
}