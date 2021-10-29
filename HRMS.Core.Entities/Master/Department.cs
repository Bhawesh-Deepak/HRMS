using HRMS.Core.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Core.Entities.Master
{
    [Table("Department", Schema = "Master")]
    public class Department: BaseModel<int>
    {
        [Required(ErrorMessage ="Department name is required.")]
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
