
using System.Collections.Generic;
namespace Melomash
{
    class json_input
    {
        public List<category> list { get; set; }
        public string error { get; set; }
    }
    class get_levels_in_category
    {
        public List<Server_Level> list { get; set; }
        public string error { get; set; }
    }
    class category
    {
        public string name { get; set; }
        public string query { get; set; }
        public string rutext { get; set; }
        public string entext { get; set; }
    }
    class Server_Level
    {
        public string name { get; set; }
        public string ident { get; set; }
        public string desc { get; set; }
        public string locale { get; set; }
        public string stars { get; set; }
        public string tracks_count { get; set; }
        public string coast { get; set; }
    }
    public class Level
    {
        public string ident { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string tracks_count { get; set; }
        public string locale { get; set; }
        public List<Artist> artists { get; set; }
    }
    public class Artist
    {
        public string word1 { get; set; }
        public string word2 { get; set; }
        public string word3 { get; set; }
        public string song1 { get; set; }
        public string song2 { get; set; }
        public string song3 { get; set; }
        public string answerFormat { get; set; }
    }
}
