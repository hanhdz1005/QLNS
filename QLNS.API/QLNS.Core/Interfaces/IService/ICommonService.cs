using QLNSCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNS.Core.Interfaces.IService
{
    public interface ICommonService<T, TDto, TAddDto> where T : BaseEntity<Guid>
    {
        Task<IEnumerable<TDto>> GetAll();
        Task<IEnumerable<TDto>> GetFilter();
        Task<TDto> GetById(Guid id);
        Task<TAddDto> Add(TDto dto);
        Task<TDto> Update(Guid id, TDto dto);
        Task<bool> DeleteById(Guid id);
    }
}
