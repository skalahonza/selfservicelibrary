using System.Threading.Tasks;

using SelfServiceLibrary.BL.DTO.User;

namespace SelfServiceLibrary.BL.Interfaces
{
    public interface IAuthorizationContext
    {
        /*
        Půjčit si / vrátit na KIOSKu
        Půjčit / vrátit si online

        Půjčit / vrátit někomu
        Editovat / přidávat / mazat záznamy
        Umožnit samooblužný provoz
        Spravovat knihovníky
         * */

        Task<bool> CanBorrow();
        Task<bool> CanBorrowTo();
        Task<bool> CanReturnFor();
        Task<bool> CanManageBooks();
        Task<bool> CanGrantSelfService();
        Task<bool> CanManageLibrarians();
        Task<UserInfoDTO?> GetUserInfo();
    }
}
