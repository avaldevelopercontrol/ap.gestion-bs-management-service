using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_DocxCobrarAdicional
    {
        public int nid_docxcobrarAd { get; set; }
        
        public int? nId_DocxCobrar { get; set; }
        public int? nId_Cartera { get; set; }
        public int? nId_Cliente { get; set; }

        public virtual av_Cartera av_Cartera { get; set; }
        public virtual av_DocxCobrar av_DocxCobrar { get; set; }
        public virtual av_Cliente av_Cliente { get; set; }

        public string? adParam01 { get; set; }
        public string? adParam02 { get; set; }
        public string? adParam03 { get; set; }
        public string? adParam04 { get; set; }
        public string? adParam05 { get; set; }
        public int? nid_persdeudor { get; set; }
        public DateTime? dFecRegistro { get; set; }
        public string? adParam06 { get; set; }
        public string? adParam07 { get; set; }
        public string? adParam08 { get; set; }
        public string? adParam09 { get; set; }
        public string? adParam10 { get; set; }
        public string? adParam11 { get; set; }
        public string? adParam12 { get; set; }
        public string? adParam13 { get; set; }
        public string? adParam14 { get; set; }
        public string? adParam15 { get; set; }
    }
}