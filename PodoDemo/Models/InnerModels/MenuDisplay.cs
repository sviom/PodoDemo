using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PodoDemo.Models.InnerModels
{
    /// <summary>
    /// 메뉴 표시용
    /// </summary>
    public class MenuDisplay
    {
        /// <summary>
        /// 대메뉴 아이디
        /// </summary>
        public long ParentMenuId { get; set; }
        /// <summary>
        /// 대메뉴 이름
        /// </summary>
        public string ParentMenuName { get; set; }        
        /// <summary>
        /// 소메뉴 아이디
        /// </summary>
        public string SubMenuId { get; set; }
        /// <summary>
        /// 소메뉴 이름
        /// </summary>
        public string SubMenuName { get; set; }
        /// <summary>
        /// 메뉴 URL
        /// </summary>
        public string MenuUrl { get; set; }
        /// <summary>
        /// 메뉴 순서
        /// </summary>
        public long MenuOrder { get; set; }
        /// <summary>
        /// 관리자용 메뉴 여부
        /// </summary>
        public bool IsManager { get; set; }
    }
}
