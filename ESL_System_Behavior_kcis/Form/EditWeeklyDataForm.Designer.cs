﻿namespace ESL_System_Behavior.Form
{
    partial class EditWeeklyDataForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnCancel = new DevComponents.DotNetBar.ButtonX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.txtGeneralComment = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.dgWeeklyReport = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.colCourseName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTeacherName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBBeginDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBEndDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgBehavior = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.colBCreateDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDetention = new DevComponents.DotNetBar.Controls.DataGridViewCheckBoxXColumn();
            this.colGood = new DevComponents.DotNetBar.Controls.DataGridViewCheckBoxXColumn();
            this.txtTeacherComment = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.dgStudentData = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.colClassName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSeatNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            ((System.ComponentModel.ISupportInitialize)(this.dgWeeklyReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgBehavior)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgStudentData)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AutoSize = true;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(911, 759);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.AutoSize = true;
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCancel.Location = new System.Drawing.Point(1012, 759);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(12, 129);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(224, 25);
            this.labelX1.TabIndex = 6;
            this.labelX1.Text = "General Comment";
            // 
            // txtGeneralComment
            // 
            this.txtGeneralComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.txtGeneralComment.Border.Class = "TextBoxBorder";
            this.txtGeneralComment.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtGeneralComment.Location = new System.Drawing.Point(12, 159);
            this.txtGeneralComment.Multiline = true;
            this.txtGeneralComment.Name = "txtGeneralComment";
            this.txtGeneralComment.Size = new System.Drawing.Size(1076, 82);
            this.txtGeneralComment.TabIndex = 1;
            // 
            // dgWeeklyReport
            // 
            this.dgWeeklyReport.AllowUserToAddRows = false;
            this.dgWeeklyReport.AllowUserToDeleteRows = false;
            this.dgWeeklyReport.AllowUserToResizeRows = false;
            this.dgWeeklyReport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgWeeklyReport.BackgroundColor = System.Drawing.Color.White;
            this.dgWeeklyReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgWeeklyReport.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCourseName,
            this.colTeacherName,
            this.colBBeginDate,
            this.colBEndDate});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgWeeklyReport.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgWeeklyReport.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgWeeklyReport.Location = new System.Drawing.Point(12, 22);
            this.dgWeeklyReport.MultiSelect = false;
            this.dgWeeklyReport.Name = "dgWeeklyReport";
            this.dgWeeklyReport.RowTemplate.Height = 24;
            this.dgWeeklyReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgWeeklyReport.Size = new System.Drawing.Size(1076, 100);
            this.dgWeeklyReport.TabIndex = 0;
            this.dgWeeklyReport.SelectionChanged += new System.EventHandler(this.dgWeeklyReport_SelectionChanged);
            // 
            // colCourseName
            // 
            this.colCourseName.HeaderText = "課程名稱";
            this.colCourseName.Name = "colCourseName";
            this.colCourseName.ReadOnly = true;
            // 
            // colTeacherName
            // 
            this.colTeacherName.HeaderText = "教師名稱";
            this.colTeacherName.Name = "colTeacherName";
            this.colTeacherName.ReadOnly = true;
            // 
            // colBBeginDate
            // 
            this.colBBeginDate.HeaderText = "開始日期";
            this.colBBeginDate.Name = "colBBeginDate";
            this.colBBeginDate.ReadOnly = true;
            // 
            // colBEndDate
            // 
            this.colBEndDate.HeaderText = "結束日期";
            this.colBEndDate.Name = "colBEndDate";
            this.colBEndDate.ReadOnly = true;
            // 
            // dgBehavior
            // 
            this.dgBehavior.AllowUserToAddRows = false;
            this.dgBehavior.AllowUserToDeleteRows = false;
            this.dgBehavior.AllowUserToResizeRows = false;
            this.dgBehavior.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgBehavior.BackgroundColor = System.Drawing.Color.White;
            this.dgBehavior.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgBehavior.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBCreateDate,
            this.colBComment,
            this.colDetention,
            this.colGood});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgBehavior.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgBehavior.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgBehavior.Location = new System.Drawing.Point(405, 438);
            this.dgBehavior.Name = "dgBehavior";
            this.dgBehavior.RowTemplate.Height = 24;
            this.dgBehavior.Size = new System.Drawing.Size(683, 296);
            this.dgBehavior.TabIndex = 4;
            // 
            // colBCreateDate
            // 
            this.colBCreateDate.HeaderText = "日期";
            this.colBCreateDate.Name = "colBCreateDate";
            this.colBCreateDate.ReadOnly = true;
            // 
            // colBComment
            // 
            this.colBComment.HeaderText = "事由";
            this.colBComment.Name = "colBComment";
            // 
            // colDetention
            // 
            this.colDetention.Checked = true;
            this.colDetention.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.colDetention.CheckValue = "N";
            this.colDetention.HeaderText = "Detention";
            this.colDetention.Name = "colDetention";
            // 
            // colGood
            // 
            this.colGood.Checked = true;
            this.colGood.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.colGood.CheckValue = "N";
            this.colGood.HeaderText = "Good";
            this.colGood.Name = "colGood";
            // 
            // txtTeacherComment
            // 
            this.txtTeacherComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.txtTeacherComment.Border.Class = "TextBoxBorder";
            this.txtTeacherComment.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtTeacherComment.Location = new System.Drawing.Point(405, 298);
            this.txtTeacherComment.Multiline = true;
            this.txtTeacherComment.Name = "txtTeacherComment";
            this.txtTeacherComment.Size = new System.Drawing.Size(683, 91);
            this.txtTeacherComment.TabIndex = 3;
            // 
            // dgStudentData
            // 
            this.dgStudentData.AllowUserToAddRows = false;
            this.dgStudentData.AllowUserToDeleteRows = false;
            this.dgStudentData.AllowUserToResizeRows = false;
            this.dgStudentData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgStudentData.BackgroundColor = System.Drawing.Color.White;
            this.dgStudentData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgStudentData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colClassName,
            this.colSeatNo,
            this.colName});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgStudentData.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgStudentData.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgStudentData.Location = new System.Drawing.Point(12, 266);
            this.dgStudentData.Name = "dgStudentData";
            this.dgStudentData.RowTemplate.Height = 24;
            this.dgStudentData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgStudentData.Size = new System.Drawing.Size(376, 468);
            this.dgStudentData.TabIndex = 2;
            this.dgStudentData.SelectionChanged += new System.EventHandler(this.dgStudentData_SelectionChanged);
            // 
            // colClassName
            // 
            this.colClassName.HeaderText = "班級";
            this.colClassName.Name = "colClassName";
            this.colClassName.ReadOnly = true;
            // 
            // colSeatNo
            // 
            this.colSeatNo.HeaderText = "座號";
            this.colSeatNo.Name = "colSeatNo";
            this.colSeatNo.ReadOnly = true;
            this.colSeatNo.Width = 70;
            // 
            // colName
            // 
            this.colName.HeaderText = "姓名";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // labelX2
            // 
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(405, 406);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(224, 25);
            this.labelX2.TabIndex = 10;
            this.labelX2.Text = "Behavor";
            // 
            // labelX3
            // 
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(405, 266);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(224, 25);
            this.labelX3.TabIndex = 11;
            this.labelX3.Text = "Teacher Comment";
            // 
            // EditWeeklyDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1115, 806);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.dgBehavior);
            this.Controls.Add(this.txtTeacherComment);
            this.Controls.Add(this.dgStudentData);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.txtGeneralComment);
            this.Controls.Add(this.dgWeeklyReport);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(981, 845);
            this.Name = "EditWeeklyDataForm";
            this.Text = "修改 Weekly Report ";
            this.Load += new System.EventHandler(this.EditWeeklyDataForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgWeeklyReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgBehavior)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgStudentData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnCancel;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtGeneralComment;
        private DevComponents.DotNetBar.Controls.DataGridViewX dgWeeklyReport;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCourseName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTeacherName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBBeginDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBEndDate;
        private DevComponents.DotNetBar.Controls.DataGridViewX dgBehavior;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBCreateDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBComment;
        private DevComponents.DotNetBar.Controls.DataGridViewCheckBoxXColumn colDetention;
        private DevComponents.DotNetBar.Controls.DataGridViewCheckBoxXColumn colGood;
        private DevComponents.DotNetBar.Controls.TextBoxX txtTeacherComment;
        private DevComponents.DotNetBar.Controls.DataGridViewX dgStudentData;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClassName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSeatNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX3;
    }
}