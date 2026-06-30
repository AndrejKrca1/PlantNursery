using System;

namespace PlantNursery.Core
{

    public enum PlantType
    {
        Sobna,
        Vrtna,
        Sukulenti,
        Jestiva,
        Zacinska
    }

    public enum CareType
    {
        Zalijevanje,
        Gnojenje,
        Presadivanje
    }

    public struct CareRecord
    {
        public CareType Type { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }

        public CareRecord(CareType type, DateTime date, string note)
        {
            Type = type;
            Date = date;
            Note = note;
        }

        public override string ToString()

            => $"{Date:dd.MM.yyyy} - {Type}: {Note}";
    }

    public class InvalidCareFrequencyException : Exception
    {
        public InvalidCareFrequencyException(string message) : base(message) { }
    }
}
