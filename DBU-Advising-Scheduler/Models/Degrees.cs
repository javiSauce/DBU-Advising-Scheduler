using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;


namespace DBU_Advising_Scheduler.Models
{
    public class Degrees
    {
        [JsonProperty(PropertyName = "College of Business")]
        public List<string> CollegeOfBusiness { get; set; }

        [JsonProperty(PropertyName = "College of Christian Faith")]
        public List<string> CollegeOfChristianFaith { get; set; }

        [JsonProperty(PropertyName = "College of Education")]
        public List<string> CollegeOfEducation { get; set; }

        [JsonProperty(PropertyName = "College of Fine Arts")]
        public List<string> CollegeOfFineArts { get; set; }

        [JsonProperty(PropertyName = "College of Humanities and Social Sciences")]
        public List<string> CollegeOfHumanitiesAndSS { get; set; }

        [JsonProperty(PropertyName = "College of Natural Sciences and Mathematics")]
        public List<string> CollegeOfNaturalSciences { get; set; }

        [JsonProperty(PropertyName = "School of Leadership")]
        public List<string> SchoolOfLeadership { get; set; }

        [JsonProperty(PropertyName = "College of Business and Graduate School of Business")]
        public List<string> GradSchoolOfBusiness { get; set; }

        [JsonProperty(PropertyName = "College of Christian Faith and Graduate School of Ministry")]
        public List<string> GraduateSchoolOfMinistry { get; set; }

        [JsonProperty(PropertyName = "College of Humanities")]
        public List<string> CollegeOfHumanities { get; set; }

        [JsonProperty(PropertyName = "College of Natural Science and Mathematics Masters")]
        public List<string> MastersOfNaturalScience { get; set; }

        [JsonProperty(PropertyName = "Cook School of Leadership")]
        public List<string> MastersSchoolOfLeadership { get; set; }
    }
}