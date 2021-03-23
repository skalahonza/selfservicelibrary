using System;
using System.Collections.Generic;

using MongoDB.Bson.Serialization.Attributes;

namespace SelfServiceLibrary.DAL.Entities
{
    public class Book
    {
        /// <summary>
        /// Název knihy
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// První / hlavní autor, u sborníku většinou edito
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Seznam dalších autorů
        /// </summary>
        public List<string> CoAuthors { get; set; } = new List<string>();

        /// <summary>
        /// Druh-Publikace – Typ publikace podle knihovny (GL, CMP, …)
        /// </summary>
        public string? PublicationType { get; set; }

        /// <summary>
        /// Deponováno – Fyzická část knihovny, kde je publikace uložena (zatím GL, CMP)
        /// </summary>
        public string? Depended { get; set; }

        /// <summary>
        /// Číslo přiřazované ústřední knihovnou ČVUT (určuje typ knihy, stejné knihy mají stejné číslo)
        /// </summary>
        public string? SystemNumber { get; set; }

        /// <summary>
        /// Evidenční-Číslo-FEL – Číslo přiřazované ústřední knihovnou ČVUT (pořadové číslo knihy evidované v centrální knihovně, mělo by být unikátní, raději asi jako text)
        /// </summary>
        public string? FelNumber { get; set; }

        /// <summary>
        /// Evidenční-Číslo-Oddělení – Unikátní Číslo přiřazené naší katedrou (v současnosti GL-XXXXX, CMP-XXXXX)
        /// </summary>
#pragma warning disable CS8618 // Database id cannot be empty
        [BsonId]
        public string DepartmentNumber { get; set; }
#pragma warning restore CS8618 // Database id cannot be empty

        /// <summary>
        /// Čárový-Kód – Číslo z čárového kódu nalepeného na začátku knihy
        /// </summary>
        public string? BarCode { get; set; }

        /// <summary>
        /// Rok-Vydání – Rok vydání publikace
        /// </summary>
        public int? YearOfPublication { get; set; }

        /// <summary>
        /// Pořadové číslo vydání (verze)
        /// </summary>
        public int? Publication { get; set; }

        /// <summary>
        /// Vydavatel – Kdo publikaci vydal (nakladatel)
        /// </summary>
        public string? Publisher { get; set; }

        /// <summary>
        /// Země-Vydání – Stát kde byla publikace vydána (asi pouze pro hledání, moc se nepoužívá)
        /// </summary>
        public string? CountryOfPublication { get; set; }

        /// <summary>
        /// ISBNorISSN – Podle toho co publikace obsahuje (pokud jej obsahuje)
        /// </summary>
        public string? ISBNorISSN { get; set; }

        /// <summary>
        /// Počet-Stran – Počet stran (ne u časopisů)
        /// </summary>
        public int? Pages { get; set; }

        /// <summary>
        /// Číslo časopisu
        /// </summary>
        public string? MagazineNumber { get; set; }

        /// <summary>
        /// Ročník časopisu
        /// </summary>
        public int? MagazineYear { get; set; }

        /// <summary>
        /// Konference – Z jaké konference je sborník (asi pouze pro sborník)
        /// </summary>
        public string? Conference { get; set; }

        /// <summary>
        /// Cena – Cena publikace (pokud je uvedena) v CZK
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Klíčová-Slova – Klíčová charakterizující publikaci
        /// </summary>
        public List<string> Keywords { get; set; } = new List<string>();

        /// <summary>
        /// Poznámka
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Vloženo – Kdy byl záznam o publikaci vložen (doplnit automaticky při vložení)
        /// </summary>
        public DateTime? Entered { get; set; }

        /// <summary>
        /// Určuje zda je kniha k zapůjčení
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Stav knihy
        /// </summary>
        public BookStatus Status { get; set; } = new BookStatus();

        /// <summary>
        /// Fyzický stav nebo podoba knihy
        /// </summary>
        public string? FormType { get; set; }

        /// <summary>
        /// Evidováno
        /// </summary>
        public bool StsLocal { get; set; }

        /// <summary>
        /// Evidováno ústřední knihovnou
        /// </summary>
        public bool StsUK { get; set; }

        /// <summary>
        /// Sériové / výrobní číslo NFC tagu na publikaci
        /// </summary>
        public string? NFCIdent { get; set; }

        /// <summary>
        /// Číslo uvedené na QR kódu (?)
        /// </summary>
        public string? QRIdent { get; set; }

        /// <summary>
        /// Full text search match score
        /// </summary>
        [BsonIgnoreIfNull]
        public double? TextMatchScore { get; set; }

        #region Related entities
        public Issue? CurrentIssue { get; set; }

        public List<string> IssueIds { get; set; } = new List<string>();
        #endregion
    }
}
