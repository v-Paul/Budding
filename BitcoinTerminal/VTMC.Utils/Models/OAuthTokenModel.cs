using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTMC.Utils.Models
{
    /// <summary>
    /// Token/RefreshToken Model
    /// </summary>
    public class OAuthTokenModel
    {
        /// <summary>
        /// 过期时间
        /// </summary>
        private DateTime expire;

        /// <summary>
        /// AccessToken
        /// </summary>
        private string accessToken;

        /// <summary>
        /// RefreshToken
        /// </summary>
        private string refreshToken;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="accessToken">AccessToken</param>
        /// <param name="refreshToken">RefreshToken</param>
        /// <param name="expire">Expire</param>
        public OAuthTokenModel(string accessToken, string refreshToken, DateTime expire)
        {
            this.accessToken = accessToken;
            this.refreshToken = refreshToken;
            this.expire = expire;
        }

        /// <summary>
        /// AccessToken
        /// </summary>
        public string AccessToken
        {
            get
            {
                return accessToken;
            }
        }

        /// <summary>
        /// RefreshToken
        /// </summary>
        public string RefreshToken
        {
            get
            {
                return refreshToken;
            }
        }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime Expire
        {
            get
            {
                return expire;
            }
        }

        /// <summary>
        /// 是否过期
        /// </summary>
        public bool IsExpired
        {
            get
            {
                var utcNow = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
                return utcNow > Expire;
            }
        }
    }
}
