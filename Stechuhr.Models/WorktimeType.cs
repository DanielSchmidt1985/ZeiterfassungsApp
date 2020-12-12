namespace Stechuhr.Models
{
    public enum WorktimeType
    {
        /// <summary>Regular</summary>
        R = 1,
        /// <summary>Urlaub</summary>
        U = 2,
        /// <summary>Urlaub 0,5 Tage</summary>
        UH = 4,
        /// <summary>Krank</summary>
        K = 8,
        /// <summary>Kurzarbeit</summary>
        KA = 16,
        /// <summary>Kurzarbeit 0,5 Tage</summary>
        KAH = 32,
        F = 64,
    }


}

