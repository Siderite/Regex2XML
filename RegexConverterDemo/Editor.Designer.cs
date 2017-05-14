namespace RegexConverterDemo
{
    partial class Editor
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
            this.components = new System.ComponentModel.Container();
            this.tbRegex = new System.Windows.Forms.TextBox();
            this.tbXml = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cbCompiled = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cbCultureInvariant = new System.Windows.Forms.CheckBox();
            this.cbECMAScript = new System.Windows.Forms.CheckBox();
            this.cbExplicitCapture = new System.Windows.Forms.CheckBox();
            this.cbIgnoreCase = new System.Windows.Forms.CheckBox();
            this.cbIgnoreWhiteSpace = new System.Windows.Forms.CheckBox();
            this.cbMultiLine = new System.Windows.Forms.CheckBox();
            this.cbRightToLeft = new System.Windows.Forms.CheckBox();
            this.cbSingleLine = new System.Windows.Forms.CheckBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.cbWithSchema = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // tbRegex
            // 
            this.tbRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tbRegex.Location = new System.Drawing.Point(12, 12);
            this.tbRegex.MaxLength = 1000000;
            this.tbRegex.Multiline = true;
            this.tbRegex.Name = "tbRegex";
            this.tbRegex.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbRegex.Size = new System.Drawing.Size(366, 434);
            this.tbRegex.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tbRegex, "Write a .NET regular expression here");
            this.tbRegex.TextChanged += new System.EventHandler(this.tbRegex_TextChanged);
            // 
            // tbXml
            // 
            this.tbXml.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbXml.Location = new System.Drawing.Point(410, 12);
            this.tbXml.MaxLength = 1000000;
            this.tbXml.Multiline = true;
            this.tbXml.Name = "tbXml";
            this.tbXml.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbXml.Size = new System.Drawing.Size(356, 434);
            this.tbXml.TabIndex = 1;
            this.toolTip1.SetToolTip(this.tbXml, "Create or change XML regex");
            this.tbXml.TextChanged += new System.EventHandler(this.tbXml_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cbCompiled
            // 
            this.cbCompiled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbCompiled.AutoSize = true;
            this.cbCompiled.Location = new System.Drawing.Point(6, 462);
            this.cbCompiled.Name = "cbCompiled";
            this.cbCompiled.Size = new System.Drawing.Size(36, 17);
            this.cbCompiled.TabIndex = 2;
            this.cbCompiled.Text = "CI";
            this.toolTip1.SetToolTip(this.cbCompiled, "Compiled");
            this.cbCompiled.UseVisualStyleBackColor = true;
            this.cbCompiled.CheckedChanged += new System.EventHandler(this.cbCompiled_CheckedChanged);
            // 
            // cbCultureInvariant
            // 
            this.cbCultureInvariant.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbCultureInvariant.AutoSize = true;
            this.cbCultureInvariant.Location = new System.Drawing.Point(48, 462);
            this.cbCultureInvariant.Name = "cbCultureInvariant";
            this.cbCultureInvariant.Size = new System.Drawing.Size(39, 17);
            this.cbCultureInvariant.TabIndex = 3;
            this.cbCultureInvariant.Text = "CL";
            this.toolTip1.SetToolTip(this.cbCultureInvariant, "Culture Invariant");
            this.cbCultureInvariant.UseVisualStyleBackColor = true;
            this.cbCultureInvariant.CheckedChanged += new System.EventHandler(this.cbCultureInvariant_CheckedChanged);
            // 
            // cbECMAScript
            // 
            this.cbECMAScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbECMAScript.AutoSize = true;
            this.cbECMAScript.Location = new System.Drawing.Point(93, 462);
            this.cbECMAScript.Name = "cbECMAScript";
            this.cbECMAScript.Size = new System.Drawing.Size(40, 17);
            this.cbECMAScript.TabIndex = 4;
            this.cbECMAScript.Text = "ES";
            this.toolTip1.SetToolTip(this.cbECMAScript, "ECMA Script");
            this.cbECMAScript.UseVisualStyleBackColor = true;
            this.cbECMAScript.CheckedChanged += new System.EventHandler(this.cbECMAScript_CheckedChanged);
            // 
            // cbExplicitCapture
            // 
            this.cbExplicitCapture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbExplicitCapture.AutoSize = true;
            this.cbExplicitCapture.Location = new System.Drawing.Point(139, 462);
            this.cbExplicitCapture.Name = "cbExplicitCapture";
            this.cbExplicitCapture.Size = new System.Drawing.Size(40, 17);
            this.cbExplicitCapture.TabIndex = 5;
            this.cbExplicitCapture.Text = "EC";
            this.toolTip1.SetToolTip(this.cbExplicitCapture, "Explicit Capture");
            this.cbExplicitCapture.UseVisualStyleBackColor = true;
            this.cbExplicitCapture.CheckedChanged += new System.EventHandler(this.cbExplicitCapture_CheckedChanged);
            // 
            // cbIgnoreCase
            // 
            this.cbIgnoreCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbIgnoreCase.AutoSize = true;
            this.cbIgnoreCase.Location = new System.Drawing.Point(185, 462);
            this.cbIgnoreCase.Name = "cbIgnoreCase";
            this.cbIgnoreCase.Size = new System.Drawing.Size(36, 17);
            this.cbIgnoreCase.TabIndex = 6;
            this.cbIgnoreCase.Text = "IC";
            this.toolTip1.SetToolTip(this.cbIgnoreCase, "Ignore Case");
            this.cbIgnoreCase.UseVisualStyleBackColor = true;
            this.cbIgnoreCase.CheckedChanged += new System.EventHandler(this.cbIgnoreCase_CheckedChanged);
            // 
            // cbIgnoreWhiteSpace
            // 
            this.cbIgnoreWhiteSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbIgnoreWhiteSpace.AutoSize = true;
            this.cbIgnoreWhiteSpace.Location = new System.Drawing.Point(227, 462);
            this.cbIgnoreWhiteSpace.Name = "cbIgnoreWhiteSpace";
            this.cbIgnoreWhiteSpace.Size = new System.Drawing.Size(40, 17);
            this.cbIgnoreWhiteSpace.TabIndex = 7;
            this.cbIgnoreWhiteSpace.Text = "IW";
            this.toolTip1.SetToolTip(this.cbIgnoreWhiteSpace, "Ignore WhiteSpace");
            this.cbIgnoreWhiteSpace.UseVisualStyleBackColor = true;
            this.cbIgnoreWhiteSpace.CheckedChanged += new System.EventHandler(this.cbIgnoreWhiteSpace_CheckedChanged);
            // 
            // cbMultiLine
            // 
            this.cbMultiLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbMultiLine.AutoSize = true;
            this.cbMultiLine.Location = new System.Drawing.Point(273, 462);
            this.cbMultiLine.Name = "cbMultiLine";
            this.cbMultiLine.Size = new System.Drawing.Size(35, 17);
            this.cbMultiLine.TabIndex = 8;
            this.cbMultiLine.Text = "M";
            this.toolTip1.SetToolTip(this.cbMultiLine, "MultiLine");
            this.cbMultiLine.UseVisualStyleBackColor = true;
            this.cbMultiLine.CheckedChanged += new System.EventHandler(this.cbMultiLine_CheckedChanged);
            // 
            // cbRightToLeft
            // 
            this.cbRightToLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbRightToLeft.AutoSize = true;
            this.cbRightToLeft.Location = new System.Drawing.Point(314, 462);
            this.cbRightToLeft.Name = "cbRightToLeft";
            this.cbRightToLeft.Size = new System.Drawing.Size(40, 17);
            this.cbRightToLeft.TabIndex = 11;
            this.cbRightToLeft.Text = "RL";
            this.toolTip1.SetToolTip(this.cbRightToLeft, "Right to Left");
            this.cbRightToLeft.UseVisualStyleBackColor = true;
            this.cbRightToLeft.CheckedChanged += new System.EventHandler(this.cbRightToLeft_CheckedChanged);
            // 
            // cbSingleLine
            // 
            this.cbSingleLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbSingleLine.AutoSize = true;
            this.cbSingleLine.Location = new System.Drawing.Point(360, 462);
            this.cbSingleLine.Name = "cbSingleLine";
            this.cbSingleLine.Size = new System.Drawing.Size(33, 17);
            this.cbSingleLine.TabIndex = 12;
            this.cbSingleLine.Text = "S";
            this.toolTip1.SetToolTip(this.cbSingleLine, "Single Line");
            this.cbSingleLine.UseVisualStyleBackColor = true;
            this.cbSingleLine.CheckedChanged += new System.EventHandler(this.cbSingleLine_CheckedChanged);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // cbWithSchema
            // 
            this.cbWithSchema.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbWithSchema.AutoSize = true;
            this.cbWithSchema.Location = new System.Drawing.Point(678, 463);
            this.cbWithSchema.Name = "cbWithSchema";
            this.cbWithSchema.Size = new System.Drawing.Size(88, 17);
            this.cbWithSchema.TabIndex = 13;
            this.cbWithSchema.Text = "With schema";
            this.toolTip1.SetToolTip(this.cbWithSchema, "With schema");
            this.cbWithSchema.UseVisualStyleBackColor = true;
            this.cbWithSchema.CheckedChanged += new System.EventHandler(this.cbWithSchema_CheckedChanged);
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 492);
            this.Controls.Add(this.cbWithSchema);
            this.Controls.Add(this.cbSingleLine);
            this.Controls.Add(this.cbRightToLeft);
            this.Controls.Add(this.cbMultiLine);
            this.Controls.Add(this.cbIgnoreWhiteSpace);
            this.Controls.Add(this.cbIgnoreCase);
            this.Controls.Add(this.cbExplicitCapture);
            this.Controls.Add(this.cbECMAScript);
            this.Controls.Add(this.cbCultureInvariant);
            this.Controls.Add(this.cbCompiled);
            this.Controls.Add(this.tbXml);
            this.Controls.Add(this.tbRegex);
            this.Name = "Editor";
            this.Text = "Editor";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbRegex;
        private System.Windows.Forms.TextBox tbXml;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox cbCompiled;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox cbCultureInvariant;
        private System.Windows.Forms.CheckBox cbECMAScript;
        private System.Windows.Forms.CheckBox cbExplicitCapture;
        private System.Windows.Forms.CheckBox cbIgnoreCase;
        private System.Windows.Forms.CheckBox cbIgnoreWhiteSpace;
        private System.Windows.Forms.CheckBox cbMultiLine;
        private System.Windows.Forms.CheckBox cbRightToLeft;
        private System.Windows.Forms.CheckBox cbSingleLine;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.CheckBox cbWithSchema;
    }
}