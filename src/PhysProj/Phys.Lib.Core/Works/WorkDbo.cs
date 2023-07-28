﻿namespace Phys.Lib.Core.Works
{
    public class WorkDbo
    {
        public string Id { get; set; }

        /// <summary>
        /// Url and file path representative code of work
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Published date
        /// </summary>
        public string Publish { get; set; }

        /// <summary>
        /// Language the work is written on
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Translatable information about work
        /// </summary>
        public List<InfoDbo> Infos { get; set; } = new List<InfoDbo>();

        /// <summary>
        /// Collection of sub works in collected work
        /// </summary>
        public List<string> SubWorksCodes { get; set; } = new List<string>();

        /// <summary>
        /// Collection of authors of work
        /// </summary>
        public List<string> AuthorsCodes { get; set; } = new List<string>();

        /// <summary>
        /// Original work translated in this work
        /// </summary>
        public string? OriginalCode { get; set; }

        public List<string> FilesCodes { get; set; } = new List<string>();

        public class InfoDbo
        {
            public string Language { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public override string? ToString()
            {
                return $"{Language} ({Name})";
            }
        }

        public override string ToString()
        {
            return $"{Code} ({Id})";
        }

        public static readonly WorkDbo None = new WorkDbo();
    }
}
