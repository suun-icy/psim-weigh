using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pism_weigh
{

    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public string email { get ; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isActive { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 超级管理员
        /// </summary>
        public string roleDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string roleId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string roleName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uuid { get; set; }
    }

    public class UserInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Data data { get; set; }
        /// <summary>
        /// 操作成功
        /// </summary>
        public string msg { get; set; }
    }
    

}
