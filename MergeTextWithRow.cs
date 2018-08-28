using System;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Data;

namespace Honoka.SmallArticle.Activities
{
    /// <summary>
    /// Merge Textfile with Row Values
    /// </summary>
    [DisplayName("Merge Text File With Row Values")]
    public class MergeTextWithRow : CodeActivity
    {

        /// <summary>
        /// Template Text file name To merge with DataRow
        /// </summary>
        [Category("Input")]
        [Description("Template text filename.")]
        [RequiredArgument]
        public InArgument<String> TemplateFile { get; set; }

        /// <summary>
        /// Sets DataRow
        /// </summary>
        [Category("Input")]
        [Description("Data row as source")]
        [RequiredArgument]
        public InArgument<DataRow> Row { get; set; }

        /// <summary>
        /// Text Encode for Reading File.
        /// </summary>
        [Description("Read Template File With UTF-8")]
        [Category("Encoding (Choice only one)")]
        public InArgument<bool> UTF8 { get; set; } = true;

        [Description("Read Template File With Unicode(UTF-16)")]
        [Category("Encoding (Choice only one)")]
        public InArgument<bool> Unicode { get; set; } = false;

        [Description("Read Template File With Shift-JIS : this is option for Japanease Locale")]
        [Category("Encoding (Choice only one)")]
        public InArgument<bool> Shift_JIS { get; set; } = false;


        /// <summary>
        /// Check %%%_ColumnName_%%% Styled value exists or not after converting.
        /// </summary>
        [Description("Checks %%%_ColumnName_%%% Styled value exists or not, after converting.")]
        [Category("Options")]
        public InArgument<bool> CheckEscapeStringRemining { get; set; } = true;

        /// <summary>
        /// Returns String with replaced by Row Data 
        /// </summary>
        [Description("Replaced string, replaced %%%_ColumnName_%%% to row values")]
        [Category("Output")]
        public OutArgument<string> Result { get; set;}
       
        /// <summary>
        /// Execute Activity
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(CodeActivityContext context)
        {
            var row = Row.Get(context);
            var sourceText = string.Empty;

            if ((UTF8.Get(context) ? 1 : 0)
                + (Unicode.Get(context)? 1 : 0)
                + (Shift_JIS.Get(context) ? 1 : 0) != 1) throw new ArgumentException("Invalid choice at Encoding");

            Encoding enc;
            if(Shift_JIS.Get(context))
            {
                enc = Encoding.GetEncoding(932);
            }
            else if(Unicode.Get(context))
            {
                enc = Encoding.Unicode;
            }
            else
            {
                enc = Encoding.UTF8;
            }

            using (var reader = new System.IO.StreamReader(TemplateFile.Get(context), enc))
            {
                sourceText = reader.ReadToEnd();
            }

            foreach(DataColumn c in row.Table.Columns)
            {
                sourceText = sourceText.Replace("%%%_" + c.ColumnName + "_%%%", row[c].ToString());
            }

            if(CheckEscapeStringRemining.Get(context))
            {
                var r = new System.Text.RegularExpressions.Regex("^.*%%%_.*_%%%.*$");
                if (r.IsMatch(sourceText)) throw new ApplicationException("Escape Characters is remining.");
            }

            Result.Set(context, sourceText);
        }
    }
}
