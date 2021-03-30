using System;

namespace SelfServiceLibrary.DAL.Entities
{
    public class Issue
    {
        /// <summary>
        /// Id v databázi
        /// </summary>
#pragma warning disable CS8618 // Id cannot be null
        public string Id { get; set; }
#pragma warning restore CS8618 // Id cannot be null

        /// <summary>
        /// Evidenční-Číslo-Oddělení – Unikátní Číslo přiřazené naší katedrou (v současnosti GL-XXXXX, CMP-XXXXX)
        /// </summary>
        public string? DepartmentNumber { get; set; }

        /// <summary>
        /// Název knihy
        /// </summary>
        public string? BookName { get; set; }

        /// <summary>
        /// Uživatelské jméno toho, komu byla kniha půjčena
        /// </summary>
        public UserInfo IssuedTo { get; set; } = new UserInfo();

        /// <summary>
        /// Datum zapůjčení
        /// </summary>
        public DateTime? IssueDate { get; set; }

        /// <summary>
        /// Předpokládané datum vrácení
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Datum vrácení
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        public bool IsReturned { get; set; }
    }
}
