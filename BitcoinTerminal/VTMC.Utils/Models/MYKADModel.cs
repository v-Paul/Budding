/*************************************************
*Author:fan danpeng
*Date:2017/04/27 18:29:02
*Des:Mykad工具类
************************************************/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace VTMC.Utils.Models
{
    /// <summary>
    /// 大马身份证信息
    /// </summary>
    public class MYKADInfo
    {
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDNum { get; set; }
        public string KPTName { get; set; }
        public string GMPCName { get; set; }
        public string OldIDNum { get; set; }
        public string Race { get; set; }
        public string Religion { get; set; }
        public string Address { get; set; }
        public string BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string Citizenship { get; set; }
        public string Gender { get; set; }
        public string DateIssued { get; set; }
        public string CardVer { get; set; }
        public string RegistrationKey { get; set; }
        public string ThumbLeft { get; set; }
        public string ThumbRight { get; set; }
        public string SocsoNum { get; set; }
        public string Locality { get; set; }
        public string GreenCardExpiry { get; set; }
        public string GreenCardNationality { get; set; }
        public string HeadPhoto { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HeadPhotoBase64 { get; set; }

    }

  
}