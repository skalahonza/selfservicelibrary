using CsvHelper.Configuration.Attributes;

namespace SelfServiceLibrary.CSV
{
    public class BookCSV
    {
        [Name("Nazev")]
        public string? Name { get; set; }
        [Name("Autor")]
        public string? Author { get; set; }
        /// <summary>
        /// Coma separated value of coauthors
        /// </summary>
        [Name("Spoluautori")]
        public string? CoAuthors { get; set; }
        [Name("DruhPublikace")]
        public string? PublicationType { get; set; }
        [Name("Deponovano")]
        public string? Storage { get; set; }
        [Name("SystemoveCislo")]
        public string? SystemNumber { get; set; }
        [Name("EvidencniCisloFEL")]
        public string? FelNumber { get; set; }
        [Name("EvidencniCisloOddeleni")]
        public string? DepartmentNumber { get; set; }
        [Name("CarovyKod")]
        public string? BarCode { get; set; }
        [Name("PocetStran")]
        public int? Pages { get; set; }
        [Name("Vydani")]
        public string? Publication { get; set; }
        [Name("RokVydani")]
        public int? YearOfPublication { get; set; }
        [Name("Vydavatel")]
        public string? Publisher { get; set; }
        [Name("ZemeVydani")]
        public string? CountryOfPublication { get; set; }
        [Name("ISBNorISSN")]
        public string? ISBNorISSN { get; set; }
        [Name("CisloCasopisu")]
        public string? MagazineNumber { get; set; }
        [Name("RocnikCasopisu")]
        public int? MagazineYear { get; set; }
        [Name("Konference")]
        public string? Conference { get; set; }
        [Name("Cena")]
        public string? Price { get; set; }
        /// <summary>
        /// Coma separated value of keywords
        /// </summary>
        [Name("KlicovaSlova")]
        public string? Keywords { get; set; }
        [Name("Poznamka")]
        public string? Note { get; set; }
        [Name("FormType")]
        public string? FormType { get; set; }
        [Name("IntStatus")]
        public string? IntStatus { get; set; }
        [Name("StsCheck")]
        public string? StsLocal { get; set; }
        [Name("StsUK")]
        public string? StsUK { get; set; }
        [Name("NfcCode")]
        public string? NFCIdent { get; set; }
        [Name("QRCode")]
        public string? QRIdent { get; set; }
    }
}
