using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Siderite.Code;

namespace RegexConverterDemo
{
    public partial class Editor : Form
    {
        private bool _regexChanged;
        private bool _safeEdit;

        public Editor()
        {
            InitializeComponent();
        }

        private void tbRegex_TextChanged(object sender, EventArgs e)
        {
            if (!_safeEdit)
                RegexChanged();
        }

        private void RegexChanged()
        {
            timer1.Stop();
            timer1.Start();
            _regexChanged = true;
        }

        private void tbXml_TextChanged(object sender, EventArgs e)
        {
            if (!_safeEdit)
                XmlChanged();
        }

        private void XmlChanged()
        {
            timer1.Stop();
            timer1.Start();
            _regexChanged = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_regexChanged)
            {
                try
                {
                    RegexOptions ro = RegexOptions.None;
                    if (cbCompiled.Checked) ro |= RegexOptions.Compiled;
                    if (cbCultureInvariant.Checked) ro |= RegexOptions.CultureInvariant;
                    if (cbECMAScript.Checked) ro |= RegexOptions.ECMAScript;
                    if (cbExplicitCapture.Checked) ro |= RegexOptions.ExplicitCapture;
                    if (cbIgnoreCase.Checked) ro |= RegexOptions.IgnoreCase;
                    if (cbIgnoreWhiteSpace.Checked) ro |= RegexOptions.IgnorePatternWhitespace;
                    if (cbMultiLine.Checked) ro |= RegexOptions.Multiline;
                    if (cbRightToLeft.Checked) ro |= RegexOptions.RightToLeft;
                    if (cbSingleLine.Checked) ro |= RegexOptions.Singleline;
                    var reg = new Regex(tbRegex.Text, ro);
                    SetXml(GetXml(reg));
                    Regex reg2 = GetReg(tbXml.Text);
                    if (reg.ToString() != reg2.ToString())
                    {
                        errorProvider1.SetError(tbRegex,
                                                "Original regex and regenerated regex are not the same. Please contact the author.");
                    }
                    else
                    {
                        errorProvider1.Clear();
                    }
                }
                catch (Exception ex)
                {
                    errorProvider1.SetError(tbRegex, ex.Message);
                    SetXml("");
                }
            }
            else
            {
                try
                {
                    Regex reg = GetReg(tbXml.Text);
                    cbCompiled.Checked = ((reg.Options & RegexOptions.Compiled) == RegexOptions.Compiled);
                    cbCultureInvariant.Checked = ((reg.Options & RegexOptions.CultureInvariant) ==
                                                  RegexOptions.CultureInvariant);
                    cbECMAScript.Checked = ((reg.Options & RegexOptions.ECMAScript) == RegexOptions.ECMAScript);
                    cbExplicitCapture.Checked = ((reg.Options & RegexOptions.ExplicitCapture) ==
                                                 RegexOptions.ExplicitCapture);
                    cbIgnoreCase.Checked = ((reg.Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase);
                    cbIgnoreWhiteSpace.Checked = ((reg.Options & RegexOptions.IgnorePatternWhitespace) ==
                                                  RegexOptions.IgnorePatternWhitespace);
                    cbMultiLine.Checked = ((reg.Options & RegexOptions.Multiline) == RegexOptions.Multiline);
                    cbRightToLeft.Checked = ((reg.Options & RegexOptions.RightToLeft) == RegexOptions.RightToLeft);
                    cbSingleLine.Checked = ((reg.Options & RegexOptions.Singleline) == RegexOptions.Singleline);
                    SetRegex(reg.ToString());
                    string xml2 = GetXml(reg);
                    if (xml2 != tbXml.Text)
                    {
                        errorProvider1.SetError(tbRegex,
                                                "Original xml and regenerated xml are not the same. Please contact the author.");
                    }
                    else
                    {
                        errorProvider1.Clear();
                    }
                }
                catch (Exception ex)
                {
                    errorProvider1.SetError(tbXml, ex.Message);
                    SetRegex("");
                }
            }
        }


        private void SetXml(string xml)
        {
            _safeEdit = true;
            timer1.Stop();
            tbXml.Text = xml;
            _safeEdit = false;
        }

        private void SetRegex(string pattern)
        {
            _safeEdit = true;
            timer1.Stop();
            tbRegex.Text = pattern;
            _safeEdit = false;
        }

        private string GetXml(Regex reg)
        {
            var rc = new RegexConverter(reg);
            XmlMode xmlMode = cbWithSchema.Checked
                                  ? XmlMode.XmlWithEmbeddedSchema
                                  : XmlMode.XmlOnly;
            return rc.GetXmlString(xmlMode);
        }

        private static Regex GetReg(string xml)
        {
            Regex reg = RegexConverter.GetRegex(xml);
            return reg;
        }

        private void cbCompiled_CheckedChanged(object sender, EventArgs e)
        {
            RegexChanged();
        }

        private void cbCultureInvariant_CheckedChanged(object sender, EventArgs e)
        {
            RegexChanged();
        }

        private void cbECMAScript_CheckedChanged(object sender, EventArgs e)
        {
            RegexChanged();
        }

        private void cbExplicitCapture_CheckedChanged(object sender, EventArgs e)
        {
            RegexChanged();
        }

        private void cbIgnoreCase_CheckedChanged(object sender, EventArgs e)
        {
            RegexChanged();
        }

        private void cbIgnoreWhiteSpace_CheckedChanged(object sender, EventArgs e)
        {
            RegexChanged();
        }

        private void cbMultiLine_CheckedChanged(object sender, EventArgs e)
        {
            RegexChanged();
        }

        private void cbRightToLeft_CheckedChanged(object sender, EventArgs e)
        {
            RegexChanged();
        }

        private void cbSingleLine_CheckedChanged(object sender, EventArgs e)
        {
            RegexChanged();
        }

        private void cbWithSchema_CheckedChanged(object sender, EventArgs e)
        {
            RegexChanged();
        }
    }
}