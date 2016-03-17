using System.Collections.Generic;
using Newtonsoft.Json;

namespace MusicStoreES.Models
{
    public partial class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public List<Album> Albums { get; set; }
    }
}
