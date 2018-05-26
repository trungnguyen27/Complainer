using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplainerProd.Models
{
    public class Voting
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "userID")]
        public string UserID { get; set; }

        [JsonProperty(PropertyName = "feedbackID")]
        public string FeedbackID { get; set; }

        [JsonProperty(PropertyName = "voteStatus")]
        public int VoteStatus { get; set; }
    }
}
