using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalEshop.Application.DTO;

namespace MinimalEshop.Application.Interface
{
    public interface IUser
    {
        Task<RegisterResponseDto> RegisterUser(RegisterRequestDto registerRequestDto);
    }
}
