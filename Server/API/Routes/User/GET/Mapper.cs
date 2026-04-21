using Server.API.Models;

namespace Server.API.Routes.User.GET;

public static class GetUserMapper
{
    public static GetUsersResponse ToResponse(List<UserModel> users)
    {
        return new GetUsersResponse
        {
            Users = users.Select(u => new UserDTO
            {
                UserId = u.Id,
                Name = u.Name,
                Email = u.Email,
                RoleName = u.Role.Name
            }).ToList()
        };
    }
}