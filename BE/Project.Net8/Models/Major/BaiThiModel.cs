using DTC.DefaultRepository.Constants;
using DTC.DefaultRepository.Models.Base;
using DTC.T;
using MongoDB.Bson.Serialization.Attributes;
using Project.Net8.Models.Core;
using System.Runtime.CompilerServices;
using CoreModel = DTC.DefaultRepository.Models.Core.CoreModel;

namespace Project.Net8.Models.Major;

public class BaiThiModel: Audit, TEntity<string>
{
   
    #region bai 5
    public string HoLot { get; set; }
    public string Ten { get; set; }// có thể bỏ vì có trường name trong audit rồi 
    public string TaiKhoan { get; set; }
    public string DiaChiKhachHang { get; set; }
    public DateTime? NgayThamGia { get; set; }

    [BsonIgnore]
    public string NgayThamGiaShow// lấy ra này nè nghen 
    {
        get { return NgayThamGia.HasValue ? NgayThamGia.Value.ToLocalTime().ToString(FormatTime.FORMAT_DATE_CORE) : ""; }
    }
    public int DiemSo { get; set; }

    #endregion 


}



