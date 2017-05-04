using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PodoDemo.Models.InnerModels
{
    /// <summary>
    /// 드롭다운 리스트 조건 검색 정보
    /// </summary>
    public class DDL
    {
        /// <summary>
        /// 옵션 마스터 고유 번호
        /// </summary>
        public string SearchKey { get; set; }
        /// <summary>
        /// 사용자 아이디
        /// </summary>
        public string Userid { get; set; }
        /// <summary>
        /// 옵션 마스터 값
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 옵션 마스터 텍스트
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 기본 값 설정 여부
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
