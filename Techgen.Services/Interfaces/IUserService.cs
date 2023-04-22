using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Models.Enum;
using Techgen.Models.Enums;
using Techgen.Models.RequestModels;
using Techgen.Models.RequestModels.Base.CursorPagination;
using Techgen.Models.ResponseModels;
using Techgen.Models.ResponseModels.Base.CursorPagination;

namespace Techgen.Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="model"></param>
        /// <param name="getAdmins"></param>
        /// <returns></returns>
        PaginationResponseModel<UserTableRowResponseModel> GetAll(PaginationRequestModel<UserTableColumn> model, bool getAdmins = false);

        /// <summary>
        /// Get all users
        /// </summary>
        /// <param name="model"></param>
        /// <param name="getAdmins"></param>
        /// <returns></returns>
        CursorPaginationBaseResponseModel<UserTableRowResponseModel> GetAll(CursorPaginationRequestModel<UserTableColumn> model, bool getAdmins = false);

        /// <summary>
        /// Soft delete user (leave in db)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserResponseModel SoftDeleteUser(int id);

        /// <summary>
        /// Hard delete user (delete from db)
        /// </summary>
        /// <param name="id"></param>
        Task HardDeleteUser(int id);
    }
}
